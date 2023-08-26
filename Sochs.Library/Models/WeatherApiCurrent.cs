using System.Text.Json.Serialization;

namespace Sochs.Library.Models
{
  public class WeatherApiCurrent
  {
    [JsonPropertyName("condition")]
    public WeatherApiCondition? Condition { get; set; }
    
    [JsonPropertyName("feelslike_f")]
    public decimal FeelsLikeF { get; set; }

    [JsonPropertyName("temp_f")]
    public decimal TemperatureF { get; set; }

    [JsonIgnore]
    public string TemperatureImagePath { get; set; } = string.Empty;
  }
}
