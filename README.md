# SAP NetWeaver RFC library

[![Build status](https://ci.appveyor.com/api/projects/status/6yd37vurchtbeb6c/branch/master?svg=true)](https://ci.appveyor.com/project/huysentruitw/sapnwrfc/branch/master)

This library allows you to call SAP NetWeaver RFC functions from .NET Framework and .NET Core.

## Get it on [NuGet](https://www.nuget.org/packages/SapNwRfc/)

    dotnet add package SapNwRfc
    
or

    PM> Install-Package SapNwRfc

## Prerequisites

This library requires the SAP NetWeaver RFC Library 7.50 SDK C++ binaries that should be installed locally. For download and installation instructions see [SAP Note 2573790](https://launchpad.support.sap.com/#/notes/2573790).

You can either place the DLL's in your project output folder or put them in a folder available in the systems `PATH` (Windows), `LD_LIBRARY_PATH` (Linux) or `DYLD_LIBRARY_PATH` (macOS) environment variable.

On Windows, the 7.50 version of the SAP binaries also require you to install the 64-bit version of the Visual C++ 2013 Redistributable package which can be downloaded and installed from [here](https://www.microsoft.com/en-us/download/details.aspx?id=40784).

## Usage

### Connect with SAP NetWeaver

```csharp
string connectionString = "AppServerHost=MY_SERVER_HOST; SystemNumber=00; User=MY_SAP_USER; Password=SECRET; Client=100; Language=EN; PoolSize=5; Trace=8";

using var connection = new SapConnection(connectionString);
connection.Connect();
```

### Call function without input or output parameters

```csharp
using var someFunction = connection.CreateFunction("ZBAPI_SOME_FUNCTION_NAME");
someFunction.Invoke();
```

### Call function with input parameters but no output parameters

```csharp
class SomeFunctionParameters
{
    [SapName("SOME_FIELD")]
    public string SomeField { get; set; }
}

using var someFunction = connection.CreateFunction("ZBAPI_SOME_FUNCTION_NAME");
someFunction.Invoke(new SomeFunctionParameters
{
    SomeField = "Some value",
});
```

### Call function with input and output parameters

```csharp
class SomeFunctionParameters
{
    [SapName("SOME_FIELD")]
    public string SomeField { get; set; }
}

class SomeFunctionResult
{
    [SapName("RES_ABC")]
    public string Abc { get; set; }
}

using var someFunction = connection.CreateFunction("ZBAPI_SOME_FUNCTION_NAME");
var result = someFunction.Invoke<SomeFunctionResult>(new SomeFunctionParameters
{
    SomeField = "Some value",
});

// Do something with result.Abc
```

### Define models with a nested structure

```csharp
class SomeFunctionResult
{
    [SapName("RES_ABC")]
    public string Abc { get; set; }

    [SapName("RES_ADDR")]
    public SomeFunctionResultItem Address { get; set; }
}

class SomeFunctionResultAddress
{
    [SapName("STREET")]
    public string Street { get; set; }

    [SapName("NR")]
    public string Number { get; set; }
}
```

### Define models with a nested table

```csharp
class SomeFunctionResult
{
    [SapName("RES_ABC")]
    public string Abc { get; set; }

    [SapName("RES_ITEMS")]
    public SomeFunctionResultItem[] Items { get; set; }
}

class SomeFunctionResultItem
{
    [SapName("ITM_NAME")]
    public string Name { get; set; }
}
```

## Input and output mapping

Input and output models used in function calls are mapped to and from SAP RFC parameter types by convention. In case the property name of the model differs from the SAP RFC parameter name, the `[SapName]`-attribute can be used.

For each input and output model type, the library builds and caches a mapping function using expression trees.

SAP RFC parameter types don't have to be specified as they're converted by convention. Here's an overview of supported type mappings:

C# type | SAP RFC type | Remarks
--- | --- | ---
`int` | RFCTYPE_INT | 4-byte integer
`long` | RFCTYPE_INT8 | 8-byte integer
`double` | RFCTYPE_FLOAT | Floating point, double precision
`decimal` | RFCTYPE_BCD |
`string` | RFCTYPE_CHAR |
`DateTime` | RFCTYPE_DATE | Only the day, month and year value is used
`TimeSpan` | RFCTYPE_TIME | Only the hour, minute and second value is used
`T` | RFCTYPE_STRUCTURE | Structures are constructed from nested objects (T) in the input or output model
`Array<T>` | RFCTYPE_TABLE | Tables are constructed from arrays of nested objects (T) in the input or output model
