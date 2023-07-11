using System;
using System.Runtime.InteropServices;

namespace SapNwRfc.Internal.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RfcServerAttributes
    {
        public IntPtr ServerName;

        [MarshalAs(UnmanagedType.I4)]
        public SapRfcProtocolType Type;

        [MarshalAs(UnmanagedType.U4)]
        public uint RegistrationCount;

        [MarshalAs(UnmanagedType.I4)]
        public SapRfcServerState State;

        [MarshalAs(UnmanagedType.U4)]
        public uint CurrentBusyCount;

        [MarshalAs(UnmanagedType.U4)]
        public uint PeakBusyCount;
    }
}
