using Foogenda.Framework;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foogenda.RestApi
{
	public class Clients : ApiController, IRestApiController<Client>, IClientProvider
	{
		public Clients(IDocumentSession documentSession)
		{
			_documentSession = documentSession;
		}

		IQueryable<Client> IRestApiController<Client>.Get(string args)
		{
			throw new NotImplementedException();
		}

		Client IRestApiController<Client>.Get(Guid id)
		{
			throw new NotImplementedException();
		}

		Client IRestApiController<Client>.Post(Client value)
		{
			throw new NotImplementedException();
		}

		Client IRestApiController<Client>.Put(Guid id, Client value)
		{
			throw new NotImplementedException();
		}

		void IRestApiController<Client>.Delete(Guid id)
		{
			throw new NotImplementedException();
		}

		Client IClientProvider.GetClient(string id)
		{
			var guid = Guid.Parse(id);
			return _documentSession.Load<Client>(guid);
		}

		private readonly IDocumentSession _documentSession;

	}
}
