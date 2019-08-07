namespace Folke.Core
{
    public class ServiceOverrideOptions
    {
        public bool OverrideElm { get; set; } = false;
        public bool OverrideIdentity { get; set; } = false;
        public bool OverrideAuthorizations { get; set; } = false;
        public bool OverrideElmIdentity { get; set; } = false;
        public bool OverrideIdentityServer { get; set; } = false;
        public bool OverrideRoleIdentityServer { get; set; } = false;
    }
}
