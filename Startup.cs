using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using apiauthz.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using apiauthz.Models;
using apiauthz.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace api_authz
{
    public class Startup
    {
      //  private readonly IWebHostEnvironment _hostingEnv;

        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
           // _hostingEnv = env;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //var domain = $"https://{Configuration["Auth0:Domain"]}/";
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = $"https://{Configuration["Auth0:Domain"]}/";
                options.Audience = Configuration["Auth0:Audience"];
            });

            services.AddAuthorization();

            // requires using Microsoft.Extensions.Options
            services.Configure<ResourceAccessDatabaseSettings>(
                Configuration.GetSection(nameof(ResourceAccessDatabaseSettings)));

            services.AddSingleton<IResourceAccessDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<ResourceAccessDatabaseSettings>>().Value);

            services.AddSingleton<PolicyService>();
            services.AddSingleton<RoleService>();
            services.AddSingleton<AuthorizationService>();

            services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Resource Access Management API",
                        Description = "Resource Access Management API (ASP.NET Core 3.0)",
                        Contact = new OpenApiContact()
                        {
                            Name = "API Acceleration Center",
                            Url = new System.Uri("https://github.com/swagger-api/swagger-codegen"),
                            Email = "aac@deloitte.com"
                        },
                        TermsOfService = new System.Uri("http://google.com")
                    });
                    //c.CustomSchemaIds(type => type.FriendlyId(true));
                    //c.DescribeAllEnumsAsStrings();
                    //c.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{_hostingEnv.ApplicationName}.xml");
                    // Sets the basePath property in the Swagger document generated
                    //c.DocumentFilter<BasePathFilter>("/security/resourceaccessmanagement/v1");

                    // Include DataAnnotation attributes on Controller Action parameters as Swagger validation rules (e.g required, pattern, ..)
                    // Use [ValidateModelState] on Actions to actually validate it in C# as well!
                    c.OperationFilter<GeneratePathParamsValidationFilter>();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    //TODO: Either use the SwaggerGen generated Swagger contract (generated from C# classes)
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Resource Access Management API");

                    //TODO: Or alternatively use the original Swagger contract that's included in the static files
                    // c.SwaggerEndpoint("/swagger-original.json", "Resource Access Management API Original");
                });
        }
    }
}
