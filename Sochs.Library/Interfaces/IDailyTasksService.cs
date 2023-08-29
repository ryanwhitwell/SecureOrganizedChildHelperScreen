using Sochs.Library.Events;
using Sochs.Library.Models;

namespace Sochs.Library.Interfaces
{
  public interface IDailyTasksService
  {
    event EventHandler<ActiveDailyTasksChangeEventArgs>? OnActiveDailyTasksChange;

    event EventHandler<DailyTasksResetEventArgs>? OnDailyTasksReset;

    event EventHandler<DailyTaskUpdatedEventArgs>? OnDailyTaskUpdated;

    public IEnumerable<DailyTask> ActiveTasks { get; }
  }
}
