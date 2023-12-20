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
using Aserto.TodoApp.Persistence.Contexts;
using Aserto.TodoApp.Services;
using Aserto.TodoApp.Configuration;
using Aserto.TodoApp.Options;
using Google.Protobuf.WellKnownTypes;
using System;
using Microsoft.AspNetCore.Http;
using Aserto.TodoApp.Mapping;
using AspNetCore.Authentication.Basic;
using Aserto.Authorizer.V2.API;
using System.Security.Claims;
using System.Collections.Generic;

namespace Aserto.TodoApp
{
    public class Startup
    {
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
                    options.UseInMemoryDatabase("aserto-todo-app-in-memory");
                },
                ServiceLifetime.Transient
            );

            // services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ITodoService, TodoService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddAuthentication(BasicDefaults.AuthenticationScheme).AddBasic<BasicUserValidationService>(options => { options.Realm = "My App"; });

            //Aserto options handling
            services.AddAsertoAuthorization(options =>
            {
                Configuration.GetSection("Aserto").Bind(options);
                options.ResourceMapper = AuthzResourceContext.Instance.ResourceMapper;
                //options.IdentityMapper = myIdentityMapper;
            });
            //end Aserto options handling

            services.Configure<AsertoConfig>(Configuration.GetSection("Aserto"));
            services.Configure<DirectoryConfig>(Configuration.GetSection("Directory"));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Aserto", policy => policy.Requirements.Add(new AsertoDecisionRequirement()));
            });
            // Only authorizes the endpoints that have the [Authorize("Aserto")] attribute

            services.AddControllers();
            services.AddAutoMapper(typeof(Startup).Assembly);

        }
        /*
        private IdentityContext myIdentityMapper(ClaimsPrincipal principal, IEnumerable<string> enumerable)
        {
            return new IdentityContext()
            {
                Type = IdentityType.Sub,
                Identity = "rick@the-citadel.com",
            };
        }*/

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
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            

            AuthzResourceContext.Instance.ResourceMapper = (policyRoot, httpRequest) =>
            {
                Struct result = new Struct();

                if (httpRequest.RouteValues.ContainsKey("id"))
                {
                    using (var scope = app.ApplicationServices.CreateScope())
                    {
                        var todoService = scope.ServiceProvider.GetService<ITodoService>();
                        var todoTask = todoService.GetAsync((string)httpRequest.RouteValues["id"]);
                        todoTask.Wait();

                        result.Fields.Add("ownerID", Value.ForString(todoTask.Result.OwnerID));
                    }
                }

                return result;
            };
        }
    }
}
