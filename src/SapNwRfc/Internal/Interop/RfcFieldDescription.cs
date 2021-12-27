using System;
using System.Runtime.InteropServices;

namespace SapNwRfc.Internal.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RfcFieldDescription
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30 + 1)]
        public string Name;

        [MarshalAs(UnmanagedType.I4)]
        public SapRfcType Type;

        [MarshalAs(UnmanagedType.U4)]
        public uint NucLength;

        [MarshalAs(UnmanagedType.U4)]
        public uint NucOffset;

        [MarshalAs(UnmanagedType.U4)]
        public uint UcLength;

        [MarshalAs(UnmanagedType.U4)]
        public uint UcOffset;

        [MarshalAs(UnmanagedType.U4)]
        public uint Decimals;

        public IntPtr TypeDescHandle;

        public IntPtr ExtendedDescription;
    }
}
