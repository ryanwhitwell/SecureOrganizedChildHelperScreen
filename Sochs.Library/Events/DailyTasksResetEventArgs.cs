using Sochs.Library.Models;

namespace Sochs.Library.Events
{
  public class DailyTasksResetEventArgs : EventArgs
  {
    public DailyTaskData? TaskData { get; set; }
  }
}
