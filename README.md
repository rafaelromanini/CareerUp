# 🎓 CareerUp API  

API RESTful desenvolvida para o sistema de recomendação de carreira utilizando **IA com ML.NET** para gerar recomendações personalizadas de cursos e vagas baseadas no perfil profissional do usuário. Construída com **ASP.NET Core 9.0**, **Oracle Database**, **JWT Authentication**, **HATEOAS**, **OpenTelemetry/Jaeger** para tracing distribuído e **testes unitários** completos.

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
- **ASP.NET Core 9.0 Web API** - Framework robusto com alta performance
- **Entity Framework Core 9.0** - ORM com suporte completo ao Oracle
- **Oracle Database** - Banco empresarial com alta confiabilidade
- **ML.NET 3.0** - Machine Learning para recomendações de carreira (SDCA Maximum Entropy)
- **Padrão Repository + Service** - Separação clara de responsabilidades (Clean Architecture)
- **DTOs** para contratos de API seguros
- **JWT Bearer** para autenticação stateless e segura
- **BCrypt** para hash de senhas (work factor 12)
- **API Versioning** - Suporte a múltiplas versões da API (v1 e v2)
- **OpenTelemetry + Jaeger** para tracing distribuído e observabilidade
- **Swagger/OpenAPI 3.0** para documentação interativa
- **xUnit + Moq** para testes unitários com cobertura de regras de negócio

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
4. **Usuário comum vê apenas suas recomendações, Gerente vê todas**
5. **Exclusão em cascata** (Usuario → Login, Habilidades, Recomendações)
6. **Senha mínima de 6 caracteres**, criptografada com BCrypt (work factor 12)
7. **ML.NET gera recomendações** baseadas em: Cargo + 3 Habilidades
8. **Modelo treinado com 15 perfis de carreira** (100% de acurácia no dataset)
9. **Recomendações personalizadas** com cursos, vagas e plano de desenvolvimento

---

## 🚀 **Instruções de Execução**

