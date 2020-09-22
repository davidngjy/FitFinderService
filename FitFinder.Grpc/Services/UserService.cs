using System;
using System.Threading.Tasks;
using AutoMapper;
using FitFinder.Application.Users.Command.CreateUser;
using FitFinder.Application.Users.Query.GetUserProfile;
using FitFinder.Application.Users.Query.VerifyUser;
using FitFinderService.Proto;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace FitFinder.Grpc.Services
{
	public class UserService : FitFinderService.Proto.UserService.UserServiceBase
	{
		private readonly ILogger<UserService> _logger;
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;

		public UserService(ILogger<UserService> logger, IMediator mediator, IMapper mapper)
		{
			_logger = logger;
			_mediator = mediator;
			_mapper = mapper;
		}

		[AllowAnonymous]
		public override async Task ConnectUser(ConnectUserRequest request, IServerStreamWriter<ConnectUserResponse> responseStream, ServerCallContext context)
		{
			_logger.LogInformation("Received TokenId: {token}", request.RequestIdToken);
			var ct = context.CancellationToken;

			try
			{
				await responseStream.WriteAsync(new ConnectUserResponse {Status = ConnectUserResponse.Types.Status.Verifying});

				var verifiedUser = await _mediator.Send(new VerifyUserQuery {TokenId = request.RequestIdToken}, ct);

				var user = await _mediator.Send(new GetUserProfileQuery {GoogleId = verifiedUser.GoogleId}, ct);

				if (user == null)
				{
					// First time login, create the user
					await responseStream.WriteAsync(new ConnectUserResponse {Status = ConnectUserResponse.Types.Status.Creating});
					await _mediator.Send(_mapper.Map<VerifiedUser, CreateUserCommand>(verifiedUser), ct);
				}

				await responseStream.WriteAsync(new ConnectUserResponse {Status = ConnectUserResponse.Types.Status.Retrieving});
				var userProfile = await _mediator.Send(new GetUserProfileQuery {GoogleId = verifiedUser.GoogleId}, ct);

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
						UserRole = (UserProfile.Types.UserRole) userProfile.UserRole
					}
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while trying to connect user token: {token}", request.RequestIdToken);
				await responseStream.WriteAsync(new ConnectUserResponse { Status = ConnectUserResponse.Types.Status.Failed });
			}
			
		}
	}
}
