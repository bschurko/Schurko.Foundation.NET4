
using System;



namespace Schurko.Foundation.Utilities
{
  public static class AppConfigSettings
  {
  
    public static T GetSetting<T>(string key, string def)
    {
      string str = System.Configuration.ConfigurationManager.AppSettings[key] ?? string.Empty;
      if (string.IsNullOrEmpty(str))
        str = def;
      return typeof (T).IsEnum ? (T) Enum.Parse(typeof (T), str) : (T) Convert.ChangeType((object) str, typeof (T));
    }
  }
}
