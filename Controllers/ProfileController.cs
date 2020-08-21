using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogary.Models;
using Blogary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blogary.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IBlogRepository _blogRepository;

        public ProfileController(UserManager<ApplicationUser> userManager,
                                IBlogRepository blogRepository)
        {
            this.userManager = userManager;
            _blogRepository = blogRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Author(string username)
        {
            var author = await userManager.FindByNameAsync(username);

            IEnumerable<Blog> blogs = _blogRepository.GetAllBlogs();
            List<Blog> writtenBlogs = new List<Blog>();

            if (author == null)
            {
                // User not found page
                ViewBag.ErrorTitle = "User Not found";
                ViewBag.ErrorMessage = $"User with Username {username} cannot be found";
                return View("Error");
            }

            // User's blogs
            foreach (Blog blog in blogs)
            {
                if (blog.UserId == author.Id)
                {
                    writtenBlogs.Add(blog);
                }
            }

            // Populate model
            AuthorProfileViewModel model = new AuthorProfileViewModel
            {
                Id = author.Id,
                User = author,
                Blogs = writtenBlogs
            };

            ViewBag.Title = username + "\'s profile";

            // Alerts
            if (TempData["Alert"] != null && TempData["AlertClass"] != null)
            {
                ViewBag.Alert = TempData["Alert"].ToString();
                ViewBag.AlertClass = TempData["AlertClass"].ToString();
            }

            return View(model);
        }


    }
}
