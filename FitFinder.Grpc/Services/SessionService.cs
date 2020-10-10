using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitFinder.Protos;
using Grpc.Core;

namespace FitFinder.Grpc.Services
{
	public class SessionService : SessionProtocol.SessionProtocolBase
	{
		public override Task<AddSessionReply> AddSession(AddSessionRequest request, ServerCallContext context)
		{
			return base.AddSession(request, context);
		}
	}
}
