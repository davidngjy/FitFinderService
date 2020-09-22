using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FitFinderService.Application.Interface;
using FitFinderService.Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitFinderService.Application.Users.Query.GetUserProfile
{
	public class GetUserProfileQuery : IRequest<UserProfileDto>
	{
		public string GoogleId { get; set; }
	}

	internal class GetUserProfileQueryHandler
		: IRequestHandler<GetUserProfileQuery, UserProfileDto>
	{
		private readonly IApplicationDbContext _context;
		private readonly IMapper _mapper;

		public GetUserProfileQueryHandler(IApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		public async Task<UserProfileDto> Handle(GetUserProfileQuery query,
			CancellationToken cancellationToken)
		{
			var user = await _context.Users
				.FirstOrDefaultAsync(u => u.GoogleId == query.GoogleId, cancellationToken);

			return _mapper.Map<User, UserProfileDto>(user);
		}
	}
}
