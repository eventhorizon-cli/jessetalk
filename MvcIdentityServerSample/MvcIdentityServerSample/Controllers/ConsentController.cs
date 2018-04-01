using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using MvcIdentityServerSample.Services;
using MvcIdentityServerSample.ViewModels;

namespace MvcIdentityServerSample.Controllers
{
    public class ConsentController : Controller
    {
        private readonly ConsentService _consentService;

        public ConsentController(ConsentService consentService)
        {
            _consentService = consentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var model = await _consentService.BuildConsentViewModelAsync(returnUrl);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(InputConsentViewModel viewModel)
        {
            var result = await _consentService.ProcessGrantAsync(viewModel);

            if (result.IsRedirectUrl)
            {
                return Redirect(result.RedirectUrl);
            }

            if (!string.IsNullOrEmpty(result.ValidationError))
            {
                ModelState.AddModelError(string.Empty, result.ValidationError);
            }

            return View(result.ViewModel);
        }
    }
}