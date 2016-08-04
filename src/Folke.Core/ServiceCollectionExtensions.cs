using Folke.Core.Entities;
using Folke.Core.Services;
using Folke.Core.ViewModels;
using Folke.Elm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Folke.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFolkeCore<TDataBaseDriver>(this IServiceCollection serviceCollection, Action<ElmOptions> elmSetupOptions)
            where TDataBaseDriver: class, IDatabaseDriver
        {
            return serviceCollection.AddFolkeCore<TDataBaseDriver>(elmSetupOptions, options => { });
        }
        
        public static IServiceCollection AddFolkeCore<TDataBaseDriver>(this IServiceCollection serviceCollection, Action<ElmOptions> elmSetupOptions, Action<IMvcBuilder> mvcBuilderSetupAction)
             where TDataBaseDriver : class, IDatabaseDriver
        {
            return serviceCollection.AddFolkeCore<TDataBaseDriver>(elmSetupOptions, mvcBuilderSetupAction, options =>
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

        public static IServiceCollection AddFolkeCore<TDataBaseDriver>(
            this IServiceCollection serviceCollection,
            Action<ElmOptions> elmSetupOptions,
            Action<IMvcBuilder> mvcBuilderSetupAction,
            Action<IdentityOptions> identitySetupOptions,
            Action<AuthorizationOptions> authorizationOptions)
                where TDataBaseDriver : class, IDatabaseDriver
        {
            serviceCollection.AddElm<TDataBaseDriver>(elmSetupOptions);

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
