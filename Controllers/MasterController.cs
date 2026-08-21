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
    public class MasterController : Controller
    {
        private readonly string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //
        // GET: /Master/AddGrade ,Display the Add Grade form along with the existing grade list

        public ActionResult AddGrade()
        {
            var grades = GetAllGrades();  // Retrieve all grades from the database
            return View(grades);
        }

        // POST: Handle form submission to add a new grade
        [HttpPost]
        public ActionResult AddGrade(GradeModels model)
        {
            
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    if (model.GradeSerial != 0)
                    {
                        // Update logic 
                        SqlCommand cmd = new SqlCommand("UpdateGrade", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@GradeSerial", model.GradeSerial); 
                        cmd.Parameters.AddWithValue("@GradeName", model.GradeName); 
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
                            TempData["Message"] = "Grade already exists!"; //alert when grade is already exists
                        }
                        else
                        {
                            TempData["Message"] = "Updated successfully!"; //alert for success
                        }
                    }
                    else
                    {
                        // Insert logic 
                        SqlCommand cmd = new SqlCommand("InsertGrade", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@GradeName", model.GradeName); 
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
                            TempData["Message"] = "Grade already exists!";
                        }
                        else
                        {
                            TempData["Message"] = "Saved successfully!"; //alert for success
                        }
                    }
                    
                }

                return RedirectToAction("AddGrade"); // Redirects and shows message
            

            var grades = GetAllGrades(); // reload updated list
            return View(grades); // Return view with model 
        }

        //get all grades to table
        private List<GradeModels> GetAllGrades()
        {
            List<GradeModels> list = new List<GradeModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllGrades", con);   // Retrieve all grades from the database
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new GradeModels
                    {
                        No = i++,
                        GradeSerial = Convert.ToInt32(dr["GradeSerial"].ToString()),
                        GradeName = dr["GradeName"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
            }
            return list;
        }


        //Get teachers to dropdown for batch head dropdown
        private List<TeacherModels> GetAllTeachersActiveForBatchHead()
        {
            List<TeacherModels> list = new List<TeacherModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllTeachersActiveForBatchHead", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new TeacherModels
                    {
                        TeacherSerial = Convert.ToInt32(dr["TeacherSerial"].ToString()),
                        FullName = dr["FullName"].ToString()
                    });
                }
            }
            return list;
        }


        //
        // GET: /Master/AddBatch 

        public ActionResult AddBatch()
        {
            var batches = GetAllBatches();   // Retrieve all batches from the database
            var grades = GetAllGradesActive();
            var teachers = GetAllTeachersActive();

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

            ViewBag.Teachers = teachers;
            ViewBag.Grades = grades;
            return View(batches);
        }

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

        //Get teachers to dropdown
        private List<TeacherModels> GetAllTeachersActive()
        {
            List<TeacherModels> list = new List<TeacherModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllTeachersActive", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new TeacherModels
                    {
                        TeacherSerial = Convert.ToInt32(dr["TeacherSerial"].ToString()),
                        FullName = dr["FullName"].ToString()
                    });
                }
            }
            return list;
        }


        //Years load to year dropdown
        public ActionResult YearLoad()
        {
            List<int> years = new List<int>();
            int currentYear = DateTime.Now.Year;

            for (int i = currentYear + 1; i >= 2020; i--) // load years sincce 2020
            {
                years.Add(i);
            }

            ViewBag.Years = new SelectList(years.Select(y => new SelectListItem
            {
                Text = y.ToString(),
                Value = y.ToString()
            }), "Value", "Text");

            return View();

        }

        //POST: Handle form submission to add a new batches
        [HttpPost]
        public ActionResult AddBatch(BatchModels model)
        {

            using (SqlConnection con = new SqlConnection(conStr))
            {
                if (model.BatchSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateBatch", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@BatchName", model.BatchName);
                    cmd.Parameters.AddWithValue("@GradeSerial", model.GradeSerial);
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@TeacherSerial", model.TeacherSerial);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "Batch already exists!";
                    }
                    else
                    {
                        TempData["Message"] = "Updated successfully!"; //alert
                    }
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertBatch", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BatchName", model.BatchName);
                    cmd.Parameters.AddWithValue("@GradeSerial", model.GradeSerial);
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@StartDate", model.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", model.EndDate);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@TeacherSerial", model.TeacherSerial);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "Batch already exists!";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!"; // alert
                    }
                }

            }

            return RedirectToAction("AddBatch"); // Redirects and shows message


            var batch = GetAllBatches(); // reload updated list
            return View(batch); // Return view with model 
        }

        //get all batches to table
        private List<BatchModels> GetAllBatches()
        {
            List<BatchModels> list = new List<BatchModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllBatches", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new BatchModels
                    {
                        No = i++,
                        BatchSerial = Convert.ToInt32(dr["BatchSerial"].ToString()),
                        BatchName = dr["BatchName"].ToString(),
                        GradeSerial = Convert.ToInt32(dr["GradeSerial"].ToString()),
                        GradeName = dr["GradeName"].ToString(),
                        Year = Convert.ToInt32(dr["Year"].ToString()),
                        StartDate = Convert.ToDateTime(dr["StartDate"].ToString()),
                        EndDate = Convert.ToDateTime(dr["EndDate"].ToString()),
                        Status = dr["Status"].ToString(),
                        TeacherSerial = Convert.ToInt32(dr["TeacherSerial"].ToString()),
                        FullName = dr["FullName"].ToString(),
                    });
                }
            }
            return list;
        }

        //Get teachers to dropdown for class head teacher
        private List<TeacherModels> GetAllTeachersActiveForClassTeacher()
        {
            List<TeacherModels> list = new List<TeacherModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllTeachersActiveForClassTeacher", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new TeacherModels
                    {
                        TeacherSerial = Convert.ToInt32(dr["TeacherSerial"].ToString()),
                        FullName = dr["FullName"].ToString()
                    });
                }
            }
            return list;
        }


        //
        // GET: /Master/AddClass

        public ActionResult AddClass()
        {
            var teachers = GetAllTeachersActive();
            var classes = GetAllClasses();

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

            ViewBag.Teachers = teachers;
            ViewBag.Batches = new SelectList(new List<SelectListItem>());
            return View(classes);
        }



        //Get batches to dropdown
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

        //get all active batches,serial, name and year
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


        // POST: Handle form submission to add a new class
        [HttpPost]
        public ActionResult AddClass(ClassModels model)
        {

            using (SqlConnection con = new SqlConnection(conStr))
            {
                if (model.ClassSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateClass", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@ClassName", model.ClassName);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@Capacity", model.Capacity);
                    cmd.Parameters.AddWithValue("@TeacherSerial", model.TeacherSerial);

                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "Class already exists!";
                    }
                    else
                    {
                        TempData["Message"] = "Updated successfully!";
                    }
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertClass", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ClassName", model.ClassName);
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@Capacity", model.Capacity);
                    cmd.Parameters.AddWithValue("@TeacherSerial", model.TeacherSerial);
                    // Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    int result = (int)returnParam.Value;
                    con.Close();

                    if (result == -1)
                    {
                        TempData["Message"] = "Class already exists!";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!";
                    }
                }

            }

            return RedirectToAction("AddClass"); // Redirects and shows message


            var classes = GetAllClasses(); // reload updated list
            return View(classes); // Return view with model 
        }

        //get all classes to table
        private List<ClassModels> GetAllClasses()
        {
            List<ClassModels> list = new List<ClassModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllClasses", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new ClassModels
                    {
                        No = i++,
                        ClassSerial = Convert.ToInt32(dr["ClassSerial"].ToString()),
                        ClassName = dr["ClassName"].ToString(),
                        Status = dr["Status"].ToString(),
                        BatchName = dr["BatchName"].ToString(),
                        BatchSerial = Convert.ToInt32(dr["BatchSerial"].ToString()),
                        FullName = dr["FullName"].ToString(),
                        TeacherSerial = Convert.ToInt32(dr["TeacherSerial"].ToString()),
                        Capacity =  dr.IsDBNull(dr.GetOrdinal("Capacity")) ? (int?)null : dr.GetInt32(dr.GetOrdinal("Capacity")),
                        Year = Convert.ToInt32(dr["Year"].ToString())
                    });
                }
            }
            return list;
        }


        //
        // GET: /Master/AddSection

        public ActionResult AddSection()
        {
            var sections = GetAllSections();
            var teachers = GetAllTeachersActive();

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

            ViewBag.Teachers = teachers;
            ViewBag.Batches = new SelectList(new List<SelectListItem>());
            return View(sections);

        }

        //Get grades to dropdown
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
                        GradeName = dr["GradeName"].ToString()
                    });
                }
            }
            return list;
        }

        // POST: Handle form submission to add a new section
        [HttpPost]
        public ActionResult AddSection(SectionModels model)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                if (model.SectionSerial != 0)
                {
                    // Update logic only for existing section
                    SqlCommand updateCmd = new SqlCommand("UpdateSection", con);
                    updateCmd.CommandType = CommandType.StoredProcedure;
                    updateCmd.Parameters.AddWithValue("@SectionSerial", model.SectionSerial);
                    updateCmd.Parameters.AddWithValue("@SectionName", model.SectionName);
                    updateCmd.Parameters.AddWithValue("@Year", model.Year);
                    updateCmd.Parameters.AddWithValue("@Status", model.Status);
                    updateCmd.Parameters.AddWithValue("@TeacherSerial", model.TeacherSerial);
                    updateCmd.ExecuteNonQuery();

                    // Step 2: Delete existing section-batch mappings
                    SqlCommand deleteCmd = new SqlCommand("DELETE FROM SectionDetail WHERE SectionSerial = @SectionSerial", con);
                    deleteCmd.Parameters.AddWithValue("@SectionSerial", model.SectionSerial);
                    deleteCmd.ExecuteNonQuery();

                    // Step 3: Insert new section-batch mappings
                    if (model.BatchSerials != null && model.BatchSerials.Any())
                    {
                        foreach (var batchSerial in model.BatchSerials)
                        {
                            SqlCommand insertDetailCmd = new SqlCommand("InsertSectionDetailsBatch", con);
                            insertDetailCmd.CommandType = CommandType.StoredProcedure;
                            insertDetailCmd.Parameters.AddWithValue("@SectionSerial", model.SectionSerial);
                            insertDetailCmd.Parameters.AddWithValue("@BatchSerial", batchSerial);
                            insertDetailCmd.ExecuteNonQuery();
                        }
                    }

                    TempData["Message"] = "Updated successfully!";
                }
                else
                {
                    // Step 1: Insert into Sections table and get inserted SectionSerial
                    SqlCommand insertCmd = new SqlCommand("InsertSection", con);
                    insertCmd.CommandType = CommandType.StoredProcedure;
                    insertCmd.Parameters.AddWithValue("@SectionName", model.SectionName);
                    insertCmd.Parameters.AddWithValue("@Year", model.Year);
                    insertCmd.Parameters.AddWithValue("@Status", model.Status);
                    insertCmd.Parameters.AddWithValue("@TeacherSerial", model.TeacherSerial);

                    SqlParameter resultCodeParam = new SqlParameter("@ResultCode", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    SqlParameter newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    insertCmd.Parameters.Add(resultCodeParam);
                    insertCmd.Parameters.Add(newIdParam);
                    insertCmd.ExecuteNonQuery();
                    int resultCode = (int)insertCmd.Parameters["@ResultCode"].Value;

                    if (resultCode == 1)
                    {
                        int newSectionSerial = (int)insertCmd.Parameters["@NewId"].Value;
                        // Step 2: Insert into mapping table (SectionBatchMapping)
                        if (model.BatchSerials != null && model.BatchSerials.Any())
                        {
                            foreach (var batchSerial in model.BatchSerials)
                            {
                                SqlCommand mapCmd = new SqlCommand("InsertSectionDetailsBatch", con);
                                mapCmd.CommandType = CommandType.StoredProcedure;
                                mapCmd.Parameters.AddWithValue("@SectionSerial", newSectionSerial);
                                mapCmd.Parameters.AddWithValue("@BatchSerial", batchSerial);
                                mapCmd.ExecuteNonQuery();
                            }
                        }

                        TempData["Message"] = "Saved successfully!";
                    }
                    else if (resultCode == -1)
                    {
                        TempData["Message"] = "Section already exists!";
                    }


                }

                con.Close();
            }

            return RedirectToAction("AddSection");
        }



        //get all sections to table
        private List<SectionModels> GetAllSections()
        {
            List<SectionModels> list = new List<SectionModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllSections", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new SectionModels
                    {
                        No = i++,
                        SectionSerial = Convert.ToInt32(dr["SectionSerial"].ToString()),
                        SectionName = dr["SectionName"].ToString(),
                        Year = Convert.ToInt32(dr["Year"].ToString()),
                        Status = dr["Status"].ToString(),
                        TeacherSerial = Convert.ToInt32(dr["TeacherSerial"].ToString()),
                        FullName = dr["FullName"].ToString()
                    });
                }
            }
            return list;
        }

        //Get Section Detail data

        [HttpGet]
        public JsonResult GetSectionDetails(int sectionSerial)
        {
            List<object> batchList = new List<object>();


            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand("GetSectionDetailsBySectionSerial", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SectionSerial", sectionSerial);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        batchList.Add(new
                        {
                            BatchSerial = reader["BatchSerial"],
                            BatchName = reader["BatchName"].ToString()
                        });
                    }
                }
            }

            return Json(batchList, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Master/AddSubject

        public ActionResult AddSubject()
        {
            var subjects = GetAllSubjects();
            var grades = GetAllGradesActive();

            // Pass both sections and grades to the view
            ViewBag.Grades = grades;
            return View(subjects);
        }


        // POST: Handle form submission to add a new subjects
        [HttpPost]
        public ActionResult AddSubject(SubjectModels model)
        {

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                if (model.SubjectSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateSubject", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
                    cmd.Parameters.AddWithValue("@SubjectName", model.SubjectName);
                    cmd.Parameters.AddWithValue("@Medium", model.Medium);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                   
                    cmd.ExecuteNonQuery();

                    // Step 2: Delete existing subject-grade mappings
                    SqlCommand deleteCmd = new SqlCommand("DELETE FROM SubjectDetail WHERE SubjectSerial = @SubjectSerial", con);
                    deleteCmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
                    deleteCmd.ExecuteNonQuery();

                    // Step 3: Insert new subject-grade mappings
                    if (model.GradeSerials != null && model.GradeSerials.Any())
                    {
                        foreach (var gradeSerial in model.GradeSerials)
                        {
                            SqlCommand insertDetailCmd = new SqlCommand("InsertSubjectDetailsGrade", con);
                            insertDetailCmd.CommandType = CommandType.StoredProcedure;
                            insertDetailCmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
                            insertDetailCmd.Parameters.AddWithValue("@GradeSerial", gradeSerial);
                            insertDetailCmd.ExecuteNonQuery();
                        }
                    }
                    TempData["Message"] = "Updated successfully!";
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertSubject", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SubjectName", model.SubjectName);
                    cmd.Parameters.AddWithValue("@Medium", model.Medium);
                    cmd.Parameters.AddWithValue("@Status", model.Status);

                    SqlParameter resultCodeParam = new SqlParameter("@ResultCode", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    SqlParameter newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(resultCodeParam);
                    cmd.Parameters.Add(newIdParam);
                    cmd.ExecuteNonQuery();
                    int resultCode = (int)cmd.Parameters["@ResultCode"].Value;

                    if (resultCode == 1)
                    {
                        int newSectionSerial = (int)cmd.Parameters["@NewId"].Value;
                        // Step 2: Insert into mapping table (SubjectGradeMapping)
                        if (model.GradeSerials != null && model.GradeSerials.Any())
                        {
                            foreach (var gradeSerial in model.GradeSerials)
                            {
                                SqlCommand mapCmd = new SqlCommand("InsertSubjectDetailsGrade", con);
                                mapCmd.CommandType = CommandType.StoredProcedure;
                                mapCmd.Parameters.AddWithValue("@SubjectSerial", newSectionSerial);
                                mapCmd.Parameters.AddWithValue("@GradeSerial", gradeSerial);
                                mapCmd.ExecuteNonQuery();
                            }
                        }

                        TempData["Message"] = "Saved successfully!";
                    }


                    else if (resultCode == -1)
                    {
                        TempData["Message"] = "Section already exists!";
                    }
                }

                con.Close();
            }

            return RedirectToAction("AddSubject"); // Redirects and shows message


            var subjects = GetAllSubjects(); // reload updated list
            return View(subjects); // Return view with model 
        }


        //get all subjects to table
        private List<SubjectModels> GetAllSubjects()
        {
            List<SubjectModels> list = new List<SubjectModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllSubjects", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new SubjectModels
                    {
                        No = i++,
                        SubjectSerial = Convert.ToInt32(dr["SubjectSerial"].ToString()),
                        SubjectName = dr["SubjectName"].ToString(),
                        Medium = dr["Medium"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
            }
            return list;
        }


        //  get subject detail data, 
        [HttpGet]
        public JsonResult GetSubjectDetails(int subjectserial)
        {
            List<object> gradeList = new List<object>();


            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand("GetSubjectDetailsBySubjectSerial", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        gradeList.Add(new
                        {
                            GradeSerial = reader["GradeSerial"],
                            GradeName = reader["GradeName"].ToString()
                        });
                    }
                }
            }

            return Json(gradeList, JsonRequestBehavior.AllowGet);
        }


        //
        // GET: /Master/AddTerm

        public ActionResult AddTerm()
        {
            var terms = GetAllTerms();
            return View(terms);
        }


        // POST: Handle form submission to add a new terms
        [HttpPost]
        public ActionResult AddTerm(TermModels model)
        {

            using (SqlConnection con = new SqlConnection(conStr))
            {
                if (model.TermSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateTerm", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
                    cmd.Parameters.AddWithValue("@TermName", model.TermName);
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
                        TempData["Message"] = "Term already exists!";
                    }
                    else
                    {
                        TempData["Message"] = "Updated successfully!";
                    }
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertTerm", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TermName", model.TermName);
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
                        TempData["Message"] = "Term already exists!";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!";
                    }
                }

            }

            return RedirectToAction("AddTerm"); // Redirects and shows message


            var terms = GetAllTerms(); // reload updated list
            return View(terms); // Return view with model 
        }

        //get all terms to table
        private List<TermModels> GetAllTerms()
        {
            List<TermModels> list = new List<TermModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllTerms", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new TermModels
                    {
                        No = i++,
                        TermSerial = Convert.ToInt32(dr["TermSerial"].ToString()),
                        TermName = dr["TermName"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
            }
            return list;
        }

        //Get terms to dropdown
        private List<TermModels> GetAllTermsActive()
        {
            List<TermModels> list = new List<TermModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllTermsActive", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new TermModels
                    {
                        TermSerial = Convert.ToInt32(dr["TermSerial"].ToString()),
                        TermName = dr["TermName"].ToString()
                    });
                }
            }
            return list;
        }

        //
        // GET: /Master/AddExam

        public ActionResult AddExam()
        {
            var exams = GetAllExams();
            var terms = GetAllTermsActive();

            // Pass terms
            ViewBag.Terms = terms;
            return View(exams);
        }

        // POST: Handle form submission to add a new exam
        [HttpPost]
        public ActionResult AddExam(ExamModels model)
        {

            using (SqlConnection con = new SqlConnection(conStr))
            {
                if (model.ExamSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateExam", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExamSerial", model.ExamSerial);
                    cmd.Parameters.AddWithValue("@ExamName", model.ExamName);
                    cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
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
                        TempData["Message"] = "Exam already exists!";
                    }
                    else
                    {
                        TempData["Message"] = "Updated successfully!";
                    }
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertExam", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExamName", model.ExamName);
                    cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
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
                        TempData["Message"] = "Exam already exists!";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!";
                    }
                }

            }

            return RedirectToAction("AddExam"); // Redirects and shows message


            var exams = GetAllExams(); // reload updated list
            return View(exams); // Return view with model 
        }

        //get all exams to table
        private List<ExamModels> GetAllExams()
        {
            List<ExamModels> list = new List<ExamModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllExams", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new ExamModels
                    {
                        No = i++,
                        ExamSerial = Convert.ToInt32(dr["ExamSerial"].ToString()),
                        ExamName = dr["ExamName"].ToString(),
                        TermSerial = Convert.ToInt32(dr["TermSerial"].ToString()),
                        TermName = dr["TermName"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
            }
            return list;
        }


        //get classess by filtering selected batch
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

        //get subjects by filtering selected batch
        public JsonResult GetSubjectsByBatch(int batchSerial)
        {
            List<SelectListItem> subjects = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetSubjectsByBatch", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BatchSerial", batchSerial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    subjects.Add(new SelectListItem
                    {
                        Value = rdr["SubjectSerial"].ToString(),
                        Text = rdr["SubjectName"].ToString()
                    });
                }
            }

            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        //get teacher by filtering selected subject
        public JsonResult GetTeachersBySubject(int subjectserial)
        {
            List<SelectListItem> teachers = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetTeachersBySubject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    teachers.Add(new SelectListItem
                    {
                        Value = rdr["TeacherSerial"].ToString(),
                        Text = rdr["FullName"].ToString()
                    });
                }
            }

            return Json(teachers, JsonRequestBehavior.AllowGet);
        }


        //
        // GET: /Master/AddClassSubjectTeacher

        public ActionResult AddClassSubjectTeacher()
        {
            var clz = GetAllAddClassSubjectTeacher();

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

            return View(clz);
        }


        [HttpPost]
        public ActionResult AddClassSubjectTeacher(ClassSubjectTeacherModels model)
        {

            using (SqlConnection con = new SqlConnection(conStr))
            {
                if (model.ClassSubjectTeacherSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateClassSubjectTeacher", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ClassSubjectTeacherSerial", model.ClassSubjectTeacherSerial);
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                    cmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
                    cmd.Parameters.AddWithValue("@TeacherSerial", model.TeacherSerial);
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
                        TempData["Message"] = "A Teacher assign to this subject already!";
                    }
                    else
                    {
                        TempData["Message"] = "Updated successfully!";
                    }
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertClassSubjectTeacher", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                    cmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
                    cmd.Parameters.AddWithValue("@TeacherSerial", model.TeacherSerial);
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
                        TempData["Message"] = "A Teacher assign to this subject already!";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!";
                    }
                }

            }

            return RedirectToAction("AddClassSubjectTeacher"); // Redirects and shows message


            var clz = GetAllAddClassSubjectTeacher(); // reload updated list
            return View(clz); // Return view with model 
        }

        //get all exams to table
        private List<ClassSubjectTeacherModels> GetAllAddClassSubjectTeacher()
        {
            List<ClassSubjectTeacherModels> list = new List<ClassSubjectTeacherModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllAddClassSubjectTeacher", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new ClassSubjectTeacherModels
                    {
                        No = i++,
                        ClassSubjectTeacherSerial = Convert.ToInt32(dr["ClassSubjectTeacherSerial"].ToString()),
                        Year = Convert.ToInt32(dr["Year"].ToString()),
                        BatchSerial = Convert.ToInt32(dr["BatchSerial"].ToString()),
                        BatchName = dr["BatchName"].ToString(),
                        ClassSerial = Convert.ToInt32(dr["ClassSerial"].ToString()),
                        ClassName = dr["ClassName"].ToString(),
                        SubjectSerial = Convert.ToInt32(dr["SubjectSerial"].ToString()),
                        SubjectName = dr["SubjectName"].ToString(),
                        TeacherSerial = Convert.ToInt32(dr["TeacherSerial"].ToString()),
                        FullName = dr["FullName"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
            }
            return list;
        }
    }
}
