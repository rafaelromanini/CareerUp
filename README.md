# 🎓 CareerUp API  

API RESTful desenvolvida para o sistema de recomendação de carreira utilizando IA (ML.NET) com ASP.NET Core e Oracle Database. Implementa autenticação JWT, HATEOAS, tracing distribuído com OpenTelemetry/Jaeger e health checks.

---

# 👥 **Integrantes**
- **Vinicius Leandro de Araujo Bernardes** - RM554728 - TURMA 2TDSPY
- **Edvan Davi Murilo Santos do Nascimento** - RM554733 - TURMA 2TDSPZ  
- **Rafael Romanini de Oliveira** - RM554637 - TURMA 2TDSPZ

---

## 🏗️ **Justificativa da Arquitetura**

### **Domínio Escolhido: Sistema de Recomendação de Carreira com IA**
A escolha do domínio de recomendação de carreira se justifica pela complexidade adequada para demonstrar integração com Machine Learning, autenticação segura e regras de negócio específicas:

#### **Entidades Principais:**
1. **Usuario** - Representa os usuários do sistema com papéis (USUARIO/GERENTE)
2. **Habilidade** - Três habilidades principais do usuário
3. **LoginUsuario** - Credenciais de autenticação
4. **Recomendacao** - Sugestões de carreira geradas pela IA

#### **Arquitetura Técnica:**
- **ASP.NET Core Web API** - Framework robusto com alta performance
- **Entity Framework Core** - ORM com suporte completo ao Oracle
- **Oracle Database** - Banco empresarial com alta confiabilidade
- **Padrão Repository + Service** - Separação clara de responsabilidades
- **DTOs** para contratos de API
- **JWT Bearer** para autenticação stateless
- **ML.NET** para recomendações de carreira
- **OpenTelemetry + Jaeger** para tracing distribuído
- **Swagger/OpenAPI** para documentação automática

#### **Justificativas das Escolhas:**
- **Arquitetura em Camadas** (Controllers → Services → Repositories → Data)
- **SOLID** e **Clean Architecture** aplicados
- **Autenticação JWT** com BCrypt para senhas
- **HATEOAS** para navegabilidade da API
- **Paginação** para performance em grandes volumes
- **Observabilidade** com tracing e health checks
- **Validações robustas** com Data Annotations

### **Regras de Negócio Implementadas:**
1. **CPF, Email e Login devem ser únicos**
2. **Usuário comum só pode atualizar suas próprias habilidades**
3. **Apenas gerentes podem alterar cargo e excluir usuários**
4. **Exclusão em cascata** (Usuario → Login, Habilidades, Recomendações)
5. **Senha mínima de 6 caracteres**, criptografada com BCrypt

---

## 🚀 **Instruções de Execução**

### **Pré-requisitos:**
- .NET 9.0 SDK
- Oracle Database (ou acesso ao oracle.fiap.com.br)
- Docker (para Jaeger - opcional)
- Visual Studio Code ou Visual Studio

### **1. Clone o Repositório:**
```bash
git clone https://github.com/rafaelromanini/CareerUp.git
cd CareerUp
```

### **2. Configure a String de Conexão:**
No arquivo `CareerUp/appsettings.json`, configure:
```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=RM554637;Password=SUA_SENHA;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle.fiap.com.br)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=ORCL)))"
  },
  "Jwt": {
    "Key": "CareerUp-Super-Secret-Key-2025-Minimum-32-Characters-Required!",
    "Issuer": "CareerUpAPI",
    "Audience": "CareerUpClient",
    "ExpirationMinutes": 120
  }
}
```

### **3. Instale as Dependências:**
```bash
cd CareerUp
dotnet restore
```

### **4. Execute as Migrations:**
```bash
dotnet ef database update
```

### **5. (Opcional) Inicie o Jaeger para Tracing:**
```bash
docker run -d --name jaeger \
  -p 4318:4318 \
  -p 16686:16686 \
  jaegertracing/all-in-one:latest
```

Acesse o Jaeger UI em: **http://localhost:16686**

### **6. Compile e Execute:**
```bash
dotnet build
dotnet run
```

### **7. Acesse a API:**
- **Swagger UI:** https://localhost:XXXX/ (porta exibida no console)
- **Health Check:** https://localhost:XXXX/health
- **Jaeger UI:** http://localhost:16686 (se iniciado)

---

## 🔍 **Observabilidade - Tracing Distribuído**

### **OpenTelemetry + Jaeger**

A aplicação implementa tracing distribuído completo para rastreamento de requisições:

