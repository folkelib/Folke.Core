using Folke.Core.Entities;
using Folke.Core.ViewModels;
using Folke.Identity.Server.Services;

namespace Folke.Core.Services
{
    public class RoleService : IRoleService<Role, RoleViewModel>
    {
        public RoleViewModel MapToRoleView(Role role)
        {
            return new RoleViewModel
            {
                Name = role.Name,
                Id = role.Id
            };
        }

        public Role CreateNewRole(string name)
        {
            return new Role
            {
                Name = name
            };
        }
    }
}
