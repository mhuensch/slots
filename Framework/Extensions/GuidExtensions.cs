using System;

namespace Foogenda.Framework
{
	public static class GuidExtensions
	{
		public static bool IsEmpty(this Guid source)
		{
			return source == Guid.Empty;
		}

		public static bool HasValue(this Guid source)
		{
			return source != Guid.Empty;
		}
	}
}