### **Pré-requisitos:**
- .NET 9.0 SDK ([Download aqui](https://dotnet.microsoft.com/download/dotnet/9.0))
- Oracle Database (ou acesso ao oracle.fiap.com.br)
- Docker (para Jaeger - opcional)
- Visual Studio Code, Visual Studio ou Rider

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
- **Swagger UI:** https://localhost:5005 (ou porta exibida no console)
- **Health Check:** https://localhost:5005/health
- **Jaeger UI:** http://localhost:16686 (se iniciado)

### **8. (Opcional) Treinar Modelo ML.NET:**
Se quiser retreinar o modelo de Machine Learning:
```bash
cd CareerUp.Trainer
dotnet run
```
O modelo `CareerModel.zip` será gerado e copiado automaticamente para a API.

### **9. Executar Testes Unitários:**
```bash
dotnet test CareerUp.Tests/CareerUp.Tests.csproj --verbosity normal
```
**Resultado esperado:** 11 testes passando ✅

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

---

### **Recomendações (`/api/v1/recomendacoes` - Requer Autenticação)**

#### **Gerar Nova Recomendação com IA:**
```bash
POST /api/v1/recomendacoes/gerar
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "idRecomendacao": 1,
  "dataGeracao": "2025-11-23T15:30:00Z",
  "resultadoIa": "🎯 Recomendações Personalizadas para Desenvolvedor Backend:\n\n📚 CURSOS RECOMENDADOS:\n1. .NET Avançado e Microserviços\n2. Azure Cloud Architecture\n3. Docker e Kubernetes\n\n💼 VAGAS SUGERIDAS:\n- Desenvolvedor .NET Sênior\n- Arquiteto de Soluções Cloud\n\n🚀 PLANO DE DESENVOLVIMENTO:\n- Aprofundar conhecimentos em Azure\n- Certificação AZ-204\n- Praticar padrões de arquitetura",
  "idUsuario": 1,
  "nomeUsuario": "João Silva",
  "cargo": "Desenvolvedor Backend",
  "links": [...]
}
```

#### **Listar Minhas Recomendações:**
```bash
GET /api/v1/recomendacoes/minhas?pageNumber=1&pageSize=5
Authorization: Bearer {token}
```

#### **Buscar Recomendação por ID:**
```bash
GET /api/v1/recomendacoes/1
Authorization: Bearer {token}
```

#### **Listar Recomendações de Usuário Específico (Apenas GERENTE):**
```bash
GET /api/v1/recomendacoes/usuario/1?pageNumber=1&pageSize=5
Authorization: Bearer {token_gerente}
```

#### **Excluir Recomendação:**
```bash
DELETE /api/v1/recomendacoes/1
Authorization: Bearer {token}
```
*Gerente pode excluir qualquer recomendação, usuário comum só as próprias*

---

### **🆕 API v2 - Recomendações com Filtro por Mês (`/api/v2/recomendacoes`)**

A **versão 2** da API introduz **filtro por mês** nas recomendações, permitindo buscar apenas recomendações geradas em um mês específico.

#### **Por que usar a v2?**
- 📅 **Filtro por mês** - Busque recomendações de Janeiro (1), Fevereiro (2), etc.
- 📈 **Análise temporal** - Acompanhe evolução mês a mês
- ⚡ **Performance** - Menos dados retornados quando filtrado
- 🔄 **Retrocompatibilidade** - v1 continua funcionando normalmente

#### **Listar Minhas Recomendações (v2 - com filtro de mês):**

**Sem filtro (mesmo comportamento da v1):**
```bash
GET /api/v2/recomendacoes/minhas?pageNumber=1&pageSize=5
Authorization: Bearer {token}
```

**Com filtro por mês:**
```bash
# Recomendações de Janeiro (mês 1)
GET /api/v2/recomendacoes/minhas?mes=1&pageNumber=1&pageSize=5
Authorization: Bearer {token}

# Recomendações de Novembro (mês 11)
GET /api/v2/recomendacoes/minhas?mes=11&pageNumber=1&pageSize=5
Authorization: Bearer {token}
```

**Parâmetros:**
- `mes` (opcional) - Número do mês (1-12)
  - `1` = Janeiro
  - `2` = Fevereiro
  - `3` = Março
  - `4` = Abril
  - `5` = Maio
  - `6` = Junho
  - `7` = Julho
  - `8` = Agosto
  - `9` = Setembro
  - `10` = Outubro
  - `11` = Novembro
  - `12` = Dezembro
- `pageNumber` (opcional, padrão: 1) - Número da página
- `pageSize` (opcional, padrão: 5) - Tamanho da página

**Resposta:**
```json
{
  "data": [
    {
      "idRecomendacao": 5,
      "dataGeracao": "2025-11-15T10:30:00Z",
      "resultadoIa": "...",
      "idUsuario": 1,
      "nomeUsuario": "João Silva",
      "cargo": "Desenvolvedor Backend",
      "links": [...]
    },
    {
      "idRecomendacao": 4,
      "dataGeracao": "2025-11-10T14:20:00Z",
      "resultadoIa": "...",
      "idUsuario": 1,
      "nomeUsuario": "João Silva",
      "cargo": "Desenvolvedor Backend",
      "links": [...]
    }
  ],
  "pageNumber": 1,
  "pageSize": 5,
  "totalRecords": 2,
  "links": [...]
}
```

**Validações:**
- ❌ Mês inválido (< 1 ou > 12) retorna `400 Bad Request`
- ✅ Mês omitido retorna todas as recomendações (comportamento padrão)

#### **Exemplo de Uso - Análise Temporal:**

```bash
# Ver recomendações de Janeiro
curl -X GET "https://localhost:5005/api/v2/recomendacoes/minhas?mes=1" \
  -H "Authorization: Bearer {token}"

# Ver recomendações de Fevereiro
curl -X GET "https://localhost:5005/api/v2/recomendacoes/minhas?mes=2" \
  -H "Authorization: Bearer {token}"

# Ver todas as recomendações (sem filtro)
curl -X GET "https://localhost:5005/api/v2/recomendacoes/minhas" \
  -H "Authorization: Bearer {token}"
```

#### **Comparação v1 vs v2:**

| Feature | v1 | v2 |
|---------|----|----||
| Listar recomendações | ✅ | ✅ |
| Paginação | ✅ | ✅ |
| Filtro por mês | ❌ | ✅ |
| HATEOAS | ✅ | ✅ |
| Autorização | ✅ | ✅ |

**💡 Dica:** Use `/swagger` para testar ambas as versões interativamente!

---

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
│   │   ├── UsuariosController.cs      # CRUD de usuários
│   │   ├── RecomendacoesController.cs # Endpoints v1 de recomendações IA
│   │   └── RecomendacoesV2Controller.cs # Endpoints v2 (filtro por mês)
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
│   ├── MLModel/                       # Modelo ML.NET treinado
│   │   └── CareerModel.zip           # Modelo binário (33KB, 100% accuracy)
│   ├── Models/                        # Entidades do domínio
│   │   ├── Enums/
│   │   │   └── PapelUsuario.cs       # Enum de papéis
│   │   ├── DTOs/                     # Data Transfer Objects
│   │   │   ├── Auth/
│   │   │   ├── Usuario/
│   │   │   ├── Recomendacao/
│   │   │   └── Common/
│   │   ├── ML/                       # Modelos ML.NET
│   │   │   ├── CareerInput.cs        # Input para predição
│   │   │   └── CareerPrediction.cs   # Output da predição
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
│   │   ├── HabilidadeRepository.cs
│   │   └── RecomendacaoRepository.cs
│   ├── Services/                      # Lógica de negócio
│   │   ├── Interfaces/
│   │   ├── AuthService.cs
│   │   ├── UsuarioService.cs
│   │   ├── MLPredictionService.cs    # Serviço de predição ML.NET
│   │   └── RecomendacaoService.cs    # Lógica de recomendações
│   ├── appsettings.json              # Configurações
│   └── Program.cs                    # Ponto de entrada
│
├── CareerUp.Trainer/                  # Projeto de treinamento ML.NET
│   ├── Data/
│   │   └── training-data.csv         # Dataset com 15 perfis de carreira
│   ├── Models/
│   │   ├── CareerData.cs             # Modelo de dados de treino
│   │   └── CareerPrediction.cs       # Modelo de saída
│   ├── Program.cs                    # Pipeline de treinamento
│   └── CareerModel.zip               # Modelo gerado (copiado para API)
│
└── CareerUp.Tests/                    # Projeto de testes unitários
    ├── Models/                        # Testes de modelos
    │   ├── UsuarioTests.cs
    │   ├── HabilidadeTests.cs
    │   └── RecomendacaoTests.cs
    └── Services/                      # Testes de serviços (com Moq)
        ├── RecomendacaoServiceTests.cs  # 6 testes de regras de negócio
        └── UsuarioServiceTests.cs       # 2 testes de validações
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

### **Recomendações (Requer Autenticação):**
| Método | Endpoint | Permissão | Descrição |
|---------|----------|-----------|-----------|
| `POST` | `/api/v1/recomendacoes/gerar` | Todos | Gera nova recomendação com IA |
| `GET` | `/api/v1/recomendacoes/minhas` | Todos | Lista recomendações do usuário (paginado) |
| `GET` | `/api/v1/recomendacoes/{id}` | Próprio ou GERENTE | Busca recomendação por ID |
| `GET` | `/api/v1/recomendacoes/usuario/{id}` | GERENTE | Lista recomendações de usuário específico |
| `DELETE` | `/api/v1/recomendacoes/{id}` | Próprio ou GERENTE | Remove recomendação |

### **Recomendações v2 - Filtro por Mês (Requer Autenticação):**
| Método | Endpoint | Permissão | Descrição |
|---------|----------|-----------|-----------|
| `GET` | `/api/v2/recomendacoes/minhas?mes={1-12}` | Todos | Lista recomendações filtradas por mês (paginado) |

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
- **ASP.NET Core 9.0** - Framework web moderno e de alta performance
- **Entity Framework Core 9.0** - ORM com migrações e queries otimizadas
- **Oracle.EntityFrameworkCore 9.23.80** - Provider oficial Oracle

### **Machine Learning:**
- **Microsoft.ML 3.0.1** - Framework de ML da Microsoft
- **SDCA Maximum Entropy** - Algoritmo de classificação multiclasse
- **TF-IDF** - Vetorização de texto para features
- **Dataset:** 15 perfis de carreira (Desenvolvedor, Analista, DevOps, QA, Designer, etc.)
- **Acurácia:** 100% no dataset de treinamento

### **Autenticação e Segurança:**
- **JWT Bearer** - Tokens stateless com expiração configurável
- **BCrypt.Net** - Hash de senhas com work factor 12
- **ASP.NET Core Identity** - Framework de autenticação

### **Versionamento:**
- **Asp.Versioning.Mvc 8.1.0** - Versionamento de API por URL
- **Asp.Versioning.Mvc.ApiExplorer 8.1.0** - Suporte a Swagger multi-versão
- **v1 e v2** - Múltiplas versões da API em produção

### **Observabilidade:**
- **OpenTelemetry 1.10.x** - Tracing distribuído
- **Jaeger** - Visualização de traces
- **ILogger** - Logging estruturado
- **Health Checks** - Monitoramento

### **Documentação:**
- **Swashbuckle.AspNetCore 7.2.0** - Swagger/OpenAPI 3.0

### **Testes:**
- **xUnit 2.9.2** - Framework de testes unitários
- **Moq 4.20.72** - Biblioteca para mocking de dependências
- **11 testes unitários** - Cobertura de regras de negócio críticas

### **Machine Learning:**
- **Microsoft.ML 3.0.1** - Framework ML.NET para IA
- **SDCA Maximum Entropy** - Algoritmo de classificação multiclasse
- **TF-IDF** - Vetorização de features de texto
- **15 perfis de carreira** - Dataset de treinamento
- **100% de acurácia** - Micro e Macro Accuracy

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

---

## 🧪 **Testes Unitários**

O projeto inclui **11 testes unitários** validando regras de negócio críticas com **100% de aprovação**.

### **Testes de Modelos** (4 testes):
- ✅ `Usuario_DeveTerPropriedadesObrigatorias` - Valida modelo Usuario
- ✅ `Usuario_DevePermitirPapelGerente` - Valida enum PapelUsuario
- ✅ `Habilidade_DeveTerTresCamposPreenchidos` - Valida 3 habilidades obrigatórias
- ✅ `Recomendacao_DeveArmazenarResultadoIaEVinculoComUsuario` - Valida relacionamento

### **Testes de Serviços com Mocks** (7 testes):

**RecomendacaoService (6 testes):**
- ✅ `GerarRecomendacaoAsync_DeveGerarRecomendacao_QuandoUsuarioExiste`
- ✅ `GerarRecomendacaoAsync_DeveLancarExcecao_QuandoUsuarioNaoExiste`
- ✅ `GetByIdAsync_DeveRetornar_ApenasRecomendacaoDoProprioUsuario`
- ✅ `GetByIdAsync_DeveLancarUnauthorized_QuandoUsuarioTentaAcessarRecomendacaoDeOutro`
- ✅ `GetByIdAsync_DevePermitir_GerenteAcessarQualquerRecomendacao`

**UsuarioService (2 testes):**
- ✅ `UpdateCargoAsync_DeveLancarExcecao_QuandoUsuarioNaoExiste`
- ✅ `UpdateHabilidadesAsync_DeveLancarExcecao_QuandoUsuarioNaoExiste`

### **Executar Testes:**
```bash
dotnet test CareerUp.Tests/CareerUp.Tests.csproj --verbosity normal
```

**Resultado esperado:**
```
Test summary: total: 11, failed: 0, succeeded: 11, skipped: 0
Build succeeded in 1,0s
```

---

## 🎯 **Fluxo Completo de Uso**

### **1. Registrar Usuário:**
```bash
POST /api/v1/auth/register
```

### **2. Fazer Login:**
```bash
POST /api/v1/auth/login
```
→ Copie o `accessToken` retornado

### **3. Gerar Recomendação com IA:**
```bash
POST /api/v1/recomendacoes/gerar
Authorization: Bearer {accessToken}
```

A IA irá:
- ✅ Analisar seu cargo e habilidades
- ✅ Processar via modelo ML.NET (SDCA)
- ✅ Gerar recomendação personalizada
- ✅ Salvar no banco de dados

### **4. Ver Suas Recomendações:**
```bash
GET /api/v1/recomendacoes/minhas
Authorization: Bearer {accessToken}
```

### **5. Atualizar Habilidades (para melhorar recomendações):**
```bash
PUT /api/v1/usuarios/me/habilidades
Authorization: Bearer {accessToken}
```

### **6. Gerar Nova Recomendação:**
```bash
POST /api/v1/recomendacoes/gerar
Authorization: Bearer {accessToken}
```

→ A nova recomendação será baseada nas habilidades atualizadas! 🚀

---

## 🔢 **Versionamento de API**

A API CareerUp utiliza **versionamento por URL** para garantir compatibilidade e evolução contínua.

### **Versões Disponíveis:**

#### **v1 (Versão Estável)**
- **Base URL:** `/api/v1`
- **Endpoints:** Autenticacao, Usuarios, Recomendacoes
- **Status:** ✅ Estável e em produção
- **Recursos:**
  - Autenticação JWT
  - CRUD de usuários
  - Geração de recomendações com ML.NET
  - Lista de recomendações com paginação
  - HATEOAS

#### **v2 (Nova Versão - Filtros Avançados)**
- **Base URL:** `/api/v2`
- **Endpoints:** Recomendacoes (com filtro por mês)
- **Status:** ✅ Estável
- **Novos Recursos:**
  - 🆕 **Filtro por mês** em `/recomendacoes/minhas`
  - Parâmetro `?mes={1-12}` para filtrar recomendações por mês
  - Retrocompatível - funciona sem o parâmetro `mes`

### **Como Escolher a Versão:**

```bash
# Usar v1 (versão original)
GET /api/v1/recomendacoes/minhas

# Usar v2 (com filtro de mês)
GET /api/v2/recomendacoes/minhas?mes=11
```

### **Política de Versionamento:**

1. **Versionamento Semântico** - Novas versões para mudanças breaking
2. **Retrocompatibilidade** - v1 continua disponível indefinidamente
3. **Deprecação Gradual** - Avisos com 6 meses de antecedência
4. **Documentação Completa** - Cada versão documentada no Swagger

### **Headers de Versionamento:**

A API retorna informações de versão nos headers:

```http
api-supported-versions: 1.0, 2.0
api-version: 2.0
```

### **Swagger por Versão:**

Acesse a documentação específica de cada versão:

- **v1:** `https://localhost:5005/swagger` (selecione "CareerUp API v1")
- **v2:** `https://localhost:5005/swagger` (selecione "CareerUp API v2")

### **Roadmap de Versões:**

| Versão | Lançamento | Features | Status |
|---------|-------------|----------|--------|
| v1 | Nov 2025 | API base com ML.NET | ✅ Estável |
| v2 | Nov 2025 | Filtro por mês | ✅ Estável |
| v3 | Futuro | Filtros avançados (data range, cargo, skill) | 📅 Planejado |

---

## 📊 **Diagrama de Arquitetura**

```
┌─────────────┐
│   Cliente   │
│  (Swagger)  │
└──────┬──────┘
       │ HTTP/HTTPS
       ▼
┌─────────────────────────────────────┐
│       Controllers (API REST)         │
│  • AuthController                    │
│  • UsuariosController                │
│  • RecomendacoesController           │
└──────┬──────────────────────────────┘
       │
       ▼
┌─────────────────────────────────────┐
│         Services (Regras)            │
│  • AuthService                       │
│  • UsuarioService                    │
│  • RecomendacaoService               │
│  • MLPredictionService (ML.NET)      │
└──────┬──────────────────────────────┘
       │
       ├────────────┐
       │            │
       ▼            ▼
┌─────────────┐  ┌──────────────┐
│ Repositories│  │  ML.NET      │
│  (Data)     │  │  Prediction  │
│  • Usuario  │  │  Engine      │
│  • Login    │  │              │
│  • Habili.  │  │ CareerModel  │
│  • Recomen. │  │   .zip       │
└──────┬──────┘  └──────────────┘
       │
       ▼
┌─────────────────┐
│ Oracle Database │
│  • tb_usuario   │
│  • tb_login     │
│  • tb_habilid.  │
│  • tb_recomen.  │
└─────────────────┘
```