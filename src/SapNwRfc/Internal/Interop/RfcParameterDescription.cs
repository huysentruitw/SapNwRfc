using System;
using System.Runtime.InteropServices;

namespace SapNwRfc.Internal.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RfcParameterDescription
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30 + 1)]
        public string Name;

        [MarshalAs(UnmanagedType.I4)]
        public SapRfcType Type;

        [MarshalAs(UnmanagedType.I4)]
        public SapRfcDirection Direction;

        [MarshalAs(UnmanagedType.U4)]
        public uint NucLength;

        [MarshalAs(UnmanagedType.U4)]
        public uint UcLength;

        [MarshalAs(UnmanagedType.U4)]
        public uint Decimals;

        public IntPtr TypeDescHandle;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30 + 1)]
        public string DefaultValue;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 79 + 1)]
        public string ParameterText;

        public byte Optional;

        public IntPtr ExtendedDescription;
    }
}
