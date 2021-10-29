using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FindYourMentorProject.Models
{
    [MetadataType(typeof(AddNotesMenteeMetadata))]
    public partial class AddNotesMentee
    {
           
    }
    public class AddNotesMenteeMetadata
    {
        [Display(Name = "Title")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title required")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description required")]
        [MinLength(3, ErrorMessage = "Atleast 3 characters are expected")]
        public string Description { get; set; }

        public Nullable<System.DateTime> CreationDate { get; set; }
    }
}