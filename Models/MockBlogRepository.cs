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
                    Title = "Harcoded Blog Example 1",
                    BriefDescription = "Brief Description of Blog 1",
                    BlogContent = "Blog 1 Contents",
                    Date = DateTime.Today,
                    Topic = Topic.Technology,
                    UserId = "0"
                },
                new Blog() {
                    Id = 2,
                    Approved = true,
                    Title = "Harcoded Blog Example 2",
                    BriefDescription = "Brief Description of Blog 2",
                    BlogContent = "Blog 2 Contents",
                    Date = DateTime.Today,
                    Topic = Topic.Fashion,
                    UserId = "1"
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
