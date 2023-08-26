using Sochs.Library.Enums;
using Sochs.Library.Events;
using Sochs.Library.Interfaces;

namespace Sochs.Library
{
	public class TimeService : ITimeService, IDisposable
	{
    private const int UpdateIntervalSeconds = 1;

    private readonly Timer _timer;
		private bool disposedValue;

		public TimeService()
		{
			var autoEvent = new AutoResetEvent(false);
			_timer = new Timer(UpdateTimeOfDay_Callback, autoEvent, new TimeSpan(0, 0, 0), new TimeSpan(0, 0, UpdateIntervalSeconds));
		}

		public event EventHandler<TimeUpdatedEventArgs>? OnTimeUpdated;

		private void UpdateTimeOfDay_Callback(object? stateInfo)
		{
			_ = stateInfo ?? throw new ArgumentNullException(nameof(stateInfo));

			var now              = DateTime.Now;
			string timeImagePath = GetTimeImagePath(now);
			string dateImagePath = GetDateImagePath(now);
			string dayImagePath  = GetDayImagePath(now);

      OnTimeUpdated?.Invoke(this, new TimeUpdatedEventArgs() { DateTime = DateTime.Now, TimeImagePath = timeImagePath, DateImagePath = dateImagePath, DayImagePath = dayImagePath });
		}

    private string GetDayImagePath(DateTime now)
    {
      throw new NotImplementedException();
    }

    private string GetDateImagePath(DateTime now)
    {
      throw new NotImplementedException();
    }

    private string GetTimeImagePath(DateTime now)
    {
      throw new NotImplementedException();
    }

    private DayOfWeek GetDayOfWeek(DateTime now)
    {
			throw new NotImplementedException();
    }

    private Season GetSeason(DateTime now)
    {
      throw new NotImplementedException();
    }

    private TimeOfDay GetTimeOfDay(DateTime now)
    {
      throw new NotImplementedException();
    }

    protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_timer?.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
