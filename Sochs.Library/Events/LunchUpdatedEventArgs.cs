namespace Sochs.Library.Events
{
  public class LunchUpdatedEventArgs : EventArgs
  {
    public IEnumerable<string> TodayLunch { get; set; } = new List<string>();

    public IEnumerable<string> TomorrowLunch { get; set; } = new List<string>();

    public bool HasData { get { return TodayLunch.Any() || TomorrowLunch.Any(); } }
  }
}
