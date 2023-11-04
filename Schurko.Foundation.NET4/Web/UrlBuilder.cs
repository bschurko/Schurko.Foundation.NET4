
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


#nullable enable
namespace Schurko.Foundation.Web
{
  public class UrlBuilder
  {
    public const char PATH_SEPARATOR_CHAR = '/';

    private List<string> Parts { get; set; }

    private List<KeyValuePair<string, string>> QueryString { get; set; }

    public UrlBuilder(string baseUrl)
    {
      if (string.IsNullOrEmpty(baseUrl))
        throw new ArgumentException("baseUrl is empty.");
      this.Parts = new List<string>();
      this.QueryString = new List<KeyValuePair<string, string>>();
      while (baseUrl.EndsWith("/"))
        baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
      this.Parts.Add(baseUrl);
    }

    public UrlBuilder AddPart(string part)
    {
      if (!string.IsNullOrEmpty(part))
        this.Parts.Add(this.StripBoth(part));
      return this;
    }

    public UrlBuilder AddQueryString(string key, string value)
    {
      if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
        this.QueryString.Add(new KeyValuePair<string, string>(key, value));
      return this;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = string.Join("/", this.Parts.ToArray());
      List<string> list = this.QueryString.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (s => string.Format("{0}={1}", (object) s.Key, (object) s.Value))).ToList<string>();
      stringBuilder.Append(str1);
      if (list.Count > 0)
      {
        stringBuilder.Append("?");
        foreach (string str2 in list)
        {
          stringBuilder.Append(str2);
          stringBuilder.Append("&");
        }
        if (stringBuilder[stringBuilder.Length - 1] == '&')
          stringBuilder.Remove(stringBuilder.Length - 1, 1);
      }
      return stringBuilder.ToString();
    }

    private string StripBoth(string part) => part.Trim(new char[1]
    {
      '/'
    });
  }
}
