using System.Threading.Tasks;
using FitFinderService.Application.Users.Query.VerifyUser;

namespace FitFinderService.Application.Interface
{
	public interface IGoogleGateway
	{
		Task<VerifiedUser> VerifyUserToken(string token);
	}
}