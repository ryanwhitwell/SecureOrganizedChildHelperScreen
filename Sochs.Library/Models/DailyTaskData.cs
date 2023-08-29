using Sochs.Library.Enums;
using System.Collections.Concurrent;

namespace Sochs.Library.Models
{
  public class DailyTaskData
  {
    public ConcurrentDictionary<string, DailyTask> Tasks { get; set; } = new ConcurrentDictionary<string, DailyTask>();

    public bool HasData { get { return Tasks != null && Tasks.Any(); } }

    public DateTime DateTime { get; set; }

    public IEnumerable<KeyValuePair<string, DailyTask>> GetTasks(Child child, TimeOfDay timeOfDay)
    {
      return Tasks.Where(x => x.Value.Child == child && x.Value.TimeOfDay == timeOfDay);
    }
  }
}
