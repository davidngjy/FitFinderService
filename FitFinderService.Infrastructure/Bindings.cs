using FitFinderService.Application.Interface;
using FitFinderService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitFinderService.Infrastructure
{
	public static class Bindings
	{
		public static void RegisterInfrastructure(this IServiceCollection service, IConfiguration configuration)
		{
			service.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					configuration.GetConnectionString("DefaultConnectionString")));

			service.AddScoped<IApplicationDbContext>(provider =>
				provider.GetService<ApplicationDbContext>());
		}
	}
}
