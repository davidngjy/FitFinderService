using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitFinder.Application.Interface;
using FitFinder.Domain.Common;
using FitFinder.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace FitFinder.Infrastructure.Persistence
{
	internal class ApplicationDbContext : DbContext, IApplicationDbContext
	{
		private readonly ICurrentUserService _currentUserService;

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService) 
			: base(options)
		{
			_currentUserService = currentUserService;
		}

		public DbSet<User> Users { get; set; }

		// To generate UserRole table
		private DbSet<UserRole> UserRoles { get; set; } 

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.HasIndex(u => u.GoogleId)
				.IsClustered(false)
				.IsUnique();

			modelBuilder.Entity<User>()
				.Property(p => p.UserRoleId)
				.HasDefaultValue(UserRoleId.User);

			// Generate data for UserRole table
			modelBuilder.Entity<UserRole>()
				.HasData(Enum.GetValues(typeof(UserRoleId))
					.Cast<UserRoleId>()
					.Select(r => new UserRole
					{
						UserRoleId = r,
						Name = r.ToString()
					}));
		}

		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
		{
			var currentUserId = _currentUserService.CurrentUserId;

			foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
			{
				if (entry.State == EntityState.Added)
				{
					entry.Entity.CreatedByUserId = currentUserId;
					entry.Entity.CreatedUtc = DateTime.UtcNow;
				}

				if (entry.State == EntityState.Added ||
				    entry.State == EntityState.Modified)
				{
					entry.Entity.LastModifiedByUserId = currentUserId;
					entry.Entity.LastModifiedUtc = DateTime.UtcNow;
				}
			}

			return base.SaveChangesAsync(cancellationToken);
		}
	}
}