#### **O que é rastreado:**
- ✅ **Requisições HTTP** (ASP.NET Core)
- ✅ **Queries SQL** (Entity Framework Core)
- ✅ **Chamadas HTTP externas** (HttpClient)
- ✅ **Operações customizadas** (via classe `Tracing`)

#### **Subir Jaeger com Docker:**
```bash
# Iniciar Jaeger (All-in-One)
docker run -d --name jaeger \
  -e COLLECTOR_OTLP_ENABLED=true \
  -p 4318:4318 \
  -p 16686:16686 \
  jaegertracing/all-in-one:latest

# Verificar se está rodando
docker ps | grep jaeger

# Logs do container
docker logs jaeger

# Parar Jaeger
docker stop jaeger

# Remover container
docker rm jaeger
```

#### **Acessar Jaeger UI:**
1. Abra o navegador em: **http://localhost:16686**
2. Selecione o serviço: **CareerUp.Api**
3. Clique em "Find Traces"
4. Explore os traces das requisições!

#### **Exemplo de Trace:**
```
POST /api/v1/auth/register
├── HTTP POST Request (200ms)
│   ├── SQL INSERT tb_usuario (45ms)
│   ├── SQL INSERT tb_login_usuario (12ms)
│   └── SQL INSERT tb_habilidade (8ms)
└── Response Sent
```

#### **Configuração do OpenTelemetry:**
```csharp
// Program.cs
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("CareerUp.Api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddSource(Tracing.GetActivitySource().Name)
            .AddAspNetCoreInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:4318/v1/traces");
            });
    });
```

#### **Criando Spans Customizados:**
```csharp
using CareerUp.Observability;

// Criar span customizado
using var activity = Tracing.StartActivity("ProcessarRecomendacao");
activity?.AddTag("usuario.id", usuarioId);
activity?.AddEvent("Iniciando processamento IA");

// ... lógica de negócio

activity?.AddEvent("Processamento concluído");
```

---

## 🏥 **Health Checks**

A API possui endpoint de health check para monitoramento:

### **Endpoint:**
```bash
GET /health
```

### **Exemplo de Resposta:**
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0123456",
  "entries": {
    "oracle-database": {
      "status": "Healthy",
      "duration": "00:00:00.0123456",
      "tags": ["db", "oracle"]
    }
  }
}
```

**Status possíveis:**
- `Healthy` - Sistema funcionando perfeitamente
- `Degraded` - Sistema funcionando com limitações
- `Unhealthy` - Sistema com problemas críticos

---

## 📖 **Exemplos de Uso dos Endpoints**

### **Autenticação (`/api/v1/auth`)**

#### **Registrar Novo Usuário:**
```bash
POST /api/v1/auth/register
Content-Type: application/json

{
  "nomeUsuario": "João Silva",
  "cpf": "12345678900",
  "email": "joao@example.com",
  "cargo": "Desenvolvedor",
  "papel": 0,
  "loginUsuario": {
    "login": "joaosilva",
    "senha": "senha123"
  },
  "habilidades": {
    "habilidadePrimaria": "C#",
    "habilidadeSecundaria": ".NET Core",
    "habilidadeTerciaria": "SQL"
  }
}
```

**Valores do campo `papel`:**
- `0` = USUARIO (usuário comum)
- `1` = GERENTE (gerente com permissões administrativas)

#### **Fazer Login:**
```bash
POST /api/v1/auth/login
Content-Type: application/json

{
  "login": "joaosilva",
  "senha": "senha123"
}
```

**Resposta:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-11-23T18:00:00Z",
  "usuario": {
    "idUsuario": 1,
    "nomeUsuario": "João Silva",
    "email": "joao@example.com",
    "cargo": "Desenvolvedor",
    "papel": "USUARIO",
    "login": "joaosilva",
    "habilidadePrimaria": "C#",
    "habilidadeSecundaria": ".NET Core",
    "habilidadeTerciaria": "SQL",
    "links": []
  }
}
```

### **Usuários (`/api/v1/usuarios` - Requer Autenticação)**

#### **Obter Dados do Usuário Autenticado:**
```bash
GET /api/v1/usuarios/me
Authorization: Bearer {token}
```

#### **Listar Todos os Usuários (Apenas GERENTE):**
```bash
GET /api/v1/usuarios?pageNumber=1&pageSize=5
Authorization: Bearer {token_gerente}
```

#### **Atualizar Cargo (Apenas GERENTE):**
```bash
PUT /api/v1/usuarios/1/cargo
Authorization: Bearer {token_gerente}
Content-Type: application/json

{
  "cargo": "Desenvolvedor Sênior"
}
```

