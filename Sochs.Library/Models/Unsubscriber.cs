namespace Sochs.Library.Models
{
  public sealed class Unsubscriber<TimeOfDayInfo> : IDisposable
  {
    private readonly ISet<IObserver<TimeOfDayInfo>> _observers;
    private readonly IObserver<TimeOfDayInfo> _observer;

    public Unsubscriber(ISet<IObserver<TimeOfDayInfo>> observers, IObserver<TimeOfDayInfo> observer) => (_observers, _observer) = (observers, observer);

    public void Dispose()
    {
      _observers.Remove(_observer);
    }
  }
}
