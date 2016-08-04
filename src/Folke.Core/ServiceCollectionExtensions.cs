using Folke.Core.Entities;
using Folke.Core.Services;
using Folke.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Folke.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFolkeCore(this IServiceCollection serviceCollection, Action<IMvcBuilder> mvcBuilderSetupAction)
        {
            return serviceCollection.AddFolkeCore(mvcBuilderSetupAction, options =>
            {
                options.Password = new PasswordOptions
                {
                    RequireDigit = false,
                    RequiredLength = 6,
                    RequireLowercase = false,
                    RequireNonAlphanumeric = false,
                    RequireUppercase = false
                };
            }, options => { });
        }

        public static IServiceCollection AddFolkeCore(
            this IServiceCollection serviceCollection,
            Action<IMvcBuilder> mvcBuilderSetupAction,
            Action<IdentityOptions> identitySetupOptions,
            Action<AuthorizationOptions> authorizationOptions)
        {
            serviceCollection.AddIdentity<User, Role>(identitySetupOptions).AddDefaultTokenProviders();
            var mvcBuilder = serviceCollection.AddMvc().AddFolkeCore();
            mvcBuilderSetupAction(mvcBuilder);

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
                authorizationOptions(options);
            });

            serviceCollection.AddElmIdentity<User, Role, int>();
            serviceCollection.AddIdentityServer<User, int, EmailService, UserService, UserViewModel>();
            serviceCollection.AddRoleIdentityServer<Role, RoleService, RoleViewModel>();

            return serviceCollection;
        }
    }
}
