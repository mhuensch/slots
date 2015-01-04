using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Foogenda.Framework;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Owin;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Cors;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;

[assembly: OwinStartup(typeof(Foogenda.WebHost.Startup))]
namespace Foogenda.WebHost
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			RegisterWebRoutes();
			RegisterApiRoutes();
			RegisterFilters();

			var container = BuildIocContainer();
			ConfigureOAuth(app, container);
			Configure(container);

			GlobalConfiguration.Configuration.EnsureInitialized();
		}

		private static void RegisterWebRoutes()
		{
			RouteTable.Routes.LowercaseUrls = true;
			RouteTable.Routes.MapPageRoute("Root", "", "~/brochure/index.html");
		}

		private static void RegisterApiRoutes()
		{
			var configuration = GlobalConfiguration.Configuration;

			configuration.MapHttpAttributeRoutes();

			configuration.Routes.MapHttpRoute(
				name: "Registration",
				routeTemplate: "api/register",
				defaults: new { controller = "accounts", action = "register" }
			);

			configuration.Routes.MapHttpRoute(
				name: "Default",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			configuration.Routes.MapHttpRoute(
				name: "ByOwner",
				routeTemplate: "api/{controller}/{owner}/{key}",
				defaults: new { id = RouteParameter.Optional }
			);
		}

		private static void RegisterFilters()
		{
			GlobalFilters.Filters.Add(new HandleErrorAttribute());
		}

		private static IWindsorContainer BuildIocContainer()
		{
			var path = Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).LocalPath);
			var assemblyFilter = new AssemblyFilter(path);
			var container = new WindsorContainer();

			container.Kernel.ComponentModelCreated += OnComponentModelCreated;
			container.Install(FromAssembly.InDirectory(assemblyFilter));

			container.Register(Component.For<ExternalAuthController>().ImplementedBy<ExternalAuthController>());

			return container;
		}

		private static void ConfigureOAuth(IAppBuilder app, IWindsorContainer container)
		{
			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

			var externalAuthConfigs = container.ResolveAll<IExternalOAuthConfiguration>();
			try
			{
				foreach (var config in externalAuthConfigs)
					config.Install(app);
			}
			finally
			{
				foreach (var config in externalAuthConfigs)
					container.Release(config);
			}


			var oAuthBearerOptions = container.Resolve<OAuthBearerAuthenticationOptions>();
			try
			{
				app.UseOAuthBearerAuthentication(oAuthBearerOptions);
			}
			finally
			{
				container.Release(oAuthBearerOptions);
			}

			var oAuthServerOptions = container.Resolve<OAuthAuthorizationServerOptions>();
			try
			{
				app.UseOAuthAuthorizationServer(oAuthServerOptions);
			}
			finally
			{
				container.Release(oAuthServerOptions);
			}

		}

		private static void Configure(IWindsorContainer container)
		{
			var configuration = GlobalConfiguration.Configuration;
			configuration.EnableCors(new EnableCorsAttribute("*", "*", "*"));

			configuration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
			configuration.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings() { ContractResolver = new LowercaseContractResolver() };

			configuration.Filters.Add(new System.Web.Http.AuthorizeAttribute());
			configuration.Filters.Add(new Framework.RequireHttpsAttribute());

			configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorControllerActivator(container));
			configuration.Services.Replace(typeof(IHttpControllerSelector), new ApiControllerSelector(GlobalConfiguration.Configuration));
			configuration.Services.Replace(typeof(IHttpActionSelector), new ApiActionSelector());
		}

		private static void OnComponentModelCreated(ComponentModel model)
		{
			if (model.LifestyleType != LifestyleType.Undefined)
				return;

			model.LifestyleType = LifestyleType.PerWebRequest;
		}
	}
}
