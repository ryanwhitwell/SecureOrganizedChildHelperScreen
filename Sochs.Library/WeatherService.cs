using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sochs.Library.Events;
using Sochs.Library.Interfaces;
using Sochs.Library.Models;
using System.Text.Json;
using System.Web;

namespace Sochs.Library
{
  public class WeatherService : IWeatherService, IDisposable
	{
		private const int UpdateIntervalMinutes = 10;
    private const string WeatherApiZipCode = "06460";
    private const string WeatherApiBase = "http://api.weatherapi.com";
    private const string WeatherApiResource = "/v1/current.json";

    private readonly Timer _timer;
		private bool disposedValue;
		private readonly HttpClient _client;
		private readonly Uri _weatherUri;
		private readonly IConfiguration _config;
    private readonly ILogger<WeatherService> _log;

    public WeatherService(HttpClient client, IConfiguration config, ILogger<WeatherService> log)
		{
      _ = client ?? throw new ArgumentNullException(nameof(client));
      _ = config ?? throw new ArgumentNullException(nameof(config));
      _ = log ?? throw new ArgumentNullException(nameof(log));

      _client = client;
			_config = config;
			_log = log;

      //_client.BaseAddress = new Uri(WeatherApiBase);
      _weatherUri = GenerateWeatherApiUri();

      var autoEvent = new AutoResetEvent(false);
			_timer = new Timer(UpdateWeather_Callback, autoEvent, new TimeSpan(0, 0, 0), new TimeSpan(0, UpdateIntervalMinutes, 0));
		}

    private Uri GenerateWeatherApiUri()
    {
      // Get API key value from ocnfig file
      var weatherApiKey = _config["Application:WeatherApiKey"];

      if (string.IsNullOrWhiteSpace(weatherApiKey)) { throw new InvalidOperationException("Invalid or missing config value located at Application:WeatherApiKey"); }

      // Setup weather Uri
      var builder = new UriBuilder(WeatherApiBase)
      {
        Path = WeatherApiResource
      };

      var query = HttpUtility.ParseQueryString(string.Empty);
      query["key"] = weatherApiKey;
      query["q"] = WeatherApiZipCode;
      query["aqi"] = "no";
      builder.Query = query.ToString();

      return new Uri(builder.ToString());
    }

    public event EventHandler<WeatherUpdatedEventArgs>? OnWeatherUpdated;

		private async Task WeatherUpdate()
		{
      try
      {
        var response = await _client.GetAsync(_weatherUri).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var responseContentAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var weatherApiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(responseContentAsString) ?? throw new InvalidOperationException($"There was an error parsing the response from Weather API. HTTP Response: {response}");

        var temp = weatherApiResponse.Current?.TempF;
        _log.LogTrace("Temperature: {temp}", temp);

        OnWeatherUpdated?.Invoke(this, new WeatherUpdatedEventArgs() { WeatherInfo = weatherApiResponse });
      }
      catch (Exception ex)
      {
        _log.LogError(ex, "Error getting data from Weather API.");
        throw;
      }
    }

    private void MockWeatherUpdate()
    {
      var iconPath = _config["Icons:TrollFace"];

      if (string.IsNullOrWhiteSpace(iconPath)) { throw new InvalidOperationException("Invalid or missing config value located at Icons:TrollFace"); }

      var mockResponse = new WeatherApiResponse()
      {
        Current = new WeatherApiCurrent()
        {
          TempF = 70.0m,
          Condition = new WeatherApiCondition()
          {
            Text = "MockedCondition",
            Icon = iconPath
          }
        }
      };
      
      OnWeatherUpdated?.Invoke(this, new WeatherUpdatedEventArgs() { WeatherInfo = mockResponse });
    }

    private async void UpdateWeather_Callback(object? stateInfo)
		{
			_ = stateInfo ?? throw new ArgumentNullException(nameof(stateInfo));

      if (bool.TryParse(_config["Application:MockEnabled"], out var mockEnabled) == false) { throw new InvalidOperationException("Invalid or missing config value located at Application:MockEnabled"); }

      if (mockEnabled)
      {
        MockWeatherUpdate();
      }
      else
      {
        await WeatherUpdate().ConfigureAwait(false);
      }
    }

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_timer?.Dispose();
					_client?.Dispose();
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
