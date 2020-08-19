using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogary.Models
{
    public interface IBlogRepository
    {

        IEnumerable<Blog> GetAllBlogs();

        Blog GetBlog(int Id);
        Blog Add(Blog blog);
        Blog Update(Blog blogChanged);
        Blog Delete(int Id);
    }
}
