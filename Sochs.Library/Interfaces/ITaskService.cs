using Sochs.Library.Events;
using Sochs.Library.Models;

namespace Sochs.Library.Interfaces
{
  public interface ITaskService
  {
    event EventHandler<TimeOfDayTasksChangeEventArgs>? OnTimeOfDayTasksChange;

    event EventHandler<DailyTasksResetEventArgs>? OnDailyTasksReset;

    event EventHandler<TaskUpdatedEventArgs>? OnTaskUpdated;

    public IEnumerable<DailyTask> ActiveTasks { get; }
  }
}
