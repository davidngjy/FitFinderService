﻿namespace FitFinder.Application.Interface
{
	public interface ICurrentUserService
	{
		long CurrentUserId { get; }
		string UserName { get; }
	}
}