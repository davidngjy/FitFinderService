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
			service.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					configuration.GetConnectionString("DefaultConnection")));

			service.AddScoped<IApplicationDbContext>(provider =>
				provider.GetService<ApplicationDbContext>());
		}
	}
}
