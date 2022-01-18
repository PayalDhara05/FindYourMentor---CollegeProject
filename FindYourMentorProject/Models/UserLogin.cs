using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FindYourMentorProject.Models
{
    public class UserLogin
    {
            public enum Role
            {
                Student,
                Mentor
            }

            [Display(Name = "EmailID")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "EmailID required")]
            [DataType(DataType.EmailAddress)]
            //[EmailAddress(ErrorMessage = "Invalid Email Address")]
            [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
            public string EmailID { get; set; }

            [Display(Name = "Password")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Select Your Role")]
            [Required(AllowEmptyStrings = false, ErrorMessage = "Role is required")]
            public string UserRole { get; set; }

            [Display(Name = "Remember me")]
            public Boolean RememberMe { get; set; }
    }
}