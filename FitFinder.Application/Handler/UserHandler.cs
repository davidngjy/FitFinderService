using System;
using FitFinder.Application.Interface;
using FitFinder.Application.Model;
using FitFinder.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitFinder.Protos;

namespace FitFinder.Application.Handler
{
	internal class UserHandler : IUserHandler
	{
		private readonly IApplicationDbContext _context;
		private readonly IGoogleGateway _googleGateway;
		private readonly IUserSubscription _userSubscription;

		public UserHandler(IApplicationDbContext context, IGoogleGateway googleGateway, IUserSubscription userSubscription)
		{
			_context = context;
			_googleGateway = googleGateway;
			_userSubscription = userSubscription;
		}

		public Task<VerifiedUser> VerifyUser(string googleTokenId)
		{
			return _googleGateway.VerifyUserToken(googleTokenId);
		}

		public async Task CreateUser(VerifiedUser user, CancellationToken ct)
		{
			await _context.Users.AddAsync(new User
			{
				GoogleId = user.GoogleId,
				DisplayName = user.DisplayName,
				Email = user.Email,
				ProfilePictureUri = user.ProfilePictureUri,
				UserRoleId = UserRoleId.User
			}, ct);

			await _context.SaveChangesAsync(ct);
		}

		public async Task<UserProfile> GetUserProfile(string googleId, CancellationToken ct)
		{
			var user = await _context.Users
				.Where(u => u.GoogleId == googleId)
				.FirstOrDefaultAsync(ct);

			return user == null 
				? null 
				: new UserProfile
				{
					Id = user.Id,
					GoogleId = user.GoogleId,
					Email = user.Email,
					DisplayName = user.DisplayName,
					ProfilePictureUri = user.ProfilePictureUri,
					UserRole = (UserProfile.Types.UserRole) user.UserRoleId
				};
		}

		public async Task<UserProfile> GetUserProfile(long userId, CancellationToken ct)
		{
			var user = await _context.Users
				.Where(u => u.Id == userId)
				.FirstOrDefaultAsync(ct);

			return user == null
				? null
				: new UserProfile
				{
					Id = user.Id,
					GoogleId = user.GoogleId,
					Email = user.Email,
					DisplayName = user.DisplayName,
					ProfilePictureUri = user.ProfilePictureUri,
					UserRole = (UserProfile.Types.UserRole)user.UserRoleId
				};
		}

		public async Task UpdateUserProfile(UpdateUserProfileRequest updateRequest, CancellationToken ct)
		{
			var user = await _context
				.Users
				.Where(u => u.Id == updateRequest.UserId)
				.FirstOrDefaultAsync(ct);

			if (user == null)
				return;

			user.DisplayName = updateRequest.DisplayName;
			user.Email = updateRequest.Email;
			user.ProfilePictureUri = updateRequest.ProfilePictureUri;
			await _context.SaveChangesAsync(ct);
		}

		public IDisposable SubscribeToUserProfile(long userId, Action<UserProfile> callback)
		{
			return _userSubscription.SubscribeToUser(userId, user =>
			{
				callback(new UserProfile
				{
					Id = user.Id,
					GoogleId = user.GoogleId,
					DisplayName = user.DisplayName,
					Email = user.Email,
					ProfilePictureUri = user.ProfilePictureUri,
					UserRole = (UserProfile.Types.UserRole) user.UserRoleId
				});
			});
		}
	}
}
