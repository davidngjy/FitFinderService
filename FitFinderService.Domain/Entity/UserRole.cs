namespace FitFinderService.Domain.Entity
{
	public enum UserRole
	{
		Admin = 1,
		Trainer = 2,
		User = 3
	}

	public static class Role
	{
		public const string Admin = "Admin";
		public const string Trainer = "Trainer";
		public const string User = "User";
	}
}
