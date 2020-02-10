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
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888

            app.UseHangfireDashboard();
        }
    }
}
