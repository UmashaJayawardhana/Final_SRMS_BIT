using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;

namespace MvcApplication1.Models
{
    public static class HtmlHelperExtensions
    {
        public static bool HasPermission(this HtmlHelper htmlHelper, string pageName, string permissionType)
        {
            var session = htmlHelper.ViewContext.HttpContext.Session;
            if (session["UserGroupSerial"] == null) return false;

            int userGroupSerial = Convert.ToInt32(session["UserGroupSerial"]);
            if (userGroupSerial == 1) return true; // Super admin always allowed

            string cacheKey = "UserPermissions_" + userGroupSerial;
            var permissions = session[cacheKey] as Dictionary<string, bool>;
            
            if (permissions == null)
            {
                permissions = LoadPermissionsFromDatabase(userGroupSerial);
                session[cacheKey] = permissions;
            }

            string checkKey = (pageName.Trim() + "_" + permissionType.Trim()).ToLower();
            bool allowed;
            if (permissions.TryGetValue(checkKey, out allowed))
            {
                return allowed;
            }

            // Also check partial matching (e.g. if pageName is "AddGrade" and menu url is "../Master/AddGrade")
            foreach (var key in permissions.Keys)
            {
                if (key.Contains(pageName.ToLower()) && key.EndsWith("_" + permissionType.ToLower()))
                {
                    return permissions[key];
                }
            }

            return false;
        }

        public static Dictionary<string, bool> LoadPermissionsFromDatabase(int userGroupSerial)
        {
            var dict = new Dictionary<string, bool>();
            string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = @"
                    SELECT m.MenuName, m.Url, m.ControllerName, m.ActionName,
                           rp.CanView, rp.CanAdd, rp.CanEdit, rp.CanDelete
                    FROM RolePrivilege rp
                    JOIN AppMenu m ON rp.MenuId = m.MenuId
                    WHERE rp.UserGroupSerial = @UserGroupSerial";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserGroupSerial", userGroupSerial);
                con.Open();
                
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string menuName = rdr["MenuName"].ToString().ToLower();
                        string url = rdr["Url"] != DBNull.Value ? rdr["Url"].ToString().ToLower() : "";
                        string controller = rdr["ControllerName"] != DBNull.Value ? rdr["ControllerName"].ToString().ToLower() : "";
                        string action = rdr["ActionName"] != DBNull.Value ? rdr["ActionName"].ToString().ToLower() : "";
                        
                        bool canView = Convert.ToBoolean(rdr["CanView"]);
                        bool canAdd = Convert.ToBoolean(rdr["CanAdd"]);
                        bool canEdit = Convert.ToBoolean(rdr["CanEdit"]);
                        bool canDelete = Convert.ToBoolean(rdr["CanDelete"]);

                        AddPermissionKeys(dict, menuName, canView, canAdd, canEdit, canDelete);
                        
                        if (!string.IsNullOrEmpty(url))
                        {
                            AddPermissionKeys(dict, url, canView, canAdd, canEdit, canDelete);
                            
                            // strip leading path characters like .. or / to allow simpler view matches
                            string strippedUrl = url.Replace("..", "").Replace("/", "").Trim();
                            if (!string.IsNullOrEmpty(strippedUrl))
                            {
                                AddPermissionKeys(dict, strippedUrl, canView, canAdd, canEdit, canDelete);
                            }
                        }
                        if (!string.IsNullOrEmpty(controller))
                        {
                            AddPermissionKeys(dict, controller, canView, canAdd, canEdit, canDelete);
                        }
                        if (!string.IsNullOrEmpty(action))
                        {
                            AddPermissionKeys(dict, action, canView, canAdd, canEdit, canDelete);
                        }
                    }
                }
            }
            return dict;
        }

        private static void AddPermissionKeys(Dictionary<string, bool> dict, string key, bool view, bool add, bool edit, bool del)
        {
            if (string.IsNullOrEmpty(key)) return;
            
            dict[key + "_view"] = view;
            dict[key + "_add"] = add;
            dict[key + "_edit"] = edit;
            dict[key + "_delete"] = del;
        }
    }
}
