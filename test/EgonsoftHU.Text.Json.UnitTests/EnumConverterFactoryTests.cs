// Copyright © 2023-2024 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using EgonsoftHU.Extensions.Bcl.Enumerations;
using EgonsoftHU.Text.Json.UnitTests.Stubs;

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

        [Theory]
        [InlineData(DayOfWeek.Friday, "\"friday\"")]
        [InlineData((DayOfWeek)7, "7")]
        [InlineData((DayOfWeek)0, "\"sunday\"")]
        [InlineData((DayOfWeek)(-1), "-1")]
        public void SerializeTest<TEnum>(TEnum value, string expectedSerializedValue)
            where TEnum : struct, Enum
        {
            // Arrange

            // Act
            string serializedValue = JsonSerializer.Serialize(value, JsonSerializerOptionsProvider.Current);

            // Assert
            serializedValue.Should().Be(expectedSerializedValue);
        }

        [Theory]
        [InlineData("\"friday\"", DayOfWeek.Friday)]
        [InlineData("\"Friday\"", DayOfWeek.Friday)]
        [InlineData("\"FrIdAy\"", DayOfWeek.Friday)]
        [InlineData("7", (DayOfWeek)7)]
        [InlineData("0", DayOfWeek.Sunday)]
        [InlineData("-1", (DayOfWeek)(-1))]
        [InlineData("\"r\"", MetricPrefix.Ronto)]
        [InlineData("\"m\"", MetricPrefix.Milli)]
        [InlineData("\"milli\"", MetricPrefix.Milli)]
        [InlineData("\"Milli\"", MetricPrefix.Milli)]
        [InlineData("\"MiLLi\"", MetricPrefix.Milli)]
        [InlineData("\"M\"", MetricPrefix.Mega)]
        public void DeserializeTest<TEnum>(string serializedValue, TEnum expectedValue)
            where TEnum : struct, Enum
        {
            // Arrange

            // Act
            TEnum value = JsonSerializer.Deserialize<TEnum>(serializedValue, JsonSerializerOptionsProvider.Current);

            // Assert
            value.Should().Be(expectedValue);
        }

        [Fact]
        public void CallingEnumInfoDefaultConcurrentlyShouldNotFail()
        {
            // Arrange
            Type enumType = Type.GetType("System.Reflection.Emit.OpCodeValues") ?? typeof(System.Runtime.InteropServices.VarEnum);

            var methodInvokes =
                Enumerable
                    .Range(1, 1000)
                    .Select<int, Action>(_ => () => GetDefaultValue(enumType))
                    .ToList();

            // Act
            Action sut = () => Parallel.ForEach(methodInvokes, getDefaultValue => getDefaultValue.Invoke());

            // Assert
            sut.Should().NotThrow();
        }

        private void GetDefaultValue(Type enumType)
        {
            try
            {
                Type enumInfoType = typeof(EnumInfo<>).MakeGenericType(enumType);
                PropertyInfo? field = enumInfoType.GetTypeInfo().GetRuntimeProperty("DefaultValue");
                Logger.Information("[{EnumType}]::DefaultValue = [{Value}]", enumInfoType.FullName, field?.GetValue(null));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Get default value failed.");
                throw;
            }
        }

        [Fact]
        public void CreateConvertersOfTheSameTypeConcurrentlyShouldNotFail()
        {
            // Arrange
            JsonSerializerOptions options = JsonSerializerOptionsProvider.Current;
            JsonStringEnumConverter converter = options.Converters.OfType<JsonStringEnumConverter>().Single();

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
