# SAP NetWeaver RFC library

[![Build status](https://github.com/huysentruitw/SapNwRfc/actions/workflows/build-test-publish.yml/badge.svg?branch=master)](https://github.com/huysentruitw/SapNwRfc/actions/workflows/build-test-publish.yml?query=branch%3Amaster)

This cross-platform library allows you to call SAP NetWeaver RFC functions from .NET 5+, .NET Core and the .NET Framework.

The library is fully tested and production ready. Supported operating systems are Windows, Linux and macOS.

Also supports connection pooling for more complex applications, see [below](#connection-pooling).

## Get it on [NuGet](https://www.nuget.org/packages/SapNwRfc/)

    dotnet add package SapNwRfc
    
or

    PM> Install-Package SapNwRfc

## Prerequisites

This library requires the SAP NetWeaver RFC Library 7.50 SDK C++ binaries that should be installed locally. For download and installation instructions see [SAP Note 2573790](https://launchpad.support.sap.com/#/notes/2573790).

You can either place the DLL's in your project output folder or put them in a folder available in the systems `PATH` (Windows), `LD_LIBRARY_PATH` (Linux) or `DYLD_LIBRARY_PATH` (macOS) environment variable.

On Windows, the 7.50 version of the SAP binaries also require you to install the 64-bit version of the Visual C++ 2013 Redistributable package which can be downloaded and installed from [here](https://www.microsoft.com/en-us/download/details.aspx?id=40784).

### MacOSX and DYLD_LIBRARY_PATH

When your application is started as a child-process (f.e. when running it using Visual Studio for Mac), it is possible that the `DYLD_LIBRARY_PATH` is not forwarded to the child-process. This is because of [this runtime protection](https://developer.apple.com/library/archive/documentation/Security/Conceptual/System_Integrity_Protection_Guide/RuntimeProtections/RuntimeProtections.html) Apple has added to OSX.

The simplest solution to this problem is to make sure the SAP binaries are placed in the working directory of your application.

or to use the [`NativeLibrary.SetDllImportResolver`](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.nativelibrary.setdllimportresolver?view=net-5.0) method in your application.

A more flexible workaround is to set the import resolver for the `SapLibrary` using the [SetDllImportResolver](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.nativelibrary.setdllimportresolver?view=net-5.0)-method like this:

```c#
NativeLibrary.SetDllImportResolver(
    assembly: typeof(SapLibrary).Assembly,
    resolver: (string libraryName, Assembly assembly, DllImportSearchPath? searchPath) =>
    {
        if (libraryName == "sapnwrfc")
        {
            NativeLibrary.TryLoad(
                libraryPath: Path.Combine("<your_sap_rfc_binaries_path>", libraryName),
                handle: out IntPtr handle);

            return handle;
        }

        return IntPtr.Zero;
    });
```

## Usage

### Connect with SAP NetWeaver

```csharp
string connectionString = "AppServerHost=MY_SERVER_HOST; SystemNumber=00; User=MY_SAP_USER; Password=SECRET; Client=100; Language=EN; PoolSize=5; Trace=8";

using var connection = new SapConnection(connectionString);
connection.Connect();
```

### Call function without input or output parameters

```csharp
using var someFunction = connection.CreateFunction("BAPI_SOME_FUNCTION_NAME");
someFunction.Invoke();
```

### Call function with input parameters but no output parameters

```csharp
class SomeFunctionParameters
{
    [SapName("SOME_FIELD")]
    public string SomeField { get; set; }
}

using var someFunction = connection.CreateFunction("BAPI_SOME_FUNCTION_NAME");
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

using var someFunction = connection.CreateFunction("BAPI_SOME_FUNCTION_NAME");
var result = someFunction.Invoke<SomeFunctionResult>(new SomeFunctionParameters
{
    SomeField = "Some value",
});

// Do something with result.Abc
```

### Call function with dynamic input and output parameters

```csharp
using var someFunction = connection.CreateFunction("BAPI_SOME_FUNCTION_NAME");
var result = someFunction.Invoke<dynamic>(new
{
    SOME_FIELD = "Some value",
});

string abc = result.RES_ABC;
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

### Define models with an IEnumerable

```csharp
class SomeFunctionResult
{
    [SapName("RES_ITEMS")]
    public IEnumerable<SomeFunctionResultItem> Items { get; set; }
}

class SomeFunctionResultItem
{
    [SapName("ITM_NAME")]
    public string Name { get; set; }
}
```

### Exclude properties from mapping

```csharp
class SomeFunctionParameters
{
    [SapIgnore]
    public string IgnoredProperty { get; set; }

    [SapName("SOME_FIELD")]
    public string SomeField { get; set; }
}

class SomeFunctionResult
{
    [SapIgnore]
    public string IgnoredProperty { get; set; }

    [SapName("SOME_FIELD")]
    public string SomeField { get; set; }
}
```

### RFC Server Generic Handler

```csharp
string connectionString = "AppServerHost=MY_SERVER_HOST; SystemNumber=00; User=MY_SAP_USER; Password=SECRET; Client=100; Language=EN; PoolSize=5; Trace=8";

SapServer.InstallGenericServerFunctionHandler((string functionName, SapAttributes attributes) =>
{
    using var connection = new SapConnection(connectionString);
    connection.Connect();
    return connection.GetFunctionMetadata(functionName);
});
```

### RFC Server

```csharp
string connectionString = "GatewayHost=MY_GW_HOST; GatewayService=MY_GW_SERV; ProgramId=MY_PROGRAM_ID; RegistrationCount=1";

using var server = SapServer.Create(connectionString, (ISapServerConnection connection, ISapServerFunction function) =>
{
    var attributes = connection.GetAttributes();

    switch (function.GetName())
    {
        case "BAPI_SOME_FUNCTION_NAME":
            var parameters = function.GetParameters<SomeFunctionParameters>();
            function.SetResult(new SomeFunctionResult { Abc = "Some Value" });
        break;
    }
});
server.Error += (object sender, SapServerErrorEventArgs args) => Console.WriteLine(args.Error.Message);
server.StateChange += (object sender, SapServerStateChangeEventArgs args) => Console.WriteLine(args.OldState + " -> " + args.NewState);
server.Launch();
```

### Type Metadata

```csharp
var typeMetadata = connection.GetTypeMetadata("MY_STRUCTURE");
var typeName = typeMetadata.GetTypeName();
var fieldCount = typeMetadata.Fields.Count;
foreach (var fieldMetadata in typeMetadata.Fields)
{
    var fieldName = fieldMetadata.Name;
    var fieldType = fieldMetadata.Type;
}
typeMetadata.Fields.TryGetValue("FIELD_NAME", out var fieldNameMetadata);
```

### Function Metadata

```csharp
var functionMetadata = connection.GetFunctionMetadata("BAPI_SOME_FUNCTION_NAME");
var functionName = functionMetadata.GetName();
var parameterCount = functionMetadata.Parameters.Count;
foreach (var parameterMetadata in functionMetadata.Parameters)
{
    var parameterName = parameterMetadata.Name;
    var parameterType = parameterMetadata.Type;
}
functionMetadata.Parameters.TryGetValue("PARAMETER_NAME", out var parameterNameMetadata);
```

### Ensure the SAP RFC SDK binaries are present

```csharp
SapLibrary.EnsureLibraryPresent();
```

This will throw an [SapLibraryNotFoundException](/src/SapNwRfc/Exceptions/SapLibraryNotFoundException.cs) with a meaningful message in case the SAP RFC SDK binaries were not found.

## Connection String parameters

The [SapConnection](/src/SapNwRfc/SapConnection.cs) and [SapConnectionPool](/src/SapNwRfc/Pooling/SapConnectionPool.cs) class both take a [SapConnectionParameters](/src/SapNwRfc/SapConnectionParameters.cs) instance or a connection string in the form of:

```csharp
"AppServerHost=MY_SERVER_HOST; SystemNumber=00; User=MY_SAP_USER; Password=SECRET; Client=100; Language=EN; PoolSize=5; Trace=3";
```

<details>
  <summary>Click here to expand the list of supported connection parameters.</summary>

  | Field                        | SAP Field
  |:---------------------------- |:---
  | AppServerHost                | ASHOST
  | SncLibraryPath               | SNC_LIB
  | SncQop                       | SNC_QOP
  | Trace                        | TRACE
  | SapRouter                    | SAPROUTER
  | NoCompression                | NO_COMPRESSION
  | OnCharacterConversionError   | ON_CCE
  | CharacterFaultIndicatorToken | CFIT
  | MaxPoolSize                  | MAX_POOL_SIZE
  | PoolSize                     | POOL_SIZE
  | SncPartnerNames              | SNC_PARTNER_NAMES
  | IdleTimeout                  | IDLE_TIMEOUT
  | MaxPoolWaitTime              | MAX_POOL_WAIT_TIME
  | RegistrationCount            | REG_COUNT
  | PasswordChangeEnforced       | PASSWORD_CHANGE_ENFORCED
  | Name                         | NAME
  | RepositoryDestination        | REPOSITORY_DESTINATION
  | RepositoryUser               | REPOSITORY_USER
  | RepositoryPassword           | REPOSITORY_PASSWD
  | RepositorySncMyName          | REPOSITORY_SNC_MYNAME
  | RepositoryX509Certificate    | REPOSITORY_X509CERT
  | IdleCheckTime                | IDLE_CHECK_TIME
  | SncMyName                    | SNC_MYNAME
  | SncPartnerName               | SNC_PARTNERNAME
  | ProgramId                    | PROGRAM_ID
  | AppServerService             | ASSERV
  | MessageServerHost            | MSHOST
  | MessageServerService         | MSSERV
  | R3Name                       | R3NAME
  | LogonGroup                   | GROUP
  | GatewayHost                  | GWHOST
  | GatewayService               | GWSERV
  | SystemNumber                 | SYSNR
  | User                         | USER
  | AliasUser                    | ALIAS_USER
  | SncMode                      | SNC_MODE
  | Client                       | CLIENT
  | Password                     | PASSWD
  | Codepage                     | CODEPAGE
  | PartnerCharSize              | PCS
  | SystemId                     | SYSID
  | SystemIds                    | SYS_IDS
  | X509Certificate              | X509CERT
  | SapSso2Ticket                | MYSAPSSO2
  | UseSapGui                    | USE_SAPGUI
  | AbapDebug                    | ABAP_DEBUG
  | LogonCheck                   | LCHECK
  | Language                     | LANG
</details>

Additional connection parameters can be added by creating a class that inherits from [SapConnectionParameters](/src/SapNwRfc/SapConnectionParameters.cs):

```csharp
public class MySapConnectionParameters : SapConnectionParameters
{
    [SapName("CST_PARAM")]
    public string CustomParameter { get; set; }
}
```

## Input and output mapping

Input and output models used in function calls are mapped to and from SAP RFC parameter types by convention. In case the property name of the model differs from the SAP RFC parameter name, the `[SapName]`-attribute can be used.

For each input and output model type, the library builds and caches a mapping function using expression trees.

SAP RFC parameter types don't have to be specified as they're converted by convention. Here's an overview of supported type mappings:

| C# type           | SAP RFC type                        | Remarks
|:---------------   |:----------------------------        |:---
| `int`             | RFCTYPE_INT                         | 4-byte integer
| `long`            | RFCTYPE_INT8                        | 8-byte integer
| `double`          | RFCTYPE_FLOAT                       | Floating point, double precision
| `decimal`         | RFCTYPE_BCD                         |
| `string`          | RFCTYPE_STRING / RFCTYPE_CHAR / ... | Gets a data field as string 
| `byte[]`          | RFCTYPE_BYTE                        | Raw binary data, fixed length. Has to be used in conjunction with the `[SapBufferLength]`-attribute
| `char[]`          | RFCTYPE_CHAR                        | Char data, fixed length. Has to be used in conjunction with the `[SapBufferLength]`-attribute
| `DateTime?`       | RFCTYPE_DATE                        | Only the day, month and year value is used
| `TimeSpan?`       | RFCTYPE_TIME                        | Only the hour, minute and second value is used
| `T`               | RFCTYPE_STRUCTURE                   | Structures are constructed from nested objects (T) in the input or output model (see [example](#define-models-with-a-nested-structure))
| `Array<T>`        | RFCTYPE_TABLE                       | Tables are constructed from arrays of nested objects (T) in the input or output model (see [example](#define-models-with-a-nested-table))
| `IEnumerable<T>`  | RFCTYPE_TABLE                       | Tables returned as IEnumerable<T>. Yields elements one-by-one for better memory management when handling large datasets

## Connection pooling

The usage examples above are for simple applications that execute functions one-by-one on a single connection.

For more complex applications that require concurrency, connection pooling and retry on disconnect, it is advised to use the [`SapPooledConnection`](/src/SapNwRfc/Pooling/SapPooledConnection.cs) and [`SapConnectionPool`](/src/SapNwRfc/Pooling/SapConnectionPool.cs) from the `Pooling` namespace.

See the [`SapConnectionPool`](/src/SapNwRfc/Pooling/SapConnectionPool.cs) class for configuration options. 

### Connection pooling in ASP.NET Core application

#### Register pool and pooled connection

In the Startup.cs `ConfigureServices` method, add:

```csharp
services.AddSingleton<ISapConnectionPool>(_ => new SapConnectionPool(connectionString));
services.AddScoped<ISapPooledConnection, SapPooledConnection>();
```

#### Resolve a connection from the pool

In an `ApiController` or service, do:

```csharp
[ApiController]
public class UserController : ControllerBase
{
    public UserController(ISapPooledConnection connection)
    {
        _connection = connection;
    }

    [HttpPost]
    public IActionResult DoSomething(string action)
    {
        _connection.InvokeFunction("BAPI_SOME_FUNCTION_NAME");
        return Ok();
    } 
}
```
