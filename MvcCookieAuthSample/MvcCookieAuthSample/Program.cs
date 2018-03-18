using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvcCookieAuthSample.Data;

namespace MvcCookieAuthSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigratedBContext<ApplicationDbContext>(async (context, services) =>
                {
                    await new ApplicationContextSeed().SeedAsync(context, services);
                }).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
