using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FindYourMentorProject.Models
{
    public class AdvertisementBookmarkViewModel
    {
        public List<CourseAdvertisement> CourseAdvertisement { get; set; }
        public List<SavedList> SavedList { get; set; }
    }
}