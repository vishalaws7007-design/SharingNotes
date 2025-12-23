using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharingNotes.Models;
using SharingNotes.Service;
using System.Security.Claims;

namespace SharingNotes.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        /* ================= LOGIN ================= */

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Login login)
        {
            if (!ModelState.IsValid)
                return View(login);

            var user = await _context.signupViewModels
                .FirstOrDefaultAsync(x => x.Username.ToLower() == login.Username.ToLower());

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(login);
            }

            var hasher = new PasswordHasher<SignupViewModel>();
            var result = hasher.VerifyHashedPassword(user, user.Password, login.Password);

            if (result != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(login);
            }

            /* 🔐 COOKIE AUTHENTICATION */
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true, // remember login
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                });

            /* OPTIONAL: session for UI */
            HttpContext.Session.SetString("UserName", user.Username);

            return RedirectToAction("Index", "Home");
        }

        /* ================= SIGNUP ================= */

        [HttpGet]
        public IActionResult SignUP()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new SignupViewModel
            {
                Name = model.Name,
                Username = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Age = model.Age
            };

            var hasher = new PasswordHasher<SignupViewModel>();
            user.Password = hasher.HashPassword(user, model.Password);
            user.ConfirmPassword = hasher.HashPassword(user, model.ConfirmPassword);

            _context.signupViewModels.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Account created successfully!";
            return RedirectToAction("SignupSuccess");
        }

        public IActionResult SignupSuccess()
        {
            return View();
        }

        /* ================= FORGOT PASSWORD ================= */

        [HttpGet]
        public IActionResult Forget()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Forget(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = Guid.NewGuid().ToString();

            var resetLink = Url.Action(
                "ResetPassword",
                "Auth",
                new { token = token, email = model.Email },
                Request.Scheme
            );

            var emailService = new EmailService();
            emailService.SendPasswordResetEmail(model.Email, resetLink);

            TempData["SuccessMessage"] = "Password reset link has been sent to your email.";
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /* ================= RESET PASSWORD ================= */

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return BadRequest("Invalid token");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            TempData["SuccessMessage"] = "Your password has been reset successfully!";
            return RedirectToAction("Index");
        }

        /* ================= LOGOUT ================= */

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Index", "Auth");
        }
    }
}
