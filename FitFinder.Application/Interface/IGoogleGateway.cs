using System.Threading.Tasks;
using FitFinder.Application.Users.Query.VerifyUser;

namespace FitFinder.Application.Interface
{
	public interface IGoogleGateway
	{
		Task<VerifiedUser> VerifyUserToken(string token);
	}
}