using System.Text.Json.Serialization;

namespace Sochs.Library.Models
{
  public class WeatherApiCurrent
  {
    [JsonPropertyName("condition")]
    public WeatherApiCondition? Condition { get; set; }
    
    [JsonPropertyName("temp_f")]
    public decimal TempF { get; set; }
  }
}
