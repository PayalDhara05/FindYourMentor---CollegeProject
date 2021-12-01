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
    }
}