#### **Atualizar Habilidades (Próprio Usuário):**
```bash
PUT /api/v1/usuarios/1/habilidades
Authorization: Bearer {token}
Content-Type: application/json

{
  "habilidadePrimaria": "C# Avançado",
  "habilidadeSecundaria": "Azure",
  "habilidadeTerciaria": "Docker"
}
```

#### **Excluir Usuário (Apenas GERENTE):**
```bash
DELETE /api/v1/usuarios/1
Authorization: Bearer {token_gerente}
```

### **Exemplo de Resposta com HATEOAS:**
```json
{
  "idUsuario": 1,
  "nomeUsuario": "João Silva",
  "cpf": "12345678900",
  "email": "joao@example.com",
  "cargo": "Desenvolvedor",
  "papel": "USUARIO",
  "login": "joaosilva",
  "habilidadePrimaria": "C#",
  "habilidadeSecundaria": ".NET Core",
  "habilidadeTerciaria": "SQL",
  "links": [
    {
      "rel": "self",
      "href": "https://localhost:7XXX/api/v1/usuarios/1",
      "method": "GET"
    },
    {
      "rel": "update-cargo",
      "href": "https://localhost:7XXX/api/v1/usuarios/1/cargo",
      "method": "PUT"
    },
    {
      "rel": "update-habilidades",
      "href": "https://localhost:7XXX/api/v1/usuarios/1/habilidades",
      "method": "PUT"
    },
    {
      "rel": "delete",
      "href": "https://localhost:7XXX/api/v1/usuarios/1",
      "method": "DELETE"
    }
  ]
}
```

### **Resposta Paginada com HATEOAS:**
```json
{
  "data": [
    { "idUsuario": 1, "nomeUsuario": "João Silva", ... },
    { "idUsuario": 2, "nomeUsuario": "Maria Santos", ... }
  ],
  "pageNumber": 1,
  "pageSize": 5,
  "totalPages": 3,
  "totalRecords": 15,
  "hasPrevious": false,
  "hasNext": true,
  "links": [
    {
      "rel": "self",
      "href": "https://localhost:7XXX/api/v1/usuarios?pageNumber=1&pageSize=5",
      "method": "GET"
    },
    {
      "rel": "first",
      "href": "https://localhost:7XXX/api/v1/usuarios?pageNumber=1&pageSize=5",
      "method": "GET"
    },
    {
      "rel": "next",
      "href": "https://localhost:7XXX/api/v1/usuarios?pageNumber=2&pageSize=5",
      "method": "GET"
    },
    {
      "rel": "last",
      "href": "https://localhost:7XXX/api/v1/usuarios?pageNumber=3&pageSize=5",
      "method": "GET"
    }
  ]
}
```

---

## 🏗️ **Estrutura do Projeto**

```
CareerUp/
├── CareerUp/                           # Projeto principal da API
│   ├── Controllers/                    # Controllers da API REST
│   │   ├── AuthController.cs          # Autenticação (login/registro)
│   │   └── UsuariosController.cs      # CRUD de usuários
│   ├── Data/                          # Contexto e Mappings EF Core
│   │   ├── Mappings/                  # Configurações Fluent API
│   │   │   ├── UsuarioMapping.cs
│   │   │   ├── HabilidadeMapping.cs
│   │   │   ├── LoginUsuarioMapping.cs
│   │   │   └── RecomendacaoMapping.cs
│   │   └── OracleDbContext.cs         # DbContext Oracle
│   ├── Helpers/                       # Classes utilitárias
│   │   └── HateoasLinks.cs           # Helper HATEOAS
│   ├── Migrations/                    # Migrations do EF Core
│   ├── Models/                        # Entidades do domínio
│   │   ├── Enums/
│   │   │   └── PapelUsuario.cs       # Enum de papéis
│   │   ├── DTOs/                     # Data Transfer Objects
│   │   │   ├── Auth/
│   │   │   ├── Usuario/
│   │   │   └── Common/
│   │   ├── Usuario.cs
│   │   ├── Habilidade.cs
│   │   ├── LoginUsuario.cs
│   │   └── Recomendacao.cs
│   ├── Observability/                 # Tracing e Observabilidade
│   │   └── Tracing.cs                # Helper de tracing
│   ├── Repositories/                  # Camada de acesso a dados
│   │   ├── Interfaces/
│   │   ├── UsuarioRepository.cs
│   │   ├── LoginUsuarioRepository.cs
│   │   └── HabilidadeRepository.cs
│   ├── Services/                      # Lógica de negócio
│   │   ├── Interfaces/
│   │   ├── AuthService.cs
│   │   └── UsuarioService.cs
│   ├── appsettings.json              # Configurações
│   └── Program.cs                    # Ponto de entrada
```

