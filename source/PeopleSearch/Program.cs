using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeopleSearch.Data.Contexts;

namespace PeopleSearch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    var context = scope.ServiceProvider.GetService<PeopleContext>();
                    context.Database.Migrate();
                    logger.LogInformation("Database migration: Success.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Database migration: Error.");
                }
            }

            host.Run();
        }
        private static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
               .ConfigureServices(services =>
                {
                    services.AddAutofac();
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}