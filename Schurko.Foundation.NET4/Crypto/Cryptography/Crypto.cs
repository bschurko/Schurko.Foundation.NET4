

using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;


#nullable enable
namespace Schurko.Foundation.Crypto.Cryptography
{
    public static class Crypto
    {
        private const char ReplaceEqualsChar = '-';
        private const char ReplaceSlashChar = '_';
        private const char ReplacePlusChar = ',';
        private const char Ver = '1';
        private const char Delim = '\u0001';
        private static readonly byte[][] _mKeys = new byte[12][]
        {
      new byte[16]
      {
         122,
         202,
         62,
         121,
         227,
         42,
         83,
         141,
         244,
         76,
         197,
         225,
         54,
         142,
         12,
         128
      },
      new byte[16]
      {
         139,
         198,
         138,
         22,
         124,
         195,
         19,
         79,
         188,
         226,
         77,
         207,
         163,
         12,
         93,
         52
      },
      new byte[16]
      {
         113,
         75,
         163,
         134,
         172,
         231,
         253,
         163,
         112,
         158,
         110,
         182,
         61,
         233,
         238,
         244
      },
      new byte[16]
      {
         206,
         138,
         90,
         117,
         44,
         32,
         133,
         3,
         244,
         219,
         197,
         233,
         192,
         147,
         228,
         18
      },
      new byte[16]
      {
         208,
         155,
         91,
         204,
         244,
         115,
         234,
         142,
         98,
         155,
         44,
         130,
         141,
         118,
         202,
         242
      },
      new byte[16]
      {
         133,
         156,
         186,
         96,
         201,
         90,
         203,
         98,
         199,
         69,
         13,
         103,
         237,
         22,
         144,
         139
      },
      new byte[16]
      {
         182,
         146,
         26,
         172,
         184,
         254,
         193,
         131,
         56,
         180,
         191,
         57,
         168,
         137,
         150,
         199
      },
      new byte[16]
      {
         216,
         141,
         199,
         250,
         4,
         232,
         142,
         75,
         178,
         19,
         62,
         54,
         167,
         62,
         200,
         72
      },
      new byte[16]
      {
         190,
         240,
         123,
         112,
         217,
         99,
         35,
         174,
         14,
         26,
         58,
         48,
        byte.MaxValue,
         188,
         29,
         156
      },
      new byte[16]
      {
         122,
         154,
         242,
         37,
         163,
         201,
         169,
         125,
         79,
         227,
         97,
         86,
         231,
         215,
         226,
         66
      },
      new byte[16]
      {
         20,
         68,
         178,
         58,
         177,
         193,
         195,
         163,
         5,
         227,
         166,
         107,
         211,
         25,
         17,
         27
      },
      new byte[16]
      {
         113,
         43,
         179,
         119,
         170,
         39,
         241,
         161,
         66,
         226,
         227,
         100,
         19,
         154,
         46,
         74
      }
        };
        private static readonly byte[][] _miVs = new byte[12][]
        {
      new byte[8]
      {
         167,
         192,
         233,
         109,
         144,
         31,
         63,
         100
      },
      new byte[8]
      {
         195,
         28,
         243,
         141,
         227,
         248,
         120,
         201
      },
      new byte[8]
      {
         50,
         213,
         147,
         6,
         183,
         220,
         138,
         111
      },
      new byte[8]
      {
         253,
         254,
         95,
         179,
         41,
         35,
         47,
         67
      },
      new byte[8]
      {
         224,
         33,
         4,
         245,
         220,
         92,
         182,
         116
      },
      new byte[8]
      {
         111,
         249,
         10,
         223,
         204,
        byte.MaxValue,
         141,
         206
      },
      new byte[8]
      {
         94,
         187,
         196,
         146,
         128,
         131,
         95,
         203
      },
      new byte[8]
      {
         217,
         24,
         107,
         147,
         140,
         126,
         185,
         25
      },
      new byte[8]
      {
         122,
         30,
         60,
         148,
         222,
         196,
         168,
         204
      },
      new byte[8]
      {
         231,
         221,
         204,
         13,
         135,
         62,
         91,
         14
      },
      new byte[8]
      {
         103,
         35,
         205,
         13,
         38,
         227,
         80,
         2
      },
      new byte[8]
      {
         37,
         17,
         58,
         218,
         69,
         169,
         192,
         30
      }
        };

        public static bool Encrypt(int intToEncrypt, int offset, out string strEncrypted) => Encrypt(intToEncrypt.ToString(CultureInfo.InvariantCulture), offset, out strEncrypted);

