using Sochs.Library.Enums;

namespace Sochs.Library.Models
{
  public class DailyTask
  {
    public string Id { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ImagePath { get; set; } = string.Empty;

    public TimeOfDay TimeOfDay { get; set; }

    public DayType DayType { get; set; }
    
    public Child Child { get; set; }

    public bool IsCompleted { get; set; }
  }
}
