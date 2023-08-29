using System.Collections.Concurrent;

namespace Sochs.Library.Models
{
  public class DailyTaskData
  {
    public ConcurrentDictionary<string, DailyTask> Tasks { get; set; } = new ConcurrentDictionary<string, DailyTask>();

    // Has data if any of the task lists have elements in them
    public bool HasData { get { return Tasks != null && Tasks.Any(); } }

    public DateTime DateTime { get; set; }

    public IEnumerable<KeyValuePair<string, DailyTask>> AllAliceTasks =>
       Tasks.Where(x => x.Value.Child == Enums.Child.Alice);

    public IEnumerable<KeyValuePair<string, DailyTask>> AllClaraTasks =>
      Tasks.Where(x => x.Value.Child == Enums.Child.Clara);

    public bool AllMorningTasksCompleted =>
      Tasks.Where(x => x.Value.TimeOfDay == Enums.TimeOfDay.Morning).All(x => x.Value.IsCompleted == true);

    public bool AllAfternoonTasksCompleted =>
      Tasks.Where(x => x.Value.TimeOfDay == Enums.TimeOfDay.Afternoon).All(x => x.Value.IsCompleted == true);

    public bool AllEveningTasksCompleted =>
      Tasks.Where(x => x.Value.TimeOfDay == Enums.TimeOfDay.Evening).All(x => x.Value.IsCompleted == true);

    public bool AllNightTasksCompleted =>
      Tasks.Where(x => x.Value.TimeOfDay == Enums.TimeOfDay.Night).All(x => x.Value.IsCompleted == true);
  }
}
