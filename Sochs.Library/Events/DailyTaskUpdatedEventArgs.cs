using Sochs.Library.Models;

namespace Sochs.Library.Events
{
  public class DailyTaskUpdatedEventArgs : EventArgs
  {
    public DailyTaskData? TaskData { get; set; }
  }
}
