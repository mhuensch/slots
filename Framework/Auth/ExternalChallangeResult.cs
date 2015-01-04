using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foogenda.Framework
{
	public class ExternalChallengeResult : IHttpActionResult
	{
		public ExternalChallengeResult(string loginProvider, ApiController controller)
		{
			_loginProvider = loginProvider;
			_request = controller.Request;
		}

		public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			_request.GetOwinContext().Authentication.Challenge(_loginProvider);

			var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
			response.RequestMessage = _request;
			return Task.FromResult(response);
		}

		private readonly string _loginProvider;
		private readonly HttpRequestMessage _request;
	}
}