namespace Sochs.Library.Events
{
  public class ClassesUpdatedEventArgs : EventArgs
  {
    public string TodaysSpecialClass { get; set; } = string.Empty;

    public string TodaysSpecialClassImagePath { get; set; } = string.Empty;

    public string TomorrowSpecialClass { get; set; } = string.Empty;

    public string TomorrowSpecialClassImagePath { get; set; } = string.Empty;

    public bool TodayIsWeekday { get; set; }

    public bool TomorrowIsWeekday { get; set; }
  }
}
