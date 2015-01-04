using System;

namespace Foogenda.RestApi
{
	public sealed class Contact
	{
		public Guid Id { get; set; }
		public string Owner { get; set; }
		public string Key { get; set; }
		public string Title { get; set; }
		public string Email { get; set; }
		public string Image { get; set; }
		public string Provider { get; set; }
		public string ProviderKey { get; set; }
	}
}