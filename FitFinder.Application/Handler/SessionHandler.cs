using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FitFinder.Application.Interface;
using FitFinder.Domain.Entity;
using FitFinder.Protos;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;

namespace FitFinder.Application.Handler
{
	internal class SessionHandler : ISessionHandler
	{
		private readonly IApplicationDbContext _context;
		private readonly ISessionSubscription _sessionSubscription;

		public SessionHandler(IApplicationDbContext context, ISessionSubscription sessionSubscription)
		{
			_context = context;
			_sessionSubscription = sessionSubscription;
		}

		public async Task<IEnumerable<UserSession>> GetAvailableSessionByRegion(Region region, CancellationToken ct)
		{
			var coordinates = new[]
			{
				new Coordinate(region.NorthEastBound.Longitude, region.NorthEastBound.Latitude),
				new Coordinate(region.SouthWestBound.Longitude, region.NorthEastBound.Latitude),
				new Coordinate(region.SouthWestBound.Longitude, region.SouthWestBound.Latitude),
				new Coordinate(region.NorthEastBound.Longitude, region.SouthWestBound.Latitude),
				new Coordinate(region.NorthEastBound.Longitude, region.NorthEastBound.Latitude)
			};

			var border = new GeometryFactory(new PrecisionModel(PrecisionModel.MaximumPreciseValue), 4326).CreatePolygon(new CoordinateArraySequence(coordinates));

			return await _context
				.Sessions
				.Include(s => s.Booking)
				.Where(s => s.Location.Within(border)
				            && s.Booking == null
				            && s.SessionDateTime > DateTime.UtcNow)
				.Select(s => new UserSession
				{
					SessionId = s.SessionId,
					TrainerUserId = s.TrainerUserId,
					Title = s.Title,
					Description = s.Description,
					SessionDateTime = DateTime.SpecifyKind(s.SessionDateTime, DateTimeKind.Utc).ToTimestamp(),
					Location = new LatLng { Longitude = s.Location.X, Latitude = s.Location.Y },
					LocationString = s.LocationString,
					IsOnline = s.IsOnline,
					IsInPerson = s.IsInPerson,
					Price = s.Price,
					Duration = s.Duration.ToDuration(),
					BookingId = null,
					ClientUserId = null,
					BookingStatus = Protos.BookingStatus.Unknown
				})
				.ToListAsync(ct);
		}

		public async Task<UserSession> GetSession(long sessionId, CancellationToken ct)
		{
			var session = await _context
				.Sessions
				.Where(s => s.SessionId == sessionId)
				.FirstOrDefaultAsync(ct);

			return new UserSession
			{
				SessionId = session.SessionId,
				TrainerUserId = session.TrainerUserId,
				Title = session.Title,
				Description = session.Description,
				SessionDateTime = DateTime.SpecifyKind(session.SessionDateTime, DateTimeKind.Utc).ToTimestamp(),
				Location = new LatLng {Longitude = session.Location.X, Latitude = session.Location.Y},
				LocationString = session.LocationString,
				IsOnline = session.IsOnline,
				IsInPerson = session.IsInPerson,
				Price = session.Price,
				Duration = session.Duration.ToDuration(),
				BookingId = session.Booking?.BookingId,
				ClientUserId = session.Booking?.ClientUserId,
				BookingStatus = (Protos.BookingStatus) (session.Booking?.BookingStatusId ?? 0)
			};
		}

		public async Task<IEnumerable<UserSession>> GetUserSessions(long userId, CancellationToken ct)
		{
			var sessions = await _context
				.Sessions
				.Where(s => s.TrainerUserId == userId)
				.Include(s => s.Booking)
				.ToListAsync(ct);

			return sessions
				.Select(s => new UserSession
				{
					SessionId = s.SessionId,
					TrainerUserId = s.TrainerUserId,
					Title = s.Title,
					Description = s.Description,
					SessionDateTime = DateTime.SpecifyKind(s.SessionDateTime, DateTimeKind.Utc).ToTimestamp(),
					Location = new LatLng{Longitude = s.Location.X, Latitude = s.Location.Y},
					LocationString = s.LocationString,
					IsOnline = s.IsOnline,
					IsInPerson = s.IsInPerson,
					Price = s.Price,
					Duration = s.Duration.ToDuration(),
					BookingId = s.Booking?.BookingId,
					ClientUserId = s.Booking?.ClientUserId,
					BookingStatus = (Protos.BookingStatus)(s.Booking?.BookingStatusId ?? 0)
				});
		}

