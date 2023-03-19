// Copyright © 2023 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EgonsoftHU.Text.Json.Serialization.Converters
{
    internal sealed class EnumConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return IsEnumWithNoFlagsAttribute(typeToConvert);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return Create(typeToConvert, options, jsonNamingPolicy: null);
        }

        internal static JsonConverter? Create(Type enumType, JsonSerializerOptions jsonSerializerOptions, JsonNamingPolicy? jsonNamingPolicy)
        {
            return (JsonConverter?)Activator.CreateInstance(GetEnumConverterType(enumType), jsonSerializerOptions, jsonNamingPolicy);
        }

        private static Type GetEnumConverterType(Type type)
        {
            return typeof(EnumConverter<>).MakeGenericType(type);
        }

        internal static bool IsEnumWithNoFlagsAttribute(Type typeToConvert)
        {
            TypeInfo typeInfo = typeToConvert.GetTypeInfo();

            return typeInfo.IsEnum && typeInfo.GetCustomAttribute<FlagsAttribute>() is null;
        }
    }
}
