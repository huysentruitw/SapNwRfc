using System;
using System.Diagnostics.CodeAnalysis;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Represents an RFC connection to the SAP application server.
    /// </summary>
    public sealed class SapConnection : ISapConnection
    {
        private readonly RfcInterop _interop;
        private readonly SapConnectionParameters _parameters;
        private IntPtr _rfcConnectionHandle = IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapConnection"/> class with the given connection parameters.
        /// </summary>
        /// <param name="parameters">The connection parameters.</param>
        [ExcludeFromCodeCoverage]
        public SapConnection(SapConnectionParameters parameters)
            : this(new RfcInterop(), parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SapConnection"/> class with the given connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
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

        /// <summary>
        /// Disposes the connection. Disposing automatically disconnects from the SAP application server.
        /// </summary>
        public void Dispose()
        {
            Disconnect(disposing: true);
        }

        /// <inheritdoc cref="ISapConnection"/>
        public void Connect()
        {
            RfcConnectionParameter[] interopParameters = _parameters.ToInterop();

            _rfcConnectionHandle = _interop.OpenConnection(
                connectionParams: interopParameters,
                paramCount: (uint)interopParameters.Length,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError(beforeThrow: Clear);
        }

        /// <inheritdoc cref="ISapConnection"/>
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

        /// <inheritdoc cref="ISapConnection"/>
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

        /// <inheritdoc cref="ISapConnection"/>
        public bool Ping()
        {
            if (_rfcConnectionHandle == IntPtr.Zero)
                return false;

            RfcResultCode resultCode = _interop.Ping(
                rfcHandle: _rfcConnectionHandle,
                errorInfo: out _);

            return resultCode == RfcResultCode.RFC_OK;
        }

        /// <inheritdoc cref="ISapConnection"/>
        public SapConnectionAttributes GetAttributes()
        {
            RfcResultCode resultCode = _interop.GetConnectionAttributes(
                rfcHandle: _rfcConnectionHandle,
                attributes: out RfcAttributes attributes,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);

            return new SapConnectionAttributes(attributes);
        }

        /// <inheritdoc cref="ISapConnection"/>
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
}
