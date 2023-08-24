namespace Sochs.Library.Events
{
  public class WeatherUpdatedEventArgs : EventArgs
  {
    public string WeatherInfo { get; set; } = string.Empty;
  }
}
