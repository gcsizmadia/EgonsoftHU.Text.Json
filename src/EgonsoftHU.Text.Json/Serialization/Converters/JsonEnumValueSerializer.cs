// Copyright © 2023-2024 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Linq;
using System.Text.Json;

#if NET9_0_OR_GREATER
using System.Text.Json.Serialization;
#else
using EgonsoftHU.Extensions.Bcl;
#endif

using EgonsoftHU.Extensions.Bcl.Constants;
using EgonsoftHU.Extensions.Bcl.Enumerations;
using EgonsoftHU.Extensions.Bcl.Enumerations.Serialization;

namespace EgonsoftHU.Text.Json.Serialization.Converters
{
    /// <summary>
    /// An implementation of the <see cref="EnumValueSerializer"/> base class that uses attributes and a JSON naming policy
    /// to convert/retrieve an enum member value to/from a string.
    /// </summary>
    public class JsonEnumValueSerializer : EnumValueSerializer
    {
        private readonly JsonNamingPolicy? jsonNamingPolicy;
        private readonly EnumMemberNameSelectorOption nameSelectorOption;

        /// <summary>
        /// Initializes an instance of the <see cref="JsonEnumValueSerializer"/> class with the specified <see cref="JsonNamingPolicy"/>.
        /// </summary>
        /// <param name="jsonNamingPolicy">The optional naming policy for reading and writing enum values.</param>
        /// <param name="nameSelectorOption"></param>
        public JsonEnumValueSerializer(
            JsonNamingPolicy? jsonNamingPolicy = null,
            EnumMemberNameSelectorOption nameSelectorOption = EnumMemberNameSelectorOption.UseJsonStringEnumMemberNameAttribute
        )
        {
            if (!EnumInfo<EnumMemberNameSelectorOption>.TryFromValue(nameSelectorOption, out _))
            {
                throw new ArgumentOutOfRangeException(nameof(nameSelectorOption));
            }

            this.jsonNamingPolicy = jsonNamingPolicy;
            this.nameSelectorOption = nameSelectorOption;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The conversion methods in order of priority:
        /// <list type="bullet">
        /// <item>
        /// The value of the attribute selected by <see cref="EnumMemberNameSelectorOption"/>,
        /// if that attribute is applied to the current enum member.
        /// </item>
        /// <item><see cref="JsonNamingPolicy"/>, if specified.</item>
        /// <item>The enum member's name.</item>
        /// </list>
        /// </remarks>
        public override string Serialize<TEnum>(EnumInfo<TEnum> enumeration)
        {
            var serializedValues =
                enumeration
                    .Select(
                        flag =>
                            GetEnumMemberName(flag.Attributes)
                            ??
                            jsonNamingPolicy?.ConvertName(flag.Name)
                            ??
                            flag.Name
                    )
                    .ToList();

            string serializedValue =
                serializedValues.Count == 1
                    ? serializedValues[0]
                    : String.Join(Strings.CommaSpaceSeparator, serializedValues);

            return serializedValue;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The <paramref name="serializedValue"/> will be compared to the following values, in order of priority:
        /// <list type="bullet">
        /// <item>
        /// The value of the attribute selected by <see cref="EnumMemberNameSelectorOption"/>,
        /// if that attribute is applied to the current enum member.
        /// </item>
        /// <item>The value produced by the specified <see cref="JsonNamingPolicy"/>.</item>
        /// <item>The enum member's name. The comparing is case insensitive.</item>
        /// </list>
        /// </remarks>
        public override EnumInfo<TEnum>? Deserialize<TEnum>(string serializedValue)
        {
            string[] names = GetNames(serializedValue);

            var membersFound =
                names
                    .Select(
                        name =>
                            EnumInfo<TEnum>.DeclaredMembers.FirstOrDefault(
                                member =>
                                (
                                    GetEnumMemberName(member.Attributes) is string attributeName
                                    &&
                                    String.Equals(name, attributeName, StringComparison.Ordinal)
                                )
                                ||
                                (
                                    jsonNamingPolicy is not null
                                    &&
                                    String.Equals(name, jsonNamingPolicy.ConvertName(member.Name), StringComparison.Ordinal)
                                )
                                ||
                                String.Equals(name, member.Name, StringComparison.OrdinalIgnoreCase))
                    )
                    .OfType<EnumInfo<TEnum>>()
                    .ToList();

            EnumInfo<TEnum>? enumeration =
                membersFound.Count switch
                {
                    1 => membersFound[0],
                    > 1 => membersFound.Aggregate((first, second) => first | second),
                    _ => EnumInfo<TEnum>.Default
                };

            return enumeration;
        }

        private string? GetEnumMemberName(IEnumerationAttributes attributes)
        {
            return nameSelectorOption switch
            {
                EnumMemberNameSelectorOption.UseJsonStringEnumMemberNameAttribute => GetJsonStringEnumMemberName(attributes),
                EnumMemberNameSelectorOption.UseEnumMemberAttribute => attributes.EnumMember?.Value,
                EnumMemberNameSelectorOption.UseDescriptionAttribute => attributes.Description?.Description,
                EnumMemberNameSelectorOption.UseDisplayNameAttribute => attributes.DisplayName?.DisplayName,
                EnumMemberNameSelectorOption.UseDisplayAttributeGetName => attributes.Display?.GetName(),
                EnumMemberNameSelectorOption.UseDisplayAttributeGetShortName => attributes.Display?.GetShortName(),
                EnumMemberNameSelectorOption.UseDisplayAttributeGetDescription => attributes.Display?.GetDescription(),
                _ => throw new InvalidOperationException("Invalid enum value.")
            };
        }

        private static string? GetJsonStringEnumMemberName(IEnumerationAttributes attributes)
        {
#if NET9_0_OR_GREATER
#else
            const string AttributeTypeFullName = "System.Text.Json.Serialization.JsonStringEnumMemberNameAttribute";
            const string AttributePropertyName = "Name";
#endif

            return
#if NET9_0_OR_GREATER
                attributes.GetAttribute<JsonStringEnumMemberNameAttribute>()?.Name
#else
                attributes
                    .GetAll()
                    .FirstOrDefault(
                        attribute => String.Equals(AttributeTypeFullName, TypeHelper.GetTypeName(attribute.GetType()), StringComparison.Ordinal)
                    )
                    is Attribute attribute
                &&
                attribute.TryGetPropertyValue(AttributePropertyName, out object? value)
                &&
                value is string name
                    ? name
                    : null
#endif
                ;

        }
    }
}
