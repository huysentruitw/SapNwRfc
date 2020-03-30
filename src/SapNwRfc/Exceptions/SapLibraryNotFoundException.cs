using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SapNwRfc.Exceptions
{
    public sealed class SapLibraryNotFoundException : Exception
    {
        public SapLibraryNotFoundException(Exception innerException)
            : base(BuildMessage(), innerException)
        {
        }

        private static string BuildMessage()
        {
            var message = new StringBuilder();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                message.AppendLine("The SAP RFC libraries were not found in the output folder or in a folder contained in the systems PATH environment variable.");
                message.AppendLine();
                message.AppendLine("Required files for Windows:");
                message.AppendLine("  sapnwrfc.dll");
                message.AppendLine("  icudtXX.dll");
                message.AppendLine("  icuinXX.dll");
                message.AppendLine("  icuucXX.dll");
                message.AppendLine();
                message.AppendLine("Also make sure the 64-bit version of the Visual C++ 2013 Redistributable is installed");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                message.AppendLine("The SAP RFC libraries were not found in the output folder or in a folder contained in the systems DYLD_LIBRARY_PATH environment variable.");
                message.AppendLine();
                message.AppendLine("Required files for macOS:");
                message.AppendLine("  libsapnwrfc.dylib");
                message.AppendLine("  libicudata.XX.dylib");
                message.AppendLine("  libicui18n.XX.dylib");
                message.AppendLine("  libicuuc.XX.dylib");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                message.AppendLine("The SAP RFC libraries were not found in the output folder or in a folder contained in the systems LD_LIBRARY_PATH environment variable.");
                message.AppendLine();
                message.AppendLine("Required files for Linux:");
                message.AppendLine("  libsapnwrfc.so");
                message.AppendLine("  libicudata.so.XX");
                message.AppendLine("  libicui18n.so.XX");
                message.AppendLine("  libicuuc.so.XX");
            }

            return message.ToString();
        }
    }
}
