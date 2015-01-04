using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Foogenda.Framework
{

	public class ApiControllerSelector : DefaultHttpControllerSelector
	{
		public ApiControllerSelector(HttpConfiguration configuration)
			: base(configuration)
		{
			_configuration = configuration;
		}

		public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
		{
			//TODO: figure out race condition that causes this to error on the first request when the server starts up
			var controllerName = base.GetControllerName(request);
			var controllerType = ControllerTypes[controllerName];
			return new HttpControllerDescriptor(_configuration, controllerName, controllerType);
		}

		private Dictionary<string, Type> ControllerTypes
		{
			get
			{
				if (_controllerTypes != null)
					return _controllerTypes;

				_controllerTypes = new Dictionary<string, Type>();
				//TODO: this is far too slow
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					//TODO: Fix this as we shouldn't be using this but an assembly level attribute or something
					if (assembly.FullName.Contains("RestApi") == false && assembly.FullName.Contains("Foogenda") == false && assembly.FullName.Contains("Framework") == false)
						continue;
					foreach (var type in assembly.GetTypes())
					{
						if (type.GetInterface(typeof(IHttpController).FullName) == null)
							continue;

						_controllerTypes.Add(Regex.Replace(type.Name.ToLower(), @"controller\z", ""), type);
					}
				}
				return _controllerTypes;
			}
		}

		private readonly HttpConfiguration _configuration;
		private Dictionary<string, Type> _controllerTypes;
	}
}
