using Microsoft.Extensions.Configuration;
using Sochs.Library.Enums;
using Sochs.Library.Events;
using Sochs.Library.Interfaces;

namespace Sochs.Library
{
	public class TimeService : ITimeService, IDisposable
	{
    private const int UpdateIntervalSeconds = 1;

    private readonly Timer _timer;

    private readonly string _sundayImagePath;
    private readonly string _mondayImagePath;
    private readonly string _tuesdayImagePath;
    private readonly string _wednesdayImagePath;
    private readonly string _thursdayImagePath;
    private readonly string _fridayImagePath;
    private readonly string _saturdayImagePath;

    private readonly string _morningImagePath;
    private readonly string _afternoonImagePath;
    private readonly string _eveningImagePath;

    private readonly string _springImagePath;
    private readonly string _summerImagePath;
    private readonly string _fallImagePath;
    private readonly string _winterImagePath;

    private bool disposedValue;

		public TimeService(IConfiguration config)
		{
      _ = config ?? throw new ArgumentNullException(nameof(config));

      _sundayImagePath    = config.GetString("Time:DayOfWeekImagePaths:Sunday");
      _mondayImagePath    = config.GetString("Time:DayOfWeekImagePaths:Monday");
      _tuesdayImagePath   = config.GetString("Time:DayOfWeekImagePaths:Tuesday");
      _wednesdayImagePath = config.GetString("Time:DayOfWeekImagePaths:Wednesday");
      _thursdayImagePath  = config.GetString("Time:DayOfWeekImagePaths:Thursday");
      _fridayImagePath    = config.GetString("Time:DayOfWeekImagePaths:Friday");
      _saturdayImagePath  = config.GetString("Time:DayOfWeekImagePaths:Saturday");

      _morningImagePath   = config.GetString("Time:TimeOfDayImagePaths:Morning");
      _afternoonImagePath = config.GetString("Time:TimeOfDayImagePaths:Afternoon");
      _eveningImagePath   = config.GetString("Time:TimeOfDayImagePaths:Evening");

      _springImagePath = config.GetString("Time:SeasonImagePaths:Spring");
      _summerImagePath = config.GetString("Time:SeasonImagePaths:Summer");
      _fallImagePath   = config.GetString("Time:SeasonImagePaths:Fall");
      _winterImagePath = config.GetString("Time:SeasonImagePaths:Winter");

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

      OnTimeUpdated?.Invoke(this, new TimeUpdatedEventArgs() { DateTime = DateTime.Now, TimeOfDayImagePath = timeImagePath, SeasonImagePath = dateImagePath, DayOfWeekImagePath = dayImagePath });
		}

    private string GetDayImagePath(DateTime now)
    {
      var dayOfWeek = GetDayOfWeek(now);
      
      return dayOfWeek switch
      {
        DayOfWeek.Sunday    => _sundayImagePath,
        DayOfWeek.Monday    => _mondayImagePath,
        DayOfWeek.Tuesday   => _tuesdayImagePath,
        DayOfWeek.Wednesday => _wednesdayImagePath,
        DayOfWeek.Thursday  => _thursdayImagePath,
        DayOfWeek.Friday    => _fridayImagePath,
        DayOfWeek.Saturday  => _saturdayImagePath,
        _                   => throw new InvalidOperationException($"Cannot determine day of week image path")
      };
    }

    private string GetDateImagePath(DateTime now)
    {
      var season = GetSeason(now);

      return season switch
      {
        Season.Spring => _springImagePath,
        Season.Summer => _summerImagePath,
        Season.Fall   => _fallImagePath,
        Season.Winter => _winterImagePath,
        _             => throw new InvalidOperationException($"Cannot determine day of week image path")
      };
    }

    private string GetTimeImagePath(DateTime now)
    {
      var timeOfDay = GetTimeOfDay(now);

      return timeOfDay switch
      {
        TimeOfDay.Morning   => _morningImagePath,
        TimeOfDay.Afternoon => _afternoonImagePath,
        TimeOfDay.Evening   => _eveningImagePath,
        _                   => throw new InvalidOperationException($"Cannot determine day of week image path")
      };
    }

    private static DayOfWeek GetDayOfWeek(DateTime now)
    {
      return now.DayOfWeek;
    }

    private static Season GetSeason(DateTime now)
    {
      var month = now.Month;

      if (month >= 3 && month <= 5)
      {
        return Season.Spring;
      }
      else if (month > 5 && month <= 8)
      {
        return Season.Summer;
      }
      else if (month > 8 && month <= 11)
      {
        return Season.Fall;
      }
      else
      {
        return Season.Winter;
      }
    }

    private static TimeOfDay GetTimeOfDay(DateTime now)
    {
      var hour = now.Hour;

      if (hour >= 5 && hour < 12)
      {
        return TimeOfDay.Morning;
      }
      else if (hour >= 12 && hour < 18)
      {
        return TimeOfDay.Afternoon;
      }
      else
      {
        return TimeOfDay.Evening;
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
