using Blogary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogary.ViewModels
{
    public class AuthorProfileViewModel
    {
        public AuthorProfileViewModel()
        {
            Blogs = new List<Blog>();
        }

        public string Id { get; set; }

        public ApplicationUser User { get; set; }

        public List<Blog> Blogs { get; set; }
    }
}
