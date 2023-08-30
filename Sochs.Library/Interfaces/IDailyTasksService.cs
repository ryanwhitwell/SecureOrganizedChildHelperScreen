using Sochs.Library.Enums;
using Sochs.Library.Events;

namespace Sochs.Library.Interfaces
{
  public interface IDailyTasksService
  {
    public Child Child { get; set; }

    event EventHandler<DailyTasksResetEventArgs>? OnDailyTasksReset;

    event EventHandler<DailyTaskUpdatedEventArgs>? OnDailyTaskUpdated;

    public void UpdateTask(int id, bool isCompleted);
  }
}
