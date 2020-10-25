using FitFinder.Application.Interface;
using FitFinder.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers;
using FitFinder.Domain.Common;

namespace FitFinder.Infrastructure.Persistence
{
	internal class ApplicationDbContext : DbContextWithTriggers, IApplicationDbContext
	{
		private readonly ICurrentUserService _currentUserService;

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService)
			: base(options)
		{
			_currentUserService = currentUserService;
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Session> Sessions { get; set; }
		public DbSet<Booking> Bookings { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			ConfigureUsers(modelBuilder);

			ConfigureSessions(modelBuilder);

			base.OnModelCreating(modelBuilder);
		}

		private static void ConfigureUsers(ModelBuilder modelBuilder)
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

		private static void ConfigureSessions(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Session>()
				.HasOne(s => s.TrainerUser)
				.WithMany(u => u.Sessions)
				.IsRequired(false);

			modelBuilder.Entity<Session>()
				.HasOne(s => s.Booking)
				.WithOne(b => b.Session)
				.IsRequired(false);

			modelBuilder.Entity<Booking>()
				.HasOne(b => b.Session)
				.WithOne(s => s.Booking)
				.HasForeignKey<Booking>(b => b.SessionId)
				.IsRequired();

			modelBuilder.Entity<Booking>()
				.HasOne(b => b.ClientUser)
				.WithMany(u => u.Bookings)
				.HasForeignKey(b => b.ClientUserId)
				.IsRequired();

			modelBuilder.Entity<Booking>()
				.Property(b => b.BookingStatusId)
				.HasDefaultValue(BookingStatusId.Pending);

			// Generate data for BookingStatus table
			modelBuilder.Entity<BookingStatus>()
				.HasData(Enum.GetValues(typeof(BookingStatusId))
					.Cast<BookingStatusId>()
					.Select(b => new BookingStatus
					{
						BookingStatusId = b,
						Name = b.ToString()
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
