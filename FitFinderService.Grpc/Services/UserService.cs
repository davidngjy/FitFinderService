using System.Threading.Tasks;
using FitFinderService.Application.Interface;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace FitFinderService.Grpc.Services
{
	public class UserService : Grpc.UserService.UserServiceBase
	{
		private readonly ILogger<UserService> _logger;
		private readonly IGoogleGateway _googleGateway;

		public UserService(ILogger<UserService> logger, IGoogleGateway googleGateway)
		{
			_logger = logger;
			_googleGateway = googleGateway;
		}

		public override Task<GetUserReply> GetUser(GetUserRequest request, ServerCallContext context)
		{
			_logger.LogInformation($"Token received: {request.RequestIdToken}");

			var response = _googleGateway.VerifyUserToken(request.RequestIdToken);

			return Task.FromResult(new GetUserReply
			{
				Id = 12345
			});
		}

		public override async Task GetUserStream(GetUserRequest request, IServerStreamWriter<GetUserReply> responseStream, ServerCallContext context)
		{
			_logger.LogInformation($"Token received: {request.RequestIdToken}");

			await Task.Delay(10000);
			await responseStream.WriteAsync(new GetUserReply { Id = 1 });
			await Task.Delay(10000);
			await responseStream.WriteAsync(new GetUserReply { Id = 2 });
			await Task.Delay(10000);
			await responseStream.WriteAsync(new GetUserReply { Id = 3 });
			await Task.Delay(10000);
			await responseStream.WriteAsync(new GetUserReply { Id = 4 });
			await Task.Delay(10000);
			await responseStream.WriteAsync(new GetUserReply { Id = 5 });

		}
	}
}
