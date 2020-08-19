using Blogary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogary.ViewModels
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            BlogInTopic = new Dictionary<Topic, Blog>();
        }

        public Blog LatestBlog { get; set; }
        public Dictionary<Topic, Blog> BlogInTopic { get; set; }
    }
}
