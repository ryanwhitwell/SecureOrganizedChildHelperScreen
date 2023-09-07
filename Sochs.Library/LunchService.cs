using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sochs.Library.Events;
using Sochs.Library.Interfaces;
using Sochs.Library.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace Sochs.Library
{
  public class LunchService : ILunchService, IDisposable
	{
    private const int UpdateIntervalMinutes = 30;
    private const int MaxLunchItems = 4;

    private const string LunchApiBase        = "https://thrillshare-cmsv2.services.thrillshare.com";  // TODO: Move to config file
    private const string LunchApiAllResource = "/api/v2/s/126312/menus";                              // TODO: Move to config file

    private readonly Timer _timer;
    private readonly IConfiguration _config;
    private readonly HttpClient _client;
    private readonly Uri _lunchUri;
    private readonly ILogger<LunchService> _log;

    private bool disposedValue;

		public LunchService(HttpClient client, IConfiguration config, ILogger<LunchService> log)
		{
      _ = client ?? throw new ArgumentNullException(nameof(client));
      _ = config ?? throw new ArgumentNullException(nameof(config));
      _ = log ?? throw new ArgumentNullException(nameof(log));

      _client = client;
      _config = config;
      _log = log;

      _client.BaseAddress = new Uri(LunchApiBase);
      _client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() {  NoCache = true, MustRevalidate = true  };

      _lunchUri = GenerateLunchUri();

      var autoEvent = new AutoResetEvent(false);
			_timer = new Timer(UpdateLunch_Callback, autoEvent, new TimeSpan(0, 0, 0), new TimeSpan(0, 0, UpdateIntervalMinutes));
		}

    private static Uri GenerateLunchUri()
    {
      // Setup lunch Uri
      var builder = new UriBuilder(LunchApiBase)
      {
        Path = LunchApiAllResource
      };

      var query = HttpUtility.ParseQueryString(string.Empty);
      query["locale"] = "en";
      query["query_id"] = "21637";
      builder.Query = query.ToString();

      return new Uri(builder.ToString());
    }

    public event EventHandler<LunchUpdatedEventArgs>? OnLunchUpdated;

		private async void UpdateLunch_Callback(object? stateInfo)
		{
			_ = stateInfo ?? throw new ArgumentNullException(nameof(stateInfo));

      if (bool.TryParse(_config["Application:MockEnabled"], out var mockEnabled) == false) { throw new InvalidOperationException("Invalid or missing config value located at Application:MockEnabled"); }

      if (mockEnabled)
      {
        MockLunchUpdated();
      }
      else
      {
        await LunchUpdate().ConfigureAwait(false);
      }
    }

    private async Task LunchUpdate()
    {
      try
      {
        var response = await _client.GetAsync(_lunchUri).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var responseContentAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        _log.LogTrace("Lunch API Response: {responseContentAsString}", responseContentAsString);

        var apiResponse = JsonSerializer.Deserialize<LunchApiResponse>(responseContentAsString) ?? throw new InvalidOperationException($"There was an error parsing the response from Weather API. HTTP Response: {response}");

        var todayString    = DateTime.Now.ToString("yyyy-MM-dd");
        var tommorowString = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
        var nextDayString  = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");

        var todayLunch    = apiResponse.Menus?.Where(x => x.Name.Equals(todayString, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.Lunch.Split("\r\n").Take(MaxLunchItems) ?? Array.Empty<string>();
        var tomorrowLunch = apiResponse.Menus?.Where(x => x.Name.Equals(tommorowString, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.Lunch.Split("\r\n").Take(MaxLunchItems) ?? Array.Empty<string>();
        var nextDayLunch  = apiResponse.Menus?.Where(x => x.Name.Equals(nextDayString, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.Lunch.Split("\r\n").Take(MaxLunchItems) ?? Array.Empty<string>();

        var args = new LunchUpdatedEventArgs()
        {
          TodayLunch    = todayLunch,
          TomorrowLunch = tomorrowLunch,
          NextDayLunch  = nextDayLunch
        };

        OnLunchUpdated?.Invoke(this, args);
      }
      catch (Exception ex)
      {
        _log.LogError(ex, "Error getting data from Lunch API.");
        throw;
      }
    }

    private void MockLunchUpdated()
    {
      var args = new LunchUpdatedEventArgs()
      {
        TodayLunch    = "Penne Pasta w/Marinara Sauce & Meatballs\r\nBagel w/ Yogurt & string cheese.\r\nHummus Plate w/String Cheese".Split("\r\n"),
        TomorrowLunch = "Sals Fresh Pizza\r\nBagel w/ Yogurt & string cheese.\r\nHummus Plate w/String Cheese\r\nPotato Crusted Fish w/ Lemon & Dinner Roll".Split("\r\n"),
        NextDayLunch  = "Chicken Patty on a WG Bun\r\nBagel w/ Yogurt & string cheese.\r\nHummus Plate w/String Cheese".Split("\r\n")
      };

      OnLunchUpdated?.Invoke(this, args);
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
