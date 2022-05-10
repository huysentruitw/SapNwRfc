using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace SapNwRfc.Internal.Interop
{
    [ExcludeFromCodeCoverage]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "Exception for interop class")]
    internal class RfcInterop
    {
        private const string SapNwRfcDllName = "sapnwrfc";

        #region General

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcGetVersion(out uint majorVersion, out uint minorVersion, out uint patchLevel);

        public virtual RfcResultCode GetVersion(out uint majorVersion, out uint minorVersion, out uint patchLevel)
            => RfcGetVersion(out majorVersion, out minorVersion, out patchLevel);

        #endregion

        #region Connection

        [DllImport(SapNwRfcDllName)]
        private static extern IntPtr RfcOpenConnection(RfcConnectionParameter[] connectionParams, uint paramCount, out RfcErrorInfo errorInfo);

        public virtual IntPtr OpenConnection(RfcConnectionParameter[] connectionParams, uint paramCount, out RfcErrorInfo errorInfo)
            => RfcOpenConnection(connectionParams, paramCount, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcCloseConnection(IntPtr rfcHandle, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode CloseConnection(IntPtr rfcHandle, out RfcErrorInfo errorInfo)
            => RfcCloseConnection(rfcHandle, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcIsConnectionHandleValid(IntPtr rfcHandle, out int isValid, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode IsConnectionHandleValid(IntPtr rfcHandle, out int isValid, out RfcErrorInfo errorInfo)
            => RfcIsConnectionHandleValid(rfcHandle, out isValid, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcPing(IntPtr rfcHandle, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode Ping(IntPtr rfcHandle, out RfcErrorInfo errorInfo)
            => RfcPing(rfcHandle, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcGetConnectionAttributes(IntPtr rfcHandle, out RfcAttributes attributes, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetConnectionAttributes(IntPtr rfcHandle, out RfcAttributes attributes, out RfcErrorInfo errorInfo)
            => RfcGetConnectionAttributes(rfcHandle, out attributes, out errorInfo);

        #endregion

        #region Function

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern IntPtr RfcGetFunctionDesc(IntPtr rfcHandle, string funcName, out RfcErrorInfo errorInfo);

        public virtual IntPtr GetFunctionDesc(IntPtr rfcHandle, string funcName, out RfcErrorInfo errorInfo)
            => RfcGetFunctionDesc(rfcHandle, funcName, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern IntPtr RfcDescribeFunction(IntPtr rfcHandle, out RfcErrorInfo errorInfo);

        public virtual IntPtr DescribeFunction(IntPtr rfcHandle, out RfcErrorInfo errorInfo)
            => RfcDescribeFunction(rfcHandle, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetFunctionName(IntPtr rfcHandle, StringBuilder funcName, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetFunctionName(IntPtr rfcHandle, out string funcName, out RfcErrorInfo errorInfo)
        {
            var buffer = new StringBuilder(31);
            RfcResultCode resultCode = RfcGetFunctionName(rfcHandle, buffer, out errorInfo);
            funcName = buffer.ToString();
            return resultCode;
        }

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern IntPtr RfcCreateFunction(IntPtr funcDescHandle, out RfcErrorInfo errorInfo);

        public virtual IntPtr CreateFunction(IntPtr funcDescHandle, out RfcErrorInfo errorInfo)
            => RfcCreateFunction(funcDescHandle, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcDestroyFunction(IntPtr funcHandle, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode DestroyFunction(IntPtr funcHandle, out RfcErrorInfo errorInfo)
            => RfcDestroyFunction(funcHandle, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcGetParameterCount(IntPtr funcDesc, out uint count, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetParameterCount(IntPtr funcDesc, out uint count, out RfcErrorInfo errorInfo)
            => RfcGetParameterCount(funcDesc, out count, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetParameterDescByIndex(IntPtr funcDesc, uint index, out RfcParameterDescription paramDesc, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetParameterDescByIndex(IntPtr funcDesc, uint index, out RfcParameterDescription paramDesc, out RfcErrorInfo errorInfo)
            => RfcGetParameterDescByIndex(funcDesc, index, out paramDesc, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetParameterDescByName(IntPtr funcDesc, string name, out RfcParameterDescription paramDesc, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetParameterDescByName(IntPtr funcDesc, string name, out RfcParameterDescription paramDesc, out RfcErrorInfo errorInfo)
            => RfcGetParameterDescByName(funcDesc, name, out paramDesc, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcGetExceptionCount(IntPtr funcDesc, out uint count, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetExceptionCount(IntPtr funcDesc, out uint count, out RfcErrorInfo errorInfo)
            => RfcGetExceptionCount(funcDesc, out count, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetExceptionDescByIndex(IntPtr funcDesc, uint index, out RfcExceptionDescription excDesc, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetExceptionDescByIndex(IntPtr funcDesc, uint index, out RfcExceptionDescription excDesc, out RfcErrorInfo errorInfo)
            => RfcGetExceptionDescByIndex(funcDesc, index, out excDesc, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetExceptionDescByName(IntPtr funcDesc, string name, out RfcExceptionDescription excDesc, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetExceptionDescByName(IntPtr funcDesc, string name, out RfcExceptionDescription excDesc, out RfcErrorInfo errorInfo)
            => RfcGetExceptionDescByName(funcDesc, name, out excDesc, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcInvoke(IntPtr rfcHandle, IntPtr funcHandle, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode Invoke(IntPtr rfcHandle, IntPtr funcHandle, out RfcErrorInfo errorInfo)
            => RfcInvoke(rfcHandle, funcHandle, out errorInfo);

        #endregion

        #region Type

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcGetFieldCount(IntPtr typeHandle, out uint count, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetFieldCount(IntPtr typeHandle, out uint count, out RfcErrorInfo errorInfo)
            => RfcGetFieldCount(typeHandle, out count, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetFieldDescByName(IntPtr typeHandle, string name, out RfcFieldDescription fieldDesc, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetFieldDescByName(IntPtr typeHandle, string name, out RfcFieldDescription fieldDesc, out RfcErrorInfo errorInfo)
            => RfcGetFieldDescByName(typeHandle, name, out fieldDesc, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetFieldDescByIndex(IntPtr typeHandle, uint index, out RfcFieldDescription fieldDesc, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetFieldDescByIndex(IntPtr typeHandle, uint index, out RfcFieldDescription fieldDesc, out RfcErrorInfo errorInfo)
            => RfcGetFieldDescByIndex(typeHandle, index, out fieldDesc, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern IntPtr RfcDescribeType(IntPtr dataHandle, out RfcErrorInfo errorInfo);

        public virtual IntPtr DescribeType(IntPtr dataHandle, out RfcErrorInfo errorInfo)
            => RfcDescribeType(dataHandle, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern IntPtr RfcGetTypeDesc(IntPtr rfcHandle, string typeName, out RfcErrorInfo errorInfo);

        public virtual IntPtr GetTypeDesc(IntPtr rfcHandle, string typeName, out RfcErrorInfo errorInfo)
            => RfcGetTypeDesc(rfcHandle, typeName, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetTypeName(IntPtr rfcHandle, StringBuilder typeName, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetTypeName(IntPtr rfcHandle, out string typeName, out RfcErrorInfo errorInfo)
        {
            var buffer = new StringBuilder(31);
            RfcResultCode resultCode = RfcGetTypeName(rfcHandle, buffer, out errorInfo);
            typeName = buffer.ToString();
            return resultCode;
        }

        #endregion

        #region Data

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetString(IntPtr dataHandle, string name, char[] stringBuffer, uint bufferLength, out uint stringLength, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetString(IntPtr dataHandle, string name, char[] stringBuffer, uint bufferLength, out uint stringLength, out RfcErrorInfo errorInfo)
            => RfcGetString(dataHandle, name, stringBuffer, bufferLength, out stringLength, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcSetString(IntPtr dataHandle, string name, string value, uint valueLength, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode SetString(IntPtr dataHandle, string name, string value, uint valueLength, out RfcErrorInfo errorInfo)
            => RfcSetString(dataHandle, name, value, valueLength, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetInt(IntPtr dataHandle, string name, out int value, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetInt(IntPtr dataHandle, string name, out int value, out RfcErrorInfo errorInfo)
            => RfcGetInt(dataHandle, name, out value, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcSetInt(IntPtr dataHandle, string name, int value, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode SetInt(IntPtr dataHandle, string name, int value, out RfcErrorInfo errorInfo)
            => RfcSetInt(dataHandle, name, value, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetInt8(IntPtr dataHandle, string name, out long value, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetInt8(IntPtr dataHandle, string name, out long value, out RfcErrorInfo errorInfo)
            => RfcGetInt8(dataHandle, name, out value, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcSetInt8(IntPtr dataHandle, string name, long value, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode SetInt8(IntPtr dataHandle, string name, long value, out RfcErrorInfo errorInfo)
            => RfcSetInt8(dataHandle, name, value, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetFloat(IntPtr dataHandle, string name, out double value, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetFloat(IntPtr dataHandle, string name, out double value, out RfcErrorInfo errorInfo)
            => RfcGetFloat(dataHandle, name, out value, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcSetFloat(IntPtr dataHandle, string name, double value, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode SetFloat(IntPtr dataHandle, string name, double value, out RfcErrorInfo errorInfo)
            => RfcSetFloat(dataHandle, name, value, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetDate(IntPtr dataHandle, string name, char[] emptyDate, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetDate(IntPtr dataHandle, string name, char[] emptyDate, out RfcErrorInfo errorInfo)
            => RfcGetDate(dataHandle, name, emptyDate, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcSetDate(IntPtr dataHandle, string name, char[] date, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode SetDate(IntPtr dataHandle, string name, char[] date, out RfcErrorInfo errorInfo)
            => RfcSetDate(dataHandle, name, date, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetTime(IntPtr dataHandle, string name, char[] emptyTime, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetTime(IntPtr dataHandle, string name, char[] emptyTime, out RfcErrorInfo errorInfo)
            => RfcGetTime(dataHandle, name, emptyTime, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcSetTime(IntPtr dataHandle, string name, char[] time, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode SetTime(IntPtr dataHandle, string name, char[] time, out RfcErrorInfo errorInfo)
            => RfcSetTime(dataHandle, name, time, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetXString(IntPtr dataHandle, string name, byte[] bytesBuffer, uint bufferLength, out uint xstringLength, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetXString(IntPtr dataHandle, string name, byte[] bytesBuffer, uint bufferLength, out uint xstringLength, out RfcErrorInfo errorInfo)
            => RfcGetXString(dataHandle, name, bytesBuffer, bufferLength, out xstringLength, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcSetBytes(IntPtr dataHandle, string name, byte[] value, uint valueLength, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode SetBytes(IntPtr dataHandle, string name, byte[] value, uint valueLength, out RfcErrorInfo errorInfo)
            => RfcSetBytes(dataHandle, name, value, valueLength, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetChars(IntPtr dataHandle, string name, char[] charBuffer, uint bufferLength, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetChars(IntPtr dataHandle, string name, char[] charBuffer, uint bufferLength, out RfcErrorInfo errorInfo)
            => RfcGetChars(dataHandle, name, charBuffer, bufferLength, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcSetChars(IntPtr dataHandle, string name, char[] value, uint valueLength, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode SetChars(IntPtr dataHandle, string name, char[] value, uint valueLength, out RfcErrorInfo errorInfo)
            => RfcSetChars(dataHandle, name, value, valueLength, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetStructure(IntPtr dataHandle, string name, out IntPtr structHandle, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetStructure(IntPtr dataHandle, string name, out IntPtr structHandle, out RfcErrorInfo errorInfo)
            => RfcGetStructure(dataHandle, name, out structHandle, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcGetTable(IntPtr dataHandle, string name, out IntPtr tableHandle, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetTable(IntPtr dataHandle, string name, out IntPtr tableHandle, out RfcErrorInfo errorInfo)
            => RfcGetTable(dataHandle, name, out tableHandle, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcGetRowCount(IntPtr tableHandle, out uint rowCount, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetRowCount(IntPtr tableHandle, out uint rowCount, out RfcErrorInfo errorInfo)
            => RfcGetRowCount(tableHandle, out rowCount, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern IntPtr RfcGetCurrentRow(IntPtr tableHandle, out RfcErrorInfo errorInfo);

        public virtual IntPtr GetCurrentRow(IntPtr tableHandle, out RfcErrorInfo errorInfo)
            => RfcGetCurrentRow(tableHandle, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcMoveToNextRow(IntPtr tableHandle, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode MoveToNextRow(IntPtr tableHandle, out RfcErrorInfo errorInfo)
            => RfcMoveToNextRow(tableHandle, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcMoveToFirstRow(IntPtr tableHandle, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode MoveToFirstRow(IntPtr tableHandle, out RfcErrorInfo errorInfo)
            => RfcMoveToFirstRow(tableHandle, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern IntPtr RfcAppendNewRow(IntPtr tableHandle, out RfcErrorInfo errorInfo);

        public virtual IntPtr AppendNewRow(IntPtr tableHandle, out RfcErrorInfo errorInfo)
            => RfcAppendNewRow(tableHandle, out errorInfo);

        #endregion

        #region Server

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate RfcResultCode RfcServerFunction(IntPtr connectionHandle, IntPtr functionHandle, out RfcErrorInfo errorInfo);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate RfcResultCode RfcFunctionDescriptionCallback(string functionName, RfcAttributes attributes, ref IntPtr funcDescHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RfcServerErrorListener(IntPtr serverHandle, in RfcAttributes clientInfo, in RfcErrorInfo errorInfo);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RfcServerStateChangeListener(IntPtr serverHandle, in RfcStateChange stateChange);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcInstallGenericServerFunction(RfcServerFunction serverFunction, RfcFunctionDescriptionCallback funcDescPointer, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode InstallGenericServerFunction(RfcServerFunction serverFunction, RfcFunctionDescriptionCallback funcDescPointer, out RfcErrorInfo errorInfo)
            => RfcInstallGenericServerFunction(serverFunction, funcDescPointer, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern IntPtr RfcCreateServer(RfcConnectionParameter[] connectionParams, uint paramCount, out RfcErrorInfo errorInfo);

        public virtual IntPtr CreateServer(RfcConnectionParameter[] connectionParams, uint paramCount, out RfcErrorInfo errorInfo)
            => RfcCreateServer(connectionParams, paramCount, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcDestroyServer(IntPtr rfcHandle, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode DestroyServer(IntPtr rfcHandle, out RfcErrorInfo errorInfo)
            => RfcDestroyServer(rfcHandle, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcLaunchServer(IntPtr rfcHandle, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode LaunchServer(IntPtr rfcHandle, out RfcErrorInfo errorInfo)
            => RfcLaunchServer(rfcHandle, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcShutdownServer(IntPtr rfcHandle, uint timeout, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode ShutdownServer(IntPtr rfcHandle, uint timeout, out RfcErrorInfo errorInfo)
            => RfcShutdownServer(rfcHandle, timeout, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcAddServerErrorListener(IntPtr rfcHandle, RfcServerErrorListener errorListener, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode AddServerErrorListener(IntPtr rfcHandle, RfcServerErrorListener errorListener, out RfcErrorInfo errorInfo)
            => RfcAddServerErrorListener(rfcHandle, errorListener, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcAddServerStateChangedListener(IntPtr rfcHandle, RfcServerStateChangeListener stateChangeListener, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode AddServerStateChangedListener(IntPtr rfcHandle, RfcServerStateChangeListener stateChangeListener, out RfcErrorInfo errorInfo)
            => RfcAddServerStateChangedListener(rfcHandle, stateChangeListener, out errorInfo);

        #endregion
    }
}
