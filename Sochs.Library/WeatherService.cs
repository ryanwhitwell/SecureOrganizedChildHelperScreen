using Sochs.Library.Events;
using Sochs.Library.Interfaces;

namespace Sochs.Library
{
  public class WeatherService : IWeatherService, IDisposable
	{
		private readonly Timer _timer;
		private bool disposedValue;
		private readonly HttpClient _client;

    public WeatherService(HttpClient client)
		{
      _ = client ?? throw new ArgumentNullException(nameof(client));
			_client = client;

			var autoEvent = new AutoResetEvent(false);
			_timer = new Timer(UpdateWeather_Callback, autoEvent, new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 5));
		}

		public event EventHandler<WeatherUpdatedEventArgs>? OnWeatherUpdated;

		private void UpdateWeather_Callback(object? stateInfo)
		{
			_ = stateInfo ?? throw new ArgumentNullException(nameof(stateInfo));

			AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;

			OnWeatherUpdated?.Invoke(this, new WeatherUpdatedEventArgs() { WeatherInfo = "foobar weather data" });
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
