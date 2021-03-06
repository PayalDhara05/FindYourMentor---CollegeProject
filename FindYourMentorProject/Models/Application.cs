//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FindYourMentorProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Application
    {
        public int ApplicationID { get; set; }
        public int AdvertisementID { get; set; }
        public int MentorID { get; set; }
        public int MenteeID { get; set; }
        public string MenteeMessage { get; set; }
        public string MenteeBackground { get; set; }
        public string MenteeExpectations { get; set; }
        public string ApplicationStatus { get; set; }
        public string MenteeName { get; set; }
        public string MenteeEmailID { get; set; }
        public string MenteeState { get; set; }
        public string MenteeContactNo { get; set; }
        public Nullable<int> MenteeAge { get; set; }
        public string MenteeAddress { get; set; }
        public string WorkingStatus { get; set; }
        public string MenteeGithub { get; set; }
        public string MenteeLinkedin { get; set; }
        public string MentorEmailID { get; set; }
        public int statusCounter { get; set; }
        public Nullable<System.DateTime> StatusUpdateTime { get; set; }
        public Nullable<System.DateTime> AppliedTime { get; set; }
        public string MentorRemoveStatus { get; set; }
        public string ClassName { get; set; }
        public string CourseName { get; set; }
        public string MentorName { get; set; }
    
        public virtual CourseAdvertisement CourseAdvertisement { get; set; }
        public virtual RegisterStudent RegisterStudent { get; set; }
    }
}
