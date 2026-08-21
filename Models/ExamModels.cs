using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class ExamModels
    {
        public int No { get; set; }
        public int ExamSerial { get; set; }
        public string ExamName { get; set; }
        public int TermSerial { get; set; }
        public string TermName { get; set; }
        public string Status { get; set; }
    }
}