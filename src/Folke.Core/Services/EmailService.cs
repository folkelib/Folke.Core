using Folke.Core.Entities;
using Folke.Identity.Server.Services;
using System.Threading.Tasks;

namespace Folke.Core.Services
{
    public class EmailService : IUserEmailService<User>
    {
        public Task SendEmailConfirmationEmail(User user, string code)
        {
            return Task.FromResult(0);
        }

        public Task SendPasswordResetEmail(User user, string code)
        {
            return Task.FromResult(0);
        }
    }
}
