using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FitFinder.Application.Interface;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FitFinder.Grpc.Authentication
{
	public class GoogleTokenEvent : JwtBearerEvents
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
					new Claim(ClaimTypes.Role, user.UserRoleId.ToString()),
					new Claim("UserId", user.UserId.ToString())
				};
				var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
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
