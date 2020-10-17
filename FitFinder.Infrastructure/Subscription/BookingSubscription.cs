using EntityFrameworkCore.Rx;
using FitFinder.Domain.Entity;
using System;
using System.Reactive.Linq;

namespace FitFinder.Infrastructure.Subscription
{
	internal class BookingSubscription
	{
		public IDisposable SubscribeToBookingInsert(long sessionId, Action<Booking> callback)
		{
			return DbObservable<Booking>
				.FromInserted()
				.Where(x => x.Entity.SessionId == sessionId)
				.Subscribe(x => callback(x.Entity));
		}

		public IDisposable SubscribeToBookingUpdate(long sessionId, Action<Booking> callback)
		{
			return DbObservable<Booking>
				.FromUpdated()
				.Where(x => x.Entity.SessionId == sessionId)
				.Subscribe(x => callback(x.Entity));
		}
	}
}
