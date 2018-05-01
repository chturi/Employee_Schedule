using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Employee_Schedule.Startup))]
namespace Employee_Schedule
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
