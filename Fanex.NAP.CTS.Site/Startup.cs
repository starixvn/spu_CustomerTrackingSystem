[assembly: Microsoft.Owin.OwinStartupAttribute(typeof(Fanex.NAP.CTS.Websites.Startup))]

namespace Fanex.NAP.CTS.Websites
{
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}