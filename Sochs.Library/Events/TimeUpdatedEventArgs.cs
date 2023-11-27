using Sochs.Library.Enums;

namespace Sochs.Library.Events
{
  public class TimeUpdatedEventArgs : EventArgs
  {
    public DateTime DateTime { get; set; }

    public string TimeOfDayImagePath { get; set; } = string.Empty;

    public string SeasonImagePath { get; set; } = string.Empty;

    public string DayOfWeekImagePath { get; set; } = string.Empty;

    public bool EnableDarkMode { get; set; }

    public TimeOfDay TimeOfDay { get; set; }

    public DayType DayType { get; set; }

    public double MinutesUntilNextTimeOfDay {  get; set; }

    public int DaysUntilXmas { get; set; }
  }
}
