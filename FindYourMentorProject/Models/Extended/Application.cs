using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FindYourMentorProject.Models
{
    [MetadataType(typeof(ApplicationMetadata))]
    public partial class Application
    {
       
    }
    public class ApplicationMetadata
    {
        [Display(Name = "Message")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Message is required")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string MenteeMessage { get; set; }

        [Display(Name = "Background")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Background is required")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string MenteeBackground { get; set; }

        [Display(Name = "Expectations")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string MenteeExpectations { get; set; }
        public string ApplicationStatus { get; set; }

        [Display(Name = "Name")]
        public string MenteeName { get; set; }

        [Display(Name = "EmailID")]
        public string MenteeEmailID { get; set; }

        [Display(Name = "State")]
        public string MenteeState { get; set; }

        [Display(Name = "Contact number")]
        public string MenteeContactNo { get; set; }

        [Display(Name = "Your Age")]
        public int MenteeAge { get; set; }

        [Display(Name = "Address")]
        public string MenteeAddress { get; set; }

        [Display(Name = "Working Status")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select one !!")]
        public string WorkingStatus { get; set; }

        [Display(Name = "GitHub Profile")]
        public string MenteeGithub { get; set; }

        [Display(Name = "LinkedIn Profile")]
        public string MenteeLinkedin { get; set; }

        public Nullable<System.DateTime> StatusUpdateTime { get; set; }

        [Display(Name = "Application Date and Time")]
        public Nullable<System.DateTime> AppliedTime { get; set; }
    }
}