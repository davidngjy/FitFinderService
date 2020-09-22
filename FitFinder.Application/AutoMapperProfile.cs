using AutoMapper;
using FitFinder.Application.Model;
using FitFinder.Domain.Entity;

namespace FitFinder.Application
{
	internal class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<User, UserProfile>()
				.ForMember(d => d.UserRole, 
					opt => opt.MapFrom(s => (int) s.UserRoleId));

			CreateMap<VerifiedUser, User>();
		}
	}
}
