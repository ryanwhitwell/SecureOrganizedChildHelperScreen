namespace Sochs.Library.Events
{
  public class TimeUpdatedEventArgs : EventArgs
  {
    public DateTime DateTime { get; set; }

    public string TimeImagePath { get; set; } = string.Empty;

    public string DateImagePath { get; set; } = string.Empty;

    public string DayImagePath { get; set; } = string.Empty;
  }
}
