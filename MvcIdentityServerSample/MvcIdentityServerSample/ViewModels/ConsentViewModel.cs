using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcIdentityServerSample.ViewModels
{
    public class ConsentViewModel : InputConsentViewModel
    {
        public string ClientName { get; set; }
        public string ClientLogoUri { get; set; }
        public string ClienUri { get; set; }
        public bool AllowRememberConsent { get; set; }
        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
        public IEnumerable<ScopeViewModel> ResourcesScopes { get; set; }
    }
}
