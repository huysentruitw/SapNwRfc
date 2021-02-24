using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SapNwRfc.Internal.Fields;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal
{
    internal static class InputMapper
    {
        private static readonly Lazy<MethodInfo> FieldApplyMethod = new Lazy<MethodInfo>(GetFieldApplyMethod);

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
        {
            Expression<Action<IField>> expression = field => field.Apply(default(RfcInterop), default(IntPtr));
            return ((MethodCallExpression)expression.Body).Method;
        }

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
            // skip property from mapping
            if (Attribute.IsDefined(propertyInfo, typeof(SapIgnoreAttribute)))
            {
                return null;
            }

            SapNameAttribute nameAttribute = propertyInfo.GetCustomAttribute<SapNameAttribute>();
            ConstantExpression name = Expression.Constant(nameAttribute?.Name ?? propertyInfo.Name.ToUpper());

            // var value = propertyInfo.GetValue(input);
            Expression property = Expression.Property(inputParameter, propertyInfo);

            ConstructorInfo fieldConstructor = null;
            if (propertyInfo.PropertyType == typeof(string))
            {
                // new RfcStringField(name, (string)value);
                fieldConstructor = GetFieldConstructor(() => new StringField(default, default));
            }
            else if (propertyInfo.PropertyType == typeof(int))
            {
                // new RfcIntField(name, (int)value);
                fieldConstructor = GetFieldConstructor(() => new IntField(default, default));
            }
            else if (propertyInfo.PropertyType == typeof(long))
            {
                // new RfcLongField(name, (long)value);
                fieldConstructor = GetFieldConstructor(() => new LongField(default, default));
            }
            else if (propertyInfo.PropertyType == typeof(double))
            {
                // new RfcDoubleField(name, (double)value);
                fieldConstructor = GetFieldConstructor(() => new DoubleField(default, default));
            }
            else if (propertyInfo.PropertyType == typeof(decimal))
            {
                // new RfcDecimalField(name, (decimal)value);
                fieldConstructor = GetFieldConstructor(() => new DecimalField(default, default));
            }
            else if (propertyInfo.PropertyType == typeof(byte[]))
            {
                // new BytesField(name, value);
                fieldConstructor = GetFieldConstructor(() => new BytesField(default, default));
            }
            else if (propertyInfo.PropertyType == typeof(char[]))
            {
                // new CharsField(name, value);
                fieldConstructor = GetFieldConstructor(() => new CharsField(default, default));
            }
            else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
            {
                // new RfcDateField(name, (DateTime?)value);
                fieldConstructor = GetFieldConstructor(() => new DateField(default, default));
                property = Expression.Convert(property, typeof(DateTime?));
            }
            else if (propertyInfo.PropertyType == typeof(TimeSpan) || propertyInfo.PropertyType == typeof(TimeSpan?))
            {
                // new RfcTimeField(name, (TimeSpan?)value);
                fieldConstructor = GetFieldConstructor(() => new TimeField(default, default));
                property = Expression.Convert(property, typeof(TimeSpan?));
            }
            else if (propertyInfo.PropertyType.IsArray)
            {
                // new RfcTableField<TElementType>(name, (TElementType[])value);
                Type tableFieldType = typeof(TableField<>).MakeGenericType(propertyInfo.PropertyType.GetElementType());
                fieldConstructor = tableFieldType.GetConstructor(new[] { typeof(string), propertyInfo.PropertyType });
            }
            else if (!propertyInfo.PropertyType.IsPrimitive)
            {
                // new RfcStructureField<T>(name, (T)value);
                Type structureFieldType = typeof(StructureField<>).MakeGenericType(propertyInfo.PropertyType);
                fieldConstructor = structureFieldType.GetConstructor(new[] { typeof(string), propertyInfo.PropertyType });
            }

            NewExpression fieldNewExpression = Expression.New(
                constructor: fieldConstructor ?? throw new InvalidOperationException("No matching field constructor found"),
                name,
                property);

            // instance.Apply(interopParameter, dataHandleParameter);
            return Expression.Call(
                instance: fieldNewExpression,
                method: FieldApplyMethod.Value,
                arguments: new[] { interopParameter, dataHandleParameter });
        }

        private static ConstructorInfo GetFieldConstructor(Expression<Func<IField>> constructor)
            => ((NewExpression)constructor.Body).Constructor;
    }
}
