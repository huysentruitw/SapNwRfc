using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SapNwRfc.Internal.Interop
{
    internal static class SapConnectionParametersExtensions
    {
        private static readonly ConcurrentDictionary<Type, (string Name, Func<object, string> GetValue)[]> TypePropertiesCache =
            new ConcurrentDictionary<Type, (string Name, Func<object, string> GetValue)[]>();

        public static RfcConnectionParameter[] ToInterop<TParameters>(this TParameters parameters)
            where TParameters : SapConnectionParameters
        {
            (string Name, Func<object, string> GetValue)[] properties =
                TypePropertiesCache.GetOrAdd(typeof(TParameters), Build);

            return properties
                .Select(property =>
                    new RfcConnectionParameter
                    {
                        Name = property.Name,
                        Value = property.GetValue(parameters),
                    })
                .Where(parameter => !string.IsNullOrEmpty(parameter.Value))
                .ToArray();
        }

        private static (string Name, Func<object, string> GetValue)[] Build(Type type)
            => type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(propertyInfo =>
                {
                    SapNameAttribute nameAttribute = propertyInfo.GetCustomAttribute<SapNameAttribute>();

                    ParameterExpression instanceParameter = Expression.Parameter(typeof(object));
                    var propertyValueResolver = Expression.Lambda<Func<object, string>>(
                        Expression.Property(Expression.Convert(instanceParameter, type), propertyInfo),
                        instanceParameter);

                    return (
                        Name: nameAttribute?.Name ?? propertyInfo.Name.ToUpper(),
                        GetValue: propertyValueResolver.Compile()
                    );
                })
                .ToArray();
    }
}
