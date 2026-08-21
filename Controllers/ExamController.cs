using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using CrystalDecisions.CrystalReports.Engine;
using MvcApplication1.Models;
using MvcApplication1.Report;

namespace MvcApplication1.Controllers
{
    public class ExamController : Controller
    {

        private readonly string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


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


        //get exam by filtering selected term only assignments
        public JsonResult GetExamsByTerm(int termserial)
        {
            List<SelectListItem> exams = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetExamsByTerm", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TermSerial", termserial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    exams.Add(new SelectListItem
                    {
                        Value = rdr["ExamSerial"].ToString(),
                        Text = rdr["ExamName"].ToString()
                    });
                }
            }

            return Json(exams, JsonRequestBehavior.AllowGet);
        }

        //get classess by filtering selected batch and logging teacher serial
        public JsonResult GetClassesByBatchByTeacherSerial(int batchSerial)
        {
            int teacherserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;

            List<SelectListItem> classes = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetClassesByBatchByTeacherSerial", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BatchSerial", batchSerial);
                cmd.Parameters.AddWithValue("@TeacherSerial", teacherserial);
                cmd.Parameters.AddWithValue("@UserSerial", userserial);

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

        //Get batches according to year and llogging batch head teacher
        public JsonResult GetBatchesByYearBothStatusByBatchHead(int year)
        {
            int teacherserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;


            List<SelectListItem> batch = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetBatchesByYearBothStatusByBatchHead", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@TeacherSerial", teacherserial);
                cmd.Parameters.AddWithValue("@UserSerial", userserial);

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



        //get subjects by filtering selected batch and teacher serial
        public JsonResult GetSubjectsByBatchByTeacher(int batchSerial)
        {
            int teacherserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;

            List<SelectListItem> subjects = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetSubjectsByBatchByTeacher", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BatchSerial", batchSerial);
                cmd.Parameters.AddWithValue("@TeacherSerial", teacherserial);

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

        //get subjects by filtering selected class and teacher serial
        public JsonResult GetSubjectsByClassByTeacher(int classserial)
        {
            int teacherserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;

            List<SelectListItem> subjects = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetSubjectsByClassByTeacher", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@TeacherSerial", teacherserial);
                cmd.Parameters.AddWithValue("@UserSerial", userserial);

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

        //get classess by filtering selected batch and logging Class teacher serial
        public JsonResult GetClassesByBatchByClassTeacher(int batchSerial)
        {
            int teacherserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;


            List<SelectListItem> classes = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetClassesByBatchByClassTeacher", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BatchSerial", batchSerial);
                cmd.Parameters.AddWithValue("@TeacherSerial", teacherserial);
                cmd.Parameters.AddWithValue("@UserSerial", userserial);


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

        //
        // GET: /ExamSchedule/

        public ActionResult ExamSchedule()
        {
            var exams = GetAllExamsScheduleTeacherSerial();
            var terms = GetAllTermsActive(); //for term dropdown


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


            // Pass terms
            ViewBag.Terms = terms;
            return View(exams);
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


        // POST: Handle form submission to add exam schedule / assignments
        [HttpPost]
        public ActionResult ExamSchedule(ExamSceduleAssignmentModels model)
        {
            int teacherserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;

            if (model.Status == "Held" && model.DateTime > DateTime.Now)
            {
                TempData["Message"] = "Exam status cannot be set to 'Held' for future dates.";
                return RedirectToAction("ExamSchedule");
            }
            if (model.Status == "Pending" && model.DateTime < DateTime.Now)
            {
                TempData["Message"] = "Exam date and time cannot be in the past.";
                return RedirectToAction("ExamSchedule");
            }

            // Weekend validation
            if (model.DateTime.DayOfWeek == DayOfWeek.Saturday || model.DateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                TempData["Message"] = "Assignments cannot be scheduled on weekends (Saturday and Sunday).";
                return RedirectToAction("ExamSchedule");
            }

            // School hours validation (8:00 AM to 1:00 PM)
            var timeOfDay = model.DateTime.TimeOfDay;
            var startTimeLimit = new TimeSpan(8, 0, 0); // 8:00 AM
            var endTimeLimit = new TimeSpan(13, 0, 0);  // 1:00 PM
            if (timeOfDay < startTimeLimit || timeOfDay > endTimeLimit)
            {
                TempData["Message"] = "Assignments can only be scheduled between 8:00 AM and 1:00 PM.";
                return RedirectToAction("ExamSchedule");
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                // Validate if there's any conflicting assignment for the same class within 30 minutes
               

                using (SqlCommand checkCmd = new SqlCommand("CheckExamScheduleConflict", con))
                {
                    checkCmd.CommandType = CommandType.StoredProcedure;

                    checkCmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                    checkCmd.Parameters.AddWithValue("@ExamScheduleAssignmentSerial", model.ExamScheduleAssignmentSerial);
                    checkCmd.Parameters.AddWithValue("@DateTime", model.DateTime);

                    con.Open();
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    con.Close();

                    if (count > 0)
                    {
                        TempData["Message"] = "Conflict: Another assignment is already scheduled for this class within a 30-minute interval.";
                        return RedirectToAction("ExamSchedule");
                    }
                }

                if (model.ExamScheduleAssignmentSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateExamScheduleAssignment", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExamScheduleAssignmentSerial", model.ExamScheduleAssignmentSerial);
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                    cmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
                    cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
                    cmd.Parameters.AddWithValue("@ExamSerial", model.ExamSerial);
                    cmd.Parameters.AddWithValue("@DateTime", model.DateTime);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@ExamType", (object)model.ExamType ?? DBNull.Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    TempData["Message"] = "Updated successfully!";
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertExamScheduleAssignment", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                    cmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
                    cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
                    cmd.Parameters.AddWithValue("@ExamSerial", model.ExamSerial);
                    cmd.Parameters.AddWithValue("@DateTime", model.DateTime);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@TeacherSerial", teacherserial);
                    cmd.Parameters.AddWithValue("@UserSerial", userserial);
                    cmd.Parameters.AddWithValue("@ExamType", (object)model.ExamType ?? DBNull.Value);

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

            return RedirectToAction("ExamSchedule"); // Redirects and shows message


            var exams = GetAllExamsScheduleTeacherSerial(); // reload updated list
            return View(exams); // Return view with model 
        }

        //get all exams to table only assignments
        private List<ExamScheduleModels> GetAllExamsSchedule()
        {
            List<ExamScheduleModels> list = new List<ExamScheduleModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllExamSchedules", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new ExamScheduleModels
                    {
                        No = i++,
                        ExamScheduleSerial = Convert.ToInt32(dr["ExamScheduleSerial"].ToString()),
                        Year = Convert.ToInt32(dr["Year"].ToString()),
                        BatchSerial = Convert.ToInt32(dr["BatchSerial"].ToString()),
                        BatchName = dr["BatchName"].ToString(),
                        ClassSerial = Convert.ToInt32(dr["ClassSerial"].ToString()),
                        ClassName = dr["ClassName"].ToString(),
                        SubjectSerial = Convert.ToInt32(dr["SubjectSerial"].ToString()),
                        SubjectName = dr["SubjectName"].ToString(),
                        ExamSerial = Convert.ToInt32(dr["ExamSerial"].ToString()),
                        ExamName = dr["ExamName"].ToString(),
                        TermSerial = Convert.ToInt32(dr["TermSerial"].ToString()),
                        TermName = dr["TermName"].ToString(),
                        Status = dr["Status"].ToString(),
                        DateTime = Convert.ToDateTime(dr["DateTime"].ToString()),
                        ExamType = dr["ExamType"] != DBNull.Value ? dr["ExamType"].ToString() : null

                    });
                }
            }
            return list;
        }


        //get all exams to table only assignments filter with teacher serial
        private List<ExamSceduleAssignmentModels> GetAllExamsScheduleTeacherSerial()
        {
            int teacherserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;

            List<ExamSceduleAssignmentModels> list = new List<ExamSceduleAssignmentModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllExamsScheduleTeacherSerial", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TeacherSerial", teacherserial);
                cmd.Parameters.AddWithValue("@UserSerial", userserial);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new ExamSceduleAssignmentModels
                    {
                        No = i++,
                        ExamScheduleAssignmentSerial = Convert.ToInt32(dr["ExamScheduleAssignmentSerial"].ToString()),
                        Year = Convert.ToInt32(dr["Year"].ToString()),
                        BatchSerial = Convert.ToInt32(dr["BatchSerial"].ToString()),
                        BatchName = dr["BatchName"].ToString(),
                        ClassSerial = Convert.ToInt32(dr["ClassSerial"].ToString()),
                        ClassName = dr["ClassName"].ToString(),
                        SubjectSerial = Convert.ToInt32(dr["SubjectSerial"].ToString()),
                        SubjectName = dr["SubjectName"].ToString(),
                        ExamSerial = Convert.ToInt32(dr["ExamSerial"].ToString()),
                        ExamName = dr["ExamName"].ToString(),
                        TermSerial = Convert.ToInt32(dr["TermSerial"].ToString()),
                        TermName = dr["TermName"].ToString(),
                        Status = dr["Status"].ToString(),
                        DateTime = Convert.ToDateTime(dr["DateTime"].ToString()),
                        ExamType = dr["ExamType"] != DBNull.Value ? dr["ExamType"].ToString() : null

                    });
                }
            }
            return list;
        }


        //
        // GET: /FinalExamSchedule/

        public ActionResult FinalExamSchedule()
        {
            var exams = GetAllExamsScheduleFinal();
            var terms = GetAllTermsActive(); //for term dropdown


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


            // Pass terms
            ViewBag.Terms = terms;
            return View(exams);
        }


        // POST: Handle form submission to add exam schedule / final
        [HttpPost]
        public ActionResult FinalExamSchedule(ExamScheduleModels model)
        {
            int teacherserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;

            if (model.Status == "Held" && model.DateTime > DateTime.Now)
            {
                TempData["Message"] = "Exam status cannot be set to 'Held' for future dates.";
                return RedirectToAction("FinalExamSchedule");
            }
            if (model.Status == "Pending" && model.DateTime < DateTime.Now)
            {
                TempData["Message"] = "Exam date and time cannot be in the past.";
                return RedirectToAction("FinalExamSchedule");
            }

            // Weekend validation
            if (model.DateTime.DayOfWeek == DayOfWeek.Saturday || model.DateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                TempData["Message"] = "Final exams cannot be scheduled on weekends (Saturday and Sunday).";
                return RedirectToAction("FinalExamSchedule");
            }

            // School hours validation (8:00 AM to 12:00 PM)
            var timeOfDay = model.DateTime.TimeOfDay;
            var startTimeLimit = new TimeSpan(8, 0, 0); // 8:00 AM
            var endTimeLimit = new TimeSpan(12, 0, 0);  // 12:00 PM
            if (timeOfDay < startTimeLimit || timeOfDay > endTimeLimit)
            {
                TempData["Message"] = "Final exams can only be scheduled between 8:00 AM and 12:00 PM.";
                return RedirectToAction("FinalExamSchedule");
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                // Validate if there's any conflicting final exam for the same batch within 90 minutes
              
                using (SqlCommand checkCmd = new SqlCommand("CheckFinalExamScheduleConflict", con))
                {

                    checkCmd.CommandType = CommandType.StoredProcedure;

                    checkCmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    checkCmd.Parameters.AddWithValue("@ExamScheduleSerial", model.ExamScheduleSerial);
                    checkCmd.Parameters.AddWithValue("@DateTime", model.DateTime);

                    con.Open();
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    con.Close();

                    if (count > 0)
                    {
                        TempData["Message"] = "Conflict: Another final exam is already scheduled for this batch within a 90-minute interval.";
                        return RedirectToAction("FinalExamSchedule");
                    }
                }

                if (model.ExamScheduleSerial != 0)
                {
                    // Update logic 
                    SqlCommand cmd = new SqlCommand("UpdateExamSchedule", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExamScheduleSerial", model.ExamScheduleSerial);
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@ClassSerial", DBNull.Value);
                    cmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
                    cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
                    cmd.Parameters.AddWithValue("@ExamSerial", model.ExamSerial);
                    cmd.Parameters.AddWithValue("@DateTime", model.DateTime);
                    cmd.Parameters.AddWithValue("@Status", model.Status);

                    // Add return value parameter
                    SqlParameter returnValue = new SqlParameter();
                    returnValue.Direction = ParameterDirection.ReturnValue;
                    returnValue.DbType = DbType.Int32;
                    cmd.Parameters.Add(returnValue);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    int result = Convert.ToInt32(returnValue.Value);

                    if (result == -1)
                    {
                        TempData["Message"] = "Cannot update: Exam status is 'Held' but not approved by Principal.";
                    }
                    else
                    {
                        TempData["Message"] = "Updated successfully!";
                    }
                }
                else
                {
                    // Insert logic 
                    SqlCommand cmd = new SqlCommand("InsertExamSchedule", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Year", model.Year);
                    cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                    cmd.Parameters.AddWithValue("@ClassSerial", DBNull.Value);
                    cmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
                    cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
                    cmd.Parameters.AddWithValue("@ExamSerial", model.ExamSerial);
                    cmd.Parameters.AddWithValue("@DateTime", model.DateTime);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@TeacherSerial", teacherserial);
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
                        TempData["Message"] = "Exam already exists!";
                    }
                    else
                    {
                        TempData["Message"] = "Saved successfully!";
                    }
                }

            }

            return RedirectToAction("FinalExamSchedule"); // Redirects and shows message


            var exams = GetAllExamsScheduleFinal(); // reload updated list
            return View(exams); // Return view with model 
        }


        //get all exams to table only final
        private List<ExamScheduleModels> GetAllExamsScheduleFinal()
        {

            int teacherserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;

            List<ExamScheduleModels> list = new List<ExamScheduleModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllExamSchedulesFinal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TeacherSerial", teacherserial);
                cmd.Parameters.AddWithValue("@UserSerial", userserial);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                int i = 1;
                while (dr.Read())
                {
                    list.Add(new ExamScheduleModels
                    {
                        No = i++,
                        ExamScheduleSerial = Convert.ToInt32(dr["ExamScheduleSerial"].ToString()),
                        Year = Convert.ToInt32(dr["Year"].ToString()),
                        BatchSerial = Convert.ToInt32(dr["BatchSerial"].ToString()),
                        BatchName = dr["BatchName"].ToString(),
                        SubjectSerial = Convert.ToInt32(dr["SubjectSerial"].ToString()),
                        SubjectName = dr["SubjectName"].ToString(),
                        ExamSerial = Convert.ToInt32(dr["ExamSerial"].ToString()),
                        ExamName = dr["ExamName"].ToString(),
                        TermSerial = Convert.ToInt32(dr["TermSerial"].ToString()),
                        TermName = dr["TermName"].ToString(),
                        Status = dr["Status"].ToString(),
                        DateTime = Convert.ToDateTime(dr["DateTime"].ToString())

                    });
                }
            }
            return list;
        }




        //get exam by filtering selected term only for final
        public JsonResult GetExamsByTermForFinal(int termserial)
        {
            List<SelectListItem> exams = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetExamsByTermForFinal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TermSerial", termserial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    exams.Add(new SelectListItem
                    {
                        Value = rdr["ExamSerial"].ToString(),
                        Text = rdr["ExamName"].ToString()
                    });
                }
            }

            return Json(exams, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Exam/AttedanceAssignment

        public ActionResult AttedanceAssignment()
        {

            var terms = GetAllTermsActive(); //for term dropdown

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


            // Pass terms
            ViewBag.Terms = terms;
            var model = new List<ExamSceduleAssignmentModels>(); //  empty list on initial load
            return View(model);
        }


        //search student according to year, batch, class,term, subject, exam
        public ActionResult SearchStudentsforAssignmentAttendance(int year, int batchserial, int classserial, int termserial, int subjectserial, int examserial)
        {

            List<ExamSceduleAssignmentModels> students = new List<ExamSceduleAssignmentModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforAssignmentAttendance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new ExamSceduleAssignmentModels
                    {
                        ExamAssignmentAttedanceSerial = reader.IsDBNull(reader.GetOrdinal("ExamAssignmentAttedanceSerial"))
                                            ? (int?)null
                                            : reader.GetInt32(reader.GetOrdinal("ExamAssignmentAttedanceSerial")),
                        ExamScheduleAssignmentSerial = Convert.ToInt32(reader["ExamScheduleAssignmentSerial"]),
                        StudentSerial = Convert.ToInt32(reader["StudentSerial"]),
                        FullName = reader["FullName"].ToString(),
                        RegNo = reader["RegNo"].ToString(),
                        Attedance = reader["Attedance"] == DBNull.Value ? null : reader["Attedance"].ToString()

                    });
                }
            }
            if (students == null || !students.Any())
            {
                TempData["Message"] = "No students found.";
            }


            return PartialView("_StudentAssignmentAttendancePartial", students);


        }

        [HttpPost]
        public ActionResult AttedanceAssignment(int studentserial, string attedance, int? examassignmentattedanceserial, int year, int batchserial, int classserial, int termserial, int subjectserial, int examserial, int examscheduleassignmentserial)
        {
            string message = "";
            int result = 0;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                //update attednacen
                if (examassignmentattedanceserial != 0)
                {
                    SqlCommand cmd = new SqlCommand("UpdateStudentAttedanceAssignment", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExamScheduleAssignmentSerial", examscheduleassignmentserial);
                    cmd.Parameters.AddWithValue("@StudentSerial", studentserial);
                    cmd.Parameters.AddWithValue("@Attedance", attedance);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                    cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                    cmd.Parameters.AddWithValue("@TermSerial", termserial);
                    cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);
                    cmd.Parameters.AddWithValue("@ExamScheduleAssignmentSerial", examscheduleassignmentserial);


                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    message = "Updated successfully!";
                    result = 1;
                }

                else
                {
                    //save students in new batch and class
                    SqlCommand cmd = new SqlCommand("InsertStudentAttedanceAssignment", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StudentSerial", studentserial);
                    cmd.Parameters.AddWithValue("@Attedance", attedance);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                    cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                    cmd.Parameters.AddWithValue("@TermSerial", termserial);
                    cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);
                    cmd.Parameters.AddWithValue("@ExamSerial", examserial);
                    cmd.Parameters.AddWithValue("@ExamScheduleAssignmentSerial", examscheduleassignmentserial);


                    //Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    result = (int)returnParam.Value;
                    con.Close();

                    message = result == -1 ? "Student already exists!" : "Saved successfully!";


                }
            }
            return Json(new { success = result != -1, message = message }, JsonRequestBehavior.AllowGet);
        }


        //get exam by filtering selected term only assignments , only held ones
        public JsonResult GetExamsByTermOnlyHeld(int termserial, int subjectserial)
        {
            List<SelectListItem> exams = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetExamsByTermOnlyHeld", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    exams.Add(new SelectListItem
                    {
                        Value = rdr["ExamSerial"].ToString(),
                        Text = rdr["ExamName"].ToString()
                    });
                }
            }

            return Json(exams, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Exam/AttedanceFinal

        public ActionResult AttedanceFinal()
        {

            var terms = GetAllTermsActive(); //for term dropdown

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


            // Pass terms
            ViewBag.Terms = terms;
            var model = new List<ExamScheduleModels>(); //  empty list on initial load
            return View(model);
        }

        //get exam by filtering selected term only assignments , only held ones
        public JsonResult GetExamsByTermOnlyHeldFinal(int termserial, int subjectserial)
        {
            List<SelectListItem> exams = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetExamsByTermOnlyHeldFinal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    exams.Add(new SelectListItem
                    {
                        Value = rdr["ExamSerial"].ToString(),
                        Text = rdr["ExamName"].ToString()
                    });
                }
            }

            return Json(exams, JsonRequestBehavior.AllowGet);
        }

        //search student according to year, batch, class,term, subject, exam
        public ActionResult SearchStudentsforFinalAttendance(int year, int batchserial, int classserial, int termserial, int subjectserial, int examserial)
        {

            List<ExamScheduleModels> students = new List<ExamScheduleModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforFinalAttendance", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new ExamScheduleModels
                    {
                        ExamResultSerial = reader.IsDBNull(reader.GetOrdinal("ExamResultSerial"))
                                           ? (int?)null
                                           : reader.GetInt32(reader.GetOrdinal("ExamResultSerial")),
                        ExamScheduleSerial = Convert.ToInt32(reader["ExamScheduleSerial"]),
                        StudentSerial = Convert.ToInt32(reader["StudentSerial"]),
                        FullName = reader["FullName"].ToString(),
                        RegNo = reader["RegNo"].ToString(),
                        Attedance = reader["Attedance"] == DBNull.Value ? null : reader["Attedance"].ToString()
                    });
                }
            }
            if (students == null || !students.Any())
            {
                TempData["Message"] = "No students found.";
            }


            return PartialView("_StudentFinalAttendancePartial", students);


        }

        [HttpPost]
        public ActionResult AttedanceFinal(int studentserial, string attedance, int? examresultserial, int year, int batchserial, int classserial, int termserial, int subjectserial, int examserial, int examscheduleserial)
        {
            string message = "";
            int result = 0;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                //update attednacen
                if (examresultserial != 0)
                {
                    SqlCommand cmd = new SqlCommand("UpdateStudentAttedanceFinal", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExamResultSerial", examresultserial);
                    cmd.Parameters.AddWithValue("@StudentSerial", studentserial);
                    cmd.Parameters.AddWithValue("@Attedance", attedance);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                    cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                    cmd.Parameters.AddWithValue("@TermSerial", termserial);
                    cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);
                    cmd.Parameters.AddWithValue("@ExamSerial", examserial);
                    cmd.Parameters.AddWithValue("@ExamScheduleSerial", examscheduleserial);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    message = "Updated successfully!";
                    result = 1;
                }

                else
                {
                    //save students in new batch and class
                    SqlCommand cmd = new SqlCommand("InsertStudentAttedanceFinal", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StudentSerial", studentserial);
                    cmd.Parameters.AddWithValue("@Attedance", attedance);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                    cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                    cmd.Parameters.AddWithValue("@TermSerial", termserial);
                    cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);
                    cmd.Parameters.AddWithValue("@ExamSerial", examserial);
                    cmd.Parameters.AddWithValue("@ExamScheduleSerial", examscheduleserial);


                    //Add return value parameter
                    SqlParameter returnParam = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParam.Direction = ParameterDirection.ReturnValue;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    result = (int)returnParam.Value;
                    con.Close();

                    message = result == -1 ? "Student already exists!" : "Saved successfully!";


                }
            }
            return Json(new { success = result != -1, message = message }, JsonRequestBehavior.AllowGet);
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



        //get exam by filtering selected term and subject both final & assignments
        public JsonResult GetExamsAllActive(int termserial)
        {
            List<SelectListItem> exams = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetExamsAllActive", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TermSerial", termserial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    exams.Add(new SelectListItem
                    {
                        Value = rdr["ExamSerial"].ToString(),
                        Text = rdr["ExamName"].ToString()
                    });
                }
            }

            return Json(exams, JsonRequestBehavior.AllowGet);
        }


        //get exam by filtering selected term , final
        public JsonResult GetExamsAllActiveHeldOnlyFinal(int termserial)
        {
            List<SelectListItem> exams = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetExamsAllActiveHeldOnlyFinal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TermSerial", termserial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    exams.Add(new SelectListItem
                    {
                        Value = rdr["ExamSerial"].ToString(),
                        Text = rdr["ExamName"].ToString()
                    });
                }
            }

            return Json(exams, JsonRequestBehavior.AllowGet);
        }

        //get exam by filtering selected term , final status = pending
        public JsonResult GetExamsAllActivePendingOnlyFinal(int termserial)
        {
            List<SelectListItem> exams = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetExamsAllActivePendingOnlyFinal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TermSerial", termserial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    exams.Add(new SelectListItem
                    {
                        Value = rdr["ExamSerial"].ToString(),
                        Text = rdr["ExamName"].ToString()
                    });
                }
            }

            return Json(exams, JsonRequestBehavior.AllowGet);
        }


        //
        // GET: /FinalExamScheduleApprovedbyPrincipal/

        public ActionResult FinalExamScheduleApprovedbyPrincipal()
        {
            var terms = GetAllTermsActive(); //for term dropdown

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


            // Pass terms
            ViewBag.Terms = terms;
            var model = new List<ExamScheduleModels>(); //  empty list on initial load
            return View(model);
        }

        //search student according to year, barch, class, term, exam
        public JsonResult SearchFinalExamScheduleApprovedbyPrincipal(int year, int batchserial, int termserial, int examserial)
        {

            List<ExamScheduleModels> students = new List<ExamScheduleModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchFinalExamScheduleApprovedbyPrincipal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new ExamScheduleModels
                    {
                        ExamScheduleSerial = Convert.ToInt32(reader["ExamScheduleSerial"].ToString()),
                        Year = Convert.ToInt32(reader["Year"].ToString()),
                        BatchName = reader["BatchName"].ToString(),
                        SubjectName = reader["SubjectName"].ToString(),
                        DateTime = Convert.ToDateTime(reader["DateTime"].ToString()),
                        ExamName = reader["ExamName"].ToString(),
                        TermName = reader["TermName"].ToString(),
                        Status = reader["Status"].ToString(),
                        ApprovedByPrincipal = reader["ApprovedByPrincipal"].ToString()

                    });
                }
            }
            if (students == null || !students.Any())
            {
                TempData["Message"] = "No Exams found.";
            }


            return Json(students, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult FinalExamScheduleApprovedbyPrincipal(List<string> ExamScheduleSerials)
        {
            string teacherserial = Session["EmpSerial"] != null ? Session["EmpSerial"].ToString() : "";
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;


            if (ExamScheduleSerials != null && ExamScheduleSerials.Count > 0 && !string.IsNullOrEmpty(teacherserial))
            {

                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    conn.Open();

                    foreach (var serial in ExamScheduleSerials)
                    {
                        using (SqlCommand cmd = new SqlCommand("FinalExamScheduleApprovedbyPrincipal", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ExamScheduleSerial", serial);
                            cmd.Parameters.AddWithValue("@PrincipalSerial", teacherserial);
                            cmd.Parameters.AddWithValue("@UserSerial", userserial);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                TempData["Message"] = "Exam approved successfully.";
            }
            else
            {
                TempData["Message"] = "No records to approve.";
            }

            return RedirectToAction("FinalExamScheduleApprovedbyPrincipal");
        }


        //get exam by filtering selected term , assignment and status = pending or held
        public JsonResult GetExamsAllAssignmentsPending(int termserial)
        {
            List<SelectListItem> exams = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetExamsAllAssignmentsPending", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TermSerial", termserial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    exams.Add(new SelectListItem
                    {
                        Value = rdr["ExamSerial"].ToString(),
                        Text = rdr["ExamName"].ToString()
                    });
                }
            }

            return Json(exams, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /ExamTimeTable/

        public ActionResult ExamTimeTable()
        {
            var terms = GetAllTermsActive(); //for term dropdown

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


            // Pass terms
            ViewBag.Terms = terms;
            var model = new List<ExamScheduleModels>(); //  empty list on initial load
            return View(model);

        }



        //search exams according to year, barch, class, term, exam
        public JsonResult SearchforExamTimeTable(int year, int batchserial, int classserial, int termserial, int examserial)
        {

            List<ExamScheduleModels> students = new List<ExamScheduleModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchforExamTimeTable", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                int i = 1;


                while (reader.Read())
                {
                    students.Add(new ExamScheduleModels
                    {
                        No = i++,
                        Year = Convert.ToInt32(reader["Year"].ToString()),
                        BatchName = reader["BatchName"].ToString(),
                        SubjectName = reader["SubjectName"].ToString(),
                        DateTime = Convert.ToDateTime(reader["DateTime"].ToString()),
                        ExamName = reader["ExamName"].ToString(),
                        TermName = reader["TermName"].ToString(),
                        ClassName = reader["ClassName"].ToString(),

                    });
                }
            }
            if (students == null || !students.Any())
            {
                TempData["Message"] = "No Exams found.";
            }


            return Json(students, JsonRequestBehavior.AllowGet);


        }


        [HttpPost]
        public ActionResult GenerateExamTimeTablePDF(int year, int batchserial, int classserial, int termserial, int examserial)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchforExamTimeTable", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();
                    row["S1"] = reader["Year"].ToString();
                    row["S2"] = reader["BatchName"].ToString();
                    row["S3"] = reader["ClassName"].ToString();
                    row["S4"] = reader["TermName"].ToString();
                    row["S5"] = reader["ExamName"].ToString();
                    row["S6"] = reader["SubjectName"].ToString();
                    row["S7"] = Convert.ToDateTime(reader["DateTime"]).ToString("yyyy-MM-dd hh:mm tt");
                    row["S8"] = reader["No"].ToString();
                    dataTable.AddDataTable1Row(row);
                }
                reader.Close();
            }

            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found.";
                return RedirectToAction("ExamTimeTable");
            }

            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/ExamTimeTable.rpt"));
            report.SetDataSource(dataset);

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "ExamTimeTable.pdf");
        }



        [HttpPost]
        public ActionResult UploadExamTimeTablePDF(int year, int batchserial, int classserial, int termserial, int examserial, HttpPostedFileBase uploadFile)
        {
            if (uploadFile == null || uploadFile.ContentLength == 0)
            {
                TempData["Message"] = "Please select a PDF file to upload.";
                return RedirectToAction("ExamTimeTable");
            }

            byte[] fileBytes = null;
            string fileType = uploadFile.ContentType;

            using (var binaryReader = new BinaryReader(uploadFile.InputStream))
            {
                fileBytes = binaryReader.ReadBytes(uploadFile.ContentLength);
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("InsertExamTimeTable", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                cmd.Parameters.Add("@FileUpload", SqlDbType.Image).Value = fileBytes;
                cmd.Parameters.AddWithValue("@Filetype", fileType ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

            }

            TempData["Message"] = "PDF uploaded successfully.";
            return RedirectToAction("ExamTimeTable");
        }

        [HttpPost]
        public ActionResult UploadExamTimeTableDirectly(int year, int batchserial, int classserial, int termserial, int examserial)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchforExamTimeTable", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();
                    row["S1"] = reader["Year"].ToString();
                    row["S2"] = reader["BatchName"].ToString();
                    row["S3"] = reader["ClassName"].ToString();
                    row["S4"] = reader["TermName"].ToString();
                    row["S5"] = reader["ExamName"].ToString();
                    row["S6"] = reader["SubjectName"].ToString();
                    row["S7"] = Convert.ToDateTime(reader["DateTime"]).ToString("yyyy-MM-dd hh:mm tt");
                    row["S8"] = reader["No"].ToString();
                    dataTable.AddDataTable1Row(row);
                }
                reader.Close();
            }

            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found to upload.";
                return RedirectToAction("ExamTimeTable");
            }

            byte[] fileBytes = null;
            using (ReportDocument report = new ReportDocument())
            {
                report.Load(Server.MapPath("~/Report/ExamTimeTable.rpt"));
                report.SetDataSource(dataset);

                using (Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                }
                report.Close();
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("InsertExamTimeTable", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                cmd.Parameters.Add("@FileUpload", SqlDbType.Image).Value = fileBytes;
                cmd.Parameters.AddWithValue("@Filetype", "application/pdf");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Message"] = "Timetable data uploaded successfully.";
            return RedirectToAction("ExamTimeTable");
        }

        //get exam by filtering selected term , final and status = pending or held
        public JsonResult GetExamsAllFinalPending(int termserial)
        {
            List<SelectListItem> exams = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetExamsAllFinalPending", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TermSerial", termserial);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    exams.Add(new SelectListItem
                    {
                        Value = rdr["ExamSerial"].ToString(),
                        Text = rdr["ExamName"].ToString()
                    });
                }
            }

            return Json(exams, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /FinalExamTimeTable/

        public ActionResult FinalExamTimeTable()
        {
            var terms = GetAllTermsActive(); //for term dropdown

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


            // Pass terms
            ViewBag.Terms = terms;
            var model = new List<ExamScheduleModels>(); //  empty list on initial load
            return View(model);

        }



        //search exams according to year, barch, term, exam
        public JsonResult SearchforFinalExamTimeTable(int year, int batchserial, int termserial, int examserial)
        {

            List<ExamScheduleModels> students = new List<ExamScheduleModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchforFinalExamTimeTable", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                int i = 1;


                while (reader.Read())
                {
                    students.Add(new ExamScheduleModels
                    {
                        No = i++,
                        Year = Convert.ToInt32(reader["Year"].ToString()),
                        BatchName = reader["BatchName"].ToString(),
                        SubjectName = reader["SubjectName"].ToString(),
                        DateTime = Convert.ToDateTime(reader["DateTime"].ToString()),
                        ExamName = reader["ExamName"].ToString(),
                        TermName = reader["TermName"].ToString(),

                    });
                }
            }
            if (students == null || !students.Any())
            {
                TempData["Message"] = "No Exams found.";
            }


            return Json(students, JsonRequestBehavior.AllowGet);


        }


        [HttpPost]
        public ActionResult GenerateFianlExamTimeTablePDF(int year, int batchserial, int termserial, int examserial)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchforFinalExamTimeTable", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();
                    row["S1"] = reader["Year"].ToString();
                    row["S2"] = reader["BatchName"].ToString();
                    row["S3"] = reader["TermName"].ToString();
                    row["S4"] = reader["ExamName"].ToString();
                    row["S5"] = reader["SubjectName"].ToString();
                    row["S6"] = Convert.ToDateTime(reader["DateTime"]).ToString("yyyy-MM-dd hh:mm tt");
                    row["S7"] = reader["No"].ToString();
                    dataTable.AddDataTable1Row(row);
                }
                reader.Close();
            }

            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found.";
                return RedirectToAction("FinalExamTimeTable");
            }

            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/FinalExamTimeTable.rpt"));
            report.SetDataSource(dataset);

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "FinalExamTimeTable.pdf");
        }



        [HttpPost]
        public ActionResult UploadFianlExamTimeTablePDF(int year, int batchserial, int termserial, int examserial, HttpPostedFileBase uploadFile)
        {
            if (uploadFile == null || uploadFile.ContentLength == 0)
            {
                TempData["Message"] = "Please select a PDF file to upload.";
                return RedirectToAction("ExamTimeTable");
            }

            byte[] fileBytes = null;
            string fileType = uploadFile.ContentType;

            using (var binaryReader = new BinaryReader(uploadFile.InputStream))
            {
                fileBytes = binaryReader.ReadBytes(uploadFile.ContentLength);
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("InsertFinalExamTimeTable", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                cmd.Parameters.Add("@FileUpload", SqlDbType.Image).Value = fileBytes;
                cmd.Parameters.AddWithValue("@Filetype", fileType ?? (object)DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

            }

            TempData["Message"] = "PDF uploaded successfully.";
            return RedirectToAction("FinalExamTimeTable");
        }

        [HttpPost]
        public ActionResult UploadFinalExamTimeTableDirectly(int year, int batchserial, int termserial, int examserial)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchforFinalExamTimeTable", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();
                    row["S1"] = reader["Year"].ToString();
                    row["S2"] = reader["BatchName"].ToString();
                    row["S3"] = reader["TermName"].ToString();
                    row["S4"] = reader["ExamName"].ToString();
                    row["S5"] = reader["SubjectName"].ToString();
                    row["S6"] = Convert.ToDateTime(reader["DateTime"]).ToString("yyyy-MM-dd hh:mm tt");
                    row["S7"] = reader["No"].ToString();
                    dataTable.AddDataTable1Row(row);
                }
                reader.Close();
            }

            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found to upload.";
                return RedirectToAction("FinalExamTimeTable");
            }

            byte[] fileBytes = null;
            using (ReportDocument report = new ReportDocument())
            {
                report.Load(Server.MapPath("~/Report/FinalExamTimeTable.rpt"));
                report.SetDataSource(dataset);

                using (Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                }
                report.Close();
            }

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("InsertFinalExamTimeTable", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                cmd.Parameters.Add("@FileUpload", SqlDbType.Image).Value = fileBytes;
                cmd.Parameters.AddWithValue("@Filetype", "application/pdf");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Message"] = "Final timetable data uploaded successfully.";
            return RedirectToAction("FinalExamTimeTable");
        }


        //
        // GET: /DownloadExamTimeTable/

        public ActionResult DownloadExamTimeTable()
        {
            List<ExamScheduleModels> list = new List<ExamScheduleModels>();

        using (SqlConnection con = new SqlConnection(conStr))
        using (SqlCommand cmd = new SqlCommand("GetExamTimeTables", con))
        {
        cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new ExamScheduleModels
                            {
                                ExamScheduleSerial = Convert.ToInt32(reader["ExamTimeTableSerial"]),
                                Year = Convert.ToInt32(reader["Year"]),
                                BatchSerial = Convert.ToInt32(reader["BatchSerial"]),
                                BatchName = reader["BatchName"].ToString(),
                                TermSerial = Convert.ToInt32(reader["TermSerial"]),
                                TermName = reader["TermName"].ToString(),
                                ExamSerial = Convert.ToInt32(reader["ExamSerial"]),
                                ExamName = reader["ExamName"].ToString(),
                                Filetype = reader["Filetype"].ToString()
                            };

                            if (reader["ClassSerial"] != DBNull.Value)
                            {
                                item.ClassSerial = Convert.ToInt32(reader["ClassSerial"]);
                                item.ClassName = reader["ClassName"].ToString();
                            }
                            else
                            {
                                item.ClassName = "All Classes (Final)";
                            }

                            list.Add(item);
                        }
                    
                }
            }

            return View(list);
        }

        public ActionResult ViewExamTimeTable(int year, int batchserial, int classserial, int termserial, int examserial)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetUploadedExamTimeTable", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    byte[] fileBytes = (byte[])reader["FileUpload"];
                    string fileType = reader["Filetype"].ToString();
                    return File(fileBytes, fileType, "ExamTimeTable.pdf");
                }
                else
                {
                    TempData["Message"] = "No exam timetable found.";
                    return RedirectToAction("DownloadExamTimeTable");
                }
            }
        }


        public ActionResult ViewFinalExamTimeTable(int year, int batchserial, int termserial, int examserial)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetUploadedFinalExamTimeTable", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    byte[] fileBytes = (byte[])reader["FileUpload"];
                    string fileType = reader["Filetype"].ToString();
                    return File(fileBytes, fileType, "FinalExamTimeTable.pdf");
                }
                else
                {
                    TempData["Message"] = "No exam timetable found.";
                    return RedirectToAction("DownloadExamTimeTable");
                }
            }
        }
    }
}
