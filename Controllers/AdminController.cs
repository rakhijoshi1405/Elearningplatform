using System.Linq;
using System.Web.Mvc;
using Elearningplatform.Models;
using Elearningplatform.Data;
using System.Web.Security;

namespace Elearningplatform.Controllers
{
    public class AdminController : Controller
    {
        private ElearningDbContext db = new ElearningDbContext();

        private ActionResult CheckAdminSession()
        {
            var userId = Session["UserId"] as int?;
            var role = Session["Role"] as string;

            if (userId == null || role != "Admin")
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                return RedirectToAction("Login", "Account");
            }
            return null;
        }

        public ActionResult Index()
        {
            var check = CheckAdminSession();
            if (check != null) return check;

            // Stats
            ViewBag.TotalUsers = db.Users.Count();
            ViewBag.TotalTeachers = db.Users.Count(u => u.Role == "Teacher");
            ViewBag.TotalStudents = db.Users.Count(u => u.Role == "Student");
            ViewBag.TotalCourses = db.Courses.Count();
            ViewBag.TotalQuizzes = db.Quizzes.Count();
            ViewBag.TotalQuestions = db.Questions.Count();
            ViewBag.PendingTeachers = db.Users.Count(u => u.Role == "Teacher" && !u.IsApproved);

            // Separate lists
            ViewBag.Admins = db.Users.Where(u => u.Role == "Admin").OrderBy(u => u.Username).ToList();
            ViewBag.TeachersList = db.Users.Where(u => u.Role == "Teacher").OrderBy(u => u.Username).ToList();
            ViewBag.StudentsList = db.Users.Where(u => u.Role == "Student").OrderBy(u => u.Username).ToList();
            ViewBag.AllCourses = db.Courses.OrderBy(c => c.Title).ToList();

            return View();
        }

        public ActionResult PendingTeachers()
        {
            var check = CheckAdminSession();
            if (check != null) return check;

            var pendingTeachers = db.Users.Where(u => u.Role == "Teacher" && !u.IsApproved).ToList();
            ViewBag.PendingTeachers = pendingTeachers;
            return View();
        }

        public ActionResult ApproveTeacher(int id)
        {
            var check = CheckAdminSession();
            if (check != null) return check;

            var teacher = db.Users.Find(id);
            if (teacher != null)
            {
                teacher.IsApproved = true;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Teacher approved successfully!";
            }
            return RedirectToAction("PendingTeachers");
        }

        public ActionResult AllTeachers()
        {
            var check = CheckAdminSession();
            if (check != null) return check;

            var allTeachers = db.Users
                .Where(u => u.Role == "Teacher")
                .OrderBy(u => u.IsApproved)
                .ThenBy(u => u.Username)
                .ToList();

            ViewBag.Teachers = allTeachers;
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}