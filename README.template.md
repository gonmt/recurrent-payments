# Clean Architecture DDD Template

A comprehensive .NET template implementing **Clean Architecture** with **Domain-Driven Design (DDD)** principles. This template provides a solid foundation for building scalable, maintainable, and testable web applications.

## 🏗️ Architecture

```
src/
├── Archetype.Api/          # Web API layer (Controllers, Endpoints)
└── Archetype.Core/         # Domain and Application logic
    ├── Auth/              # Authentication domain
    ├── Shared/            # Shared domain components
    └── Users/             # Users domain

tests/
├── Archetype.Api.IntegrationTests/  # API integration tests
└── Archetype.Core.Tests/             # Unit tests
```

## ✨ Features

- **Clean Architecture**: Clear separation of concerns with dependency inversion
- **Domain-Driven Design**: Rich domain models and ubiquitous language
- **Modern .NET**: Targeting .NET 8.0/9.0 with latest features
- **Minimal APIs**: Using .NET's minimal API endpoints
- **Comprehensive Testing**: Unit tests and integration tests included
- **JWT Authentication**: Secure token-based authentication
- **Entity Framework Core**: Data persistence with EF Core
- **Validation**: FluentValidation for request validation
- **Docker Support**: Containerization ready
- **GitHub Actions**: CI/CD pipeline included
- **Quality Gates**: Code coverage, mutation testing, static analysis

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK (or .NET 8.0)
- Docker (optional)

### Installation

1. **Install the template**:
   ```bash
   dotnet new install Archetype.CleanArchitecture.Template
   ```

2. **Create a new project**:
   ```bash
   dotnet new cleanarch -n MyAwesomeProject --ProjectPrefix MyCompany
   ```

3. **Navigate to your project**:
   ```bash
   cd MyAwesomeProject
   ```

4. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

5. **Run the application**:
   ```bash
   dotnet run --project src/MyAwesomeProject.Api
   ```

### Template Parameters

| Parameter | Default | Description |
|-----------|---------|-------------|
| `ProjectName` | `MyProject` | Name of your project |
| `ProjectPrefix` | `MyCompany` | Company/prefix for namespaces |
| `TargetFramework` | `net9.0` | .NET version (net8.0, net9.0) |
| `IncludeDocker` | `true` | Include Docker files |
| `IncludeGitHubActions` | `true` | Include GitHub Actions |

**Example with custom parameters**:
```bash
dotnet new cleanarch -n FoodDelivery \
  --ProjectPrefix FoodCo \
  --TargetFramework net8.0 \
  --IncludeDocker true \
  --IncludeGitHubActions true
```

## 📁 Project Structure

### Core Domain (`Archetype.Core`)

- **Domain Layer**: Business logic, entities, value objects
- **Application Layer**: Use cases, handlers, interfaces
- **Infrastructure Layer**: External concerns (database, external services)

### API Layer (`Archetype.Api`)

- **Endpoints**: Minimal API endpoints
- **Dependency Injection**: Service registration
- **Configuration**: App settings, JWT configuration

### Testing Projects

- **Unit Tests**: Testing domain logic with xUnit
- **Integration Tests**: API testing with WebApplicationFactory

## 🧪 Development Workflow

### Running Tests
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run mutation testing
dotnet stryker
```

### Code Quality
```bash
# Format code
dotnet format

# Static analysis
dotnet analyze
```

### Docker Development
```bash
# Build image
docker build -t myproject .

# Run with Docker Compose
docker-compose up
```

## 🔧 Configuration

### JWT Configuration
Update `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "your-secret-key-here",
    "Issuer": "your-issuer",
    "Audience": "your-audience",
    "ExpirationMinutes": 60
  }
}
```

### Database Configuration
The template uses Entity Framework Core with SQLite by default. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  }
}
```

## 📚 Learning Resources

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) by Robert C. Martin
- [Domain-Driven Design](https://domaindrivendesign.org/) by Eric Evans
- [Minimal APIs in .NET](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)

## 🤝 Contributing

Feel free to submit issues and enhancement requests!

## 📄 License

This template is provided as-is for educational and commercial use.

---

**Created with ❤️ using Clean Architecture principles**