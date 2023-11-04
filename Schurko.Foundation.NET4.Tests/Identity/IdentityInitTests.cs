using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Schurko.Foundation.Data;
using Schurko.Foundation.Identity.Auth;
using Schurko.Foundation.Identity.Auth.Identity;
using Schurko.Foundation.IoC.DI;
using Schurko.Foundation.Tests.Identity;
using Schurko.Foundation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Tests.Util
{
    [TestClass]
    public class IdentityInitTests
    {
        [TestMethod]
        public void Identity_Init_Core_Tests()
        {

            var host = Foundation.IoC.DI.IoC.InitDependency((s) =>
            {
                var config = StaticConfigurationManager.GetConfiguration();
                // Add services to the container.
                var connectionString = config.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

                s.AddDbContext<IdentityDbContext>(options =>
                    options.UseSqlServer(connectionString));

 

                var dbContextOptions = new DbContextOptions<IdentityDbContext>();
                var identityDbContext = new IdentityDbContext(dbContextOptions);

                s.AddScoped<DbContext, IdentityDbContext>();

                IConnectionString cs = new ConnectionString("DefaultConnectionString", connectionString);
                s.AddSingleton<IConnectionString>(cs);

                s.AddIdentity<AppUser, ApplicationRole>();

                var identityBuilder = s.AddDefaultIdentity<AppUser>(options => { })
                    .AddEntityFrameworkStores<IdentityDbContext>()
                    .AddDefaultTokenProviders()
                    .AddUserStore<AppUser>()
                    .AddUserManager<UserManagerAppUser>();

                s.AddIdentityCore<AppUser>()
                    .AddRoles<ApplicationRole>()
                    .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<AppUser, ApplicationRole>>();
                

                s.AddScoped<Microsoft.AspNetCore.Identity.IUserStore<AppUser>, UserStore<AppUser>>();

                s.AddScoped<Microsoft.AspNetCore.Identity.UserManager<UserManagerAppUser>,
                    Microsoft.AspNetCore.Identity.UserManager<UserManagerAppUser>>();

                // identityDbContext
                Microsoft.AspNetCore.Identity.IUserStore<AppUser> userStore
                    = new Microsoft.AspNetCore.Identity.EntityFrameworkCore.UserStore<AppUser>(identityDbContext);

                Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager
                    = new Microsoft.AspNetCore.Identity.UserManager<AppUser>(userStore, null, new PasswordHasher<AppUser>(),
                    null, null, null, null, null, null);
            });

            var userStore = host.GetService<Microsoft.AspNetCore.Identity.IUserStore<AppUser>>();
            var userManager = host.GetService<Microsoft.AspNetCore.Identity.UserManager<UserManagerAppUser>>();

        }
    }
}
