using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogary.Models
{
    public class MockBlogRepository : IBlogRepository
    {
        private List<Blog> _blogList;

        public MockBlogRepository()
        {
            _blogList = new List<Blog>()
            {
                new Blog() {
                    Id = 1,
                    Approved = true,
                    Title = "",
                    BriefDescription = "Brief Description of Blog",
                    BlogContent = "Blog Contents",
                    Date = DateTime.Today,
                    Topic = Topic.Technology,
                    UserId = "0"
                }
            };
        }

        public Blog Add(Blog blog)
        {
            blog.Id = _blogList.Count + 1;
            _blogList.Add(blog);
            return blog;
        }

        public Blog Delete(int Id)
        {
            Blog blog = _blogList.FirstOrDefault(e => e.Id == Id);
            if (blog != null)
            {
                _blogList.Remove(blog);
            }

            return blog;
        }

        public IEnumerable<Blog> GetAllBlogs()
        {
            return _blogList;
        }

        public Blog GetBlog(int Id)
        {
            return _blogList.FirstOrDefault(e => e.Id == Id);
        }

        public Blog Update(Blog blogChanged)
        {
            Blog blog = _blogList.FirstOrDefault(e => e.Id == blogChanged.Id);
            if (blog != null)
            {
                blog.Approved = false;
                blog.BlogContent = blogChanged.BlogContent;
                blog.BriefDescription = blogChanged.BriefDescription;
                blog.Title = blogChanged.Title;
                blog.Topic = blogChanged.Topic;
                blog.Date = blogChanged.Date;
            }

            return blog;
        }

    }
}
