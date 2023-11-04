
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;


#nullable enable
namespace Schurko.Foundation.Crypto
{
  public class EncryptionProvider
  {
    private static readonly string EncryptionCertificateSubject;
    public static string EncryptionAlgorithm = "AES/CBC/PKCS5Padding";
    public static string SignatureAlgorithm = "SHA1withRSA";
    private static X509Certificate2 encryptionCertificate;

    public static X509Certificate2 EncryptionCertificate
    {
      get
      {
        X509Certificate2 encryptionCertificate = EncryptionProvider.encryptionCertificate;
        if (encryptionCertificate != null)
          return encryptionCertificate;
        string certificateSubject = EncryptionProvider.EncryptionCertificateSubject;
        StoreName? storeName = new StoreName?();
        return EncryptionProvider.encryptionCertificate = CertificateManager.FindCertificateBySubjectName(certificateSubject, storeName);
      }
    }

    public static bool VerifyWithGpasPublicKey(byte[] data, byte[] signature) => ((RSACryptoServiceProvider) EncryptionProvider.EncryptionCertificate.PublicKey.Key).VerifyData(data, (object) "SHA1", signature);

    public static byte[] Encrypt(string textToEncrypt) => EncryptionProvider.Encrypt(Encoding.UTF8.GetBytes(textToEncrypt));

    public static byte[] Encrypt(byte[] textToEncrypt)
    {
      if (!(EncryptionProvider.EncryptionCertificate.PublicKey.Key is RSACryptoServiceProvider key))
        throw new ApplicationException("Failed to find public key in certificate during payment request encryption.");
      return key.Encrypt(textToEncrypt, false);
    }

    public static byte[] Decrypt(string textToEncrypt) => EncryptionProvider.Decrypt(Convert.FromBase64String(textToEncrypt));

    public static byte[] Decrypt(byte[] textToEncrypt)
    {
      if (!(EncryptionProvider.EncryptionCertificate.PrivateKey is RSACryptoServiceProvider privateKey))
        throw new ApplicationException("Failed to find private key in certificate during payment request encryption.");
      return privateKey.Decrypt(textToEncrypt, false);
    }

    public static void RsaAesEncrypt(
      byte[] data,
      out string rsaEncryptedKey,
      out string aesEncryptedData)
    {
      new RsaCryptoProvider(EncryptionProvider.EncryptionCertificate).RsaAesEncrypt(data, out rsaEncryptedKey, out aesEncryptedData);
    }

    public static byte[] RsaAesDecrypt(string rsaEncryptedKey, string aesEncryptedData) => new RsaCryptoProvider(EncryptionProvider.EncryptionCertificate).RsaAesDecrypt(rsaEncryptedKey, aesEncryptedData);

    public static byte[] EncryptStringToBytes(string plainText, byte[] Key)
    {
      if (plainText == null || plainText.Length <= 0)
        throw new ArgumentNullException(nameof (plainText));
      if (Key == null || Key.Length == 0)
        throw new ArgumentNullException(nameof (Key));
      byte[] numArray = new byte[16];
      using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
      {
        rijndaelManaged.Mode = CipherMode.CBC;
        rijndaelManaged.Padding = PaddingMode.PKCS7;
        rijndaelManaged.KeySize = 128;
        rijndaelManaged.BlockSize = 128;
        rijndaelManaged.Key = Key;
        rijndaelManaged.IV = numArray;
        ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, encryptor, CryptoStreamMode.Write))
          {
            using (StreamWriter streamWriter = new StreamWriter((Stream) cryptoStream))
              streamWriter.Write(plainText, (object) 0, (object) plainText.Length);
            return memoryStream.ToArray();
          }
        }
      }
    }

    public static string DecryptStringFromBytes(byte[] cipherText, byte[] Key)
    {
      if (cipherText == null || cipherText.Length == 0)
        throw new ArgumentNullException(nameof (cipherText));
      if (Key == null || Key.Length == 0)
        throw new ArgumentNullException(nameof (Key));
      byte[] numArray = new byte[16];
      using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
      {
        rijndaelManaged.Mode = CipherMode.CBC;
        rijndaelManaged.Padding = PaddingMode.PKCS7;
        rijndaelManaged.KeySize = 128;
        rijndaelManaged.BlockSize = 128;
        rijndaelManaged.Key = Key;
        rijndaelManaged.IV = numArray;
        ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
        using (MemoryStream memoryStream = new MemoryStream(cipherText))
        {
          using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, decryptor, CryptoStreamMode.Read))
          {
            using (StreamReader streamReader = new StreamReader((Stream) cryptoStream))
              return streamReader.ReadToEnd();
          }
        }
      }
    }
  }
}
