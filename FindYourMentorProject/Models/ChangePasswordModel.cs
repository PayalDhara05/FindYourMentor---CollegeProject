﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FindYourMentorProject.Models
{
    public class ChangePasswordModel
    {
        [Display(Name = "Select Your Role")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Role is required")]
        public Role UserRole { get; set; }

        [Display(Name = "Old Password")]
        [Required(ErrorMessage = "Old password required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name ="New Password")]
        [Required(ErrorMessage = "New password required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Retype New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password does not match")]
        public string ConfirmPassword { get; set; }
    }
}