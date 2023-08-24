using Sochs.Library.Events;

namespace Sochs.Library.Interfaces
{
  public interface ITimeService
  {
    event EventHandler<TimeUpdatedEventArgs>? OnTimeUpdated;
  }
}
