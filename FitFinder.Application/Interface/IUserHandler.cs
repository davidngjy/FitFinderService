using System.Threading;
using System.Threading.Tasks;
using FitFinder.Application.Model;

namespace FitFinder.Application.Interface
{
	public interface IUserHandler
	{
		Task<VerifiedUser> VerifyUser(string googleTokenId);
		Task CreateUser(VerifiedUser user, CancellationToken ct);
		Task<UserProfile> GetUserProfile(string googleId, CancellationToken ct);
	}
}