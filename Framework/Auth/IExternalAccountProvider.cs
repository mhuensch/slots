using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foogenda.Framework
{
	public interface IExternalAccountProvider
	{
		string GetExternalAccountKey(string provider, string providerKey);
		void CreateExternalAccount(string provider, string providerKey, string givenName, string email);
	}
}
