using System.Web.Mvc;
using System.Web.Security;

namespace Elearningplatform.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                Session?.Clear();
                ViewBag.IsAuthenticated = false;
                ViewBag.Role = null;
                ViewBag.Username = null;
            }
            else
            {
                ViewBag.IsAuthenticated = true;
                ViewBag.Role = Session?["Role"];
                ViewBag.Username = Session?["Username"];
            }

            return View();
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            var role = Session?["Role"] as string;
            var username = Session?["Username"] as string;

            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username))
            {
                FormsAuthentication.SignOut();
                Session?.Clear();
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Role = role;
            ViewBag.Username = username;
            return View();
        }
    }
}