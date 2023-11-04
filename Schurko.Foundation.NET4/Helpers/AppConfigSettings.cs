
using System;


#nullable enable
namespace Schurko.Foundation.Utilities
{
  public static class AppConfigSettings
  {
    private static Microsoft.Extensions.Configuration.ConfigurationManager _ConfigManager;

    public static Microsoft.Extensions.Configuration.ConfigurationManager? ConfigManager
    {
      get
      {
        AppConfigSettings._ConfigManager = new Microsoft.Extensions.Configuration.ConfigurationManager();
        return AppConfigSettings._ConfigManager;
      }
      private set => AppConfigSettings._ConfigManager = new Microsoft.Extensions.Configuration.ConfigurationManager();
    }

    public static T GetSetting<T>(string key, string def)
    {
      string str = System.Configuration.ConfigurationManager.AppSettings[key] ?? string.Empty;
      if (string.IsNullOrEmpty(str))
        str = def;
      return typeof (T).IsEnum ? (T) Enum.Parse(typeof (T), str) : (T) Convert.ChangeType((object) str, typeof (T));
    }
  }
}
