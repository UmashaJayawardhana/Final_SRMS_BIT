using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class UserMasterModels
    {
        public int No { get; set; }
        public int UserSerial { get; set; }
        public string UserName { get; set; }
        public int TeacherSerial { get; set; }
        public int StudentSerial { get; set; }
        public string FullName { get; set; }
        public string RegNo { get; set; }
        public int UserGroupSerial { get; set; }
        public string UserGroupName { get; set; }
        public string PinNo { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string GuardianName { get; set; }
        public string GuardianType { get; set; }
    }
}