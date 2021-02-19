using System;
using SapNwRfc.Internal;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Represents an SAP RFC function.
    /// </summary>
    public sealed class SapFunction : ISapFunction
    {
        private readonly RfcInterop _interop;
        private readonly IntPtr _rfcConnectionHandle;
        private readonly IntPtr _functionHandle;

        private SapFunction(
            RfcInterop interop,
            IntPtr rfcConnectionHandle,
            IntPtr functionHandle)
        {
            _interop = interop;
            _rfcConnectionHandle = rfcConnectionHandle;
            _functionHandle = functionHandle;
        }

        internal static ISapFunction CreateFromDescriptionHandle(
            RfcInterop interop,
            IntPtr rfcConnectionHandle,
            IntPtr functionDescriptionHandle)
        {
            IntPtr functionHandle = interop.CreateFunction(
                funcDescHandle: functionDescriptionHandle,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return new SapFunction(
                interop: interop,
                rfcConnectionHandle: rfcConnectionHandle,
                functionHandle: functionHandle);
        }

        /// <summary>
        /// Disposes the SAP RFC function. Disposing automatically frees the underlying resource tied to this remote function.
        /// </summary>
        public void Dispose()
        {
            RfcResultCode resultCode = _interop.DestroyFunction(
                funcHandle: _functionHandle,
                out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);
        }

        /// <inheritdoc cref="ISapFunction"/>
        public void Invoke()
        {
            RfcResultCode resultCode = _interop.Invoke(
                rfcHandle: _rfcConnectionHandle,
                funcHandle: _functionHandle,
                out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);
        }

        /// <inheritdoc cref="ISapFunction"/>
        public void Invoke(object input)
        {
            InputMapper.Apply(_interop, _functionHandle, input);

            Invoke();
        }

        /// <inheritdoc cref="ISapFunction"/>
        public TOutput Invoke<TOutput>()
        {
            Invoke();

            return OutputMapper.Extract<TOutput>(_interop, _functionHandle);
        }

        /// <inheritdoc cref="ISapFunction"/>
        public TOutput Invoke<TOutput>(object input)
        {
            Invoke(input);

            return OutputMapper.Extract<TOutput>(_interop, _functionHandle);
        }
    }
}
