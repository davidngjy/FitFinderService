using System.Threading;
using System.Threading.Tasks;
using FitFinderService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace FitFinderService.Application.Interface
{
	public interface IApplicationDbContext
	{
		DbSet<User> Users { get; set; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	}
}
