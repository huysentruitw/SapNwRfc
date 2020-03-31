using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SapNwRfc.Exceptions;
using SapNwRfc.Internal.Fields;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal
{
    internal static class InputMapper
    {
        private static readonly Lazy<MethodInfo> RfcFieldApplyMethod = new Lazy<MethodInfo>(GetFieldApplyMethod);

        private static readonly ConcurrentDictionary<Type, Action<RfcInterop, IntPtr, object>> ApplyActionsCache =
            new ConcurrentDictionary<Type, Action<RfcInterop, IntPtr, object>>();

        public static void Apply(RfcInterop interop, IntPtr dataHandle, object input)
        {
            if (input == null)
                return;

            Type inputType = input.GetType();
            Action<RfcInterop, IntPtr, object> applyAction = ApplyActionsCache.GetOrAdd(inputType, BuildApplyAction);
            applyAction(interop, dataHandle, input);
        }

        private static MethodInfo GetFieldApplyMethod()
            => typeof(IField).GetMethod(nameof(IField.Apply), new[] { typeof(RfcInterop), typeof(IntPtr) })
               ?? throw new SapException($"{nameof(IField)}.{nameof(IField.Apply)} not found");

        private static Action<RfcInterop, IntPtr, object> BuildApplyAction(Type type)
        {
            ParameterExpression interopParameter = Expression.Parameter(typeof(RfcInterop));
            ParameterExpression dataHandleParameter = Expression.Parameter(typeof(IntPtr));
            ParameterExpression inputParameter = Expression.Parameter(typeof(object));
            UnaryExpression castedInputParameter = Expression.Convert(inputParameter, type);

            Expression[] applyExpressionsForProperties = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(propertyInfo => BuildApplyExpressionForProperty(
                    propertyInfo: propertyInfo,
                    interopParameter: interopParameter,
                    dataHandleParameter: dataHandleParameter,
                    inputParameter: castedInputParameter))
                .Where(x => x != null)
                .ToArray();

            var expression = Expression.Lambda<Action<RfcInterop, IntPtr, object>>(
                Expression.Block(applyExpressionsForProperties),
                interopParameter,
                dataHandleParameter,
                inputParameter);

            return expression.Compile();
        }

        private static Expression BuildApplyExpressionForProperty(
            PropertyInfo propertyInfo,
            Expression interopParameter,
            Expression dataHandleParameter,
            Expression inputParameter)
        {
            SapNameAttribute nameAttribute = propertyInfo.GetCustomAttribute<SapNameAttribute>();
            ConstantExpression name = Expression.Constant(nameAttribute?.Name ?? propertyInfo.Name.ToUpper());

            // var value = propertyInfo.GetValue(input);
            Expression property = Expression.Property(inputParameter, propertyInfo);

            NewExpression fieldNewExpression = null;
            if (propertyInfo.PropertyType == typeof(string))
            {
                // new RfcStringField(name, (string)value);
                ConstructorInfo constructor =
                    typeof(StringField).GetConstructor(new[] { typeof(string), typeof(string) })
                    ?? throw new InvalidOperationException("Constructor not found");

                fieldNewExpression = Expression.New(constructor, name, property);
            }
            else if (propertyInfo.PropertyType == typeof(int))
            {
                // new RfcIntField(name, (int)value);
                ConstructorInfo constructor =
                    typeof(IntField).GetConstructor(new[] { typeof(string), typeof(int) })
                    ?? throw new InvalidOperationException("Constructor not found");

                fieldNewExpression = Expression.New(constructor, name, property);
            }
            else if (propertyInfo.PropertyType == typeof(long))
            {
                // new RfcLongField(name, (long)value);
                ConstructorInfo constructor =
                    typeof(LongField).GetConstructor(new[] { typeof(string), typeof(long) })
                    ?? throw new InvalidOperationException("Constructor not found");

                fieldNewExpression = Expression.New(constructor, name, property);
            }
            else if (propertyInfo.PropertyType == typeof(double))
            {
                // new RfcDoubleField(name, (double)value);
                ConstructorInfo constructor =
                    typeof(DoubleField).GetConstructor(new[] { typeof(string), typeof(double) })
                    ?? throw new InvalidOperationException("Constructor not found");

                fieldNewExpression = Expression.New(constructor, name, property);
            }
            else if (propertyInfo.PropertyType == typeof(decimal))
            {
                // new RfcDecimalField(name, (decimal)value);
                ConstructorInfo constructor =
                    typeof(DecimalField).GetConstructor(new[] { typeof(string), typeof(decimal) })
                    ?? throw new InvalidOperationException("Constructor not found");

                fieldNewExpression = Expression.New(constructor, name, property);
            }
            else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
            {
                // new RfcDateField(name, (DateTime?)value);
                ConstructorInfo constructor =
                    typeof(DateField).GetConstructor(new[] { typeof(string), typeof(DateTime?) })
                    ?? throw new InvalidOperationException("Constructor not found");

                fieldNewExpression = Expression.New(constructor, name, Expression.Convert(property, typeof(DateTime?)));
            }
            else if (propertyInfo.PropertyType == typeof(TimeSpan) || propertyInfo.PropertyType == typeof(TimeSpan?))
            {
                // new RfcTimeField(name, (TimeSpan?)value);
                ConstructorInfo constructor =
                    typeof(TimeField).GetConstructor(new[] { typeof(string), typeof(TimeSpan?) })
                    ?? throw new InvalidOperationException("Constructor not found");

                fieldNewExpression = Expression.New(constructor, name, Expression.Convert(property, typeof(TimeSpan?)));
            }
            else if (propertyInfo.PropertyType.IsArray)
            {
                // new RfcTableField<TElementType>(name, (TElementType[])value);
                Type tableFieldType =
                    typeof(TableField<>).MakeGenericType(propertyInfo.PropertyType.GetElementType());

                ConstructorInfo constructor =
                    tableFieldType.GetConstructor(new[] { typeof(string), propertyInfo.PropertyType })
                    ?? throw new InvalidOperationException("Constructor not found");

                fieldNewExpression = Expression.New(constructor, name, property);
            }
            else
            {
                // new RfcStructureField<T>(name, (T)value);
                Type structureFieldType =
                    typeof(StructureField<>).MakeGenericType(propertyInfo.PropertyType);

                ConstructorInfo constructor =
                    structureFieldType.GetConstructor(new[] { typeof(string), propertyInfo.PropertyType })
                    ?? throw new InvalidOperationException("Constructor not found");

                fieldNewExpression = Expression.New(constructor, name, property);
            }

            // instance.Apply(interopParameter, dataHandleParameter);
            return Expression.Call(
                instance: fieldNewExpression,
                method: RfcFieldApplyMethod.Value,
                arguments: new[] { interopParameter, dataHandleParameter });
        }
    }
}
