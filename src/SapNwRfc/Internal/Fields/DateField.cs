using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Fields
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Reflection use")]
    internal sealed class DateField : Field<DateTime?>
    {
        private static readonly string ZeroRfcDateString = new string('0', 8);
        private static readonly string EmptyRfcDateString = new string(' ', 8);
        private static readonly string RfcDateFormat = "yyyyMMdd";

        public DateField(string name, DateTime? value)
            : base(name, value)
        {
        }

        public override void Apply(RfcInterop interop, IntPtr dataHandle)
        {
#if NETSTANDARD2_0
            char[] buffer = (Value?.ToString(RfcDateFormat, CultureInfo.InvariantCulture) ?? ZeroRfcDateString).ToCharArray();
#else
            char[] buffer = ZeroRfcDateString.ToCharArray();
            Value?.TryFormat(buffer, out var _, RfcDateFormat, CultureInfo.InvariantCulture);
#endif

            RfcResultCode resultCode = interop.SetDate(
                dataHandle: dataHandle,
                name: Name,
                date: buffer,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);
        }

        public static DateField Extract(RfcInterop interop, IntPtr dataHandle, string name)
        {
            char[] buffer = EmptyRfcDateString.ToCharArray();

            RfcResultCode resultCode = interop.GetDate(
                dataHandle: dataHandle,
                name: name,
                emptyDate: buffer,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

#if NETSTANDARD2_0
            string dateString = new string(buffer);

            if (dateString == EmptyRfcDateString || dateString == ZeroRfcDateString)
                return new DateField(name, null);
#else
            Span<char> dateString = buffer.AsSpan();

            if (dateString.SequenceEqual(EmptyRfcDateString) || dateString.SequenceEqual(ZeroRfcDateString))
                return new DateField(name, null);
#endif

            if (!DateTime.TryParseExact(dateString, RfcDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                return new DateField(name, null);

            return new DateField(name, date);
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
            => Value.HasValue ? $"{Name} = {Value:yyyy-MM-dd}" : $"{Name} = No date";
    }
}
