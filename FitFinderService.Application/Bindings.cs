using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AutoMapper;

namespace FitFinderService.Application
{
	public static class Bindings
	{
		public static void RegisterApplication(this IServiceCollection service)
		{
			service.AddMediatR(Assembly.GetExecutingAssembly());
			service.AddAutoMapper(a => a.AddProfile(new AutoMapperProfile()));
		}
	}
}
