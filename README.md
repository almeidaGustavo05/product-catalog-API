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
- **FluentValidation**
- **Swagger/OpenAPI**
- **xUnit** (testes)
- **FluentAssertions** (testes)
- **Moq** (mocks para testes)
- **Docker & Docker Compose**

## 📋 Pré-requisitos

### Para execução com Docker (Recomendado)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Para execução local
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/)

## 🚀 Como Executar o Projeto

### 🐳 Opção 1: Com Docker (Recomendado)

Esta é a forma mais simples de executar o projeto, pois não requer instalação de dependências locais.

1. **Clone o repositório:**
```bash
git clone https://github.com/almeidaGustavo05/product-catalog-API.git
cd product-catalog-API
```

2. **Execute com Docker Compose:**
```bash
docker-compose up --build
```

3. **Acesse a aplicação:**
   - **API Base:** http://localhost
   - **Swagger UI:** http://localhost/swagger
   - **Banco PostgreSQL:** localhost:5432

> **Nota:** O Docker Compose irá:
> - Criar e configurar automaticamente o banco PostgreSQL
> - Compilar e executar a aplicação .NET
> - Aplicar as migrações do banco de dados
> - Expor a API na porta 80

### 💻 Opção 2: Execução Local

Para desenvolvimento local ou quando não for possível usar Docker.

#### 2.1. Preparação do Ambiente

1. **Clone o repositório:**
```bash
git clone https://github.com/almeidaGustavo05/product-catalog-API.git
cd product-catalog-API
```

2. **Configure o PostgreSQL:**
   - Instale o PostgreSQL 15+
   - Crie um banco de dados chamado `productcatalogdb`
   - Configure usuário e senha (padrão: postgres/postgres)

3. **Configure a string de conexão:**
   
   Edite o arquivo `ProductCatalog.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=productcatalogdb;Username=postgres;Password=postgres"
  }
}
```

#### 2.2. Execução

1. **Restaure as dependências:**
```bash
dotnet restore
```

2. **Aplique as migrações do banco:**
```bash
dotnet ef database update --project ProductCatalog.Infrastructure --startup-project ProductCatalog.API
```

3. **Execute a aplicação:**
```bash
dotnet run --project ProductCatalog.API
```

4. **Acesse a aplicação:**
   - **API Base:** https://localhost:7137 ou http://localhost:5062
   - **Swagger UI:** https://localhost:7137/swagger ou http://localhost:5062/swagger

## 🧪 Executando os Testes

### Executar todos os testes
```bash
dotnet test
```

### Executar testes com relatório de cobertura
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Executar testes de um projeto específico
```bash
# Testes de domínio
dotnet test ProductCatalog.Tests/ProductCatalog.Tests.csproj --filter "Category=Domain"

# Testes de aplicação
dotnet test ProductCatalog.Tests/ProductCatalog.Tests.csproj --filter "Category=Application"

# Testes de infraestrutura
dotnet test ProductCatalog.Tests/ProductCatalog.Tests.csproj --filter "Category=Infrastructure"
```

## 📊 Testando a API

### Usando Swagger UI
1. Acesse o Swagger UI conforme as instruções de execução acima
2. Explore e teste todos os endpoints disponíveis
3. Use os exemplos fornecidos para criar produtos de teste

### Usando curl (exemplos)

**Criar um produto:**
```bash
curl -X POST "http://localhost/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Smartphone Samsung Galaxy",
    "description": "Smartphone com 128GB de armazenamento",
    "price": 1299.99,
    "category": "Eletrônicos"
  }'
```

**Listar produtos:**
```bash
curl -X GET "http://localhost/api/products"
```

**Buscar produto por ID:**
```bash
curl -X GET "http://localhost/api/products/1"
```

**Filtrar produtos:**
```bash
curl -X GET "http://localhost/api/products?category=Eletrônicos&minPrice=1000&maxPrice=2000"
```

## 📁 Estrutura do Projeto

