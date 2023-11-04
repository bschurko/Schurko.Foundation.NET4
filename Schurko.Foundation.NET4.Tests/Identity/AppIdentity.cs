using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Schurko.Foundation.Identity.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Tests.Identity
{

    public class UserManagerAppUser : Microsoft.AspNetCore.Identity.UserManager<AppUser>
    {
        public UserManagerAppUser(Microsoft.AspNetCore.Identity.IUserStore<AppUser> store,
            IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<AppUser> passwordHasher,
            IEnumerable<IUserValidator<AppUser>> userValidators,
            IEnumerable<IPasswordValidator<AppUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services,
            ILogger<Microsoft.AspNetCore.Identity.UserManager<AppUser>> logger)
                : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer,
                      errors, services, logger)
        {
        }
    }
}
