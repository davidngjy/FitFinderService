using FitFinder.Application.Interface;
using FitFinderService.Proto;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FitFinder.Grpc.Services
{
	public class UserService : UserProtocol.UserProtocolBase
	{
		private readonly ILogger<UserService> _logger;
		private readonly IUserHandler _userHandler;

		public UserService(ILogger<UserService> logger, IUserHandler userHandler)
		{
			_logger = logger;
			_userHandler = userHandler;
		}

		[AllowAnonymous]
		public override async Task ConnectUser(ConnectUserRequest request, IServerStreamWriter<ConnectUserResponse> responseStream, ServerCallContext context)
		{
			_logger.LogInformation("Received TokenId: {token}", request.GoogleTokenId);
			var ct = context.CancellationToken;

			try
			{
				await responseStream.WriteAsync(new ConnectUserResponse {Status = ConnectUserResponse.Types.Status.Verifying});
				
				var verifiedUser = await _userHandler.VerifyUser(request.GoogleTokenId);
				var user = await _userHandler.GetUserProfile(verifiedUser.GoogleId, ct);

				if (user == null)
				{
					// First time login, create the user
					await responseStream.WriteAsync(new ConnectUserResponse {Status = ConnectUserResponse.Types.Status.Creating});
					await _userHandler.CreateUser(verifiedUser, ct);
				}

				await Task.Delay(2000);
				await responseStream.WriteAsync(new ConnectUserResponse {Status = ConnectUserResponse.Types.Status.Retrieving});
				var userProfile = await _userHandler.GetUserProfile(verifiedUser.GoogleId, ct);
				await Task.Delay(2000);
				// Done return the user profile!
				await responseStream.WriteAsync(new ConnectUserResponse
				{
					Status = ConnectUserResponse.Types.Status.Connected,
					UserProfile = new UserProfile
					{
						Id = userProfile.Id,
						GoogleId = userProfile.GoogleId,
						DisplayName = userProfile.DisplayName,
						Email = userProfile.Email,
						ProfilePictureUri = userProfile.ProfilePictureUri,
						UserRole = (UserProfile.Types.UserRole)userProfile.UserRole
					}
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while trying to connect user token: {token}", request.GoogleTokenId);
				await responseStream.WriteAsync(new ConnectUserResponse { Status = ConnectUserResponse.Types.Status.Failed });
			}
			
		}
	}
}
