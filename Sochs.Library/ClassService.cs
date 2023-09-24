using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sochs.Library.Enums;
using Sochs.Library.Events;
using Sochs.Library.Interfaces;

namespace Sochs.Library
{
  public class ClassService : IClassService, IDisposable
	{
    private const int UpdateIntervalMinutes = 30;

    private readonly Timer _timer;
    private readonly IConfiguration _config;
    private readonly IJSRuntime _js;

    private bool disposedValue;

		public ClassService(IConfiguration config, IJSRuntime js)
		{
      _ = config ?? throw new ArgumentNullException(nameof(config));
      _ = js ?? throw new ArgumentNullException(nameof(js));

      _config = config;
      _js = js;

      var autoEvent = new AutoResetEvent(false);
			_timer = new Timer(UpdateClasses_Callback, autoEvent, new TimeSpan(0, 0, 0), new TimeSpan(0, 0, UpdateIntervalMinutes));
		}

    public Child Child { get; set; }

    public event EventHandler<ClassesUpdatedEventArgs>? OnClassesUpdated;

		private async void UpdateClasses_Callback(object? stateInfo)
		{
      try
      {
        _ = stateInfo ?? throw new ArgumentNullException(nameof(stateInfo));

        var args = new ClassesUpdatedEventArgs();

        var today = DateTime.Now.DayOfWeek;
        var tomorrow = DateTime.Now.AddDays(1).DayOfWeek;

        // Today is weekday
        if (today != DayOfWeek.Sunday && today != DayOfWeek.Saturday)
        {
          var className = _config.GetString($"SpecialClasses:{Child}:{today}:Name");
          var imagePath = _config.GetString($"SpecialClasses:{Child}:{today}:ImagePath");

          args.TodaysSpecialClass          = className;
          args.TodaysSpecialClassImagePath = imagePath;
          args.TodayIsWeekday              = true;
        }

        // Tomorrow is weekday
        if (tomorrow != DayOfWeek.Sunday && tomorrow != DayOfWeek.Saturday)
        {
          var className = _config.GetString($"SpecialClasses:{Child}:{tomorrow}:Name");
          var imagePath = _config.GetString($"SpecialClasses:{Child}:{tomorrow}:ImagePath");

          args.TomorrowSpecialClass          = className;
          args.TomorrowSpecialClassImagePath = imagePath;
          args.TomorrowIsWeekday             = true;
        }

        OnClassesUpdated?.Invoke(this, args);
      }
      catch (Exception e)
      {
        await _js.InvokeVoidAsync("alert", $"Error in SlassService.UpdateClasses_Callback. {e}");
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
