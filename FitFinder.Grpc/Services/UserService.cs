using FitFinder.Application.Interface;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using FitFinder.Protos;
using Google.Protobuf.WellKnownTypes;

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

				await responseStream.WriteAsync(new ConnectUserResponse {Status = ConnectUserResponse.Types.Status.Retrieving});
				var userProfile = await _userHandler.GetUserProfile(verifiedUser.GoogleId, ct);
				
				// Done return the user profile!
				await responseStream.WriteAsync(new ConnectUserResponse
				{
					Status = ConnectUserResponse.Types.Status.Connected,
					UserProfile = userProfile
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while trying to connect user token: {token}", request.GoogleTokenId);
				await responseStream.WriteAsync(new ConnectUserResponse { Status = ConnectUserResponse.Types.Status.Failed });
			}
		}

		public override async Task SubscribeToUserProfile(SubscribeUserProfileRequest request, IServerStreamWriter<UserProfile> responseStream, ServerCallContext context)
		{
			try
			{
				var user = await _userHandler.GetUserProfile(request.Id, context.CancellationToken);
				await responseStream.WriteAsync(user);

				using (_userHandler.SubscribeToUserProfile(request.Id, async user => await responseStream.WriteAsync(user)))
					await Task.Delay(-1, context.CancellationToken);
			}
			catch (TaskCanceledException ex)
			{
				_logger.LogInformation(ex , "User {id} stopped subscribing to UserProfile", request.Id);
			}
		}

		public override async Task<Empty> UpdateUserProfile(UpdateUserProfileRequest request, ServerCallContext context)
		{
			await _userHandler.UpdateUserProfile(request, context.CancellationToken);
			return new Empty();
		}
	}
}
