using System.Text.Json.Serialization;

namespace Sochs.Library.Models
{
  public class WeatherApiCondition
  {
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonIgnore]
    public string ImagePath { get; set;} = string.Empty;
  }
}
