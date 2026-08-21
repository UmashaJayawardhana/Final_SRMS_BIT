using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class SectionModels
    {
        public int Year { get; set; }
        public int No { get; set; }
        public int SectionSerial { get; set; }
        public string SectionName { get; set; }
        public string Status { get; set; }
        public int BatchSerial { get; set; }
        public string BatchName { get; set; }
        public List<int> BatchSerials { get; set; }
        public int TeacherSerial { get; set; }
        public string FullName { get; set; }
    }
}