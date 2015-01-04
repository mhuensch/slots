using System.Security.Claims;

namespace Foogenda.Framework
{
	public interface IIdentityProvider
	{
		ClaimsIdentity GetClaims(string authenticationType, string username, string password);
	}
}
