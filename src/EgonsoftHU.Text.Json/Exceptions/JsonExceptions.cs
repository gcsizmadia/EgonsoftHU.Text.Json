// Copyright © 2023-2024 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System.Text.Json;

namespace EgonsoftHU.Text.Json.Exceptions
{
    internal static class JsonExceptions
    {
        internal static class MessageTemplates
        {
            public const string InvalidToken = "Invalid token.";

            public const string InvalidEnumValue = "Invalid enum value.";
        }

        internal static JsonException InvalidToken()
        {
            var ex = new JsonException(MessageTemplates.InvalidToken);

            return ex;
        }

        internal static JsonException InvalidEnumValue()
        {
            var ex = new JsonException(MessageTemplates.InvalidEnumValue);

            return ex;
        }
    }
}
