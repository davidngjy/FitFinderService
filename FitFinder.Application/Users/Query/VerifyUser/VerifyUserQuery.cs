using System.Threading;
using System.Threading.Tasks;
using FitFinder.Application.Interface;
using MediatR;

namespace FitFinder.Application.Users.Query.VerifyUser
{
	public class VerifyUserQuery : IRequest<VerifiedUser>
	{
		public string TokenId { get; set; }
	}

	internal class VerifyUserQueryHandler
		: IRequestHandler<VerifyUserQuery, VerifiedUser>
	{
		private readonly IGoogleGateway _googleGateway;

		public VerifyUserQueryHandler(IGoogleGateway googleGateway)
		{
			_googleGateway = googleGateway;
		}

		public async Task<VerifiedUser> Handle(VerifyUserQuery query,
			CancellationToken cancellationToken)
		{
			return await _googleGateway.VerifyUserToken(query.TokenId);
		}
	}
}
