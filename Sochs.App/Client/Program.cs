using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sochs.Library;
using Sochs.Library.Interfaces;

namespace Sochs.App
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebAssemblyHostBuilder.CreateDefault(args);
      builder.RootComponents.Add<App>("#app");
      builder.RootComponents.Add<HeadOutlet>("head::after");

      builder.Logging.SetMinimumLevel(LogLevel.Trace);

      builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri("http://api.weatherapi.com") });

      builder.Services.AddSingleton<ITimeService, TimeService>();
      builder.Services.AddSingleton<IWeatherService, WeatherService>();

      await builder.Build().RunAsync();
    }
  }
}