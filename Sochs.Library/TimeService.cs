using Microsoft.Extensions.Configuration;
using Sochs.Library.Events;
using Sochs.Library.Interfaces;

namespace Sochs.Library
{
  public class TimeService : ITimeService, IDisposable
	{
    private const int UpdateIntervalSeconds = 1;

    private readonly Timer _timer;
    private readonly IConfiguration _config;

    private bool disposedValue;

		public TimeService(IConfiguration config)
		{
      _ = config ?? throw new ArgumentNullException(nameof(config));
      _config = config;

      var autoEvent = new AutoResetEvent(false);
			_timer = new Timer(UpdateTimeOfDay_Callback, autoEvent, new TimeSpan(0, 0, 0), new TimeSpan(0, 0, UpdateIntervalSeconds));
		}

		public event EventHandler<TimeUpdatedEventArgs>? OnTimeUpdated;

		private void UpdateTimeOfDay_Callback(object? stateInfo)
		{
			_ = stateInfo ?? throw new ArgumentNullException(nameof(stateInfo));

			var now = DateTime.Now;

			string timeImagePath = GetTimeImagePath(now);
			string dateImagePath = GetDateImagePath(now);
			string dayImagePath  = GetDayImagePath(now);

      var args = new TimeUpdatedEventArgs()
      {
        DateTime           = DateTime.Now, 
        TimeOfDayImagePath = timeImagePath, 
        SeasonImagePath    = dateImagePath, 
        DayOfWeekImagePath = dayImagePath 
      };

      OnTimeUpdated?.Invoke(this, args);
		}

    private string GetDayImagePath(DateTime now)
    {
      var dayOfWeek = now.DayOfWeek;

      return dayOfWeek switch
      {
        DayOfWeek.Sunday    => _config.GetString("Time:DayOfWeekImagePaths:Sunday"),
        DayOfWeek.Monday    => _config.GetString("Time:DayOfWeekImagePaths:Monday"),
        DayOfWeek.Tuesday   => _config.GetString("Time:DayOfWeekImagePaths:Tuesday"),
        DayOfWeek.Wednesday => _config.GetString("Time:DayOfWeekImagePaths:Wednesday"),
        DayOfWeek.Thursday  => _config.GetString("Time:DayOfWeekImagePaths:Thursday"),
        DayOfWeek.Friday    => _config.GetString("Time:DayOfWeekImagePaths:Friday"),
        DayOfWeek.Saturday  => _config.GetString("Time:DayOfWeekImagePaths:Saturday"),
        _                   => throw new InvalidOperationException($"Cannot determine day of week image path")
      };
    }

    private string GetTimeImagePath(DateTime now)
    {
      var hour = now.Hour;

      if (hour >= 5 && hour < 12) // Morning
      {
        return _config.GetString("Time:TimeOfDayImagePaths:Morning");
      }
      else if (hour >= 12 && hour < 18) // Afternoon
      {
        return _config.GetString("Time:TimeOfDayImagePaths:Afternoon");
      }
      else // Evening
      {
        return _config.GetString("Time:TimeOfDayImagePaths:Evening");
      }
    }

    private string GetDateImagePath(DateTime now)
    {
      var month = now.Month;

      if (month >= 3 && month <= 5) // Spring
      {
        return _config.GetString("Time:SeasonImagePaths:Spring");
      }
      else if (month > 5 && month <= 8) // Summer
      {
        return _config.GetString("Time:SeasonImagePaths:Summer");
      }
      else if (month > 8 && month <= 11) // Fall
      {
        return _config.GetString("Time:SeasonImagePaths:Fall");
      }
      else // Winter
      {
        return _config.GetString("Time:SeasonImagePaths:Winter");
      }
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
