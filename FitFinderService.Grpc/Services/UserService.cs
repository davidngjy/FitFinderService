using FitFinderService.Application.Interface;
using FitFinderService.Proto;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FitFinderService.Grpc.Services
{
	public class UserService : Proto.UserService.UserServiceBase
	{
		private readonly ILogger<UserService> _logger;
		private readonly IGoogleGateway _googleGateway;

		public UserService(ILogger<UserService> logger, IGoogleGateway googleGateway)
		{
			_logger = logger;
			_googleGateway = googleGateway;
		}

		[AllowAnonymous]
		public override async Task ConnectUser(ConnectUserRequest request, IServerStreamWriter<ConnectUserResponse> responseStream, ServerCallContext context)
		{
			_logger.LogInformation($"Connect requested TokenId: {request.RequestIdToken}");

			await responseStream.WriteAsync(new ConnectUserResponse {Status = ConnectUserResponse.Types.Status.Verifying});
			_googleGateway.VerifyUserToken(request.RequestIdToken);
			
			// Check if user existed
			// If not
			await responseStream.WriteAsync(new ConnectUserResponse {Status = ConnectUserResponse.Types.Status.Creating});
			// Create the user

			await responseStream.WriteAsync(new ConnectUserResponse {Status = ConnectUserResponse.Types.Status.Retrieving});
			// Then retrieve user from DB

			// Done return the user profile!
			await responseStream.WriteAsync(new ConnectUserResponse {Status = ConnectUserResponse.Types.Status.Connected, UserProfile = new UserProfile()});
		}
	}
}
