using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FindYourMentorProject.Models
{
    [MetadataType(typeof(RegisterStudentMetadata))]  //tells the compiler that the class RegisterStudentMetadata contains additional data for the RegisterStudent class and use that data whenever the RegisterStudent class is used.
    public partial class RegisterStudent
    {
        public string ConfirmPassword { get; set; }

        public enum States
        {
            AndamanandNicobar,
            AndhraPradesh,
            ArunachalPradesh,   
            Assam,
            Bihar,
            Chandigarh,
            Chhattisgarh,
            DadraandNagarHaveli,
            DamanandDiu,
            Delhi,
            Goa,
            Gujarat,
            Haryana,
            HimachalPradesh,
            JammuandKashmir,
            Jharkhand,
            Karnataka,
            Kerala,
            Lakshadweep,
            MadhyaPradesh,
            Maharashtra,
            Manipur,
            Meghalaya,
            Mizoram,
            Nagaland,
            Orissa,
            Puducherry,
            Punjab,
            Rajasthan,
            Sikkim,
            TamilNadu,
            Telangana,
            Tripura,
            UttarPradesh,
            Uttarakhand,
            WestBengal,	
        }
    }
    public class RegisterStudentMetadata
    {
        [Display(Name = "Lastname")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters are allowed")]
        public string LastName { get; set; }

        [Display(Name = "Firstname")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Firstname required")]
        [MinLength(3,ErrorMessage ="Atleast 3 characters are expected")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters are allowed")]
        public string FirstName { get; set; }

        [Display(Name = "EmailID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "EmailID required")]
        [DataType(DataType.EmailAddress)]
        [Remote("ValidateMenteeEmailID", "User")]
        //[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        [RegularExpression("^[A-Za-z0-9]{1}[A-Za-z0-9.-]{2,}@[A-Za-z]{3,}[.][A-Za-z]{2,6}$", ErrorMessage = "Invalid Email Address")]
        public string EmailID { get; set; }

        [Display(Name = "State")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "State is required")]
        public string State { get; set; }

        [Display(Name = "Pincode")]
        [RegularExpression("^[1-9][0-9]{5}$", ErrorMessage = "Invalid Pincode")]
        public int? Pincode { get; set; }

        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Minimum 8 characters required")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Confirm password and password do not match")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Student Description")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        [MaxLength(500, ErrorMessage = "Limit is 500 characters")]
        public string Description { get; set; }
        public string Role { get; set; }

        [Display(Name = "Your Age")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your age !!")]
        [Range(5, 85, ErrorMessage = "Age must be between 5-85 years")]
        public int Age { get; set; }

        [Display(Name = "Contact number")]
        [StringLength(13, MinimumLength = 10)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Contact Number is required")]
        [RegularExpression("^[789][0-9]{9,9}$", ErrorMessage = "Contact Number is in invalid format")]
        public string ContactNo { get; set; }

        [Display(Name = "Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Address is required")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string Address { get; set; }

        [Display(Name = "GitHub Profile ID")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string GitHubID { get; set; }

        [Display(Name = "LinkedIn Profile ID")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string LinkedInID { get; set; }

        [Display(Name = "City")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string City { get; set; }
    }
}