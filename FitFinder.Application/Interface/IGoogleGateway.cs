using System.Threading.Tasks;
using FitFinder.Application.Model;

namespace FitFinder.Application.Interface
{
	public interface IGoogleGateway
	{
		Task<VerifiedUser> VerifyUserToken(string tokenId);
	}
}