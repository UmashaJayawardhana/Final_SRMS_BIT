using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication1.Models
{
    public class UserProfileModels
    {
        public int UserSerial { get; set; }
        public string UserName { get; set; }
        public string UserGroupName { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string FullName { get; set; }
        public string RegNo { get; set; }
        public byte[] PhotoUpload { get; set; }
        public string Phototype { get; set; }
        public string LookupPhoto { get; set; }
    }
}