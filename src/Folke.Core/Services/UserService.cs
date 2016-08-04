using Folke.Core.Entities;
using Folke.Core.ViewModels;
using Folke.Elm;
using Folke.Elm.Fluent;
using Folke.Identity.Server.Services;
using Folke.Identity.Server.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Folke.Core.Services
{
    public class UserService : BaseUserService<User, UserViewModel>
    {
        private readonly IFolkeConnection connection;

        public UserService(IFolkeConnection connection, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager) : base(httpContextAccessor, userManager)
        {
            this.connection = connection;
        }

        public override Task<IList<User>> Search(UserSearchFilter name, int offset, int limit, string sortColumn)
        {
            return connection.SelectAllFrom<User>().OrderBy(x => x.UserName).Limit(offset, limit).ToListAsync();
        }

        public override UserViewModel MapToUserView(User user)
        {
            if (user == null)
            {
                return new UserViewModel
                {
                    Logged = false
                };
            }

            return new UserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                HasPassword = user.PasswordHash != null,
                Id = user.Id,
                Logged = true
            };
        }

        public async Task<bool> IsUser(User loggedUser, User accessedUser)
        {
            return loggedUser.Id == accessedUser.Id || await UserManager.IsInRoleAsync(loggedUser, RoleNames.Administrator);
        }

        public Task<bool> HasRole(User user, string role)
        {
            return UserManager.IsInRoleAsync(user, role);
        }

        public async Task<IEnumerable<User>> GetUsersAsync(IEnumerable<UserViewModel> userViews)
        {
            var keys = userViews.Select(x => x.Id);
            return await connection.SelectAllFrom<User>().Where(x => keys.Contains(x.Id)).ToListAsync();
        }

        public override User CreateNewUser(string userName, string email, bool emailConfirmed)
        {
            return new User
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = emailConfirmed
            };
        }
    }
}
