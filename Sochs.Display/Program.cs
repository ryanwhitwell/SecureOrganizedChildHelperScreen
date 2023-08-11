using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sochs.Core;
using Sochs.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sochs.Display
{
  internal static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var services = new ServiceCollection();
      ConfigureServices(services);
      using (ServiceProvider serviceProvider = services.BuildServiceProvider())
      {
        var form1 = serviceProvider.GetRequiredService<MainDisplay>();
        Application.Run(form1);
      }
    }

    private static void ConfigureServices(ServiceCollection services)
    {
      services.AddSingleton<MainDisplay>()
              .AddLogging(configure => configure.AddConsole());
    }
  }
}
