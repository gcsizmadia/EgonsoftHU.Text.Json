// Copyright © 2023 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace EgonsoftHU.Text.Json.Serialization.Converters
{
    internal static class EnumMemberInfo
    {
        public static void InitializeCache<TEnum>()
            where TEnum : struct, Enum
        {
            TypeInfo typeInfo = typeof(TEnum).GetTypeInfo();

            ThrowIfHasFlagsAttribute(typeInfo);

            Array values = Enum.GetValues(typeof(TEnum));

            if (EnumMemberInfo<TEnum>.Cache.Count == values.Length)
            {
                return;
            }

            values
                .Cast<TEnum>()
                .ToList()
                .ForEach(value => GetMemberInfo(value));
        }

        public static void InitializeCache(Type enumType)
        {
            TypeInfo typeInfo = enumType.GetTypeInfo();

            ThrowIfNotEnumType(typeInfo);

            typeof(EnumMemberInfo)
                .GetTypeInfo()
                .GetMethods()
                .Single(
                    methodInfo =>
                    String.Equals(nameof(InitializeCache), methodInfo.Name, StringComparison.OrdinalIgnoreCase)
                    &&
                    methodInfo.IsGenericMethod
                )
                .MakeGenericMethod(enumType)
                .Invoke(obj: null, parameters: null);
        }

        public static EnumMemberInfo<TEnum> GetMemberInfo<TEnum>(TEnum value)
            where TEnum : struct, Enum
        {
            TypeInfo typeInfo = typeof(TEnum).GetTypeInfo();

            ThrowIfHasFlagsAttribute(typeInfo);
            ThrowIfUndefinedValue(value);

            TypeCode enumTypeCode = Type.GetTypeCode(typeInfo);
            ulong key = ConvertToUInt64(value, enumTypeCode);

            if (EnumMemberInfo<TEnum>.Cache.TryGetValue(key, out EnumMemberInfo<TEnum>? enumMemberInfo))
            {
                return enumMemberInfo;
            }

            FieldInfo fieldInfo = GetFieldInfo(value, typeInfo);

            (string? name, string? description) = GetDisplayInfo(fieldInfo);

            enumMemberInfo = new EnumMemberInfo<TEnum>()
            {
                UInt64Value = key,
                Value = value,
                MemberName = GetMemberName(value),
                MemberValue = GetMemberValue(fieldInfo),
                DisplayName = name,
                Description = description
            };

            EnumMemberInfo<TEnum>.Cache.Add(key, enumMemberInfo);

            return enumMemberInfo;
        }

        private static void ThrowIfNotEnumType(TypeInfo typeInfo)
        {
            if (!typeInfo.IsEnum)
            {
                var ex = new ArgumentException("Only enum types are allowed.", nameof(typeInfo));
                ex.Data[nameof(Type)] = typeInfo.FullName ?? typeInfo.Name;

                throw ex;
            }
        }

        private static void ThrowIfHasFlagsAttribute(TypeInfo typeInfo)
        {
            if (typeInfo.GetCustomAttribute<FlagsAttribute>() is not null)
            {
                var ex = new NotSupportedException("Enum types with System.FlagsAttribute not supported.");
                ex.Data["EnumType"] = typeInfo.FullName ?? typeInfo.Name;

                throw ex;
            }
        }

        private static void ThrowIfUndefinedValue<TEnum>(TEnum value)
            where TEnum : struct, Enum
        {
#if NET5_0_OR_GREATER
            if (!Enum.IsDefined(value))
#else
            if (!Enum.IsDefined(typeof(TEnum), value))
#endif
            {
                TypeInfo typeInfo = typeof(TEnum).GetTypeInfo();

                var ex = new ArgumentException("Undefined enum values not supported.", nameof(value));
                ex.Data["EnumType"] = typeInfo.FullName ?? typeInfo.Name;
                ex.Data["Value"] = value;

                throw ex;
            }
        }

        private static FieldInfo GetFieldInfo<TEnum>(TEnum value, TypeInfo typeInfo)
            where TEnum : struct, Enum
        {
            return typeInfo.DeclaredFields.Single(fieldInfo => fieldInfo.IsStatic && value.Equals(fieldInfo.GetValue(null)));
        }

        private static string GetMemberName<TEnum>(TEnum value)
            where TEnum : struct, Enum
        {
            return
#if NET5_0_OR_GREATER
                Enum.GetName(value)
#else
                Enum.GetName(typeof(TEnum), value)
#endif
                ??
                value.ToString();
        }

        private static string? GetMemberValue(FieldInfo fieldInfo)
        {
            EnumMemberAttribute? enumMemberAttribute = fieldInfo.GetCustomAttribute<EnumMemberAttribute>();

            return enumMemberAttribute?.Value;
        }

        private static (string? Name, string? Description) GetDisplayInfo(FieldInfo fieldInfo)
        {
            DisplayAttribute? displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();

            return (displayAttribute?.Name, displayAttribute?.Description);
        }

        private static ulong ConvertToUInt64(object value, TypeCode enumTypeCode)
        {
            return enumTypeCode switch
            {
                TypeCode.Int32 => (ulong)(int)value,
                TypeCode.UInt32 => (uint)value,
                TypeCode.UInt64 => (ulong)value,
                TypeCode.Int64 => (ulong)(long)value,
                TypeCode.SByte => (ulong)(sbyte)value,
                TypeCode.Byte => (byte)value,
                TypeCode.Int16 => (ulong)(short)value,
                TypeCode.UInt16 => (ushort)value,
                _ => throw new InvalidOperationException(),
            };
        }
    }

    internal class EnumMemberInfo<TEnum>
        where TEnum : struct, Enum
    {
        internal static Dictionary<ulong, EnumMemberInfo<TEnum>> Cache { get; } = new();

        public ulong UInt64Value { get; internal init; }

        public TEnum Value { get; internal init; }

        public string MemberName { get; internal init; } = String.Empty;

        public string? MemberValue { get; internal init; }

        public string? DisplayName { get; internal init; }

        public string? Description { get; internal init; }
    }
}
