using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FitFinder.Application.Interface;
using FitFinder.Domain.Entity;
using MediatR;

namespace FitFinder.Application.Users.Command.CreateUser
{
	public class CreateUserCommand : IRequest
	{
		public string GoogleId { get; set; }

		public string DisplayName { get; set; }

		public string Email { get; set; }

		public string ProfilePictureUri { get; set; }
	}

	internal class CreateUserCommandHandler
		: IRequestHandler<CreateUserCommand>
	{
		private readonly IApplicationDbContext _context;
		private readonly IMapper _mapper;

		public CreateUserCommandHandler(IApplicationDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
		{
			var newUser = _mapper.Map<CreateUserCommand, User>(request);
			await _context.Users.AddAsync(newUser, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			return Unit.Value;
		}
	}
}
