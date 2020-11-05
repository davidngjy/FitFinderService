using System;
using System.Threading;
using System.Threading.Tasks;
using FitFinder.Application.Model;
using FitFinder.Protos;

namespace FitFinder.Application.Interface
{
	public interface IUserHandler
	{
		Task<VerifiedUser> VerifyUser(string googleTokenId);
		Task CreateUser(VerifiedUser user, CancellationToken ct);
		Task<UserProfile> GetUserProfile(string googleId, CancellationToken ct);
		Task<UserProfile> GetUserProfile(long userId, CancellationToken ct);
		Task UpdateUserProfile(long userId, UpdateUserProfileRequest updateRequest, CancellationToken ct);
		Task<LimitedUserProfile> GetLimitedUserProfile(long userId, CancellationToken ct);
		IDisposable SubscribeToUserProfile(long userId, Action<UserProfile> callback);
	}
}