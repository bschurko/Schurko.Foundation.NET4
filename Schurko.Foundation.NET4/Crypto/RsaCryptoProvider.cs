
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;


#nullable enable
namespace Schurko.Foundation.Crypto
{
  public class RsaCryptoProvider
  {
    private const int AesKeySize = 32;
    private readonly byte[] _iv = new byte[16]
    {
      (byte) 96,
      byte.MaxValue,
      (byte) 1,
      (byte) 0,
      (byte) 179,
      (byte) 0,
      (byte) 85,
      (byte) 0,
      (byte) 105,
      (byte) 124,
      (byte) 0,
      (byte) 152,
      (byte) 0,
      (byte) 239,
      (byte) 1,
      (byte) 242
    };
    private readonly X509Certificate2 _certificate;

    public RsaCryptoProvider(X509Certificate2 certificate) => this._certificate = certificate;

    public string RsaEncrypt(string data) => Convert.ToBase64String(this.RsaEncrypt(this.StringToBytes(data)));

    public string RsaDecrypt(string encryptedData) => this.BytesToString(this.RsaDecrypt(Convert.FromBase64String(encryptedData)));

    public void RsaAesEncrypt(byte[] data, out string rsaEncryptedKey, out string aesEncryptedData)
    {
      byte[] randomKey = this.CreateRandomKey(32);
      byte[] inArray = this.AesEncrypt(data, randomKey);
      aesEncryptedData = Convert.ToBase64String(inArray);
      rsaEncryptedKey = Convert.ToBase64String(this.RsaEncrypt(randomKey));
    }

    public void RsaAesEncryptString(
      string data,
      out string rsaEncryptedKey,
      out string aesEncryptedData)
    {
      this.RsaAesEncrypt(this.StringToBytes(data), out rsaEncryptedKey, out aesEncryptedData);
    }

    public byte[] RsaAesDecrypt(string rsaEncryptedKey, string aesEncryptedData)
    {
      byte[] aesEncryptionKey = this.RsaDecrypt(Convert.FromBase64String(rsaEncryptedKey));
      return this.AesDecrypt(Convert.FromBase64String(aesEncryptedData), aesEncryptionKey);
    }

    public string RsaAesDecryptString(string rsaEncryptedKey, string aesEncryptedData) => this.BytesToString(this.RsaAesDecrypt(rsaEncryptedKey, aesEncryptedData));

    public byte[] RsaEncrypt(byte[] data)
    {
      if (!(this._certificate.PublicKey.Key is RSACryptoServiceProvider key))
        return (byte[]) null;
      int num = key.KeySize / 8 - 42;
      int length = data.Length;
      if (length > num)
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(83, 2);
        interpolatedStringHandler.AppendLiteral("RsaEncryption can't be used on data larger than ");
        interpolatedStringHandler.AppendFormatted<int>(num);
        interpolatedStringHandler.AppendLiteral(" bytes, currently encrypting ");
        interpolatedStringHandler.AppendFormatted<int>(length);
        interpolatedStringHandler.AppendLiteral(" bytes");
        throw new CryptoException(interpolatedStringHandler.ToStringAndClear());
      }
      return key.Encrypt(data, false);
    }

    public byte[] RsaDecrypt(byte[] encryptedData)
    {
      if (!this._certificate.HasPrivateKey || this._certificate.PrivateKey == null)
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(72, 1);
        interpolatedStringHandler.AppendLiteral("The ");
        interpolatedStringHandler.AppendFormatted<X500DistinguishedName>(this._certificate.SubjectName);
        interpolatedStringHandler.AppendLiteral(" certificate does not contain a private key, decryption not possible");
        throw new CryptoException(interpolatedStringHandler.ToStringAndClear());
      }
      return ((RSACryptoServiceProvider) this._certificate.PrivateKey).Decrypt(encryptedData, false);
    }

    private byte[] AesEncrypt(byte[] data, byte[] aesEncryptionKey)
    {
      using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
      {
        rijndaelManaged.Mode = CipherMode.CBC;
        rijndaelManaged.Padding = PaddingMode.PKCS7;
        rijndaelManaged.KeySize = aesEncryptionKey.Length * 8;
        rijndaelManaged.BlockSize = 128;
        rijndaelManaged.Key = aesEncryptionKey;
        rijndaelManaged.IV = this._iv;
        ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, encryptor, CryptoStreamMode.Write))
          {
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.Flush();
          }
          return memoryStream.ToArray();
        }
      }
    }

    private byte[] AesDecrypt(byte[] encryptedData, byte[] aesEncryptionKey)
    {
      using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
      {
        rijndaelManaged.Mode = CipherMode.CBC;
        rijndaelManaged.Padding = PaddingMode.PKCS7;
        rijndaelManaged.KeySize = aesEncryptionKey.Length * 8;
        rijndaelManaged.BlockSize = 128;
        rijndaelManaged.Key = aesEncryptionKey;
        rijndaelManaged.IV = this._iv;
        ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, decryptor, CryptoStreamMode.Write))
          {
            cryptoStream.Write(encryptedData, 0, encryptedData.Length);
            cryptoStream.Close();
          }
          return memoryStream.ToArray();
        }
      }
    }

    private string BytesToString(byte[] data) => Encoding.UTF8.GetString(data);

    private byte[] StringToBytes(string data) => Encoding.UTF8.GetBytes(data);

    private byte[] CreateRandomKey(int length)
    {
      byte[] data = new byte[length];
      RandomNumberGenerator.Create().GetBytes(data);
      return data;
    }
  }
}
