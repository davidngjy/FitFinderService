using System.Collections.Generic;

namespace FitFinder.Domain.Entity
{
	public enum UserRoleId
	{
		Admin = 0,
		Trainer = 1,
		User = 2
	}

	public class UserRole
	{
		public UserRoleId UserRoleId { get; set; }
		public string Name { get; set; }

		public ICollection<User> Users { get; set; }
	}

	public static class Role
	{
		public const string Admin = "Admin";
		public const string Trainer = "Trainer";
		public const string User = "User";
	}
}
