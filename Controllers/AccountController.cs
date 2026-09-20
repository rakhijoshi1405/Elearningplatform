using Elearningplatform.Data;
using Elearningplatform.Models;
using Elearningplatform.Utilities;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ElearningPlatform.Controllers
{
    public class AccountController : Controller
    {
        private ElearningDbContext db = new ElearningDbContext();

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user, string role)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (db.Users.Any(u => u.Username == user.Username))
                    {
                        ModelState.AddModelError("Username", "Username already exists.");
                        return View(user);
                    }

                    if (db.Users.Any(u => u.Email == user.Email))
                    {
                        ModelState.AddModelError("Email", "Email already registered.");
                        return View(user);
                    }

                    user.Password = PasswordHasher.HashPassword(user.Password);
                    user.Role = role;
                    user.IsApproved = (role == "Student" || role == "Admin");
                    user.CreatedDate = DateTime.Now;

                    db.Users.Add(user);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Registration successful! You can now login.";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error during registration: " + ex.Message);
            }

            return View(user);
        }

        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                if (Session != null)
                {
                    Session.Clear();
                    Session.Abandon();
                }
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            // DEBUG LINES - Check output in Visual Studio Output window
            System.Diagnostics.Debug.WriteLine("========================================");
            System.Diagnostics.Debug.WriteLine("=== LOGIN ATTEMPT ===");
            System.Diagnostics.Debug.WriteLine("Username entered: " + model.Username);
            System.Diagnostics.Debug.WriteLine("Password entered: " + model.Password);
            System.Diagnostics.Debug.WriteLine("========================================");

            try
            {
                if (ModelState.IsValid)
                {
                    System.Diagnostics.Debug.WriteLine("ModelState is valid");

                    var user = db.Users.FirstOrDefault(u => u.Username == model.Username);

                    if (user == null)
                    {
                        System.Diagnostics.Debug.WriteLine("USER NOT FOUND: " + model.Username);
                        TempData["ErrorMessage"] = "Invalid username or password.";
                        return RedirectToAction("Login");
                    }

                    System.Diagnostics.Debug.WriteLine("USER FOUND: " + user.Username);
                    System.Diagnostics.Debug.WriteLine("User Role: " + user.Role);
                    System.Diagnostics.Debug.WriteLine("Stored Password Hash: " + user.Password);

                    string hashedEnteredPassword = PasswordHasher.HashPassword(model.Password);
                    System.Diagnostics.Debug.WriteLine("Entered Password Hash: " + hashedEnteredPassword);

                    if (user.Password == hashedEnteredPassword)
                    {
                        System.Diagnostics.Debug.WriteLine("PASSWORD MATCHED! Login successful.");

                        if (!user.IsApproved && user.Role == "Teacher")
                        {
                            System.Diagnostics.Debug.WriteLine("Teacher account not approved yet.");
                            TempData["ErrorMessage"] = "Your account is pending approval by Admin.";
                            return RedirectToAction("Login");
                        }

                        Session["UserId"] = user.UserId;
                        Session["Role"] = user.Role;
                        Session["Username"] = user.Username;

                        FormsAuthentication.SetAuthCookie(user.Username, false);

                        System.Diagnostics.Debug.WriteLine("Session set - UserId: " + user.UserId + ", Role: " + user.Role);
                        System.Diagnostics.Debug.WriteLine("Redirecting based on role: " + user.Role);

                        // Redirect based on role
                        if (user.Role == "Admin")
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else if (user.Role == "Teacher")
                        {
                            return RedirectToAction("Index", "Teacher");
                        }
                        else if (user.Role == "Student")
                        {
                            return RedirectToAction("Index", "Student");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("PASSWORD MISMATCH!");
                        System.Diagnostics.Debug.WriteLine("Stored: " + user.Password);
                        System.Diagnostics.Debug.WriteLine("Entered Hash: " + hashedEnteredPassword);
                        TempData["ErrorMessage"] = "Invalid username or password.";
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ModelState is INVALID");
                    TempData["ErrorMessage"] = "Please enter username and password.";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            if (Session != null)
            {
                Session.Clear();
                Session.Abandon();
            }

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                HttpCookie sessionCookie = new HttpCookie("ASP.NET_SessionId");
                sessionCookie.Expires = DateTime.Now.AddDays(-1);
                sessionCookie.HttpOnly = true;
                Response.Cookies.Add(sessionCookie);
            }

            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName);
                authCookie.Expires = DateTime.Now.AddDays(-1);
                authCookie.HttpOnly = true;
                Response.Cookies.Add(authCookie);
            }

            TempData.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogoutGet()
        {
            FormsAuthentication.SignOut();
            if (Session != null)
            {
                Session.Clear();
                Session.Abandon();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult FixPasswords()
        {
            try
            {
                var allUsers = db.Users.ToList();
                int count = 0;
                foreach (var user in allUsers)
                {
                    user.Password = PasswordHasher.HashPassword("123456");
                    count++;
                }
                db.SaveChanges();
                ViewBag.Message = "Fixed " + count + " users. All users now have password: 123456";
                ViewBag.Success = true;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
                ViewBag.Success = false;
            }
            return View();
        }

        public ActionResult CreateTestUser()
        {
            try
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Username == "test");
                if (existingUser != null)
                {
                    db.Users.Remove(existingUser);
                    db.SaveChanges();
                }

                User testUser = new User();
                testUser.Username = "test";
                testUser.Email = "test@test.com";
                testUser.Password = PasswordHasher.HashPassword("123456");
                testUser.Role = "Student";
                testUser.IsApproved = true;
                testUser.CreatedDate = DateTime.Now;

                db.Users.Add(testUser);
                db.SaveChanges();

                ViewBag.Message = "Test user created successfully!";
                ViewBag.Success = true;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
                ViewBag.Success = false;
            }
            return View();
        }

        public ActionResult CreateTestTeacher()
        {
            try
            {
                var existingTeacher = db.Users.FirstOrDefault(u => u.Username == "teacher");
                if (existingTeacher != null)
                {
                    db.Users.Remove(existingTeacher);
                    db.SaveChanges();
                }

                User testTeacher = new User();
                testTeacher.Username = "teacher";
                testTeacher.Email = "teacher@test.com";
                testTeacher.Password = PasswordHasher.HashPassword("123456");
                testTeacher.Role = "Teacher";
                testTeacher.IsApproved = true;
                testTeacher.CreatedDate = DateTime.Now;

                db.Users.Add(testTeacher);
                db.SaveChanges();

                ViewBag.Message = "Test teacher created successfully!";
                ViewBag.Success = true;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
                ViewBag.Success = false;
            }
            return View();
        }

        public ActionResult DebugLogin()
        {
            try
            {
                var allUsers = db.Users.ToList();
                ViewBag.Users = allUsers;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}