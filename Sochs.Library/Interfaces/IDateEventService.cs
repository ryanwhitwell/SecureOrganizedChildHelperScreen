using Sochs.Library.Enums;
using Sochs.Library.Events;

namespace Sochs.Library.Interfaces
{
  public interface IDateEventService
  {
    event EventHandler<DateEventsUpdatedEventArgs>? OnDateEventsUpdated;
  }
}
