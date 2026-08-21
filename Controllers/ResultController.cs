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
    public class ResultController : Controller
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

        //get exam by filtering selected term and subject both final & assignments, year, batch, class
        public JsonResult GetExamsAllActiveFilterTermSubject(int year, int batchserial, int classserial, int termserial, int subjectserial)
        {
            List<SelectListItem> exams = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetExamsAllActiveFilterTermSubject", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);

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
        // GET: /AddExamResult/

        public ActionResult AddExamResult()
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
            var model = new List<ResultModels>(); //  empty list on initial load
            return View(model);
        }


        //search student according to year, batch, class,term, subject, exam
        public ActionResult SearchStudentsforAddExamResult(int year, int batchserial, int classserial, int termserial, int subjectserial, int examserial)
        {

            List<ResultModels> students = new List<ResultModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforAddExamResult", con);
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
                    students.Add(new ResultModels
                    {
                        ExamResultSerial = reader.IsDBNull(reader.GetOrdinal("ExamResultSerial"))
                                            ? (int?)null
                                            : reader.GetInt32(reader.GetOrdinal("ExamResultSerial")),
                        StudentSerial = Convert.ToInt32(reader["StudentSerial"]),
                        FullName = reader["FullName"].ToString(),
                        RegNo = reader["RegNo"].ToString(),
                        Attedance = reader["Attedance"] == DBNull.Value ? null : reader["Attedance"].ToString(),
                        Result = reader.IsDBNull(reader.GetOrdinal("Result"))
                                            ? (decimal?)null
                                            : reader.GetDecimal(reader.GetOrdinal("Result")),

                    });
                }
            }
            if (students == null || !students.Any())
            {
                TempData["Message"] = "No students found.";
            }


            return PartialView("_StudentAddExamResultPartial", students);


        }

        [HttpPost]
        public ActionResult AddExamResult(int subjectserial, int examresultserial, decimal result, string approvedclassteacher)
        {
            string message = "";
            int result1 = 0;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                //update sp exam result
                if (examresultserial != 0)
                {
                    SqlCommand cmd = new SqlCommand("UpdateStudentAddExamResult", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExamResultSerial", examresultserial);
                    cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);
                    cmd.Parameters.AddWithValue("@Result", result);
                    cmd.Parameters.AddWithValue("@ApprovedClassTeacher", approvedclassteacher);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    message = "Result Updated successfully!";
                    result1 = 1;
                }

               
            }
            return Json(new { success = result1 != -1, message = message }, JsonRequestBehavior.AllowGet);
        }

        public class ExamBulkSaveModel
        {
            public string ExamResultSerial { get; set; }
            public string SubjectSerial { get; set; }
            public string Result { get; set; }
            public string ApprovedClassTeacher { get; set; }
        }

        [HttpPost]
        public ActionResult AddExamResultBulk(string resultsJson)
        {
            string message = "";
            int successCount = 0;

            if (!string.IsNullOrEmpty(resultsJson))
            {
                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExamBulkSaveModel>>(resultsJson);
                
                if (results != null && results.Count > 0)
                {
                    using (SqlConnection con = new SqlConnection(conStr))
                    {
                        con.Open();
                        foreach (var item in results)
                        {
                            int examResultSerial = 0;
                            int subjectSerial = 0;
                            decimal resultVal = 0;

                            if (int.TryParse(item.ExamResultSerial, out examResultSerial) && examResultSerial != 0 &&
                                int.TryParse(item.SubjectSerial, out subjectSerial) &&
                                decimal.TryParse(item.Result, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out resultVal))
                            {
                                using (SqlCommand cmd = new SqlCommand("UpdateStudentAddExamResult", con))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@ExamResultSerial", examResultSerial);
                                    cmd.Parameters.AddWithValue("@SubjectSerial", subjectSerial);
                                    cmd.Parameters.AddWithValue("@Result", resultVal);
                                    cmd.Parameters.AddWithValue("@ApprovedClassTeacher", item.ApprovedClassTeacher ?? "Pending");
                                    cmd.ExecuteNonQuery();
                                }
                                successCount++;
                            }
                        }
                        con.Close();
                    }
                    message = successCount + " Exam Results Updated successfully!";
                }
                else
                {
                    message = "No results to update.";
                }
            }
            else
            {
                message = "Invalid data received.";
            }

            return Json(new { success = successCount > 0, message = message }, JsonRequestBehavior.AllowGet);
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

        // GET: /AddAssignmentResult/
        public ActionResult AddAssignmentResult()
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

            ViewBag.Terms = GetAllTermsActive();

            var model = new List<AssignmentResultModels>(); // empty list on initial load
            return View(model);
        }

        // GET: SearchStudentsforAddAssignmentResult
        public ActionResult SearchStudentsforAddAssignmentResult(int year, int batchserial, int classserial, int termserial, int subjectserial, int examserial)
        {
            List<AssignmentResultModels> students = new List<AssignmentResultModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforAddAssignmentResult", con);
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
                    students.Add(new AssignmentResultModels
                    {
                        ExamAssignmentAttedanceSerial = reader.IsDBNull(reader.GetOrdinal("ExamAssignmentAttedanceSerial"))
                                            ? (int?)null
                                            : reader.GetInt32(reader.GetOrdinal("ExamAssignmentAttedanceSerial")),
                        StudentSerial = Convert.ToInt32(reader["StudentSerial"]),
                        FullName = reader["FullName"].ToString(),
                        RegNo = reader["RegNo"].ToString(),
                        Attedance = reader["Attedance"] == DBNull.Value ? null : reader["Attedance"].ToString(),
                        Result = reader.IsDBNull(reader.GetOrdinal("Result"))
                                            ? (decimal?)null
                                            : reader.GetDecimal(reader.GetOrdinal("Result")),
                    });
                }
            }

            if (students == null || !students.Any())
            {
                TempData["Message"] = "No students found.";
            }

            return PartialView("_StudentAddAssignmentResultPartial", students);
        }

        [HttpPost]
        public ActionResult AddAssignmentResult(int examassignmentattedanceserial, decimal result)
        {
            string message = "";
            int result1 = 0;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                if (examassignmentattedanceserial != 0)
                {
                    SqlCommand cmd = new SqlCommand("UpdateStudentAddAssignmentResult", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExamAssignmentAttedanceSerial", examassignmentattedanceserial);
                    cmd.Parameters.AddWithValue("@Result", result);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    message = "Assignment Result Updated successfully!";
                    result1 = 1;
                }
            }
            return Json(new { success = result1 != -1, message = message }, JsonRequestBehavior.AllowGet);
        }

        public class AssignmentBulkSaveModel
        {
            public string ExamAssignmentAttedanceSerial { get; set; }
            public string Result { get; set; }
        }

        [HttpPost]
        public ActionResult AddAssignmentResultBulk(string resultsJson)
        {
            string message = "";
            int successCount = 0;

            if (!string.IsNullOrEmpty(resultsJson))
            {
                var results = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AssignmentBulkSaveModel>>(resultsJson);
                
                if (results != null && results.Count > 0)
                {
                    using (SqlConnection con = new SqlConnection(conStr))
                    {
                        con.Open();
                        foreach (var item in results)
                        {
                            int serial = 0;
                            decimal resultVal = 0;

                            // Using InvariantCulture to safely parse the decimal dot
                            if (int.TryParse(item.ExamAssignmentAttedanceSerial, out serial) && serial != 0 && decimal.TryParse(item.Result, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out resultVal))
                            {
                                using (SqlCommand cmd = new SqlCommand("UpdateStudentAddAssignmentResult", con))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@ExamAssignmentAttedanceSerial", serial);
                                    cmd.Parameters.AddWithValue("@Result", resultVal);
                                    cmd.ExecuteNonQuery();
                                }
                                successCount++;
                            }
                        }
                        con.Close();
                    }
                    message = successCount + " Assignment Results Updated successfully!";
                }
                else
                {
                    message = "No results to update.";
                }
            }
            else
            {
                message = "Invalid data received.";
            }

            return Json(new { success = successCount > 0, message = message }, JsonRequestBehavior.AllowGet);
        }



        //
        // GET: /ApprovedResultClassTeacher/

        public ActionResult ApprovedResultClassTeacher()
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
            var model = new List<ResultModels>(); //  empty list on initial load
            return View(model);
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


        //search student according to year, barch, class, term, exam
        public JsonResult SearchStudentsforApproveExamResultByClassTeacher(int year, int batchserial, int classserial, int termserial, int examserial)
        {

            List<ResultModels> students = new List<ResultModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforApproveExamResultByClassTeacher", con);
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
                    var student = new ResultModels
                    {
                        FullName = reader["FullName"].ToString(),
                        RegNo = reader["RegNo"].ToString(),
                        SubjectName = reader["SubjectName"].ToString(),
                        Result = reader.IsDBNull(reader.GetOrdinal("Result"))
                                            ? (decimal?)null
                                            : reader.GetDecimal(reader.GetOrdinal("Result")),
                        ApprovedClassTeacher = reader["ApprovedClassTeacher"].ToString()
                    };

                    if (examserial == 3 || examserial == 6 || examserial == 9)
                    {
                        student.ExamResultSerial = reader.IsDBNull(reader.GetOrdinal("ExamResultSerial"))
                                            ? (int?)null
                                            : reader.GetInt32(reader.GetOrdinal("ExamResultSerial"));
                    }
                    else
                    {
                        student.ExamResultSerial = reader.IsDBNull(reader.GetOrdinal("ExamAssignmentAttedanceSerial"))
                                            ? (int?)null
                                            : reader.GetInt32(reader.GetOrdinal("ExamAssignmentAttedanceSerial"));
                    }

                    students.Add(student);
                }
            }
            if (students == null || !students.Any())
            {
                TempData["Message"] = "No students found.";
            }


            return Json(students, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult ApprovedResultClassTeacher(List<string> ExamResultSerials, int ExamSerial)
        {
             string teacherserial = Session["EmpSerial"] != null ? Session["EmpSerial"].ToString() : "";
             int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;


             if (ExamResultSerials != null && ExamResultSerials.Count > 0 && !string.IsNullOrEmpty(teacherserial))
    {

                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    conn.Open();

                    foreach (var serial in ExamResultSerials)
                    {
                        using (SqlCommand cmd = new SqlCommand("ApproveClassTeacherResult", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ExamResultSerial", serial);
                            cmd.Parameters.AddWithValue("@ClassTeacherSerial", teacherserial);
                            cmd.Parameters.AddWithValue("@UserSerial", userserial);
                            cmd.Parameters.AddWithValue("@ExamSerial", ExamSerial);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                TempData["Message"] = "Results approved successfully.";
            }
            else
            {
                TempData["Message"] = "No records to approve.";
            }

            return RedirectToAction("ApprovedResultClassTeacher");
        }

        //
        // GET: /ApprovedResultBatchHead/

        public ActionResult ApprovedResultBatchHead()
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
            var model = new List<ResultModels>(); //  empty list on initial load
            return View(model);
        }

        //search student according to year, barch, class, term, exam
        public JsonResult SearchStudentsforApproveExamResultByBatchTeacher(int year, int batchserial, int classserial, int termserial,  int examserial)
        {

            List<ResultModels> students = new List<ResultModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforApproveExamResultByBatchTeacher", con);
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
                    students.Add(new ResultModels
                    {
                        ExamResultSerial = reader.IsDBNull(reader.GetOrdinal("ExamResultSerial"))
                                            ? (int?)null
                                            : reader.GetInt32(reader.GetOrdinal("ExamResultSerial")),
                        FullName = reader["FullName"].ToString(),
                        RegNo = reader["RegNo"].ToString(),
                        SubjectName = reader["SubjectName"].ToString(),
                        Result = reader.IsDBNull(reader.GetOrdinal("Result"))
                                            ? (decimal?)null
                                            : reader.GetDecimal(reader.GetOrdinal("Result")),
                        ApprovedClassTeacher = reader["ApprovedClassTeacher"].ToString(),
                        ApprovedByBatchHead = reader["ApprovedByBatchHead"].ToString()

                    });
                }
            }
            if (students == null || !students.Any())
            {
                TempData["Message"] = "No students found.";
            }


            return Json(students, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult ApprovedResultBatchHead(List<string> ExamResultSerials)
        {
            string teacherserial = Session["EmpSerial"] != null ? Session["EmpSerial"].ToString() : "";
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;


            if (ExamResultSerials != null && ExamResultSerials.Count > 0 && !string.IsNullOrEmpty(teacherserial))
            {

                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    conn.Open();

                    foreach (var serial in ExamResultSerials)
                    {
                        using (SqlCommand cmd = new SqlCommand("ApprovedResultBatchHead", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ExamResultSerial", serial);
                            cmd.Parameters.AddWithValue("@BatchHeadSerial", teacherserial);
                            cmd.Parameters.AddWithValue("@UserSerial", userserial);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                TempData["Message"] = "Results approved successfully.";
            }
            else
            {
                TempData["Message"] = "No records to approve.";
            }

            return RedirectToAction("ApprovedResultBatchHead");
        }

        //
        // GET: /ApprovedResultSectionHead/

        public ActionResult ApprovedResultSectionHead()
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
            var model = new List<ResultModels>(); //  empty list on initial load
            return View(model);
        }


        //search student according to year, barch, class, term, exam
        public JsonResult SearchStudentsforApproveExamResultBySectionHead(int year, int batchserial, int classserial, int termserial, int examserial)
        {

            List<ResultModels> students = new List<ResultModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforApproveExamResultBySectionHead", con);
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
                    students.Add(new ResultModels
                    {
                        ExamResultSerial = reader.IsDBNull(reader.GetOrdinal("ExamResultSerial"))
                                            ? (int?)null
                                            : reader.GetInt32(reader.GetOrdinal("ExamResultSerial")),
                        FullName = reader["FullName"].ToString(),
                        RegNo = reader["RegNo"].ToString(),
                        SubjectName = reader["SubjectName"].ToString(),
                        Result = reader.IsDBNull(reader.GetOrdinal("Result"))
                                            ? (decimal?)null
                                            : reader.GetDecimal(reader.GetOrdinal("Result")),
                        ApprovedClassTeacher = reader["ApprovedClassTeacher"].ToString(),
                        ApprovedByBatchHead = reader["ApprovedByBatchHead"].ToString(),
                        ApprovedBySectionHead = reader["ApprovedBySectionHead"].ToString()

                    });
                }
            }
            if (students == null || !students.Any())
            {
                TempData["Message"] = "No students found.";
            }


            return Json(students, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult ApprovedResultSectionHead(List<string> ExamResultSerials)
        {
            string teacherserial = Session["EmpSerial"] != null ? Session["EmpSerial"].ToString() : "";
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;


            if (ExamResultSerials != null && ExamResultSerials.Count > 0 && !string.IsNullOrEmpty(teacherserial))
            {

                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    conn.Open();

                    foreach (var serial in ExamResultSerials)
                    {
                        using (SqlCommand cmd = new SqlCommand("ApprovedResultSectionHead", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ExamResultSerial", serial);
                            cmd.Parameters.AddWithValue("@SectionHeadSerial", teacherserial);
                            cmd.Parameters.AddWithValue("@UserSerial", userserial);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                TempData["Message"] = "Results approved successfully.";
            }
            else
            {
                TempData["Message"] = "No records to approve.";
            }

            return RedirectToAction("ApprovedResultSectionHead");
        }

        //
        // GET: /ApprovedResultPrincipal/

        public ActionResult ApprovedResultPrincipal()
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
            var model = new List<ResultModels>(); //  empty list on initial load
            return View(model);
        }

        //search student according to year, barch, class, term, exam
        public JsonResult SearchStudentsforApprovedResultPrincipal(int year, int batchserial, int classserial, int termserial, int examserial)
        {

            List<ResultModels> students = new List<ResultModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforApprovedResultPrincipal", con);
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
                    students.Add(new ResultModels
                    {
                        ExamResultSerial = reader.IsDBNull(reader.GetOrdinal("ExamResultSerial"))
                                            ? (int?)null
                                            : reader.GetInt32(reader.GetOrdinal("ExamResultSerial")),
                        FullName = reader["FullName"].ToString(),
                        RegNo = reader["RegNo"].ToString(),
                        SubjectName = reader["SubjectName"].ToString(),
                        Result = reader.IsDBNull(reader.GetOrdinal("Result"))
                                            ? (decimal?)null
                                            : reader.GetDecimal(reader.GetOrdinal("Result")),
                        ApprovedClassTeacher = reader["ApprovedClassTeacher"].ToString(),
                        ApprovedByBatchHead = reader["ApprovedByBatchHead"].ToString(),
                        ApprovedBySectionHead = reader["ApprovedBySectionHead"].ToString(),
                        ApprovedByPrincipal = reader["ApprovedByPrincipal"].ToString()

                    });
                }
            }
            if (students == null || !students.Any())
            {
                TempData["Message"] = "No students found.";
            }


            return Json(students, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult ApprovedResultPrincipal(List<string> ExamResultSerials)
        {
            string teacherserial = Session["EmpSerial"] != null ? Session["EmpSerial"].ToString() : "";
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;

            if (ExamResultSerials != null && ExamResultSerials.Count > 0 && !string.IsNullOrEmpty(teacherserial))
            {

                using (SqlConnection conn = new SqlConnection(conStr))
                {
                    conn.Open();

                    foreach (var serial in ExamResultSerials)
                    {
                        using (SqlCommand cmd = new SqlCommand("ApprovedResultPrincipal", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ExamResultSerial", serial);
                            cmd.Parameters.AddWithValue("@PrincipalSerial", teacherserial);
                            cmd.Parameters.AddWithValue("@UserSerial", userserial);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                TempData["Message"] = "Results approved successfully.";
            }
            else
            {
                TempData["Message"] = "No records to approve.";
            }

            return RedirectToAction("ApprovedResultPrincipal");
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

        //get classess by filtering selected batch, and logging teacher serial
        public JsonResult GetClassesByBatchBothStatusByTeacherSerial(int batchSerial)
        {
            int teacherserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;

            List<SelectListItem> classes = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetClassesByBatchBothStatusByTeacherSerial", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BatchSerial", batchSerial);
                cmd.Parameters.AddWithValue("@TeacherSerial", teacherserial);

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

        //Get batches according to year and llogging section head teacher
        public JsonResult GetBatchesByYearBothStatusBySectionHeadTeacher(int year)
        {
            int teacherserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int userserial = Session["UserSerial"] != null ? Convert.ToInt32(Session["UserSerial"].ToString()) : 0;


            List<SelectListItem> batch = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetBatchesByYearBothStatusBySectionHeadTeacher", con);
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

    }
}