        public static bool Encrypt(long lngToEncrypt, int offset, out string strEncrypted) => Encrypt(lngToEncrypt.ToString(CultureInfo.InvariantCulture), offset, out strEncrypted);

        internal static bool Encrypt<T1, T2>(
          EncryptIndex encryptIndex,
          T1 key1,
          T2 key2,
          out string strEncrypted)
          where T1 : IConvertible
          where T2 : IConvertible
        {
            return Encrypt(string.Format(CultureInfo.InvariantCulture, "{0}${1}", new object[2]
            {
         key1,
         key2
            }), (int)encryptIndex, out strEncrypted);
        }

        internal static bool Encrypt<T1, T2, T3>(
          EncryptIndex encryptIndex,
          T1 key1,
          T2 key2,
          T3 key3,
          out string strEncrypted)
          where T1 : IConvertible
          where T2 : IConvertible
          where T3 : IConvertible
        {
            return Encrypt(string.Format(CultureInfo.InvariantCulture, "{0}${1}${2}", new object[3]
            {

         key1,
         key2,
         key3
            }), (int)encryptIndex, out strEncrypted);
        }

        internal static bool DefaultEncrypt(
          string strToEncrypt,
          int keyOffset,
          out string strEncrypted)
        {
            int length1 = _mKeys.GetLength(0);
            int length2 = _miVs.GetLength(0);
            strEncrypted = null;
            try
            {
                ICryptoTransform encryptor = new RC2CryptoServiceProvider().CreateEncryptor(_mKeys[keyOffset % length1], _miVs[keyOffset % length2]);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                byte[] bytes = Encoding.UTF8.GetBytes(strToEncrypt);
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] array = memoryStream.ToArray();
                strEncrypted = Convert.ToBase64String(array);
                strEncrypted = strEncrypted.Replace('=', '-');
                strEncrypted = strEncrypted.Replace('/', '_');
                strEncrypted = strEncrypted.Replace('+', ',');
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool Decrypt(string strToDecrypt, int keyOffset, out int intDecrypted)
        {
            intDecrypted = 0;
            string strEncrypted;
            return !Decrypt(strToDecrypt, keyOffset, out strEncrypted) || int.TryParse(strEncrypted, out intDecrypted);
        }

        public static bool Decrypt(string strToDecrypt, int keyOffset, out long lngDecrypted)
        {
            lngDecrypted = 0L;
            string strEncrypted;
            return !Decrypt(strToDecrypt, keyOffset, out strEncrypted) || long.TryParse(strEncrypted, out lngDecrypted);
        }

        internal static bool Decrypt<T1, T2>(
          EncryptIndex encryptIndex,
          string strToDecrypt,
          out T1 key1,
          out T2 key2)
          where T1 : IConvertible
          where T2 : IConvertible
        {
            bool flag = false;
            key1 = default;
            key2 = default;
            try
            {
                string strEncrypted;
                if (!Decrypt(strToDecrypt, (int)encryptIndex, out strEncrypted))
                    return flag;
                string[] strArray = strEncrypted.Split('$');
                if (strArray.Length != 2)
                    return flag;
                key1 = (T1)Convert.ChangeType(strArray[0], typeof(T1));
                key2 = (T2)Convert.ChangeType(strArray[1], typeof(T2));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        internal static bool Decrypt<T1, T2, T3>(
          EncryptIndex encryptIndex,
          string strToDecrypt,
          out T1 key1,
          out T2 key2,
          out T3 key3)
          where T1 : IConvertible
          where T2 : IConvertible
          where T3 : IConvertible
        {
            bool flag = false;
            key1 = default;
            key2 = default;
            key3 = default;
            try
            {
                string strEncrypted;
                if (!Decrypt(strToDecrypt, (int)encryptIndex, out strEncrypted))
                    return flag;
                string[] strArray = strEncrypted.Split('$');
                if (strArray.Length != 3)
                    return flag;
                key1 = (T1)Convert.ChangeType(strArray[0], typeof(T1));
                key2 = (T2)Convert.ChangeType(strArray[1], typeof(T2));
                key3 = (T3)Convert.ChangeType(strArray[2], typeof(T3));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DefaultDecrypt(string strToDecrypt, int keyOffset, out string strDecrypted)
        {
            int length1 = _mKeys.GetLength(0);
            int length2 = _miVs.GetLength(0);
            strDecrypted = null;
            try
            {
                strToDecrypt = strToDecrypt.Replace('-', '=');
                strToDecrypt = strToDecrypt.Replace('_', '/');
                strToDecrypt = strToDecrypt.Replace(',', '+');
                ICryptoTransform decryptor = new RC2CryptoServiceProvider().CreateDecryptor(_mKeys[keyOffset % length1], _miVs[keyOffset % length2]);
                byte[] buffer1 = Convert.FromBase64String(strToDecrypt);
                CryptoStream cryptoStream = new CryptoStream(new MemoryStream(buffer1), decryptor, CryptoStreamMode.Read);
                byte[] bytes = new byte[buffer1.Length];
                byte[] buffer2 = bytes;
                int length3 = bytes.Length;
                cryptoStream.Read(buffer2, 0, length3);
                strDecrypted = Encoding.UTF8.GetString(bytes);
                strDecrypted = strDecrypted.Substring(0, strDecrypted.IndexOf("\0", StringComparison.Ordinal));
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool EncryptUserID(int retailerID, int userID, out string encUserID)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
            interpolatedStringHandler.AppendFormatted(retailerID);
            interpolatedStringHandler.AppendLiteral("$");
            interpolatedStringHandler.AppendFormatted(userID);
            return Encrypt(interpolatedStringHandler.ToStringAndClear(), 1, out encUserID);
        }

        public static bool DecryptUserID(string encUserID, out int retailerID, out int userID)
        {
            retailerID = 0;
            userID = 0;
            try
            {
                string strEncrypted;
                if (!Decrypt(encUserID, 1, out strEncrypted))
                    return false;
                retailerID = Convert.ToInt32(strEncrypted.Split('$')[0]);
                userID = Convert.ToInt32(strEncrypted.Split('$')[1]);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool EncryptAlbumID(int retailerID, int albumID, out string encAlbumID)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
            interpolatedStringHandler.AppendFormatted(retailerID);
            interpolatedStringHandler.AppendLiteral("$");
            interpolatedStringHandler.AppendFormatted(albumID);
            return Encrypt(interpolatedStringHandler.ToStringAndClear(), 3, out encAlbumID);
        }

        public static bool DecryptAlbumID(string encAlbumID, out int retailerID, out int albumID)
        {
            retailerID = 0;
            albumID = 0;
            try
            {
                string strEncrypted;
                if (!Decrypt(encAlbumID, 3, out strEncrypted))
                    return false;
                retailerID = Convert.ToInt32(strEncrypted.Split('$')[0]);
                albumID = Convert.ToInt32(strEncrypted.Split('$')[1]);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool EncryptMediaID(
          int retailerID,
          int userID,
          int mediaID,
          out string encMediaID)
        {
            return Encrypt(EncryptIndex.MediaID, retailerID, userID, mediaID, out encMediaID);
        }

        public static bool EncryptMediaID(
          int retailerID,
          int userID,
          long mediaID,
          out string encMediaID)
        {
            return Encrypt(EncryptIndex.MediaID, retailerID, userID, mediaID, out encMediaID);
        }

        public static bool DecryptMediaID(
          string encMediaID,
          out int retailerID,
          out int userID,
          out int mediaID)
        {
            return Decrypt(EncryptIndex.MediaID, encMediaID, out retailerID, out userID, out mediaID);
        }

        public static bool DecryptMediaID(
          string encMediaID,
          out int retailerID,
          out int userID,
          out long mediaID)
        {
            return Decrypt(EncryptIndex.MediaID, encMediaID, out retailerID, out userID, out mediaID);
        }

        public static bool EncryptMediaFileID(
          int retailerID,
          int userID,
          int mediaFileID,
          out string encMediaFileID)
        {
            return Encrypt(EncryptIndex.DictRC2Mac, retailerID, userID, mediaFileID, out encMediaFileID);
        }

        public static bool EncryptMediaFileID(
          int retailerID,
          int userID,
          long mediaFileID,
          out string encMediaFileID)
        {
            return Encrypt(EncryptIndex.DictRC2Mac, retailerID, userID, mediaFileID, out encMediaFileID);
        }

        public static bool DecryptMediaFileID(
          string encMediaFileID,
          out int retailerID,
          out int userID,
          out int mediaFileID)
        {
            return Decrypt(EncryptIndex.DictRC2Mac, encMediaFileID, out retailerID, out userID, out mediaFileID);
        }

        public static bool DecryptMediaFileID(
          string encMediaFileID,
          out int retailerID,
          out int userID,
          out long mediaFileID)
        {
            return Decrypt(EncryptIndex.DictRC2Mac, encMediaFileID, out retailerID, out userID, out mediaFileID);
        }

        public static bool EncryptAppContextID(int appContextID, out string encAppContextID) => Encrypt(appContextID, 4, out encAppContextID);

        public static bool DecryptAppContextID(string encAppContextID, out int appContextID)
        {
            appContextID = 0;
            try
            {
                return Decrypt(encAppContextID, 4, out appContextID);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool EncryptShareID(int retailerID, int shareID, out string encShareID)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
            interpolatedStringHandler.AppendFormatted(retailerID);
            interpolatedStringHandler.AppendLiteral("$");
            interpolatedStringHandler.AppendFormatted(shareID);
            return Encrypt(interpolatedStringHandler.ToStringAndClear(), 8, out encShareID);
        }

        public static bool DecryptShareID(string encShareID, out int retailerID, out int shareID)
        {
            retailerID = 0;
            shareID = 0;
            try
            {
                string strEncrypted;
                if (!Decrypt(encShareID, 8, out strEncrypted))
                    return false;
                retailerID = Convert.ToInt32(strEncrypted.Split('$')[0]);
                shareID = Convert.ToInt32(strEncrypted.Split('$')[1]);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool EncryptProjectID(int retailerID, int projectID, out string encProjectID)
        {
            encProjectID = null;
            try
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
                interpolatedStringHandler.AppendFormatted(retailerID);
                interpolatedStringHandler.AppendLiteral("$");
                interpolatedStringHandler.AppendFormatted(projectID);
                return Encrypt(interpolatedStringHandler.ToStringAndClear(), 10, out encProjectID);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DecryptProjectID(string encProjectID, out int retailerID, out int projectID)
        {
            retailerID = 0;
            projectID = 0;
            try
            {
                string strEncrypted;
                if (!Decrypt(encProjectID, 10, out strEncrypted))
                    return false;
                retailerID = Convert.ToInt32(strEncrypted.Split('$')[0]);
                projectID = Convert.ToInt32(strEncrypted.Split('$')[1]);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool EncryptStoreID(int retailerID, int storeID, out string encStoreID)
        {
            encStoreID = null;
            try
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
                interpolatedStringHandler.AppendFormatted(retailerID);
                interpolatedStringHandler.AppendLiteral("$");
                interpolatedStringHandler.AppendFormatted(storeID);
                return Encrypt(interpolatedStringHandler.ToStringAndClear(), 9, out encStoreID);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DecryptStoreID(string encStoreID, out int retailerID, out int storeID)
        {
            retailerID = 0;
            storeID = 0;
            try
            {
                string strEncrypted;
                if (!Decrypt(encStoreID, 9, out strEncrypted))
                    return false;
                retailerID = Convert.ToInt32(strEncrypted.Split('$')[0]);
                storeID = Convert.ToInt32(strEncrypted.Split('$')[1]);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool EncryptCreditCardNum(string creditCardNum, out string encCreditCardNum)
        {
            encCreditCardNum = null;
            try
            {
                return Encrypt(creditCardNum, 5, out encCreditCardNum);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DecryptCreditCardNum(string encCreditCardNum, out string creditCardNum)
        {
            creditCardNum = null;
            try
            {
                return Decrypt(encCreditCardNum, 5, out creditCardNum);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool EncryptOrderPullerSessionID(int labID, out string sessionID)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
            interpolatedStringHandler.AppendFormatted(labID);
            interpolatedStringHandler.AppendLiteral("$");
            interpolatedStringHandler.AppendFormatted(DateTime.Now);
            return Encrypt(interpolatedStringHandler.ToStringAndClear(), 3, out sessionID);
        }

        public static bool EncryptOrderPullerSessionID(int labID, int version, out string sessionID)
        {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
            interpolatedStringHandler.AppendFormatted(labID);
            interpolatedStringHandler.AppendLiteral("$");
            interpolatedStringHandler.AppendFormatted(version);
            interpolatedStringHandler.AppendLiteral("$");
            interpolatedStringHandler.AppendFormatted(DateTime.Now);
            return Encrypt(interpolatedStringHandler.ToStringAndClear(), 3, out sessionID);
        }

        public static bool DecryptOrderPullerSessionID(
          string sessionID,
          out int labID,
          out int? version,
          out DateTime sessionStart)
        {
            labID = 0;
            version = new int?();
            sessionStart = DateTime.MinValue;
            try
            {
                string strEncrypted;
                if (!Decrypt(sessionID, 3, out strEncrypted))
                    return false;
                string[] strArray = strEncrypted.Split('$');
                if (strArray.Length == 2)
                {
                    labID = Convert.ToInt32(strArray[0]);
                    sessionStart = DateTime.Parse(strArray[1]);
                }
                else
                {
                    labID = Convert.ToInt32(strArray[0]);
                    version = new int?(Convert.ToInt32(strArray[1]));
                    sessionStart = DateTime.Parse(strArray[2]);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool EncryptOrderItemID(
          int retailerID,
          int orderItemID,
          out string encOrderItemID)
        {
            return Encrypt(EncryptIndex.OrderItemID, retailerID, orderItemID, out encOrderItemID);
        }

        public static bool EncryptOrderItemID(
          int retailerID,
          long orderItemID,
          out string encOrderItemID)
        {
            return Encrypt(EncryptIndex.OrderItemID, retailerID, orderItemID, out encOrderItemID);
        }

        public static bool DecryptOrderItemID(
          string encOrderItemID,
          out int retailerID,
          out int orderItemID)
        {
            return Decrypt(EncryptIndex.OrderItemID, encOrderItemID, out retailerID, out orderItemID);
        }

        public static bool DecryptOrderItemID(
          string encOrderItemID,
          out int retailerID,
          out long orderItemID)
        {
            return Decrypt(EncryptIndex.OrderItemID, encOrderItemID, out retailerID, out orderItemID);
        }

        public static string EncryptDictionary(IDictionary dict) => EncryptDictionary(dict, string.Empty);

        public static string EncryptDictionary(IDictionary dict, string identifier)
        {
            Type type = typeof(double);
            StringBuilder stringBuilder = new StringBuilder();
            DefaultInterpolatedStringHandler interpolatedStringHandler;
            foreach (DictionaryEntry dictionaryEntry in dict)
            {
                if (dictionaryEntry.Value == null)
                {
                    interpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 2);
                    interpolatedStringHandler.AppendLiteral("Null key or value: ");
                    interpolatedStringHandler.AppendFormatted(dictionaryEntry.Key);
                    interpolatedStringHandler.AppendLiteral(",");
                    interpolatedStringHandler.AppendFormatted<object>(dictionaryEntry.Value);
                    throw new NoNullAllowedException(interpolatedStringHandler.ToStringAndClear());
                }
                string str1 = dictionaryEntry.Key.ToString();
                string str2 = dictionaryEntry.Value.GetType() == type ? ((double)dictionaryEntry.Value).ToString(NumberFormatInfo.InvariantInfo) : dictionaryEntry.Value.ToString();
                if (str1.IndexOf('\u0001') != -1)
                    throw new InvalidOperationException("Key contains delimeter character, which is not allowed");
                if (str2.IndexOf('\u0001') != -1)
                    throw new InvalidOperationException("Value contains delimeter character, which is not allowed");
                stringBuilder.Append('\u0001');
                stringBuilder.Append(str1);
                stringBuilder.Append('\u0001');
                stringBuilder.Append(str2);
            }
            string str = dict.Count == 0 ? string.Empty : stringBuilder.ToString().Substring(1);
            string strEncrypted;
            if (!Encrypt(ComputeMac(str + "\u0001" + identifier, 6) + str, 5, out strEncrypted))
            {
                interpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
                interpolatedStringHandler.AppendLiteral("Encrypting string: ");
                interpolatedStringHandler.AppendFormatted(stringBuilder);
                interpolatedStringHandler.AppendLiteral(" failed.");
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            return "1" + strEncrypted;
        }

        public static IDictionary DecryptDictionary(string ciphertext) => DecryptDictionary(ciphertext, string.Empty);

        public static IDictionary DecryptDictionary(string ciphertext, string identifier)
        {
            IDictionary dictionary = new Hashtable();
            if (ciphertext.Length > 0)
            {
                ciphertext = ciphertext[0] == '1' ? ciphertext.Substring(1) : throw new InvalidOperationException("Version mismatch decrypting");
                string strEncrypted;
                if (!Decrypt(ciphertext, 5, out strEncrypted))
                    throw new InvalidOperationException("Decrypting string: " + ciphertext + " failed.");
                string str = strEncrypted.Substring(0, 16);
                strEncrypted = strEncrypted.Substring(16);
                string mac = ComputeMac(strEncrypted + "\u0001" + identifier, 6);
                if (str != mac)
                    throw new InvalidOperationException("MAC checksum mismatch decrypting data. Identifier is wrong, or encrypted data has been altered.");
                string[] strArray = strEncrypted.Split('\u0001');
                for (int index = 0; index < strArray.Length; index += 2)
                    dictionary[strArray[index]] = strArray[index + 1];
            }
            return dictionary;
        }

        public static string HashString(string input)
        {
            byte[] bytes = new UnicodeEncoding().GetBytes(input);
            return Convert.ToBase64String(((HashAlgorithm)CryptoConfig.CreateFromName("SHA1")).ComputeHash(bytes));
        }

        private static string ComputeMac(string str, int keyOffset)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(new HMACSHA1(_mKeys[keyOffset]).ComputeHash(bytes)).Substring(0, 16);
        }

        public static string EncryptString(string val) => EncryptString(val, string.Empty);

        public static string EncryptString(string val, string identifier)
        {
            Hashtable dict = new Hashtable();
            dict[string.Empty] = val;
            return EncryptDictionary(dict, identifier);
        }

        public static string EncryptInt(int val) => EncryptString(val.ToString(CultureInfo.InvariantCulture), string.Empty);

        public static string EncryptInt(int val, string identifier) => EncryptString(val.ToString(CultureInfo.InvariantCulture), identifier);

        public static string EncryptLong(int val) => EncryptString(val.ToString(CultureInfo.InvariantCulture), string.Empty);

        public static string EncryptLong(int val, string identifier) => EncryptString(val.ToString(CultureInfo.InvariantCulture), identifier);

        public static string DecryptString(string encryptedStr) => DecryptString(encryptedStr, string.Empty);

        public static string DecryptString(string encryptedStr, string identifier) => (string)DecryptDictionary(encryptedStr, identifier)[string.Empty];

        public static int DecryptInt(string encryptedStr) => int.Parse(DecryptString(encryptedStr, string.Empty));

        public static int DecryptInt(string encryptedStr, string identifier) => int.Parse(DecryptString(encryptedStr, identifier));

        public static int DecryptLong(string encryptedStr) => int.Parse(DecryptString(encryptedStr, string.Empty));

        public static int DecryptLong(string encryptedStr, string identifier) => int.Parse(DecryptString(encryptedStr, identifier));

        public static RsaCryptoProvider GetRsaCryptoProvider(
          string subjectName,
          StoreName storeName = StoreName.My,
          StoreLocation storeLocation = StoreLocation.LocalMachine)
        {
            X509Certificate2 certificateBySubjectName = CertificateManager.FindCertificateBySubjectName(subjectName, new StoreName?(storeName), storeLocation);
            if (certificateBySubjectName == null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 3);
                interpolatedStringHandler.AppendLiteral("Certificate ");
                interpolatedStringHandler.AppendFormatted(subjectName);
                interpolatedStringHandler.AppendLiteral(" not found in ");
                interpolatedStringHandler.AppendFormatted(storeName);
                interpolatedStringHandler.AppendLiteral(" on ");
                interpolatedStringHandler.AppendFormatted(storeLocation);
                throw new CryptoException(interpolatedStringHandler.ToStringAndClear());
            }
            return new RsaCryptoProvider(certificateBySubjectName);
        }

        public static bool Encrypt(string strToEncrypt, int keyOffset, out string strEncrypted)
        {
            bool flag = false;
            strEncrypted = "";
            try
            {
                if (EncryptionProvider.EncryptionCertificate != null)
                {
                    byte[] inArray = EncryptionProvider.Encrypt(strToEncrypt);
                    strEncrypted = Convert.ToBase64String(inArray);
                    flag = !string.IsNullOrEmpty(strEncrypted);
                }
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag || DefaultEncrypt(strToEncrypt, keyOffset, out strEncrypted);
        }

        public static bool Decrypt(string strToDecrypt, int keyOffset, out string strEncrypted)
        {
            bool flag = false;
            strEncrypted = "";
            try
            {
                if (EncryptionProvider.EncryptionCertificate != null)
                {
                    byte[] bytes = EncryptionProvider.Decrypt(strToDecrypt);
                    strEncrypted = Encoding.UTF8.GetString(bytes);
                    flag = !string.IsNullOrEmpty(strEncrypted);
                }
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag || DefaultDecrypt(strToDecrypt, keyOffset, out strEncrypted);
        }
    }
}
