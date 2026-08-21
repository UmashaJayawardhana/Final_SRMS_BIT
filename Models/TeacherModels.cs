using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class TeacherModels
    {
        public int No { get; set; }
        public int TeacherSerial { get; set; }
        public string RegNo { get; set; }
        public string Initials { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime DateofBirth { get; set; }
        public string Gender { get; set; }
        public string NIC { get; set; }
        public string TelePhone { get; set; }
        public DateTime RegisterDate { get; set; }
        public string Status { get; set; }
        public byte[] PhotoUpload { get; set; }
        public string Phototype { get; set; }
        public string LookupPhoto { get; set; }
        public int UserGroupSerial { get; set; }
        public string UserGroupName { get; set; }
        public bool CreateUserAccount { get; set; }
    }
}