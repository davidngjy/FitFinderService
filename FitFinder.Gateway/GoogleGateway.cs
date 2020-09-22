using System.Threading.Tasks;
using FitFinder.Application.Interface;
using FitFinder.Application.Model;
using Google.Apis.Auth;

namespace FitFinder.Gateway
{
	internal class GoogleGateway : IGoogleGateway
	{
		public async Task<VerifiedUser> VerifyUserToken(string tokenId)
		{
			var payload = await GoogleJsonWebSignature.ValidateAsync(tokenId);
			var verifiedUser = new VerifiedUser
			{
				GoogleId = payload.Subject,
				DisplayName = payload.Name,
				Email = payload.Email,
				ProfilePictureUri = payload.Picture
			};

			return verifiedUser;
		}
	}
}
