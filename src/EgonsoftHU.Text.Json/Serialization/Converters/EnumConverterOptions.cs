// Copyright © 2023-2024 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;

namespace EgonsoftHU.Text.Json.Serialization.Converters
{
    [Flags]
    internal enum EnumConverterOptions
    {
        AllowStrings = 1,
        AllowNumbers = 2
    }
}
