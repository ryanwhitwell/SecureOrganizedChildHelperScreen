using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sochs.Library.Enums;
using Sochs.Library.Events;
using Sochs.Library.Interfaces;
using Sochs.Library.Models;

namespace Sochs.Library
{
  public class DailyTasksService : IDailyTasksService, IDisposable
  {
    private const int UpdateIntervalSeconds = 1;

    private bool disposedValue;

    // This is the container for the concurrent dictionary of tasks read in from the config file
    private DailyTaskData? _taskData;

    private readonly Timer _timer;
    private readonly IConfiguration _config;
    private readonly ILogger<DailyTasksService> _log;

    public DailyTasksService(IConfiguration config, ILogger<DailyTasksService> log)
    {
      _ = config ?? throw new ArgumentNullException(nameof(config));
      _ = log ?? throw new ArgumentNullException(nameof(log));

      _config = config;
      _log = log;

      var autoEvent = new AutoResetEvent(false);
      _timer = new Timer(UpdateDailyTasks_Callback, autoEvent, new TimeSpan(0, 0, 0), new TimeSpan(0, 0, UpdateIntervalSeconds));
    }

    public event EventHandler<ActiveDailyTasksChangeEventArgs>? OnActiveDailyTasksChange;
    public event EventHandler<DailyTasksResetEventArgs>? OnDailyTasksReset;
    public event EventHandler<DailyTaskUpdatedEventArgs>? OnDailyTaskUpdated;

    public Child Child { get; set; }

    public void UpdateTask(string name, bool isCompleted)
    {
      if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentNullException(nameof(name)); }

      if (_taskData == null || !_taskData.HasData) { throw new InvalidOperationException("Cannot update task. Task data is empty or null."); }

      _taskData.Tasks[name].IsCompleted = isCompleted;

      OnDailyTaskUpdated?.Invoke(this, new DailyTaskUpdatedEventArgs() { TaskData = _taskData });
    }

    private DailyTaskData GetNewDailyTaskData()
    {
      // Read from config
      var data = new DailyTaskData()
      {
        DateTime = DateTime.Now
      };

      // Load data structure from config file
      var allTasks = _config.GetSection("Tasks");

      foreach (var item in allTasks.GetChildren())
      {
        var key         = item.Key;
        var description = item.GetString("Description");
        var imagePath   = item.GetString("ImagePath");
        var child       = item.GetEnum<Child>("Child");
        var timeOfDay   = item.GetEnum<TimeOfDay>("TimeOfDay");
        var dayType     = item.GetEnum<DayType>("DayType");

        var newTask = new DailyTask()
        {
          Id          = key,
          Description = description,
          ImagePath   = imagePath,
          Child       = child,
          TimeOfDay   = timeOfDay,
          DayType     = dayType
        };

        data.Tasks.TryAdd(key, newTask);
      }

      return data;
    }

    private void UpdateDailyTasks_Callback(object? stateInfo)
    {
      // If there's already task data and the current Date is NOT different from the DateTime.Date in the data, then do nothing
      if (_taskData != null && _taskData.DateTime.Date == DateTime.Now.Date)
      {
        return;
      }

      // If current day IS different from the currentTaskDay, then delete and regenerate the collection and fire the reset event
      _taskData = GetNewDailyTaskData();
      OnDailyTasksReset?.Invoke(this, new DailyTasksResetEventArgs() { TaskData = _taskData });
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          _timer?.Dispose();
          _taskData = null;
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
