using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sochs.Library.Events;
using Sochs.Library.Interfaces;
using Sochs.Library.Models;
using System.Text.Json;
using System.Web;

namespace Sochs.Library
{
  public class LunchService : ILunchService, IDisposable
	{
    private const int UpdateIntervalHours = 4;

    private const string LunchApiBase        = "https://thrillshare-cmsv2.services.thrillshare.com";
    private const string LunchApiAllResource = "/api/v2/s/126312/menus";

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

      client.BaseAddress = new Uri(LunchApiBase);
      _lunchUri = GenerateLunchUri();

      var autoEvent = new AutoResetEvent(false);
			_timer = new Timer(UpdateLunch_Callback, autoEvent, new TimeSpan(0, 0, 0), new TimeSpan(UpdateIntervalHours, 0, 0));
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

        var args = new LunchUpdatedEventArgs()
        {
          LunchInfo = apiResponse
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
      var apiResponse = new LunchApiResponse()
      {
        Menus = new List<LunchApiMenu>()
      };

      var args = new LunchUpdatedEventArgs()
      {
        LunchInfo = apiResponse
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
