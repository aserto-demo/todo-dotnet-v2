using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using Aserto.AspNetCore.Middleware.Extensions;
using Aserto.AspNetCore.Middleware.Policies;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Services;
using Aserto.TodoApp.Configuration;
using Aserto.TodoApp.Options;
using Google.Protobuf.WellKnownTypes;
using System;
using Microsoft.AspNetCore.Http;
using Aserto.TodoApp.Mapping;
using System.Collections.Generic;
using System.Security.Claims;
using Aserto.AspNetCore.Middleware.Options;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Aserto.Authorizer.V2.API;
using Aserto.TodoApp.Persistence.Contexts;

namespace Aserto.TodoApp
{
    public class Startup
    {
        public CheckOptions checkOptions = new CheckOptions();

        delegate Struct ResolveResourceContext(string policyRoot, HttpContext httpContext);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
             
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            string domain = $"https://{Configuration["OAuth:Domain"]}/";

            services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(@"Data Source=Application.db;Cache=Shared");
                },
                ServiceLifetime.Singleton
            );

            // services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ITodoService, TodoService>();
            services.AddTransient<IUserService, UserService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = Configuration["OAuth:Audience"];
            });

            //Aserto options handling
            services.AddAsertoAuthorization(options =>
            {
                options.PolicyRoot = Configuration.GetSection("Aserto")["PolicyRoot"];
                options.IdentityMapper = AuthzIdentityContext.Instance.IdentityMapper;              
                options.ResourceMapper = (policyRoot, httpRequest) =>
                {
                    Struct result = new Struct();
                    if (httpRequest.RouteValues.ContainsKey("id"))
                    {
                        result.Fields.Add("object_id", Value.ForString((string)httpRequest.RouteValues["id"]));
                    }
                    return result;
                };
            },
            authorizerConfig =>
            {
                Configuration.GetSection("Aserto").Bind(authorizerConfig);
            }
            );
            //end Aserto options handling

            var checkResourceRules = new Dictionary<string, Func<string, HttpRequest, Struct>>();

            checkResourceRules.Add("member", (policyRoot, httpRequest) =>
            {
                Struct result = new Struct();
               
                result.Fields.Add("object_id", Value.ForString("resource-creators"));
                result.Fields.Add("object_type", Value.ForString("resource-creator"));
                result.Fields.Add("relation", Value.ForString("member"));
                return result;                  
            });
          
            Configuration.GetSection("Aserto").Bind(checkOptions.BaseOptions);
            
            checkOptions.ResourceMappingRules = checkResourceRules;            
            checkOptions.BaseOptions.Enabled = true;         
            checkOptions.BaseOptions.PolicyRoot = "rebac";
            checkOptions.BaseOptions.PolicyPathMapper = (policyRoot, httpRequest) =>
            {
                return policyRoot+".check";
            };

            // Adding the check middleware with the 'member' resource context rule
            // will populate the resource context for controllers that have the check attribute set to admin
            services.AddAsertoCheckAuthorization(checkOptions,
            authorizerConfig =>
            {
                Configuration.GetSection("Aserto").Bind(authorizerConfig);
            });

           

            services.Configure<AsertoConfig>(Configuration.GetSection("Aserto"));
            services.Configure<DirectoryConfig>(Configuration.GetSection("Directory"));

            string[] claimTypes = { ClaimTypes.Email };
                        
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Aserto", policy => policy.Requirements.Add(new AsertoDecisionRequirement(claimTypes)));                
            });
            // Only authorizes the endpoints that have the [Authorize("Aserto")] attribute

            services.AddControllers();                    
            services.AddAutoMapper(typeof(Startup).Assembly);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // global cors policy
            app.UseCors(x =>
            {
                x.AllowAnyHeader();
                x.AllowAnyMethod();
                x.WithOrigins("http://localhost:3000");
                x.AllowCredentials();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();            
            app.MapWhen(
                httpContext => httpContext.Request.Method != HttpMethods.Post,
                subApp =>
                {
                    subApp.UseRouting();
                    subApp.UseAuthorization();
                    subApp.UseAsertoAuthorization();
                    subApp.UseEndpoints(endpoints => endpoints.MapControllers());
                }
                ) ;            
            app.UseAsertoCheckAuthorization();            
            app.UseEndpoints(endpoints => endpoints.MapControllers());            
        }
    }
}