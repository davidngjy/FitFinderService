using System;
using System.Threading.Tasks;
using FitFinder.Application.Interface;
using FitFinder.Grpc.Extensions;
using FitFinder.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Utils;
using Microsoft.Extensions.Logging;

namespace FitFinder.Grpc.Services
{
	public class SessionService : SessionProtocol.SessionProtocolBase
	{
		private readonly ILogger<SessionService> _logger;
		private readonly ISessionHandler _sessionHandler;

		public SessionService(ILogger<SessionService> logger, ISessionHandler sessionHandler)
		{
			_logger = logger;
			_sessionHandler = sessionHandler;
		}

		public override async Task<Response> AddSession(AddSessionRequest request, ServerCallContext context)
		{
			var userId = context.GetUserId();

			await _sessionHandler.AddSession(userId, request, context.CancellationToken);

			return new Response { ResultStatus = Response.Types.Status.Success };
		}

		public override async Task<Response> EditSession(EditSessionRequest request, ServerCallContext context)
		{
			var userId = context.GetUserId();
			if (userId != request.TrainerUserId)
				throw new UnauthorizedAccessException();

			var isSuccess = await _sessionHandler.EditSession(request, context.CancellationToken);

			return isSuccess 
				? new Response { ResultStatus = Response.Types.Status.Success } 
				: new Response { ResultStatus = Response.Types.Status.Failed };
		}

		public override async Task SubscribeToUserSession(Empty request, IServerStreamWriter<UserSession> responseStream, ServerCallContext context)
		{
			var userId = context.GetUserId();

			var sessions = await _sessionHandler.GetUserSessions(userId, context.CancellationToken);

			await responseStream.WriteAllAsync(sessions);

			async Task WriteUserSession(UserSession session)
			{
				await responseStream.WriteAsync(session);
			}

			try
			{
				using (_sessionHandler.SubscribeToUserSessionInsert(userId, async s => await WriteUserSession(s)))
				using (_sessionHandler.SubscribeToUserSessionUpdate(userId, async s => await WriteUserSession(s)))
				{
					await Task.Delay(-1, context.CancellationToken);
				}
			}
			catch (TaskCanceledException ex)
			{
				_logger.LogInformation(ex, "User {userId} stopped subscribing to UserSession", userId);
			}
		}

		public override async Task SubscribeToAvailableSessions(Empty request, IServerStreamWriter<UserSession> responseStream, ServerCallContext context)
		{
			var sessions = await _sessionHandler.GetAvailableSession(context.CancellationToken);

			await responseStream.WriteAllAsync(sessions);

			async Task WriteUserSession(UserSession session)
			{
				await responseStream.WriteAsync(session);
			}

			try
			{
				using (_sessionHandler.SubscribeToSessionInsert(async s => await WriteUserSession(s)))
				using (_sessionHandler.SubscribeToSessionUpdate(async s => await WriteUserSession(s)))
				{
					await Task.Delay(-1, context.CancellationToken);
				}
			}
			catch (TaskCanceledException ex)
			{
				_logger.LogInformation(ex, "User {userId} stopped subscribing to available sessions", context.GetUserId());
			}
		}
	}
}
