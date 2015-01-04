
namespace Foogenda.Framework
{
	public interface IExternalLoginListener
	{
		void LoginSuccessfull(string provider, string providerKey, string accessToken);
	}
}
