using AspNet.Security.ApiKey.Providers;
using AspNet.Security.ApiKey.Providers.Events;
using AspNet.Security.ApiKey.Providers.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SkillsHeroes.IssuesApi.Data;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SkillsHeroes.IssuesApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<IssuesContext>(options =>
                {
                    options
                        .UseSqlServer(Configuration.GetConnectionString("IssuesContext"));
                });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = ApiKeyDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = ApiKeyDefaults.AuthenticationScheme;
                })
                .AddApiKey(options =>
                {
                    options.Header = "X-API-KEY";
                    options.HeaderKey = string.Empty;

                    options.Events = new ApiKeyEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ApiKeyEvents>>();

                            logger.LogError(context.Exception, "Error during authentication");

                            return Task.CompletedTask;
                        },
                        OnApiKeyValidated = context =>
                        {
                            var issuesContext = context.HttpContext.RequestServices.GetRequiredService<IssuesContext>();

                            var application = issuesContext.Applications.SingleOrDefault(a => a.ApiKey == context.ApiKey);

                            if (application != null)
                            {
                                // Create and add your application's identities here, if required.
                                var identity = new ClaimsIdentity(new[]
                                {
                                    new Claim("API_KEY", application.ApiKey)
                                });

                                context.Principal.AddIdentity(identity);

                                // Mark success if you are happy the API key in the request is valid.
                                context.Success();
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services
                .AddMvc()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("X-API-KEY", new ApiKeyScheme
                {
                    Description = "Standard Authorization header using the X-API-KEY.",
                    In = "header",
                    Name = "X-API-KEY",
                    Type = "apiKey"
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>(true, "X-API-KEY");
                c.DescribeAllEnumsAsStrings();
                c.SwaggerDoc("v1", new Info { Title = "Issues API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddHttpContextAccessor();

            services.AddScoped(provider =>
            {
                var accessor = provider.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
                var dbContext = provider.GetService<Data.IssuesContext>();

                var apiKey = accessor.HttpContext.User.Claims.Single(c => c.Type == "API_KEY").Value;

                return dbContext.Applications
                    .AsNoTracking()
                    .SingleOrDefault(a => a.ApiKey == apiKey);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseSwagger();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Issues API");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