```
ProductCatalog/
├── ProductCatalog.API/           # Controllers e configurações da API
│   ├── Controllers/              # Controllers da Web API
│   ├── Middleware/               # Middlewares customizados
│   ├── Properties/               # Configurações de launch
│   └── Program.cs                # Ponto de entrada da aplicação
├── ProductCatalog.Application/   # Serviços, DTOs e interfaces de aplicação
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Interfaces/               # Interfaces de serviços de aplicação
│   ├── Mappings/                 # Perfis do AutoMapper
│   ├── Services/                 # Implementação dos serviços
│   └── Validators/               # Validadores FluentValidation
├── ProductCatalog.Domain/        # Entidades, enums e interfaces de domínio
│   ├── Entities/                 # Entidades de domínio
│   ├── Enums/                    # Enumerações
│   └── Interfaces/               # Interfaces de repositórios
├── ProductCatalog.Infrastructure/# Repositórios, contexto EF e serviços
│   ├── Data/                     # Contexto e configurações do EF Core
│   ├── Repositories/             # Implementação dos repositórios
│   └── Services/                 # Serviços de infraestrutura
├── ProductCatalog.Tests/         # Testes unitários e de integração
│   ├── Application/              # Testes dos serviços de aplicação
│   ├── Domain/                   # Testes das entidades de domínio
│   └── Infrastructure/           # Testes dos repositórios
├── docker-compose.yml            # Configuração do Docker Compose
├── Dockerfile                    # Imagem Docker da aplicação
└── README.md                     # Este arquivo
```

## 🔧 Configurações Importantes

### Variáveis de Ambiente (Docker)
- `ASPNETCORE_ENVIRONMENT`: Ambiente da aplicação (Development/Production)
- `ConnectionString__DbConfig`: String de conexão do PostgreSQL

### Configurações do Banco de Dados
- **Host:** postgres (Docker) / localhost (local)
- **Porta:** 5432
- **Banco:** productcatalogdb
- **Usuário:** productcatalogadmin (Docker) / postgres (local)
- **Senha:** senha (Docker) / postgres (local)

### Upload de Imagens
- As imagens são armazenadas localmente na pasta `uploads/`
- Formatos aceitos: JPG, JPEG, PNG, GIF
- Tamanho máximo: 5MB
- As imagens são servidas como arquivos estáticos

## 📊 Modelo de Dados

### Tabela: Products
| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | int (PK) | Identificador único do produto |
| Name | varchar(200) | Nome do produto (obrigatório) |
| Description | varchar(1000) | Descrição detalhada |
| Price | decimal(18,2) | Preço do produto (obrigatório) |
| Category | varchar(100) | Categoria do produto (obrigatório) |
| Status | varchar(50) | Status (Active/Inactive) |
| ImageUrl | varchar(500) | URL da imagem do produto |
| CreatedAt | datetime | Data de criação |
| UpdatedAt | datetime | Data da última atualização |
| DeletedAt | datetime | Data de exclusão (soft delete) |

### Índices Criados
- `IX_Products_Category` - Para consultas por categoria
- `IX_Products_Status` - Para filtros por status
- `IX_Products_Price` - Para consultas por faixa de preço
- `IX_Products_DeletedAt` - Para otimizar soft deletes

## 🧪 Cobertura de Testes

O projeto inclui testes abrangentes para:

### Testes de Domínio
- ✅ Validação de regras de negócio das entidades
- ✅ Comportamento dos construtores e métodos
- ✅ Validações de dados obrigatórios

### Testes de Aplicação
- ✅ Lógica de negócio dos serviços
- ✅ Mapeamentos entre DTOs e entidades
- ✅ Validações de entrada
- ✅ Tratamento de exceções

### Testes de Infraestrutura
- ✅ Operações CRUD dos repositórios
- ✅ Consultas com filtros
- ✅ Configurações do Entity Framework

## 📝 Documentação da API

A documentação completa da API está disponível através do Swagger UI quando a aplicação está em execução. O Swagger inclui:

- 📋 Lista completa de endpoints
- 📝 Descrição detalhada de cada operação
- 🔧 Exemplos de requisições e respostas
- 🧪 Interface para testar os endpoints
- 📊 Modelos de dados (DTOs)

## ❓ Solução de Problemas

### Problemas Comuns

**Erro de conexão com banco de dados:**
- Verifique se o PostgreSQL está rodando
- Confirme a string de conexão no appsettings.json
- Para Docker: aguarde o container do postgres inicializar completamente

**Erro ao aplicar migrações:**
```bash
# Limpe e recrie as migrações se necessário
dotnet ef database drop --project ProductCatalog.Infrastructure --startup-project ProductCatalog.API
dotnet ef database update --project ProductCatalog.Infrastructure --startup-project ProductCatalog.API
```

**Porta já em uso:**
- Altere as portas no docker-compose.yml ou launchSettings.json
- Ou pare outros serviços que estejam usando as mesmas portas

**Problemas com upload de imagens:**
- Verifique se a pasta `uploads/` tem permissões de escrita
- Confirme o tamanho máximo do arquivo (5MB)
- Verifique o formato do arquivo (JPG, PNG, GIF)
