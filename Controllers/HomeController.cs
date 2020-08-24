using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Blogary.Models;
using Blogary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Blogary.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogRepository _blogRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public HomeController(ILogger<HomeController> logger,
                                IBlogRepository blogRepository,
                                UserManager<ApplicationUser> userManager,
                                RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _blogRepository = blogRepository;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {

            IEnumerable<Blog> allBlogs = _blogRepository.GetAllBlogs().Where(b => b.Approved == true);

            if (allBlogs == null)
            {
                return View(new IndexViewModel
                {
                    BlogInTopic = null,
                    LatestBlog = null
                });
            }

            List<Blog> blogs = new List<Blog>();

            Dictionary<Topic, Blog> blogInTopic = new Dictionary<Topic, Blog>();

            // Get one latest blog from each topic
            foreach (Blog blog in allBlogs)
            {
                if (!blogInTopic.ContainsKey(blog.Topic))
                {
                    blogInTopic.Add(blog.Topic, blog);
                }
                else
                {
                    Blog existedBlog = blogInTopic[blog.Topic];
                    int timeDiff = DateTime.Compare(blog.Date, existedBlog.Date);

                    // Find a later blog with the same topic
                    if (timeDiff < 0)
                    {
                        blogInTopic.Remove(blog.Topic);
                        blogInTopic.Add(blog.Topic, blog);
                    }
                }
            }

            // Latest blog will be headline
            Blog latestBlog = new Blog { };

            if (blogInTopic.Count > 0)
            {
                latestBlog = blogInTopic.ElementAt(0).Value;

                foreach (KeyValuePair<Topic, Blog> entry in blogInTopic)
                {
                    int timeDiff = DateTime.Compare(latestBlog.Date, entry.Value.Date);

                    // Find a later blog with the same topic
                    if (timeDiff < 0)
                    {
                        latestBlog = entry.Value;
                    }
                }
            }

            // View model with blog contents
            IndexViewModel model = new IndexViewModel
            {
                BlogInTopic = blogInTopic,
                LatestBlog = latestBlog
            };

            return View(model);

        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ViewBlog(int BlogId)
        {
            // Get blog from DB
            var blog = _blogRepository.GetBlog(BlogId);

            if (blog == null)
            {
                ViewBag.ErrorTitle = "Blog cannot be found!";
                ViewBag.ErrorMessage = "No blog with such ID was found.";
                return View("Error");
            }
            else
            {
                var author = await userManager.FindByIdAsync(blog.UserId);

                // Unapprove blogs
                if (blog.Approved == false)
                {
                    if (User.IsInRole("Admin") || author.UserName.Equals(User.Identity.Name))
                    {
                        // Author or admin view unapproved blog
                        ViewBag.Alert = "Blog is not approved, therefore not published to public";
                        ViewBag.AlertClass = "alert-warning";
                    }
                    else
                    {
                        // Not admin and not author
                        ViewBag.ErrorTitle = "Unapproved blog!";
                        ViewBag.ErrorMessage = "The blog still awaits approval from moderators or admins.";
                        return View("Error");
                    }
                }

                // Populate ViewModel with blog data
                ViewBlogViewModel model = new ViewBlogViewModel
                {
                    Title = blog.Title,
                    BriefDescription = blog.BriefDescription,
                    Topic = blog.Topic,
                    Date = blog.Date,
                    BlogContent = blog.BlogContent.Split("\n"),
                    Username = author.UserName
                };

                return View(model);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ViewTopic(int TopicId)
        {
            Topic selectedTopic;

            try
            {
                selectedTopic = (Topic)TopicId;
            }
            catch (Exception)
            {
                ViewBag.ErrorTitle = "Invalid topic";
                ViewBag.ErrorMessage = "No topic with such ID was found.";
                return View("Error");
            }

            ViewBag.TopicTitle = selectedTopic;

            // Find matching topic
            var allBlogs = _blogRepository.GetAllBlogs().Where(x => x.Approved == true); ;
            List<Blog> blogsInTopic = allBlogs.Where(blog => blog.Topic == selectedTopic).ToList();

            // Sort by date
            List<Blog> blogsInTopicSorted = blogsInTopic.OrderBy(o => o.Date).ToList();
            return View(blogsInTopicSorted);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult SortByDate()
        {
            // Get all blog from DB
            var blogs = _blogRepository.GetAllBlogs().Where(b => b.Approved == true);

            List<Blog> sortedBlogs = blogs.OrderByDescending(x => x.Date).ToList();

            // Get 25 latest blogs
            if (sortedBlogs.Count > 25)
            {
                sortedBlogs = sortedBlogs.GetRange(0, 25);
            }

            ViewBag.SortBy = "Date";
            return View("SortBy", sortedBlogs);
        }

        // Get Create Blog Page
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateBlog(string Username)
        {
            var user = await userManager.FindByNameAsync(Username);
            if (user != null)
            {
                ViewBag.UserId = user.Id;
                return View();
            }
            else
            {
                ViewBag.ErrorTitle = "Something went wrong!";
                ViewBag.ErrorMessage = "Contact Admin";
                return View("Error");
            }
        }

        // Post Created Blog
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBlog(CreateBlogViewModel model)
        {
            if (ModelState.IsValid)
            {
                Blog blog = new Blog
                {
                    UserId = model.UserId,
                    Title = model.Title,
                    BriefDescription = model.BriefDescription,
                    Topic = model.Topic,
                    BlogContent = model.BlogContent,
                    Date = DateTime.Now,
                    Approved = false
                };

                // Add to Database
                _blogRepository.Add(blog);

                //Show all blog from user
                var user = await userManager.FindByIdAsync(model.UserId);

                // Message
                TempData["Alert"] = $"Blog with title \"{blog.Title}\" awaits approval from admin";
                TempData["AlertClass"] = "alert-warning";
                return RedirectToAction("Author", "Profile", new { userName = user.UserName });
            }

            return View(model);
        }


        // Get edit blog page
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditBlog(int BlogId)
        {
            // Get blog from DB 
            var blog = _blogRepository.GetBlog(BlogId);

            if (blog == null)
            {
                ViewBag.ErrorTitle = "Blog cannot be found!";
                ViewBag.ErrorMessage = "No blog with such ID was found.";
                return View("Error");
            }

            var author = await userManager.FindByIdAsync(blog.UserId);

            // Prevent editing someone else's blog
            if (!author.UserName.Equals(User.Identity.Name) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Build model
            EditBlogViewModel model = new EditBlogViewModel
            {
                Title = blog.Title,
                BriefDescription = blog.BriefDescription,
                Topic = blog.Topic,
                Date = blog.Date,
                BlogContent = blog.BlogContent,
                BlogId = BlogId,
                UserId = blog.UserId
            };

            return View(model);
        }

        // Post new blogs
        [Authorize]
        [HttpPost]
        public IActionResult EditBlog(EditBlogViewModel model)
        {
            if (ModelState.IsValid)
            {
                var blog = _blogRepository.GetBlog(model.BlogId);

                if (blog == null)
                {
                    ViewBag.ErrorTitle = "Error in finding the blog!";
                    ViewBag.ErrorMessage = "No blog with such ID was found.";
                    return View("Error");
                }

                // Update all fields
                blog.Title = model.Title;
                blog.BriefDescription = model.BriefDescription;
                blog.Topic = model.Topic;
                blog.BlogContent = model.BlogContent;
                blog.Date = DateTime.Today;
                blog.Approved = false;

                var updatedBlog = _blogRepository.Update(blog);
                if (updatedBlog != null)
                {
                    // Success alert
                    TempData["Alert"] = $"Blog with title \"{updatedBlog.Title}\" has been updated";
                    TempData["AlertClass"] = "alert-success";
                    return RedirectToAction("Author", "Profile", new { username = User.Identity.Name });
                }
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteBlog(int BlogId)
        {

            var blog = _blogRepository.GetBlog(BlogId);

            // Invalid blog
            if (blog == null)
            {
                ViewBag.ErrorTitle = "Error in deleting the blog!";
                ViewBag.ErrorMessage = "No blog with such ID was found.";
                return View("Error");
            }

            var author = await userManager.FindByIdAsync(blog.UserId);

            // Not delete by author
            if (blog.UserId != author.Id)
            {
                return View("~/Views/Account/AccessDenied.cshtml");
            }

            // Delete blog
            var deletedBlog = _blogRepository.Delete(BlogId);

            // Success alert
            //TempData["Alert"] = $"Blog with title \"{deletedBlog.Title}\"  has been deleted";
            //TempData["AlertClass"] = "alert-success";
            return RedirectToAction("Author", "Profile", new { userName = User.Identity.Name });

        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> AuthorList()
        {
            // Get all user id from blog list
            List<Blog> ApprovedBlogs = _blogRepository.GetAllBlogs().Where(x => x.Approved == true).ToList();
            List<string> userIds = ApprovedBlogs.Select(x => x.UserId).ToList();

            // Remove duplicates
            HashSet<string> uniqueIds = new HashSet<string>();

            foreach (string userId in userIds)
            {
                uniqueIds.Add(userId);
            }

            List<ApplicationUser> authors = new List<ApplicationUser>();

            // Get username
            foreach (string userId in uniqueIds)
            {
                var author = await userManager.FindByIdAsync(userId);
                if (author != null)
                {
                    authors.Add(author);
                }
            }

            return View(authors);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AdminDeleteBlog(int BlogId)
        {
            var blog = _blogRepository.GetBlog(BlogId);

            // Invalid blog
            if (blog == null)
            {
                ViewBag.ErrorTitle = "Error in deleting the blog!";
                ViewBag.ErrorMessage = "No blog with such ID was found.";
                return View("Error");
            }

            // Not admin
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Delete Blog
            var deletedBlog = _blogRepository.Delete(BlogId);

            TempData["Alert"] = $"Blog with title \"{deletedBlog.Title}\" has been deleted";
            TempData["AlertClass"] = "alert-success";
            return RedirectToAction("BlogsPending", "Home");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult BlogsPending()
        {
            // Get all unapprove blog
            var unapprovedBlogs = _blogRepository.GetAllBlogs().Where(b => b.Approved == false).ToList();

            // Show success alert if any
            if (TempData["Alert"] != null && TempData["AlertClass"] != null)
            {
                ViewBag.Alert = TempData["Alert"].ToString();
                ViewBag.AlertClass = TempData["AlertClass"].ToString();
            }

            return View(unapprovedBlogs);
        }

        // Approve blog post
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult ApproveBlog(int BlogId)
        {
            Blog blog = _blogRepository.GetBlog(BlogId);

            // Invalid ID
            if (blog == null)
            {
                ViewBag.ErrorTitle = "Error in approving the blog!";
                ViewBag.ErrorMessage = "No blog with such ID was found.";
                return View("Error");
            }

            blog.Approved = true;

            // Update DB
            _blogRepository.Update(blog);

            // Alert
            TempData["Alert"] = $"Blog with title \"{blog.Title}\" has been approved";
            TempData["AlertClass"] = "alert-success";
            return RedirectToAction("BlogsPending");
        }

    }
}
