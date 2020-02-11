using System;
using System.Threading.Tasks;
using System.Web.Http;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BridgeCare.Startup))]

namespace BridgeCare
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var dashboardPath = "/hangfire";

            app.UseHangfireServer();
            app.UseHangfireDashboard(dashboardPath);
        }
    }
}
