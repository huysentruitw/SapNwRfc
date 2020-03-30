using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Fields
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Reflection use")]
    internal sealed class TimeField : Field<TimeSpan?>
    {
        private static readonly string ZeroRfcTimeString = new string('0', 6);
        private static readonly string EmptyRfcTimeString = new string(' ', 6);

        public TimeField(string name, TimeSpan? value)
            : base(name, value)
        {
        }

        public override void Apply(RfcInterop interop, IntPtr dataHandle)
        {
            string stringValue = Value?.ToString("hhmmss") ?? ZeroRfcTimeString;

            RfcResultCode resultCode = interop.SetTime(
                dataHandle: dataHandle,
                name: Name,
                time: stringValue.ToCharArray(),
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

            var timeString = new string(buffer);

            if (timeString == EmptyRfcTimeString || timeString == ZeroRfcTimeString)
                return new TimeField(name, null);

            Match match = Regex.Match(timeString, "^(?<Hours>[0-9]{2})(?<Minutes>[0-9]{2})(?<Seconds>[0-9]{2})$");
            if (!match.Success)
                return new TimeField(name, null);

            int hours = int.Parse(match.Groups["Hours"].Value);
            int minutes = int.Parse(match.Groups["Minutes"].Value);
            int seconds = int.Parse(match.Groups["Seconds"].Value);

            return new TimeField(name, new TimeSpan(hours, minutes, seconds));
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
            => $"{Name} = {Value:hh:mm:ss}";
    }
}
