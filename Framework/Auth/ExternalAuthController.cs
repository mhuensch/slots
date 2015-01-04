using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foogenda.Framework
{
	[RoutePrefix("ExternalAuth")]
	public class ExternalAuthController : ApiController
	{
		public ExternalAuthController(IExternalAccountProvider accounts, IEnumerable<IExternalLoginListener> listeners, IEnumerable<IExternalOAuthConfiguration> authConfigs, OAuthBearerAuthenticationOptions oAuthBearerOptions, OAuthAuthorizationServerOptions oAuthServerOptions)
		{
			_accounts = accounts;
			_listeners = listeners;
			_authConfigs = authConfigs;
			_oAuthBearerOptions = oAuthBearerOptions;
			_oAuthServerOptions = oAuthServerOptions;
		}

		[HttpGet]
		[AllowAnonymous]
		[OverrideAuthentication]
		[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
		public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
		{
			if (error != null)
				return BadRequest(Uri.EscapeDataString(error));

			if (User.Identity.IsAuthenticated == false)
				return new ExternalChallengeResult(provider, this);

			var redirectUri = string.Empty;
			var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref redirectUri);
			if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
				return BadRequest(redirectUriValidationResult);

			var identity = User.Identity as ClaimsIdentity;
			if (identity == null)
				return InternalServerError();

			var providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
			if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer) || String.IsNullOrEmpty(providerKeyClaim.Value))
				return InternalServerError();

			if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
				return InternalServerError();

			if (providerKeyClaim.Issuer != provider)
			{
				Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
				return new ExternalChallengeResult(provider, this);
			}

			var accountKey = _accounts.GetExternalAccountKey(providerKeyClaim.Issuer, providerKeyClaim.Value);
			if (accountKey == null)
			{
				_accounts.CreateExternalAccount(providerKeyClaim.Issuer, providerKeyClaim.Value, identity.FindFirstValue(ClaimTypes.Name), identity.FindFirstValue(ClaimTypes.Email));
			}

			var externalToken = identity.FindFirstValue("ExternalAccessToken");
			redirectUri = string.Format("{0}#/login/provider={1}&external_access_token={2}", redirectUri, provider, externalToken);

			return Redirect(redirectUri);

		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
		{

			if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
			{
				return BadRequest("Provider or external access token is not sent");
			}

			var config = _authConfigs.FirstOrDefault(c => c.MatchesProviderName(provider));
			if (config == null)
			{
				return BadRequest("Invalid Provider");
			}

			var providerKey = config.GetVerifiedUserId(externalAccessToken);
			if (providerKey == null)
			{
				return BadRequest("Invalid Provider or External Access Token");
			}


			var accountKey = _accounts.GetExternalAccountKey(provider, providerKey);
			var identity = ((IIdentityProvider)_accounts).GetClaims(OAuthDefaults.AuthenticationType, provider, providerKey);
			var tokenExpiration = _oAuthServerOptions.AccessTokenExpireTimeSpan;
			var props = new AuthenticationProperties()
			{
				IssuedUtc = DateTime.UtcNow,
				ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
			};


			var ticket = new AuthenticationTicket(identity, props);

			var accessToken = _oAuthBearerOptions.AccessTokenFormat.Protect(ticket);

			var tokenResponse = new JObject(
				new JProperty("userName", accountKey),
				new JProperty("access_token", accessToken),
				new JProperty("token_type", "bearer"),
				new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
				new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
				new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
			);

			foreach (var listener in _listeners)
			{
				listener.LoginSuccessfull(provider, providerKey, externalAccessToken);
			}


			return Ok(tokenResponse);

		}

		private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
		{

			Uri redirectUri;

			var redirectUriString = GetQueryString(Request, "redirect_uri");

			if (string.IsNullOrWhiteSpace(redirectUriString))
			{
				return "redirect_uri is required";
			}

			bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

			if (!validUri)
			{
				return "redirect_uri is invalid";
			}

			var clientId = GetQueryString(Request, "client_id");

			if (string.IsNullOrWhiteSpace(clientId))
			{
				return "client_Id is required";
			}


			//TODO: implement allowed origin
			//var client = _repo.FindClient(clientId);

			//if (client == null)
			//{
			//	return string.Format("Client_id '{0}' is not registered in the system.", clientId);
			//}

			//if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
			//{
			//	return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
			//}

			redirectUriOutput = redirectUri.AbsoluteUri;

			return string.Empty;

		}

		private string GetQueryString(HttpRequestMessage request, string key)
		{
			var queryStrings = request.GetQueryNameValuePairs();

			if (queryStrings == null) return null;

			var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

			if (string.IsNullOrEmpty(match.Value)) return null;

			return match.Value;
		}


		private readonly IExternalAccountProvider _accounts;
		private readonly IEnumerable<IExternalOAuthConfiguration> _authConfigs;
		private readonly IEnumerable<IExternalLoginListener> _listeners;
		private readonly OAuthBearerAuthenticationOptions _oAuthBearerOptions;
		private readonly OAuthAuthorizationServerOptions _oAuthServerOptions;

	}
}