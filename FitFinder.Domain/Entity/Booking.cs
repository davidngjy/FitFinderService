using System.ComponentModel.DataAnnotations;
using FitFinder.Domain.Common;

namespace FitFinder.Domain.Entity
{
	public class Booking : AuditableEntity
	{
		[Key]
		public long BookingId { get; set; }

		[Required]
		public BookingStatusId BookingStatusId { get; set; }

		[Required]
		public long ClientUserId { get; set; }
		public User ClientUser { get; set; }

		[Required]
		public long SessionId { get; set; }
		public Session Session { get; set; }
	}
}