---

## 🔗 **Endpoints da API**

### **Autenticação (Público):**
| Método | Endpoint | Descrição |
|---------|----------|-----------|
| `POST` | `/api/v1/auth/register` | Registra um novo usuário |
| `POST` | `/api/v1/auth/login` | Autentica usuário e retorna token JWT |

### **Usuários (Requer Autenticação):**
| Método | Endpoint | Permissão | Descrição |
|---------|----------|-----------|-----------|
| `GET` | `/api/v1/usuarios` | GERENTE | Lista usuários com paginação |
| `GET` | `/api/v1/usuarios/me` | Todos | Dados do usuário autenticado |
| `GET` | `/api/v1/usuarios/{id}` | Próprio ou GERENTE | Busca por ID |
| `PUT` | `/api/v1/usuarios/{id}/cargo` | GERENTE | Atualiza cargo |
| `PUT` | `/api/v1/usuarios/{id}/habilidades` | Próprio | Atualiza habilidades |
| `DELETE` | `/api/v1/usuarios/{id}` | GERENTE | Remove usuário |

### **Health Checks (Público):**
| Método | Endpoint | Descrição |
|---------|----------|-----------|
| `GET` | `/health` | Verifica saúde da aplicação |

---

## 🔐 **Autenticação JWT**

### **Como Autenticar:**

1. **Registrar/Login** para obter token
2. **Copiar o `accessToken`** da resposta
3. **Adicionar header** em todas as requisições protegidas:
   ```
   Authorization: Bearer {accessToken}
   ```

### **No Swagger UI:**
1. Clique no botão **🔒 Authorize**
2. Digite: `Bearer {seu_token}`
3. Clique em **Authorize**
4. Teste os endpoints protegidos!

### **Configurações:**
- **Expiração:** 120 minutos (configurável)
- **Algoritmo:** HMAC SHA-256
- **Senhas:** BCrypt com work factor 12

---

## 🎯 **Papéis e Permissões**

### **USUARIO (valor: 0):**
- ✅ Ver seus próprios dados
- ✅ Atualizar suas habilidades
- ✅ Gerar recomendações (quando implementado)
- ❌ Ver outros usuários
- ❌ Alterar cargo
- ❌ Excluir usuários

### **GERENTE (valor: 1):**
- ✅ Todas as permissões de USUARIO
- ✅ Listar todos os usuários
- ✅ Ver dados de qualquer usuário
- ✅ Alterar cargo de qualquer usuário
- ✅ Excluir usuários

---

## 🚀 **Tecnologias Utilizadas**

### **Backend:**
- **ASP.NET Core 9.0** - Framework web moderno
- **Entity Framework Core 9.0** - ORM
- **Oracle.EntityFrameworkCore** - Provider Oracle

### **Autenticação:**
- **JWT Bearer** - Tokens stateless
- **BCrypt.Net** - Criptografia de senhas

### **Observabilidade:**
- **OpenTelemetry 1.10.x** - Tracing distribuído
- **Jaeger** - Visualização de traces
- **ILogger** - Logging estruturado
- **Health Checks** - Monitoramento

### **Documentação:**
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI

### **Machine Learning (Futuro):**
- **Microsoft.ML** - Framework ML.NET

---

## 📝 **Regras de Negócio**

1. ✅ **CPF, Email e Login únicos**
2. ✅ **Senha mínima de 6 caracteres**
3. ✅ **Senhas criptografadas com BCrypt**
4. ✅ **Token JWT expira em 120 minutos**
5. ✅ **Usuário comum só edita seus dados**
6. ✅ **Gerente tem acesso total**
7. ✅ **Exclusão em cascata** (Login, Habilidades, Recomendações)
8. ✅ **Paginação padrão:** 5 itens por página
9. ✅ **Ordenação alfabética** por nome de usuário

---

## ⚠️ **Códigos de Status HTTP**

- `200 OK` - Sucesso
- `201 Created` - Recurso criado
- `204 No Content` - Sucesso sem conteúdo
- `400 Bad Request` - Dados inválidos
- `401 Unauthorized` - Não autenticado
- `403 Forbidden` - Sem permissão
- `404 Not Found` - Recurso não encontrado
- `500 Internal Server Error` - Erro interno
