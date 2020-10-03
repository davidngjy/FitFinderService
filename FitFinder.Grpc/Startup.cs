using System.Security.Claims;
using FitFinder.Application;
using FitFinder.Application.Interface;
using FitFinder.Gateway;
using FitFinder.Grpc.Authentication;
using FitFinder.Grpc.Services;
using FitFinder.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FitFinder.Grpc
{
	public class Startup
	{
		private IConfiguration Configuration { get; }
		private IWebHostEnvironment Environment { get; }

		public Startup(IConfiguration configuration, IWebHostEnvironment environment)
		{
			Configuration = configuration;
			Environment = environment;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			if (Configuration.GetValue<string>("PORT", null) != null)
			{
				services.Configure<KestrelServerOptions>(opt =>
				{
					opt.ListenAnyIP(Configuration.GetValue("PORT", 80));
				});
			}

			services.AddGrpc();
			services.RegisterGateway();
			services.RegisterInfrastructure(Configuration);
			services.RegisterApplication();

			services.AddHttpContextAccessor();

			services.AddScoped<ICurrentUserService, CurrentUserService>();

			if (!Environment.IsDevelopment())
			{
				services.AddAuthentication(o =>
				{
					o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

				}).AddJwtBearer(o =>
				{
					o.SecurityTokenValidators.Clear();
					o.SecurityTokenValidators.Add(new GoogleTokenValidator());
					o.Events = new GoogleTokenEvent();
				});

				services.AddAuthorization(o =>
				{
					o.FallbackPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
						.RequireAuthenticatedUser()
						.RequireClaim(ClaimTypes.Role)
						.Build();
				});
			}
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			if (!Environment.IsDevelopment())
			{
				app.UseAuthentication();
				app.UseAuthorization();
			}

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGrpcService<UserService>();

				endpoints.MapGet("/", async context =>
				{
					await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
				});
			});
		}
	}
}
