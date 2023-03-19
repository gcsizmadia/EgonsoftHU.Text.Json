// Copyright © 2023 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

using EgonsoftHU.Extensions.Bcl;

namespace EgonsoftHU.Text.Json.Serialization.Converters
{
    internal sealed class EnumConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : struct, Enum
    {
        private readonly ConcurrentDictionary<ulong, JsonEncodedText> nameCache = new();
        private readonly ConcurrentDictionary<ulong, string> convertedNameCache = new();

        public EnumConverter(JsonSerializerOptions jsonSerializerOptions, JsonNamingPolicy? jsonNamingPolicy)
        {
            JavaScriptEncoder? encoder = jsonSerializerOptions.Encoder;

            EnumMemberInfo.InitializeCache<TEnum>();

            foreach (EnumMemberInfo<TEnum> enumMemberInfo in EnumMemberInfo<TEnum>.Cache.Values)
            {
                ulong key = enumMemberInfo.UInt64Value;

                string? convertedName = jsonNamingPolicy?.ConvertName(enumMemberInfo.MemberName);

                if (!convertedName.IsNullOrWhiteSpace())
                {
                    convertedNameCache.TryAdd(key, convertedName);
                }

                string valueToSerialize =
                    enumMemberInfo.MemberValue.IsNullOrWhiteSpace()
                        ? convertedName ?? enumMemberInfo.MemberName
                        : enumMemberInfo.MemberValue;

                nameCache.TryAdd(key, JsonEncodedText.Encode(valueToSerialize, encoder));
            }
        }

        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                {
                    string? serializedValue = reader.GetString();

                    EnumMemberInfo<TEnum>? result =
                        EnumMemberInfo<TEnum>.Cache.Values.SingleOrDefault(
                            enumMemberInfo =>
                            String.Equals(serializedValue, enumMemberInfo.MemberValue, StringComparison.OrdinalIgnoreCase)
                            ||
                            String.Equals(serializedValue, enumMemberInfo.MemberName, StringComparison.OrdinalIgnoreCase)
                            ||
                            (
                                convertedNameCache.TryGetValue(enumMemberInfo.UInt64Value, out string? convertedName)
                                &&
                                String.Equals(serializedValue, convertedName, StringComparison.OrdinalIgnoreCase)
                            )
                        );

                    return result?.Value ?? throw new JsonException(ErrorMessages.InvalidToken);
                }

                case JsonTokenType.Number:
                default:
                    throw new JsonException(ErrorMessages.InvalidToken);
            }
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            var enumMemberInfo = EnumMemberInfo.GetMemberInfo(value);

            ulong key = enumMemberInfo.UInt64Value;

            if (!nameCache.TryGetValue(key, out JsonEncodedText name))
            {
                throw new JsonException(ErrorMessages.InvalidEnumValue);
            }

            writer.WriteStringValue(name);
        }

        private static class ErrorMessages
        {
            public const string InvalidToken = "Invalid token.";

            public const string InvalidEnumValue = "Invalid enum value.";
        }
    }
}
