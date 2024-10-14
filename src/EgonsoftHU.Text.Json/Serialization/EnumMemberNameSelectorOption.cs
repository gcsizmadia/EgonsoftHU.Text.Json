// Copyright © 2023-2024 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
#if NET9_0_OR_GREATER
using System.Text.Json.Serialization;
#endif

using EgonsoftHU.Text.Json.Serialization.Converters;

namespace EgonsoftHU.Text.Json.Serialization
{
    /// <summary>
    /// Specifies options for <see cref="JsonEnumValueSerializer"/> which attribute to use.
    /// </summary>
    public enum EnumMemberNameSelectorOption
    {
#if NET9_0_OR_GREATER
        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="JsonStringEnumMemberNameAttribute"/> attribute.
        /// </summary>
#else
        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the
        /// <see href="https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonstringenummembernameattribute">
        /// System.Text.Json.Serialization.JsonStringEnumMemberNameAttribute
        /// </see> attribute.
        /// </summary>
#endif
        UseJsonStringEnumMemberNameAttribute,

        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="EnumMemberAttribute"/> attribute.
        /// </summary>
        UseEnumMemberAttribute,

        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="DescriptionAttribute"/> attribute.
        /// </summary>
        UseDescriptionAttribute,

        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="DisplayNameAttribute"/> attribute.
        /// </summary>
        UseDisplayNameAttribute,

        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="DisplayAttribute.GetName"/> method.
        /// </summary>
        UseDisplayAttributeGetName,

        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="DisplayAttribute.GetShortName"/> method.
        /// </summary>
        UseDisplayAttributeGetShortName,

        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="DisplayAttribute.GetDescription"/> method.
        /// </summary>
        UseDisplayAttributeGetDescription
    }
}
