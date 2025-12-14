using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharingNotes.Models;
using SharingNotes.Service;

namespace SharingNotes.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(Login login)
        {
            Console.WriteLine("Email :"+login.Email);
            Console.WriteLine("PassWord :"+login.Password);

            HttpContext.Session.SetString("UserName", login.Email);


            return RedirectToAction("Index", "Home");
          
        }

        public IActionResult SignUP()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(SignupViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Add user to database
                // Example:
                // var user = new User
                // {
                //     Name = model.Name,
                //     Username = model.Username,
                //     Email = model.Email,
                //     PhoneNumber = model.PhoneNumber,
                //     Age = model.Age
                // };
                // _context.Users.Add(user);
                // await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Account created successfully!";
                return RedirectToAction("SignupSuccess");
            }

            return View(model);
        }

      
        public IActionResult SignupSuccess()
        {
            return View();
        }
        public IActionResult Forget()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Forget(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if user exists
                /*var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "No account found with this email.");
                    return View(model);
                }
                */

                // Generate reset token
                var token = Guid.NewGuid().ToString();
                //user.ResetToken = token;
                //user.ResetTokenExpiry = DateTime.Now.AddHours(1);
               // await _context.SaveChangesAsync();

                // Build reset link
                var resetLink = Url.Action("ResetPassword", "Auth",
                    new { token = token, email = model.Email }, Request.Scheme);

                // Send email using EmailService
                var _emailService =new EmailService();

                _emailService.SendPasswordResetEmail(model.Email, resetLink);

                TempData["SuccessMessage"] = "Password reset link has been sent to your email.";
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return BadRequest("Invalid token");

         /*   var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.ResetToken == token);
         
            if (user == null || user.ResetTokenExpiry < DateTime.Now)
                return View("InvalidOrExpiredToken");
         */
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Update password in database

                TempData["SuccessMessage"] = "Your password has been reset successfully!";
                return RedirectToAction("index");
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

    }
}
