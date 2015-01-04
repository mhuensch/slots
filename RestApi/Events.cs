using Foogenda.Framework;
using Raven.Abstractions.Commands;
using Raven.Client;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;

namespace Foogenda.RestApi
{
	public class Events : ApiController, IRestApiController<Event>
	{
		public Events(IDocumentSession repository, IContext userContext)
		{
			_context = userContext;
			_repository = repository;
		}

		IQueryable<Event> IRestApiController<Event>.Get(string args)
		{
			return _repository.Filter<Event>(args).Where(CanRead());
		}

		Event IRestApiController<Event>.Get(Guid id)
		{
			var result = _repository.Load<Event>(id);

			if (CanRead().Compile().Invoke(result) == false)
				throw new UnauthorizedAccessException("You do not have permission to view this event");

			return result;
		}

		Event IRestApiController<Event>.Post([FromBody]Event value)
		{
			if (CanUpdate(value.Id) == false)
				throw new UnauthorizedAccessException("You do not have permission to modify this event");

			if (value.Title.HasValue())
				value.Key = value.Title.Slugify();

			if (value.Owner.IsEmpty() && value.Id.IsEmpty())
			{
				value.OwnerId = _context.Id;
				value.Owner = _context.Key;
			}


			Validate(value);

			_repository.Store(value);
			_repository.SaveChanges();
			return value;
		}

		Event IRestApiController<Event>.Put(Guid id, [FromBody]Event value)
		{
			if (CanUpdate(id) == false)
				throw new UnauthorizedAccessException("You do not have permission to modify this event");

			Validate(value);

			_repository.Store(value);
			_repository.SaveChanges();
			return value;
		}

		void IRestApiController<Event>.Delete(Guid id)
		{
			var fEvent = _repository.Load<Event>(id);
			if (fEvent.OwnerId != _context.Id)
				throw new UnauthorizedAccessException("You do not have permission to delete this event");

			_repository.Advanced.Defer(new DeleteCommandData { Key = id.ToString() });
		}

		private Expression<Func<Event, bool>> CanRead()
		{
			return fEvent =>
				fEvent.IsPublic ||
				fEvent.OwnerId == _context.Id ||
				fEvent.Invitations.Any(i => i.ContactId == _context.Id);
		}

		private bool CanUpdate(Guid id)
		{
			var fEvent = _repository.Load<Event>(id);
			if (fEvent == null)
				return true;

			_repository.Advanced.Evict(fEvent);

			if (fEvent.OwnerId != _context.Id)
				return false;

			return true;
		}


		private void Validate(Event changedEvent)
		{
			if (string.IsNullOrWhiteSpace(changedEvent.Title))
				throw new ArgumentException("The title of an event can not be empty");

			if (string.IsNullOrWhiteSpace(changedEvent.Owner))
				throw new ArgumentException("The owner of an event can not be empty");

			if (string.IsNullOrWhiteSpace(changedEvent.Key))
				throw new ArgumentException("The key of an event can not be empty");
		}

		private readonly IContext _context;
		private readonly IDocumentSession _repository;

	}
}
