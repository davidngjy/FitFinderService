using EntityFrameworkCore.Rx;
using FitFinder.Domain.Entity;
using System;
using System.Reactive.Linq;
using FitFinder.Application.Interface;

namespace FitFinder.Infrastructure.Subscription
{
	internal class SessionSubscription : ISessionSubscription
	{
		public IDisposable SubscribeToUserSessionInsert(long userId, Action<Session> callback)
		{
			return DbObservable<Session>
				.FromInserted()
				.Where(s => s.Entity.TrainerUserId == userId)
				.Subscribe(s => callback(s.Entity));
		}

		public IDisposable SubscribeToUserSessionUpdate(long userId, Action<Session> callback)
		{
			return DbObservable<Session>
				.FromUpdated()
				.Where(s => s.Entity.TrainerUserId == userId)
				.Subscribe(s => callback(s.Entity));
		}

		public IDisposable SubscribeToSessionBookingInsert(Action<Booking> callback)
		{
			return DbObservable<Booking>
				.FromInserted()
				.Subscribe(b => callback(b.Entity));
		}

		public IDisposable SubscribeToSessionBookingUpdate(Action<Booking> callback)
		{
			return DbObservable<Booking>
				.FromUpdated()
				.Subscribe(b => callback(b.Entity));
		}
	}
}
