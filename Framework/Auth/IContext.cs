using System;
namespace Foogenda.Framework
{
	public interface IContext
	{
		Guid Id { get; }
		string Key { get; }
		DateTime Expiration { get; }
		string Provider { get; }
		string ProviderKey { get; }
		string Role { get; }
		string GivenName { get; }
		string Email { get; }
	}
}
