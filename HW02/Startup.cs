using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(HW02.Startup))]

namespace HW02
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}