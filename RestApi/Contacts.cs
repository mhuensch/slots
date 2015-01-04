using Foogenda.Framework;
using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Foogenda.RestApi
{
	public class Contacts : ApiController, IRestApiController<Contact>, IExternalLoginListener
	{
		public Contacts(IDocumentSession repository, IContext context)
		{
			_repository = repository;
			_context = context;
		}


		void IExternalLoginListener.LoginSuccessfull(string provider, string providerKey, string accessToken)
		{
			//TODO: this should be happening as a background task shortly after login, rather than at login time.
			var account = _repository.Query<Account>()
				.Where(a => a.Provider == provider && a.ProviderKey == providerKey)
				.FirstOrDefault();

			if (account == null)
				return;

			var settings = new RequestSettings("<var>foogenda</var>", accessToken);
			settings.AutoPaging = true;

			var cr = new ContactsRequest(settings);
			var query = new ContactsQuery(ContactsQuery.CreateContactsUri("default"));
			query.StartDate = account.SyncedContactsOn;

			if (account.SyncedContactsOn != DateTime.MinValue)
				query.ShowDeleted = true;


			var feed = cr.Get<Google.Contacts.Contact>(query);
			if (feed.Entries.Count() == 0)
				return;

			//There is probably a more efficient way to do this, but for now we
			//are loading all of the contacts in order to compare them from the cache.
			var contacts = _repository.Query<Contact>().ToList();
			var updatedContacts = new List<Contact>();
			foreach (var entry in feed.Entries)
			{

				var contact = contacts.Where(c => c.Provider == provider && c.ProviderKey == entry.Id).FirstOrDefault();
				if (contact == null)
					contact = new Contact() { Provider = provider, ProviderKey = entry.Id, Owner = account.Key };

				if (entry.Deleted)
				{
					_repository.Delete(contact);
					continue;
				}

				var name = entry.Title;
				if (entry.Name != null && entry.Name.FullName.HasValue())
					name = entry.Name.FullName;
				if (name.IsEmpty())
					continue;

				var image = string.Empty;
				if (entry.PhotoUri != null)
					image = entry.PhotoUri.ToString();

				var email = entry.Emails.Select(e => e.Address).FirstOrDefault();
				if (entry.PrimaryEmail != null)
					email = entry.PrimaryEmail.Address;

				contact.Key = name.Slugify();
				contact.Title = name;
				contact.Email = email;
				contact.Image = image;

				_repository.Store(contact);
			}

			account.SyncedContactsOn = DateTime.UtcNow;
			_repository.SaveChanges();
		}


		IQueryable<Contact> IRestApiController<Contact>.Get(string args)
		{
			return _repository.Filter<Contact>(args);
		}

		Contact IRestApiController<Contact>.Get(Guid id)
		{
			throw new NotImplementedException();
		}

		Contact IRestApiController<Contact>.Post([FromBody]Contact value)
		{
			throw new NotImplementedException();
		}

		Contact IRestApiController<Contact>.Put(Guid id, [FromBody]Contact value)
		{
			throw new NotImplementedException();
		}

		void IRestApiController<Contact>.Delete(Guid id)
		{
			throw new NotImplementedException();
		}

		private readonly IDocumentSession _repository;
		private readonly IContext _context;
	}
}
