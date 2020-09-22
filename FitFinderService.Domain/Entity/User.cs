using System.ComponentModel.DataAnnotations;
using FitFinderService.Domain.Common;

namespace FitFinderService.Domain.Entity
{
	public class User : AuditableEntity
	{
		[Key]
		public long Id { get; set; }
		[Required]
		public string GoogleId { get; set; }
		public string DisplayName{ get; set; }
		public string Email { get; set; }
		public string ProfilePictureUri { get; set; }

		[Required]
		public UserRoleId UserRoleId { get; set; }
		public UserRole UserRole { get; set; }
	}
}
