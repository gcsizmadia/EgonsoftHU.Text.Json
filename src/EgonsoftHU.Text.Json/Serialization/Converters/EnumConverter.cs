// Copyright © 2023-2024 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using EgonsoftHU.Extensions.Bcl;
using EgonsoftHU.Extensions.Bcl.Enumerations;
using EgonsoftHU.Extensions.Bcl.Enumerations.Serialization;
using EgonsoftHU.Text.Json.Exceptions;

namespace EgonsoftHU.Text.Json.Serialization.Converters
{
    internal sealed class EnumConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : struct, Enum
    {
        private readonly EnumConverterOptions converterOptions;
        private readonly EnumValueSerializer enumValueSerializer;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public EnumConverter(
            EnumConverterOptions converterOptions,
            EnumValueSerializer enumValueSerializer,
            JsonSerializerOptions jsonSerializerOptions
        )
        {
            this.converterOptions = converterOptions;
            this.enumValueSerializer = enumValueSerializer;
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                {
                    if (!converterOptions.HasFlag(EnumConverterOptions.AllowStrings))
                    {
                        goto default;
                    }

                    string? serializedValue = reader.GetString();

                    if (serializedValue.IsNullOrWhiteSpace())
                    {
                        goto default;
                    }

                    bool isInCache = NameCache.ForReading.TryGetValue(serializedValue, out EnumInfo<TEnum>? enumeration);

                    EnumInfo<TEnum>? result =
                        isInCache
                            ? enumeration
                            : enumValueSerializer.Deserialize<TEnum>(serializedValue);

                    if (result is not null)
                    {
                        if (!isInCache)
                        {
                            NameCache.ForReading.TryAdd(serializedValue, result);
                        }

                        return result.Value;
                    }

                    goto default;
                }

                case JsonTokenType.Number:
                {
                    if (!converterOptions.HasFlag(EnumConverterOptions.AllowNumbers))
                    {
                        goto default;
                    }

                    return
                        EnumInfo<TEnum>.EnumTypeCode switch
                        {
                            TypeCode.SByte => GetValue(reader.TryGetSByte(out sbyte value), ref value),
                            TypeCode.Int16 => GetValue(reader.TryGetInt16(out short value), ref value),
                            TypeCode.Int32 => GetValue(reader.TryGetInt32(out int value), ref value),
                            TypeCode.Int64 => GetValue(reader.TryGetInt64(out long value), ref value),
                            TypeCode.Byte => GetValue(reader.TryGetByte(out byte value), ref value),
                            TypeCode.UInt16 => GetValue(reader.TryGetUInt16(out ushort value), ref value),
                            TypeCode.UInt32 => GetValue(reader.TryGetUInt32(out uint value), ref value),
                            TypeCode.UInt64 => GetValue(reader.TryGetUInt64(out ulong value), ref value),
                            _ => throw JsonExceptions.InvalidToken()
                        };
                }

                default:
                    throw JsonExceptions.InvalidToken();
            }
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            if (converterOptions.HasFlag(EnumConverterOptions.AllowStrings))
            {
                if (EnumInfo.TryFromValue(value, out EnumInfo<TEnum>? enumeration))
                {
                    if (!NameCache.ForWriting.TryGetValue(enumeration.UInt64Value, out JsonEncodedText text))
                    {
                        string serializedValue = enumValueSerializer.Serialize(enumeration);

                        text = JsonEncodedText.Encode(serializedValue, jsonSerializerOptions.Encoder);

                        NameCache.ForWriting.TryAdd(enumeration.UInt64Value, text);
                    }

                    writer.WriteStringValue(text);
                    return;
                }
            }

            if (converterOptions.HasFlag(EnumConverterOptions.AllowNumbers))
            {
                switch (EnumInfo<TEnum>.EnumTypeCode)
                {
                    case TypeCode.SByte:
                        writer.WriteNumberValue(Unsafe.As<TEnum, sbyte>(ref value));
                        return;

                    case TypeCode.Int16:
                        writer.WriteNumberValue(Unsafe.As<TEnum, short>(ref value));
                        return;

                    case TypeCode.Int32:
                        writer.WriteNumberValue(Unsafe.As<TEnum, int>(ref value));
                        return;

                    case TypeCode.Int64:
                        writer.WriteNumberValue(Unsafe.As<TEnum, long>(ref value));
                        return;

                    case TypeCode.Byte:
                        writer.WriteNumberValue(Unsafe.As<TEnum, byte>(ref value));
                        return;

                    case TypeCode.UInt16:
                        writer.WriteNumberValue(Unsafe.As<TEnum, ushort>(ref value));
                        return;

                    case TypeCode.UInt32:
                        writer.WriteNumberValue(Unsafe.As<TEnum, uint>(ref value));
                        return;

                    case TypeCode.UInt64:
                        writer.WriteNumberValue(Unsafe.As<TEnum, ulong>(ref value));
                        return;

                    default:
                        break;
                }
            }

            throw JsonExceptions.InvalidEnumValue();
        }

        private static TEnum GetValue<TUnderlying>(bool readSuccess, ref TUnderlying underlyingValue)
            where TUnderlying : struct
        {
            return
                readSuccess
                    ? Unsafe.As<TUnderlying, TEnum>(ref underlyingValue)
                    : throw JsonExceptions.InvalidToken();
        }

        internal static class NameCache
        {
            internal static ConcurrentDictionary<ulong, JsonEncodedText> ForWriting { get; } = new();

            internal static ConcurrentDictionary<string, EnumInfo<TEnum>> ForReading { get; } = new();
        }
    }
}
