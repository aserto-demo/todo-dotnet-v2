using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using Aserto.AspNetCore.Middleware.Extensions;
using Aserto.AspNetCore.Middleware.Policies;
using Aserto.TodoApp.Domain.Repositories;
using Aserto.TodoApp.Domain.Services;
using Aserto.TodoApp.Persistence.Contexts;
using Aserto.TodoApp.Persistence.Repositories;
using Aserto.TodoApp.Services;
using Aserto.TodoApp.Configuration;
using Aserto.TodoApp.Options;

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
        options.UseInMemoryDatabase("aserto-todo-app-in-memory");
      });

      services.AddScoped<ITodoRepository, TodoRepository>();
      services.AddScoped<IUnitOfWork, UnitOfWork>();
      services.AddScoped<ITodoService, TodoService>();
      services.AddScoped<IUserService, UserService>();

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
      {
        options.Authority = domain;
        options.Audience = Configuration["OAuth:Audience"];
      });

      //Aserto options handling
      services.AddAsertoAuthorization(options => Configuration.GetSection("Aserto").Bind(options));
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

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      // global cors policy
      app.UseCors(x =>
      {
        x.AllowAnyHeader();
        x.AllowAnyMethod();
        x.WithOrigins("http://localhost:3000");
        x.AllowCredentials();
      });

      app.UseAuthentication();

      app.UseAuthorization();

      app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
  }
}