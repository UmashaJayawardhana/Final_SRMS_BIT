using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class StudentDropoutModels
    {
        public int No { get; set; }
        public int StudentDropoutSerial { get; set; }
        public int Year { get; set; }
        public int BatchSerial { get; set; }
        public int ClassSerial { get; set; }
        public int StudentSerial { get; set; }
        public string FullName { get; set; }
        public string DropoutReason { get; set; }
        public DateTime DropoutDate { get; set; }
        public string BatchName { get; set; }
        public string ClassName { get; set; }
    }
}