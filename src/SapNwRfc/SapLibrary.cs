using System;
using System.Diagnostics.CodeAnalysis;
#if NETCOREAPP3_1
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
#endif
using SapNwRfc.Exceptions;
using SapNwRfc.Internal.Interop;

namespace SapNwRfc
{
    /// <summary>
    /// Static class that allows ensuring the SAP RFC binaries are present.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SapLibrary
    {
        /// <summary>
        /// Ensures the SAP RFC binaries are present. Throws an <see cref="SapLibraryNotFoundException"/> exception when the SAP RFC binaries could not be found.
        /// </summary>
        /// <param name="optionalSapRfcBinariesPath">Optional path to search for the SAP RFC binaries.</param>
#if NETCOREAPP3_1
        public static void EnsureLibraryPresent(string optionalSapRfcBinariesPath = null)
        {
            if (!string.IsNullOrEmpty(optionalSapRfcBinariesPath))
            {
                NativeLibrary.SetDllImportResolver(
                    assembly: typeof(SapLibrary).Assembly,
                    resolver: (string libraryName, Assembly assembly, DllImportSearchPath? searchPath) =>
                    {
                        if (libraryName == Internal.Interop.RfcInterop.SapNwRfcDllName)
                        {
                            NativeLibrary.TryLoad(
                                libraryPath: Path.Combine(optionalSapRfcBinariesPath, libraryName),
                                handle: out IntPtr handle);

                            return handle;
                        }

                        return IntPtr.Zero;
                    });
            }

#else
        public static void EnsureLibraryPresent()
        {
#endif
            GetVersion();
        }

        /// <summary>
        /// Gets the SAP RFC library version.
        /// </summary>
        /// <returns>The SAP RFC library version.</returns>
        public static SapLibraryVersion GetVersion()
        {
            try
            {
                RfcResultCode resultCode = new RfcInterop()
                    .GetVersion(out uint majorVersion, out uint minorVersion, out uint patchLevel);

                return new SapLibraryVersion
                {
                    Major = majorVersion,
                    Minor = minorVersion,
                    Patch = patchLevel,
                };
            }
            catch (DllNotFoundException ex)
            {
                throw new SapLibraryNotFoundException(ex);
            }
        }
    }
}
