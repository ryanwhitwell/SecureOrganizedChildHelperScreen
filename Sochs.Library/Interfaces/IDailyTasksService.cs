using Sochs.Library.Enums;
using Sochs.Library.Events;
using Sochs.Library.Models;

namespace Sochs.Library.Interfaces
{
  public interface IDailyTasksService
  {
    public Child Child { get; set; }

    event EventHandler<ActiveDailyTasksChangeEventArgs>? OnActiveDailyTasksChange;

    event EventHandler<DailyTasksResetEventArgs>? OnDailyTasksReset;

    event EventHandler<DailyTaskUpdatedEventArgs>? OnDailyTaskUpdated;

    public void UpdateTask(string name, bool isCompleted);
  }
}
