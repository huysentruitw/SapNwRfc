using System;
using System.Runtime.InteropServices;

namespace SapNwRfc.Internal.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RfcServerContext
    {
        [MarshalAs(UnmanagedType.I4)]
        public RfcCallType CallType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24 + 1)]
        public string Tid;

        public IntPtr UnitIdentifier;

        public IntPtr UnitAttributes;

        [MarshalAs(UnmanagedType.U4)]
        public uint IsStateful;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32 + 1)]
        public string SessionId;
    }
}
