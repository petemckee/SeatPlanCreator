using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer;
using SeatPlanCreatorWeb.Domain.Pocos;
using SeatPlanCreatorWeb.Domain.Services;
using Spire.Presentation;

namespace SeatPlanCreatorWeb.Controllers
{
	public partial class CreateController : Controller
	{
		[Authorize]
		public ActionResult Index()
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		public ActionResult Index(HttpPostedFileBase planData, HttpPostedFileBase template)
		{
			// TODO - Validation to check for valid xls/xlsx file

			var seatPlanCreatorService = new SeatPlanCreatorService();
			var presentationResult = seatPlanCreatorService.Create(planData, template);

			if (!presentationResult.Success)
			{
				// TODO - Return errors information
				return View();
			}

			presentationResult.Presentation.SaveToHttpResponse(
				presentationResult.FileName,
				FileFormat.Pptx2010,
				System.Web.HttpContext.Current.Response);

			System.Web.HttpContext.Current.Response.End();

			return View();
		}




		[HttpPost]
		public ActionResult IndexWithInfo(HttpPostedFileBase planData, HttpPostedFileBase template)
		{

			var seatPlanCreatorService = new SeatPlanCreatorService();
			var presentationResult = seatPlanCreatorService.CreateWithInfo(planData, template);

			presentationResult.FileData = Convert.ToBase64String(presentationResult.Presentation.GetBytes());
			//presentationResult.FileData = presentationResult.Presentation.GetStream()

			presentationResult.Presentation = null;

			return new JsonResult()
			{
				Data = presentationResult
			};
		}
	}
}