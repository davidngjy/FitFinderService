using System;
using FitFinder.Application.Interface;
using FitFinder.Application.Model;
using FitFinder.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FitFinder.Protos;
using Google.Protobuf;

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
			using (var webClient = new WebClient())
			{
				_context.Users.Add(new User
				{
					GoogleId = user.GoogleId,
					DisplayName = user.DisplayName,
					Email = user.Email,
					ProfilePicture = webClient.DownloadData(user.ProfilePictureUri),
					UserRoleId = UserRoleId.User
				});
			}
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
					Id = user.UserId,
					GoogleId = user.GoogleId,
					Email = user.Email,
					DisplayName = user.DisplayName,
					ProfilePicture = ByteString.CopyFrom(user.ProfilePicture),
					UserRole = (UserProfile.Types.UserRole) user.UserRoleId
				};
		}

		public async Task<UserProfile> GetUserProfile(long userId, CancellationToken ct)
		{
			var user = await _context.Users
				.Where(u => u.UserId == userId)
				.FirstOrDefaultAsync(ct);

			return user == null
				? null
				: new UserProfile
				{
					Id = user.UserId,
					GoogleId = user.GoogleId,
					Email = user.Email,
					DisplayName = user.DisplayName,
					ProfilePicture = ByteString.CopyFrom(user.ProfilePicture),
					UserRole = (UserProfile.Types.UserRole)user.UserRoleId
				};
		}

		public async Task UpdateUserProfile(long userId, UpdateUserProfileRequest updateRequest, CancellationToken ct)
		{
			var user = await _context
				.Users
				.Where(u => u.UserId == userId)
				.FirstOrDefaultAsync(ct);

			if (user == null)
				return;

			user.DisplayName = updateRequest.DisplayName;
			user.Email = updateRequest.Email;
			user.ProfilePicture = updateRequest.ProfilePicture.ToByteArray();
			await _context.SaveChangesAsync(ct);
		}

		public async Task<LimitedUserProfile> GetLimitedUserProfile(long userId, CancellationToken ct)
		{
			var user = await _context
				.Users
				.Where(u => u.UserId == userId)
				.FirstOrDefaultAsync(ct);

			if (user == null)
				return null;

			return new LimitedUserProfile
			{
				UserId = user.UserId,
				DisplayName = user.DisplayName,
				Email = user.Email,
				ProfilePicture = ByteString.CopyFrom(user.ProfilePicture),
			};
		}

		public IDisposable SubscribeToUserProfile(long userId, Action<UserProfile> callback)
		{
			return _userSubscription.SubscribeToUser(userId, user =>
			{
				callback(new UserProfile
				{
					Id = user.UserId,
					GoogleId = user.GoogleId,
					DisplayName = user.DisplayName,
					Email = user.Email,
					ProfilePicture = ByteString.CopyFrom(user.ProfilePicture),
					UserRole = (UserProfile.Types.UserRole) user.UserRoleId
				});
			});
		}
	}
}
