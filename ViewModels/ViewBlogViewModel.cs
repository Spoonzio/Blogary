using Blogary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogary.ViewModels
{
    public class ViewBlogViewModel
    {
        public string Title { get; set; }

        public string BriefDescription { get; set; }

        public Topic Topic { get; set; }

        public string[] BlogContent { get; set; }

        public DateTime Date { get; set; }

        public string Username { get; set; }

    }
}
