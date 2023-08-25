using System.Text.Json.Serialization;

namespace Sochs.Library.Models
{
  public class WeatherApiResponse
  {
    [JsonPropertyName("current")]
    public WeatherApiCurrent? Current { get; set; }
  }
}
