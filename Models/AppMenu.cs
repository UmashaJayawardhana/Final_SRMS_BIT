using System;
using System.Collections.Generic;

namespace MvcApplication1.Models
{
    public class AppMenu
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int? ParentMenuId { get; set; }
        public string ParentMenuName { get; set; }
        public int DisplayOrder { get; set; }
        public string Status { get; set; }
        
        public AppMenu()
        {
            ChildMenus = new List<AppMenu>();
        }

        // Navigation helper for hierarchical rendering in views
        public List<AppMenu> ChildMenus { get; set; }
    }
}
