using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FindYourMentorProject.Models
{
        [MetadataType(typeof(CourseAdvertisementMetadata))]
        public partial class CourseAdvertisement
        {
            
        }
        public class CourseAdvertisementMetadata
        {
        [Display(Name = "Class Name")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string ClassName { get; set; }

        [Display(Name = "Course Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Course name is required")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string CourseName { get; set; }

        [Display(Name = "Mentor Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mentor name is required")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string MentorName { get; set; }

        [Display(Name = "Description")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string Description { get; set; }

        [Display(Name = "Contact number")]
        [StringLength(13, MinimumLength = 10)]
        [DataType(DataType.PhoneNumber)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Contact Number is required")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Contact Number is in invalid format")]
        public string ContactNo { get; set; }

        [Display(Name = "EmailID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "EmailID required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailID { get; set; }

        [Display(Name = "Number of Batches(Full)")]
        public Nullable<int> BatchesFull { get; set; }

        [Display(Name = "Number of Batches(Available)")]
        public Nullable<int> BatchesAvailable { get; set; }

        [Display(Name = "Total number of Students")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Field is required")]
        public int TotalSeats { get; set; }

        [Display(Name = "Number of Seats Occupied")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Field is required")]
        public int SeatsOccupied { get; set; }

        [Display(Name = "Total Fees of this course")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Fee field is required")]
        public int Fees { get; set; }

        [Display(Name = "Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Address is required")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string Address { get; set; }

        [Display(Name = "Spoken Language")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string SpokenLanguage1 { get; set; }

        [Display(Name = "GitHub Account")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string GitHubAccount { get; set; }

        [Display(Name = "Field")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Field name is required")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string Field { get; set; }

        [Display(Name = "State")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "State name is required")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string State { get; set; }

        [Display(Name = "City")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City name is required")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string City { get; set; }

        public System.DateTime CreationDate { get; set; }

        [Display(Name = "Total Years of Experience hello")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "This field is required")]
        public int YearsOfExperience { get; set; }

        [NotMapped]
        public HttpPostedFileBase VideoUpload1 { get; set; }

        [NotMapped]
        public HttpPostedFileBase VideoUpload2 { get; set; }

        [NotMapped]
        public HttpPostedFileBase VideoUpload3 { get; set; }

        [NotMapped]
        public HttpPostedFileBase VideoUpload4 { get; set; }

        [Display(Name = "Demo Lecture 1")]
        public string DemoLec1 { get; set; }

        [Display(Name = "Demo Lecture 2")]
        public string DemoLec2 { get; set; }
    }
}