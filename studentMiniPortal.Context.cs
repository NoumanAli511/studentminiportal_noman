﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace studentminiportal
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class projectdatabaseEntities12 : DbContext
    {
        public projectdatabaseEntities12()
            : base("name=projectdatabaseEntities12")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<admin> admin { get; set; }
        public virtual DbSet<Alumni> Alumni { get; set; }
        public virtual DbSet<Alumni_> Alumni_ { get; set; }
        public virtual DbSet<AssignedSurvey> AssignedSurvey { get; set; }
        public virtual DbSet<bothstudent> bothstudent { get; set; }
        public virtual DbSet<commentedOnEvent> commentedOnEvent { get; set; }
        public virtual DbSet<CompletedSurveys> CompletedSurveys { get; set; }
        public virtual DbSet<courses> courses { get; set; }
        public virtual DbSet<createsurvey> createsurvey { get; set; }
        public virtual DbSet<EventComments> EventComments { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<JobApplications> JobApplications { get; set; }
        public virtual DbSet<JobDetails> JobDetails { get; set; }
        public virtual DbSet<posts> posts { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<survey_questions> survey_questions { get; set; }
        public virtual DbSet<surveyAllumini> surveyAllumini { get; set; }
        public virtual DbSet<surveyCurrentStudent> surveyCurrentStudent { get; set; }
        public virtual DbSet<SurveyQuestionOptions> SurveyQuestionOptions { get; set; }
        public virtual DbSet<SurveyResponse> SurveyResponse { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<ViewedEvents> ViewedEvents { get; set; }
    }
}
