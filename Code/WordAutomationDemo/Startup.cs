using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WordAutomationDemo.Startup))]
namespace WordAutomationDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
