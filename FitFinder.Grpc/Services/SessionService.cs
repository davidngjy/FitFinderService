using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitFinder.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace FitFinder.Grpc.Services
{
	public class SessionService : SessionProtocol.SessionProtocolBase
	{
		public override Task<Empty> AddSession(AddSessionRequest request, ServerCallContext context)
		{
			return Task.FromResult(new Empty());
		}

	}
}
