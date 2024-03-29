﻿using FitFinder.Application.Handler;
using FitFinder.Application.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace FitFinder.Application
{
	public static class Bindings
	{
		public static void RegisterApplication(this IServiceCollection service)
		{
			service.AddTransient<IUserHandler, UserHandler>();
			service.AddTransient<ISessionHandler, SessionHandler>();
		}
	}
}
