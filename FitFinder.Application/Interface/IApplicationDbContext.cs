using System.Threading;
using System.Threading.Tasks;
using FitFinder.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FitFinder.Application.Interface
{
	public interface IApplicationDbContext
	{
		DatabaseFacade Database { get; }

		DbSet<User> Users { get; set; }

		DbSet<Session> Sessions { get; set; }

		DbSet<Booking> Bookings { get; set; }

		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	}
}
