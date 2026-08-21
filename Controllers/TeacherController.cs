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
    public class TeacherController : Controller
    {

        private readonly string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //Get Max Reg No

        public string GetNextRegNo()
        {
            string maxRegNo = "";

            StudentModels list = new StudentModels();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetMaxTeacherRegNo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    maxRegNo = dr["MaxRegNo"].ToString();

                }
            }
            return GenerateNextRegNo(maxRegNo);
        }

        //Generate Max Reg No

        private string GenerateNextRegNo(string currentMax)
        {
            if (string.IsNullOrEmpty(currentMax))
                return DateTime.Now.Year + "0001";

            int number = int.Parse(currentMax.Substring(4)) + 1;
            return DateTime.Now.Year + number.ToString("D4");
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

        private List<TeacherModels> GetAllTeachersWithDetails()
        {
            List<TeacherModels> list = new List<TeacherModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllTeachersWithDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        int i = 1;
                        while (dr.Read())
                        {
                            list.Add(new TeacherModels
                            {
                                No = i++,
                                TeacherSerial = Convert.ToInt32(dr["TeacherSerial"]),
                                RegNo = dr["RegNo"].ToString(),
                                Initials = dr["Initials"].ToString(),
                                FirstName = dr["FirstName"].ToString(),
                                LastName = dr["LastName"].ToString(),
                                FullName = dr["FullName"].ToString(),
                                DateofBirth = Convert.ToDateTime(dr["DateofBirth"]),
                                Gender = dr["Gender"].ToString(),
                                NIC = dr["NIC"].ToString(),
                                TelePhone = dr["TelePhone"].ToString(),
                                RegisterDate = Convert.ToDateTime(dr["RegisterDate"]),
                                Status = dr["Status"].ToString(),
                                UserGroupSerial = Convert.ToInt32(dr["UserGroupSerial"]),
                                UserGroupName = dr["UserGroupName"] != DBNull.Value ? dr["UserGroupName"].ToString() : ""
                            });
                        }
                    
                }
            }
            return list;
        }

        //
        // GET: /Teacher/

        public ActionResult AddTeacher()
        {
            ViewBag.AllTeachers = GetAllTeachers(); // for dropdown

            ViewBag.RegNo = GetNextRegNo();

            var teachers = GetAllTeachersWithDetails();
            return View(teachers);
        }

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


        // POST: Handle form submission to add a teacher
        [HttpPost]
        public ActionResult AddTeacher(TeacherModels model)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                //update photo
                var file = Request.Files["Photo"];
                if (file != null && file.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        model.PhotoUpload = binaryReader.ReadBytes(file.ContentLength);
                    }
                    model.Phototype = file.ContentType;
                }

                // Update logic 
                if (model.TeacherSerial != 0)
                {
                    SqlCommand cmd = new SqlCommand("UpdateTeacher", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TeacherSerial", model.TeacherSerial);
                    cmd.Parameters.AddWithValue("@RegNo", model.RegNo);
                    cmd.Parameters.AddWithValue("@Initials", model.Initials);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@FullName", model.FullName);
                    cmd.Parameters.AddWithValue("@DateofBirth", model.DateofBirth);
                    cmd.Parameters.AddWithValue("@Gender", model.Gender);
                    cmd.Parameters.AddWithValue("@TelePhone", model.TelePhone);
                    cmd.Parameters.AddWithValue("@NIC", model.NIC);
                    cmd.Parameters.AddWithValue("@RegisterDate", model.RegisterDate);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@UserGroupSerial", model.UserGroupSerial);
                    if (model.PhotoUpload != null)
                    {
                        cmd.Parameters.Add("@Photo", SqlDbType.Image).Value = model.PhotoUpload;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Photo", SqlDbType.Image).Value = DBNull.Value;
                    }
                    cmd.Parameters.AddWithValue("@Phototype", model.Phototype ?? (object)DBNull.Value);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "Teacher not found!";
                    }
                    else if (result == -2)
                    {
                        TempData["Message"] = "Teacher already exists!";
                    }
                    else if (result == 2)
                    {
                        TempData["Message"] = "Can't Add more Admins, Already have one!";
                    }
                    else if (result == -3)
                    {
                        TempData["Message"] = "Can't Add more Principal, Already have one!";
                    }
                    else if (result == 3)
                    {
                        TempData["Message"] = "NIC is Already exists!";
                    }
                    else
                    {
                        if (model.CreateUserAccount || Request.Form["CreateUserAccount"] == "true")
                        {
                            string createdUser, createdPw;
                            bool userSuccess = CreateAutoUserAccount(model, out createdUser, out createdPw);
                            if (userSuccess)
                            {
                                TempData["Message"] = "Updated successfully & User Account created! (Username: " + createdUser + ", Password: " + createdPw + ")";
                            }
                            else
                            {
                                TempData["Message"] = "Updated successfully, but User Account could not be created.";
                            }
                        }
                        else
                        {
                            TempData["Message"] = "Updated successfully!";
                        }
                    }
                }
                else
                {
                    // Insert logic 


                    SqlCommand cmd = new SqlCommand("InsertTeacher", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RegNo", model.RegNo);
                    cmd.Parameters.AddWithValue("@Initials", model.Initials);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@FullName", model.FullName);
                    cmd.Parameters.AddWithValue("@DateofBirth", model.DateofBirth);
                    cmd.Parameters.AddWithValue("@Gender", model.Gender);
                    cmd.Parameters.AddWithValue("@TelePhone", model.TelePhone);
                    cmd.Parameters.AddWithValue("@NIC", model.NIC);
                    cmd.Parameters.AddWithValue("@RegisterDate", model.RegisterDate);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@UserGroupSerial", model.UserGroupSerial);
                    if (model.PhotoUpload != null)
                    {
                        cmd.Parameters.Add("@Photo", SqlDbType.Image).Value = model.PhotoUpload;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Photo", SqlDbType.Image).Value = DBNull.Value;
                    }
                    cmd.Parameters.AddWithValue("@Phototype", model.Phototype ?? (object)DBNull.Value);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "Teacher already exists!";
                    }
                    else if (result == -2)
                    {
                        TempData["Message"] = "Can't Add more Admins, Already have one!";
                    }
                    else if (result == 2)
                    {
                        TempData["Message"] = "Can't Add more Principal, Already have one!";
                    }
                    else if (result == 3)
                    {
                        TempData["Message"] = "NIC is Already exists!";
                    }
                    else
                    {
                        if (model.CreateUserAccount || Request.Form["CreateUserAccount"] == "true")
                        {
                            string createdUser, createdPw;
                            bool userSuccess = CreateAutoUserAccount(model, out createdUser, out createdPw);
                            if (userSuccess)
                            {
                                TempData["Message"] = "Saved successfully & User Account created! (Username: " + createdUser + ", Password: " + createdPw + ")";
                            }
                            else
                            {
                                TempData["Message"] = "Saved successfully, but User Account could not be created.";
                            }
                        }
                        else
                        {
                            TempData["Message"] = "Saved successfully!";
                        }
                    }


                }

                return RedirectToAction("AddTeacher"); // Redirects and shows message

            }

            return RedirectToAction("AddTeacher");
        }

        private bool CreateAutoUserAccount(TeacherModels model, out string createdUserName, out string createdPassword)
        {
            createdUserName = "";
            createdPassword = "";

            try
            {
                int teacherSerial = model.TeacherSerial;
                if (teacherSerial == 0)
                {
                    using (SqlConnection lookupCon = new SqlConnection(conStr))
                    {
                        using (SqlCommand lookupCmd = new SqlCommand("LookupTeacher", lookupCon))
                        {
                            lookupCmd.CommandType = CommandType.StoredProcedure;
                            lookupCmd.Parameters.AddWithValue("@RegNo", model.RegNo);
                            lookupCon.Open();
                            using (SqlDataReader reader = lookupCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    teacherSerial = Convert.ToInt32(reader["TeacherSerial"]);
                                }
                            }
                        }
                    }
                }

                if (teacherSerial <= 0) return false;

                string firstName = !string.IsNullOrWhiteSpace(model.FirstName) ? model.FirstName.Trim() : (!string.IsNullOrWhiteSpace(model.FullName) ? model.FullName.Split(' ')[0].Trim() : "Teacher");
                createdUserName = firstName;
                createdPassword = firstName + "123";
                string autoPin = "1234";

                using (SqlConnection userCon = new SqlConnection(conStr))
                {
                    SqlCommand userCmd = new SqlCommand("InsertCreateUser", userCon);
                    userCmd.CommandType = CommandType.StoredProcedure;
                    userCmd.Parameters.AddWithValue("@UserName", createdUserName);
                    userCmd.Parameters.AddWithValue("@EmpSerial", teacherSerial);
                    userCmd.Parameters.AddWithValue("@PinNo", autoPin);
                    userCmd.Parameters.AddWithValue("@Password", createdPassword);
                    userCmd.Parameters.AddWithValue("@UserGroupSerial", model.UserGroupSerial);
                    userCmd.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(model.Status) ? "Active" : model.Status);

                    SqlParameter returnParam = userCmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    userCon.Open();
                    userCmd.ExecuteNonQuery();
                    int userResult = (int)returnParam.Value;
                    userCon.Close();

                    return userResult != -1 && userResult != -2;
                }
            }
            catch
            {
                return false;
            }
        }

        //get all teacher details according to reg no
        public JsonResult LookupTeacher(string regNo)
        {
            TeacherModels teahcer = null;
            using (SqlConnection con = new SqlConnection(conStr))
            {

                using (SqlCommand cmd = new SqlCommand("LookupTeacher", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RegNo", regNo);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            teahcer = new TeacherModels
                            {
                                TeacherSerial = Convert.ToInt32(reader["TeacherSerial"].ToString()),
                                RegNo = reader["RegNo"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Initials = reader["Initials"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                DateofBirth = Convert.ToDateTime(reader["DateofBirth"]),
                                TelePhone = reader["TelePhone"].ToString(),
                                NIC = reader["NIC"].ToString(),
                                RegisterDate = Convert.ToDateTime(reader["RegisterDate"]),
                                Status = reader["Status"].ToString(),
                                UserGroupSerial = Convert.ToInt32(reader["UserGroupSerial"])
                            };

                            if (reader["Photo"] != DBNull.Value)
                            {
                                byte[] photoData = (byte[])reader["Photo"];
                                string photoFormat = reader["Phototype"].ToString();
                                string base64String = Convert.ToBase64String(photoData);
                                teahcer.LookupPhoto = "data:" + photoFormat + ";base64," + base64String;

                            }
                        }
                    }
                }
            }

            return Json(teahcer, JsonRequestBehavior.AllowGet);
        }


        //
        // GET: /TeacherAddToSubject/

        public ActionResult TeacherAddToSubject()
        {
            var teacherAddTosubject = AllTeacherAddToSubject();

            ViewBag.Teachers = GetTeachersActiveFilterUserGroup();// for teacher dropdown

            ViewBag.Subjects = GetSubjectsActive();// for subject  dropdown

            return View(teacherAddTosubject);
        }


        // POST: Handle form submission to add subject to teacher
        [HttpPost]
        public ActionResult TeacherAddToSubject(TeacherAddToSubjectModels model)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                if (model.TeacherSubjectSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateTeacherAddToSubject", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TeacherSubjectSerial", model.TeacherSubjectSerial);
                    cmd.Parameters.AddWithValue("@TeacherSerial", model.TeacherSerial);
                    cmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
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
                        TempData["Message"] = "Teacher already assign to Subject!";
                    }
                    else
                    {
                        TempData["Message"] = "Updated successfully!";
                    }
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertTeacherAddToSubject", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TeacherSerial", model.TeacherSerial);
                    cmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
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
                        TempData["Message"] = "Teacher already assign to Subject!";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!";
                    }
                }

            }

            return RedirectToAction("TeacherAddToSubject"); // Redirects and shows message


            var teacherAddTosubject = AllTeacherAddToSubject(); // reload updated list
            return View(teacherAddTosubject); // Return view with model 
        }

        //get all teacher- subject to table
        private List<TeacherAddToSubjectModels> AllTeacherAddToSubject()
        {
            List<TeacherAddToSubjectModels> list = new List<TeacherAddToSubjectModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("AllTeacherAddToSubject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new TeacherAddToSubjectModels
                    {
                        No = i++,
                        TeacherSubjectSerial = Convert.ToInt32(dr["TeacherSubjectSerial"].ToString()),
                        TeacherSerial = Convert.ToInt32(dr["TeacherSerial"].ToString()),
                        SubjectSerial = Convert.ToInt32(dr["SubjectSerial"].ToString()),
                        FullName = dr["FullName"].ToString(),
                        SubjectName = dr["SubjectName"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
            }
            return list;
        }


        //get all active subjects 
        private List<SubjectModels> GetSubjectsActive()
        {
            List<SubjectModels> list = new List<SubjectModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllActiveSubjects", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new SubjectModels
                    {
                        SubjectSerial = Convert.ToInt32(dr["SubjectSerial"].ToString()),
                        SubjectName = dr["SubjectName"].ToString()
                    });
                }
            }
            return list;
        }


        //get all active teachers , where usergroup serial is 2, insert as a teacher
        private List<TeacherModels> GetTeachersActiveFilterUserGroup()
        {
            List<TeacherModels> list = new List<TeacherModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllActiveTeachersFilterUsergroup", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new TeacherModels
                    {
                        No = i++,
                        TeacherSerial = Convert.ToInt32(dr["TeacherSerial"].ToString()),
                        FullName = dr["FullName"].ToString()
                    });
                }
            }
            return list;
        }

        //
        // GET: /TeacherDropout/

        public ActionResult TeacherDropout()
        {
            var teacherDropout = AllTeacherDropout();

            ViewBag.Teachers = GetAllActiveTeachersWithoutStatus();// for teacher dropdown

            return View(teacherDropout);
        }


        // POST: Handle form submission to dropout teacher
        [HttpPost]
        public ActionResult TeacherDropout(TeacherDropoutModels model)
        {
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;
            using (SqlConnection con = new SqlConnection(conStr))
            {
                if (model.TeacherDropoutSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateTeacherDropout", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TeacherDropoutSerial", model.TeacherDropoutSerial);
                    cmd.Parameters.AddWithValue("@TeacherSerial", model.TeacherSerial);
                    cmd.Parameters.AddWithValue("@DropoutReason", model.DropoutReason);
                    cmd.Parameters.AddWithValue("@DropoutDate", model.DropoutDate);
                    cmd.Parameters.AddWithValue("@UserSerial", userserial);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;
                    con.Open();
                    cmd.ExecuteNonQuery();
                     int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "Teacher already dropout!";
                    }
                    else
                    {
                        TempData["Message"] = "Updated successfully!";
                    }
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertTeacherDropout", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TeacherSerial", model.TeacherSerial);
                    cmd.Parameters.AddWithValue("@DropoutReason", model.DropoutReason);
                    cmd.Parameters.AddWithValue("@DropoutDate", model.DropoutDate);
                    cmd.Parameters.AddWithValue("@UserSerial", userserial);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "Teacher already dropout!";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!";
                    }
                }

            }

            return RedirectToAction("TeacherDropout"); // Redirects and shows message


            var teacherDropout = AllTeacherDropout(); // reload updated list
            return View(teacherDropout); // Return view with model 
        }

        //get all teacher- subject to table
        private List<TeacherDropoutModels> AllTeacherDropout()
        {
            List<TeacherDropoutModels> list = new List<TeacherDropoutModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllTeacherDropout", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new TeacherDropoutModels
                    {
                        No = i++,
                        TeacherDropoutSerial = Convert.ToInt32(dr["TeacherDropoutSerial"].ToString()),
                        TeacherSerial = Convert.ToInt32(dr["TeacherSerial"].ToString()),
                        FullName = dr["FullName"].ToString(),
                        DropoutReason = dr["DropoutReason"].ToString(),
                        DropoutDate = Convert.ToDateTime(dr["DropoutDate"].ToString())
                    });
                }
            }
            return list;
        }
    


        //get all active teachers 
        private List<TeacherModels> GetTeachersActive()
        {
            List<TeacherModels> list = new List<TeacherModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllActiveTeachers", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new TeacherModels
                    {
                        No = i++,
                        TeacherSerial = Convert.ToInt32(dr["TeacherSerial"].ToString()),
                        FullName = dr["FullName"].ToString()
                    });
                }
            }
            return list;
        }

        //get all active teachers 
        private List<TeacherModels> GetAllActiveTeachersWithoutStatus()
        {

            List<TeacherModels> list = new List<TeacherModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllActiveTeachersWithoutStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new TeacherModels
                    {
                        No = i++,
                        TeacherSerial = Convert.ToInt32(dr["TeacherSerial"].ToString()),
                        FullName = dr["FullName"].ToString()
                    });
                }
            }
            return list;
        }
    }
}
