using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;

namespace MvcApplication1.Models
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public string PageName { get; set; }
        public string PermissionType { get; set; } // "View", "Add", "Edit", "Delete"

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            if (session["UserGroupSerial"] == null)
            {
                filterContext.Result = new RedirectResult("~/Account/LogOn");
                return;
            }

            int userGroupSerial = Convert.ToInt32(session["UserGroupSerial"]);
            if (userGroupSerial == 1) // Super Admin has access to everything
            {
                return;
            }

            // Default to Controller Name or Action Name if PageName not specified
            string targetPage = PageName ?? filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            bool hasAccess = CheckPermissionFromCache(session, userGroupSerial, targetPage, PermissionType);

            // Also double check Action Name if not matching controller
            if (!hasAccess && PageName == null)
            {
                string actionName = filterContext.ActionDescriptor.ActionName;
                hasAccess = CheckPermissionFromCache(session, userGroupSerial, actionName, PermissionType);
            }

            if (!hasAccess)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new { success = false, message = "Access Denied: You do not have privilege to perform this action." },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/Home/AccessDenied");
                }
            }
        }

        private bool CheckPermissionFromCache(HttpSessionStateBase session, int userGroupSerial, string pageName, string permissionType)
        {
            string cacheKey = "UserPermissions_" + userGroupSerial;
            var permissions = session[cacheKey] as Dictionary<string, bool>;
            
            if (permissions == null)
            {
                permissions = HtmlHelperExtensions.LoadPermissionsFromDatabase(userGroupSerial);
                session[cacheKey] = permissions;
            }

            string checkKey = (pageName.Trim() + "_" + permissionType.Trim()).ToLower();
            bool allowed;
            if (permissions.TryGetValue(checkKey, out allowed))
            {
                return allowed;
            }

            // Partial matching fallback
            foreach (var key in permissions.Keys)
            {
                if (key.Contains(pageName.ToLower()) && key.EndsWith("_" + permissionType.ToLower()))
                {
                    return permissions[key];
                }
            }

            return false;
        }
    }
}
