using AutoMapper;
using FitFinderService.Application.Users.Command.CreateUser;
using FitFinderService.Application.Users.Query.GetUserProfile;
using FitFinderService.Application.Users.Query.VerifyUser;
using FitFinderService.Domain.Entity;

namespace FitFinderService.Application
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
