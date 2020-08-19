using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blogary.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required]
        public string BriefDescription { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public Topic Topic { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string BlogContent { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public bool Approved { get; set; }

    }
}
