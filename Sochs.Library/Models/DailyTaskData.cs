using Sochs.Library.Enums;
using System.Collections.Concurrent;

namespace Sochs.Library.Models
{
  public class DailyTaskData
  {
    public ConcurrentDictionary<int, DailyTask> Tasks { get; set; } = new ConcurrentDictionary<int, DailyTask>();

    public bool HasData { get { return Tasks != null && Tasks.Any(); } }

    public DateTime DateTime { get; set; }

    public IEnumerable<KeyValuePair<int, DailyTask>> GetTasks(Child child, TimeOfDay timeOfDay, DayOfWeek dayOfWeek)
    {
      return Tasks.Where(x => x.Value.Child == child && x.Value.DayOfWeek == dayOfWeek && x.Value.TimeOfDay == timeOfDay).OrderBy(x => x.Value.Id);
    }

    public bool MorningTasksCompleted(Child child, DayOfWeek dayOfWeek)
    {
      return Tasks.Where(x => x.Value.Child == child && x.Value.DayOfWeek == dayOfWeek && x.Value.TimeOfDay == TimeOfDay.Morning).All(x => x.Value.IsCompleted == true);
    }

    public bool AfternoonTasksCompleted(Child child, DayOfWeek dayOfWeek)
    {
      return Tasks.Where(x => x.Value.Child == child && x.Value.DayOfWeek == dayOfWeek && x.Value.TimeOfDay == TimeOfDay.Afternoon).All(x => x.Value.IsCompleted == true);
    }

    public bool EveningTasksCompleted(Child child, DayOfWeek dayOfWeek)
    {
      return Tasks.Where(x => x.Value.Child == child && x.Value.DayOfWeek == dayOfWeek && x.Value.TimeOfDay == TimeOfDay.Evening).All(x => x.Value.IsCompleted == true);
    }

    public bool NightTasksCompleted(Child child, DayOfWeek dayOfWeek)
    {
      return Tasks.Where(x => x.Value.Child == child && x.Value.DayOfWeek == dayOfWeek && x.Value.TimeOfDay == TimeOfDay.Night).All(x => x.Value.IsCompleted == true);
    }
  }
}
