using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FindYourMentorProject.Models
{
    [MetadataType(typeof(AppointmentMetadata))]
    public partial class Appointment
    {

    }
    public class AppointmentMetadata
    {
        [Display(Name = "Name")]
        public string MenteeName { get; set; }

        [Display(Name = "State")]
        public string MenteeState { get; set; }

        [Display(Name = "Address")]
        public string MenteeAddress { get; set; }

        [Display(Name = "Your Age")]
        public int MenteeAge { get; set; }

        [Display(Name = "Contact number")]
        public string MenteeContactNo { get; set; }

        [Display(Name = "EmailID")]
        public string MenteeEmailID { get; set; }

        [Display(Name = "Working Status")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select one !!")]
        public string MenteeWorkingStatus { get; set; }

        [Display(Name = "Mentee GitHub")]
        public string MenteeGithubID { get; set; }

        [Display(Name = "Mentee LinkedIn")]
        public string MenteeLinkedinID { get; set; }

        [Display(Name = "Appointment Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date is required")]
        public Nullable<System.DateTime> AppointmentDate { get; set; }

        [Display(Name = "Specify the time you want to start the appointment")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Field is required")]
        [Range(1, 24, ErrorMessage = "please specify the time between 1-24")]
        public Nullable<int> StartTime { get; set; }

        [Display(Name = "Specify the time you want to end the appointment")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Field is required")]
        [Range(1, 24, ErrorMessage = "please specify the time between 1-24")]
        public Nullable<int> EndTime { get; set; }

    }
}