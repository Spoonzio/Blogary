using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blogary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogRepository _blogRepository;

        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        // GET 
        // api/Blog/ID
        [HttpGet("{id}")]
        public Blog Get(int Id)
        {
            Blog blog = _blogRepository.GetBlog(Id);

            return blog;
        }
    }
}