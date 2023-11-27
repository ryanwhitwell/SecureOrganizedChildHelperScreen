using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using Sochs.Library.Enums;
using Sochs.Library.Events;
using Sochs.Library.Interfaces;

namespace Sochs.Library
{
  public class TimeService : ITimeService, IDisposable
  {
    // Days out to start showing Xmas countdown
    public const int XmasDaysThreshold = 45;

    private const int UpdateIntervalSeconds = 1;

    // Time of Day
    private const int NightStartHour     = 20;
    private const int MorningStartHour   = 5;
    private const int AfternoonStartHour = 11;
    private const int EveningStartHour   = 18;

    // Seasons
    private const int SummerStartMonth = 6;
    private const int SummerStartDay   = 21;
    private const int SpringStartMonth = 3;
    private const int SpringStartDay   = 19;
    private const int FallStartMonth   = 9;
    private const int FallStartDay     = 23;
    private const int WinterStartMonth = 12;
    private const int WinterStartDay   = 21;

    private readonly Timer _timer;
    private readonly IConfiguration _config;
    private readonly IJSRuntime _js;

    private bool disposedValue;

    public TimeService(IConfiguration config, IJSRuntime js)
    {
      _ = config ?? throw new ArgumentNullException(nameof(config));
      _ = js ?? throw new ArgumentNullException(nameof(js));

      _config = config;
      _js = js;

      var autoEvent = new AutoResetEvent(false);
      _timer = new Timer(UpdateTimeOfDay_Callback, autoEvent, new TimeSpan(0, 0, 0), new TimeSpan(0, 0, UpdateIntervalSeconds));
    }

    public event EventHandler<TimeUpdatedEventArgs>? OnTimeUpdated;

    private async void UpdateTimeOfDay_Callback(object? stateInfo)
    {
      try
      {
        _ = stateInfo ?? throw new ArgumentNullException(nameof(stateInfo));

        var now = DateTime.Now;

        TimeOfDay timeOfDay  = GetTimeOfDay(now);
        DayType dayType      = GetDayType(now);
        string timeImagePath = GetTimeImagePath(timeOfDay);
        string dateImagePath = GetDateImagePath(now);
        string dayImagePath  = GetDayImagePath(now);
        bool enableDarkMode  = timeOfDay == TimeOfDay.Evening || timeOfDay == TimeOfDay.Night;
        int daysUntilXmas    = GetDaysUntilXmas(now);

        double minutesUntilNextTimeOfDay = GetMinutesUntilNextTimeOfDay(now, timeOfDay);

        var args = new TimeUpdatedEventArgs()
        {
          DateTime = DateTime.Now,
          TimeOfDayImagePath = timeImagePath,
          SeasonImagePath = dateImagePath,
          DayOfWeekImagePath = dayImagePath,
          EnableDarkMode = enableDarkMode,
          TimeOfDay = timeOfDay,
          DayType = dayType,
          MinutesUntilNextTimeOfDay = minutesUntilNextTimeOfDay,
          DaysUntilXmas = daysUntilXmas
        };

        OnTimeUpdated?.Invoke(this, args);
      }
      catch (Exception e)
      {
        await _js.InvokeVoidAsync("alert", $"Error in TimeService.UpdateTimeOfDay_Callback. {e}");
      }
    }

    private static double GetMinutesUntilNextTimeOfDay(DateTime now, TimeOfDay timeOfDay)
    {
      return timeOfDay switch
      {
        TimeOfDay.Morning => (new DateTime(now.Year, now.Month, now.Day, AfternoonStartHour, 0, 0) - now).TotalMinutes,
        TimeOfDay.Afternoon => (new DateTime(now.Year, now.Month, now.Day, EveningStartHour, 0, 0) - now).TotalMinutes,
        TimeOfDay.Evening => (new DateTime(now.Year, now.Month, now.Day, NightStartHour, 0, 0) - now).TotalMinutes,
        TimeOfDay.Night => (new DateTime(now.Year, now.Month, now.Day, MorningStartHour, 0, 0).AddDays(1) - now).TotalMinutes,
        _ => throw new InvalidOperationException($"Cannot determine day of week image path")
      }; ;
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

    private string GetTimeImagePath(TimeOfDay timeOfDay)
    {
      return timeOfDay switch
      {
        TimeOfDay.Morning   => _config.GetString("Time:TimeOfDayImagePaths:Morning"),
        TimeOfDay.Afternoon => _config.GetString("Time:TimeOfDayImagePaths:Afternoon"),
        TimeOfDay.Evening   => _config.GetString("Time:TimeOfDayImagePaths:Evening"),
        TimeOfDay.Night     => _config.GetString("Time:TimeOfDayImagePaths:Night"),
        _                   => throw new InvalidOperationException($"Cannot determine time of day image path based on time of day")
      };
    }

    private static int GetDaysUntilXmas(DateTime now)
    {
      DateTime christmasDate = new(DateTime.Now.Year, 12, 25);

      // If Christmas has already occurred this year, calculate days until next Christmas
      if (now > christmasDate)
      {
        christmasDate = new(now.Year + 1, 12, 25);
      }

      TimeSpan timeUntilChristmas = christmasDate - now;
      return timeUntilChristmas.Days;
    }

    public static TimeOfDay GetTimeOfDay(DateTime now)
    {
      var hour = now.Hour;

      if (hour >= MorningStartHour && hour < AfternoonStartHour) // Morning 5 AM - 10:59 AM
      {
        return TimeOfDay.Morning;
      }
      else if (hour >= AfternoonStartHour && hour < EveningStartHour) // Afternoon 11 AM - 5:59 PM
      {
        return TimeOfDay.Afternoon;
      }
      else if (hour >= EveningStartHour && hour < NightStartHour) // Evening 6 PM - 7:59 PM
      {
        return TimeOfDay.Evening;
      }
      else // Night 8 PM - 4:59 AM
      {
        return TimeOfDay.Night;
      }
    }

    private static DayType GetDayType(DateTime now)
    {
      var dayOfWeek = now.DayOfWeek;


      if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
      {
        return DayType.Weekend;
      }

      return DayType.Weekday;
    }

    private string GetDateImagePath(DateTime now)
    {
      DateTime springStartDate = new(now.Year, SpringStartMonth,  SpringStartDay);
      DateTime summerStartDate = new(now.Year, SummerStartMonth,  SummerStartDay);
      DateTime fallStartDate   = new(now.Year, FallStartMonth,    FallStartDay);
      DateTime winterStartDate = new(now.Year, WinterStartMonth,  WinterStartDay);

      if (now >= springStartDate && now < summerStartDate) // Spring
      {
        return _config.GetString("Time:SeasonImagePaths:Spring");
      }
      else if (now > summerStartDate && now < fallStartDate) // Summer
      {
        return _config.GetString("Time:SeasonImagePaths:Summer");
      }
      else if (now >= fallStartDate && now < winterStartDate) // Fall
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
					// Remove the timer
          _timer?.Dispose();
				}

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
