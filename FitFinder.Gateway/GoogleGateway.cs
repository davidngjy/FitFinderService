using System.Threading.Tasks;
using FitFinder.Application.Interface;
using FitFinder.Application.Users.Query.VerifyUser;
using Google.Apis.Auth;

namespace FitFinder.Gateway
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
