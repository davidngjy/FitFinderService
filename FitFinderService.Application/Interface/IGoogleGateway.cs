using System.Threading.Tasks;
using FitFinderService.Domain.Entity;

namespace FitFinderService.Application.Interface
{
	public interface IGoogleGateway
	{
		Task<User> VerifyUserToken(string token);
	}
}