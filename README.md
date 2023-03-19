# Egonsoft.HU JSON Library

[![GitHub](https://img.shields.io/github/license/gcsizmadia/EgonsoftHU.Text.Json?label=License)](https://opensource.org/licenses/MIT)
[![Nuget](https://img.shields.io/nuget/v/EgonsoftHU.Text.Json?label=NuGet)](https://www.nuget.org/packages/EgonsoftHU.Text.Json)
[![Nuget](https://img.shields.io/nuget/dt/EgonsoftHU.Text.Json?label=Downloads)](https://www.nuget.org/packages/EgonsoftHU.Text.Json)

`System.Text.Json` extensions. Includes a `JsonStringEnumConverter` that supports `EnumMemberAttribute`.

## Table of Contents

- [Introduction](#introduction)
- [Releases](#releases)
- [Features](#features)
  * [Naming policies](#naming-policies)
  * [EnumMemberAttribute support](#enummemberattribute-support)
  * [JsonStringEnumConverter](#jsonstringenumconverter)
    + [Serialization](#serialization)
    + [Deserialization](#deserialization)
  * [JsonSerializerOptions extension method](#jsonserializeroptions-extension-method)
  * [Unsupported features](#unsupported-features)

## Introduction

The motivation behind this project is to extend `System.Text.Json` features.

## Releases

You can download the package from [nuget.org](https://www.nuget.org/).
- [EgonsoftHU.Text.Json](https://www.nuget.org/packages/EgonsoftHU.Text.Json)

You can find the release notes [here](https://github.com/gcsizmadia/EgonsoftHU.Text.Json/releases).

## Features

`System.Text.Json` does not implement various naming policies.

The only available `JsonNamingPolicy` is `JsonCamelCaseNamingPolicy` that is available through `JsonNamingPolicy.CamelCase` property.

There are casings that are not (yet) supported by `System.Text.Json`:

- `UPPER_SNAKE_CASE`
- `lower_snake_case`
- `UPPER-KEBAB-CASE`
- `lower-kebab-case`

This library comes with the following features.

### Naming policies

- `EgonsoftHU.Text.Json.JsonUpperCaseNamingPolicy`
- `EgonsoftHU.Text.Json.JsonLowerCaseNamingPolicy`

### EnumMemberAttribute support

Using `EnumMemberAttribute` you can specify the desired output for JSON serialization.

**With no `JsonNamingPolicy`**

```csharp
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(EgonsoftHU.Text.Json.Serialization.JsonStringEnumConverter))]
public enum CalendarWeekRule
{
    [EnumMember(Value = "FIRST_DAY")]
    FirstDay = 0,

    [EnumMember(Value = "FIRST_FULL_WEEK")]
    FirstFullWeek = 1,

    [EnumMember(Value = "FIRST_FOUR_DAY_WEEK")]
    FirstFourDayWeek = 2
}
```

**With `JsonNamingPolicy`**

You can also add it to the list of the converters.

The `EgonsoftHU.Text.Json.JsonStringEnumConverter` accepts a `JsonNamingPolicy` while the value in `EnumMemberAttribute` takes precedence.

```csharp
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddControllers()
    .AddJsonOptions(
        jsonOptions =>
        jsonOptions.JsonSerializerOptions.Converters.Add(
            new EgonsoftHU.Text.Json.Serialization.JsonStringEnumConverter(
                new EgonsoftHU.Text.Json.JsonUpperCaseNamingPolicy()
            )
        )
    );
```

### JsonStringEnumConverter

#### Serialization

`EgonsoftHU.Text.Json.Serialization.JsonStringEnumConverter` takes the possible values for each and every enum member in the following order, from highest to lowest priority:

1. `EnumMemberAttribute.Value` property. (In this case `JsonNamingPolicy` is ignored.)  
   _Except if it is `null`, `String.Empty` or consists only of white-space characters._
2. The enum member name converted using a `JsonNamingPolicy`, if specified.
3. The enum member name as it is written, if no `JsonNamingPolicy` specified.

**Examples**

|`JsonNamingPolicy`|`EnumMemberAttribute`|Original value|Serialized value|
|-|-|-|-|
|not specified|not specified|`CalendarWeekRule.FirstDay`|`FirstDay`|
|not specified|`Value = null`|`CalendarWeekRule.FirstDay`|`FirstDay`|
|not specified|`Value = ""` (`String.Empty`)|`CalendarWeekRule.FirstDay`|`FirstDay`|
|not specified|`Value = " "` (white-space)|`CalendarWeekRule.FirstDay`|`FirstDay`|
|not specified|`Value = "FIRST_DAY"`|`CalendarWeekRule.FirstDay`|`FIRST_DAY`|
|`JsonNamingPolicy.CamelCase`|not specified|`CalendarWeekRule.FirstDay`|`firstDay`|
|`JsonNamingPolicy.CamelCase`|`Value = null`|`CalendarWeekRule.FirstDay`|`firstDay`|
|`JsonNamingPolicy.CamelCase`|`Value = ""` (`String.Empty`)|`CalendarWeekRule.FirstDay`|`firstDay`|
|`JsonNamingPolicy.CamelCase`|`Value = " "` (white-space)|`CalendarWeekRule.FirstDay`|`firstDay`|
|`JsonNamingPolicy.CamelCase`|`Value = "FIRST_DAY"`|`CalendarWeekRule.FirstDay`|`FIRST_DAY`|

#### Deserialization

The converter will check if the JSON string equals to one of the following values:
- `EnumMemberAttribute.Value` property value
- the enum member name converted with the specified `JsonNamingPolicy`
- the enum member name as it is written.

The equality check is case insensitive.

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

### Unsupported features

The current implementation does not support the followings:

- Undefined enum values  
  _`JsonException` will be thrown when trying to serialize._  

- Enum types with `FlagsAttribute`  
  _`EgonsoftHU.Text.Json.Serialization.JsonStringEnumConverter.CanConvert(Type)` method will return `false`._

The support for these will be added later.
