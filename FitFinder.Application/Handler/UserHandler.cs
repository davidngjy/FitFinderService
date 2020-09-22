using AutoMapper;
using FitFinder.Application.Interface;
using FitFinder.Application.Model;
using FitFinder.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FitFinder.Application.Handler
{
	internal class UserHandler : IUserHandler
	{
		private readonly IApplicationDbContext _context;
		private readonly IGoogleGateway _googleGateway;
		private readonly IMapper _mapper;

		public UserHandler(IApplicationDbContext context, IGoogleGateway googleGateway, IMapper mapper)
		{
			_context = context;
			_googleGateway = googleGateway;
			_mapper = mapper;
		}

		public Task<VerifiedUser> VerifyUser(string googleTokenId)
		{
			return _googleGateway.VerifyUserToken(googleTokenId);
		}

		public async Task CreateUser(VerifiedUser user, CancellationToken ct)
		{
			var userToCreate = _mapper.Map<VerifiedUser, User>(user);
			await _context.Users.AddAsync(userToCreate, ct);
			await _context.SaveChangesAsync(ct);
		}

		public async Task<UserProfile> GetUserProfile(string googleId, CancellationToken ct)
		{
			var user = await _context.Users
				.Where(u => u.GoogleId == googleId)
				.FirstOrDefaultAsync(ct);
			return _mapper.Map<User, UserProfile>(user);
		}
	}
}
