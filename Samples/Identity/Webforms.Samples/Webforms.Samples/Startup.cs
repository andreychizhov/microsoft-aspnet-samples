using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Webforms.Samples.Startup))]
namespace Webforms.Samples
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
