using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IntervieweeNet45.Models
{
    [Table("Interviewees")]
    public class Interviewee
    {
        //[CsvIgnore]
        public int Id { get; set; }
        public string Gender { get; set; }
        public string Title { get; set; }
        public string Occupation { get; set; }
        public string Company { get; set; }
        public string GivenName { get; set; }
        public string MiddleInitial { get; set; }
        public string Surname { get; set; }
        public string BloodType { get; set; }
        public string EmailAddress { get; set; }
    }
}