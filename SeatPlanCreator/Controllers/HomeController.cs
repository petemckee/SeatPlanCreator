using System.Web.Mvc;

namespace SeatPlanCreatorWeb.Controllers
{
	public class HomeController : Controller
	{
		[Authorize]
		public ActionResult Index()
		{
			return RedirectToAction("Index", "Create");
		}
	}
}