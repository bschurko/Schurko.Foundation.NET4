using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Identity.Auth
{
    public class AppUserBase : IdentityUser, IUser
    {
        public AppUserBase(ClaimsPrincipal user)
        {
            User = user;
        }

        public AppUserBase() { }

        /// <summary>
        /// The underlying user
        /// </summary>
        public ClaimsPrincipal User { get; set; }


        /// <summary>
        /// Optionally set Http Context which is needed for Cookie access
        /// to properties
        /// </summary>
        public HttpContext HttpContext { get; set; }


        /// <summary>
        /// The list of claims for this user
        /// </summary>
        public IEnumerable<Claim> Claims => User?.Claims;

        #region Claims Validation

        /// <summary>
        /// Checks if the user is authenticated
        /// </summary>
        /// <returns></returns>
        public virtual bool IsAuthenticated()
        {
            if (User == null || User.Identity == null)
                return false;

            return User.Identity.IsAuthenticated;
        }

        /// <summary>
        /// Checks to see if the user identity is set
        /// </summary>
        /// <returns></returns>
        public virtual bool IsEmpty()
        {
            return User?.Identity == null;
        }

        /// <summary>
        /// Checks if a role exists
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool HasRole(string role)
        {
            return User.IsInRole(role);
        }
        #endregion

        #region Claims and Roles

        /// <summary>
        /// Helper to retrieve a claim by name on the current logged in user
        /// </summary>
        /// <param name="claimName"></param>
        /// <returns></returns>
        public string GetClaim(string claimName)
        {
            return User.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;
        }

        /// <summary>
        /// Add a claim to the user (use this if multiple claims for the same setting can be set)
        /// </summary>
        /// <param name="claimName"></param>
        /// <param name="value"></param>
        public void AddClaim(string claimName, string value)
        {
            ((ClaimsIdentity)User.Identity).AddClaim(new Claim(claimName, value));
        }

        /// <summary>
        /// Adds or updates a claim for the user. Use this for unique claims
        /// </summary>
        /// <param name="claimName"></param>
        /// <param name="value"></param>
        public void AddOrUpdateClaim(string claimName, string value)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == claimName);

            if (claim != null && claim.Value != value)
                ((ClaimsIdentity)User.Identity).RemoveClaim(claim);

            ((ClaimsIdentity)User.Identity).AddClaim(new Claim(claimName, value));
        }

        /// <summary>
        /// Removes a claim from the claims collection
        /// </summary>
        /// <param name="claimName">Exact match of the claim's name</param>
        public void RemoveClaim(string claimName)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Value == claimName);
            if (claim != null)
            {
                var ci = User.Identity as ClaimsIdentity;
                ci.RemoveClaim(claim);
            }
        }

        public void AddRole(string userRole)
        {
            ((ClaimsIdentity)User.Identity).AddClaim(new Claim(ClaimTypes.Role, userRole));
        }

        #endregion

        #region Login Logout

        /// <summary>
        /// Logins a user with Cookie Authentication
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task LoginUserAsync(HttpContext context, ClaimsIdentity identity = null)
        {
            if (identity == null)
            {
                identity = User.Identity as ClaimsIdentity;
                if (identity == null)
                    throw new InvalidOperationException("User is not authenticated.");
            }

            // Set cookie and attach claims
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(2)
                });
        }

        /// <summary>
        /// Logs in a user with Cookie Authentication
        /// </summary>
        /// <param name="context"></param>
        public void LoginUser(HttpContext context, ClaimsIdentity identity = null)
        {
            if (identity == null)
            {
                identity = User.Identity as ClaimsIdentity;
                if (identity == null)
                    throw new InvalidOperationException("User is not authenticated.");
            }

            // Set cookie and attach claims
            context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(2)
                });
        }

        /// <summary>
        /// Logs out a user with Cookie Authentication
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task LogoutUserAsync(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        /// <summary>
        /// Logs out a user with Cookie Authentication
        /// </summary>
        /// <param name="context"></param>

        public void LogoutUser(HttpContext context)
        {
            context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        #endregion

    }
}
