using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitFinder.Application.Interface;
using FitFinder.Application.Model;
using Microsoft.EntityFrameworkCore;

namespace FitFinder.Application.Handler
{
	internal class SessionHandler
	{
		private readonly IApplicationDbContext _context;

		public SessionHandler(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<SessionBooking>> GetUserSessions(long userId, CancellationToken ct)
		{
			var sessions = await _context
				.Sessions
				.Where(s => s.TrainerUserId == userId)
				.Include(s => s.Booking)
				.ToListAsync(ct);

			return sessions
				.Select(s => new SessionBooking
				{
					SessionId = s.Id,
					TrainerUserId = s.TrainerUserId,
					Title = s.Title,
					Description = s.Description,
					SessionDateTime = s.SessionDateTime,
					Location = s.Location,
					LocationString = s.LocationString,
					IsOnline = s.IsOnline,
					IsInPerson = s.IsInPerson,
					Price = s.Price,
					Duration = s.Duration,
					BookingId = s.BookingId,
					ClientUserId = s.Booking?.ClientUserId,
					BookingStatusId = (int?) s.Booking?.BookingStatusId
				});
		}
	}
}
