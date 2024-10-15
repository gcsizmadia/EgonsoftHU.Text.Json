# Egonsoft.HU JSON Library

[![GitHub](https://img.shields.io/github/license/gcsizmadia/EgonsoftHU.Text.Json?label=License)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/EgonsoftHU.Text.Json?label=NuGet)](https://www.nuget.org/packages/EgonsoftHU.Text.Json)
[![Nuget](https://img.shields.io/nuget/dt/EgonsoftHU.Text.Json?label=Downloads)](https://www.nuget.org/packages/EgonsoftHU.Text.Json)

`System.Text.Json` extensions.

- Supports any attribute for enum member serialization.
- Provides option for using your own custom serializer implementation for enum members.
- Many more...

## Table of Contents

- [Introduction](#introduction)
- [Releases](#releases)
- [Features](#features)
  * [EnumValueSerializer](#enumvalueserializer)
    + [Default implementation](#default-implementation)
  * [JsonEnumValueSerializer](#jsonenumvalueserializer)
    + [Value precedence](#value-precedence)
  * [Custom implementation of EnumValueSerializer](#custom-implementation-of-enumvalueserializer)
  * [JsonStringEnumConverter](#jsonstringenumconverter)
  * [JsonSerializerOptionsProvider](#jsonserializeroptionsprovider)
  * [JsonSerializerOptions extension method](#jsonserializeroptions-extension-method)
  * [Naming policies](#naming-policies)
- [Example configuration](#example-configuration)
- [JsonStringEnumMemberNameAttribute support](#jsonstringenummembernameattribute-support)

## Introduction

The motivation behind this project is to extend `System.Text.Json` features.

## Releases

You can download the package from [nuget.org](https://www.nuget.org/).
- [EgonsoftHU.Text.Json](https://www.nuget.org/packages/EgonsoftHU.Text.Json)

You can find the release notes [here](https://github.com/gcsizmadia/EgonsoftHU.Text.Json/releases).

## Features

This library comes with the following features.

### EnumValueSerializer

This library can make use of an implementation of this base class provided by the `EgonsoftHU.Extensions.Bcl` (`>= 3.0.0`) nuget package.

It has a static `Current` property that can be set to your custom implementation.

#### Default implementation

By default, the `Current` property is set to a default implementation that supports the following attribute:
- `System.Runtime.Serialization.EnumMemberAttribute`

Converting / retrieving an enum member value to / from a string uses these values in the following order, from highest to lowest priority:

1. The value of the `EnumMemberAttribute.Value` property, if that attribute is applied to the current enum member.
   - Both reading and writing are case sensitive.
2. The name of the enum member.
   - Reading is case insensitive.
   - Writing is case sensitive.

**Please note:**  
The base class makes use of the `EgonsoftHU.Extensions.Bcl.Enumerations.EnumInfo<TEnum>` type.

When accessing a specific `EnumInfo<TEnum>` type then its `SerializedValue` instance property
will be initialized by the current serializer set in the `EnumValueSerializer.Current` property;
therefore, it is highly recommended to set that property to the same implementation you will use
for configuring JSON serializer options.

### JsonEnumValueSerializer

This is an implementation of the above mentioned `EgonsoftHU.Extensions.Bcl.Enumerations.Serialization.EnumValueSerializer` type and
it supports attributes and `System.Text.Json.JsonNamingPolicy` to convert/retrieve enum member values to/from a string.

The following attributes are supported:

- `System.Text.Json.Serialization.JsonStringEnumMemberNameAttribute` (available in `System.Text.Json` (`>= 9.0.0`))  
- `System.Runtime.Serialization.EnumMemberAttribute`
- `System.ComponentModel.DescriptionAttribute`
- `System.ComponentModel.DisplayNameAttribute`
- `System.ComponentModel.DataAnnotations.DisplayAttribute` (`Description`, `Name` and `ShortName` properties are supported.)

To create an instance and select an attribute using the `EgonsoftHU.Text.Json.Serialization.EnumMemberNameSelectorOption` enum type:

```csharp
using System.Text.Json;
using EgonsoftHU.Text.Json.Serialization;
using EgonsoftHU.Text.Json.Serialization.Converters;

var serializer =
    new JsonEnumValueSerializer(
        JsonNamingPolicy.CamelCase,
        // The second parameter is optional. This is the default value:
        EnumMemberNameSelectorOption.UseJsonStringEnumMemberNameAttribute
    );
```

#### Value precedence

Converting / retrieving an enum member value to / from a string uses these values in the following order, from highest to lowest priority:

1. The value of the selected attribute, if that attribute is applied to the current enum member.
   - Both reading and writing are case sensitive.
2. The value produced by the specified `JsonNamingPolicy`.
   - Both reading and writing are case sensitive.
3. The name of the enum member.
   - Reading is case insensitive.
   - Writing is case sensitive.

### Custom implementation of EnumValueSerializer

You can create and use your own `EnumValueSerializer` implementation if the above mentioned
`JsonEnumValueSerializer` implementation does not fit your needs.

Additionally, you can use the `EnumInfo<TEnum>.Attributes` instance property to access all the
attributes that are applied to an enum member.

### JsonStringEnumConverter

This is an implementation of the `System.Text.Json.Serialization.JsonConverterFactory` that you can add to the
`System.Text.Json.JsonSerializerOptions.Converters` collection.

You can specify
- a `JsonNamingPolicy`
- a factory delegate to create an instance of an implementation of the `EnumValueSerializer` type using
  an instance of `JsonSerializerOptions` and an instance of `JsonNamingPolicy`.

### JsonSerializerOptionsProvider

This is a static class that provides
- read-only default (`Default`, `DefaultWriteIndented`) properties and
- a writable current (`Current`) property

to hold instances of the `System.Text.Json.JsonSerializerOptions` type.

The default instance is created this way:

```csharp
// EgonsoftHU namespaces are displayed only for clarity.

var defaultEnumValueSerializer =
    new EgonsoftHU.Text.Json.Serialization.Converters.JsonEnumValueSerializer(JsonNamingPolicy.CamelCase);

EgonsoftHU.Extensions.Bcl.Enumerations.Serialization.EnumValueSerializer.Current = defaultEnumValueSerializer;

JsonSerializerOptions options =
    new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = false
    };

options.Converters.Add(
    new EgonsoftHU.Text.Json.Serialization.JsonStringEnumConverter(
        JsonNamingPolicy.CamelCase,
        enumValueSerializerFactory: (jsonSerializerOptions, jsonNamingPolicy) => defaultEnumValueSerializer
    )
);
```

### JsonSerializerOptions extension method

Suppose you already have an existing instance of the `JsonSerializerOptions` class that you would like to use when configuring ASP.NET Core.

This `CopyTo()` extension method comes in handy in this case:

```csharp
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

JsonSerializerOptions options = /* your already existing instance */;

builder
    .Services
    .AddControllers()
    // source.CopyTo(target) will override target's settings with the source's settings.
    .AddJsonOptions(jsonOptions => options.CopyTo(jsonOptions.JsonSerializerOptions));
```

### Naming policies

Although these are not the most wanted naming policies, they are available in case you need them.

- `EgonsoftHU.Text.Json.JsonUpperCaseNamingPolicy`
- `EgonsoftHU.Text.Json.JsonLowerCaseNamingPolicy`

## Example configuration

In this example configuration converting / retrieving an enum member value to / from a string
will use these values in the following order, from highest to lowest priority:

1. `JsonStringEnumMemberNameAttribute.Name`, if that attribute is applied to the current enum member.
   - Both reading and writing are case sensitive.
2. The value produced by `JsonNamingPolicy.CamelCase`.
   - Both reading and writing are case sensitive.
3. The name of the enum member.
   - Reading is case insensitive.
   - Writing is case sensitive.

```csharp
using System.Text.Json;
using EgonsoftHU.Text.Json.Serialization;
using EgonsoftHU.Text.Json.Serialization.Converters;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

JsonSerializerOptions options =
    new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = false
    };

options.Converters.Add(
    new JsonStringEnumConverter(
        JsonNamingPolicy.CamelCase
        // If omitting the delegate factory then
        // the value of the EnumValueSerializer.Current property will be used.
    )
);

EnumValueSerializer.Current = new JsonEnumValueSerializer(JsonNamingPolicy.CamelCase);
JsonSerializerOptionsProvider.Current = options;

builder
    .Services
    .AddControllers()
    .AddJsonOptions(jsonOptions => options.CopyTo(jsonOptions.JsonSerializerOptions));
```

## JsonStringEnumMemberNameAttribute support

### If you target .NET 9

It already includes the newer (but not stable) `9.0.0` version of `System.Text.Json` nuget package
that contains this attribute.

So this package - since it targets `net9.0` as well - directly uses this attribute type.

### If you target a framework other than .NET 9

For target frameworks other than `net9.0` currently this package references the latest stable
(`8.0.5`) version of the `System.Text.Json` nuget package that does not contain this attribute.

It means that in this case the `JsonEnumValueSerializer` checks for this attribute by name and uses
reflection to get the value of its `Name` property.

This package will be updated to use the `9.0.0` version of the `System.Text.Json` nuget package
as soon as a stable version of it is available.

### Using this attribute

Since the `JsonEnumValueSerializer` can check for this attribute by name you can start using
(or migrate to) this attribute without the need to reference the newer (but not stable)
`9.0.0` version of `System.Text.Json` nuget package.

All you need to do is to create an attribute with the same name (`JsonStringEnumMemberNameAttribute`)
within the same namespace (`System.Text.Json.Serialization`) in your project.

You can find the source code for this attribute here:  
[JsonStringEnumMemberNameAttribute](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/JsonStringEnumMemberNameAttribute.cs)

**Please note:**  
For this workaround to work neither the copy of the attribute nor the enum types using that copy
should be in an assembly that references the `9.0.0` version of the `System.Text.Json` nuget package.
Otherwise, you will get either a
[CS0436 compiler warning](https://learn.microsoft.com/en-us/dotnet/csharp/misc/cs0436) or a
[CS0433 compiler error](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/cs0433).

*Important:*
`System.Text.Json` will not target `net6.0` starting with version `9.0.0` which means
that the `net6.0` version of this package will keep using the `8.0.5` version of it,
hence you can keep your copy of the attribute in this case.

#### AppContext switch

To be on the safe side you also need to set a switch at application startup so that
the `JsonEnumValueSerializer` will check for this attribute always by name
even if you upgrade to the `9.0.0` version of the `System.Text.Json` nuget package.

Option #1 - Set the switch in the code at application startup

```csharp
using System;
using EgonsoftHU.Text.Json.Constants;

AppContext.SetSwitch(AppContextSwitches.AlwaysCheckForJsonStringEnumMemberAttributeByName, true);
```

Option #2 - Set the switch in the startup project's .csproj file (in case of an SDK-style project)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <!-- Rest is omitted for clarity -->
  <ItemGroup>
    <RuntimeHostConfigurationOption
      Include="Switch.EgonsoftHU.Text.Json.AlwaysCheckForJsonStringEnumMemberAttributeByName"
      Value="true"
    />
  </ItemGroup>
</Project>
```

Option #3 - Set the switch in the startup project's App.config file (in case of a classic .NET Framework project)

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <!-- Rest is omitted for clarity -->
  <runtime>
    <AppContextSwitchOverrides
      value="Switch.EgonsoftHU.Text.Json.AlwaysCheckForJsonStringEnumMemberAttributeByName=true"
    />
  </runtime>
</configuration>
```

**Please note:**  
Before removing this switch after upgrading to the `9.0.0` version of the `System.Text.Json` nuget package,
make sure you deleted your copy of the attribute and your enum types reference the original attribute.
