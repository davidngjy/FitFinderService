using EntityFrameworkCore.Rx;
using FitFinder.Domain.Entity;
using System;
using System.Reactive.Linq;
using FitFinder.Application.Interface;

namespace FitFinder.Infrastructure.Subscription
{
	internal class UserSubscription : IUserSubscription
	{
		public IDisposable SubscribeToUser(long userId, Action<User> callback)
		{
			return DbObservable<User>
				.FromUpdated()
				.Where(u => u.Entity.Id == userId)
				.Subscribe(entry => callback(entry.Entity));
		}
	}
}
