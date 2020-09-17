using FitFinderService.Application.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace FitFinderService.Gateway
{
	public static class Bindings
	{
		public static void RegisterGateway(this IServiceCollection service)
		{
			service.AddTransient<IGoogleGateway, GoogleGateway>();
		}
	}
}
