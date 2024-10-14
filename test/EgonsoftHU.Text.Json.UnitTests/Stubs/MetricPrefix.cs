// Copyright © 2023-2024 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System.Text.Json.Serialization;

namespace EgonsoftHU.Text.Json.UnitTests.Stubs
{
    internal enum MetricPrefix
    {
        [JsonStringEnumMemberName("q")]
        Kvekto,
        [JsonStringEnumMemberName("r")]
        Ronto,
        [JsonStringEnumMemberName("y")]
        Jokto,
        [JsonStringEnumMemberName("z")]
        Zepto,
        [JsonStringEnumMemberName("a")]
        Atto,
        [JsonStringEnumMemberName("f")]
        Femto,
        [JsonStringEnumMemberName("p")]
        Piko,
        [JsonStringEnumMemberName("n")]
        Nano,
        [JsonStringEnumMemberName("µ")]
        Mikro,
        [JsonStringEnumMemberName("m")]
        Milli,
        [JsonStringEnumMemberName("k")]
        Kilo,
        [JsonStringEnumMemberName("M")]
        Mega,
        [JsonStringEnumMemberName("G")]
        Giga,
        [JsonStringEnumMemberName("T")]
        Tera,
        [JsonStringEnumMemberName("P")]
        Peta,
        [JsonStringEnumMemberName("E")]
        Exa,
        [JsonStringEnumMemberName("Z")]
        Zetta,
        [JsonStringEnumMemberName("Y")]
        Jotta,
        [JsonStringEnumMemberName("R")]
        Ronna,
        [JsonStringEnumMemberName("Q")]
        Kvetta
    }
}
