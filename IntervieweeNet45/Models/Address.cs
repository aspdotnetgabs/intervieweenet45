using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IntervieweeNet45.Models
{
    [Table("Provinces")]
    public class Province
    {
        [Key]
        public int id { get; set; }
        public int psgcCode { get; set; }
        public string provDesc { get; set; }
        public int provCode { get; set; }
    }

    [Table("CityMuns")]
    public class CityMun
    {
        [Key]
        public int id { get; set; }
        public int psgcCode { get; set; }
        public string citymunDesc { get; set; }
        public int provCode { get; set; }
        public int citymunCode { get; set; }
    }

    [Table("Barangays")]
    public class Barangay
    {
        [Key]
        public int id { get; set; }
        public int brgyCode { get; set; }
        public string brgyDesc { get; set; }
        public int provCode { get; set; }
        public int citymunCode { get; set; }
    }
}