
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;


#nullable enable
namespace Schurko.Foundation.Crypto
{
  public class CertificateManager
  {
    public static X509Certificate2 FindCertificateBySubjectName(
      string subjectName,
      StoreName? storeName = null,
      StoreLocation storeLocation = StoreLocation.LocalMachine)
    {
      if (string.IsNullOrEmpty(subjectName))
        return (X509Certificate2) null;
      X509Store x509Store = (X509Store) null;
      try
      {
        if (storeName == null) storeName = (StoreName)5;
        x509Store = new X509Store((StoreName) ((int)(storeName)), storeLocation);
        x509Store.Open(OpenFlags.OpenExistingOnly);
        X509Certificate2Collection certificate2Collection = x509Store.Certificates.Find(X509FindType.FindBySubjectName, (object) subjectName, false);
        return certificate2Collection.Count != 0 ? certificate2Collection[0] : throw new ApplicationException("EncryptionProvider could not locate certificate with subject name : " + subjectName);
      }
      catch (Exception ex)
      {
        Trace.WriteLine("Exception trying to load B2BGatewayCertificate: ", ex.Message);
        return (X509Certificate2) null;
      }
      finally
      {
        x509Store?.Close();
      }
    }
  }
}
