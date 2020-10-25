using System;
using FitFinder.Domain.Entity;

namespace FitFinder.Application.Interface
{
	public interface ISessionSubscription
	{
		IDisposable SubscribeToUserSessionInsert(long userId, Action<Session> callback);
		IDisposable SubscribeToUserSessionUpdate(long userId, Action<Session> callback);
		IDisposable SubscribeToSessionBookingInsert(Action<Booking> callback);
		IDisposable SubscribeToSessionBookingUpdate(Action<Booking> callback);
	}
}
