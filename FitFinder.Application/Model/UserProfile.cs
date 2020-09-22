namespace FitFinder.Application.Model
{
	public class UserProfile
	{
		public long Id { get; set; }

		public string GoogleId { get; set; }

		public string DisplayName { get; set; }

		public string Email { get; set; }

		public int UserRole { get; set; }

		public string ProfilePictureUri { get; set; }
	}
}
