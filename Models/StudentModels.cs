using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class StudentModels
    {
        public int No { get; set; }
        public int StudentSerial { get; set; }
        public string RegNo{ get; set; }
        public string Initials { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime DateofBirth { get; set; }
        public string Gender{ get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3{ get; set; }
        public string City { get; set; }
        public string ResidencePhone { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string Status { get; set; }
        public string GuardianType { get; set; }
        public string GuardianName { get; set; }
        public string GuardianPhone { get; set; }
        public byte[] PhotoUpload { get; set; }
        public string Phototype { get; set; }
        public int Year { get; set; }
        public int BatchSerial{ get; set; }
        public int ClassSerial{ get; set; }
        public string LookupPhoto { get; set; }

        public string ClassName { get; set; }
        public string BatchName { get; set; }
        public bool CreateUserAccount { get; set; }

    }
}