 
using System;


#nullable enable
namespace Schurko.Foundation.Identity.Impersonation
{
    public class SecurityImpersonation : IDisposable
    {
        private ICredentialProvider provider;

        public SecurityImpersonation(ICredentialProvider provider) => this.provider = provider;

        public void Dispose() => provider = null;

        public SecurityImpersonation GetImpersonation() => provider == null ? null : new SecurityImpersonation(provider);
    }
}
