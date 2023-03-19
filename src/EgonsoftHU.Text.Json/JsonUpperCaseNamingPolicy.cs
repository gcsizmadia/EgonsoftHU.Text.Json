// Copyright © 2023 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System.Text.Json;

using EgonsoftHU.Extensions.Bcl;

namespace EgonsoftHU.Text.Json
{
    /// <summary>
    /// The naming policy for upper-casing.
    /// </summary>
    public class JsonUpperCaseNamingPolicy : JsonNamingPolicy
    {
        /// <summary>
        /// Converts the specified <paramref name="name"/> to upper case.
        /// </summary>
        /// <inheritdoc/>
        public override string ConvertName(string name)
        {
            return name.IsNullOrWhiteSpace() ? name : name.ToUpperInvariant();
        }
    }
}
