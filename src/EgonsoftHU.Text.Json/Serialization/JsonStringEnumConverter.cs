// Copyright © 2023 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

using EgonsoftHU.Text.Json.Serialization.Converters;

namespace EgonsoftHU.Text.Json.Serialization
{
    /// <summary>
    /// Converter to convert enums to and from strings.
    /// </summary>
    /// <remarks>
    /// Reading is case insensitive, writing can be customized
    /// via a <see cref="JsonNamingPolicy" /> or
    /// via using <see cref="EnumMemberAttribute"/> on the enum member.
    /// <para>
    /// PLEASE NOTE:
    /// <list type="bullet">
    /// <item>Undefined enum values are not supported.</item>
    /// <item>Enum types with <see cref="FlagsAttribute"/> are not supported</item>
    /// </list>
    /// In both cases a <see cref="JsonException"/> will be thrown.
    /// </para>
    /// </remarks>
    public class JsonStringEnumConverter : JsonConverterFactory
    {
        private readonly JsonNamingPolicy? jsonNamingPolicy;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonStringEnumConverter"/> class.
        /// </summary>
        public JsonStringEnumConverter()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonStringEnumConverter"/> class with the specified naming policy.
        /// </summary>
        /// <param name="jsonNamingPolicy">The naming policy used to convert a string-based name to another format, such as a camel-casing format.</param>
        public JsonStringEnumConverter(JsonNamingPolicy? jsonNamingPolicy = null)
        {
            this.jsonNamingPolicy = jsonNamingPolicy;
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
        {
            return EnumConverterFactory.IsEnumWithNoFlagsAttribute(typeToConvert);
        }

        /// <inheritdoc/>
        public sealed override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return EnumConverterFactory.Create(typeToConvert, options, jsonNamingPolicy);
        }
    }
}
