using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class TeacherAddToSubjectModels
    {
        public int No { get; set; }
        public int TeacherSubjectSerial { get; set; }
        public int TeacherSerial { get; set; }
        public string FullName { get; set; }
        public int SubjectSerial { get; set; }
        public string SubjectName { get; set; }
        public string Status { get; set; }
    }
}