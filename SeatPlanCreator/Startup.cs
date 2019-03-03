using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SeatPlanCreatorWeb.Startup))]
namespace SeatPlanCreatorWeb
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}
	}
}