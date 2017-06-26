using Folke.Elm;
using Folke.Identity.Server;

namespace Folke.Core
{
    public class FolkeCoreOptions
    {
        public string AdministratorEmail { get; set; }
        public string AdministratorPassword { get; set; }
        public IdentityServerOptions IdentityServerOptions { get; set; } = new IdentityServerOptions();
        public ElmOptions ElmOptions { get; set; } = new ElmOptions();
    }
}
