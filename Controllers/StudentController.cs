using System;
using System.Linq;
using System.Web.Mvc;
using Elearningplatform.Models;
using Elearningplatform.Data;
using System.Web.Security;
using System.Data.Entity;

namespace Elearningplatform.Controllers
{
    public class StudentController : Controller
    {
        private ElearningDbContext db = new ElearningDbContext();

        /// <summary>
        /// Checks if the current session belongs to a student.
        /// </summary>
        private ActionResult CheckStudentSession()
        {
            var userId = Session["UserId"] as int?;
            var role = Session["Role"] as string;

            if (userId == null || role != "Student")
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                return RedirectToAction("Login", "Account");
            }
            return null;
        }

        public ActionResult Index()
        {
            var check = CheckStudentSession();
            if (check != null) return check;

            ViewBag.Username = Session["Username"];
            return View();
        }

        public ActionResult AvailableCourses()
        {
            var check = CheckStudentSession();
            if (check != null) return check;

            var courses = db.Courses.Include(c => c.Teacher).Where(c => c.IsPublished).ToList();
            ViewBag.Courses = courses;
            return View();
        }

        public ActionResult Enroll(int? courseId)
        {
            var check = CheckStudentSession();
            if (check != null) return check;

            if (courseId == null)
            {
                return RedirectToAction("MyCourses");
            }

            var userId = Session["UserId"] as int?;
            if (userId == null) return RedirectToAction("Login", "Account");

            int studentId = userId.Value;

            var existingEnrollment = db.Enrollments
                .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

            if (existingEnrollment == null)
            {
                Enrollment enrollment = new Enrollment();
                enrollment.StudentId = studentId;
                enrollment.CourseId = courseId.Value;
                enrollment.EnrollmentDate = DateTime.Now;

                db.Enrollments.Add(enrollment);
                db.SaveChanges();
            }

            return RedirectToAction("MyCourses");
        }

        public ActionResult MyCourses()
        {
            var check = CheckStudentSession();
            if (check != null) return check;

            var userId = Session["UserId"] as int?;
            if (userId == null) return RedirectToAction("Login", "Account");

            int studentId = userId.Value;
            var enrollments = db.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId)
                .ToList();

