using System.Collections.Generic;

namespace FitFinder.Domain.Entity
{
	public enum BookingStatusId
	{
		Pending = 0,
		Confirmed = 1,
		Cancelled = 2
	}

	public class BookingStatus
	{
		public BookingStatusId BookingStatusId { get; set; }
		public string Name { get; set; }

		public ICollection<Booking> Bookings { get; set; }
	}
}
