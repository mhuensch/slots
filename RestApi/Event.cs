
using System;
using System.Collections.Generic;

namespace Foogenda.RestApi
{
	public sealed class Event
	{
		public Guid Id { get; set; }

		public string Owner { get; set; }

		public Guid OwnerId { get; set; }

		public string Key { get; set; }

		public bool IsPublic { get; set; }

		public string Title { get; set; }

		public DateTime Start { get; set; }

		public DateTime End { get; set; }

		public int Minimum { get; set; }

		public string Location { get; set; }

		public string Description { get; set; }

		public IEnumerable<Invitation> Invitations { get; set; }


		public sealed class Invitation
		{
			public Guid ContactId { get; set; }
			public string Name { get; set; }
			public bool? Accepted { get; set; }
		}
	}
}