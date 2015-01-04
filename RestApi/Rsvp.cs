using System;

namespace Foogenda.RestApi
{
	public sealed class Rsvp
	{
		public Guid Id { get; set; }
		public string Owner { get; set; }
		public string Key { get; set; }
		public bool IsPublic { get; set; }
		public string Title { get; set; }
	}
}