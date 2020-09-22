using AutoMapper;
using FitFinder.Application.Users.Command.CreateUser;
using FitFinder.Application.Users.Query.GetUserProfile;
using FitFinder.Application.Users.Query.VerifyUser;
using FitFinder.Domain.Entity;

namespace FitFinder.Application
{
	internal class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<User, UserProfileDto>()
				.ForMember(d => d.UserRole, 
					opt => opt.MapFrom(s => (int) s.UserRoleId));

			CreateMap<VerifiedUser, CreateUserCommand>();

			CreateMap<CreateUserCommand, User>();
		}
	}
}
