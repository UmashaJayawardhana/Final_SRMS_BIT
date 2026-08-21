using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class StudentAddToNewBatchModels
    {

        public int StudentSerial { get; set; }
        public string RegNo { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public int PrevClassSerial { get; set; }
        public int? NewClassSerial { get; set; }
        public int PrevYear { get; set; }
        public int PrevBatchSerial { get; set; }
        public int NewYear { get; set; }
        public int NewBatchSerial { get; set; }
        public bool IsSelected { get; set; }
    }

    public class StudentNewBatchAssignment
    {
        public int StudentSerial { get; set; }
        public string Status { get; set; }
        public int? NewClassSerial { get; set; }
        public int PrevYear { get; set; }
        public int PrevBatchSerial { get; set; }
        public int PrevClassSerial { get; set; }
        public int NewYear { get; set; }
        public int NewBatchSerial { get; set; }
    }
}