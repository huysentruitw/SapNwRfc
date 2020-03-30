using System;
using SapNwRfc.Internal;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
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

        public void Dispose()
        {
            RfcResultCode resultCode = _interop.DestroyFunction(
                funcHandle: _functionHandle,
                out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);
        }

        public void Invoke()
        {
            RfcResultCode resultCode = _interop.Invoke(
                rfcHandle: _rfcConnectionHandle,
                funcHandle: _functionHandle,
                out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);
        }

        public void Invoke(object input)
        {
            InputMapper.Apply(_interop, _functionHandle, input);

            Invoke();
        }

        public TOutput Invoke<TOutput>()
        {
            Invoke();

            return OutputMapper.Extract<TOutput>(_interop, _functionHandle);
        }

        public TOutput Invoke<TOutput>(object input)
        {
            Invoke(input);

            return OutputMapper.Extract<TOutput>(_interop, _functionHandle);
        }
    }

    public interface ISapFunction : IDisposable
    {
        void Invoke();

        void Invoke(object input);

        TOutput Invoke<TOutput>();

        TOutput Invoke<TOutput>(object input);
    }
}
