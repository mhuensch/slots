using System;

namespace Foogenda.Framework
{
	public class Client
	{
		public Guid Id { get; set; }
		public string Secret { get; set; }
		public string Name { get; set; }
		public bool IsNativeConfidential { get; set; }
		public bool Active { get; set; }
		//public int RefreshTokenLifeTime { get; set; }
		public string AllowedOrigin { get; set; }
	}
}
