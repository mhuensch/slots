using Microsoft.Owin.Security.Google;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Foogenda.Framework
{
	public class GoogleAuthProvider : IGoogleOAuth2AuthenticationProvider
	{
		void IGoogleOAuth2AuthenticationProvider.ApplyRedirect(GoogleOAuth2ApplyRedirectContext context)
		{
			context.Response.Redirect(context.RedirectUri);
		}

		Task IGoogleOAuth2AuthenticationProvider.Authenticated(GoogleOAuth2AuthenticatedContext context)
		{
			context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
			return Task.FromResult<object>(null);
		}

		Task IGoogleOAuth2AuthenticationProvider.ReturnEndpoint(GoogleOAuth2ReturnEndpointContext context)
		{
			return Task.FromResult<object>(null);
		}
	}
}