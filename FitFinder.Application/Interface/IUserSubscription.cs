using FitFinder.Domain.Entity;
using System;

namespace FitFinder.Application.Interface
{
	public interface IUserSubscription
	{
		IDisposable SubscribeToUser(long userId, Action<User> callback);
	}
}
