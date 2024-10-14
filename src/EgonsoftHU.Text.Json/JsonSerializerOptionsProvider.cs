// Copyright © 2023-2024 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System.Runtime.Serialization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using EgonsoftHU.Extensions.Bcl.Enumerations.Serialization;
using EgonsoftHU.Text.Json.Serialization;
using EgonsoftHU.Text.Json.Serialization.Converters;

namespace EgonsoftHU.Text.Json
{
    /// <summary>
    /// Provides a read-only default and a writable current instance of the <see cref="JsonSerializerOptions"/> class.
    /// </summary>
    public static class JsonSerializerOptionsProvider
    {
        private static readonly EnumValueSerializer defaultEnumValueSerializer;

        static JsonSerializerOptionsProvider()
        {
            defaultEnumValueSerializer = new JsonEnumValueSerializer(JsonNamingPolicy.CamelCase);

            Default = CreateDefault(writeIndented: false);
            DefaultWriteIndented = CreateDefault(writeIndented: true);
            Current = CreateDefault(writeIndented: false);

            EnumValueSerializer.Current = defaultEnumValueSerializer;
        }

        /// <summary>
        /// Gets the default instance of the <see cref="JsonSerializerOptions"/> type.
        /// </summary>
        /// <remarks>
        /// The following options are set:
        /// <list type="bullet">
        /// <item><see cref="JsonSerializerDefaults.Web"/> is used.</item>
        /// <item>
        /// <see cref="JsonSerializerOptions.Encoder"/> is set to an instance of the <see cref="JavaScriptEncoder"/> class
        /// that specifies <see cref="UnicodeRanges.All"/> the encoder is allowed to not encode.
        /// </item>
        /// <item><see cref="JsonSerializerOptions.WriteIndented"/> is set to <see langword="false"/>.</item>
        /// <item>
        /// An instance of the <see cref="JsonStringEnumConverter"/> class is added to the <see cref="JsonSerializerOptions.Converters"/> collection
        /// that uses:
        /// <list type="bullet">
        /// <item>An instance of the <see cref="JsonEnumValueSerializer"/> class with the following serialization options in order of priority:</item>
        /// <list type="bullet">
        /// <item><see cref="EnumMemberAttribute.Value"/></item>
        /// <item><see cref="JsonNamingPolicy.CamelCase"/></item>
        /// <item>The enum member's name.</item>
        /// </list>
        /// </list>
        /// </item>
        /// </list>
        /// </remarks>
        public static JsonSerializerOptions Default { get; }

        /// <summary>
        /// Gets the default instance of the <see cref="JsonSerializerOptions"/> type.
        /// </summary>
        /// <remarks>
        /// The following options are set:
        /// <list type="bullet">
        /// <item><see cref="JsonSerializerDefaults.Web"/> is used.</item>
        /// <item>
        /// <see cref="JsonSerializerOptions.Encoder"/> is set to an instance of the <see cref="JavaScriptEncoder"/> class
        /// that specifies <see cref="UnicodeRanges.All"/> the encoder is allowed to not encode.
        /// </item>
        /// <item><see cref="JsonSerializerOptions.WriteIndented"/> is set to <see langword="true"/>.</item>
        /// <item>
        /// An instance of the <see cref="JsonStringEnumConverter"/> class is added to the <see cref="JsonSerializerOptions.Converters"/> collection
        /// that uses:
        /// <list type="bullet">
        /// <item>An instance of the <see cref="JsonEnumValueSerializer"/> class with the following serialization options in order of priority:</item>
        /// <list type="bullet">
        /// <item><see cref="EnumMemberAttribute.Value"/></item>
        /// <item><see cref="JsonNamingPolicy.CamelCase"/></item>
        /// <item>The enum member's name.</item>
        /// </list>
        /// </list>
        /// </item>
        /// </list>
        /// </remarks>
        public static JsonSerializerOptions DefaultWriteIndented { get; }

        /// <summary>
        /// Gets or sets the current instance of the <see cref="JsonSerializerOptions"/> class.
        /// </summary>
        /// <remarks>
        /// By default, it is an instance with the same settings as the one returned by the <see cref="Default"/> property.
        /// <para>
        /// If you set an instance that uses <see cref="JsonStringEnumConverter"/> with an implementation of <see cref="EnumValueSerializer"/>
        /// then it is recommended to also set the <see cref="EnumValueSerializer.Current"/> property to that implementation.
        /// </para>
        /// </remarks>
        public static JsonSerializerOptions Current { get; set; }

        private static JsonSerializerOptions CreateDefault(bool writeIndented)
        {
            JsonSerializerOptions options =
                new(JsonSerializerDefaults.Web)
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = writeIndented
                };

            options.Converters.Add(
                new JsonStringEnumConverter(
                    JsonNamingPolicy.CamelCase,
                    enumValueSerializerFactory: (jsonSerializerOptions, jsonNamingPolicy) => defaultEnumValueSerializer
                )
            );

            return options;
        }
    }
}
