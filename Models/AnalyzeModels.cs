using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class AnalyzeModels
    {

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


        public decimal? Sinhala { get; set; }
        public decimal? Mathematics { get; set; }
        public decimal? Science { get; set; }
        public decimal? English { get; set; }
        public decimal? History { get; set; }
        public decimal? Buddhism { get; set; }

        public decimal Present { get; set; }
        public decimal Absent { get; set; }
    }
}