// Copyright © 2023-2024 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;

namespace EgonsoftHU.Text.Json.Constants
{
    /// <summary>
    /// Contains <see cref="AppContext"/> switches.
    /// </summary>
    public static class AppContextSwitches
    {
        private const string Prefix = "Switch.EgonsoftHU.Text.Json.";

        /// <summary>
        /// The <c>Switch.EgonsoftHU.Text.Json.AlwaysCheckForJsonStringEnumMemberAttributeByName</c> switch.
        /// </summary>
        public static readonly string AlwaysCheckForJsonStringEnumMemberAttributeByName =
            $"{Prefix}{nameof(AlwaysCheckForJsonStringEnumMemberAttributeByName)}";
    }
}
