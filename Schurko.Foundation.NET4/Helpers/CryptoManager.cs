
using Microsoft.Extensions.Logging;
using Schurko.Foundation.Logging;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;


#nullable enable
namespace Schurko.Foundation.Utilities
{
  public static class CryptoManager
  {
    private static readonly char[] Padding = new char[1]
    {
      '='
    };

    private static string UrlEncode(string s) => s.TrimEnd(CryptoManager.Padding).Replace('+', '-').Replace('/', '_');

    private static string UrlDecode(string s)
    {
      string str = s.Replace('_', '/').Replace('-', '+');
      switch (s.Length % 4)
      {
        case 2:
          str += "==";
          break;
        case 3:
          str += "=";
          break;
      }
      return str;
    }

    public static string Encrypt(string dataToEncrypt, string salt, bool encodeUrl = true)
    {
      AesManaged aesManaged = new AesManaged();
      byte[] bytes1 = new UTF8Encoding().GetBytes(salt);
      Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(salt, bytes1);
      aesManaged.Key = rfc2898DeriveBytes.GetBytes(16);
      aesManaged.IV = rfc2898DeriveBytes.GetBytes(16);
      aesManaged.BlockSize = 128;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, aesManaged.CreateEncryptor(), CryptoStreamMode.Write))
        {
          byte[] bytes2 = Encoding.UTF8.GetBytes(dataToEncrypt);
          cryptoStream.Write(bytes2, 0, bytes2.Length);
          cryptoStream.FlushFinalBlock();
          cryptoStream.Close();
          return encodeUrl ? HttpUtility.UrlEncode(memoryStream.ToArray()) : Convert.ToBase64String(memoryStream.ToArray());
        }
      }
    }

    public static string Decrypt(string encryptedString, string salt, bool decodeUrl = true)
    {
      AesManaged aesManaged = new AesManaged();
      byte[] buffer = decodeUrl ? Encoding.ASCII.GetBytes(HttpUtility.UrlDecode(encryptedString)) : Convert.FromBase64String(encryptedString);
      if (buffer == null)
      {
        Log.Logger.LogError("CryptoManager.Decrypt Exception");
        throw new Exception("CryptoManager.Decrypt Exception");
      }
      byte[] bytes = new UTF8Encoding().GetBytes(salt);
      Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(salt, bytes);
      aesManaged.Key = rfc2898DeriveBytes.GetBytes(16);
      aesManaged.IV = rfc2898DeriveBytes.GetBytes(16);
      aesManaged.BlockSize = 128;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, aesManaged.CreateDecryptor(), CryptoStreamMode.Write))
        {
          try
          {
            cryptoStream.Write(buffer, 0, buffer.Length);
            cryptoStream.Flush();
            cryptoStream.Close();
          }
          catch (Exception ex)
          {
            throw;
          }
          byte[] array = memoryStream.ToArray();
          return Encoding.UTF8.GetString(array, 0, array.Length);
        }
      }
    }
  }
}
