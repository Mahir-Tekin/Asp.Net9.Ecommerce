namespace Asp.Net9.Ecommerce.Shared.Results
{
    public enum ErrorType
    {
        Validation,
        NotFound,
        Unauthorized,
        Forbidden,
        General,
        Conflict,    // For duplicate entries, etc.
        Internal     // For system/internal errors
    }
} 