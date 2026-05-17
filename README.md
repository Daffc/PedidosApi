# 🛒 Pedidos API

API REST desenvolvida em **.NET 8** para gerenciamento de pedidos, seguindo os princípios de **Clean Architecture** com separação clara de responsabilidades entre as camadas.

---

## 📋 Índice

- [Tecnologias](#-tecnologias)
- [Arquitetura](#-arquitetura)
- [Pré-requisitos](#-pré-requisitos)
- [Execução](#-execução)
  - [Com Docker](#com-docker)
  - [Desenvolvimento Local](#desenvolvimento-local)
- [Endpoints](#-endpoints)
- [Modelos de Dados](#-modelos-de-dados)
- [Regras de Negócio](#-regras-de-negócio)
- [Tratamento de Erros](#-tratamento-de-erros)
- [Testes](#-testes)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Convenções](#-convenções)

---

## 🛠️ Tecnologias

| Tecnologia | Finalidade |
|---|---|
| .NET 8 | Runtime e SDK |
| ASP.NET Core Web API | Framework HTTP |
| Entity Framework Core | ORM / Persistência |
| SQLite | Banco de dados |
| FluentValidation | Validação de requests |
| AutoMapper | Mapeamento DTO ↔ Entidade |
| xUnit | Framework de testes |
| Docker | Containerização |
| Swagger (Swashbuckle) | Documentação interativa |

---

## 🏗️ Arquitetura

A solução adota uma **Clean Architecture pragmática**, com as camadas organizadas da seguinte forma:

```
┌──────────────────────────────────────────────────┐
│                     Api                          |
│  (Controllers, Middlewares, DI, Program.cs)      |
├──────────────────────────────────────────────────┤
│                  Application                     |
│  (DTOs, Services, Validators, Interfaces,        |
│   AutoMapper Profiles, Paginação)                |
├──────────────────────────────────────────────────┤
│                   Domain                         |
│  (Entidades, Enums, Exceções, Regras de Negócio) |
├──────────────────────────────────────────────────┤
│                Infrastructure                    |
│  (EF Core, DbContext, Migrations, Repositories,  |
│   Configurações Fluent API, DI)                  |
└──────────────────────────────────────────────────┘
```

**Princípio de dependência:** camadas exteriores dependem de camadas interiores. O `Domain` não depende de nenhuma outra camada.

---

## 📋 Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) — para execução local
- [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/) — para execução em container

---

## ▶️ Execução

### Com Docker

**Modo avaliação (API + Swagger):**

```bash
docker compose up --build server
```

A API ficará disponível em `http://localhost:8080`  
O Swagger estará acessível em `http://localhost:8080/swagger`

**Modo desenvolvimento:**

```bash
docker compose up --build dev
```

O container entra em modo *sleep*, permitindo executar comandos manualmente dentro dele:
```bash
docker compose exec dev dotnet run --project src/Api
```

### Desenvolvimento Local

```bash
# Restaurar pacotes
dotnet restore

# Executar a API
dotnet run --project src/Api

# A aplicação estará disponível em http://localhost:5000
```

> A string de conexão com o SQLite é parametrizada por variáveis de ambiente:
> - `ASPNETCORE_ENVIRONMENT`
> - `ConnectionStrings__DefaultConnection`

---

## 🔌 Endpoints

Todos os endpoints estão sob o prefixo `/pedidos`.

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/pedidos` | Cria um novo pedido |
| `GET` | `/pedidos/{id}` | Busca um pedido por ID |
| `GET` | `/pedidos` | Lista pedidos com paginação e filtro opcional por status |
| `PATCH` | `/pedidos/{id}/cancelar` | Cancela um pedido |
| `PATCH` | `/pedidos/{id}/pagar` | Marca um pedido como pago |

### Exemplos de Requisição

#### Criar Pedido — `POST /pedidos`
```json
{
  "clienteNome": "João da Silva",
  "itens": [
    { "produtoNome": "Teclado Mecânico", "quantidade": 1, "precoUnitario": 350.00 },
    { "produtoNome": "Mouse Gamer",    "quantidade": 2, "precoUnitario": 120.00 }
  ]
}
```

#### Listar Pedidos — `GET /pedidos`
Parâmetros de query:
| Parâmetro | Tipo | Descrição |
|---|---|---|
| `page` | `int` | Número da página (padrão: `1`) |
| `pageSize` | `int` | Itens por página (padrão: `10`) |
| `statusPedido` | `string` | Filtro por status: `Novo`, `Pago` ou `Cancelado` (opcional) |

Exemplo: `GET /pedidos?page=1&pageSize=10&statusPedido=Novo`

---

## 🧩 Modelos de Dados

### Enums

```csharp
public enum StatusPedido
{
    Novo     = 1,  // Pedido criado, aguardando pagamento
    Pago     = 2,  // Pedido confirmado e pago
    Cancelado = 3  // Pedido cancelado
}
```

### Entidades de Domínio

#### `Pedido` (Aggregate Root)
| Propriedade | Tipo | Descrição |
|---|---|---|
| `Id` | `Guid` | Identificador único |
| `ClienteNome` | `string` | Nome do cliente |
| `DataCriacao` | `DateTime` | Data de criação (UTC) |
| `Status` | `StatusPedido` | Situação atual do pedido |
| `Itens` | `IReadOnlyCollection<ItemPedido>` | Lista de itens |
| `ValorTotal` | `decimal` | Soma dos subtotais (calculado dinamicamente) |

#### `ItemPedido`
| Propriedade | Tipo | Descrição |
|---|---|---|
| `Id` | `Guid` | Identificador único |
| `PedidoId` | `Guid` | Referência ao pedido |
| `ProdutoNome` | `string` | Nome do produto |
| `Quantidade` | `int` | Quantidade (deve ser > 0) |
| `PrecoUnitario` | `decimal` | Preço unitário (deve ser > 0) |
| `Subtotal` | `decimal` | `Quantidade × PrecoUnitario` (calculado dinamicamente) |

### DTOs

#### Request — `CreatePedidoRequest`
```csharp
record CreatePedidoRequest(string ClienteNome, IReadOnlyCollection<ItemPedidoRequest> Itens)
record ItemPedidoRequest(string ProdutoNome, int Quantidade, decimal PrecoUnitario)
```

#### Response — `PedidoResponse`
```csharp
record PedidoResponse(Guid Id, string ClienteNome, DateTime DataCriacao, string Status, decimal ValorTotal, IReadOnlyCollection<PedidoItemResponse> Itens)
record PedidoItemResponse(Guid Id, string ProdutoNome, int Quantidade, decimal PrecoUnitario, decimal Subtotal)
```

#### Paginação — `PagedResponse<T>`
```csharp
record PagedResponse<T>(int Page, int PageSize, int TotalItems, int TotalPages, IReadOnlyCollection<T> Items)
```

---

## 📏 Regras de Negócio

### Domínio — `Pedido`
- 🔒 **Nome do cliente é obrigatório** — não pode ser vazio ou nulo.
- 🔒 **O pedido deve possuir pelo menos um item.**
- 🔒 **Pedido pago não pode ser cancelado.**
- 🔒 **Pedido cancelado não não pode ser pago.**
- 🔒 **`ValorTotal` é calculado automaticamente** como a soma dos `Subtotal` dos itens.
- 🔒 `Status` inicial é `Novo`.

### Domínio — `ItemPedido`
- 🔒 **ProdutoNome é obrigatório.**
- 🔒 **Quantidade deve ser maior que zero.**
- 🔒 **Preço unitário deve ser maior que zero.**
- 🔒 **`Subtotal` é calculado dinamicamente** como `Quantidade × PrecoUnitario`.

### Fluxo de status permitido:
```
Novo ──► Pago
  │
  └──────► Cancelado
```

Fila de status irretrospectiva: uma vez `Cancelado`, não pode voltar para `Novo`. Uma vez `Pago`, não pode ser `Cancelado`.

---

## ⚠️ Tratamento de Erros

A API utiliza um **middleware global de exceções** (`GlobalExceptionHandlingMiddleware`) que captura todas as exceções não tratadas e retorna uma resposta padronizada em `ProblemDetails` com `traceId`.

| Tipo de Exceção | Status HTTP | Descrição |
|---|---|---|
| `DomainException` | `400 Bad Request` | Violação de regra de negócio |
| `ValidationException` | `400 Bad Request` | Erro de validação FluentValidation |
| `KeyNotFoundException` | `404 Not Found` | Recurso não encontrado |
| `ArgumentException` | `400 Bad Request` | Argumento inválido |
| Qualquer outra exceção | `500 Internal Server Error` | Erro interno genérico |

A resposta sempre inclui os campos: `Title`, `Status`, `Detail`, `Instance` e `traceId`.

---

## 🧪 Testes

O projeto possui **três suítes de testes** organizadas por escopo:

### Estrutura de Testes

```
tests/
├── UnitTests/       → Domínio, Validadores, Services (sem dependências externas)
├── IntegrationTests → EF Core, Repositories, Services com SQLite real
└── ApiTests/        → Controllers, Middleware, pipeline HTTP completo
```

### Executar todos os testes

```bash
dotnet test
```

### Executar por suíte

```bash
# Testes unitários (Domínio + Validators + Services)
dotnet test tests/UnitTests

# Testes de integração (EF Core + Repository + Service com SQLite)
dotnet test tests/IntegrationTests

# Testes de API (Controllers + Middleware + Pipeline HTTP)
dotnet test tests/ApiTests
```

### Cobertura

| Suíte | Escopo |
|---|---|
| `UnitTests` | Entidades do Domínio, `StatusPedido`, `DomainException`, `PedidoService`, Validadores FluentValidation |
| `IntegrationTests` | Configurações EF Core, `AppDbContext`, `PedidoService` com banco SQLite real |
| `ApiTests` | `PedidosController`, `GlobalExceptionHandlingMiddleware`, pipeline HTTP completo |

---

## 🗂️ Estrutura do Projeto

```
PedidosApi/
├── src/
│   ├── Api/
│   │   ├── Controllers/
│   │   │   └── PedidosController.cs
│   │   ├── Middlewares/
│   │   │   └── GlobalExceptionHandlingMiddleware.cs
│   │   └── Program.cs
│   ├── Application/
│   │   ├── DTOs/
│   │   │   ├── Requests/       → Create/Get Pedido requests
│   │   │   └── Responses/      → PedidoResponse, PagedResponse
│   │   ├── Interfaces/
│   │   │   ├── Repositories/IPedidoRepository.cs
│   │   │   └── Services/IPedidoService.cs
│   │   ├── Services/
│   │   │   └── PedidoService.cs
│   │   ├── Validators/
│   │   │   ├── Common/         → PaginationRequestValidator
│   │   │   └── Requests/       → Create/Get Pedido + ItemPedido validators
│   │   ├── Mappings/
│   │   │   └── PedidoProfile.cs (AutoMapper)
│   │   └── DependencyInjection/
│   │       └── ApplicationDependencyInjection.cs
│   ├── Domain/
│   │   ├── Common/
│   │   │   └── Entity.cs
│   │   ├── Entities/
│   │   │   ├── Pedido.cs       (Aggregate Root)
│   │   │   └── ItemPedido.cs
│   │   ├── Enums/
│   │   │   └── StatusPedido.cs
│   │   └── Exceptions/         → Domain exceptions específicas
│   └── Infrastructure/
│       ├── Persistence/
│       │   ├── AppDbContext.cs
│       │   ├── Configurations/ → Fluent API mappings
│       │   ├── Migrations/     → EF Core migrations
│       │   └── Database/
│       │       └── MigrationManager.cs
│       ├── Repositories/
│       │   └── PedidoRepository.cs
│       └── DependencyInjection/
│           └── InfrastructureDependencyInjection.cs
├── tests/
│   ├── UnitTests/
│   ├── IntegrationTests/
│   └── ApiTests/
├── Dockerfile
├── Dockerfile.dev
├── docker-compose.yml
├── PedidosApi.sln
└── AGENTS.md
```

---

## 📝 Convenções

### Entidades
- Encapsulamento rigoroso: setters são `private` ou `protected`
- Regras de negócio permanecem no domínio
- Sem `DataAnnotations`

### Persistência
- Configurações via **Fluent API** (sem DataAnnotations)
- Propriedades derivadas usam `Ignore()` no EF Core (ex: `ValorTotal`, `Subtotal`)

### Controllers
- Controllers são **finos** — delegam toda a lógica para services
- Uso obrigatório de **DTOs** nas entradas e saídas

### Exceções
- Regras de negócio lançam `DomainException` ou exceções específicas
- Erros HTTP são centralizados no **middleware global**

### Logs
- Uso de `ILogger<T>` em controllers e middleware
- Inclusão de IDs de entidade nas mensagens de log

### Testes
- Novas regras devem possuir testes
- Priorizar testes de domínio
- Evitar mocks desnecessários em testes de integração

---

## 🐳 Docker

### Arquivos

| Arquivo | Finalidade |
|---|---|
| `Dockerfile` | Build multi-stage para avaliação |
| `Dockerfile.dev` | Container de desenvolvimento com volume bind-mount |
| `docker-compose.yml` | Orquestração dos serviços |

### Comandos

```bash
# Avaliação
docker compose up --build server

# Desenvolvimento
docker compose up --build dev
```

Swagger disponível em `http://localhost:8080/swagger` no modo avaliação.
