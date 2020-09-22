using System.Threading;
using System.Threading.Tasks;
using FitFinder.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace FitFinder.Application.Interface
{
	public interface IApplicationDbContext
	{
		DbSet<User> Users { get; set; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	}
}
