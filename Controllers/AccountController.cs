using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogary.Models;
using Blogary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
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
                    Email = model.Email
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
                    await SendEmail(userEmail, userName, subject, plainText, htmlText);

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


        // Get modify page
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile(string username)
        {
            // Check if accessor is user
            if (username != User.Identity.Name)
            {
                return RedirectToAction("AccessDenied");
            }

            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                ViewBag.ErrorTitle = "User cannot be found!";
                ViewBag.ErrorMessage = "No user with such ID was found.";
                return View("Error");
            }

            // Create view model for page
            EditProfileViewModel model = new EditProfileViewModel()
            {
                Id = user.Id,
                NewUsername = username
            };

            return View(model);
        }

        // Post Edit Profile Page 
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            // Check identity
            var editor = await userManager.FindByNameAsync(User.Identity.Name);
            if (editor.Id != model.Id)
            {
                return RedirectToAction("AccessDenied");
            }

            // Edit current user
            editor.UserName = model.NewUsername;

            // Update
            var result = await userManager.UpdateAsync(editor);

            // Succeed
            if (result.Succeeded)
            {
                return RedirectToAction("Logout");
            }

            // Return error to view page
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AssignAdmin()
        {
            var role = await roleManager.FindByNameAsync("Admin");

            if (role == null)
            {
                ViewBag.ErrorTitle = $"Invalid role";
                ViewBag.ErrorMessage = $"Admin role cannot be found";
                return View("Error");
            }

            var model = new List<UserRoleViewModel>();
            var users = await userManager.Users.ToListAsync();

            // Build List of model
            foreach (var user in users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                userRoleViewModel.IsSelected = await userManager.IsInRoleAsync(user, role.Name);

                model.Add(userRoleViewModel);
            }

            if (TempData["Alert"] != null && TempData["AlertClass"] != null)
            {
                ViewBag.Alert = TempData["Alert"].ToString();
                ViewBag.AlertClass = TempData["AlertClass"].ToString();
            }


            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AssignAdmin(List<UserRoleViewModel> model)
        {
            var role = await roleManager.FindByNameAsync("Admin");

            if (role == null)
            {
                ViewBag.ErrorTitle = $"Invalid role";
                ViewBag.ErrorMessage = $"Admin role cannot be found";
                return View("Error");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    // Assigned and was not admin
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    // Unassigned and was admin
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    // State unchanged
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            // Redirect after last user
            ViewBag.Alert = "Admins have been updated";
            ViewBag.AlertClass = "alert-success";
            return View(model);
        }


        // Get Forgot Password Page
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Post for Forget Password
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string email = model.Email;

                var user = await userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    // Generate token for email
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

                    // Send reset link to user's email 
                    string userEmail = user.Email;
                    string userName = user.UserName;
                    string subject = "Blogary - Reset Password";
                    string plainText = $"Hi {userName}, click the link below to reset your password. \n {passwordResetLink} \n Thank you, The Blogary Team";
                    string htmlText = "Hi " + userName + ", click the link below to reset your password. <br>"
                                        + "<a href='" + passwordResetLink + "'>" + passwordResetLink + "</a> <br>"
                                        + " Thank you, The Blogary Team.";

                    await SendEmail(userEmail, userName, subject, plainText, htmlText);
                }

                // Show confirmation either way to prevent brute force attack
                return View("ForgetPasswordConfirmation");
            }

            return View(model);
        }

        // Access From Reset Password Link
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (email == null || token == null)
            {
                ViewBag.ErrorTitle = "Link is invalid!";
                ViewBag.ErrorMessage = "Please double check the link";
                return View("Error");
            }

            return View();
        }

        // Post for Reset Password
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    // Confirm nonetheless to prevent brute force attack
                    return View("ResetPasswordConfirmation");
                }

                // Reset password action
                var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);

                if (result.Succeeded)
                {
                    return View("ResetPasswordConfirmation");
                }
                else
                {
                    // Error in reset
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                }
            }

            return View(model);
        }

        // Confirm email for user
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            // Find user
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorTitle = $"Invalid detail";
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("Error");
            }

            // User found
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }

            // User cannot be found
            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }

    }
}
