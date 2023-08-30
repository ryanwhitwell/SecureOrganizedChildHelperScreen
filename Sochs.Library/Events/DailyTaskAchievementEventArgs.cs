using Sochs.Library.Enums;
using Sochs.Library.Models;

namespace Sochs.Library.Events
{
  public class DailyTaskAchievementEventArgs : EventArgs
  {
    public Child Child { get; set; }

    public DailyTaskData? TaskData { get; set; }
  }
}
