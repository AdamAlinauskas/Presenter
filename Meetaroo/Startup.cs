using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Meetaroo
{
    public class Startup
    {
        // public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // TODO AP : Use Configuration loaded from appSettings.json and environment for these strings
            var dbHost = Environment.GetEnvironmentVariable("DATABASE_HOST");
            var connectionString = string.Format("Server={0};Database=meetaroo;Username=meetaroo;Password=x1Y6Dfb4ElF7C6JbEo170raDSaQRcb71;Search Path=meetaroo_shared", dbHost);

            services.AddScoped(serviceProvider => new NpgsqlConnection(connectionString));
            //services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });

            // This is the "right" way to use a configuration, but it doesn't appear to be working
            //var builder = new ConfigurationBuilder().AddEnvironmentVariables();
            //Configuration = builder.Build();

            // Get values like this:
            //var dbHost = Configuration.GetValue("DATABASE_HOST", "localhost");
        }
    }
}
