using System;

namespace Foogenda.RestApi
{
	public sealed class Account
	{
		public Guid Id { get; set; }

		public string Key { get; set; }

		public string TokenExpiration { get; set; }

		public string Password { get; set; }

		public string Provider { get; set; }

		public string ProviderKey { get; set; }

		public string Role { get; set; }

		public string GivenName { get; set; }

		public string Email { get; set; }

		public DateTime SyncedContactsOn { get; set; }

	}
}