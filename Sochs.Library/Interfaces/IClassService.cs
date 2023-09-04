using Sochs.Library.Enums;
using Sochs.Library.Events;

namespace Sochs.Library.Interfaces
{
  public interface IClassService
  {
    event EventHandler<ClassesUpdatedEventArgs>? OnClassesUpdated;
    public Child Child { get; set; }
  }
}
