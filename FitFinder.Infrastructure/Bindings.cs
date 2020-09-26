using FitFinder.Application.Interface;
using FitFinder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitFinder.Infrastructure
{
	public static class Bindings
	{
		public static void RegisterInfrastructure(this IServiceCollection service, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("DefaultConnection")
				.Replace("{user_name}", configuration.GetValue<string>("sql_username"))
				.Replace("{pass_word}", configuration.GetValue<string>("sql_password"));

			service.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

			service.AddScoped<IApplicationDbContext>(provider =>
				provider.GetService<ApplicationDbContext>());
		}
	}
}