		public async Task<IEnumerable<UserSession>> GetUserBookingSession(long userId, CancellationToken ct)
		{
			var sessions = await _context
				.Sessions
				.Include(s => s.Booking)
				.Where(s => s.Booking != null && (s.TrainerUserId == userId || s.Booking.ClientUserId == userId))
				.ToListAsync(ct);

			return sessions
				.Select(s => new UserSession
				{
					SessionId = s.SessionId,
					TrainerUserId = s.TrainerUserId,
					Title = s.Title,
					Description = s.Description,
					SessionDateTime = DateTime.SpecifyKind(s.SessionDateTime, DateTimeKind.Utc).ToTimestamp(),
					Location = new LatLng { Longitude = s.Location.X, Latitude = s.Location.Y },
					LocationString = s.LocationString,
					IsOnline = s.IsOnline,
					IsInPerson = s.IsInPerson,
					Price = s.Price,
					Duration = s.Duration.ToDuration(),
					BookingId = s.Booking?.BookingId,
					ClientUserId = s.Booking?.ClientUserId,
					BookingStatus = (Protos.BookingStatus)(s.Booking?.BookingStatusId ?? 0)
				});
		}

		public async Task AddSession(long userId, AddSessionRequest request, CancellationToken ct)
		{
			var newSession = new Session
			{
				Title = request.Title,
				Description = request.Description,
				SessionDateTime = request.SessionDateTime.ToDateTime(),
				Location = new Point(request.Location.Longitude, request.Location.Latitude) {SRID = 4326 },
				LocationString = request.LocationString,
				IsOnline = request.IsOnline,
				IsInPerson = request.IsInPerson,
				Price = request.Price,
				Duration = request.Duration.ToTimeSpan(),
				TrainerUserId = userId
			};

			_context.Sessions.Add(newSession);
			await _context.SaveChangesAsync(ct);
		}

		public async Task<bool> EditSession(EditSessionRequest request, CancellationToken ct)
		{
			var session = await _context
				.Sessions
				.Where(s => s.SessionId == request.SessionId)
				.FirstOrDefaultAsync(ct);

			if (session == null)
				return false;

			session.Title = request.Title;
			session.Description = request.Description;
			session.SessionDateTime = request.SessionDateTime.ToDateTime();
			session.Location = new Point(request.Location.Longitude, request.Location.Latitude);
			session.LocationString = request.LocationString;
			session.IsOnline = request.IsOnline;
			session.IsInPerson = request.IsInPerson;
			session.Price = request.Price;
			session.Duration = request.Duration.ToTimeSpan();

			await _context.SaveChangesAsync(ct);
			return true;
		}

		public async Task BookSession(long sessionId, long userId, CancellationToken ct)
		{
			var session = await _context
				.Sessions
				.Include(s => s.Booking)
				.Where(s => s.SessionId == sessionId)
				.FirstAsync(ct);

			if (session.Booking != null)
				throw new InvalidOperationException($"Session has been booked! SessionId:{sessionId} userId:{userId}");

			var booking = new Booking
			{
				SessionId = sessionId,
				ClientUserId = userId,
				BookingStatusId = BookingStatusId.Pending
			};

			_context.Bookings.Add(booking);
			await _context.SaveChangesAsync(ct);
		}

