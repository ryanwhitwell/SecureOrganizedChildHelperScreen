using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sochs.Library.Enums;
using Sochs.Library.Events;
using Sochs.Library.Interfaces;
using Sochs.Library.Models;
using System.Text.Json;
using System.Web;

namespace Sochs.Library
{
  public class WeatherService : IWeatherService, IDisposable
	{
		private const int UpdateIntervalMinutes = 1;
    private const string WeatherApiZipCode = "06460";
    private const string WeatherApiBase = "http://api.weatherapi.com";
    private const string WeatherApiResource = "/v1/current.json";

    private bool disposedValue;

    private readonly Timer _timer;
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

    private string GetShoesImagePath(WeatherApiResponse? weatherApiResponse)
    {
      _ = weatherApiResponse ?? throw new ArgumentNullException(nameof(weatherApiResponse));

      throw new NotImplementedException();
    }

    private string GetPantsImagePath(WeatherApiResponse? weatherApiResponse)
    {
      _ = weatherApiResponse ?? throw new ArgumentNullException(nameof(weatherApiResponse));
      throw new NotImplementedException();

    }

    private string GetShirtImagePath(WeatherApiResponse? weatherApiResponse)
    {
      _ = weatherApiResponse ?? throw new ArgumentNullException(nameof(weatherApiResponse));
      throw new NotImplementedException();

    }

    private string GetJacketImagePath(WeatherApiResponse? weatherApiResponse)
    {
      _ = weatherApiResponse ?? throw new ArgumentNullException(nameof(weatherApiResponse));
      throw new NotImplementedException();
    }

    private string GetTemperatureImagePath(WeatherApiResponse? weatherApiResponse)
    {
      _ = weatherApiResponse ?? throw new ArgumentNullException(nameof(weatherApiResponse));

      // Set summary based on Feels Like Temperature
      var tempF = weatherApiResponse.Current?.FeelsLikeF;

      var lowThreshold  = _config.GetDecimal("Weather:Thresholds:Low");
      var midThreshold  = _config.GetDecimal("Weather:Thresholds:Mid");
      var highThreshold = _config.GetDecimal("Weather:Thresholds:High");

      if (tempF < lowThreshold)
      {
        return _config.GetString("Weather:TemperatureFeelingImagePaths:Cold");
      }
      else if (tempF >= lowThreshold && tempF <= midThreshold)
      {
        return _config.GetString("Weather:TemperatureFeelingImagePaths:Cool");
      }
      else if (tempF >= midThreshold && tempF <= highThreshold)
      {
        return _config.GetString("Weather:TemperatureFeelingImagePaths:Warm");
      }
      else if (tempF > highThreshold)
      {
        return _config.GetString("Weather:TemperatureFeelingImagePaths:Hot");
      }
      else
      {
        throw new InvalidOperationException("Cannot determine temperature feeling from weather info.");
      }
    }

    private string GetConditionImagePath(WeatherApiResponse? weatherApiResponse)
    {
      _ = weatherApiResponse ?? throw new ArgumentNullException(nameof(weatherApiResponse));

      // Get the code from the Api reponse
      var code = weatherApiResponse.Current?.Condition?.Code ?? throw new InvalidOperationException("Cannot get code from Weather API Response");

      // Get the path based on the code from the local data structure
      var path = _config.GetString($"Weather:ConditionImagePaths:{code}");

      return path;
    }

		private async Task WeatherUpdate()
		{
      try
      {
        var response = await _client.GetAsync(_weatherUri).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var responseContentAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var weatherApiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(responseContentAsString) ?? throw new InvalidOperationException($"There was an error parsing the response from Weather API. HTTP Response: {response}");

        var conditionImagePath = GetConditionImagePath(weatherApiResponse);

        var temperatureImagePath = GetTemperatureImagePath(weatherApiResponse);

        var args = new WeatherUpdatedEventArgs()
        {
          WeatherInfo          = weatherApiResponse,
          ConditionImagePath   = conditionImagePath,
          TemperatureImagePath = temperatureImagePath
        };

        OnWeatherUpdated?.Invoke(this, args);
      }
      catch (Exception ex)
      {
        _log.LogError(ex, "Error getting data from Weather API.");
        throw;
      }
    }

    private void MockWeatherUpdate()
    {
      var mockResponse = new WeatherApiResponse()
      {
        Current = new WeatherApiCurrent()
        {
          TemperatureF = 70.0m,
          FeelsLikeF = 73.0m,
          Condition = new WeatherApiCondition()
          {
            Text = "MockedCondition",
            Code = 1003
          }
        }
      };

      var conditionImagePath = GetConditionImagePath(mockResponse);

      var temperatureImagePath = GetTemperatureImagePath(mockResponse);

      var args = new WeatherUpdatedEventArgs()
      {
        WeatherInfo = mockResponse,
        ConditionImagePath = conditionImagePath,
        TemperatureImagePath = temperatureImagePath
      };

      OnWeatherUpdated?.Invoke(this, args);
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
