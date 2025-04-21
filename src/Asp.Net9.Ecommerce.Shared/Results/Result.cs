namespace Asp.Net9.Ecommerce.Shared.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public ErrorResponse Error { get; }

        protected Result(bool isSuccess, ErrorResponse error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);

        public static Result Failure(ErrorResponse error) => new(false, error);

        public static Result<T> Success<T>(T value) => new(value, true, null);

        public static Result<T> Failure<T>(ErrorResponse error) => new(default, false, error);

        // Helper methods for common failure cases
        public static Result ValidationFailure(List<ValidationError> errors)
            => Failure(ErrorResponse.ValidationError(errors));

        public static Result<T> ValidationFailure<T>(List<ValidationError> errors)
            => Failure<T>(ErrorResponse.ValidationError(errors));

        public static Result NotFound(string message = "Resource not found")
            => Failure(ErrorResponse.NotFound(message));

        public static Result<T> NotFound<T>(string message = "Resource not found")
            => Failure<T>(ErrorResponse.NotFound(message));

        // Additional helper methods
        public Result<TOut> Map<TOut>(Func<TOut> func)
        {
            return IsSuccess ? Result.Success(func()) : Result.Failure<TOut>(Error);
        }

        public Result OnSuccess(Action action)
        {
            if (IsSuccess)
                action();
            return this;
        }

        public Result OnFailure(Action<string> action)
        {
            if (IsFailure)
                action(Error.Message);
            return this;
        }
    }

    public class Result<T> : Result
    {
        private readonly T _value;

        public T Value => IsSuccess 
            ? _value 
            : throw new InvalidOperationException("Cannot access Value of failed result");

        protected internal Result(T value, bool isSuccess, ErrorResponse error) 
            : base(isSuccess, error)
        {
            _value = value;
        }

        // Additional helper methods for generic Result
        public Result<TOut> Map<TOut>(Func<T, TOut> func)
        {
            return IsSuccess ? Result.Success(func(Value)) : Result.Failure<TOut>(Error);
        }

        public Result<T> OnSuccess(Action<T> action)
        {
            if (IsSuccess)
                action(Value);
            return this;
        }
    }
} 