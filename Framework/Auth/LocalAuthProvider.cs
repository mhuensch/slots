using Castle.Windsor;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Foogenda.Framework
{
	//Based on: http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/
	public class LocalAuthProvider : OAuthAuthorizationServerProvider
	{
		public LocalAuthProvider(IWindsorContainer container)
		{
			_container = container;
		}

		public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			string clientId = string.Empty;
			string clientSecret = string.Empty;

			if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
			{
				context.TryGetFormCredentials(out clientId, out clientSecret);
			}

			if (context.ClientId == null)
			{
				//Remove the comments from the below line context.SetError, and invalidate context 
				//if you want to force sending clientId/secrets once obtain access tokens. 
				context.Validated();
				//context.SetError("invalid_clientId", "ClientId should be sent.");
				return Task.FromResult<object>(null);
			}

			var clientProvider = _container.Resolve<IClientProvider>();
			var client = default(Client);
			try
			{
				client = clientProvider.GetClient(context.ClientId.Replace("client_id=", ""));
			}
			finally
			{
				_container.Release(client);
			}

			if (client == null)
			{
				context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
				return Task.FromResult<object>(null);
			}

			if (client.IsNativeConfidential)
			{
				if (string.IsNullOrWhiteSpace(clientSecret))
				{
					context.SetError("invalid_clientId", "Client secret should be sent.");
					return Task.FromResult<object>(null);
				}
				else
				{
					if (client.Secret != clientSecret.GetHash())
					{
						context.SetError("invalid_clientId", "Client secret is invalid.");
						return Task.FromResult<object>(null);
					}
				}
			}

			if (!client.Active)
			{
				context.SetError("invalid_clientId", "Client is inactive.");
				return Task.FromResult<object>(null);
			}

			context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);

			context.Validated();
			return Task.FromResult<object>(null);
		}

		public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			//TODO: Implement allowed origin
			var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
			if (allowedOrigin == null) allowedOrigin = "*";

			context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });


			var identityProvider = _container.Resolve<IIdentityProvider>();
			var identity = default(ClaimsIdentity);
			try
			{
				identity = identityProvider.GetClaims(context.Options.AuthenticationType, context.UserName, context.Password);
			}
			finally
			{
				_container.Release(identityProvider);
			}



			var props = new AuthenticationProperties(new Dictionary<string, string>
			{
				{ 
						"as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
				},
				{ 
						"userName", context.UserName
				}
			});


			var ticket = new AuthenticationTicket(identity, props);
			context.Validated(ticket);

		}

		private readonly IWindsorContainer _container;
	}
}