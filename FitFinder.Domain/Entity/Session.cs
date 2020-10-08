using System;
using System.ComponentModel.DataAnnotations;
using FitFinder.Domain.Common;
using NetTopologySuite.Geometries;

namespace FitFinder.Domain.Entity
{
	public class Session : AuditableEntity
	{
		[Key]
		public long Id { get; set; }

		[Required]
		public string Title { get; set; }

		public string Description { get; set; }

		[Required]
		public DateTime SessionDateTime { get; set; }

		[Required]
		public Point Location { get; set; }

		public string LocationString { get; set; }

		public bool IsOnline { get; set; }

		public bool IsInPerson { get; set; }

		public double Price { get; set; }

		public TimeSpan Duration { get; set; }

		[Required]
		public long TrainerUserId { get; set; }
		public User TrainerUser { get; set; }

		public long? BookingId { get; set; }
		public Booking Booking { get; set; }
	}
}
