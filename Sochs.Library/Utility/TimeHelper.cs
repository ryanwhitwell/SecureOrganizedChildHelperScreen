using Sochs.Library.Enums;
using Sochs.Library.Logging;

namespace Sochs.Library.Utility
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
  }
}
