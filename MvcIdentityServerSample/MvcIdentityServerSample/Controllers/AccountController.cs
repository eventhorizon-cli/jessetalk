using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcIdentityServerSample.Models;
using MvcIdentityServerSample.ViewModels;
using Microsoft.AspNetCore.Http;

namespace MvcIdentityServerSample.Controllers
{
    public class AccountController : Controller
    {
        //public UserManager<ApplicationUser> _userManager;
        //public SignInManager<ApplicationUser> _signInManager;

        private readonly TestUserStore _users;

        //public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        //{
        //    _userManager = userManager;
        //    _signInManager = signInManager;
        //}
        public AccountController(TestUserStore users)
        {
            _users = users;
        }

        public IActionResult Register(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel, string returnUrl)
        {
            //if (ModelState.IsValid)
            //{
            //    var identityUser = new ApplicationUser
            //    {
            //        Username = registerViewModel.Username,
            //        UserName = registerViewModel.Username,
            //        NormalizedUsername = registerViewModel.Username,
            //    };

            //    var identityResult = await _userManager.CreateAsync(identityUser, registerViewModel.Password);

            //    if (identityResult.Succeeded)
            //    {
            //        await _signInManager.SignInAsync(identityUser, new AuthenticationProperties { IsPersistent = true });
            //        return RedirectToAction("Index", "Home");
            //    }
            //    else
            //    {
            //        AddErrors(identityResult);
            //    }
            //}

            return View();
        }

        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                var user = _users.FindByUsername(loginViewModel.Username);
                if (user == null)
                {
                    ModelState.AddModelError(nameof(loginViewModel.Username), "Username not exists");
                }
                else
                {
                    if (_users.ValidateCredentials(loginViewModel.Username, loginViewModel.Password))
                    {
                        var props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))
                        };
                        await HttpContext.SignInAsync(user.SubjectId, user.Username, props);
                        return RedirectToLocal(returnUrl);
                    }
                    ModelState.AddModelError(nameof(loginViewModel.Password), "Wrong password");
                }

            }
            return View();
        }

        public async Task<IActionResult> MakeLogin()
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, "hkh"),
                new Claim(ClaimTypes.Role,"admin")
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            //await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private void AddErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}