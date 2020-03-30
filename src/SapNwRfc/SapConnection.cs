using System;
using System.Diagnostics.CodeAnalysis;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    public sealed class SapConnection : ISapConnection
    {
        private readonly RfcInterop _interop;
        private readonly SapConnectionParameters _parameters;
        private IntPtr _rfcConnectionHandle = IntPtr.Zero;

        [ExcludeFromCodeCoverage]
        public SapConnection(SapConnectionParameters parameters)
            : this(new RfcInterop(), parameters)
        {
        }

        [ExcludeFromCodeCoverage]
        public SapConnection(string connectionString)
            : this(new RfcInterop(), SapConnectionParameters.Parse(connectionString))
        {
        }

        // Constructor for unit-testing
        internal SapConnection(RfcInterop interop, SapConnectionParameters parameters)
        {
            _interop = interop;
            _parameters = parameters;
        }

        public void Dispose()
        {
            Disconnect(disposing: true);
        }

        public void Connect()
        {
            RfcConnectionParameter[] interopParameters = _parameters.ToInterop();

            _rfcConnectionHandle = _interop.OpenConnection(
                connectionParams: interopParameters,
                paramCount: (uint)interopParameters.Length,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError(beforeThrow: Clear);
        }

        public void Disconnect()
        {
            Disconnect(disposing: false);
        }

        private void Disconnect(bool disposing)
        {
            if (_rfcConnectionHandle == IntPtr.Zero)
                return;

            RfcResultCode resultCode = _interop.CloseConnection(
                rfcHandle: _rfcConnectionHandle,
                errorInfo: out RfcErrorInfo errorInfo);

            Clear();

            if (!disposing)
                resultCode.ThrowOnError(errorInfo);
        }

        public bool IsValid
        {
            get
            {
                if (_rfcConnectionHandle == IntPtr.Zero)
                    return false;

                RfcResultCode resultCode = _interop.IsConnectionHandleValid(_rfcConnectionHandle, out int isValid, out _);
                return resultCode == RfcResultCode.RFC_OK && isValid > 0;
            }
        }

        public bool Ping()
        {
            if (_rfcConnectionHandle == IntPtr.Zero)
                return false;

            RfcResultCode resultCode = _interop.Ping(
                rfcHandle: _rfcConnectionHandle,
                errorInfo: out _);

            return resultCode == RfcResultCode.RFC_OK;
        }

        public ISapFunction CreateFunction(string name)
        {
            IntPtr functionDescriptionHandle = _interop.GetFunctionDesc(
                rfcHandle: _rfcConnectionHandle,
                funcName: name,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return SapFunction.CreateFromDescriptionHandle(
                interop: _interop,
                rfcConnectionHandle: _rfcConnectionHandle,
                functionDescriptionHandle: functionDescriptionHandle);
        }

        private void Clear()
        {
            _rfcConnectionHandle = IntPtr.Zero;
        }
    }

    public interface ISapConnection : IDisposable
    {
        void Connect();

        void Disconnect();

        bool IsValid { get; }

        bool Ping();

        ISapFunction CreateFunction(string name);
    }
}
