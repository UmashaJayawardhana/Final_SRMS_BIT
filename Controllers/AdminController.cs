using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MvcApplication1.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace MvcApplication1.Controllers
{
    public class AdminController : Controller
    {
        private readonly string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        //Get Usergroup to dropdown
        private List<UserGroupModels> GetAllUserGroupActive()
        {
            List<UserGroupModels> list = new List<UserGroupModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllUserGroupActive", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new UserGroupModels
                    {
                        UserGroupSerial = Convert.ToInt32(dr["UserGroupSerial"].ToString()),
                        UserGroupName = dr["UserGroupName"].ToString()
                    });
                }
            }
            return list;
        }

        //Get Teachers to dropdown
        private List<TeacherModels> GetAllTeachers()
        {
            List<TeacherModels> list = new List<TeacherModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllTeachers", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new TeacherModels
                    {
                        RegNo = dr["RegNo"].ToString(),
                        FullName = dr["FullName"].ToString()
                    });
                }
            }
            return list;
        }



        //
        // GET: /Admin/CreateUser

        public ActionResult CreateUser()
        {
            ViewBag.AllTeachers = GetAllTeachers(); // for dropdown

            ViewBag.Users = GetAllUserGroupActive();

            var userall = GetAllUsers();
            return View(userall);
        }



        [HttpPost]
        public ActionResult CreateUser(UserMasterModels model)
        {

            using (SqlConnection con = new SqlConnection(conStr))
            {
                if (model.UserSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateCreateUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserSerial", model.UserSerial);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@UserName", model.UserName);
                    cmd.Parameters.AddWithValue("@PinNo", model.PinNo);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    TempData["Message"] = "Updated successfully!";
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertCreateUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", model.UserName);
                    cmd.Parameters.AddWithValue("@EmpSerial", model.TeacherSerial);
                    cmd.Parameters.AddWithValue("@PinNo", model.PinNo);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    cmd.Parameters.AddWithValue("@UserGroupSerial", model.UserGroupSerial);
                    cmd.Parameters.AddWithValue("@Status", model.Status);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "User already exists!";
                    }
                    if (result == -2)
                    {
                        TempData["Message"] = "Duplicate User Name";
                    }
                    if (result == -2)
                    {
                        TempData["Message"] = "Duplicate  Password";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!";
                    }
                }

            }

            return RedirectToAction("CreateUser"); // Redirects and shows message


            var userall = GetAllUsers(); // reload updated list
            return View(userall); // Return view with model 
        }

        //get all exams to table
        private List<UserMasterModels> GetAllUsers()
        {
            List<UserMasterModels> list = new List<UserMasterModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllUsers", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new UserMasterModels
                    {
                        No = i++,
                        UserSerial = Convert.ToInt32(dr["UserSerial"].ToString()),
                        TeacherSerial = Convert.ToInt32(dr["TeacherSerial"].ToString()),
                        FullName = dr["FullName"].ToString(),
                        RegNo = dr["RegNo"].ToString(),
                        UserName = dr["UserName"].ToString(),
                        PinNo = dr["PinNo"].ToString(),
                        Password = dr["Password"].ToString(),
                        UserGroupSerial = Convert.ToInt32(dr["UserGroupSerial"].ToString()),
                        UserGroupName = dr["UserGroupName"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
            }
            return list;
        }


        public JsonResult IsUserNameAvailable(string username, int? userSerial)
        {
            bool exists = false;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("IsUserNameAvailable", con);
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@UserSerial", (object)userSerial ?? DBNull.Value);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        exists = true; // username exists for a different user
                    }

                }
            }

            return Json(!exists, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsPinAvailable(string pin, int? userSerial)
        {
            bool exists = false;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("IsPinAvailable", con);
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PinNo", pin);
                    cmd.Parameters.AddWithValue("@UserSerial", (object)userSerial ?? DBNull.Value);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        exists = true; // username exists for a different user
                    }

                }
            }

            return Json(!exists, JsonRequestBehavior.AllowGet);
        }


        //Get students to dropdown
        private List<StudentModels> GetAllStudentsActive()
        {
            List<StudentModels> list = new List<StudentModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllStudentsActive", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new StudentModels
                    {
                        RegNo = dr["RegNo"].ToString(),
                        FullName = dr["FullName"].ToString()
                    });
                }
            }
            return list;
        }

        //
        // GET: /Admin/CreateUserStudent

        public ActionResult CreateUserStudent()
        {
            ViewBag.AllStudents = GetAllStudentsActive(); // for dropdown


            var userall = GetAllUsersStudents();
            return View(userall);
        }



        [HttpPost]
        public ActionResult CreateUserStudent(UserMasterModels model)
        {

            using (SqlConnection con = new SqlConnection(conStr))
            {
                if (model.UserSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateCreateUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserSerial", model.UserSerial);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@UserName", model.UserName);
                    cmd.Parameters.AddWithValue("@PinNo", model.PinNo);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    TempData["Message"] = "Updated successfully!";
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertCreateUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", model.UserName);
                    cmd.Parameters.AddWithValue("@EmpSerial", model.StudentSerial);
                    cmd.Parameters.AddWithValue("@PinNo", model.PinNo);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    cmd.Parameters.AddWithValue("@UserGroupSerial", model.UserGroupSerial);
                    cmd.Parameters.AddWithValue("@Status", model.Status);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "User already exists!";
                    }
                    if (result == -2)
                    {
                        TempData["Message"] = "Duplicate User Name";
                    }
                    if (result == -2)
                    {
                        TempData["Message"] = "Duplicate  Password";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!";
                    }
                }

            }

            return RedirectToAction("CreateUserStudent"); // Redirects and shows message


            var userall = GetAllUsersStudents(); // reload updated list
            return View(userall); // Return view with model 
        }

        //get all exams to table
        private List<UserMasterModels> GetAllUsersStudents()
        {
            List<UserMasterModels> list = new List<UserMasterModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllUsersStudents", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new UserMasterModels
                    {
                        No = i++,
                        UserSerial = Convert.ToInt32(dr["UserSerial"].ToString()),
                        StudentSerial = Convert.ToInt32(dr["StudentSerial"].ToString()),
                        FullName = dr["FullName"].ToString(),
                        RegNo = dr["RegNo"].ToString(),
                        UserName = dr["UserName"].ToString(),
                        PinNo = dr["PinNo"].ToString(),
                        Password = dr["Password"].ToString(),
                        UserGroupSerial = Convert.ToInt32(dr["UserGroupSerial"].ToString()),
                        UserGroupName = dr["UserGroupName"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
            }
            return list;
        }

        //
        // GET: /Admin/CreateUserParent

        public ActionResult CreateUserParent()
        {
            ViewBag.AllStudents = GetAllStudentsActive(); // for dropdown


            var userall = GetAllUsersParents();
            return View(userall);
        }



        [HttpPost]
        public ActionResult CreateUserParent(UserMasterModels model)
        {

            using (SqlConnection con = new SqlConnection(conStr))
            {
                if (model.UserSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateCreateUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserSerial", model.UserSerial);
                    cmd.Parameters.AddWithValue("@UserName", model.UserName);
                    cmd.Parameters.AddWithValue("@PinNo", model.PinNo);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    TempData["Message"] = "Updated successfully!";
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertCreateUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", model.UserName);
                    cmd.Parameters.AddWithValue("@EmpSerial", model.StudentSerial);
                    cmd.Parameters.AddWithValue("@PinNo", model.PinNo);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    cmd.Parameters.AddWithValue("@UserGroupSerial", model.UserGroupSerial);
                    cmd.Parameters.AddWithValue("@Status", model.Status);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "User already exists!";
                    }
                    if (result == -2)
                    {
                        TempData["Message"] = "Duplicate User Name";
                    }
                    if (result == -2)
                    {
                        TempData["Message"] = "Duplicate  Password";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!";
                    }
                }

            }

            return RedirectToAction("CreateUserParent"); // Redirects and shows message


            var userall = GetAllUsersParents(); // reload updated list
            return View(userall); // Return view with model 
        }

        //get all exams to table
        private List<UserMasterModels> GetAllUsersParents()
        {
            List<UserMasterModels> list = new List<UserMasterModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllUsersParents", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new UserMasterModels
                    {
                        No = i++,
                        UserSerial = Convert.ToInt32(dr["UserSerial"].ToString()),
                        StudentSerial = Convert.ToInt32(dr["StudentSerial"].ToString()),
                        FullName = dr["FullName"].ToString(),
                        RegNo = dr["RegNo"].ToString(),
                        GuardianType = dr["GuardianType"].ToString(),
                        GuardianName = dr["GuardianName"].ToString(),
                        UserName = dr["UserName"].ToString(),
                        PinNo = dr["PinNo"].ToString(),
                        Password = dr["Password"].ToString(),
                        UserGroupSerial = Convert.ToInt32(dr["UserGroupSerial"].ToString()),
                        UserGroupName = dr["UserGroupName"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
            }
            return list;
        }


        //
        // GET: /Admin/AddUserGroup

        public ActionResult AddUserGroup()
        {


            var userall = GetAllAddUserGroup();
            return View(userall);
        }



        [HttpPost]
        public ActionResult AddUserGroup(UserGroupModels model)
        {

            using (SqlConnection con = new SqlConnection(conStr))
            {
                if (model.UserGroupSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateUserGroup", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserGroupSerial", model.UserGroupSerial);
                    cmd.Parameters.AddWithValue("@UserGroupName", model.UserGroupName);
                    cmd.Parameters.AddWithValue("@Status", model.Status);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "User already exists!";
                    }
                    else
                    {
                        TempData["Message"] = "Updated successfully!";
                    }
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertUserGroup", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserGroupName", model.UserGroupName);
                    cmd.Parameters.AddWithValue("@Status", model.Status);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "User already exists!";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!";
                    }
                }

            }

            return RedirectToAction("AddUserGroup"); // Redirects and shows message


            var userall = GetAllAddUserGroup(); // reload updated list
            return View(userall); // Return view with model 
        }

        // GET: /Admin/ManageMenus
        public ActionResult ManageMenus()
        {
            List<AppMenu> list = GetAllMenus();
            ViewBag.ParentMenus = GetParentMenus();
            return View(list);
        }

        // POST: /Admin/SaveMenu
        [HttpPost]
        public ActionResult SaveMenu(AppMenu model)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query;
                if (model.MenuId > 0)
                {
                    SqlCommand cmd = new SqlCommand("UpdateAppMenu", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MenuId", model.MenuId);
                    cmd.Parameters.AddWithValue("@MenuName", model.MenuName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Url", model.Url ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ControllerName", model.ControllerName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ActionName", model.ActionName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Icon", model.Icon ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ParentMenuId", model.ParentMenuId.HasValue && model.ParentMenuId.Value > 0 ? model.ParentMenuId.Value : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DisplayOrder", model.DisplayOrder);
                    cmd.Parameters.AddWithValue("@Status", model.Status ?? "Active");

                    con.Open();
                    cmd.ExecuteNonQuery();
                    
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("InsertAppMenu", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    
                    cmd.Parameters.AddWithValue("@MenuName", model.MenuName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Url", model.Url ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ControllerName", model.ControllerName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ActionName", model.ActionName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Icon", model.Icon ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ParentMenuId", model.ParentMenuId.HasValue && model.ParentMenuId.Value > 0 ? model.ParentMenuId.Value : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DisplayOrder", model.DisplayOrder);
                    cmd.Parameters.AddWithValue("@Status", model.Status ?? "Active");

                    con.Open();
                    cmd.ExecuteNonQuery();
                       
                }

               
            }

            // Clear permission cache
            List<string> keysToClear = new List<string>();
            foreach (string key in Session.Keys)
            {
                if (key.StartsWith("UserPermissions_"))
                {
                    keysToClear.Add(key);
                }
            }
            foreach (var key in keysToClear)
            {
                Session[key] = null;
            }

            TempData["Message"] = "Menu saved successfully!";
            return RedirectToAction("ManageMenus");
        }

        private List<AppMenu> GetAllMenus()
        {
            List<AppMenu> list = new List<AppMenu>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllMenus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AppMenu
                        {
                            MenuId = Convert.ToInt32(reader["MenuId"]),
                            MenuName = reader["MenuName"].ToString(),
                            ControllerName = reader["ControllerName"] == DBNull.Value ? "" : reader["ControllerName"].ToString(),
                            ActionName = reader["ActionName"] == DBNull.Value ? "" : reader["ActionName"].ToString(),
                            Url = reader["Url"] == DBNull.Value ? "" : reader["Url"].ToString(),
                            Icon = reader["Icon"] == DBNull.Value ? "" : reader["Icon"].ToString(),
                            ParentMenuId = reader["ParentMenuId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["ParentMenuId"]),
                            ParentMenuName = reader["ParentMenuName"] == DBNull.Value ? "" : reader["ParentMenuName"].ToString(),
                            DisplayOrder = Convert.ToInt32(reader["DisplayOrder"]),
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        private List<AppMenu> GetParentMenus()
        {
            List<AppMenu> list = new List<AppMenu>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetParentMenus", con);
                cmd.CommandType = CommandType.StoredProcedure;

       
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AppMenu
                        {
                            MenuId = Convert.ToInt32(reader["MenuId"]),
                            MenuName = reader["MenuName"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // GET: /Admin/ManagePermissions
        public ActionResult ManagePermissions(int? roleId)
        {
            ViewBag.Roles = GetAllAddUserGroup();
            ViewBag.SelectedRoleId = roleId;

            List<RolePrivilegeModel> privileges = new List<RolePrivilegeModel>();
            if (roleId.HasValue)
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                   
                    SqlCommand cmd = new SqlCommand("GetRolePrivilegesByUserGroup", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserGroupSerial", roleId.Value);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            privileges.Add(new RolePrivilegeModel
                            {
                                MenuId = Convert.ToInt32(reader["MenuId"]),
                                MenuName = reader["MenuName"].ToString(),
                                UserGroupName = reader["ParentMenuName"] == DBNull.Value || string.IsNullOrEmpty(reader["ParentMenuName"].ToString())
                                    ? "" 
                                    : reader["ParentMenuName"].ToString(),
                                CanView = Convert.ToBoolean(reader["CanView"]),
                                CanAdd = Convert.ToBoolean(reader["CanAdd"]),
                                CanEdit = Convert.ToBoolean(reader["CanEdit"]),
                                CanDelete = Convert.ToBoolean(reader["CanDelete"])
                            });
                        }
                    }
                }
            }

            return View(privileges);
        }

        // POST: /Admin/SavePermissions
        [HttpPost]
        public ActionResult SavePermissions(int roleId)
        {
            List<int> menuIds = new List<int>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetActiveMenuIds", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        menuIds.Add(Convert.ToInt32(reader["MenuId"]));
                    }
                }
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("DeleteRolePrivilegeByUserGroup", con, trans))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@UserGroupSerial", roleId);
                            cmd.ExecuteNonQuery();
                        }

                        foreach (int menuId in menuIds)
                        {
                            bool canView = Request.Form["view_" + menuId] != null && Request.Form["view_" + menuId].Contains("true");
                            bool canAdd = Request.Form["add_" + menuId] != null && Request.Form["add_" + menuId].Contains("true");
                            bool canEdit = Request.Form["edit_" + menuId] != null && Request.Form["edit_" + menuId].Contains("true");
                            bool canDelete = Request.Form["delete_" + menuId] != null && Request.Form["delete_" + menuId].Contains("true");

                            if (canView || canAdd || canEdit || canDelete)
                            {
                                using (SqlCommand cmd = new SqlCommand("InsertRolePrivilege", con, trans))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@UserGroupSerial", roleId);
                                    cmd.Parameters.AddWithValue("@MenuId", menuId);
                                    cmd.Parameters.AddWithValue("@CanView", canView);
                                    cmd.Parameters.AddWithValue("@CanAdd", canAdd);
                                    cmd.Parameters.AddWithValue("@CanEdit", canEdit);
                                    cmd.Parameters.AddWithValue("@CanDelete", canDelete);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            // Clear permission cache
            List<string> keysToClear = new List<string>();
            foreach (string key in Session.Keys)
            {
                if (key.StartsWith("UserPermissions_"))
                {
                    keysToClear.Add(key);
                }
            }
            foreach (var key in keysToClear)
            {
                Session[key] = null;
            }

            TempData["Message"] = "Permissions saved successfully!";
            return RedirectToAction("ManagePermissions", new { roleId = roleId });
        }

        private List<UserGroupModels> GetAllAddUserGroup()
        {
            List<UserGroupModels> list = new List<UserGroupModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllUserGroup", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new UserGroupModels
                    {
                        No = i++,
                        UserGroupSerial = Convert.ToInt32(dr["UserGroupSerial"].ToString()),
                        UserGroupName = dr["UserGroupName"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
            }
            return list;
        }
    }
}
