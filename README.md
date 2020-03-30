# SAP NetWeaver RFC library

This library is allows you to call SAP NetWeaver RFC functions from .NET Framework and .NET Core. 

## Get it on NuGet

    PM> Install-Package SapNwRfc
    
or

    dotnet add package SapNwRfc

## Prerequisites

This library requires the SAP NetWeaver RFC Library 7.50 SDK C++ binaries that should be installed locally. For download and installation instructions see [SAP Note 2573790](https://launchpad.support.sap.com/#/notes/2573790).

You can either place the DLL's in your project output folder or put them in a folder available in the systems PATH (Windows), LD_LIBRARY_PATH (Linux) or DYLD_LIBRARY_PATH (macOS) environment variable.

On Windows, the 7.50 version of the SAP binaries also require you to install the 64-bit version of the Visual C++ 2013 Redistributable package which can be downloaded and installed from [here](https://www.microsoft.com/en-us/download/details.aspx?id=40784).

## Usage

