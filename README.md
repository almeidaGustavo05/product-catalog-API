# 🛒 Product Catalog API

API REST para gerenciamento de catálogo de produtos desenvolvida em .NET 8 com arquitetura em camadas, seguindo princípios SOLID e Clean Architecture.

## 🏗️ Arquitetura

O projeto está organizado em 4 camadas principais:

- **ProductCatalog.Domain**: Entidades, enums, interfaces e regras de negócio
- **ProductCatalog.Application**: Serviços de aplicação, DTOs e mapeamentos
- **ProductCatalog.Infrastructure**: Implementação de repositórios, contexto do EF Core e serviços externos
- **ProductCatalog.API**: Controllers, configurações e ponto de entrada da aplicação
- **ProductCatalog.Tests**: Testes unitários e de integração

## 🚀 Funcionalidades

### Endpoints Disponíveis

- **GET** `/api/products` - Lista produtos com filtros opcionais
- **GET** `/api/products/{id}` - Busca produto por ID
- **POST** `/api/products` - Cria novo produto
- **PUT** `/api/products/{id}` - Atualiza produto existente
- **DELETE** `/api/products/{id}` - Remove produto
- **PATCH** `/api/products/{id}/activate` - Ativa produto
- **PATCH** `/api/products/{id}/deactivate` - Desativa produto
- **POST** `/api/products/{id}/image` - Upload de imagem do produto

### Filtros Disponíveis

- Por categoria
- Por faixa de preço (mínimo e máximo)
- Por status (Ativo/Inativo)
- Combinação de filtros

## 🛠️ Tecnologias Utilizadas

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core 9.0**
- **PostgreSQL**
- **AutoMapper**
- **Swagger/OpenAPI**
- **xUnit** (testes)
- **FluentAssertions** (testes)
- **Moq** (mocks para testes)
- **Docker & Docker Compose**

## 📋 Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (para execução com containers)
- [PostgreSQL](https://www.postgresql.org/download/) (para execução local sem Docker)

## 🚀 Como Executar

### Opção 1: Com Docker (Recomendado)

1. Clone o repositório:
```bash
git clone <url-do-repositorio>
cd prova-pratica
```

2. Execute com Docker Compose:
```bash
docker-compose up --build
```

3. Acesse a aplicação:
   - API: http://localhost:8080
   - Swagger: http://localhost:8080/swagger

### Opção 2: Execução Local

1. Clone o repositório:
```bash
git clone <url-do-repositorio>
cd prova-pratica
```

2. Configure a string de conexão no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ProductCatalogDb;Username=postgres;Password=postgres"
  }
}
```

3. Restaure as dependências:
```bash
dotnet restore
```

4. Execute as migrações do banco:
```bash
dotnet ef database update --project ProductCatalog.Infrastructure --startup-project ProductCatalog.API
```

5. Execute a aplicação:
```bash
dotnet run --project ProductCatalog.API
```

6. Acesse a aplicação:
   - API: https://localhost:7001 ou http://localhost:5001
   - Swagger: https://localhost:7001/swagger

## 🧪 Executando os Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com relatório de cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📁 Estrutura do Projeto

```
ProductCatalog/
├── ProductCatalog.API/           # Controllers e configurações da API
├── ProductCatalog.Application/   # Serviços, DTOs e interfaces de aplicação
├── ProductCatalog.Domain/        # Entidades, enums e interfaces de domínio
├── ProductCatalog.Infrastructure/# Repositórios, contexto EF e serviços
├── ProductCatalog.Tests/         # Testes unitários e de integração
├── docker-compose.yml            # Configuração do Docker Compose
├── Dockerfile                    # Imagem Docker da aplicação
└── README.md                     # Este arquivo
```

## 🔧 Configurações

### Variáveis de Ambiente (Docker)

- `ASPNETCORE_ENVIRONMENT`: Ambiente da aplicação (Development/Production)
- `ConnectionStrings__DefaultConnection`: String de conexão do PostgreSQL

### Upload de Imagens

As imagens são armazenadas localmente na pasta `uploads/` e servidas como arquivos estáticos.

## 📊 Banco de Dados

### Modelo de Dados

**Tabela: Products**
- Id (UUID, PK)
- Name (string, obrigatório)
- Description (string, opcional)
- Price (decimal, obrigatório)
- Category (string, obrigatório)
- Status (enum: Active/Inactive)
- ImageUrl (string, opcional)
- CreatedAt (datetime)
- UpdatedAt (datetime)

### Índices

- Category (para consultas por categoria)
- Status (para filtros por status)
- Price (para consultas por faixa de preço)

## 🧪 Testes

O projeto inclui testes para:

- **Entidades de Domínio**: Validação de regras de negócio
- **Serviços de Aplicação**: Lógica de negócio e mapeamentos
- **Repositórios**: Operações de banco de dados (testes de integração)

## 📝 Documentação da API

A documentação completa da API está disponível através do Swagger UI quando a aplicação está em execução.

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.
