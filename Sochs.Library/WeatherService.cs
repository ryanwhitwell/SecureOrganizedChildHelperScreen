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

    private readonly decimal _lowThreshold;
    private readonly decimal _midThreshold;
    private readonly decimal _highThreshold;

    private readonly string _hotPath;
    private readonly string _warmPath;
    private readonly string _coolPath;
    private readonly string _coldPath;

    private bool disposedValue;
    private IDictionary<string, string>? _weatherConditionImages;

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

      _weatherConditionImages = GenerateWeatherConditionImageMap();

      _coldPath = _config.GetString("Weather:TemperatureFeeling:Cold");
      _coolPath = _config.GetString("Weather:TemperatureFeeling:Cool");
      _warmPath = _config.GetString("Weather:TemperatureFeeling:Warm");
      _hotPath  = _config.GetString("Weather:TemperatureFeeling:Hot");

      _lowThreshold  = _config.GetDecimal("Weather:Thresholds:Low");
      _midThreshold  = _config.GetDecimal("Weather:Thresholds:Mid");
      _highThreshold = _config.GetDecimal("Weather:Thresholds:High");

      var autoEvent = new AutoResetEvent(false);
			_timer = new Timer(UpdateWeather_Callback, autoEvent, new TimeSpan(0, 0, 0), new TimeSpan(0, UpdateIntervalMinutes, 0));
		}

    private IDictionary<string, string> GenerateWeatherConditionImageMap()
    {
      var configSection = _config.GetSection("Weather:ConditionImagePaths") 
        ?? throw new InvalidOperationException("Cannot read weather ConditionImagePaths config section.");

      IDictionary<string, string> weatherConditionImages = new Dictionary<string, string>();

      foreach (var item in configSection.GetChildren())
      {
        var code = item.Key;
        var path = item.Value;

        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(path)) { throw new InvalidOperationException("Cannot read code or path from Weather ConditionImagePaths config section."); }

        weatherConditionImages.Add(code, path);
      }

      var data = JsonSerializer.Serialize(weatherConditionImages);

      _log.LogTrace("Data as Json {data}", data);

      return weatherConditionImages;
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

    private TemperatureFeeling GetTemperatureFeeling(WeatherApiResponse weatherInfo)
    {
      // Set summary based on Feels Like Temperature
      var tempF = weatherInfo.Current?.FeelsLikeF;

      if (tempF < _lowThreshold)
      {
        return TemperatureFeeling.Cold;
      }
      else if (tempF >= _lowThreshold && tempF <= _midThreshold)
      {
        return TemperatureFeeling.Cool;
      }
      else if (tempF >= _midThreshold && tempF <= _highThreshold)
      {
        return TemperatureFeeling.Warm;
      }
      else if (tempF > _highThreshold)
      {
        return TemperatureFeeling.Hot;
      }
      else
      {
        throw new InvalidOperationException("Cannot determine temperature feeling from weather info.");
      }
    }

    private string GetTemperatureImagePath(WeatherApiResponse? weatherApiResponse)
    {
      _ = weatherApiResponse ?? throw new ArgumentNullException(nameof(weatherApiResponse));

      var temperatureFeeling = GetTemperatureFeeling(weatherApiResponse);

      return temperatureFeeling switch
      {
        TemperatureFeeling.Cold => _coldPath,
        TemperatureFeeling.Cool => _coolPath,
        TemperatureFeeling.Warm => _warmPath,
        TemperatureFeeling.Hot => _hotPath,
        _ => throw new InvalidOperationException($"Cannot determine image path based on temperature feeling {temperatureFeeling}."),
      };
    }

    private void SetTemperatureImagePath(WeatherApiResponse? weatherApiResponse)
    {
      _ = weatherApiResponse ?? throw new ArgumentNullException(nameof(weatherApiResponse));

      string path = GetTemperatureImagePath(weatherApiResponse);

      if (string.IsNullOrWhiteSpace(path)) { throw new InvalidOperationException("Cannot determine temperature image path."); }

      if (weatherApiResponse.Current != null)
      {
        // Set image path for weather condition
        weatherApiResponse.Current.TemperatureImagePath = path;
      }
    }

    private void SetConditionImagePath(WeatherApiResponse? weatherApiResponse)
    {
      _ = weatherApiResponse ?? throw new ArgumentNullException(nameof(weatherApiResponse));

      // Get the code from the Api reponse
      var code = weatherApiResponse.Current?.Condition?.Code ?? throw new InvalidOperationException("Cannot get code from Weather API Response");

      _log.LogTrace("Code from API call: {code}", code);

      // Get the path based on the code from the local data structure
      var path = _weatherConditionImages?[code.ToString()] ?? throw new InvalidOperationException("Cannot get image path from weatherConditionImages");

      _log.LogTrace("Path from data structure: {path}", path);

      // Set image path for weather condition
      weatherApiResponse.Current.Condition.ImagePath = path;
    }

		private async Task WeatherUpdate()
		{
      try
      {
        var response = await _client.GetAsync(_weatherUri).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var responseContentAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var weatherApiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(responseContentAsString) ?? throw new InvalidOperationException($"There was an error parsing the response from Weather API. HTTP Response: {response}");

        SetConditionImagePath(weatherApiResponse);

        SetTemperatureImagePath(weatherApiResponse);

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
          TemperatureF = 70.0m,
          FeelsLikeF = 73.0m,
          Condition = new WeatherApiCondition()
          {
            Text = "MockedCondition",
            Code = 1003
          }
        }
      };

      SetConditionImagePath(mockResponse);

      SetTemperatureImagePath(mockResponse);

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
          _weatherConditionImages = null;
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
