using Microsoft.Extensions.DependencyInjection;
using Folke.Identity.Server;
using Folke.Core.Entities;
using Folke.Core.ViewModels;

namespace Folke.Core
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddFolkeCore(this IMvcBuilder builder)
        {
            return builder.AddIdentityServer<int, User, UserViewModel, Role, RoleViewModel>();
        }
    }
}
