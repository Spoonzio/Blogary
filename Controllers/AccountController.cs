using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogary.Models;
using Blogary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Blogary.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(SignInManager<ApplicationUser> signInManager,
                                    UserManager<ApplicationUser> userManager,
                                    RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        // Send email using SendGrid service
        public async Task SendEmail(string toEmailAddress, string toName, string subject, string plainTextContent, string htmlContent)
        {
            string apiKey = ""; // SendGrid Api Key
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("Admin@Blogary.com", "Blogary");
            var to = new EmailAddress(toEmailAddress, toName);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }


        // Get Login Page
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Post Login Page
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.UserName);

                //Valid user with unconfirmed email
                if (user != null && !user.EmailConfirmed
                    && (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View(model);
                }

                // Validate password
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }

        // Helper for Register page
        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"The email {email} is in use.");
            }
        }

        // Helper for Register page
        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> IsUserNameInUse(string username)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"The username {username} is in use.");
            }
        }

        // Get Register Page
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Post Register Page
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Remove Email Confirmed after SendEmail is implemented
                // Create new user
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                                        new { userId = user.Id, token = token }, Request.Scheme);

                    //Send confirmation link to user's email
                    string userEmail = model.Email;
                    string userName = model.UserName;
                    string subject = "Blogary - Verify Email";
                    string plainText = $"Hi {userName}, Click the link below to verify your email. \n {confirmationLink} \n Thank you, The Blogary Team";
                    string htmlText = "Hi " + userName + ", Click the link below to verify your email. <br>"
                                        + "<a href='" + confirmationLink + "'>" + confirmationLink + "</a> <br>"
                                        + " Thank you, The Blogary Team";

                    // Remove After SendEmail is Implemented
                    //await SendEmail(userEmail, userName, subject, plainText, htmlText);

                    // Success message
                    return View("RegisterSuccess");
                }

                // Return error to view page
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // End current session and log out of account
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
