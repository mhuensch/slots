using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Database.Server;
using System;
using System.IO;
using System.Linq;

namespace Foogenda.Framework
{
	public class Installer : IWindsorInstaller
	{
		void IWindsorInstaller.Install(IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
		{
			container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));
			container.Register(Component.For<ILazyComponentLoader>().ImplementedBy<LazyOfTComponentLoader>());

			container.Register(Component.For<IContext>().ImplementedBy<Context>());

			var assemblyFilter = new AssemblyFilter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"));
			container.Register(Classes.FromAssemblyInDirectory(assemblyFilter)
				.Where(type => type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRestApiController<>)).Any())
				.WithService.Self()
				.WithServiceAllInterfaces());

			container.Register(Component.For<IDocumentSession>()
				.UsingFactory<IDocumentStore, IDocumentSession>((f) => { return f.OpenSession(); }));

			container.Register(Component.For<IDocumentStore>()
				.UsingFactoryMethod<IDocumentStore>(() => { return CreateDocumentStore(); })
				.LifestyleSingleton());

			container.Register(Classes.FromAssemblyInDirectory(assemblyFilter)
				.Where(t => t.GetInterface(typeof(IExternalOAuthConfiguration).FullName) != null)
				.WithServices(new[] { typeof(IExternalOAuthConfiguration) })
				.LifestyleSingleton());

			container.Register(Component.For<OAuthBearerAuthenticationOptions>()
				.Instance(new OAuthBearerAuthenticationOptions()).LifestyleSingleton());
			container.Register(Component.For<OAuthAuthorizationServerOptions>()
				.Instance(new OAuthAuthorizationServerOptions()
				{
					//AllowInsecureHttp = true,
					TokenEndpointPath = new PathString("/token"),
					AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
					Provider = new LocalAuthProvider(container)
				}).LifestyleSingleton());
		}

		private IDocumentStore CreateDocumentStore()
		{
			var store = default(IDocumentStore);
			if (string.IsNullOrEmpty(_ravenDbApiKey) || string.IsNullOrEmpty(_ravenDbUrl))
			{
				var local = new EmbeddableDocumentStore
				{
					DataDirectory = "Data",
					UseEmbeddedHttpServer = true
				};
				local.Configuration.Port = 9999;
				NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(9999);
				store = local;
			}
			else
			{
				var production = new DocumentStore
				{
					ApiKey = _ravenDbApiKey,
					Url = _ravenDbUrl
				};
				store = production;
			}

			store.Conventions.DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
			store.Initialize();
			return store;
		}

		private readonly string _ravenDbApiKey = Environment.GetEnvironmentVariable("RAVENDB_APIKEY");
		private readonly string _ravenDbUrl = Environment.GetEnvironmentVariable("RAVENDB_URL");
	}
}