using Foogenda.Framework;
using Raven.Client;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace Foogenda.RestApi
{
	public class Accounts : ApiController, IRestApiController<Account>, IIdentityProvider, IExternalAccountProvider
	{
		public Accounts(IDocumentSession repository)
		{
			_repository = repository;
		}

		[HttpPost]
		[AllowAnonymous]
		public Account Register([FromBody]Account value)
		{
			if (_repository.Query<Account>().Any(a => a.Key == value.Key))
				throw new InvalidOperationException("The username selected is already taken");

			return ((IRestApiController<Account>)this).Post(value);
		}




		IQueryable<Account> IRestApiController<Account>.Get(string args)
		{
			return _repository.Filter<Account>(args);
		}

		Account IRestApiController<Account>.Get(Guid id)
		{
			throw new NotImplementedException();
		}

		Account IRestApiController<Account>.Post([FromBody]Account value)
		{
			if (value.Password.HasValue())
				value.Password = value.Password.GetHash();

			if (value.Role.IsEmpty())
				value.Role = Roles.User;

			_repository.Store(value);
			_repository.SaveChanges();
			return value;
		}

		Account IRestApiController<Account>.Put(Guid id, [FromBody]Account value)
		{
			throw new NotImplementedException();
		}

		void IRestApiController<Account>.Delete(Guid id)
		{
			throw new NotImplementedException();
		}

		ClaimsIdentity IIdentityProvider.GetClaims(string authenticationType, string username, string password)
		{
			var account = _repository.Query<Account>()
				.Where(a => a.Key == username && a.Password == password.GetHash())
				.FirstOrDefault();

			if (account == null)
			{
				account = _repository.Query<Account>()
				.Where(a => a.Provider == username && a.ProviderKey == password)
				.FirstOrDefault();
			}

			if (account == null)
				throw new ArgumentException("The user name or password is incorrect.");

			var identity = new ClaimsIdentity(authenticationType);
			identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()));
			identity.AddClaim(new Claim(ClaimTypes.Name, account.Key));

			if (account.Role != null)
			identity.AddClaim(new Claim(ClaimTypes.Role, account.Role));
			if (account.Provider != null)
				identity.AddClaim(new Claim(ExtraClaimTypes.Provider, account.Provider));
			if (account.ProviderKey != null)
				identity.AddClaim(new Claim(ExtraClaimTypes.ProviderKey, account.ProviderKey));
			if (account.GivenName != null)
				identity.AddClaim(new Claim(ClaimTypes.GivenName, account.GivenName));
			if (account.Email != null)
				identity.AddClaim(new Claim(ClaimTypes.Email, account.Email));

			return identity;
		}

		string IExternalAccountProvider.GetExternalAccountKey(string provider, string providerKey)
		{
			return ((IRestApiController<Account>)this).Get()
				.Where(a => a.Provider == provider && a.ProviderKey == providerKey)
				.Select(a => a.Key)
				.SingleOrDefault();
		}

		void IExternalAccountProvider.CreateExternalAccount(string provider, string providerKey, string givenName, string email)
		{
			var key = givenName.Slugify();
			var found = _repository.Query<Account>().Where(a => a.Key == key);
			if (found.Count() > 0)
				key += found.Count() + 1;

			var newAccount = new Account()
			{
				Key = key,
				Provider = provider,
				ProviderKey = providerKey,
				GivenName = givenName,
				Email = email
			};

			((IRestApiController<Account>)this).Post(newAccount);
		}

		private readonly IDocumentSession _repository;
	}
}
