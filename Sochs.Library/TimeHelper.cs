using Sochs.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sochs.Library
{
  public static class TimeHelper
  {
    public const int MorningCutOffHour = 11; // 12 PM
    public const int AfternoonCutOffHour = 16; // 5 PM
    public const int TimeCheckPeriodInMilliseconds = 60000; // 1 Minute(s)

    public const string MorningDescription = "Morning";
    public const string AfternoonDescription = "Afternoon";
    public const string EveningDescription = "Evening";

    public static TimeOfDay GetTimeOfDay(DateTime dateTime)
    {
      try
      {
        var currentHour = dateTime.Hour;

        if (currentHour >= 0 && currentHour <= MorningCutOffHour)
        {
          return TimeOfDay.Morning;
        }
        else if (currentHour > MorningCutOffHour && currentHour <= AfternoonCutOffHour)
        {
          return TimeOfDay.Afternoon;
        }
        else
        {
          return TimeOfDay.Evening;
        }
      }
      catch (Exception ex)
      {
        Log.Error("Error getting the time of day.", ex);
        throw;
      }
      
    }

    public static Task UpdateTimeOfDay(ITemporalDisplay temporalDisplay)
    {
      while (true)
      {
        try
        {
          // check for the correct time range; morning, afternoon, evening
          var now = DateTime.Now;
          var timeOfDay = GetTimeOfDay(now);

          temporalDisplay.TimeOfDay = timeOfDay;

          Log.Debug($"Updating time of day to {timeOfDay}.");

          switch (temporalDisplay.TimeOfDay)
          {
            case TimeOfDay.Morning:
              {
                temporalDisplay.SetToMorning();
                break;
              }
            case TimeOfDay.Afternoon:
              {
                temporalDisplay.SetToAfternoon();
                break;
              }
            case TimeOfDay.Evening:
              {
                temporalDisplay.SetToEvening();
                break;
              }
            default:
              throw new InvalidOperationException("Cannot determine time of day.");
          }

          // Wait for a bit so we don't kill the CPU
          Thread.Sleep(TimeCheckPeriodInMilliseconds);
        }
        catch (Exception ex)
        {
          Log.Error("Error updating the time of day.", ex);
          throw;
        }
      }
    }
  }
}
