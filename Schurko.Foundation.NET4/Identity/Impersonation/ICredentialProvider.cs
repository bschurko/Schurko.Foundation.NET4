 
#nullable enable
using Schurko;

namespace Schurko.Foundation.Identity.Impersonation
{
    public interface ICredentialProvider
    {
        string Domain { get; }

        string User { get; }

        string Password { get; }
    }

    public class CredentialProvider : ICredentialProvider
    {
        public CredentialProvider() { }

        public CredentialProvider(string domain, string user, string password)
        {
            Domain = domain;
            User = user;
            Password = password;
        }

        public string Domain { get; set; }

        public string User   { get; set; }

        public string Password   { get; set; }
    }
}
