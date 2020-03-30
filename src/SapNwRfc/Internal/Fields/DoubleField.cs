using System;
using System.Diagnostics.CodeAnalysis;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Fields
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Reflection use")]
    internal sealed class DoubleField : Field<double>
    {
        public DoubleField(string name, double value)
            : base(name, value)
        {
        }

        public override void Apply(RfcInterop interop, IntPtr dataHandle)
        {
            RfcResultCode resultCode = interop.SetFloat(
                dataHandle: dataHandle,
                name: Name,
                value: Value,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);
        }

        public static DoubleField Extract(RfcInterop interop, IntPtr dataHandle, string name)
        {
            RfcResultCode resultCode = interop.GetFloat(
                dataHandle: dataHandle,
                name: name,
                out double value,
                out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            return new DoubleField(name, value);
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
            => $"{Name} = {Value}";
    }
}
