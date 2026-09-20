using System.Linq;
using System.Web.Mvc;
using Elearningplatform.Data;
using Elearningplatform.Models;

namespace Elearningplatform.Controllers
{
    public class ViewQuestionsController : Controller
    {
        private ElearningDbContext db = new ElearningDbContext();

        public ActionResult Index(int quizId)
        {
            var questions = db.Questions
                              .Where(q => q.QuizId == quizId)
                              .ToList();

            return View(questions);
        }
    }
}