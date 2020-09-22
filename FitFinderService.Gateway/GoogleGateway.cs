using FitFinderService.Application.Interface;
using Google.Apis.Auth;
using System.Threading.Tasks;
using FitFinderService.Application.Users.Query.VerifyUser;

namespace FitFinderService.Gateway
{
	internal class GoogleGateway : IGoogleGateway
	{
		public async Task<VerifiedUser> VerifyUserToken(string token)
		{
			var payload = await GoogleJsonWebSignature.ValidateAsync(token);
			var userDto = new VerifiedUser
			{
				GoogleId = payload.Subject,
				DisplayName = payload.Name,
				Email = payload.Email,
				ProfilePictureUri = payload.Picture
			};

			return userDto;
		}
	}
}
