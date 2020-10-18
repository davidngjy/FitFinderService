using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FitFinder.Protos;

namespace FitFinder.Application.Interface
{
	public interface ISessionHandler
	{
		Task<IEnumerable<UserSession>> GetAvailableSession(CancellationToken ct);
		Task<IEnumerable<UserSession>> GetUserSessions(long userId, CancellationToken ct);
		Task AddSession(long userId, AddSessionRequest request, CancellationToken ct);
		Task<bool> EditSession(EditSessionRequest request, CancellationToken ct);
		IDisposable SubscribeToUserSessionInsert(long userId, Action<UserSession> callback);
		IDisposable SubscribeToUserSessionUpdate(long userId, Action<UserSession> callback);
		IDisposable SubscribeToSessionInsert(Action<UserSession> callback);
		IDisposable SubscribeToSessionUpdate(Action<UserSession> callback);
	}
}
