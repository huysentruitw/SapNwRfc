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
        private readonly SapConnectionParameters _parametes;

        private SapServer(RfcInterop interop, IntPtr rfcServerHandle, SapConnectionParameters parameters)
        {
            _interop = interop;
            _rfcServerHandle = rfcServerHandle;
            _parametes = parameters;
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

        internal static ISapServer Create(RfcInterop rfcInterop, SapConnectionParameters parameters)
        {
            RfcConnectionParameter[] interopParameters = parameters.ToInterop();

            IntPtr rfcServerHandle = rfcInterop.CreateServer(
                connectionParams: interopParameters,
                paramCount: (uint)interopParameters.Length,
                errorInfo: out RfcErrorInfo errorInfo);

            errorInfo.ThrowOnError();

            return new SapServer(rfcInterop, rfcServerHandle, parameters);
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
            RfcResultCode resultCode = _interop.DestroyServer(
                rfcHandle: _rfcServerHandle,
                errorInfo: out RfcErrorInfo errorInfo);

            // resultCode.ThrowOnError(errorInfo);
        }

        /// <summary>
        /// Installs a global server function handler.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="action">The RFC server function handler.</param>
        public static void InstallGenericServerFunctionHandler(string connectionString, Action<ISapServerFunction> action)
        {
            InstallGenericServerFunctionHandler(SapConnectionParameters.Parse(connectionString), action);
        }

        /// <summary>
        /// Installs a global server function handler.
        /// </summary>
        /// <param name="parameters">The connection parameters.</param>
        /// <param name="action">The RFC server function handler.</param>
        public static void InstallGenericServerFunctionHandler(SapConnectionParameters parameters, Action<ISapServerFunction> action)
        {
            InstallGenericServerFunctionHandler(new RfcInterop(), parameters, action);
        }

        internal static void InstallGenericServerFunctionHandler(RfcInterop interop, SapConnectionParameters parameters, Action<ISapServerFunction> action)
        {
            RfcResultCode resultCode = interop.InstallGenericServerFunction(
                serverFunction: (IntPtr connectionHandle, IntPtr functionHandle, out RfcErrorInfo errorInfo)
                                    => HandleGenericFunction(interop, action, connectionHandle, functionHandle, out errorInfo),
                funcDescPointer: (string functionName, ref RfcAttributes attributes, out IntPtr funcDescHandle)
                                    => HandleGenericMetadata(interop, parameters, functionName, attributes, out funcDescHandle),
                out RfcErrorInfo installFunctionErrorInfo);

            resultCode.ThrowOnError(installFunctionErrorInfo);
        }

        private static RfcResultCode HandleGenericFunction(RfcInterop interop, Action<ISapServerFunction> action, IntPtr connectionHandle, IntPtr functionHandle, out RfcErrorInfo errorInfo)
        {
            IntPtr functionDesc = interop.DescribeFunction(
                rfcHandle: functionHandle,
                errorInfo: out RfcErrorInfo functionDescErrorInfo);

            if (functionDescErrorInfo.Code != RfcResultCode.RFC_OK)
            {
                errorInfo = functionDescErrorInfo;
                return functionDescErrorInfo.Code;
            }

            RfcResultCode functionNameResultCode = interop.GetFunctionName(
                rfcHandle: functionDesc,
                funcName: out string functionName,
                errorInfo: out RfcErrorInfo functionNameErrorInfo);

            if (functionNameResultCode != RfcResultCode.RFC_OK)
            {
                errorInfo = functionNameErrorInfo;
                return functionNameResultCode;
            }

            var function = new SapServerFunction(interop, functionHandle, functionName);

            try
            {
                action(function);

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

        private static RfcResultCode HandleGenericMetadata(RfcInterop interop, SapConnectionParameters parameters, string functionName, RfcAttributes attributes, out IntPtr funcDescHandle)
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
                functionName,
                errorInfo: out RfcErrorInfo errorInfo);

            RfcResultCode resultCode = interop.CloseConnection(
                rfcHandle: connection,
                errorInfo: out RfcErrorInfo closeErrorInfo);

            return errorInfo.Code;
        }
    }
}
