# 📦 Pedidos API (.NET 8)

API REST para gerenciamento de pedidos construída com Clean Architecture. O projeto usa EF Core com SQLite, FluentValidation, AutoMapper e Docker para desenvolvimento e avaliação.

## 🏗️ Arquitetura

A solução é organizada em camadas separadas:

- `Domain`: entidades, regras de negócio e exceções do domínio.
- `Application`: casos de uso, DTOs, serviços, validações e mapeamentos.
- `Infrastructure`: persistência, repositórios, configurações e dependências externas.
- `Api`: controlador, rotas, middleware e configuração da aplicação web.

## ✨ Funcionalidades

- Criação de pedidos com itens.
- Consulta de pedido por ID.
- Listagem paginada de pedidos com filtro por status.
- Cancelamento de pedidos.
- Pagamento de pedidos.
- Validações de entrada com FluentValidation.
- Tratamento global de exceções.
- Persistência em SQLite com migrações aplicadas no startup.

## 📡 Endpoints

| Método | Endpoint | Descrição |
|---|---|---|
| POST | `/pedidos` | Cria um novo pedido |
| GET | `/pedidos/{id}` | Recupera um pedido por ID |
| GET | `/pedidos` | Lista pedidos com paginação e filtro por status |
| PATCH | `/pedidos/{id}/cancelar` | Cancela um pedido |
| PATCH | `/pedidos/{id}/pagar` | Marca um pedido como pago |

### Exemplo de listagem

```
GET /pedidos?status=Pago&page=1&pageSize=10
```

## 🧾 Regras de negócio importantes

- Pedido deve conter ao menos um item.
- Quantidade de item deve ser maior do que zero.
- Preço unitário de item deve ser maior do que zero.
- Pedido pago não pode ser cancelado.
- Pedido cancelado não pode ser pago.
- Total do pedido é calculado automaticamente.

## 🧪 Testes

O projeto contém uma suíte de testes organizada em três camadas:

- `tests/UnitTests`: testes unitários de regras de negócio e serviços.
- `tests/ApiTests`: testes de contrato e middleware da API.
- `tests/IntegrationTests`: testes de integração entre as camadas e a persistência.

### Executar todos os testes

```bash
dotnet test
```

### Executar testes específicos

```bash
dotnet test tests/UnitTests/UnitTests.csproj

dotnet test tests/ApiTests/ApiTests.csproj

dotnet test tests/IntegrationTests/IntegrationTests.csproj
```

## 🚀 Como executar

### Ambiente de avaliação

```bash
docker compose up --build server
```

A API ficará disponível em:

```bash
http://localhost:8080
```

### Ambiente de desenvolvimento

```bash
docker compose up --build dev
```

## 📘 Swagger

A documentação interativa da API está disponível em:

```bash
http://localhost:8080/swagger
```

## 🗄️ Banco de Dados

- Banco: SQLite
- Persistência via volume Docker.
- Arquivo local de desenvolvimento: `persistence/pedidos.db`
- Arquivo no container de avaliação: `/app/persistence/pedidos.db`
- O banco é criado automaticamente e as migrações são aplicadas no startup da API.

## 🧱 Tecnologias

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- FluentValidation
- AutoMapper
- Docker
- xUnit

## 📌 Observações

- A aplicação monta o banco e executa migrações tanto em modo `Development` quanto em `Production`.
- A configuração de injeção de dependências está centralizada em `Application.DependencyInjection` e `Infrastructure.DependencyInjection`.
