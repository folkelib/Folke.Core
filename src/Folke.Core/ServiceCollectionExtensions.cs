using Folke.Core.Entities;
using Folke.Core.Services;
using Folke.Core.ViewModels;
using Folke.Elm;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Identity;

namespace Folke.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFolkeCore<TDataBaseDriver>(
            this IServiceCollection serviceCollection,
            Action<FolkeCoreOptions> optionsAction)
                where TDataBaseDriver : class, IDatabaseDriver
        {
            var coreServiceOptions = new FolkeCoreOptions();
            optionsAction(coreServiceOptions);

            var overrides = new ServiceOverrideOptions();
            coreServiceOptions.Overrides(overrides);
            
            if (!overrides.OverrideElm) serviceCollection.AddElm<TDataBaseDriver>(coreServiceOptions.Elm);

            if (!overrides.OverrideIdentity) serviceCollection.AddIdentity<User, Role>(coreServiceOptions.Identity).AddDefaultTokenProviders();
            var mvcBuilder = serviceCollection.AddMvc().AddFolkeCore();
            coreServiceOptions.MvcBuilder?.Invoke(mvcBuilder);

            if (!overrides.OverrideAuthorizations) serviceCollection.AddAuthorization(options =>
            {
                options.AddPolicy("UserList", policy =>
                {
                    policy.RequireRole(RoleNames.Administrator);
                });
                options.AddPolicy("Role", policy =>
                {
                    policy.RequireRole(RoleNames.Administrator);
                });
                coreServiceOptions.Authorization?.Invoke(options);
            });

            if (!overrides.OverrideElmIdentity) serviceCollection.AddElmIdentity<User, Role, int>();
            if (!overrides.OverrideIdentityServer) serviceCollection.AddIdentityServer<User, int, EmailService, UserService, UserViewModel>();
            if (!overrides.OverrideRoleIdentityServer) serviceCollection.AddRoleIdentityServer<Role, RoleService, RoleViewModel>();

            return serviceCollection;
        }
    }
}
