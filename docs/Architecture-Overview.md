# E-Commerce Architecture Overview

## Project Structure
```
Asp.Net9.Ecommerce/
├── src/
│   ├── Asp.Net9.Ecommerce.Domain/        # Core domain models
│   ├── Asp.Net9.Ecommerce.Application/   # Application use cases
│   ├── Asp.Net9.Ecommerce.Infrastructure/# External concerns
│   ├── Asp.Net9.Ecommerce.API/           # API endpoints
│   └── Asp.Net9.Ecommerce.Shared/        # Shared kernel
└── docs/                                 # Documentation
```

## Design Patterns & Principles

### 1. Domain-Driven Design
- Rich domain models
- Encapsulation of business rules
- Value objects for immutable concepts
- Aggregates for transactional boundaries

### 2. Clean Architecture
- Dependencies flow inward
- Domain layer has no external dependencies
- Application layer orchestrates use cases
- Infrastructure implements interfaces

### 3. CQRS with MediatR
- Commands: Write operations
- Queries: Read operations
- Handlers: Business logic
- No direct controller-to-service calls

### 4. Result Pattern
- Explicit success/failure states
- Domain validation results
- No exceptions for business logic
- Type-safe error handling

## Key Technical Decisions

### 1. Authentication
- ASP.NET Core Identity
- JWT tokens
- Refresh token strategy
- See: [Identity-Implementation-Plan.md](Identity-Implementation-Plan.md)

### 2. Database
- Entity Framework Core
- SQL Server
- Code-first migrations
- Repository pattern

### 3. Validation
- FluentValidation
- Domain-level validation
- Application-level validation
- API-level validation

### 4. API
- REST principles
- Versioning strategy
- Documentation with Swagger
- Rate limiting

## Current Status
- [x] Base entity implementation
- [x] Identity user model
- [x] Result pattern
- [ ] Authentication
- [ ] Core domain models
- [ ] API endpoints 