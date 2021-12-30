using System;
using System.Collections.Concurrent;
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
        /// <param name="getFunctionMetadata">The metadata handler.</param>
        public static void InstallGenericServerFunctionHandler(Func<string, SapAttributes, ISapFunctionMetadata> getFunctionMetadata)
        {
            InstallGenericServerFunctionHandler(new RfcInterop(), getFunctionMetadata);
        }

        private static readonly ConcurrentDictionary<string, SapServer> Servers = new ConcurrentDictionary<string, SapServer>();

        // Keep a reference to the generic server function delegates so they don't get GCed
        private static readonly RfcInterop.RfcServerFunction ServerFunction = HandleGenericFunction;
        private static readonly RfcInterop.RfcFunctionDescriptionCallback GenericMetadata = HandleGenericMetadata;

#pragma warning disable SA1306 // Field names should begin with lower-case letter
        private static bool GenericServerFunctionHandlerInstalled;
        private static (RfcInterop Interop, Func<string, SapAttributes, ISapFunctionMetadata> GetFunctionMetadata) GenericServerFunctionHandler;
#pragma warning restore SA1306 // Field names should begin with lower-case letter

        private static void InstallGenericServerFunctionHandler(RfcInterop interop, Func<string, SapAttributes, ISapFunctionMetadata> getFunctionMetadata)
        {
            GenericServerFunctionHandler = (interop, getFunctionMetadata);

            if (!GenericServerFunctionHandlerInstalled)
            {
                RfcResultCode resultCode = interop.InstallGenericServerFunction(
                    serverFunction: ServerFunction,
                    funcDescPointer: GenericMetadata,
                    out RfcErrorInfo installFunctionErrorInfo);

                resultCode.ThrowOnError(installFunctionErrorInfo);

                GenericServerFunctionHandlerInstalled = true;
            }
        }

        private static RfcResultCode HandleGenericFunction(IntPtr connectionHandle, IntPtr functionHandle, out RfcErrorInfo errorInfo)
        {
            (RfcInterop interop, _) = GenericServerFunctionHandler;
            if (interop == null)
            {
                errorInfo = new RfcErrorInfo
                {
                    Code = RfcResultCode.RFC_EXTERNAL_FAILURE,
                };
                return RfcResultCode.RFC_EXTERNAL_FAILURE;
            }

            IntPtr functionDesc = interop.DescribeFunction(
                rfcHandle: functionHandle,
                errorInfo: out RfcErrorInfo functionDescErrorInfo);

            if (functionDescErrorInfo.Code != RfcResultCode.RFC_OK)
            {
                errorInfo = functionDescErrorInfo;
                return functionDescErrorInfo.Code;
            }

            RfcResultCode serverContextResultCode = interop.GetServerContext(
                rfcHandle: connectionHandle,
                serverContext: out RfcServerContext serverContext,
                errorInfo: out RfcErrorInfo serverContextErrorInfo);

            if (serverContextResultCode != RfcResultCode.RFC_OK)
            {
                errorInfo = serverContextErrorInfo;
                return serverContextResultCode;
            }

            if (serverContext.IsStateful == 0)
            {
                RfcResultCode setServerStatefulResultCode = interop.SetServerStateful(
                    rfcHandle: connectionHandle,
                    isStateful: 1,
                    errorInfo: out RfcErrorInfo setServerStatefulErrorInfo);

                if (setServerStatefulResultCode != RfcResultCode.RFC_OK)
                {
                    errorInfo = setServerStatefulErrorInfo;
                    return setServerStatefulResultCode;
                }
            }

            if (Servers.TryGetValue(serverContext.SessionId, out SapServer sapServer))
            {
                var connection = new SapServerConnection(interop, connectionHandle);
                var function = new SapServerFunction(interop, functionHandle, functionDesc);

                try
                {
                    sapServer._action(connection, function);

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

            errorInfo = new RfcErrorInfo
            {
                Code = RfcResultCode.RFC_NOT_FOUND,
            };
            return RfcResultCode.RFC_EXTERNAL_FAILURE;
        }

        private static RfcResultCode HandleGenericMetadata(string functionName, RfcAttributes attributes, out IntPtr funcDescHandle)
        {
            funcDescHandle = IntPtr.Zero;

            (RfcInterop interop, Func<string, SapAttributes, ISapFunctionMetadata> getFunctionMetadata) = GenericServerFunctionHandler;
            if (interop == null || getFunctionMetadata == null)
                return RfcResultCode.RFC_NOT_FOUND;

            try
            {
                ISapFunctionMetadata functionMetadata = getFunctionMetadata(functionName, new SapAttributes(attributes));

                if (functionMetadata is SapFunctionMetadata sapFunctionMetadata)
                {
                    funcDescHandle = sapFunctionMetadata.GetFunctionDescHandle();
                }
            }
            catch
            {
                return RfcResultCode.RFC_EXTERNAL_FAILURE;
            }

            return funcDescHandle == IntPtr.Zero ? RfcResultCode.RFC_NOT_FOUND : RfcResultCode.RFC_OK;
        }
    }
}
