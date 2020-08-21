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

        public HomeController(ILogger<HomeController> logger,
                                IBlogRepository blogRepository,
                                UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _blogRepository = blogRepository;
        }

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
                //var author = await userManager.FindByIdAsync(blog.UserId);
                ApplicationUser author = new ApplicationUser() { UserName = "DemoUser", Id = blog.UserId };

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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
