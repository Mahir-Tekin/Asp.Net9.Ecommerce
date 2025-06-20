namespace Asp.Net9.Ecommerce.Shared.Results
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public ErrorType Type { get; set; }
        public List<ValidationError> Errors { get; set; }

        public ErrorResponse(string message, string code, ErrorType type)
        {
            Message = message;
            Code = code;
            Type = type;
            Errors = new List<ValidationError>();
        }

        public ErrorResponse(string message, string code, ErrorType type, List<ValidationError> errors)
            : this(message, code, type)
        {
            Errors = errors;
        }

        public static ErrorResponse ValidationError(List<ValidationError> errors)
        {
            return new ErrorResponse(
                "One or more validation errors occurred",
                "VALIDATION_ERROR",
                ErrorType.Validation,
                errors);
        }

        public static ErrorResponse NotFound(string message = "Resource not found")
        {
            return new ErrorResponse(message, "NOT_FOUND", ErrorType.NotFound);
        }

        public static ErrorResponse Unauthorized(string message = "Unauthorized access")
        {
            return new ErrorResponse(message, "UNAUTHORIZED", ErrorType.Unauthorized);
        }

        public static ErrorResponse Forbidden(string message = "Access forbidden")
        {
            return new ErrorResponse(message, "FORBIDDEN", ErrorType.Forbidden);
        }

        public static ErrorResponse General(string message, string code = "GENERAL_ERROR")
        {
            return new ErrorResponse(message, code, ErrorType.General);
        }

        public static ErrorResponse Conflict(string message = "A conflict occurred with an existing resource")
        {
            return new ErrorResponse(message, "CONFLICT", ErrorType.Conflict);
        }

        public static ErrorResponse Internal(string message = "An internal error occurred")
        {
            return new ErrorResponse(message, "INTERNAL_ERROR", ErrorType.Internal);
        }
    }
} 