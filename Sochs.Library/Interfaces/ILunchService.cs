using Sochs.Library.Events;

namespace Sochs.Library.Interfaces
{
  public interface ILunchService
  {
    event EventHandler<LunchUpdatedEventArgs>? OnLunchUpdated;
  }
}
