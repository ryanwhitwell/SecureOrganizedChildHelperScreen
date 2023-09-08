using Sochs.Library.Models;

namespace Sochs.Library.Events
{
  public class DateEventsUpdatedEventArgs : EventArgs
  {
    public IEnumerable<DateEvent> DateEvents { get; set; } = new List<DateEvent>();

    public string DateEventImagePath { get; set; } = string.Empty;

    public bool HasEventToday => DateEvents.Any(x => x.Date.Date == DateTime.Now.Date);

    public IEnumerable<DateEvent> TodayDateEvents => DateEvents.Where(x => x.Date.Date == DateTime.Now.Date);
  }
}
