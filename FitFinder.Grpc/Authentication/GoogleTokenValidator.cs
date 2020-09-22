using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;

namespace FitFinder.Grpc.Authentication
{
	public class GoogleTokenValidator : ISecurityTokenValidator
	{
		private readonly JwtSecurityTokenHandler _tokenHandler;

		public GoogleTokenValidator()
			=> _tokenHandler = new JwtSecurityTokenHandler();

		public bool CanValidateToken => true;

		public bool CanReadToken(string securityToken)
			=> _tokenHandler.CanReadToken(securityToken);

		public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

		public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
		{
			validatedToken = null;
			
			var payload = GoogleJsonWebSignature.ValidateAsync(securityToken).Result;

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, payload.Name),
				new Claim(JwtRegisteredClaimNames.FamilyName, payload.FamilyName),
				new Claim(JwtRegisteredClaimNames.GivenName, payload.GivenName),
				new Claim(JwtRegisteredClaimNames.Email, payload.Email),
				new Claim(JwtRegisteredClaimNames.Iss, payload.Issuer),
				new Claim("GoogleId", payload.Subject)
			};

			var principle = new ClaimsPrincipal();
			principle.AddIdentity(new ClaimsIdentity(claims));
			return principle;
		}
	}
}
