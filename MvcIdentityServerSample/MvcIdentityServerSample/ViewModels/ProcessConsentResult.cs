using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcIdentityServerSample.ViewModels
{
    public class ProcessConsentResult
    {
        public string RedirectUrl { get; set; }

        public bool IsRedirectUrl => RedirectUrl != null;

        public ConsentViewModel ViewModel { get; internal set; }
        public string ValidationError { get; internal set; }
    }
}
