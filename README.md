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
  **Tip:**  
  If you use an earlier version of `System.Text.Json` and target .NET 8 or earlier  then you can use this attribute by copying it into your project.
  The fully qualified name of the type, including its namespace (but not its assembly, of course) must match.
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
