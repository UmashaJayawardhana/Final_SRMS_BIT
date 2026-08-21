using System;

namespace MvcApplication1.Models
{
    public class RolePrivilegeModel
    {
        public int PrivilegeId { get; set; }
        public int UserGroupSerial { get; set; }
        public string UserGroupName { get; set; }
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}
