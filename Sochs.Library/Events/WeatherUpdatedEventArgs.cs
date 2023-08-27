using Sochs.Library.Models;

namespace Sochs.Library.Events
{
  public class WeatherUpdatedEventArgs : EventArgs
  {
    public required WeatherApiResponse WeatherInfo { get; set; }

    public string TemperatureImagePath { get; set; } = string.Empty;

    public string ConditionImagePath { get; set; } = string.Empty;

    public string ShoesImagePath { get; set; } = string.Empty;

    public string PantsImagePath { get; set; } = string.Empty;

    public string ShirtImagePath { get; set; } = string.Empty;

    public string JacketImagePath { get; set; } = string.Empty;
  }
}
