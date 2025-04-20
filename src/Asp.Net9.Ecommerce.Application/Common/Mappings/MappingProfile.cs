using AutoMapper;
using System.Reflection;

namespace Asp.Net9.Ecommerce.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var mapFromType = typeof(IMapFrom<>);
            
            var mappingMethodName = nameof(IMapFrom<object>.Mapping);

            bool HasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;
            
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(HasInterface))
                .ToList();
            
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod(mappingMethodName) 
                    ?? type.GetInterface(mapFromType.Name)?.GetMethod(mappingMethodName);
                
                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
} 