// Copyright © 2023 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System.Text.Json;

using EgonsoftHU.Extensions.Bcl;

namespace EgonsoftHU.Text.Json
{
    /// <summary>
    /// This class contains extension methods that are available for <see cref="JsonSerializerOptions"/> type.
    /// </summary>
    public static class JsonSerializerOptionsExtensions
    {
        /// <summary>
        /// Overrides <paramref name="target"/> settings with the <paramref name="source"/> settings.
        /// </summary>
        /// <param name="source">The <see cref="JsonSerializerOptions"/> instance the settings of which should be copied.</param>
        /// <param name="target">The <see cref="JsonSerializerOptions"/> instance the settings of which will be overridden.</param>
        /// <remarks>
        /// Useful when configuring <c>Microsoft.AspNetCore.Mvc.JsonOptions</c> instance
        /// with an existing instance of <see cref="JsonSerializerOptions"/><br/>
        /// using the <c>AddJsonOptions(this IMvcBuilder builder, Action&lt;JsonOptions&gt; configure)</c> extension method.
        /// <para>Example:</para>
        /// <c>
        /// JsonSerializerOptions options = /* custom settings */;<br/>
        /// <br/>
        /// var builder = WebApplicationBuilder.CreateBuilder(args);<br/>
        /// <br/>
        /// builder.AddControllers().AddJsonOptions(jsonOptions => options.CopyTo(jsonOptions.JsonSerializerOptions));
        /// </c>
        /// </remarks>
        public static void CopyTo(this JsonSerializerOptions source, JsonSerializerOptions target)
        {
            source.ThrowIfNull();
            target.ThrowIfNull();

            target.AllowTrailingCommas = source.AllowTrailingCommas;

            target.Converters.Clear();
            target.Converters.AddRange(source.Converters);

            target.DefaultBufferSize = source.DefaultBufferSize;
            target.DefaultIgnoreCondition = source.DefaultIgnoreCondition;
            target.DictionaryKeyPolicy = source.DictionaryKeyPolicy;
            target.Encoder = source.Encoder;
            target.IgnoreReadOnlyFields = source.IgnoreReadOnlyFields;
            target.IgnoreReadOnlyProperties = source.IgnoreReadOnlyProperties;
            target.IncludeFields = source.IncludeFields;
            target.MaxDepth = source.MaxDepth;
            target.NumberHandling = source.NumberHandling;
            target.PropertyNameCaseInsensitive = source.PropertyNameCaseInsensitive;
            target.PropertyNamingPolicy = source.PropertyNamingPolicy;
            target.ReadCommentHandling = source.ReadCommentHandling;
            target.ReferenceHandler = source.ReferenceHandler;
            target.UnknownTypeHandling = source.UnknownTypeHandling;
            target.WriteIndented = source.WriteIndented;
        }
    }
}
