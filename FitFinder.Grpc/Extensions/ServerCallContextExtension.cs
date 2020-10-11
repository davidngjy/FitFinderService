using Grpc.Core;
using System;
using System.Security.Claims;

namespace FitFinder.Grpc.Extensions
{
	internal static class ServerCallContextExtension
	{
		public static long GetUserId(this ServerCallContext context)
		{ 
			var userIdString = context.GetHttpContext().User.FindFirstValue("UserId");
			return Convert.ToInt64(userIdString);
		}
	}
}
