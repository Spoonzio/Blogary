using Blogary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blogary.ViewModels
{
    public class CreateBlogViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Brief Description")]
        public string BriefDescription { get; set; }

        [Required]
        public Topic Topic { get; set; }

        [Required]
        [MinLength(140, ErrorMessage = "Your blog cannot be shorter than a tweet")]
        [Display(Name = "Blog Content")]
        public string BlogContent { get; set; }

        public string UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
