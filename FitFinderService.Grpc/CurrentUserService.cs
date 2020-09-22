using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using FitFinderService.Application.Interface;

namespace FitFinderService.Grpc
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
