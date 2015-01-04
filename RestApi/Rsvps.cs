using Foogenda.Framework;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Foogenda.RestApi
{
	public class Rsvps : ApiController, IRestApiController<Rsvp>
	{
		public Rsvps(IDocumentSession repository)
		{
			_repository = repository;
		}

		IQueryable<Rsvp> IRestApiController<Rsvp>.Get(string args)
		{
			return _repository.Filter<Rsvp>(args);
		}

		Rsvp IRestApiController<Rsvp>.Get(Guid id)
		{
			throw new NotImplementedException();
		}

		Rsvp IRestApiController<Rsvp>.Post([FromBody]Rsvp value)
		{
			throw new NotImplementedException();
		}

		Rsvp IRestApiController<Rsvp>.Put(Guid id, [FromBody]Rsvp value)
		{
			throw new NotImplementedException();
		}

		void IRestApiController<Rsvp>.Delete(Guid id)
		{
			throw new NotImplementedException();
		}

		private readonly IDocumentSession _repository;

	}
}
