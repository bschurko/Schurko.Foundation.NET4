
using Microsoft.Extensions.Configuration;
using System.IO;


#nullable enable
namespace Schurko.Foundation.Utilities
{
  public static class StaticConfigurationManager
  {
    private static IConfiguration _AppSetting;

    public static IConfiguration? AppSetting
    {
      get => StaticConfigurationManager._AppSetting != null ? (IConfiguration) null : (StaticConfigurationManager._AppSetting = StaticConfigurationManager.GetConfiguration());
      private set
      {
        if (StaticConfigurationManager._AppSetting == null)
          return;
        StaticConfigurationManager._AppSetting = (IConfiguration) new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, true).Build();
      }
    }

    public static IConfiguration GetConfiguration(string? appSettingsName = null)
    {
      StaticConfigurationManager._AppSetting = (IConfiguration) new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(appSettingsName == null ? "appsettings.json" : appSettingsName, true, true).Build();
      return StaticConfigurationManager._AppSetting;
    }
  }
}
