using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AppBanThuoc.Startup))]
namespace AppBanThuoc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
