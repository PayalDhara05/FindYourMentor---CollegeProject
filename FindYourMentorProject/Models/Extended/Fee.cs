using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FindYourMentorProject.Models
{
    [MetadataType(typeof(FeeMetadata))]
    public partial class Fee
    {

    }
    public class FeeMetadata
    {
        [Display(Name = "Mode of Payment")]
        [Required(AllowEmptyStrings =false, ErrorMessage = "Please select any one option")]
        public string PaymentMode { get; set; }

        [MinLength(3, ErrorMessage = "Atleast 3 characters are required")]
        public string CardName { get; set; }

        [MinLength(16, ErrorMessage = "16 digits are expected")]
        [MaxLength(16, ErrorMessage = "16 digits are expected")]
        public string CardNumber { get; set; }

        [MinLength(4, ErrorMessage = "4 digits are expected")]
        [MaxLength(4, ErrorMessage = "4 digits are expected")]
        public string cvv_new { get; set; }
    }    
}