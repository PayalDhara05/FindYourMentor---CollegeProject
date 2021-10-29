using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FindYourMentorProject.Models
{
    [MetadataType(typeof(ContactUMetadata))]
    public partial class ContactU
    {

    }
    public class ContactUMetadata
    {
            [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
            [MinLength(2, ErrorMessage = "Atleast 2 characters are expected")]
            public string Name { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "EmailID required")]
            [DataType(DataType.EmailAddress)]
            [EmailAddress(ErrorMessage = "Invalid Email Address")]
            public string EmailID { get; set; }

            [StringLength(13, MinimumLength = 10)]
            [DataType(DataType.PhoneNumber)]
            [RegularExpression(@"^(\d{10})$", ErrorMessage = "Contact Number is in invalid format")]
            public string ContactNo { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "Message is required")]
            [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
            [MaxLength(500, ErrorMessage = "Limit is 500 characters")]
            public string Message { get; set; }
    }
}