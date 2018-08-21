using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IntervieweeNet45.Models
{
    public class IntervieweeDbContext : DbContext
    {
        public IntervieweeDbContext() : base ("DbConnSqlLocal")
        {

        }

        public DbSet<Interviewee> Interviewees { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<CityMun> CityMuns { get; set; }
        public DbSet<Barangay> Barangays { get; set; }
    }
}