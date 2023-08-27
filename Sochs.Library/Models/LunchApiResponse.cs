using System.Text.Json.Serialization;

namespace Sochs.Library.Models
{
  public class LunchApiResponse
  {
    [JsonPropertyName("menus")]
    public IEnumerable<LunchApiMenu>? Menus { get; set; }
  }
}
