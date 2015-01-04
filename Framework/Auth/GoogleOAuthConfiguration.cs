using Microsoft.Owin.Security.Google;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Owin;
using System;
using System.Net.Http;

namespace Foogenda.Framework
{
	public class GoogleOAuthConfiguration : IExternalOAuthConfiguration
	{
		void IExternalOAuthConfiguration.Install(IAppBuilder app)
		{
			var googleAuthOptions = new GoogleOAuth2AuthenticationOptions()
			{
				ClientId = _clientId,
				ClientSecret = _clientSecret,
				Provider = new GoogleAuthProvider()
			};

			//There is a bug where adding additional scopes causes goggle auth to error.
			//As a result, we are adding open id to the scope, if we decide to add the contacts scope elsewhere
			//(i.e. when requested), the addition of open id can probably be removed.  
			//See include_granted_scopes http://stackoverflow.com/questions/23943864/incremental-google-oauth-with-asp-net-owin-oauth
			//when incremental authorization becomes necessary
			googleAuthOptions.Scope.Add("openid");
			googleAuthOptions.Scope.Add("https://www.google.com/m8/feeds");


			//TODO: look at using default Google OAuth Provider
			//GoogleOAuth2AuthenticationProvider

			app.UseGoogleAuthentication(googleAuthOptions);
		}


		bool IExternalOAuthConfiguration.MatchesProviderName(string provider)
		{
			return string.Equals(provider, "Google", StringComparison.OrdinalIgnoreCase);
		}

		string IExternalOAuthConfiguration.GetVerifiedUserId(string accessToken)
		{
			var verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);

			var client = new HttpClient();
			var uri = new Uri(verifyTokenEndPoint);
			var response = client.GetAsync(uri).Result;
			if (response.IsSuccessStatusCode == false)
				return null;

			var content = response.Content.ReadAsStringAsync().Result;

			var jObj = (JObject)JsonConvert.DeserializeObject(content);

			if (_clientId != (string)jObj["audience"])
				return null;

			return (string)jObj["user_id"];
		}

		//TODO: Read these settings from the environment variables and reset the account.
		private readonly string _clientId = "307202534612-9u9eid0d2jbhaakg5or1c9br9np9mcbg.apps.googleusercontent.com";
		//Environment.GetEnvironmentVariable("RAVENDB_APIKEY");
		private readonly string _clientSecret = "Cnjn_kkH864RScx3Dc5vDTEZ";
		// Environment.GetEnvironmentVariable("RAVENDB_URL");

	}
}
