using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SapNwRfc.Internal.Fields;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal
{
    internal static class OutputMapper
    {
        private static readonly ConcurrentDictionary<Type, Func<RfcInterop, IntPtr, object>> ExtractFuncsCache =
            new ConcurrentDictionary<Type, Func<RfcInterop, IntPtr, object>>();

        public static TOutput Extract<TOutput>(RfcInterop interop, IntPtr dataHandle)
        {
            Type outputType = typeof(TOutput);
            Func<RfcInterop, IntPtr, object> extractFunc = ExtractFuncsCache.GetOrAdd(outputType, BuildExtractFunc);
            return (TOutput)extractFunc(interop, dataHandle);
        }

        private static Func<RfcInterop, IntPtr, object> BuildExtractFunc(Type type)
        {
            ParameterExpression interop = Expression.Parameter(typeof(RfcInterop));
            ParameterExpression dataHandle = Expression.Parameter(typeof(IntPtr));
            ParameterExpression result = Expression.Variable(type);

            IEnumerable<Expression> extractExpressionsForProperties = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(propertyInfo => BuildExtractExpressionForProperty(
                    propertyInfo: propertyInfo,
                    interop: interop,
                    dataHandle: dataHandle,
                    result: result))
                .Where(x => x != null);

            Expression[] body = Array.Empty<Expression>()
                .Concat(new[] { Expression.Assign(result, Expression.New(type)) })
                .Concat(extractExpressionsForProperties)
                .Concat(new[] { result })
                .ToArray();

            var expression = Expression.Lambda<Func<RfcInterop, IntPtr, object>>(
                body: Expression.Block(
                    variables: new[] { result },
                    expressions: body),
                parameters: new[] { interop, dataHandle });

            return expression.Compile();
        }

        private static Expression BuildExtractExpressionForProperty(
            PropertyInfo propertyInfo,
            Expression interop,
            Expression dataHandle,
            Expression result)
        {
            SapNameAttribute nameAttribute = propertyInfo.GetCustomAttribute<SapNameAttribute>();
            ConstantExpression name = Expression.Constant(nameAttribute?.Name ?? propertyInfo.Name.ToUpper());

            Expression property = Expression.Property(result, propertyInfo);

            bool convertToNonNullable = false;
            MethodInfo extractMethod = null;
            if (propertyInfo.PropertyType == typeof(string))
            {
                extractMethod = typeof(StringField)
                    .GetMethod(nameof(StringField.Extract), new[] { typeof(RfcInterop), typeof(IntPtr), typeof(string) });
            }
            else if (propertyInfo.PropertyType == typeof(int))
            {
                extractMethod = typeof(IntField)
                    .GetMethod(nameof(IntField.Extract), new[] { typeof(RfcInterop), typeof(IntPtr), typeof(string) });
            }
            else if (propertyInfo.PropertyType == typeof(long))
            {
                extractMethod = typeof(LongField)
                    .GetMethod(nameof(LongField.Extract), new[] { typeof(RfcInterop), typeof(IntPtr), typeof(string) });
            }
            else if (propertyInfo.PropertyType == typeof(double))
            {
                extractMethod = typeof(DoubleField)
                    .GetMethod(nameof(DoubleField.Extract), new[] { typeof(RfcInterop), typeof(IntPtr), typeof(string) });
            }
            else if (propertyInfo.PropertyType == typeof(decimal))
            {
                extractMethod = typeof(DecimalField)
                    .GetMethod(nameof(DecimalField.Extract), new[] { typeof(RfcInterop), typeof(IntPtr), typeof(string) });
            }
            else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
            {
                convertToNonNullable = propertyInfo.PropertyType == typeof(DateTime);
                extractMethod = typeof(DateField)
                    .GetMethod(nameof(DateField.Extract), new[] { typeof(RfcInterop), typeof(IntPtr), typeof(string) });
            }
            else if (propertyInfo.PropertyType == typeof(TimeSpan) || propertyInfo.PropertyType == typeof(TimeSpan?))
            {
                convertToNonNullable = propertyInfo.PropertyType == typeof(TimeSpan);
                extractMethod = typeof(TimeField)
                    .GetMethod(nameof(TimeField.Extract), new[] { typeof(RfcInterop), typeof(IntPtr), typeof(string) });
            }
            else if (propertyInfo.PropertyType.IsArray)
            {
                Type elementType = propertyInfo.PropertyType.GetElementType();
                extractMethod = typeof(TableField<>)
                    .MakeGenericType(elementType)
                    .GetMethod(
                        name: nameof(TableField<object>.Extract),
                        types: new[] { typeof(RfcInterop), typeof(IntPtr), typeof(string) })
                    ?.MakeGenericMethod(elementType);
            }
            else
            {
                extractMethod = typeof(StructureField<>)
                    .MakeGenericType(propertyInfo.PropertyType)
                    .GetMethod(
                        name: nameof(StructureField<object>.Extract),
                        types: new[] { typeof(RfcInterop), typeof(IntPtr), typeof(string) })
                    ?.MakeGenericMethod(propertyInfo.PropertyType);
            }

            // ReSharper disable once PossibleNullReferenceException
            PropertyInfo rfcFieldValueProperty =
                extractMethod.ReturnType.GetProperty("Value")
                ?? throw new InvalidOperationException($"Value property not found on type {extractMethod.ReturnType.Name}");

            MemberExpression rfcFieldValue = Expression.Property(
                Expression.Call(
                    method: extractMethod,
                    arguments: new[] { interop, dataHandle, name }),
                rfcFieldValueProperty);

            return convertToNonNullable
                ? Expression.Assign(property, Expression.Coalesce(
                    left: rfcFieldValue,
                    right: Expression.Default(propertyInfo.PropertyType)))
                : Expression.Assign(property, rfcFieldValue);
        }
    }
}
