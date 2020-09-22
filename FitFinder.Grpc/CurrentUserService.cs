using System.Security.Claims;
using FitFinder.Application.Interface;
using Microsoft.AspNetCore.Http;

namespace FitFinder.Grpc
{
	public class CurrentUserService : ICurrentUserService
	{
		public long CurrentUserId { get; }
		public string UserName { get; }

		public CurrentUserService(IHttpContextAccessor httpContextAccessor)
		{
			var claims = httpContextAccessor
				.HttpContext?
				.User;

			CurrentUserId = long.TryParse(claims?.FindFirstValue("UserId"), out var userIdlLong) ? userIdlLong : 0;
			UserName = claims?.FindFirstValue(ClaimTypes.Name) ?? "";
		}
	}
}
