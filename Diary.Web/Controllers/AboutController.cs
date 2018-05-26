using System.Web.Mvc;

namespace Diary.Web.Controllers
{
    public class AboutController : DiaryControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}