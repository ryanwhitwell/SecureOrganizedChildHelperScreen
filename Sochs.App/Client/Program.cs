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

      builder.Logging.SetMinimumLevel(LogLevel.Information);

      builder.Services.AddTransient(sp => new HttpClient());

      builder.Services.AddSingleton<ITimeService, TimeService>();
      builder.Services.AddSingleton<IWeatherService, WeatherService>();
      builder.Services.AddSingleton<ILunchService, LunchService>();

      builder.Services.AddTransient<IDailyTasksService, DailyTasksService>();

      await builder.Build().RunAsync();
    }
  }
}