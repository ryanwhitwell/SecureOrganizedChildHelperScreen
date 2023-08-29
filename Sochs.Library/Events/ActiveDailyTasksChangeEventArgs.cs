using Sochs.Library.Models;

namespace Sochs.Library.Events
{
  public class ActiveDailyTasksChangeEventArgs : EventArgs
  {
    public DailyTaskData? TaskData { get; set; }
  }
}
