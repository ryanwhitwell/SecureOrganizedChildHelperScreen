using System.Text.Json.Serialization;

namespace Sochs.Library.Models
{
  public class LunchApiMenu
  {
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;


    [JsonPropertyName("lunch")]
    public string Lunch { get; set; } = string.Empty;
  }
}
