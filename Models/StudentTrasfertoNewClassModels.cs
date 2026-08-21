using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Models
{
    public class StudentTrasfertoNewClassModels
    {
       
            public int Year { get; set; }
            public int BatchSerial { get; set; }
            public int PrevClassSerial { get; set; }
            public int StudentSerial { get; set; }
            public int NewClassSerial { get; set; }
            public string TransferReason { get; set; }
            public DateTime TransferDate { get; set; }

            // Display properties for table
            public int No { get; set; }
            public int StudentTransferSerial { get; set; }
            public string RegNo { get; set; }
            public string FullName { get; set; }
            public string ClassName { get; set; }
            public string BatchName { get; set; }
    }
}