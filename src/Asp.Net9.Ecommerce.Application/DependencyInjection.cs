using System.Reflection;
using Asp.Net9.Ecommerce.Application.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace Asp.Net9.Ecommerce.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Register AutoMapper
            services.AddAutoMapper(assembly);
            
            // Register MediatR
            services.AddMediatR(cfg => 
            {
                cfg.RegisterServicesFromAssembly(assembly);
                // Register validation behavior
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            // Register all validators in assembly
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
} 