using Sochs.Library.Models;

namespace Sochs.Library.Events
{
  public class WeatherUpdatedEventArgs : EventArgs
  {
    public required WeatherApiResponse WeatherInfo { get; set; }
  }
}
