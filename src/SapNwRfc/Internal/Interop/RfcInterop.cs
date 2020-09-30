using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace SapNwRfc.Internal.Interop
{
    [ExcludeFromCodeCoverage]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "Exception for interop class")]
    internal class RfcInterop
    {
        internal const string SapNwRfcDllName = "sapnwrfc";

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

        #endregion

        #region Function

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern IntPtr RfcGetFunctionDesc(IntPtr rfcHandle, string funcName, out RfcErrorInfo errorInfo);

        public virtual IntPtr GetFunctionDesc(IntPtr rfcHandle, string funcName, out RfcErrorInfo errorInfo)
            => RfcGetFunctionDesc(rfcHandle, funcName, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern IntPtr RfcCreateFunction(IntPtr funcDescHandle, out RfcErrorInfo errorInfo);

        public virtual IntPtr CreateFunction(IntPtr funcDescHandle, out RfcErrorInfo errorInfo)
            => RfcCreateFunction(funcDescHandle, out errorInfo);

        [DllImport(SapNwRfcDllName, CharSet = CharSet.Unicode)]
        private static extern RfcResultCode RfcDestroyFunction(IntPtr funcHandle, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode DestroyFunction(IntPtr funcHandle, out RfcErrorInfo errorInfo)
            => RfcDestroyFunction(funcHandle, out errorInfo);

        [DllImport(SapNwRfcDllName)]
        private static extern RfcResultCode RfcInvoke(IntPtr rfcHandle, IntPtr funcHandle, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode Invoke(IntPtr rfcHandle, IntPtr funcHandle, out RfcErrorInfo errorInfo)
            => RfcInvoke(rfcHandle, funcHandle, out errorInfo);

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
        private static extern RfcResultCode RfcGetBytes(IntPtr dataHandle, string name, byte[] bytesBuffer, uint bufferLength, out RfcErrorInfo errorInfo);

        public virtual RfcResultCode GetBytes(IntPtr dataHandle, string name, byte[] bytesBuffer, uint bufferLength, out RfcErrorInfo errorInfo)
            => RfcGetBytes(dataHandle, name, bytesBuffer, bufferLength, out errorInfo);

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
        private static extern IntPtr RfcAppendNewRow(IntPtr tableHandle, out RfcErrorInfo errorInfo);

        public virtual IntPtr AppendNewRow(IntPtr tableHandle, out RfcErrorInfo errorInfo)
            => RfcAppendNewRow(tableHandle, out errorInfo);

        #endregion
    }
}
