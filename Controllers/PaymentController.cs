using System;
using System.Linq;
using System.Web.Mvc;
using Elearningplatform.Data;
using Elearningplatform.Models;

namespace Elearningplatform.Controllers
{
    public class PaymentController : Controller
    {
        private ElearningDbContext db = new ElearningDbContext();

        // Payment Page Open
        public ActionResult Pay(int courseId)
        {
            var course = db.Courses.Find(courseId);

            if (course == null)
            {
                return HttpNotFound();
            }

            ViewBag.Course = course;
            return View();
        }

        // Payment Process
        [HttpPost]
        public ActionResult ProcessPayment(int courseId)
        {
            // SESSION CHECK
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "Please login first!";
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            var course = db.Courses.Find(courseId);

            if (course == null)
            {
                TempData["ErrorMessage"] = "Course not found!";
                return RedirectToAction("AvailableCourses", "Student");
            }

            // Check if already enrolled
            var existingEnrollment = db.Enrollments
                .FirstOrDefault(e => e.StudentId == userId && e.CourseId == courseId);

            if (existingEnrollment != null)
            {
                TempData["ErrorMessage"] = "You are already enrolled in this course!";
                return RedirectToAction("MyCourses", "Student");
            }

            // Payment save
            Payment payment = new Payment();
            payment.CourseId = courseId;
            payment.StudentId = userId;
            payment.Amount = course.Price;
            payment.Status = "Success";
            payment.PaymentDate = DateTime.Now;

            db.Payments.Add(payment);

            // Enrollment after payment
            Enrollment enrollment = new Enrollment();
            enrollment.CourseId = courseId;
            enrollment.StudentId = userId;
            enrollment.EnrollmentDate = DateTime.Now;

            db.Enrollments.Add(enrollment);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Payment successful! You are now enrolled.";
            return RedirectToAction("MyCourses", "Student");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}