		public IDisposable SubscribeToUserSessionInsert(long userId, Action<UserSession> callback)
		{
			return _sessionSubscription
				.SubscribeToUserSessionInsert(userId, s => callback(new UserSession
				{
					SessionId = s.SessionId,
					TrainerUserId = s.TrainerUserId,
					Title = s.Title,
					Description = s.Description,
					SessionDateTime = DateTime.SpecifyKind(s.SessionDateTime, DateTimeKind.Utc).ToTimestamp(),
					Location = new LatLng { Longitude = s.Location.X, Latitude = s.Location.Y },
					LocationString = s.LocationString,
					IsOnline = s.IsOnline,
					IsInPerson = s.IsInPerson,
					Price = s.Price,
					Duration = s.Duration.ToDuration()
				}));
		}

		public IDisposable SubscribeToUserSessionUpdate(long userId, Action<UserSession> callback)
		{
			return _sessionSubscription
				.SubscribeToUserSessionUpdate(userId, s => callback(new UserSession
				{
					SessionId = s.SessionId,
					TrainerUserId = s.TrainerUserId,
					Title = s.Title,
					Description = s.Description,
					SessionDateTime = DateTime.SpecifyKind(s.SessionDateTime, DateTimeKind.Utc).ToTimestamp(),
					Location = new LatLng {Longitude = s.Location.X, Latitude = s.Location.Y},
					LocationString = s.LocationString,
					IsOnline = s.IsOnline,
					IsInPerson = s.IsInPerson,
					Price = s.Price,
					Duration = s.Duration.ToDuration()
				}));
		}

		public IDisposable SubscribeToSessionBookingInsert(long userId, Action<UserSession> callback, CancellationToken ct)
		{
			return _sessionSubscription
				.SubscribeToSessionBookingInsert(async b =>
				{
					var session = await _context
						.Sessions
						.Include(s => s.Booking)
						.Where(s => s.SessionId == b.SessionId)
						.FirstAsync(ct);

					if (session.TrainerUserId != userId && session.Booking.ClientUserId != userId)
						return;

					callback(new UserSession
					{
						SessionId = session.SessionId,
						TrainerUserId = session.TrainerUserId,
						Title = session.Title,
						Description = session.Description,
						SessionDateTime = DateTime.SpecifyKind(session.SessionDateTime, DateTimeKind.Utc).ToTimestamp(),
						Location = new LatLng {Longitude = session.Location.X, Latitude = session.Location.Y},
						LocationString = session.LocationString,
						IsOnline = session.IsOnline,
						IsInPerson = session.IsInPerson,
						Price = session.Price,
						Duration = session.Duration.ToDuration(),
						BookingId = session.Booking?.BookingId,
						ClientUserId = session.Booking?.ClientUserId,
						BookingStatus = (Protos.BookingStatus) (session.Booking?.BookingStatusId ?? 0)
					});
				});
		}

		public IDisposable SubscribeToSessionBookingUpdate(long userId, Action<UserSession> callback, CancellationToken ct)
		{
			return _sessionSubscription
				.SubscribeToSessionBookingUpdate(async b =>
				{
					var session = await _context
						.Sessions
						.Include(s => s.Booking)
						.Where(s => s.SessionId == b.SessionId)
						.FirstAsync(ct);

					if (session.TrainerUserId != userId && session.Booking.ClientUserId != userId)
						return;

					callback(new UserSession
					{
						SessionId = session.SessionId,
						TrainerUserId = session.TrainerUserId,
						Title = session.Title,
						Description = session.Description,
						SessionDateTime = DateTime.SpecifyKind(session.SessionDateTime, DateTimeKind.Utc).ToTimestamp(),
						Location = new LatLng { Longitude = session.Location.X, Latitude = session.Location.Y },
						LocationString = session.LocationString,
						IsOnline = session.IsOnline,
						IsInPerson = session.IsInPerson,
						Price = session.Price,
						Duration = session.Duration.ToDuration(),
						BookingId = session.Booking?.BookingId,
						ClientUserId = session.Booking?.ClientUserId,
						BookingStatus = (Protos.BookingStatus)(session.Booking?.BookingStatusId ?? 0)
					});
				});
		}
	}
}
