using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blogary.ViewModels
{
    public class EditProfileViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "New Username")]
        public string NewUsername { get; set; }

    }
}
