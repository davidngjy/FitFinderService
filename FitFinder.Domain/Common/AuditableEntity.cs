using System;

namespace FitFinder.Domain.Common
{
	public class AuditableEntity
	{
		public long CreatedByUserId { get; set; }

		public DateTime CreatedUtc { get; set; }

		public long LastModifiedByUserId { get; set; }

		public DateTime? LastModifiedUtc { get; set; }
	}
}
