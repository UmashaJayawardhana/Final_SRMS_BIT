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
    public class StudentController : Controller
    {
        

        private readonly string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //Get classes to dropdown
        private List<ClassModels> GetAllClassesActive()
        {
            List<ClassModels> list = new List<ClassModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllClassesActive", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new ClassModels
                    {
                        ClassSerial = Convert.ToInt32(dr["ClassSerial"].ToString()),
                        ClassName = dr["ClassName"].ToString()
                    });
                }
            }
            return list;
        }

        //Get batches to dropdown
        private List<BatchModels> GetAllBatchesActive()
        {
            List<BatchModels> list = new List<BatchModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllBatchesActive", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new BatchModels
                    {
                        BatchSerial = Convert.ToInt32(dr["BatchSerial"].ToString()),
                        BatchName = dr["BatchName"].ToString(),
                        Year = Convert.ToInt32(dr["Year"].ToString())
                    });
                }
            }
            return list;
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

        //Get all students with details for master view table
        private List<StudentModels> GetAllStudents()
        {
            List<StudentModels> list = new List<StudentModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllStudents", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                        int i = 1;
                        while (dr.Read())
                        {
                            var student = new StudentModels
                            {
                                No = i++,
                                StudentSerial = Convert.ToInt32(dr["StudentSerial"]),
                                RegNo = dr["RegNo"].ToString(),
                                FirstName = dr["FirstName"].ToString(),
                                LastName = dr["LastName"].ToString(),
                                FullName = dr["FullName"].ToString(),
                                Initials = dr["Initials"].ToString(),
                                Gender = dr["Gender"].ToString(),
                                DateofBirth = Convert.ToDateTime(dr["DateofBirth"]),
                                Address1 = dr["Address1"].ToString(),
                                Address2 = dr["Address2"].ToString(),
                                Address3 = dr["Address3"] != DBNull.Value ? dr["Address3"].ToString() : "",
                                City = dr["City"] != DBNull.Value ? dr["City"].ToString() : "",
                                ResidencePhone = dr["ResidencePhone"].ToString(),
                                AdmissionDate = Convert.ToDateTime(dr["AdmissionDate"]),
                                Status = dr["Status"].ToString(),
                            };
                            list.Add(student);
                        }
                    }
                
            
            return list;
        }

        //Get Max Reg No
        public string GetNextRegNo()
        {
            string maxRegNo = "";

            StudentModels list = new StudentModels();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetMaxStudentRegNo", con);
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



        //
        // GET: /Student/AddStudent

        public ActionResult AddStudent()
        {
            // Year dropdown list
            List<int> years = new List<int>();
            int currentYear = DateTime.Now.Year;

            for (int i = currentYear + 1; i >= 2020; i--)
            {
                years.Add(i);
            }

            ViewBag.Years = new SelectList(
                years.Select(y => new SelectListItem
                {
                    Text = y.ToString(),
                    Value = y.ToString()
                }), "Value", "Text");

           

            ViewBag.RegNo = GetNextRegNo();

            var students = GetAllStudents();
            return View(students);
        }


        // POST: Handle form submission to add a new student
        [HttpPost]
        public ActionResult AddStudent(StudentModels model)
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
                if (model.StudentSerial != 0)
                {
                    SqlCommand cmd = new SqlCommand("UpdateStudent", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StudentSerial", model.StudentSerial);
                    cmd.Parameters.AddWithValue("@RegNo", model.RegNo);
                    cmd.Parameters.AddWithValue("@Initials", model.Initials);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@FullName", model.FullName);
                    cmd.Parameters.AddWithValue("@DateofBirth", model.DateofBirth);
                    cmd.Parameters.AddWithValue("@Gender", model.Gender);
                    cmd.Parameters.AddWithValue("@Address1", model.Address1);
                    cmd.Parameters.AddWithValue("@Address2", model.Address2);
                    if (model.Address3 != null)
                        cmd.Parameters.AddWithValue("@Address3", model.Address3);
                    else
                        cmd.Parameters.AddWithValue("@Address3", DBNull.Value);

                    if (model.City != null)
                        cmd.Parameters.AddWithValue("@City", model.City);
                    else
                        cmd.Parameters.AddWithValue("@City", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ResidencePhone", model.ResidencePhone);
                    cmd.Parameters.AddWithValue("@AdmissionDate", model.AdmissionDate);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@GuardianType", model.GuardianType);
                    cmd.Parameters.AddWithValue("@GuardianName", model.GuardianName);
                    cmd.Parameters.AddWithValue("@GuardianPhone", model.GuardianPhone);
                    if (model.PhotoUpload != null)
                    {
                        cmd.Parameters.Add("@Photo", SqlDbType.Image).Value = model.PhotoUpload;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Photo", SqlDbType.Image).Value = DBNull.Value;
                    }
                    cmd.Parameters.AddWithValue("@Phototype", model.Phototype ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "Student not found!";
                    }
                    else if (result == -2)
                    {
                        TempData["Message"] = "Student already exists!";
                    }
                    else if (result == -3)
                    {
                        TempData["Message"] = "Class is Full!";
                    }
                    else
                    {
                        if (model.CreateUserAccount || Request.Form["CreateUserAccount"] == "true")
                        {
                            string createdUser, createdPw;
                            bool userSuccess = CreateAutoStudentUserAccount(model, out createdUser, out createdPw);
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


                    SqlCommand cmd = new SqlCommand("InsertStudent", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RegNo", model.RegNo);
                    cmd.Parameters.AddWithValue("@Initials", model.Initials);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@FullName", model.FullName);
                    cmd.Parameters.AddWithValue("@DateofBirth", model.DateofBirth);
                    cmd.Parameters.AddWithValue("@Gender", model.Gender);
                    cmd.Parameters.AddWithValue("@Address1", model.Address1);
                    cmd.Parameters.AddWithValue("@Address2", model.Address2);
                    if (model.Address3 != null)
                        cmd.Parameters.AddWithValue("@Address3", model.Address3);
                    else
                        cmd.Parameters.AddWithValue("@Address3", DBNull.Value);

                    if (model.City != null)
                        cmd.Parameters.AddWithValue("@City", model.City);
                    else
                        cmd.Parameters.AddWithValue("@City", DBNull.Value);
                    cmd.Parameters.AddWithValue("@ResidencePhone", model.ResidencePhone);
                    cmd.Parameters.AddWithValue("@AdmissionDate", model.AdmissionDate);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@GuardianType", model.GuardianType);
                    cmd.Parameters.AddWithValue("@GuardianName", model.GuardianName);
                    cmd.Parameters.AddWithValue("@GuardianPhone", model.GuardianPhone);
                    if (model.PhotoUpload != null)
                    {
                        cmd.Parameters.Add("@Photo", SqlDbType.Image).Value = model.PhotoUpload;
                    }
                    else
                    {
                        cmd.Parameters.Add("@Photo", SqlDbType.Image).Value = DBNull.Value;
                    }
                    cmd.Parameters.AddWithValue("@Phototype", model.Phototype ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "Student already exists!";
                    }
                    else if (result == -2)
                    {
                        TempData["Message"] = "Class is full!";
                    }
                    else
                    {
                        if (model.CreateUserAccount || Request.Form["CreateUserAccount"] == "true")
                        {
                            string createdUser, createdPw;
                            bool userSuccess = CreateAutoStudentUserAccount(model, out createdUser, out createdPw);
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

                return RedirectToAction("AddStudent"); // Redirects and shows message

            }

            return RedirectToAction("AddStudent");
        }

        private bool CreateAutoStudentUserAccount(StudentModels model, out string createdUserName, out string createdPassword)
        {
            createdUserName = "";
            createdPassword = "";

            try
            {
                int studentSerial = model.StudentSerial;
                if (studentSerial == 0)
                {
                    using (SqlConnection lookupCon = new SqlConnection(conStr))
                    {
                        using (SqlCommand lookupCmd = new SqlCommand("LookupStudent", lookupCon))
                        {
                            lookupCmd.CommandType = CommandType.StoredProcedure;
                            lookupCmd.Parameters.AddWithValue("@RegNo", model.RegNo);
                            lookupCon.Open();
                            using (SqlDataReader reader = lookupCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    studentSerial = Convert.ToInt32(reader["StudentSerial"]);
                                }
                            }
                        }
                    }
                }

                if (studentSerial <= 0) return false;

                string firstName = !string.IsNullOrWhiteSpace(model.FirstName) ? model.FirstName.Trim() : (!string.IsNullOrWhiteSpace(model.FullName) ? model.FullName.Split(' ')[0].Trim() : "Student");
                createdUserName = firstName;
                createdPassword = firstName + "123";
                string autoPin = "1234";

                using (SqlConnection userCon = new SqlConnection(conStr))
                {
                    SqlCommand userCmd = new SqlCommand("InsertCreateUser", userCon);
                    userCmd.CommandType = CommandType.StoredProcedure;
                    userCmd.Parameters.AddWithValue("@UserName", createdUserName);
                    userCmd.Parameters.AddWithValue("@EmpSerial", studentSerial);
                    userCmd.Parameters.AddWithValue("@PinNo", autoPin);
                    userCmd.Parameters.AddWithValue("@Password", createdPassword);
                    userCmd.Parameters.AddWithValue("@UserGroupSerial", 3); // Student User Group
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


        //get all student details accordimg to reg no
        public JsonResult LookupStudent(string regNo)
        {
            StudentModels student = null;
            using (SqlConnection con = new SqlConnection(conStr))
            {

                using (SqlCommand cmd = new SqlCommand("LookupStudent", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RegNo", regNo);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            student = new StudentModels
                            {
                                StudentSerial = Convert.ToInt32(reader["StudentSerial"].ToString()),
                                RegNo = reader["RegNo"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Initials = reader["Initials"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                DateofBirth = Convert.ToDateTime(reader["DateofBirth"]),
                                Address1 = reader["Address1"].ToString(),
                                Address2 = reader["Address2"].ToString(),
                                Address3 = reader["Address3"].ToString(),
                                City = reader["City"].ToString(),
                                ResidencePhone = reader["ResidencePhone"].ToString(),
                                GuardianName = reader["GuardianName"].ToString(),
                                GuardianPhone = reader["GuardianPhone"].ToString(),
                                GuardianType = reader["GuardianType"].ToString(),
                                AdmissionDate = Convert.ToDateTime(reader["AdmissionDate"]),
                                Status = reader["Status"].ToString(),
                                Year = Convert.ToInt32(reader["Year"]),
                                BatchSerial = Convert.ToInt32(reader["BatchSerial"]),
                                ClassSerial = Convert.ToInt32(reader["ClassSerial"])
                            };

                            if (reader["Photo"] != DBNull.Value)
                            {
                                byte[] photoData = (byte[])reader["Photo"];
                                string photoFormat = reader["Phototype"].ToString();
                                string base64String = Convert.ToBase64String(photoData);
                                student.LookupPhoto = "data:" + photoFormat + ";base64," + base64String;

                            }
                        }
                    }
                }
            }

            return Json(student, JsonRequestBehavior.AllowGet);
        }

        private List<GradeModels> GetAllGradesActive()
        {
            List<GradeModels> list = new List<GradeModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllGradesActive", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new GradeModels
                    {
                        GradeSerial = Convert.ToInt32(dr["GradeSerial"].ToString()),
                        GradeName = dr["GradeName"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
            }
            return list;
        }

        //
        // GET: /Student/StudentAddToNewBatch

        public ActionResult StudentAddToNewBatch()
        {
            // Year dropdown list
            List<int> years = new List<int>();
            int currentYear = DateTime.Now.Year;

            for (int i = currentYear + 1; i >= 2020; i--)
            {
                years.Add(i);
            }

            ViewBag.Years = new SelectList(
                years.Select(y => new SelectListItem
                {
                    Text = y.ToString(),
                    Value = y.ToString()
                }), "Value", "Text");

            ViewBag.Grades = new SelectList(GetAllGradesActive(), "GradeSerial", "GradeName");

            var model = new List<StudentAddToNewBatchModels>(); //  empty list on initial load
            return View(model);
        }

        public JsonResult GetBatchesByYearGradeBothStatus(int year, int gradeSerial)
        {
            List<SelectListItem> batch = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetBatchesByYearGradeBothStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@GradeSerial", gradeSerial);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    batch.Add(new SelectListItem
                    {
                        Value = rdr["BatchSerial"].ToString(),
                        Text = rdr["BatchName"].ToString()
                    });
                }
            }
            return Json(batch, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBatchesByYearGradeActive(int year, int gradeSerial)
        {
            List<SelectListItem> batch = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetBatchesByYearGradeActive", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@GradeSerial", gradeSerial);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    batch.Add(new SelectListItem
                    {
                        Value = rdr["BatchSerial"].ToString(),
                        Text = rdr["BatchName"].ToString()
                    });
                }
            }
            return Json(batch, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveAllStudentNewBatch(List<StudentNewBatchAssignment> assignments)
        {
            if (assignments == null || !assignments.Any())
            {
                return Json(new { success = false, message = "No assignments to save." });
            }

            int successCount = 0;
            int alreadyExistCount = 0;
            int classFullCount = 0;
            int errorCount = 0;

            foreach (var item in assignments)
            {
                int result = 0;
                try
                {
                    using (SqlConnection con = new SqlConnection(conStr))
                    {
                        SqlCommand cmd = new SqlCommand("InsertStudentAddToNewBatch", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@StudentSerial", item.StudentSerial);
                        cmd.Parameters.AddWithValue("@Status", item.Status);
                        cmd.Parameters.AddWithValue("@ClassSerial", (object)item.NewClassSerial ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PrevYear", item.PrevYear);
                        cmd.Parameters.AddWithValue("@PrevBatchSerial", item.PrevBatchSerial);
                        cmd.Parameters.AddWithValue("@PrevClassSerial", item.PrevClassSerial);
                        cmd.Parameters.AddWithValue("@Year", item.NewYear);
                        cmd.Parameters.AddWithValue("@BatchSerial", item.NewBatchSerial);

                        SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParam.Direction = ParameterDirection.ReturnValue;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        result = (int)returnParam.Value;
                    }
                }
                catch (Exception ex)
                {
                    result = -3;
                }

                if (result == 1) successCount++;
                else if (result == -1) alreadyExistCount++;
                else if (result == -2) classFullCount++;
                else errorCount++;
            }

            string msg = "Saved " + successCount + " students successfully.";
            bool success = successCount > 0;
            if (alreadyExistCount > 0) msg += " " + alreadyExistCount + " already assigned.";
            if (classFullCount > 0) msg += " " + classFullCount + " failed because class is full.";
            if (errorCount > 0) msg += " " + errorCount + " errors occurred.";

            return Json(new { success = success, message = msg });
        }

        //search student according to year, batch, class
        public ActionResult SearchStudentsforAddNewBatch(int prevYear, int prevBatch, int prevClass, int newYear, int newBatch)
        {

            List<StudentAddToNewBatchModels> students = new List<StudentAddToNewBatchModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetStudentsToAddNewBatch", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PrevYear", prevYear);
                cmd.Parameters.AddWithValue("@PrevBatchSerial", prevBatch);
                cmd.Parameters.AddWithValue("@PrevClassSerial", prevClass);
                cmd.Parameters.AddWithValue("@NewYear", newYear);
                cmd.Parameters.AddWithValue("@NewBatchSerial", newBatch);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new StudentAddToNewBatchModels
                    {
                        StudentSerial = Convert.ToInt32(reader["StudentSerial"]),
                        FullName = reader["FullName"].ToString(),
                        RegNo = reader["RegNo"].ToString()
                    });
                }
            }


            // Load classes for dropdown
            List<SelectListItem> classes = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetClassesByBatch", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BatchSerial", newBatch);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    classes.Add(new SelectListItem
                    {
                        Value = rdr["ClassSerial"].ToString(),
                        Text = rdr["ClassName"].ToString()
                    });
                }
            }
            ViewBag.Classes = classes;

            return PartialView("_StudentNewBatchGridPartial", students);
        }

        // POST: Handle form submission to add students to new batch and class
        [HttpPost]
        public ActionResult StudentAddToNewBatch(int studentserial, string status, int newclassserial, int prevYear, int prevBatch, int prevClass, int newYear, int newBatch)
        {
            int result;
            bool success = true;
           
            using (SqlConnection con = new SqlConnection(conStr))
            {

                //save students in new batch and class
                SqlCommand cmd = new SqlCommand("InsertStudentAddToNewBatch", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StudentSerial", studentserial);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@ClassSerial", newclassserial);
                cmd.Parameters.AddWithValue("@PrevYear", prevYear);
                cmd.Parameters.AddWithValue("@PrevBatchSerial", prevBatch);
                cmd.Parameters.AddWithValue("@PrevClassSerial", prevClass);
                cmd.Parameters.AddWithValue("@Year", newYear);
                cmd.Parameters.AddWithValue("@BatchSerial", newBatch);

                //Add return value parameter
                SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnParam.Direction = ParameterDirection.ReturnValue;

                con.Open();
                cmd.ExecuteNonQuery();

                result = (int)returnParam.Value;
                con.Close();

            }
            string message;
            if(result == -1)
            {
                message = "Student already exists!";
                success = false;
            }
            else if (result == -2)
            {
                message = "Class is full!";
                success = false;
            }
            else
            {
             message = "Saved successfully!";
             success = true;

            }
            return Json(new { success = success, message = message });
        }


        //
        // GET: /Student/StudentTrasfertoNewClass
        public ActionResult StudentTrasfertoNewClass()
        {
            var transfers = GetAllStudentTransfers();

            // Year dropdown list
            List<int> years = new List<int>();
            int currentYear = DateTime.Now.Year;

            for (int i = currentYear + 1; i >= 2020; i--)
            {
                years.Add(i);
            }

            ViewBag.Years = new SelectList(
                years.Select(y => new SelectListItem
                {
                    Text = y.ToString(),
                    Value = y.ToString()
                }), "Value", "Text");

            ViewBag.Students = new SelectList(new List<SelectListItem>());

            return View(transfers);
        }

        private List<StudentTrasfertoNewClassModels> GetAllStudentTransfers()
        {
            List<StudentTrasfertoNewClassModels> list = new List<StudentTrasfertoNewClassModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllStudentTransfers", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new StudentTrasfertoNewClassModels
                    {
                        No = i++,
                        StudentTransferSerial = Convert.ToInt32(dr["StudentTransferSerial"].ToString()),
                        StudentSerial = Convert.ToInt32(dr["StudentSerial"].ToString()),
                        RegNo = dr["RegNo"].ToString(),
                        FullName = dr["FullName"].ToString(),
                        Year = Convert.ToInt32(dr["Year"].ToString()),
                        BatchSerial = Convert.ToInt32(dr["BatchSerial"].ToString()),
                        BatchName = dr["BatchName"].ToString(),
                        NewClassSerial = Convert.ToInt32(dr["NewClassSerial"].ToString()),
                        ClassName = dr["ClassName"].ToString(),
                        TransferReason = dr["TransferReason"].ToString(),
                        TransferDate = Convert.ToDateTime(dr["TransferDate"].ToString())
                    });
                }
            }
            return list;
        }


        //Get students to dropdown according to year, batch, class
        public JsonResult GetStudentsByYearBatchClassWithDropout(int year, int batchserial, int classserial)
        {
            List<SelectListItem> students = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetStudentsByYearBatchClassWithDropout", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    students.Add(new SelectListItem
                    {
                        Value = rdr["StudentSerial"].ToString(),
                        Text = rdr["RegNo"].ToString()
                    });
                }
            }

            return Json(students, JsonRequestBehavior.AllowGet);
        }

        //Get students to dropdown according to year, batch, class
        public JsonResult GetStudentsByYearBatchClass(int year, int batchserial, int classserial)
        {
            List<SelectListItem> students = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetStudentsByYearBatchClass", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    students.Add(new SelectListItem
                    {
                        Value = rdr["StudentSerial"].ToString(),
                        Text = rdr["RegNo"].ToString()
                    });
                }
            }

            return Json(students, JsonRequestBehavior.AllowGet);
        }


        // POST: Handle form submission to tranfer student from one clzz to another class
        [HttpPost]
        public ActionResult StudentTrasfertoNewClass(StudentTrasfertoNewClassModels model)
        {

            using (SqlConnection con = new SqlConnection(conStr))
            {
                
                    // Insert logic 
                SqlCommand cmd = new SqlCommand("InsertStudentTrasfer", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StudentSerial", model.StudentSerial);
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@PrevClassSerial", model.PrevClassSerial);
                    cmd.Parameters.AddWithValue("@NewClassSerial", model.NewClassSerial);
                    cmd.Parameters.AddWithValue("@TransferReason", model.TransferReason);
                    cmd.Parameters.AddWithValue("@TransferDate", model.TransferDate);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "Student already exist in this Class!";
                    }
                    if (result == -2)
                    {
                        TempData["Message"] = "Class is full!";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!";
                    }
                }



            return RedirectToAction("StudentTrasfertoNewClass"); // Redirects and shows message
 
        }

        //
        // GET: /Student/StudentDropout
        public ActionResult StudentDropout()
        {

            var dropout = GetAllStudentDropout();

            // Year dropdown list
            List<int> years = new List<int>();
            int currentYear = DateTime.Now.Year;

            for (int i = currentYear + 1; i >= 2020; i--)
            {
                years.Add(i);
            }

            ViewBag.Years = new SelectList(
                years.Select(y => new SelectListItem
                {
                    Text = y.ToString(),
                    Value = y.ToString()
                }), "Value", "Text");



            ViewBag.Students = new SelectList(new List<SelectListItem>());

            return View(dropout);

        }


        // POST: Handle form submission to dropout a student
        [HttpPost]
        public ActionResult StudentDropout(StudentDropoutModels model)
        {

            using (SqlConnection con = new SqlConnection(conStr))
            {
                //update logic
                if (model.StudentDropoutSerial != 0)
                {
                    SqlCommand cmd = new SqlCommand("UpdateStudentDropout", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StudentDropoutSerial", model.StudentDropoutSerial);
                    cmd.Parameters.AddWithValue("@StudentSerial", model.StudentSerial);
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                    cmd.Parameters.AddWithValue("@DropoutReason", model.DropoutReason);
                    cmd.Parameters.AddWithValue("@DropoutDate", model.DropoutDate);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();

                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "Student is already dropout!";
                    }
                    else
                    {
                        TempData["Message"] = "Updated successfully!";
                    }
                    con.Close();
                }

                // Insert logic 
                else
                {

                    SqlCommand cmd = new SqlCommand("InsertStudentDropout", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StudentSerial", model.StudentSerial);
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                    cmd.Parameters.AddWithValue("@DropoutReason", model.DropoutReason);
                    cmd.Parameters.AddWithValue("@DropoutDate", model.DropoutDate);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "Student is already dropout!";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!";
                    }
                }
            }



            return RedirectToAction("StudentDropout"); // Redirects and shows message

        }


        //get all dropout students to table
        private List<StudentDropoutModels> GetAllStudentDropout()
        {
            List<StudentDropoutModels> list = new List<StudentDropoutModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllStudentDropout", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new StudentDropoutModels
                    {
                        No = i++,
                        StudentDropoutSerial = Convert.ToInt32(dr["StudentDropoutSerial"].ToString()),
                        ClassSerial = Convert.ToInt32(dr["ClassSerial"].ToString()),
                        ClassName = dr["ClassName"].ToString(),
                        DropoutReason = dr["DropoutReason"].ToString(),
                        DropoutDate = Convert.ToDateTime(dr["DropoutDate"].ToString()),
                        BatchName = dr["BatchName"].ToString(),
                        BatchSerial = Convert.ToInt32(dr["BatchSerial"].ToString()),
                        FullName = dr["FullName"].ToString(),
                        StudentSerial = Convert.ToInt32(dr["StudentSerial"].ToString()),
                        Year = Convert.ToInt32(dr["Year"].ToString())
                    });
                }
            }
            return list;
        }


        //Get batches according to year status = active
        public JsonResult GetBatchesByYear(int year)
        {
            List<SelectListItem> batch = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetBatchesByYear", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    batch.Add(new SelectListItem
                    {
                        Value = rdr["BatchSerial"].ToString(),
                        Text = rdr["BatchName"].ToString()
                    });
                }
            }

            return Json(batch, JsonRequestBehavior.AllowGet);
        }


        //get classess by filtering selected batch, status = active
        public JsonResult GetClassesByBatch(int batchSerial)
        {
            List<SelectListItem> classes = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetClassesByBatch", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BatchSerial", batchSerial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    classes.Add(new SelectListItem
                    {
                        Value = rdr["ClassSerial"].ToString(),
                        Text = rdr["ClassName"].ToString()
                    });
                }
            }

            return Json(classes, JsonRequestBehavior.AllowGet);
        }

        //Get batches according to year 
        public JsonResult GetBatchesByYearBothStatus(int year)
        {
            List<SelectListItem> batch = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetBatchesByYearBothStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    batch.Add(new SelectListItem
                    {
                        Value = rdr["BatchSerial"].ToString(),
                        Text = rdr["BatchName"].ToString()
                    });
                }
            }

            return Json(batch, JsonRequestBehavior.AllowGet);
        }


        //get classess by filtering selected batch
        public JsonResult GetClassesByBatchBothStatus(int batchSerial)
        {
            List<SelectListItem> classes = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetClassesByBatchBothStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BatchSerial", batchSerial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    classes.Add(new SelectListItem
                    {
                        Value = rdr["ClassSerial"].ToString(),
                        Text = rdr["ClassName"].ToString()
                    });
                }
            }

            return Json(classes, JsonRequestBehavior.AllowGet);
        }

    }
}
