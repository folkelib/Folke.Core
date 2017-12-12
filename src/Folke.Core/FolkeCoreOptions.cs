using System;
using Folke.Elm;
using Folke.Identity.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Folke.Core
{
    public class FolkeCoreOptions
    {
        public Action<AuthorizationOptions> Authorization { get; set; }
        public Action<ServiceOverrideOptions> IdentityServer { get; set; } = options => { };
        public Action<ElmOptions> Elm { get; set; } = options => { };
        public Action<IdentityOptions> Identity { get; set; } = options => options.Password = new PasswordOptions
        {
            RequireDigit = false,
            RequiredLength = 6,
            RequireLowercase = false,
            RequireNonAlphanumeric = false,
            RequireUppercase = false
        };
        public Action<IMvcBuilder> MvcBuilder { get; set; }
        
        public Action<ServiceOverrideOptions> Overrides { get; set; } = options => { };
    }
}
