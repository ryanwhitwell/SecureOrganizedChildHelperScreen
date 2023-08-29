using Microsoft.Extensions.Configuration;
using Sochs.Library.Enums;
using System;

namespace Sochs.Library
{
  public static class Extensions
  {
    public static string GetString(this IConfiguration config, string path)
    {
      var value = config[path];

      if (string.IsNullOrWhiteSpace(value)) { throw new InvalidOperationException($"Cannot get string in config at path {path}");  }

      return value;
    }

    public static decimal GetDecimal(this IConfiguration config, string path)
    {
      var value = config[path];

      if (string.IsNullOrWhiteSpace(value)) { throw new InvalidOperationException($"Cannot get decimal in config at path {path}"); }

      if (!decimal.TryParse(value, out decimal result)) { throw new InvalidOperationException($"Cannot convert string to decimal in config at path {path}"); }

      return result;
    }

    public static TEnum GetEnum<TEnum>(this IConfiguration config, string path) where TEnum : struct, Enum
    {
      string value = config[path] ?? string.Empty;

      if (string.IsNullOrWhiteSpace(value)) { throw new InvalidOperationException($"Cannot get value in config at path {path}"); }

      return (TEnum)Enum.Parse(typeof(TEnum), value);
    }
  }
}
