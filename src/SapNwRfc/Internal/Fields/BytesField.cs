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

        public static BytesField Extract(RfcInterop interop, IntPtr dataHandle, string name, int? bufferLength)
        {
            if (!bufferLength.HasValue)
                bufferLength = GetBufferLength(interop, dataHandle, name);

            if (bufferLength.Value <= 0)
                return new BytesField(name, Array.Empty<byte>());

            var buffer = new byte[bufferLength.Value];
            RfcResultCode resultCode = interop.GetXString(
                dataHandle: dataHandle,
                name: name,
                bytesBuffer: buffer,
                bufferLength: (uint)buffer.Length,
                xstringLength: out _,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            return new BytesField(name, buffer);
        }

        private static int GetBufferLength(RfcInterop interop, IntPtr dataHandle, string name)
        {
            RfcResultCode resultCode = interop.GetXString(
                dataHandle: dataHandle,
                name: name,
                bytesBuffer: Array.Empty<byte>(),
                bufferLength: 0,
                xstringLength: out uint bufferLength,
                errorInfo: out RfcErrorInfo errorInfo);

            if (resultCode != RfcResultCode.RFC_BUFFER_TOO_SMALL)
            {
                resultCode.ThrowOnError(errorInfo);
            }

            return (int)bufferLength;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
            => $"{Name} = {Value}";
    }
}
