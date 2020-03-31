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
        private static readonly ConcurrentDictionary<Type, MethodInfo> FieldExtractMethodsCache =
            new ConcurrentDictionary<Type, MethodInfo>();

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
                extractMethod = FieldExtractMethodsCache
                    .GetOrAdd(typeof(StringField), _ => GetFieldExtractMethod(() => StringField.Extract(default(RfcInterop), default(IntPtr), default(string))));
            }
            else if (propertyInfo.PropertyType == typeof(int))
            {
                extractMethod = FieldExtractMethodsCache
                    .GetOrAdd(typeof(IntField), _ => GetFieldExtractMethod(() => IntField.Extract(default(RfcInterop), default(IntPtr), default(string))));
            }
            else if (propertyInfo.PropertyType == typeof(long))
            {
                extractMethod = FieldExtractMethodsCache
                    .GetOrAdd(typeof(LongField), _ => GetFieldExtractMethod(() => LongField.Extract(default(RfcInterop), default(IntPtr), default(string))));
            }
            else if (propertyInfo.PropertyType == typeof(double))
            {
                extractMethod = FieldExtractMethodsCache
                    .GetOrAdd(typeof(DoubleField), _ => GetFieldExtractMethod(() => DoubleField.Extract(default(RfcInterop), default(IntPtr), default(string))));
            }
            else if (propertyInfo.PropertyType == typeof(decimal))
            {
                extractMethod = FieldExtractMethodsCache
                    .GetOrAdd(typeof(DecimalField), _ => GetFieldExtractMethod(() => DecimalField.Extract(default(RfcInterop), default(IntPtr), default(string))));
            }
            else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
            {
                convertToNonNullable = propertyInfo.PropertyType == typeof(DateTime);
                extractMethod = FieldExtractMethodsCache
                    .GetOrAdd(typeof(DateField), _ => GetFieldExtractMethod(() => DateField.Extract(default(RfcInterop), default(IntPtr), default(string))));
            }
            else if (propertyInfo.PropertyType == typeof(TimeSpan) || propertyInfo.PropertyType == typeof(TimeSpan?))
            {
                convertToNonNullable = propertyInfo.PropertyType == typeof(TimeSpan);
                extractMethod = FieldExtractMethodsCache
                    .GetOrAdd(typeof(TimeField), _ => GetFieldExtractMethod(() => TimeField.Extract(default(RfcInterop), default(IntPtr), default(string))));
            }
            else if (propertyInfo.PropertyType.IsArray)
            {
                Type elementType = propertyInfo.PropertyType.GetElementType();
                Type tableFieldType = typeof(TableField<>).MakeGenericType(elementType);
                extractMethod = FieldExtractMethodsCache
                    .GetOrAdd(tableFieldType, _ => GetFieldExtractMethod(() => TableField<object>.Extract<object>(default(RfcInterop), default(IntPtr), default(string)))
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(elementType));
            }
            else if (!propertyInfo.PropertyType.IsPrimitive)
            {
                Type structureFieldType = typeof(StructureField<>).MakeGenericType(propertyInfo.PropertyType);
                extractMethod = FieldExtractMethodsCache
                    .GetOrAdd(structureFieldType, _ => GetFieldExtractMethod(() => StructureField<object>.Extract<object>(default(RfcInterop), default(IntPtr), default(string)))
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(propertyInfo.PropertyType));
            }

            if (extractMethod == null)
                throw new InvalidOperationException($"No matching extract method found for type {propertyInfo.PropertyType.Name}");

            // ReSharper disable once PossibleNullReferenceException
            PropertyInfo fieldValueProperty = extractMethod.ReturnType.GetProperty(nameof(Field<object>.Value));

            MemberExpression fieldValue = Expression.Property(
                Expression.Call(
                    method: extractMethod,
                    arguments: new[] { interop, dataHandle, name }),
                fieldValueProperty);

            return convertToNonNullable
                ? Expression.Assign(property, Expression.Coalesce(
                    left: fieldValue,
                    right: Expression.Default(propertyInfo.PropertyType)))
                : Expression.Assign(property, fieldValue);
        }

        private static MethodInfo GetFieldExtractMethod(Expression<Action> extractMethod)
            => ((MethodCallExpression)extractMethod.Body).Method;
    }
}
