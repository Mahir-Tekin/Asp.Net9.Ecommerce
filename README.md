# E-Commerce Platform - ASP.NET Core 9 & Next.js

A full-stack e-commerce application I built to learn and demonstrate modern .NET development practices. This project implements Clean Architecture, Domain-Driven Design, and CQRS patterns.

## 🚀 Live Demo

- **Frontend**: [https://asp-net9-ecommerce.vercel.app](https://asp-net9-ecommerce.vercel.app) - Next.js application

## What I Built

This project includes:
- Clean Architecture with proper layer separation
- Domain-Driven Design with rich domain models
- CQRS implementation using MediatR
- JWT authentication with refresh token rotation
- Result pattern for error handling
- Full-stack implementation with Next.js frontend
- Complex product management with variants and categories

## Tech Stack

**Backend:**
- ASP.NET Core 9
- Entity Framework Core with PostgreSQL
- ASP.NET Identity for user management
- MediatR for CQRS implementation
- AutoMapper for object mapping

**Frontend:**
- Next.js 14 with TypeScript
- Tailwind CSS for styling
- React Context for state management

## Architecture

I implemented Clean Architecture to keep business logic separate from infrastructure concerns:

```
API Layer (Controllers, Middleware)
├── Application Layer (Use Cases, DTOs)
├── Domain Layer (Entities, Business Rules)
└── Infrastructure Layer (Database, External Services)
```

The project uses CQRS to separate read and write operations, making it easier to optimize each side independently.

## Features

**Product Management:**
- Products with multiple variants (size, color, etc.)
- Multiple product images with gallery display
- Complex category hierarchy
- Real-time inventory tracking
- Product reviews and ratings
- Discount and promotional pricing display

**User System:**
- JWT authentication with refresh tokens
- User profiles and address management
- Role-based permissions

**E-commerce Functionality:**
- Shopping cart with variant selection
- Order processing and tracking
- Advanced product filtering and search
- Category-based filter display (shows relevant filters per category)

**Technical Features:**
- Global error handling
- Input validation at multiple layers
- Soft deletes for data preservation
- Pagination for large datasets

## 📁 **Project Structure**

```
Asp.Net9.Ecommerce/
├── src/
│   ├── Asp.Net9.Ecommerce.API/          # Web API layer
│   │   ├── Controllers/                  # API controllers
│   │   ├── Middleware/                   # Custom middleware
│   │   └── Extensions/                   # Service extensions
│   ├── Asp.Net9.Ecommerce.Application/  # Business logic layer
│   │   ├── Catalog/                      # Product domain services
│   │   ├── Authentication/               # Auth services
│   │   ├── Orders/                       # Order processing
│   │   └── Common/                       # Shared components
│   ├── Asp.Net9.Ecommerce.Domain/       # Core domain models
│   │   ├── Catalog/                      # Product entities
│   │   ├── Identity/                     # User entities
│   │   ├── Orders/                       # Order entities
│   │   └── Common/                       # Base entities
│   ├── Asp.Net9.Ecommerce.Infrastructure/ # Data & external services
│   │   ├── Persistence/                  # EF Core configurations
│   │   ├── Identity/                     # Identity services
│   │   └── Repositories/                 # Data access
│   └── Asp.Net9.Ecommerce.Shared/       # Cross-cutting concerns
├── client/
│   └── ecommerce/                        # Next.js frontend
│       ├── src/
│       │   ├── app/                      # App router pages
│       │   ├── components/               # React components
│       │   ├── context/                  # State management
│       │   ├── hooks/                    # Custom hooks
│       │   └── services/                 # API services
└── tests/                                # Test projects
```

## 🏗️ **Domain Models**

### **Product Domain**
```csharp
// Rich domain models with encapsulated business logic
public class Product : AggregateRoot
{
    public string Name { get; private set; }
    public decimal BasePrice { get; private set; }
    public ICollection<ProductVariant> Variants { get; private set; }
    public ICollection<VariationType> VariantTypes { get; private set; }
    
    public Result AddVariant(ProductVariant variant) { /* Business logic */ }
    public Result UpdatePricing(decimal basePrice) { /* Validation */ }
}
```

### **User Domain**
```csharp
// Domain-driven user management
public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public ICollection<RefreshToken> RefreshTokens { get; private set; }
    
    public Result AddRefreshToken(string token, DateTime expiry) { /* Logic */ }
    public Result RevokeAllTokens(string reason) { /* Security */ }
}
```

## 🔐 **Security Implementation**

### **JWT Authentication**
- Access tokens with short expiration (15 minutes)
- Refresh tokens with secure rotation
- Automatic token cleanup and revocation
- Role-based authorization

### **Data Protection**
- Password hashing with ASP.NET Identity
- Environment-based configuration
- Secure cookie settings
- CORS configuration

## 📊 **Database Design**

### **Key Relationships**
- **Users ↔ Orders** - One-to-many with order history
- **Products ↔ Categories** - Many-to-one with hierarchical categories
- **Products ↔ Variants** - One-to-many with complex variant options
- **Categories ↔ VariationTypes** - Many-to-many for flexible product attributes

### **Advanced Features**
- **Soft Deletes** - Preserve data integrity
- **Audit Fields** - Track creation and modification
- **Complex Indexing** - Optimized query performance
- **Referential Integrity** - Proper foreign key constraints

## Getting Started

### Prerequisites
- .NET 9 SDK
- Node.js 18+
- PostgreSQL

### Setup

1. Clone the repository:
```bash
git clone https://github.com/yourusername/Asp.Net9.Ecommerce.git
cd Asp.Net9.Ecommerce
```

2. Backend setup:
```bash
# Restore packages
dotnet restore

# Update connection string in appsettings.json
# Run migrations
dotnet ef database update -p src/Asp.Net9.Ecommerce.Infrastructure -s src/Asp.Net9.Ecommerce.API

# Start the API
cd src/Asp.Net9.Ecommerce.API
dotnet run
```

3. Frontend setup:
```bash
cd client/ecommerce
npm install
npm run dev
```

### **Environment Variables**

**Backend (appsettings.json)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-postgresql-connection-string",
    "IdentityConnection": "your-identity-connection-string"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "your-issuer",
    "Audience": "your-audience"
  }
}
```

**Frontend (.env.local)**
```
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_APP_URL=http://localhost:3000
```

## 🧪 **Testing**

```bash
# Run unit tests
dotnet test

