using System;
using System.Linq;
using System.Web.Http.Controllers;

namespace Foogenda.Framework
{

	public class ApiActionSelector : ApiControllerActionSelector
	{

		public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
		{
			//TODO: fix this to not use TRY/CATCH
			try
			{
				return CreateDescriptor(controllerContext, null);
			}
			catch
			{
				var interfaceType = (
					from i in controllerContext.ControllerDescriptor.ControllerType.GetInterfaces()
					where i.IsGenericType
						&& i.GetGenericTypeDefinition() == typeof(IRestApiController<>)
					select i).First();

				return CreateDescriptor(controllerContext, interfaceType);
			}

		}

		private HttpActionDescriptor CreateDescriptor(HttpControllerContext controllerContext, Type controllerType)
		{
			if (controllerType != null)
				controllerContext.ControllerDescriptor.ControllerType = controllerType;

			var selector = new ApiControllerActionSelector();
			var result = selector.SelectAction(controllerContext);
			return result;
		}

	}
}
