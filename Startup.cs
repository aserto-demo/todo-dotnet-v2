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
                Configuration.GetSection("Aserto").Bind(options);
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
            CheckOptions checkOptions = new CheckOptions();
            Configuration.GetSection("Aserto").Bind(checkOptions.BaseOptions);
            // Adding the check middleware
            services.AddAsertoCheckAuthorization(checkOptions,
            authorizerConfig =>
            {
                Configuration.GetSection("Aserto").Bind(authorizerConfig);
            });

            services.Configure<AsertoConfig>(Configuration.GetSection("Aserto"));
            services.Configure<DirectoryConfig>(Configuration.GetSection("Directory"));

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
            app.UseAsertoAuthorization();        
            app.UseAsertoCheckAuthorization();            
            app.UseEndpoints(endpoints => endpoints.MapControllers());            
        }
    }
}