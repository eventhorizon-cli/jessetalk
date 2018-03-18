using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MvcCookieAuthSample.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MvcCookieAuthSample.Data
{
    public class ApplicationContextSeed
    {
        private UserManager<ApplicationUser> _userManager;

        public async Task SeedAsync(ApplicationDbContext context, IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                if (!context.Users.Any())
                {
                    _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    var defaultUser = new ApplicationUser
                    {
                        UserName = "Administrator",
                        Email = "hkh@test.com",
                        NormalizedEmail = "admin",
                    };
                    var result = await _userManager.CreateAsync(defaultUser, "Password$123");
                    if (!result.Succeeded)
                    {
                        throw new Exception("初始用户创建失败");
                    }
                }
            }
        }
    }
}
