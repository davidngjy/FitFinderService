using System.Collections.Generic;

namespace FitFinder.Domain.Entity
{
	public enum BookingStatusId
	{
		Pending = 1,
		Confirmed = 2,
		Cancelled = 3
	}

	public class BookingStatus
	{
		public BookingStatusId BookingStatusId { get; set; }
		public string Name { get; set; }

		public ICollection<Booking> Bookings { get; set; }
	}
}
