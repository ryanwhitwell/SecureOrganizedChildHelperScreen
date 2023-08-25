using Microsoft.Extensions.Configuration;

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
  }
}
