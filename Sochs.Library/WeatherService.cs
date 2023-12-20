using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sochs.Library.Enums;
using Sochs.Library.Events;
using Sochs.Library.Interfaces;
using Sochs.Library.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace Sochs.Library
{
  public class WeatherService : IWeatherService, IDisposable
	{
		private const int UpdateIntervalMinutes = 1;

    private const string WeatherApiZipCode  = "06460";                      // TODO: Move to config file
    private const string WeatherApiBase     = "http://api.weatherapi.com";  // TODO: Move to config file
    private const string WeatherApiResource = "/v1/current.json";           // TODO: Move to config file

    private readonly int[] RainyWeatherCodes = new int[] 
    {
      1030,1150,1153,1168,1180,1183,1186,1189,
      1192,1195,1240,1243,1246,1273,1276
    };

    private bool disposedValue;

    private readonly Timer _timer;
		private readonly HttpClient _client;
		private readonly Uri _weatherUri;
		private readonly IConfiguration _config;
    private readonly ILogger<WeatherService> _log;
    private readonly IJSRuntime _js;

    public WeatherService(HttpClient client, IConfiguration config, ILogger<WeatherService> log, IJSRuntime js)
		{
      _ = client ?? throw new ArgumentNullException(nameof(client));
      _ = config ?? throw new ArgumentNullException(nameof(config));
      _ = log ?? throw new ArgumentNullException(nameof(log));
      _ = js ?? throw new ArgumentNullException(nameof(js));

      _client = client;
			_config = config;
			_log = log;
      _js = js;

      _client.BaseAddress = new Uri(WeatherApiBase);
      _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true, MustRevalidate = true };

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
      
      query["key"]  = weatherApiKey;
      query["q"]    = WeatherApiZipCode;
      query["aqi"]  = "no";
      builder.Query = query.ToString();

      return new Uri(builder.ToString());
    }

    public event EventHandler<WeatherUpdatedEventArgs>? OnWeatherUpdated;

    private string GetShoesImagePath(FeelsLikeTemp feelsLikeTemp)
    {
      return feelsLikeTemp switch
      {
        FeelsLikeTemp.Hot  => _config.GetString("Clothes:Shoes:Sandals"),
        FeelsLikeTemp.Warm => _config.GetString("Clothes:Shoes:Sneakers"),
        FeelsLikeTemp.Cool => _config.GetString("Clothes:Shoes:Boots"),
        FeelsLikeTemp.Cold => _config.GetString("Clothes:Shoes:WinterBoots"),
        _                  => throw new InvalidOperationException("Cannot determine shoes from weather info.")
      };
    }

    private string GetPantsImagePath(FeelsLikeTemp feelsLikeTemp)
    {
      return feelsLikeTemp switch
      {
        FeelsLikeTemp.Hot  => _config.GetString("Clothes:Pants:ShortPants"),
        FeelsLikeTemp.Warm => _config.GetString("Clothes:Pants:ShortPants"),
        FeelsLikeTemp.Cool => _config.GetString("Clothes:Pants:LongPants"),
        FeelsLikeTemp.Cold => _config.GetString("Clothes:Pants:LongPants"),
        _                  => throw new InvalidOperationException("Cannot determine pants from weather info.")
      };
    }

    private string GetShirtImagePath(FeelsLikeTemp feelsLikeTemp)
    {
      return feelsLikeTemp switch
      {
        FeelsLikeTemp.Hot  => _config.GetString("Clothes:Shirts:ShortShirt"),
        FeelsLikeTemp.Warm => _config.GetString("Clothes:Shirts:ShortShirt"),
        FeelsLikeTemp.Cool => _config.GetString("Clothes:Shirts:LongShirt"),
        FeelsLikeTemp.Cold => _config.GetString("Clothes:Shirts:LongShirt"),
        _                  => throw new InvalidOperationException("Cannot determine shirt from weather info.")
      };
    }

    private string GetJacketImagePath(WeatherApiResponse? weatherApiResponse)
    {
      _ = weatherApiResponse ?? throw new ArgumentNullException(nameof(weatherApiResponse));

      // Any rain means raincoat
      var isRainy = IsRainy(weatherApiResponse);

      if (isRainy)
      {
        return _config.GetString("Clothes:Jackets:RainCoat");
      }

      var feelsLike = GetFeelsLikeTemp(weatherApiResponse);

      return feelsLike switch
      {
        FeelsLikeTemp.Hot  => string.Empty,
        FeelsLikeTemp.Warm => string.Empty,
        FeelsLikeTemp.Cool => _config.GetString("Clothes:Jackets:LightJacket"),
        FeelsLikeTemp.Cold => _config.GetString("Clothes:Jackets:HeavyJacket"),
        _                  => throw new InvalidOperationException($"Cannot determine shirt from weather info.{JsonSerializer.Serialize(weatherApiResponse)}")
      };
    }

    private bool IsRainy(WeatherApiResponse? weatherApiResponse)
    {
      _ = weatherApiResponse ?? throw new ArgumentNullException(nameof(weatherApiResponse));

      var conditionCode = weatherApiResponse.Current?.Condition?.Code;

      return Array.IndexOf(RainyWeatherCodes, conditionCode) >= 0 ;
    }

    private FeelsLikeTemp GetFeelsLikeTemp(WeatherApiResponse? weatherApiResponse)
    {
      _ = weatherApiResponse ?? throw new ArgumentNullException(nameof(weatherApiResponse));

      // Set summary based on Feels Like Temperature
      var tempF = weatherApiResponse.Current?.FeelsLikeF;

      var lowThreshold  = _config.GetDecimal("Weather:Thresholds:Low");
      var midThreshold  = _config.GetDecimal("Weather:Thresholds:Mid");
      var highThreshold = _config.GetDecimal("Weather:Thresholds:High");

      if (tempF < lowThreshold)
      {
        return FeelsLikeTemp.Cold;
      }
      else if (tempF >= lowThreshold && tempF <= midThreshold)
      {
        return FeelsLikeTemp.Cool;
      }
      else if (tempF >= midThreshold && tempF <= highThreshold)
      {
        return FeelsLikeTemp.Warm;
      }
      else if (tempF > highThreshold)
      {
        return FeelsLikeTemp.Hot;
      }
      else
      {
        throw new InvalidOperationException($"Cannot determine feel like temp from weather info. {JsonSerializer.Serialize(weatherApiResponse) }");
      }
    }

    private string GetTemperatureImagePath(FeelsLikeTemp feelsLikeTemp)
    {
      return feelsLikeTemp switch
      {
        FeelsLikeTemp.Cold => _config.GetString("Weather:TemperatureFeelingImagePaths:Cold"),
        FeelsLikeTemp.Cool => _config.GetString("Weather:TemperatureFeelingImagePaths:Cool"),
        FeelsLikeTemp.Warm => _config.GetString("Weather:TemperatureFeelingImagePaths:Warm"),
        FeelsLikeTemp.Hot  => _config.GetString("Weather:TemperatureFeelingImagePaths:Hot"),
        _                  => throw new InvalidOperationException("Cannot determine temperature feeling from weather info.")
      };
    }

    private string GetConditionImagePath(WeatherApiResponse? weatherApiResponse)
    {
      _ = weatherApiResponse ?? throw new ArgumentNullException(nameof(weatherApiResponse));

      // Get the code from the Api reponse
      var code = weatherApiResponse.Current?.Condition?.Code ?? throw new InvalidOperationException($"Cannot get code from Weather API Response. {JsonSerializer.Serialize(weatherApiResponse) }");

      // Get the path based on the code from the local data structure
      var path = _config.GetString($"Weather:ConditionImagePaths:{code}");

      return path;
    }

		private async Task WeatherUpdate()
		{
      var response = await _client.GetAsync(_weatherUri).ConfigureAwait(false);

      response.EnsureSuccessStatusCode();

      var responseContentAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

      _log.LogTrace("Weather API Response: {responseContentAsString}", responseContentAsString);

      var weatherApiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(responseContentAsString) ?? throw new InvalidOperationException($"There was an error parsing the response from Weather API. HTTP Response: {response}");

      var feelsLikeTemp = GetFeelsLikeTemp(weatherApiResponse);
      var conditionImagePath = GetConditionImagePath(weatherApiResponse);
      var temperatureImagePath = GetTemperatureImagePath(feelsLikeTemp);
      var shoesImagePath = GetShoesImagePath(feelsLikeTemp);
      var jacketImagePath = GetJacketImagePath(weatherApiResponse);
      var pantsImagePath = GetPantsImagePath(feelsLikeTemp);
      var shirtImagePath = GetShirtImagePath(feelsLikeTemp);

      var args = new WeatherUpdatedEventArgs()
      {
        WeatherInfo = weatherApiResponse,
        ConditionImagePath = conditionImagePath,
        TemperatureImagePath = temperatureImagePath,
        ShirtImagePath = shirtImagePath,
        PantsImagePath = pantsImagePath,
        ShoesImagePath = shoesImagePath,
        JacketImagePath = jacketImagePath
      };

      OnWeatherUpdated?.Invoke(this, args);
    }

    private void MockWeatherUpdate()
    {
      var weatherApiResponse = new WeatherApiResponse()
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

      var feelsLikeTemp        = GetFeelsLikeTemp(weatherApiResponse);
      var conditionImagePath   = GetConditionImagePath(weatherApiResponse);
      var temperatureImagePath = GetTemperatureImagePath(feelsLikeTemp);
      var shoesImagePath       = GetShoesImagePath(feelsLikeTemp);
      var jacketImagePath      = GetJacketImagePath(weatherApiResponse);
      var pantsImagePath       = GetPantsImagePath(feelsLikeTemp);
      var shirtImagePath       = GetShirtImagePath(feelsLikeTemp);

      var args = new WeatherUpdatedEventArgs()
      {
        WeatherInfo          = weatherApiResponse,
        ConditionImagePath   = conditionImagePath,
        TemperatureImagePath = temperatureImagePath,
        ShirtImagePath       = shirtImagePath,
        PantsImagePath       = pantsImagePath,
        ShoesImagePath       = shoesImagePath,
        JacketImagePath      = jacketImagePath
      };

      OnWeatherUpdated?.Invoke(this, args);
    }

    private async void UpdateWeather_Callback(object? stateInfo)
		{
      try
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
      catch (Exception e)
      {
        // await _js.InvokeVoidAsync("alert", $"Error in WeatherService.UpdateWeather_Callback. {e}");
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

          // Remove the Http Client
					_client?.Dispose();
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
