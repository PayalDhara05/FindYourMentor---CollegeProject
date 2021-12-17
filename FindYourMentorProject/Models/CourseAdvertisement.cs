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
    
    public partial class CourseAdvertisement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CourseAdvertisement()
        {
            this.Applications = new HashSet<Application>();
            this.Appointments = new HashSet<Appointment>();
            this.SavedLists = new HashSet<SavedList>();
            this.Fees1 = new HashSet<Fee>();
            this.Feedbacks = new HashSet<Feedback>();
        }
    
        public int AdvertisementID { get; set; }
        public string ClassName { get; set; }
        public string CourseName { get; set; }
        public string MentorName { get; set; }
        public string Description { get; set; }
        public string ContactNo { get; set; }
        public string EmailID { get; set; }
        public Nullable<int> BatchesFull { get; set; }
        public Nullable<int> BatchesAvailable { get; set; }
        public int TotalSeats { get; set; }
        public int Fees { get; set; }
        public string Address { get; set; }
        public string SpokenLanguage1 { get; set; }
        public string GitHubAccount { get; set; }
        public string Field { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string SpokenLanguage2 { get; set; }
        public int MentorID { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string DemoLec1 { get; set; }
        public int YearsOfExperience { get; set; }
        public string DemoLec2 { get; set; }
        public string LinkedIn { get; set; }
        public string RemovalStatus { get; set; }
        public string SpokenLanguage3 { get; set; }
        public string SpokenLanguage4 { get; set; }
        public string DemoLec3 { get; set; }
        public string DemoLec4 { get; set; }
        public int SeatsOccupied { get; set; }
        public string logo { get; set; }
        public System.DateTime StartDate { get; set; }
        public string Duration { get; set; }
        public string Mode { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Application> Applications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual RegisterMentor RegisterMentor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SavedList> SavedLists { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fee> Fees1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}
