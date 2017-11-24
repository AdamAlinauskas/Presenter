using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.S3;
using DataAccess;
using Domain;
using Meetaroo.Controllers;
using Meetaroo.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Service;

namespace Meetaroo
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }
        private IHostingEnvironment env;

        public Startup(IHostingEnvironment env)
        {
            this.env = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();

            // TODO AP : Use Configuration loaded from appSettings.json for these strings
            var dbHost = Environment.GetEnvironmentVariable("DATABASE_HOST");
            var connectionString = string.Format("Server={0};Database=meetaroo;Username=meetaroo;Password=x1Y6Dfb4ElF7C6JbEo170raDSaQRcb71;Search Path=meetaroo_shared", dbHost);

            services.AddScoped(serviceProvider => new NpgsqlConnection(connectionString));

            services.AddScoped<ICurrentSchema, CurrentSchema>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            
            //Web services
            services.AddTransient<IRetrieveIpAddress, RetrieveIpAddress>();

            //Service Layer
            services.AddTransient<IConfirmSchemaExists, ConfirmSchemaExists>();
            services.AddTransient<ICreateProfileService, CreateProfileService>();
            services.AddTransient<IUploadFileCommand, UploadFileCommand>();
            services.AddTransient<IRetrieveDocumentsQuery, RetrieveDocumentsQuery>();
            services.AddTransient<IRetrieveDocumentUrlQuery, RetrieveDocumentUrlQuery>();
            services.AddTransient<IRetrievePresentationListingQuery, RetrievePresentationListingQuery>();
            services.AddTransient<ICreatePresentationCommand, CreatePresentationCommand>();
            services.AddTransient<IRetrievePresentationToViewQuery, RetrievePresentationToViewQuery>();
            services.AddTransient<IUpdatePresentationCurrentPage, UpdatePresentationCurrentPage>();
            services.AddTransient<IPresentationCurrentPageQuery, PresentationCurrentPageQuery>();
            services.AddTransient<ICreateUserAnalyticsSessionCommand, CreateUserAnalyticsSessionCommand>();
            services.AddTransient<IRetrieveOrganizationsQuery, RetrieveOrganizationsQuery>();
            services.AddTransient<IRetrieveLocationFromIpAddress,RetrieveLocationFromIpAddress>();
            services.AddTransient<IRetrieveLocation, RetrieveLocation>();
            services.AddTransient<IRetrieveLocationFromGpsData,RetrieveLocationFromGpsData>();

            //DAL
            services.AddTransient<IConversationRepository, ConversationRepository>();
            services.AddTransient<IDocumentRepository, DocumentRepository>();
            services.AddTransient<IOrganizationRepository, OrganizationRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IPresentationRepository, PresentationRepository>();
            services.AddTransient<IUserAnalyticsSessionRepository, UserAnalyticsSessionRepository>();

            ConfigureAuth(services);

            services.AddMvc();

            AddAws(services);
        }

        private void AddAws(IServiceCollection services)
        {
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
        }

        private void ConfigureAuth(IServiceCollection services)
        {
            // Add authentication services
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options => {
                options.CookieManager = new CookieManager();
            })
            .AddOpenIdConnect("Auth0", options =>
            {
                // Set the authority to your Auth0 domain
                options.Authority = $"https://{Configuration["Auth0:Domain"]}";

                // Configure the Auth0 Client ID and Client Secret
                options.ClientId = Configuration["Auth0:ClientId"];
                options.ClientSecret = Configuration["Auth0:ClientSecret"];

                // Set response type to code
                options.ResponseType = "code";

                // Configure the scope
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                // Set the correct name claim type
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };

                // Set the callback path, so Auth0 will call back to http://localhost:5000/signin-auth0 
                // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard 
                options.CallbackPath = new PathString("/signin-auth0");

                // Configure the Claims Issuer to be Auth0
                options.ClaimsIssuer = "Auth0";

                options.SaveTokens = true;

                options.Events = new OpenIdConnectEvents
                {
                    // handle the logout redirection 
                    OnRedirectToIdentityProviderForSignOut = (context) =>
                    {
                        var logoutUri = $"https://{Configuration["Auth0:Domain"]}/v2/logout?client_id={Configuration["Auth0:ClientId"]}";

                        var postLogoutUri = context.Properties.RedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                            if (postLogoutUri.StartsWith("/"))
                            {
                                // transform to absolute
                                var request = context.Request;
                                postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                            }
                            logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
                        }

                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = (context) =>
                    {
                        var principal = context.Principal;
                        var userProfile = new User
                        {
                            Name = principal.Identity.Name,
                            Email = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value,
                            Picture = principal.Claims.FirstOrDefault(claim => claim.Type == "picture")?.Value,
                            Identifier = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value
                        };
                        return services.BuildServiceProvider().GetService<ICreateProfileService>().EnsureExists(userProfile);
                    },
                    OnRedirectToIdentityProvider = context => {
                        if (env.IsProduction())
                        {
                            var builder = new UriBuilder(context.ProtocolMessage.RedirectUri);
                            builder.Scheme = "https";
                            builder.Port = -1;
                            context.ProtocolMessage.RedirectUri = builder.ToString();
                        }
                        return Task.FromResult(0);
                    }
                };
            });

            services
                .AddDataProtection(options => { options.ApplicationDiscriminator = "findecks_web"; })
                .PersistKeysToFileSystem(new DirectoryInfo("/usr/local/share/findecks_keys"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions{
                 ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSignalR(routes =>
            {
                routes.MapHub<Presentation>("ViewPresentation");
                routes.MapHub<Meetaroo.Hubs.Conversation>("JoinConversation");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
