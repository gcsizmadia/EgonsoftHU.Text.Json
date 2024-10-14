// Copyright © 2023-2024 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using EgonsoftHU.Extensions.Bcl.Enumerations.Serialization;

namespace EgonsoftHU.Text.Json.Serialization.Converters
{
    internal sealed class EnumConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return Create(typeToConvert, options, null, EnumConverterOptions.AllowNumbers, (_, __) => EnumValueSerializer.Current);
        }

        internal static JsonConverter? Create(
            Type enumType,
            JsonSerializerOptions options,
            JsonNamingPolicy? jsonNamingPolicy,
            EnumConverterOptions converterOptions,
            Func<JsonSerializerOptions, JsonNamingPolicy?, EnumValueSerializer> enumValueSerializerFactory
        )
        {
            return
                (JsonConverter?)Activator.CreateInstance(
                    GetEnumConverterType(enumType),
                    converterOptions,
                    enumValueSerializerFactory.Invoke(options, jsonNamingPolicy),
                    options
                );
        }

        private static Type GetEnumConverterType(Type enumType)
        {
            return typeof(EnumConverter<>).MakeGenericType(enumType);
        }
    }
}
