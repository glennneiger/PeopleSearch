using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeopleSearch.Data.Contexts;
using PeopleSearch.Models;

namespace PeopleSearch
{
    public class Startup
    {
        public static IContainer ApplicationContainer { get; set; }
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var environmentConfig = Configuration.GetSection("Environment").Get<EnvironmentConfig>();
            var databaseConfig = Configuration.GetSection("Database").Get<DatabaseConfig>();

            services.AddDbContext<PeopleContext>(options => options.UseSqlServer(databaseConfig.ConnectionString));

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterInstance(environmentConfig)
                .AsSelf()
                .SingleInstance();

            containerBuilder.RegisterModule(new PeopleSearchModule());
            containerBuilder.Populate(services);
            ApplicationContainer = containerBuilder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}