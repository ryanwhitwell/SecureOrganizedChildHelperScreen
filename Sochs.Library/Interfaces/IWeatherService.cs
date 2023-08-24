using Sochs.Library.Events;

namespace Sochs.Library.Interfaces
{
  public interface IWeatherService
  {
    event EventHandler<WeatherUpdatedEventArgs>? OnWeatherUpdated;
  }
}
