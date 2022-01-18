using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FindYourMentorProject.Models
{
    public class ChangePasswordModel
    {
        [Display(Name = "Old Password")]
        [Required(ErrorMessage = "Old password required", AllowEmptyStrings = false)]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name ="New Password")]
        [Required(ErrorMessage = "New password required", AllowEmptyStrings = false)]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Retype New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password does not match")]
        public string ConfirmPassword { get; set; }
    }
}