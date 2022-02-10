using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Fields
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Reflection use")]
    internal sealed class TimeField : Field<TimeSpan?>
    {
        private static readonly string ZeroRfcTimeString = new string('0', 6);
        private static readonly string EmptyRfcTimeString = new string(' ', 6);
        private static readonly string RfcTimeFormat = "hhmmss";

        public TimeField(string name, TimeSpan? value)
            : base(name, value)
        {
        }

        public override void Apply(RfcInterop interop, IntPtr dataHandle)
        {
            char[] buffer = (Value?.ToString(RfcTimeFormat, CultureInfo.InvariantCulture) ?? ZeroRfcTimeString).ToCharArray();

            RfcResultCode resultCode = interop.SetTime(
                dataHandle: dataHandle,
                name: Name,
                time: buffer,
                out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);
        }

        public static TimeField Extract(RfcInterop interop, IntPtr dataHandle, string name)
        {
            char[] buffer = EmptyRfcTimeString.ToCharArray();

            RfcResultCode resultCode = interop.GetTime(
                dataHandle: dataHandle,
                name: name,
                emptyTime: buffer,
                out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            string timeString = new string(buffer);

            if (timeString == EmptyRfcTimeString || timeString == ZeroRfcTimeString)
                return new TimeField(name, null);

            if (!TimeSpan.TryParseExact(timeString, RfcTimeFormat, CultureInfo.InvariantCulture, out TimeSpan time))
                return new TimeField(name, null);

            return new TimeField(name, time);
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
            => $"{Name} = {Value:hh:mm:ss}";
    }
}
