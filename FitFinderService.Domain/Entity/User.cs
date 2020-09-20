using FitFinderService.Domain.Common;

namespace FitFinderService.Domain.Entity
{
	public class User : AuditableEntity
	{
		public long? Id { get; set; }
		public string GoogleId { get; set; }
		public string DisplayName{ get; set; }
		public string Email { get; set; }
		public UserRole UserRole { get; set; }
		public string ProfilePictureUri { get; set; }
	}
}
