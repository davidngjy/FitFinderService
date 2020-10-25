using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FitFinder.Protos;

namespace FitFinder.Application.Interface
{
	public interface ISessionHandler
	{
		Task<IEnumerable<UserSession>> GetAvailableSessionByRegion(Region region, CancellationToken ct);
		Task<UserSession> GetSession(long sessionId, CancellationToken ct);
		Task<IEnumerable<UserSession>> GetUserSessions(long userId, CancellationToken ct);
		Task<IEnumerable<UserSession>> GetUserBookingSession(long userId, CancellationToken ct);
		Task AddSession(long userId, AddSessionRequest request, CancellationToken ct);
		Task<bool> EditSession(EditSessionRequest request, CancellationToken ct);
		Task BookSession(long sessionId, long userId, CancellationToken ct);
		IDisposable SubscribeToUserSessionInsert(long userId, Action<UserSession> callback);
		IDisposable SubscribeToUserSessionUpdate(long userId, Action<UserSession> callback);
		IDisposable SubscribeToSessionBookingInsert(long userId, Action<UserSession> callback, CancellationToken ct);
		IDisposable SubscribeToSessionBookingUpdate(long userId, Action<UserSession> callback, CancellationToken ct);
	}
}
