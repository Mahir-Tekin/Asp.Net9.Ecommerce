using FluentValidation;
using MediatR;
using Asp.Net9.Ecommerce.Shared.Results;
using System.Reflection;

namespace Asp.Net9.Ecommerce.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                var errorMessages = failures
                    .Select(x => $"{x.PropertyName}: {x.ErrorMessage}")
                    .ToList();

                var errorMessage = string.Join(", ", errorMessages);
                
                var responseType = typeof(TResponse);

                // Handle Result<T>
                if (responseType.IsGenericType && 
                    responseType.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var resultType = responseType.GetGenericArguments()[0];
                    
                    // Find the specific Failure method that returns Result<T>
                    var failureMethod = typeof(Result).GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(m => 
                            m.Name == nameof(Result.Failure) && 
                            m.IsGenericMethod &&
                            m.ReturnType.IsGenericType &&
                            m.ReturnType.GetGenericTypeDefinition() == typeof(Result<>));

                    if (failureMethod != null)
                    {
                        var genericMethod = failureMethod.MakeGenericMethod(resultType);
                        return genericMethod.Invoke(null, new object[] { errorMessage }) as TResponse;
                    }
                }

                // Handle Result
                if (responseType == typeof(Result))
                {
                    return Result.Failure(errorMessage) as TResponse;
                }

                // For non-Result responses, throw validation exception
                throw new ValidationException(failures);
            }

            return await next();
        }
    }
} 