            ViewBag.Enrollments = enrollments;
            return View();
        }

        public ActionResult CourseDetails(int courseId)
        {
            var check = CheckStudentSession();
            if (check != null) return check;

            var course = db.Courses.Find(courseId);
            if (course == null) return HttpNotFound();

            var instructor = db.Users.Find(course.TeacherId);
            var videos = db.VideoCourseContents.Where(v => v.CourseId == courseId && v.IsProcessed).ToList();

            // Calculate total duration
            int totalMinutes = 0;
            foreach (var video in videos)
            {
                if (video.Duration.HasValue)
                {
                    totalMinutes += video.Duration.Value;
                }
            }

            string totalDuration = "";
            if (totalMinutes > 0)
            {
                int hours = totalMinutes / 60;
                int minutes = totalMinutes % 60;
                if (hours > 0)
                {
                    totalDuration = hours + "h " + minutes + "m";
                }
                else
                {
                    totalDuration = minutes + " minutes";
                }
            }
            else
            {
                totalDuration = "Self-paced";
            }

            ViewBag.Course = course;
            ViewBag.Instructor = instructor != null ? instructor.Username : "Unknown";
            ViewBag.Videos = videos;
            ViewBag.TotalDuration = totalDuration;
            ViewBag.VideoCount = videos.Count;

            return View();
        }

        public ActionResult CourseVideos(int? courseId)
        {
            var check = CheckStudentSession();
            if (check != null) return check;

            if (courseId == null)
            {
                return RedirectToAction("MyCourses");
            }

            var userId = Session["UserId"] as int?;
            if (userId == null) return RedirectToAction("Login", "Account");

            int studentId = userId.Value;

            bool isEnrolled = db.Enrollments
                .Any(e => e.StudentId == studentId && e.CourseId == courseId);

            if (!isEnrolled) return RedirectToAction("MyCourses");

            var videos = db.VideoCourseContents
                .Where(v => v.CourseId == courseId && v.IsProcessed)
                .ToList();

            ViewBag.CourseId = courseId;
            ViewBag.Videos = videos;
            return View();
        }

        public ActionResult WatchVideo(int? videoContentId)
        {
            var check = CheckStudentSession();
            if (check != null) return check;

            if (videoContentId == null)
            {
                return RedirectToAction("MyCourses");
            }

            var userId = Session["UserId"] as int?;
            if (userId == null) return RedirectToAction("Login", "Account");

            int studentId = userId.Value;

            var video = db.VideoCourseContents.Find(videoContentId);
            if (video == null) return HttpNotFound();

            bool isEnrolled = db.Enrollments
                .Any(e => e.StudentId == studentId && e.CourseId == video.CourseId);

            if (!isEnrolled) return RedirectToAction("MyCourses");

            return View(video);
        }

        public ActionResult AvailableQuizzes(int? courseId)
        {
            var check = CheckStudentSession();
            if (check != null) return check;

            if (courseId == null)
            {
                return RedirectToAction("MyCourses");
            }

            var quizzes = db.Quizzes.Where(q => q.CourseId == courseId).ToList();
            ViewBag.Quizzes = quizzes;
            ViewBag.CourseId = courseId;
            return View();
        }

        public ActionResult StartQuiz(int? quizId)
        {
            var check = CheckStudentSession();
            if (check != null) return check;

            if (quizId == null)
            {
                return RedirectToAction("MyCourses");
            }

            var quiz = db.Quizzes.Find(quizId);
            if (quiz == null) return HttpNotFound();

            var questions = db.Questions.Where(q => q.QuizId == quizId).ToList();

            ViewBag.Quiz = quiz;
            ViewBag.Questions = questions;
            ViewBag.QuestionsCount = questions.Count;

            return View();
        }

        [HttpPost]
        public ActionResult SubmitQuiz()
        {
            var check = CheckStudentSession();
            if (check != null) return check;

            int quizId = Convert.ToInt32(Request.Form["quizId"]);
            var questions = db.Questions.Where(q => q.QuizId == quizId).ToList();

            int score = 0;

            foreach (var question in questions)
            {
                string userAnswer = Request.Form["q_" + question.QuestionId];
                if (userAnswer != null && userAnswer == question.CorrectAnswer)
                {
                    score++;
                }
            }

            int totalQuestions = questions.Count;
            int percentage = (score * 100) / totalQuestions;
            bool isPassed = percentage >= 40;

            // Save result
            Result result = new Result();
            result.UserId = Convert.ToInt32(Session["UserId"]);
            result.QuizId = quizId;
            result.Score = score;
            result.AttemptDate = DateTime.Now;
            result.IsPassed = isPassed;

            db.Results.Add(result);
            db.SaveChanges();

            ViewBag.Score = score;
            ViewBag.TotalQuestions = totalQuestions;
            ViewBag.Percentage = percentage;
            ViewBag.IsPassed = isPassed;
            ViewBag.Quiz = db.Quizzes.Find(quizId);

            return View();
        }

        public ActionResult GenerateCertificate(int courseId)
        {
            var check = CheckStudentSession();
            if (check != null) return check;

            int userId = Convert.ToInt32(Session["UserId"]);

            // Check if quiz passed
            var quiz = db.Quizzes.FirstOrDefault(q => q.CourseId == courseId);
            bool quizPassed = false;

            if (quiz != null)
            {
                var result = db.Results.FirstOrDefault(r => r.UserId == userId && r.QuizId == quiz.QuizId);
                quizPassed = (result != null && result.IsPassed);
            }

            if (!quizPassed)
            {
                TempData["ErrorMessage"] = "You must pass the quiz first to get certificate!";
                return RedirectToAction("MyCourses");
            }

            // Check if certificate already exists
            var existingCert = db.Certificates
                .FirstOrDefault(c => c.UserId == userId && c.CourseId == courseId);

            if (existingCert == null)
            {
                Certificate cert = new Certificate();
                cert.UserId = userId;
                cert.CourseId = courseId;
                cert.IssueDate = DateTime.Now;
                cert.CertificateNumber = "CERT-" + userId + "-" + courseId + "-" + DateTime.Now.Ticks;

                db.Certificates.Add(cert);
                db.SaveChanges();

                ViewBag.Certificate = cert;
            }
            else
            {
                ViewBag.Certificate = existingCert;
            }

            var course = db.Courses.Find(courseId);
            var user = db.Users.Find(userId);

            ViewBag.Course = course;
            ViewBag.StudentName = user.Username;

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}