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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FindYourMentorProjectEntities : DbContext
    {
        public FindYourMentorProjectEntities()
            : base("name=FindYourMentorProjectEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AddNotesMentee> AddNotesMentees { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<ContactU> ContactUs { get; set; }
        public virtual DbSet<CourseAdvertisement> CourseAdvertisements { get; set; }
        public virtual DbSet<RegisterMentor> RegisterMentors { get; set; }
        public virtual DbSet<RegisterStudent> RegisterStudents { get; set; }
        public virtual DbSet<SavedList> SavedLists { get; set; }
        public virtual DbSet<Fee> Fees { get; set; }
        public virtual DbSet<AddNotesMentor> AddNotesMentors { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<ReplyToComment> ReplyToComments { get; set; }
    }
}
