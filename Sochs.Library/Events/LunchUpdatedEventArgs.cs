using Sochs.Library.Models;

namespace Sochs.Library.Events
{
  public class LunchUpdatedEventArgs : EventArgs
  {
    public LunchApiResponse? LunchInfo { get; set; }

    public bool HasData { get { return LunchInfo != null && LunchInfo.Menus != null && LunchInfo.Menus.Any(); } }
  }
}
