# Identity Implementation Plan

## Overview
Implementation of authentication and authorization using ASP.NET Core Identity with JWT tokens.

## Current State
- [x] Base entity implemented
- [x] AppUser entity created with basic properties
- [x] Result pattern implemented in Shared project

## Implementation Steps

### Phase 1: Infrastructure Layer Setup

#### 1.0 Identity Entities
1. [ ] Create `AppRole` entity
   ```csharp
   public class AppRole : IdentityRole<Guid>
   {
       public string Description { get; private set; }
       public DateTime CreatedAt { get; private set; }
       public DateTime? UpdatedAt { get; private set; }
       
       private AppRole() { } // For EF Core

       public static AppRole Create(string name, string description)
       {
           return new AppRole
           {
               Name = name,
               Description = description,
               CreatedAt = DateTime.UtcNow,
               NormalizedName = name.ToUpper()
           };
       }

       public void Update(string description)
       {
           Description = description;
           UpdatedAt = DateTime.UtcNow;
       }
   }
   ```

2. [ ] Define default roles
   ```csharp
   public static class AppRoles
   {
       public const string Admin = "Admin";
       public const string Customer = "Customer";
       
       public static List<AppRole> DefaultRoles => new()
       {
           AppRole.Create(Admin, "Full access to all features"),
           AppRole.Create(Customer, "Standard customer access")
       };
   }
   ```

#### 1.1 Database Setup
1. [ ] Create `AppIdentityDbContext`
   ```csharp
   public class AppIdentityDbContext : IdentityDbContext<AppUser>
   {
       // Configuration
   }
   ```
2. [ ] Add SQL Server connection string to `appsettings.json`
3. [ ] Add initial migration
4. [ ] Configure Identity options (password policy, lockout settings)

#### 1.2 JWT Setup
1. [ ] Add JWT configuration to `appsettings.json`
   ```json
   {
     "JwtSettings": {
       "SecretKey": "your-secret-key",
       "Issuer": "your-issuer",
       "Audience": "your-audience",
       "ExpirationInMinutes": 60
     }
   }
   ```
2. [ ] Create JWT interfaces
   ```csharp
   public interface ITokenService
   {
       Task<string> GenerateTokenAsync(AppUser user);
       Task<Result<ClaimsPrincipal>> ValidateTokenAsync(string token);
   }
   ```
3. [ ] Implement JWT service
4. [ ] Configure JWT authentication middleware

### Phase 2: Application Layer Setup

#### 2.1 Authentication Commands
1. [ ] Implement `LoginCommand`
   ```csharp
   public class LoginCommand : IRequest<Result<string>>
   {
       public string Email { get; private set; }
       public string Password { get; private set; }
   }
   ```
2. [ ] Implement `RegisterCommand`
   ```csharp
   public class RegisterCommand : IRequest<Result<Guid>>
   {
       public string Email { get; private set; }
       public string Password { get; private set; }
   }
   ```

#### 2.2 Command Handlers
1. [ ] Implement `LoginCommandHandler`
2. [ ] Implement `RegisterCommandHandler`
3. [ ] Add validation using FluentValidation

### Phase 3: API Layer Setup

#### 3.1 Auth Controller
1. [ ] Create `AuthController`
   ```csharp
   [Route("api/[controller]")]
   public class AuthController : ControllerBase
   {
       [HttpPost("login")]
       public Task<IActionResult> Login([FromBody] LoginCommand command);

       [HttpPost("register")]
       public Task<IActionResult> Register([FromBody] RegisterCommand command);
   }
   ```

#### 3.2 Authentication Middleware
1. [ ] Configure authentication in `Program.cs`
2. [ ] Add authorization policies
3. [ ] Configure CORS if needed

## Security Configuration

### Password Requirements
```csharp
services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
});
```

### JWT Configuration
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // JWT bearer options
    });
```

## Implementation Order
1. Infrastructure Layer
   - Database first
   - Then JWT services
2. Application Layer
   - Commands
   - Handlers
3. API Layer
   - Controllers
   - Middleware

## Future Considerations (Not in Initial Implementation)
- [ ] Refresh tokens
- [ ] Password reset
- [ ] Email verification
- [ ] User roles and permissions
- [ ] Two-factor authentication
- [ ] Social login providers 