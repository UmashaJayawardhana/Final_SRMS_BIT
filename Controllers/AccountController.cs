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

namespace MvcApplication1.Controllers
{
    public class AccountController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    SqlCommand cmd = new SqlCommand("UserLogin", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserName", model.UserName);
                    cmd.Parameters.AddWithValue("@Password", model.Password);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        // Login success
                        Session["UserSerial"] = rdr["UserSerial"].ToString();
                        Session["Username"] = rdr["Username"].ToString();
                        Session["EmpSerial"] = rdr["EmpSerial"].ToString();
                        Session["UserGroupSerial"] = rdr["UserGroupSerial"].ToString();
                        Session["UserGroupName"] = rdr["UserGroupName"].ToString();
                        Session["FullName"] = rdr["FullName"].ToString();
                        Session["UserPhoto"] = rdr["Photo"] == DBNull.Value ? null : (byte[])rdr["Photo"];
                        Session["Phototype"] = rdr["Phototype"] == DBNull.Value ? null : rdr["Phototype"].ToString();

                        return RedirectToAction("Dashboard", "Home");
                    }
                    else
                    {
                        TempData["Message"] = "Incorrect username or password!";
                       // ViewBag.AlertMessage = "Incorrect username or password!";
                        return View("LogOn", model);
                    }
                    
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult GetUserPhoto()
        {
            if (Session["UserPhoto"] != null)
            {
                byte[] photo = Session["UserPhoto"] as byte[];
                string photoType = Session["Phototype"] as string ?? "image/jpeg";

                if (photo != null)
                {
                    return File(photo, photoType);
                }
            }

            string defaultImagePath = Server.MapPath("~/Content/img/profileimg.png");
            return File(defaultImagePath, "image/png");

        }

        //
        // GET: /Account/ProfileChange

        public ActionResult ProfileChange()
        {
                int userSerial = Convert.ToInt32(Session["UserSerial"]);

        UserProfileModels model = new UserProfileModels();

        using (SqlConnection con = new SqlConnection(conStr))
        {
            SqlCommand cmd = new SqlCommand("GetUserProfileDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserSerial", userSerial);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model.UserSerial = Convert.ToInt32(reader["UserSerial"]);
                model.RegNo = reader["RegNo"].ToString();
                model.FullName = reader["FullName"].ToString();
                model.Status = reader["Status"].ToString();
                model.UserName = reader["UserName"].ToString();
                model.Password = reader["Password"].ToString();
                if (reader["Photo"] != DBNull.Value)
                {
                    byte[] photoData = (byte[])reader["Photo"];
                    string photoFormat = reader["Phototype"].ToString();
                    string base64String = Convert.ToBase64String(photoData);
                    model.LookupPhoto = "data:" + photoFormat + ";base64," + base64String;

                }
            }
            con.Close();
        }

        return View(model);
    }


        //change user name
        [HttpPost]
        public ActionResult ChangeUsername(int UserSerial, string NewUsername)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("UpdateUsername", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserSerial", UserSerial);
                cmd.Parameters.AddWithValue("@NewUsername", NewUsername);

                // Add return value parameter
                SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParam.Direction = ParameterDirection.ReturnValue;

                con.Open();
                cmd.ExecuteNonQuery();
                int result = (int)returnParam.Value;
                con.Close();

                if (result == -1)
                {
                    TempData["Message"] = "User not found!";
                }
                else if (result == -2)
                {
                    TempData["Message"] = "UserName already exists!";
                }
                else
                {
                    TempData["Message"] = "User Name Changes Succuessfully";
                }
                return RedirectToAction("LogOn"); // force re-login
            }
        }


    //Change password
        [HttpPost]
        public ActionResult ChangePassword(int UserSerial, string OldPassword, string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                TempData["Message"] = "New passwords do not match.";
                return RedirectToAction("ProfileChange");
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("UpdatePassword", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserSerial", UserSerial);
                cmd.Parameters.AddWithValue("@OldPassword", OldPassword);
                cmd.Parameters.AddWithValue("@NewPassword", NewPassword);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                if (rowsAffected == -1)
                {
                    TempData["Message"] = "Old password is incorrect.";
                    return RedirectToAction("ProfileChange");
                }
            }


            TempData["Message"] = "Password Changes Succuessfully";
            return RedirectToAction("LogOn"); // force re-login
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }




        //
        // GET: /Account/ChangePassword

        //[Authorize]
        //public ActionResult ChangePassword()
        //{
        //    return View();
        //}

        //
        // POST: /Account/ChangePassword

        //[Authorize]
        //[HttpPost]
        //public ActionResult ChangePassword(ChangePasswordModel model)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        // ChangePassword will throw an exception rather
        //        // than return false in certain failure scenarios.
        //        bool changePasswordSucceeded;
        //        try
        //        {
        //            MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
        //            changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
        //        }
        //        catch (Exception)
        //        {
        //            changePasswordSucceeded = false;
        //        }

        //        if (changePasswordSucceeded)
        //        {
        //            return RedirectToAction("ChangePasswordSuccess");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
