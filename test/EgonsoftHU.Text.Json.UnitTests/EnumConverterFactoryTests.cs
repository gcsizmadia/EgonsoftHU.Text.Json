// Copyright © 2023 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;
using Xunit.Abstractions;

using JsonStringEnumConverter = EgonsoftHU.Text.Json.Serialization.JsonStringEnumConverter;

namespace EgonsoftHU.Text.Json.UnitTests
{
    public class EnumConverterFactoryTests : UnitTest<EnumConverterFactoryTests>
    {
        public EnumConverterFactoryTests(ITestOutputHelper output, LoggingFixture<EnumConverterFactoryTests> fixture)
            : base(output, fixture)
        {
        }

        [Fact]
        public void CreateConvertersOfTheSameTypeConcurrentlyShouldNotFail()
        {
            // Arrange
            var converter = new JsonStringEnumConverter(JsonNamingPolicy.CamelCase);

            JsonSerializerOptions options =
                new(JsonSerializerDefaults.Web)
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };

            options.Converters.Add(converter);

            Type enumType = Type.GetType("System.Reflection.Emit.OpCodeValues") ?? typeof(System.Runtime.InteropServices.VarEnum);

            var methodInvokes =
                Enumerable
                    .Range(1, 1000)
                    .Select<int, Func<JsonConverter?>>(_ => () => CreateConverter(converter, enumType, options))
                    .ToList();

            // Act
            Action sut = () => Parallel.ForEach(methodInvokes, getConverter => getConverter.Invoke());

            // Assert
            sut.Should().NotThrow();
        }

        private JsonConverter? CreateConverter(JsonStringEnumConverter converter, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return converter.CreateConverter(typeToConvert, options);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Create converter failed.");
                throw;
            }
        }
    }
}
