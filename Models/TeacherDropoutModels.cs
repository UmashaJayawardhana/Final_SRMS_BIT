using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class TeacherDropoutModels
    {
        public int No { get; set; }
        public int TeacherDropoutSerial { get; set; }
        public int TeacherSerial { get; set; }
        public string DropoutReason { get; set; }
        public DateTime DropoutDate { get; set; }
        public string FullName { get; set; }
    }
}