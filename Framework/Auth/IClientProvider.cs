using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foogenda.Framework
{
	public interface IClientProvider
	{
		Client GetClient(string id);
	}
}
