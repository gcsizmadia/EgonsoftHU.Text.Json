// Copyright © 2023-2024 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

#if EGONSOFTHU_TEXT_JSON_INCLUDE_JSONSTRINGENUMMEMBERNAMEATTRIBUTE
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Text.Json.Serialization
{
    /// <summary>
    /// Determines the string value that should be used when serializing an enum member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class JsonStringEnumMemberNameAttribute : Attribute
    {
        /// <summary>
        /// Creates new attribute instance with a specified enum member name.
        /// </summary>
        /// <param name="name">The name to apply to the current enum member.</param>
        public JsonStringEnumMemberNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the enum member.
        /// </summary>
        public string Name { get; }
    }
}
#endif
