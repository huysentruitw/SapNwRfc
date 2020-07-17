using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc.Internal.Fields
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Reflection use")]
    internal sealed class StructureField<TStructure> : Field<TStructure>
    {
        public StructureField(string name, TStructure value)
            : base(name, value)
        {
        }

        public override void Apply(RfcInterop interop, IntPtr dataHandle)
        {
            RfcResultCode resultCode = interop.GetStructure(
                dataHandle: dataHandle,
                name: Name,
                structHandle: out IntPtr structHandle,
                out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            InputMapper.Apply(interop, structHandle, Value);
        }

        public static StructureField<T> Extract<T>(RfcInterop interop, IntPtr dataHandle, string name)
        {
            RfcResultCode resultCode = interop.GetStructure(
                dataHandle: dataHandle,
                name: name,
                structHandle: out IntPtr structHandle,
                out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            T structValue = OutputMapper.Extract<T>(interop, structHandle);

            var onInitialize = structValue.GetType()?.GetMethod("OnInitialize", BindingFlags.Instance);

            if (onInitialize != null)
            {
                onInitialize.Invoke(structValue, null);
            }

            return new StructureField<T>(name, structValue);
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
            => $"{Name} = {typeof(TStructure)}";
    }
}
