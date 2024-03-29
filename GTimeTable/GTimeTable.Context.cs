﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GTimeTable
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GTimeTableEntities : DbContext
    {
        public GTimeTableEntities()
            : base("name=GTimeTableEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<WorkingDaysAndHour> WorkingDaysAndHours { get; set; }
        public virtual DbSet<WorkingDaysOfWeek> WorkingDaysOfWeeks { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<Lecturer> Lecturers { get; set; }
        public virtual DbSet<LecturesSession> LecturesSessions { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<SutiableTabforRoom> SutiableTabforRooms { get; set; }
        public virtual DbSet<NotAvailbleTimesOfLecturer> NotAvailbleTimesOfLecturers { get; set; }
        public virtual DbSet<SuitableRoomsforSubjectTag> SuitableRoomsforSubjectTags { get; set; }
        public virtual DbSet<SuitableRoomsforLecturer> SuitableRoomsforLecturers { get; set; }
        public virtual DbSet<SuiableRoomsforSession> SuiableRoomsforSessions { get; set; }
        public virtual DbSet<SuitableRoomsforGroup> SuitableRoomsforGroups { get; set; }
        public virtual DbSet<NotAvailbeleTimesOfSession> NotAvailbeleTimesOfSessions { get; set; }
        public virtual DbSet<NotAvailbleTimesOfGroup> NotAvailbleTimesOfGroups { get; set; }
        public virtual DbSet<ParrelalSession> ParrelalSessions { get; set; }
    }
}
