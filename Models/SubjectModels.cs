using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class SubjectModels
    {
        public int No { get; set; }
        public int SubjectSerial { get; set; }
        public int GradeSerial { get; set; }
        public string Status { get; set; }
        public string Medium { get; set; }
        public string SubjectName { get; set; }
        public string GradeName { get; set; }
        public List<int> GradeSerials { get; set; }
    }
}