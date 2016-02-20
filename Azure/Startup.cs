using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Azure.Startup))]

namespace Azure
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}