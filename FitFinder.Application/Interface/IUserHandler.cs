using System;
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
		Task UpdateUserProfile(long userId, string displayName, string email, string profilePictureUri, CancellationToken ct);
		IDisposable SubscribeToUserProfile(long userId, Action<UserProfile> callback);
	}
}