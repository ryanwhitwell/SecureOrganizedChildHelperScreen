namespace Sochs.Library.Events
{
  public class TimeUpdatedEventArgs : EventArgs
  {
    public DateTime DateTime { get; set; }

    public string TimeOfDayImagePath { get; set; } = string.Empty;

    public string SeasonImagePath { get; set; } = string.Empty;

    public string DayOfWeekImagePath { get; set; } = string.Empty;

    public bool EnableDarkMode { get; set; }
  }
}
