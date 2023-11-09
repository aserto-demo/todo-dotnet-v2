using Aserto.AspNetCore.Middleware.Options;
using Aserto.Authorizer.V2.API;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Aserto.TodoApp.Mapping
{
    public sealed class AuthzIdentityContext
    {
        private static readonly AuthzIdentityContext instance = new AuthzIdentityContext();

        static AuthzIdentityContext()
        {

        }
        private AuthzIdentityContext()
        {

        }

        public static AuthzIdentityContext Instance
        {
            get
            {
                return instance;
            }
        }

        public Func<ClaimsPrincipal, IEnumerable<string>, IdentityContext> IdentityMapper { get; set; } = CustomIdentityContext;

        private static IdentityContext CustomIdentityContext(ClaimsPrincipal identity, IEnumerable<string> supportedClaimTypes)
        {
            var identityContext = new IdentityContext();
            identityContext.Type = IdentityType.None;

            if (identity.Identity.AuthenticationType != null && identity.Identity != null)
            {
                foreach (string supportedClaimType in supportedClaimTypes)
                {
                    var claim = identity.FindFirst(c => c.Type == supportedClaimType);
                    if (claim != null)
                    {                        
                        Console.WriteLine(claim.Value);                     
                        identityContext.Type = IdentityType.Sub;
                        identityContext.Identity = claim.Value;
                        break;
                    }
                }
            }

            return identityContext;
        }
    }
}
