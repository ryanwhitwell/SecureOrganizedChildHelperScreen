namespace Sochs.Library.Events
{
  public class ClassesUpdatedEventArgs : EventArgs
  {
    public string TodaysSpecialClass { get; set; } = string.Empty;

    public string TodaysSpecialClassImagePath { get; set; } = string.Empty;

    public bool IsWeekday { get; set; }
  }
}
