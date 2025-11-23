using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using CareerUp.Data;
using CareerUp.Repositories;
using CareerUp.Repositories.Interfaces;
using CareerUp.Services;
using CareerUp.Services.Interfaces;
using CareerUp.Models.ML;
using Microsoft.ML;
using CareerUp.Observability;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configuração do OpenTelemetry com Jaeger
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("CareerUp.Api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddSource(Tracing.GetActivitySource().Name)
            .AddAspNetCoreInstrumentation()
            .AddEntityFrameworkCoreInstrumentation(options =>
            {
                options.SetDbStatementForText = true;
                options.SetDbStatementForStoredProcedure = true;
            })
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:4318/v1/traces");
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            });
    });

// Add services to the container.

// Configuração do DbContext Oracle
builder.Services.AddDbContext<OracleDbContext>(options =>
{
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection"));
});

// Configuração de Repositories (Scoped)
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ILoginUsuarioRepository, LoginUsuarioRepository>();
builder.Services.AddScoped<IHabilidadeRepository, HabilidadeRepository>();
builder.Services.AddScoped<IRecomendacaoRepository, RecomendacaoRepository>();

// Configuração do ML.NET - Carrega modelo treinado (Singleton)
var modelPath = Path.Combine(AppContext.BaseDirectory, "MLModel", "CareerModel.zip");
if (!File.Exists(modelPath))
{
    throw new FileNotFoundException($"Modelo ML.NET não encontrado: {modelPath}");
}

var mlContext = new MLContext(seed: 42);
ITransformer mlModel = mlContext.Model.Load(modelPath, out var modelSchema);

// Criar PredictionEngine normalmente (agora CareerInput tem a coluna Recomendacao)
var predictionEngine = mlContext.Model.CreatePredictionEngine<CareerInput, CareerPrediction>(mlModel);

builder.Services.AddSingleton(predictionEngine);

// Configuração de Services (Scoped)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IMLPredictionService, MLPredictionService>();
builder.Services.AddScoped<IRecomendacaoService, RecomendacaoService>();

// Configuração de Autenticação JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key não configurada");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer não configurado");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience não configurado");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Configuração de Health Checks
builder.Services.AddHealthChecks()
    .AddOracle(
        builder.Configuration.GetConnectionString("OracleConnection")!,
        name: "oracle-database",
        tags: new[] { "db", "oracle" }
    );

builder.Services.AddControllers();

// Configuração de API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Configuração do Swagger com JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CareerUp API",
        Version = "v1",
        Description = "API REST para sistema de recomendação de carreira com IA",
        Contact = new OpenApiContact
        {
            Name = "CareerUp Team",
            Email = "contact@careerup.com"
        }
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "CareerUp API",
        Version = "v2",
        Description = "API REST para sistema de recomendação de carreira com IA - v2 com filtro por mês",
        Contact = new OpenApiContact
        {
            Name = "CareerUp Team",
            Email = "contact@careerup.com"
        }
    });

    // Configuração de autenticação JWT no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Habilita comentários XML para documentação
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerUp API v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "CareerUp API v2");
        options.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseHttpsRedirection();

// Middleware de autenticação e autorização (ordem importa!)
app.UseAuthentication();
app.UseAuthorization();

// Health Check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.Run();
