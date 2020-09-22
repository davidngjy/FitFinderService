using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FitFinderService.Application.Interface;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FitFinderService.Grpc.Authentication
{
	public class GoogleTokenEvents : JwtBearerEvents
	{
		public override async Task TokenValidated(TokenValidatedContext context)
		{
			var googleId = context.Principal.FindFirstValue("GoogleId");
			var db = context.HttpContext.RequestServices.GetRequiredService<IApplicationDbContext>();

			var user = await db.Users.Where(u => u.GoogleId == googleId).FirstOrDefaultAsync();

			if (user != null)
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Role, user.UserRole.Name),
					new Claim("UserId", user.Id.ToString())
				};
				var identity = new ClaimsIdentity(claims);
				context.Principal.AddIdentity(identity);
			}
		}

		public override Task AuthenticationFailed(AuthenticationFailedContext context)
		{
			if (context.Exception.InnerException is InvalidJwtException)
			{
				context.Response.Headers.Add("Token-Expired", "true");
				context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
			}

			return Task.CompletedTask;
		}
	}
}
