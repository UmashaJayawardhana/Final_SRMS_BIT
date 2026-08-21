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
using CrystalDecisions.CrystalReports.Engine;
using MvcApplication1.Report;
using System.IO;

namespace MvcApplication1.Controllers
{
    public class AnalyzeController : Controller
    {
        private readonly string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


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


        //
        // GET: /AttendanceReport/

        public ActionResult AttendanceReport()
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

            return View();
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

        [HttpPost]
        public ActionResult AttendanceReport(ResultModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchAttendanceReportPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
                cmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
                cmd.Parameters.AddWithValue("@ExamSerial", model.ExamSerial);


                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["AttendanceType"].ToString();
                    row["INT1"] = reader["AttendanceCount"].ToString();
                    row["S3"] = reader["Year"].ToString();
                    row["S4"] = reader["BatchName"].ToString();
                    row["S5"] = reader["ClassName"].ToString();
                    row["S6"] = reader["TermName"].ToString();
                    row["S7"] = reader["SubjectName"].ToString();
                    row["S8"] = reader["ExamName"].ToString();


                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("AttendanceReport"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/ExamAttendanceReport.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "ExamAttendanceReport.pdf");


        }

        //
        // GET: /SubjectRangeReport/

        public ActionResult SubjectRangeReport()
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

            return View();
        }

        [HttpPost]
        public ActionResult SubjectRangeReport(ResultReportModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchforSubjectRangeReportPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
                cmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
                cmd.Parameters.AddWithValue("@ExamSerial", model.ExamSerial);


                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["Year"].ToString();
                    row["S2"] = reader["ClassName"].ToString();
                    row["S3"] = reader["BatchName"].ToString();
                    row["S4"] = reader["TermName"].ToString();
                    row["S5"] = reader["SubjectName"].ToString();
                    row["S6"] = reader["ExamName"].ToString();
                    row["S7"] = reader["MarkRange"].ToString();
                    row["INT1"] = reader["StudentCount"].ToString();


                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("SubjectRangeReport"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/SubjectRangeReport.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "SubjectRangeReport.pdf");


        }

        //Get students to dropdown according to year, batch, class
        public JsonResult GetStudentsByYearBatchClass(int year, int batchserial, int classserial)
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


        //
        // GET: /StudentPerformanceReport/

        public ActionResult StudentPerformanceReport()
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

            return View();
        }

        [HttpPost]
        public ActionResult StudentPerformanceReport(ResultReportModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchforStudentPerformanceReportPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
                cmd.Parameters.AddWithValue("@StudentSerial", model.StudentSerial);
                cmd.Parameters.AddWithValue("@ExamSerial", model.ExamSerial);


                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["RegNo"].ToString();
                    row["S2"] = reader["FullName"].ToString();
                    row["S3"] = reader["Year"].ToString();
                    row["S4"] = reader["BatchName"].ToString();
                    row["S5"] = reader["ClassName"].ToString();
                    row["S6"] = reader["TermName"].ToString();
                    row["S7"] = reader["ExamName"].ToString();
                    row["S8"] = reader["Subject"].ToString();
                    row["S9"] = reader["PerformanceLevel"].ToString();
                    row["INT1"] = Convert.ToInt32(reader["PerformanceScore"].ToString());


                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("StudentPerformanceReport"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/StudentPerformanceReport.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "StudentPerformanceReport.pdf");


        }


        //
        // GET: /MarksYearlyAnalyze/

        public ActionResult MarksYearlyAnalyze()
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

            return View();
        }


        [HttpPost]
        public ActionResult MarksYearlyAnalyze(ResultReportModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchforMarksYearlyAnalyzePDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
                cmd.Parameters.AddWithValue("@ExamSerial", model.ExamSerial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["Year"].ToString();
                    row["S2"] = reader["BatchName"].ToString();
                    row["S3"] = reader["ClassName"].ToString();
                    row["S4"] = reader["TermName"].ToString();
                    row["S5"] = reader["ExamName"].ToString();
                    row["S6"] = reader["Subject"].ToString();
                    row["INT1"] = Convert.ToDecimal(reader["AverageMarks"].ToString());


                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("MarksYearlyAnalyze"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/MarksYearlyAnalyze.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "MarksYearlyAnalyze.pdf");


        }

        //Get batches according to year fitering with user group
        public JsonResult GetBatchesByYearBothStatusFilterUserGroup(int year)
        {
            int empserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int usergroupserial = Session["UserGroupSerial"] != null ?  Convert.ToInt32(Session["UserGroupSerial"].ToString()) : 0;

            List<SelectListItem> batch = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetBatchesByYearBothStatusFilterUserGroup", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@EmpSerial", empserial);
                cmd.Parameters.AddWithValue("@UserGroupSerial", usergroupserial);

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


        //get classess by filtering selected batch fitering with user group
        public JsonResult GetClassesByBatchBothStatusFilterUserGroup(int batchSerial)
        {

            int empserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int usergroupserial = Session["UserGroupSerial"] != null ? Convert.ToInt32(Session["UserGroupSerial"].ToString()) : 0;


            List<SelectListItem> classes = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetClassesByBatchBothStatusFilterUserGroup", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BatchSerial", batchSerial);
                cmd.Parameters.AddWithValue("@EmpSerial", empserial);
                cmd.Parameters.AddWithValue("@UserGroupSerial", usergroupserial);

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

        //Get students to dropdown according to year, batch, class
        public JsonResult GetStudentsByYearBatchClassFilterUserGroup(int year, int batchserial, int classserial)
        {
            int empserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int usergroupserial = Session["UserGroupSerial"] != null ? Convert.ToInt32(Session["UserGroupSerial"].ToString()) : 0;

            List<SelectListItem> students = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetStudentsByYearBatchClassFilterUserGroup", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@EmpSerial", empserial);
                cmd.Parameters.AddWithValue("@UserGroupSerial", usergroupserial);

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

        // --- Grid Search Endpoints --- //

        public JsonResult SearchAttendanceReportForGrid(int year, int batchserial, int classserial, int termserial, int subjectserial, int examserial)
        {
            List<object> results = new List<object>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchAttendanceReportPDF", con);
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
                    results.Add(new
                    {
                        AttendanceType = reader["AttendanceType"].ToString(),
                        Count = reader["AttendanceCount"].ToString()
                    });
                }
            }

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchSubjectRangeReportForGrid(int year, int batchserial, int classserial, int termserial, int subjectserial, int examserial)
        {
            List<object> results = new List<object>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchforSubjectRangeReportPDF", con);
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
                    results.Add(new
                    {
                        MarkRange = reader["MarkRange"].ToString(),
                        StudentCount = reader["StudentCount"].ToString()
                    });
                }
            }

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchStudentPerformanceReportForGrid(int year, int batchserial, int classserial, int termserial, int studentserial, int examserial)
        {
            List<object> results = new List<object>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchforStudentPerformanceReportPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@StudentSerial", studentserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new
                    {
                        Subject = reader["Subject"].ToString(),
                        PerformanceLevel = reader["PerformanceLevel"].ToString(),
                        PerformanceScore = reader["PerformanceScore"].ToString()
                    });
                }
            }

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchMarksYearlyAnalyzeForGrid(int year, int batchserial, int classserial, int termserial, int examserial)
        {
            List<object> results = new List<object>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchforMarksYearlyAnalyzePDF", con);
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
                    results.Add(new
                    {
                        Subject = reader["Subject"].ToString(),
                        AverageMarks = reader["AverageMarks"].ToString()
                    });
                }
            }

            return Json(results, JsonRequestBehavior.AllowGet);
        }

    }
}
