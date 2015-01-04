using Owin;

namespace Foogenda.Framework
{
	public interface IExternalOAuthConfiguration
	{
		void Install(IAppBuilder app);

		bool MatchesProviderName(string provider);

		string GetVerifiedUserId(string accessToken);
	}
}
