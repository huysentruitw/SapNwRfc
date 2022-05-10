using System;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Represents an SAP RFC server.
    /// </summary>
    public sealed class SapServer : ISapServer
    {
        private readonly RfcInterop _interop;
        private readonly IntPtr _rfcServerHandle;
        private readonly SapConnectionParameters _parameters;

        private SapServer(RfcInterop interop, IntPtr rfcServerHandle, SapConnectionParameters parameters)
        {
            _interop = interop;
            _rfcServerHandle = rfcServerHandle;
            _parameters = parameters;
        }

        /// <summary>
        /// Creates and connects a new RFC Server.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The SAP RFC Server.</returns>
        public static ISapServer Create(string connectionString)
        {
            return Create(SapConnectionParameters.Parse(connectionString));
        }

        /// <summary>
        /// Creates and connects a new RFC Server.
        /// </summary>
        /// <param name="parameters">The connection parameters.</param>
        /// <returns>The SAP RFC Server.</returns>
        public static ISapServer Create(SapConnectionParameters parameters)
        {
            return Create(new RfcInterop(), parameters);
        }

        private static ISapServer Create(RfcInterop rfcInterop, SapConnectionParameters parameters)
        {
            RfcConnectionParameter[] interopParameters = parameters.ToInterop();

            IntPtr rfcServerHandle = rfcInterop.CreateServer(
                connectionParams: interopParameters,
                paramCount: (uint)interopParameters.Length,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return new SapServer(rfcInterop, rfcServerHandle, parameters);
        }

        private EventHandler<SapServerErrorEventArgs> _error;

        /// <inheritdoc cref="ISapServer"/>
        public event EventHandler<SapServerErrorEventArgs> Error
        {
            add
            {
                if (_error == null)
                {
                    RfcResultCode resultCode = _interop.AddServerErrorListener(
                        rfcHandle: _rfcServerHandle,
                        errorListener: ServerErrorListener,
                        errorInfo: out RfcErrorInfo errorInfo);

                    resultCode.ThrowOnError(errorInfo);
                }

                _error += value;
            }

            remove
            {
                _error -= value;
            }
        }

        private void ServerErrorListener(IntPtr serverHandle, in RfcAttributes clientInfo, in RfcErrorInfo errorInfo)
        {
            _error?.Invoke(this, new SapServerErrorEventArgs(new SapAttributes(clientInfo), new SapErrorInfo(errorInfo)));
        }

        private EventHandler<SapServerStateChangeEventArgs> _stateChange;

        /// <inheritdoc cref="ISapServer"/>
        public event EventHandler<SapServerStateChangeEventArgs> StateChange
        {
            add
            {
                if (_stateChange == null)
                {
                    RfcResultCode resultCode = _interop.AddServerStateChangedListener(
                        rfcHandle: _rfcServerHandle,
                        stateChangeListener: ServerStateChangeListener,
                        errorInfo: out RfcErrorInfo errorInfo);

                    resultCode.ThrowOnError(errorInfo);
                }

                _stateChange += value;
            }

            remove
            {
                _stateChange -= value;
            }
        }

        private void ServerStateChangeListener(IntPtr serverHandle, in RfcStateChange stateChange)
        {
            _stateChange?.Invoke(this, new SapServerStateChangeEventArgs(stateChange));
        }

        /// <inheritdoc cref="ISapServer"/>
        public void Launch()
        {
            RfcResultCode resultCode = _interop.LaunchServer(
                rfcHandle: _rfcServerHandle,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);
        }

        /// <inheritdoc cref="ISapServer"/>
        public void Shutdown(uint timeout)
        {
            RfcResultCode resultCode = _interop.ShutdownServer(
                rfcHandle: _rfcServerHandle,
                timeout: timeout,
                errorInfo: out RfcErrorInfo errorInfo);

            resultCode.ThrowOnError(errorInfo);
        }

        /// <summary>
        /// Disposes the server. Disposing automatically disconnects from the SAP application server.
        /// </summary>
        public void Dispose()
        {
            _interop.DestroyServer(
                rfcHandle: _rfcServerHandle,
                errorInfo: out RfcErrorInfo _);
        }

        /// <summary>
        /// Installs a global server function handler.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="action">The RFC server function handler.</param>
        public static void InstallGenericServerFunctionHandler(string connectionString, Action<ISapServerConnection, ISapServerFunction> action)
        {
            InstallGenericServerFunctionHandler(SapConnectionParameters.Parse(connectionString), action);
        }

        /// <summary>
        /// Installs a global server function handler.
        /// </summary>
        /// <param name="parameters">The connection parameters.</param>
        /// <param name="action">The RFC server function handler.</param>
        public static void InstallGenericServerFunctionHandler(SapConnectionParameters parameters, Action<ISapServerConnection, ISapServerFunction> action)
        {
            InstallGenericServerFunctionHandler(new RfcInterop(), parameters, action);
        }

        private static void InstallGenericServerFunctionHandler(RfcInterop interop, SapConnectionParameters parameters, Action<ISapServerConnection, ISapServerFunction> action)
        {
            RfcResultCode resultCode = interop.InstallGenericServerFunction(
                serverFunction: (IntPtr connectionHandle, IntPtr functionHandle, out RfcErrorInfo errorInfo)
                                    => HandleGenericFunction(interop, action, connectionHandle, functionHandle, out errorInfo),
                funcDescPointer: (string functionName, RfcAttributes attributes, ref IntPtr funcDescHandle)
                                    => HandleGenericMetadata(interop, parameters, functionName, out funcDescHandle),
                out RfcErrorInfo installFunctionErrorInfo);

            resultCode.ThrowOnError(installFunctionErrorInfo);
        }

        private static RfcResultCode HandleGenericFunction(RfcInterop interop, Action<ISapServerConnection, ISapServerFunction> action, IntPtr connectionHandle, IntPtr functionHandle, out RfcErrorInfo errorInfo)
        {
            IntPtr functionDesc = interop.DescribeFunction(
                rfcHandle: functionHandle,
                errorInfo: out RfcErrorInfo functionDescErrorInfo);

            if (functionDescErrorInfo.Code != RfcResultCode.RFC_OK)
            {
                errorInfo = functionDescErrorInfo;
                return functionDescErrorInfo.Code;
            }

            var connection = new SapServerConnection(interop, connectionHandle);
            var function = new SapServerFunction(interop, functionHandle, functionDesc);

            try
            {
                action(connection, function);

                errorInfo = default;
                return RfcResultCode.RFC_OK;
            }
            catch (Exception ex)
            {
                errorInfo = new RfcErrorInfo
                {
                    Code = RfcResultCode.RFC_EXTERNAL_FAILURE,
                    Message = ex.Message,
                };
                return RfcResultCode.RFC_EXTERNAL_FAILURE;
            }
        }

        private static RfcResultCode HandleGenericMetadata(RfcInterop interop, SapConnectionParameters parameters, string functionName, out IntPtr funcDescHandle)
        {
            RfcConnectionParameter[] interopParameters = parameters.ToInterop();

            IntPtr connection = interop.OpenConnection(
                connectionParams: interopParameters,
                paramCount: (uint)interopParameters.Length,
                errorInfo: out RfcErrorInfo connectionErrorInfo);

            if (connectionErrorInfo.Code != RfcResultCode.RFC_OK)
            {
                funcDescHandle = IntPtr.Zero;
                return connectionErrorInfo.Code;
            }

            funcDescHandle = interop.GetFunctionDesc(
                rfcHandle: connection,
                funcName: functionName,
                errorInfo: out RfcErrorInfo errorInfo);

            interop.CloseConnection(
                rfcHandle: connection,
                errorInfo: out RfcErrorInfo _);

            return errorInfo.Code;
        }
    }
}
