# 📦 Pedidos API (.NET 8)
API REST para gerenciamento de pedidos com Clean Architecture, EF Core + SQLite, FluentValidation, AutoMapper e Docker.
## 🚀 Execução
### 🟢 Avaliação (local)
```
docker compose up --build server
```
Acesso:
```
http://localhost:8080
```
### 🧪 Desenvolvimento
```
docker compose up --build dev
```
## 🗄️ Banco de Dados
- SQLite
- Persistência via volume Docker
- Caminho: 
	- Avaliação (server):
		- ```/app/persistence/pedidos.db```
	- Desenvolvimento (dev)
		- ```/workspace/persistence/pedidos.db```
- Criado automaticamente na inicialização
- Migrations aplicadas no startup
## 📡 Endpoints
### Criar pedido
```
POST /pedidos
```
### Buscar por ID
```
GET /pedidos/{id}
```
### Listar pedidos
```
GET /pedidos?status=Pago&page=1&pageSize=10
```
### Cancelar pedido
```
PATCH /pedidos/{id}/cancelar
```
### Pagar Pedido
```
PATCH /pedidos/{id}/pagar
```
## 🧪 Testes
```
dotnet test
```
## 🧱 Stack
- .NET 8 Web API
- EF Core
- SQLite
- FluentValidation
- AutoMapper
- Docker
- xUnit
## 📌 Regras
- Pedido exige itens
- Quantidade > 0
- Preço > 0
- Pedido pago não cancela
- Valor total calculado automaticamente
## 🏗️ Arquitetura
- Domain / Application / Infrastructure / Api
- Regras de negócio no Domain
- Casos de uso no Application
- Persistência via EF Core