using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FindYourMentorProject.Models
{
    public class BigModel
    {
        public CourseAdvertisement CourseAdvertisement { get; set; }
        public List<Feedback> Feedback { get; set; }
        public List<Comment> Comment { get; set;  }
        public List<ReplyToComment> ReplyToComment { get; set; }
    }
}