# Run integration tests
dotnet test tests/Asp.Net9.Ecommerce.IntegrationTests/
```

## 📚 **API Documentation**

The API includes comprehensive Swagger documentation available at:
- Development: `https://localhost:5001/swagger`
- Production: `https://your-api-url/swagger`

### **Key Endpoints**
```
GET    /api/products              # Get products with filtering
GET    /api/products/{id}         # Get product details
GET    /api/categories            # Get category tree
POST   /api/auth/login           # User authentication
POST   /api/auth/refresh         # Token refresh
GET    /api/orders               # Get user orders
POST   /api/orders               # Create new order
```

## 🚢 **Deployment**

### **Frontend (Vercel)**
```bash
# Deploy to Vercel
vercel --prod
```

### **Backend (Railway/Heroku)**
```bash
# Build and deploy
docker build -t ecommerce-api .
# Deploy using your preferred cloud provider
```

## What I Learned

Building this project taught me a lot about:

- **Clean Architecture**: How to properly separate concerns and maintain dependency rules
- **Domain-Driven Design**: Creating rich domain models that encapsulate business logic
- **CQRS**: Separating read and write operations for better performance and maintainability
- **Advanced .NET**: Working with Entity Framework relationships, Identity, and modern C# patterns
- **Frontend State Management**: Managing complex state in React applications
- **Security**: Implementing secure authentication with JWT and refresh tokens
- **E-commerce UX**: Image galleries, dynamic filtering, and pricing displays

## Architecture Decisions

**Why Clean Architecture?**
I wanted to learn how to build maintainable, testable applications where business logic isn't coupled to frameworks or databases.

**Why CQRS?**
Separating commands and queries makes the code more readable and allows for different optimization strategies.

**Why Result Pattern?**
Instead of throwing exceptions for business rule violations, I use a Result type that makes error handling explicit and type-safe.

## 🤝 **Contributing**

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 **License**

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## About This Project

I built this e-commerce platform as a learning project to understand enterprise-level .NET development. The goal was to implement modern architectural patterns and best practices that I'd encounter in a professional environment.

This project demonstrates:
- Clean Architecture implementation
- Domain-Driven Design principles
- CQRS pattern with MediatR
- Modern .NET development with ASP.NET Core 9
- Full-stack development with React/Next.js
- Security best practices with JWT authentication

The complexity goes beyond typical tutorial projects - it includes real-world challenges like product variants, complex filtering, user management, and proper error handling.

## 🙏 **Acknowledgments**

- Clean Architecture principles by Robert C. Martin
- Domain-Driven Design concepts by Eric Evans
- CQRS pattern implementations
- ASP.NET Core community and documentation

---

Built with ASP.NET Core 9 and Next.js
