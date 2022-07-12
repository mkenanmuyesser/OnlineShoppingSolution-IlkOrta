using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CommerceProject.Admin.Startup))]
namespace CommerceProject.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
