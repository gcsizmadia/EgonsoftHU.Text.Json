// Copyright © 2023 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System.Text.Json;

using EgonsoftHU.Extensions.Bcl;

namespace EgonsoftHU.Text.Json
{
    /// <summary>
    /// The naming policy for lower-casing.
    /// </summary>
    public class JsonLowerCaseNamingPolicy : JsonNamingPolicy
    {
        /// <summary>
        /// Converts the specified <paramref name="name"/> to lower case.
        /// </summary>
        /// <inheritdoc/>
        public override string ConvertName(string name)
        {
            return name.IsNullOrWhiteSpace() ? name : name.ToLowerInvariant();
        }
    }
}
