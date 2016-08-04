using Folke.Core.Entities;
using Folke.Core.Services;
using Folke.Core.ViewModels;
<<<<<<< b72e71c4ad5c0e5e3fde49446a667cef8feabd72
=======
using Folke.Elm;
>>>>>>> Initial import
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Folke.Core
{
    public static class ServiceCollectionExtensions
    {
<<<<<<< b72e71c4ad5c0e5e3fde49446a667cef8feabd72
        public static IServiceCollection AddFolkeCore(this IServiceCollection serviceCollection, Action<IMvcBuilder> mvcBuilderSetupAction)
        {
            return serviceCollection.AddFolkeCore(mvcBuilderSetupAction, options =>
=======
        public static IServiceCollection AddFolkeCore<TDataBaseDriver>(this IServiceCollection serviceColletion, Action<ElmOptions> elmSetupAction)
                where TDataBaseDriver : class, IDatabaseDriver
        {
            return serviceColletion.AddFolkeCore<TDataBaseDriver>(elmSetupAction, builder => { });
        }

        public static IServiceCollection AddFolkeCore<TDataBaseDriver>(this IServiceCollection serviceCollection, Action<ElmOptions> elmSetupAction, Action<IMvcBuilder> mvcBuilderSetupAction)
                where TDataBaseDriver : class, IDatabaseDriver
        {
            return serviceCollection.AddFolkeCore<TDataBaseDriver>(elmSetupAction, mvcBuilderSetupAction, options =>
>>>>>>> Initial import
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

<<<<<<< b72e71c4ad5c0e5e3fde49446a667cef8feabd72
        public static IServiceCollection AddFolkeCore(
            this IServiceCollection serviceCollection, 
            Action<IMvcBuilder> mvcBuilderSetupAction,
            Action<IdentityOptions> identitySetupOptions,
            Action<AuthorizationOptions> authorizationOptions)
        {
            serviceCollection.AddIdentity<User, Role>(identitySetupOptions).AddDefaultTokenProviders();
=======
        public static IServiceCollection AddFolkeCore<TDataBaseDriver>(
            this IServiceCollection serviceCollection,
            Action<ElmOptions> elmSetupAction,
            Action<IMvcBuilder> mvcBuilderSetupAction,
            Action<IdentityOptions> identitySetupOptions,
            Action<AuthorizationOptions> authorizationOptions)
                where TDataBaseDriver : class, IDatabaseDriver
        {
           serviceCollection.AddIdentity<User, Role>(identitySetupOptions).AddDefaultTokenProviders();
>>>>>>> Initial import
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

<<<<<<< b72e71c4ad5c0e5e3fde49446a667cef8feabd72
=======
            serviceCollection.AddElm<TDataBaseDriver>(elmSetupAction);
>>>>>>> Initial import
            serviceCollection.AddElmIdentity<User, Role, int>();
            serviceCollection.AddIdentityServer<User, int, EmailService, UserService, UserViewModel>();
            serviceCollection.AddRoleIdentityServer<Role, RoleService, RoleViewModel>();

            return serviceCollection;
        }
    }
}
