using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sochs.Library.Enums;
using Sochs.Library.Events;
using Sochs.Library.Interfaces;
using Sochs.Library.Models;

namespace Sochs.Library
{
  public class DateEventService : IDateEventService, IDisposable
	{
    private const int UpdateIntervalMinutes = 30;

    private readonly Timer _timer;
    private readonly IConfiguration _config;
    private readonly IJSRuntime _js;

    private bool disposedValue;

		public DateEventService(IConfiguration config, IJSRuntime js)
		{
      _ = config ?? throw new ArgumentNullException(nameof(config));
      _ = js ?? throw new ArgumentNullException(nameof(js));

      _config = config;
      _js = js;

      var autoEvent = new AutoResetEvent(false);
			_timer = new Timer(UpdateDateEvents_Callback, autoEvent, new TimeSpan(0, 0, 0), new TimeSpan(0, 0, UpdateIntervalMinutes));
		}

    public event EventHandler<DateEventsUpdatedEventArgs>? OnDateEventsUpdated;

    private async void UpdateDateEvents_Callback(object? stateInfo)
		{
      try
      {
        _ = stateInfo ?? throw new ArgumentNullException(nameof(stateInfo));

        var args = new DateEventsUpdatedEventArgs
        {
          DateEventImagePath = _config.GetString("Application:DateEventImagePath")
        };

        var dateEvents = new List<DateEvent>();

        var allConfigDateEvents = _config.GetSection("DateEvents");

        foreach (var configEvent in allConfigDateEvents.GetChildren())
        {
          var newDateEvent = new DateEvent()
          {
            Date        = DateTime.Parse(configEvent.GetString("Date")),
            Description = configEvent.GetString("Description"),
            ImagePath   = configEvent.GetString("ImagePath")
          };

          dateEvents.Add(newDateEvent);
        }

        args.DateEvents = dateEvents;

        OnDateEventsUpdated?.Invoke(this, args);
      }
      catch (Exception e)
      {
        await Task.Run(() => Console.WriteLine($"Error in DateEventService.UpdateDateEvents_Callback. {e}"));
        // await _js.InvokeVoidAsync("alert", $"Error in DateEventService.UpdateDateEvents_Callback. {e}");
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
