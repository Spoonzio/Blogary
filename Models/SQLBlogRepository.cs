using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogary.Models
{
    public class SQLBlogRepository : IBlogRepository
    {
        private readonly AppDbContext context;

        public SQLBlogRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Blog Add(Blog blog)
        {
            context.Blogs.Add(blog);
            context.SaveChanges();
            return blog;
        }

        public Blog Delete(int Id)
        {
            Blog blog = context.Blogs.Find(Id);

            if (blog != null)
            {
                context.Remove(blog);
                context.SaveChanges();
            }

            return blog;
        }

        public IEnumerable<Blog> GetAllBlogs()
        {
            return context.Blogs;
        }

        public Blog GetBlog(int Id)
        {
            return context.Blogs.Find(Id);
        }

        public Blog Update(Blog blogChanged)
        {
            var blog = context.Blogs.Attach(blogChanged);
            blog.State = EntityState.Modified;
            context.SaveChanges();

            return blogChanged;
        }
    }
}
