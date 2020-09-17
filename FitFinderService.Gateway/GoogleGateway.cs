using Google.Apis.Auth;
using Google.Apis.Util;
using System.Threading.Tasks;
using FitFinderService.Application.Interface;
using FitFinderService.Domain.Entity;

namespace FitFinderService.Gateway
{
	internal class GoogleGateway : IGoogleGateway
	{
		public async Task<User> VerifyUserToken(string token)
		{
			var payload = await GoogleJsonWebSignature.ValidateAsync(token);

			payload.ThrowIfNull("Invalid token");

			return new User
			{
				GoogleId = payload.Subject,
				DisplayName = payload.Name,
				Email = payload.Email,
				ProfilePictureUri = payload.Picture
			};
		}
	}
}
