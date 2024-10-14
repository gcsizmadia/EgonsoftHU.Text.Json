// Copyright © 2023-2024 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using EgonsoftHU.Extensions.Bcl.Enumerations;
using EgonsoftHU.Extensions.Bcl.Enumerations.Serialization;
using EgonsoftHU.Text.Json.Serialization.Converters;

namespace EgonsoftHU.Text.Json.Serialization
{
    /// <summary>
    /// Converter to convert enums to and from strings.
    /// </summary>
    /// <remarks>
    /// Reading and writing can be customized by the following options:
    /// <list type="bullet">
    /// <item><see cref="JsonNamingPolicy"/></item>
    /// <item>An implementation of the <see cref="EnumValueSerializer"/> base class.</item>
    /// </list>
    /// <para>
    /// As an implementation of the <see cref="EnumValueSerializer"/> base class you can use:
    /// <list type="bullet">
    /// <item>either an instance of the <see cref="JsonEnumValueSerializer"/> class with a specified <see cref="JsonNamingPolicy"/></item>
    /// <item>or your custom implementation derived from the <see cref="EnumValueSerializer"/> class.</item>
    /// </list>
    /// </para>
    /// <para>
    /// For your custom implementation you can use <see cref="EnumInfo{TEnum}.Attributes"/> property that contains
    /// all the custom attributes applied to an enum member.
    /// </para>
    /// </remarks>
    public class JsonStringEnumConverter : JsonConverterFactory
    {
        private readonly EnumConverterOptions converterOptions;
        private readonly JsonNamingPolicy? jsonNamingPolicy;
        private readonly Func<JsonSerializerOptions, JsonNamingPolicy?, EnumValueSerializer> enumValueSerializerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonStringEnumConverter"/> class that allows integer values.
        /// </summary>
        public JsonStringEnumConverter()
            : this(allowIntegerValues: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonStringEnumConverter"/> class with
        /// a specified naming policy,
        /// a value that indicates whether undefined enumeration values are allowed and
        /// a factory delegate to create an instance of the <see cref="EnumValueSerializer"/> class.
        /// </summary>
        /// <param name="enumValueSerializerFactory">
        /// The optional factory delegate to create an instance of the <see cref="EnumValueSerializer"/> class.
        /// <para>If not specified then <see cref="EnumValueSerializer.Current"/> property will be used.</para>
        /// </param>
        /// <param name="jsonNamingPolicy">The optional naming policy for writing enum values.</param>
        /// <param name="allowIntegerValues">
        /// <see langword="true"/> to allow undefined enum values; otherwise, <see langword="false"/>.
        /// When <see langword="true"/>, if an enum value isn't defined, it will output as a number rather than a string.
        /// </param>
        public JsonStringEnumConverter(
            JsonNamingPolicy? jsonNamingPolicy = null,
            bool allowIntegerValues = true,
            Func<JsonSerializerOptions, JsonNamingPolicy?, EnumValueSerializer>? enumValueSerializerFactory = null
        )
        {
            this.jsonNamingPolicy = jsonNamingPolicy;

            converterOptions =
                allowIntegerValues
                    ? EnumConverterOptions.AllowStrings | EnumConverterOptions.AllowNumbers
                    : EnumConverterOptions.AllowStrings;

            this.enumValueSerializerFactory =
                enumValueSerializerFactory
                ??
                ((_, __) => EnumValueSerializer.Current);
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        /// <inheritdoc/>
        public sealed override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return
                EnumConverterFactory.Create(
                    typeToConvert,
                    options,
                    jsonNamingPolicy,
                    converterOptions,
                    enumValueSerializerFactory
                );
        }
    }
}
