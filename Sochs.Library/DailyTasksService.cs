using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
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
    private readonly IJSRuntime _js;

    public DailyTasksService(IConfiguration config, IJSRuntime js)
    {
      _ = config ?? throw new ArgumentNullException(nameof(config));
      _ = js ?? throw new ArgumentNullException(nameof(js));

      _config = config;
      _js = js;

      var autoEvent = new AutoResetEvent(false);
      _timer = new Timer(UpdateDailyTasks_Callback, autoEvent, new TimeSpan(0, 0, 0), new TimeSpan(0, 0, UpdateIntervalSeconds));
    }

    public event EventHandler<DailyTasksResetEventArgs>? OnDailyTasksReset;
    public event EventHandler<DailyTaskUpdatedEventArgs>? OnDailyTaskUpdated;

    public Child Child { get; set; }

    public void UpdateTask(int id, bool isCompleted)
    {
      if (_taskData == null || !_taskData.HasData) { throw new InvalidOperationException("Cannot update task. Task data is empty or null."); }

      _taskData.Tasks[id].IsCompleted = isCompleted;

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
      var tasksSection = _config.GetSection("Tasks");

      // Index the data to maintain task order
      var index = 0;

      // Child
      foreach (var allChildren in tasksSection.GetChildren())
      {
        var childKey = allChildren.Key;

        var child = Enum.Parse<Child>(childKey);

        // DayOfWeek
        foreach (var allDaysOfWeek in allChildren.GetChildren())
        {
          var dayOfWeekKey = allDaysOfWeek.Key;

          var dayOfWeek = Enum.Parse<DayOfWeek>(dayOfWeekKey);

          // TimeOfDay
          foreach (var allTimeOfDay in allDaysOfWeek.GetChildren())
          {
            var timeOfDayKey = allTimeOfDay.Key;

            var timeOfDay = Enum.Parse<TimeOfDay>(timeOfDayKey);

            // Tasks
            foreach (var task in allTimeOfDay.GetChildren())
            {
              var newTask = new DailyTask()
              {
                Id          = index,
                Description = task.GetString("Description"),
                ImagePath   = task.GetString("ImagePath"),
                Child       = child,
                DayOfWeek   = dayOfWeek,
                TimeOfDay   = timeOfDay
              };

              data.Tasks.TryAdd(newTask.Id, newTask);

              // increment the index after adding an item
              index++;
            }
          }
        }
      }

      return data;
    }

    private async void UpdateDailyTasks_Callback(object? stateInfo)
    {
      try
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
      catch (Exception e)
      {
        await _js.InvokeVoidAsync("alert", $"Error in DailyTasksService.UpdateDailyTasks_Callback. {e}");
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

          // Mark task data for GC
          _taskData = null;
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
