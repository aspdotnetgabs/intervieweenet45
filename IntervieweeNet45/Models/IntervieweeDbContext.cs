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

    }
}