using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class AssignmentResultModels
    {
        public int No { get; set; }
        public int ExamScheduleAssignmentSerial { get; set; }
        public int ExamSerial { get; set; }
        public string ExamName { get; set; }
        public int TermSerial { get; set; }
        public string TermName { get; set; }
        public string Status { get; set; }
        public int Year { get; set; }
        public int BatchSerial { get; set; }
        public int ClassSerial { get; set; }
        public string BatchName { get; set; }
        public string ClassName { get; set; }
        public int SubjectSerial { get; set; }
        public string SubjectName { get; set; }
        public DateTime DateTime { get; set; }

        public int? ExamAssignmentAttedanceSerial { get; set; }
        public int StudentSerial { get; set; }
        public string FullName { get; set; }
        public string Attedance { get; set; }
        public decimal? Result { get; set; }
        public string RegNo { get; set; }

    }
}