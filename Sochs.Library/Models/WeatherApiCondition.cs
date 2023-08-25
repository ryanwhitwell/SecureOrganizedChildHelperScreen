using System.Text.Json.Serialization;

namespace Sochs.Library.Models
{
  public class WeatherApiCondition
  {
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
  }
}
