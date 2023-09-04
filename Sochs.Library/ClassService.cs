using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<ClassService> _log;

    private bool disposedValue;

		public ClassService(IConfiguration config, ILogger<ClassService> log)
		{
      _ = config ?? throw new ArgumentNullException(nameof(config));
      _ = log ?? throw new ArgumentNullException(nameof(log));

      _config = config;
      _log = log;

      var autoEvent = new AutoResetEvent(false);
			_timer = new Timer(UpdateClasses_Callback, autoEvent, new TimeSpan(0, 0, 0), new TimeSpan(0, 0, UpdateIntervalMinutes));
		}

    public Child Child { get; set; }

    public event EventHandler<ClassesUpdatedEventArgs>? OnClassesUpdated;

		private void UpdateClasses_Callback(object? stateInfo)
		{
			_ = stateInfo ?? throw new ArgumentNullException(nameof(stateInfo));

      try
      {
        var dayOfWeek = DateTime.Now.DayOfWeek;

        var className = _config.GetString($"SpecialClasses:{Child}:{dayOfWeek}:Name");
        var imagePath = _config.GetString($"SpecialClasses:{Child}:{dayOfWeek}:ImagePath");

        var args = new ClassesUpdatedEventArgs()
        {
          TodaysSpecialClass          = className,
          TodaysSpecialClassImagePath = imagePath,
          IsWeekday                   = dayOfWeek != DayOfWeek.Sunday && dayOfWeek != DayOfWeek.Saturday,
        };

        OnClassesUpdated?.Invoke(this, args);
      }
      catch (Exception ex)
      {
        _log.LogError(ex, "Error updating classes in class service.");
        throw;
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
