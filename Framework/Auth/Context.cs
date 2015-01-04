using System;
using System.Linq;
using System.Security.Claims;

namespace Foogenda.Framework
{
	public class Context : IContext
	{
		Guid IContext.Id
		{
			get
			{
				return
					ClaimsPrincipal.Current.Claims
					.Where(c => c.Type == ClaimTypes.NameIdentifier && c.Value.HasValue())
					.Select(c => Guid.Parse(c.Value)).SingleOrDefault();
			}
		}

		string IContext.Key
		{
			get
			{
				return
					ClaimsPrincipal.Current.Claims
					.Where(c => c.Type == ClaimTypes.Name)
					.Select(c => c.Value).SingleOrDefault();
			}
		}

		DateTime IContext.Expiration
		{
			get
			{
				return
					ClaimsPrincipal.Current.Claims
					.Where(c => c.Type == ClaimTypes.Expiration && c.Value.HasValue())
					.Select(c => DateTime.Parse(c.Value)).SingleOrDefault();
			}
		}

		string IContext.Provider
		{
			get
			{
				return
					ClaimsPrincipal.Current.Claims
					.Where(c => c.Type == ExtraClaimTypes.Provider)
					.Select(c => c.Value).SingleOrDefault();
			}
		}

		string IContext.ProviderKey
		{
			get
			{
				return
					ClaimsPrincipal.Current.Claims
					.Where(c => c.Type == ExtraClaimTypes.ProviderKey)
					.Select(c => c.Value).SingleOrDefault();
			}
		}

		string IContext.Role
		{
			get
			{
				return
					ClaimsPrincipal.Current.Claims
					.Where(c => c.Type == ClaimTypes.Role)
					.Select(c => c.Value).SingleOrDefault();
			}
		}

		string IContext.GivenName
		{
			get
			{
				return
					ClaimsPrincipal.Current.Claims
					.Where(c => c.Type == ClaimTypes.GivenName)
					.Select(c => c.Value).SingleOrDefault();
			}
		}

		string IContext.Email
		{
			get
			{
				return
					ClaimsPrincipal.Current.Claims
					.Where(c => c.Type == ClaimTypes.Email)
					.Select(c => c.Value).SingleOrDefault();
			}
		}

	}
}
