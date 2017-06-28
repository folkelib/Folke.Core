using Folke.Core.Entities;
using Folke.Core.Services;
using Folke.Core.ViewModels;
using Folke.Elm;
using Microsoft.Extensions.DependencyInjection;
using System;

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
            
            serviceCollection.AddElm<TDataBaseDriver>(coreServiceOptions.Elm);

            serviceCollection.AddIdentity<User, Role>(coreServiceOptions.Identity).AddDefaultTokenProviders();
            var mvcBuilder = serviceCollection.AddMvc().AddFolkeCore();
            coreServiceOptions.MvcBuilder?.Invoke(mvcBuilder);

            serviceCollection.AddAuthorization(options =>
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

            serviceCollection.AddElmIdentity<User, Role, int>();
            serviceCollection.AddIdentityServer<User, int, EmailService, UserService, UserViewModel>();
            serviceCollection.AddRoleIdentityServer<Role, RoleService, RoleViewModel>();

            return serviceCollection;
        }
    }
}
