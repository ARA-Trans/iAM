using System;
using System.Collections.Generic;
using System.Web.Http;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;
using Unity;

[assembly: OwinStartup(typeof(BridgeCare.Startup))]

namespace BridgeCare
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = UnityConfig.Container;

            Hangfire.GlobalConfiguration.Configuration.UseActivator(new ContainerJobActivator(container));
            app.UseHangfireAspNet(GetHangfireServers);

            var dashboardPath = "/hangfire";

            app.UseHangfireDashboard(dashboardPath);
        }

        private IEnumerable<IDisposable> GetHangfireServers()
        {
            Hangfire.GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage("BridgeCareContext");

            yield return new BackgroundJobServer();
        }
        public class ContainerJobActivator : JobActivator
        {
            private IUnityContainer _container;

            public ContainerJobActivator(IUnityContainer container)
            {
                _container = container;
            }

            public override object ActivateJob(Type type)
            {
                return _container.Resolve(type);
            }
        }
    }
}
