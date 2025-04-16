namespace Asp.Net9.Ecommerce.Shared.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, string.Empty);
        public static Result Failure(string error) => new(false, error);

        public static Result<T> Success<T>(T value) => new(value, true, string.Empty);
        public static Result<T> Failure<T>(string error) => new(default, false, error);

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
                action(Error);
            return this;
        }
    }

    public class Result<T> : Result
    {
        private readonly T _value;

        public T Value => IsSuccess 
            ? _value 
            : throw new InvalidOperationException($"Cannot access value of a failed result. Error: {Error}");

        protected internal Result(T value, bool isSuccess, string error) 
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