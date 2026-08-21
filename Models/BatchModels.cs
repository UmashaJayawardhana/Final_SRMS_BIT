using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class BatchModels
    {
        public int No { get; set; }
        public int BatchSerial { get; set; }
        public string BatchName { get; set; }
        public int Year { get; set; }
        public int GradeSerial { get; set; }
        public string GradeName { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TeacherSerial { get; set; }
        public string FullName { get; set; }
    }
}