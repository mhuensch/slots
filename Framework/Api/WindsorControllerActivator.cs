using Castle.Windsor;
using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Foogenda.Framework
{
	public class WindsorControllerActivator : IHttpControllerActivator
	{
		public WindsorControllerActivator(IWindsorContainer container)
		{
			_container = container;
		}

		public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
		{
			var controller = (IHttpController)_container.Resolve(controllerType);
			request.RegisterForDispose(new Release(() => _container.Release(controller)));
			return controller;
		}

		private class Release : IDisposable
		{
			private readonly Action release;

			public Release(Action release)
			{
				this.release = release;
			}

			public void Dispose()
			{
				this.release();
			}
		}

		private readonly IWindsorContainer _container;
	}
}