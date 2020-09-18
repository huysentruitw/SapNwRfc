using System;
using System.Diagnostics.CodeAnalysis;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Fields
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Reflection use")]
    internal sealed class BytesField : Field<byte[]>
    {
        public BytesField(string name, byte[] value)
            : base(name, value)
        {
        }

        public override void Apply(RfcInterop interop, IntPtr dataHandle)
        {
            if (Value == null)
                return;

            RfcResultCode resultCode = interop.SetBytes(
                dataHandle: dataHandle,
                name: Name,
                value: Value,
                valueLength: (uint)Value.Length,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);
        }

        public static BytesField Extract(RfcInterop interop, IntPtr dataHandle, string name, int bufferLength)
        {
            var buffer = new byte[bufferLength];

            RfcResultCode resultCode = interop.GetBytes(
                dataHandle: dataHandle,
                name: name,
                bytesBuffer: buffer,
                bufferLength: (uint)buffer.Length,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            return new BytesField(name, buffer);
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
            => $"{Name} = {Value}";
    }
}
