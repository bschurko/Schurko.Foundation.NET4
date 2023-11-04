
using System.Text;


#nullable enable
namespace Schurko.Foundation.Extensions.IO
{
  public static class Extensions
  {
    public static byte[] ToUtf8ByteArray(this string str) => Encoding.UTF8.GetBytes(str);

    public static string ToUtf8String(this byte[] bytes) => Encoding.UTF8.GetString(bytes);
  }
}
