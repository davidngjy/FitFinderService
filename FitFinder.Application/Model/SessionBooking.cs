using System;
using NetTopologySuite.Geometries;

namespace FitFinder.Application.Model
{
	public class SessionBooking
	{
		public long SessionId { get; set; }

		public long TrainerUserId { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public DateTime SessionDateTime { get; set; }

		public Point Location { get; set; }

		public string LocationString { get; set; }

		public bool IsOnline { get; set; }

		public bool IsInPerson { get; set; }

		public double Price { get; set; }

		public TimeSpan Duration { get; set; }

		public long? BookingId { get; set; }

		public long? ClientUserId { get; set; }

		public int? BookingStatusId { get; set; }
	}
}
