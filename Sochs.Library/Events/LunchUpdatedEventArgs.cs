using Sochs.Library.Models;

namespace Sochs.Library.Events
{
  public class LunchUpdatedEventArgs : EventArgs
  {
    public LunchApiResponse? LunchInfo { get; set; }
  }
}
