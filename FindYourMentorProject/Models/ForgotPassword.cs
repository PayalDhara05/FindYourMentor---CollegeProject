using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FindYourMentorProject.Models
{
    public class ForgotPassword
    {
        [Display(Name = "EmailID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "EmailID required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailID { get; set; }


        [Display(Name = "Select Your Role")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Role is required")]
        public RolePassword UserRolePassword { get; set; }
    }

    public enum RolePassword
    {
        Student,
        Mentor
    }
}