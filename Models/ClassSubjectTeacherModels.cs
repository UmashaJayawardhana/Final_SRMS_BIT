using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class ClassSubjectTeacherModels
    {
        public int No { get; set; }
        public int ClassSubjectTeacherSerial { get; set; }
        public int ClassSerial { get; set; }
        public string ClassName { get; set; }
        public int BatchSerial { get; set; }
        public string BatchName { get; set; }
        public int Year { get; set; }
        public int TeacherSerial { get; set; }
        public string FullName { get; set; }
        public int SubjectSerial { get; set; }
        public string SubjectName { get; set; }
        public string Status { get; set; }
    }
}