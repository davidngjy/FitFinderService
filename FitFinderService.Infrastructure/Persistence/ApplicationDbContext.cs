using System;
using System.Threading;
using System.Threading.Tasks;
using FitFinderService.Application.Interface;
using FitFinderService.Domain.Common;
using FitFinderService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace FitFinderService.Infrastructure.Persistence
{
	internal class ApplicationDbContext : DbContext, IApplicationDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
			: base(options) { }

		public DbSet<User> Users { get; set; }

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
		{
			foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
			{
				// TODO: replace created by with claims
				if (entry.State == EntityState.Added)
				{
					entry.Entity.CreatedBy = "";
					entry.Entity.CreatedUtc = DateTime.UtcNow;
				}

				if (entry.State == EntityState.Added ||
				    entry.State == EntityState.Modified)
				{
					entry.Entity.LastModifiedBy = "";
					entry.Entity.LastModifiedUtc = DateTime.UtcNow;
				}
			}

			return base.SaveChangesAsync(cancellationToken);
		}
	}
}
