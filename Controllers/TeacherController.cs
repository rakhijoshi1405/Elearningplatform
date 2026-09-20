using Elearningplatform.Data;
using Elearningplatform.Models;
using Elearningplatform.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Elearningplatform.Controllers
{
    public class TeacherController : Controller
    {
        private ElearningDbContext db = new ElearningDbContext();

        /// <summary>
        /// Checks if the current session belongs to a teacher.
        /// </summary>
        private ActionResult CheckTeacherSession()
        {
            var userId = Session["UserId"] as int?;
            var role = Session["Role"] as string;

            if (userId == null || role != "Teacher")
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                return RedirectToAction("Login", "Account");
            }
            return null;
        }

        // ================= DASHBOARD =================

        public ActionResult Index()
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            ViewBag.Username = Session["Username"];
            return View();
        }

        // ================= COURSE MANAGEMENT =================

        public ActionResult MyCourses()
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            var userId = Session["UserId"] as int?;
            if (userId == null) return RedirectToAction("Login", "Account");

            int teacherId = userId.Value;
            var courses = db.Courses.Where(c => c.TeacherId == teacherId).ToList();
            ViewBag.Courses = courses;
            return View();
        }

        // GET: Create Course Form
        public ActionResult CreateCourse()
        {
            var check = CheckTeacherSession();
            if (check != null) return check;
            return View();
        }

        // POST: Create Course Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCourseSubmit()
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            var userId = Session["UserId"] as int?;
            if (userId == null) return RedirectToAction("Login", "Account");

            string title = Request.Form["Title"];
            string description = Request.Form["Description"];
            decimal price = Convert.ToDecimal(Request.Form["Price"]);

            if (string.IsNullOrEmpty(title))
            {
                ViewBag.ErrorMessage = "Title is required";
                return View("CreateCourse");
            }

            Course course = new Course();
            course.Title = title;
            course.Description = description;
            course.Price = price;
            course.TeacherId = userId.Value;
            course.CreatedDate = DateTime.Now;
            course.IsPublished = true;

            db.Courses.Add(course);
            db.SaveChanges();

            return RedirectToAction("MyCourses");
        }

        // ================= VIDEO MANAGEMENT =================

        // GET: Upload Video Form
        public ActionResult UploadVideo(int? courseId)
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            if (courseId == null)
            {
                return RedirectToAction("MyCourses");
            }

            var userId = Session["UserId"] as int?;
            if (userId == null) return RedirectToAction("Login", "Account");

            int teacherId = userId.Value;
            var course = db.Courses.FirstOrDefault(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null) return HttpNotFound();

            ViewBag.CourseId = courseId;
            ViewBag.CourseTitle = course.Title;
            return View();
        }

        // POST: Upload Video Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadVideoSubmit(int? courseId)
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            if (courseId == null)
            {
                return RedirectToAction("MyCourses");
            }

            var userId = Session["UserId"] as int?;
            if (userId == null) return RedirectToAction("Login", "Account");

            HttpPostedFileBase videoFile = Request.Files["videoFile"];

            if (videoFile == null || videoFile.ContentLength == 0)
            {
                ViewBag.ErrorMessage = "Please select a video file.";
                ViewBag.CourseId = courseId;
                return View("UploadVideo");
            }

            int teacherId = userId.Value;
            var course = db.Courses.FirstOrDefault(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null) return HttpNotFound();

            string title = Request.Form["Title"];
            string description = Request.Form["Description"];

            var uploadPath = Server.MapPath("~/Content/Uploads/");
            var uploadResult = VideoUploader.UploadVideo(videoFile, uploadPath);

            if (!uploadResult.IsSuccess)
            {
                ViewBag.ErrorMessage = uploadResult.ErrorMessage;
                ViewBag.CourseId = courseId;
                return View("UploadVideo");
            }

            VideoCourseContent videoContent = new VideoCourseContent();
            videoContent.CourseId = courseId.Value;
            videoContent.Title = title;
            videoContent.Description = description;
            videoContent.VideoFileName = uploadResult.VideoFileName;
            videoContent.VideoFilePath = uploadResult.VideoFilePath;
            videoContent.ThumbnailPath = uploadResult.ThumbnailPath;
            videoContent.FileSize = uploadResult.FileSize;
            videoContent.UploadDate = DateTime.Now;
            videoContent.IsProcessed = true;

            db.VideoCourseContents.Add(videoContent);
            db.SaveChanges();

            return RedirectToAction("CourseVideos", new { courseId = courseId });
        }

        public ActionResult CourseVideos(int? courseId)
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            if (courseId == null)
            {
                return RedirectToAction("MyCourses");
            }

            var userId = Session["UserId"] as int?;
            if (userId == null) return RedirectToAction("Login", "Account");

            int teacherId = userId.Value;
            var course = db.Courses.FirstOrDefault(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null) return HttpNotFound();

            var videos = db.VideoCourseContents.Where(v => v.CourseId == courseId).ToList();

            ViewBag.CourseId = courseId;
            ViewBag.CourseTitle = course.Title;
            ViewBag.Videos = videos;

            return View();
        }

        [HttpPost]
        public JsonResult DeleteVideo(int videoContentId)
        {
            var check = CheckTeacherSession();
            if (check != null) return Json(new { success = false, message = "Session expired" });

            var video = db.VideoCourseContents.Find(videoContentId);
            if (video == null) return Json(new { success = false, message = "Video not found" });

            db.VideoCourseContents.Remove(video);
            db.SaveChanges();

            return Json(new { success = true });
        }

        // ================= QUIZ MANAGEMENT =================

        // GET: Create Quiz Form
        public ActionResult CreateQuiz(int? courseId)
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            if (courseId == null)
            {
                return RedirectToAction("MyCourses");
            }

            ViewBag.CourseId = courseId;
            return View();
        }

        // POST: Create Quiz Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateQuizSubmit()
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            int courseId = Convert.ToInt32(Request.Form["CourseId"]);
            string title = Request.Form["Title"];

            Quiz quiz = new Quiz();
            quiz.CourseId = courseId;
            quiz.Title = title;

            db.Quizzes.Add(quiz);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Quiz created successfully! Now add questions.";
            return RedirectToAction("AddQuestions", new { quizId = quiz.QuizId });
        }

        // GET: Add Questions Form
        public ActionResult AddQuestions(int? quizId)
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            if (quizId == null)
            {
                return RedirectToAction("MyCourses");
            }

            var quiz = db.Quizzes.Find(quizId);
            if (quiz == null) return HttpNotFound();

            ViewBag.QuizId = quizId;
            ViewBag.CourseId = quiz.CourseId;
            ViewBag.QuizTitle = quiz.Title;

            return View();
        }

        // POST: Add Questions Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuestionsSubmit()
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            int quizId = Convert.ToInt32(Request.Form["QuizId"]);
            string questionText = Request.Form["QuestionText"];
            string optionA = Request.Form["OptionA"];
            string optionB = Request.Form["OptionB"];
            string optionC = Request.Form["OptionC"];
            string optionD = Request.Form["OptionD"];
            string correctAnswer = Request.Form["CorrectAnswer"];

            Question question = new Question();
            question.QuizId = quizId;
            question.QuestionText = questionText;
            question.OptionA = optionA;
            question.OptionB = optionB;
            question.OptionC = optionC;
            question.OptionD = optionD;
            question.CorrectAnswer = correctAnswer;

            db.Questions.Add(question);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Question added successfully!";
            return RedirectToAction("AddQuestions", new { quizId = quizId });
        }

        // ================= VIEW QUESTIONS =================

        public ActionResult ViewQuestions(int? courseId, int? quizId)
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            if (courseId == null)
            {
                return RedirectToAction("MyCourses");
            }

            Quiz quiz = null;

            // If quizId is provided, use that
            if (quizId != null)
            {
                quiz = db.Quizzes.Find(quizId);
            }
            else
            {
                // Otherwise get first quiz for this course
                quiz = db.Quizzes.FirstOrDefault(q => q.CourseId == courseId);
            }

            if (quiz == null)
            {
                ViewBag.NoQuiz = true;
                ViewBag.CourseId = courseId;
                return View();
            }

            var questions = db.Questions.Where(q => q.QuizId == quiz.QuizId).ToList();

            ViewBag.QuizTitle = quiz.Title;
            ViewBag.CourseId = courseId;
            ViewBag.QuizId = quiz.QuizId;
            ViewBag.Questions = questions;
            ViewBag.QuestionsCount = questions.Count;
            ViewBag.NoQuiz = false;

            return View();
        }

        // ================= DELETE QUESTION =================

        public ActionResult DeleteQuestion(int questionId)
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            var question = db.Questions.Find(questionId);
            if (question != null)
            {
                int quizId = question.QuizId;
                var quiz = db.Quizzes.Find(quizId);
                int courseId = quiz.CourseId;

                db.Questions.Remove(question);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Question deleted successfully!";
                return RedirectToAction("ViewQuestions", new { courseId = courseId, quizId = quizId });
            }

            return RedirectToAction("MyCourses");
        }

        // ================= DELETE COURSE =================

        public ActionResult DeleteCourse(int courseId)
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            var userId = Session["UserId"] as int?;
            if (userId == null) return RedirectToAction("Login", "Account");

            int teacherId = userId.Value;
            var course = db.Courses.FirstOrDefault(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null)
            {
                TempData["ErrorMessage"] = "Course not found!";
                return RedirectToAction("MyCourses");
            }

            // Delete all related data
            var enrollments = db.Enrollments.Where(e => e.CourseId == courseId).ToList();
            db.Enrollments.RemoveRange(enrollments);

            var quizzes = db.Quizzes.Where(q => q.CourseId == courseId).ToList();
            foreach (var quiz in quizzes)
            {
                var questions = db.Questions.Where(q => q.QuizId == quiz.QuizId).ToList();
                db.Questions.RemoveRange(questions);
            }
            db.Quizzes.RemoveRange(quizzes);

            var videos = db.VideoCourseContents.Where(v => v.CourseId == courseId).ToList();
            db.VideoCourseContents.RemoveRange(videos);

            var payments = db.Payments.Where(p => p.CourseId == courseId).ToList();
            db.Payments.RemoveRange(payments);

            var certificates = db.Certificates.Where(c => c.CourseId == courseId).ToList();
            db.Certificates.RemoveRange(certificates);

            db.Courses.Remove(course);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Course deleted successfully!";
            return RedirectToAction("MyCourses");
        }

        // ================= VIEW ALL QUIZZES (FIXED) =================

        public ActionResult ViewAllQuizzes(int courseId)
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            var userId = Session["UserId"] as int?;
            if (userId == null) return RedirectToAction("Login", "Account");

            int teacherId = userId.Value;
            var course = db.Courses.FirstOrDefault(c => c.CourseId == courseId && c.TeacherId == teacherId);

            if (course == null)
            {
                return RedirectToAction("MyCourses");
            }

            var quizzes = db.Quizzes.Where(q => q.CourseId == courseId).ToList();

            ViewBag.CourseId = courseId;
            ViewBag.CourseTitle = course.Title;
            ViewBag.Quizzes = quizzes;

            return View();
        }

        // ================= DELETE QUIZ =================

        public ActionResult DeleteQuiz(int quizId)
        {
            var check = CheckTeacherSession();
            if (check != null) return check;

            var quiz = db.Quizzes.Find(quizId);
            if (quiz != null)
            {
                int courseId = quiz.CourseId;

                var questions = db.Questions.Where(q => q.QuizId == quizId).ToList();
                db.Questions.RemoveRange(questions);

                db.Quizzes.Remove(quiz);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Quiz deleted successfully!";
                return RedirectToAction("ViewAllQuizzes", new { courseId = courseId });
            }

            return RedirectToAction("MyCourses");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}