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
    public class ResultReportController : Controller
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
        // GET: /BatchClassTotalReport/

        public ActionResult BatchClassTotalReport()
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
            var model = new List<ResultReportModels>(); //  empty list on initial load
            return View(model);
        }


        //search student according to year, batch, class,term, subject, exam
        public JsonResult SearchStudentsforBatchClassTotalReport(int year, int batchserial, int classserial, int termserial, int examserial)
        {

            List<ResultReportModels> students = new List<ResultReportModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforBatchClassTotalReport", con);
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
                    students.Add(new ResultReportModels
                    {
                        No = i++,
                        ClassName = reader["ClassName"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        RegNo = reader["RegNo"].ToString(),
                        Sinhala = reader.IsDBNull(reader.GetOrdinal("Sinhala"))
                                            ? (decimal?)null
                                            : reader.GetDecimal(reader.GetOrdinal("Sinhala")),
                        Mathematics = reader.IsDBNull(reader.GetOrdinal("Mathematics"))
                        ? (decimal?)null
                        : reader.GetDecimal(reader.GetOrdinal("Mathematics")),
                        Total = reader.IsDBNull(reader.GetOrdinal("Total"))
                                            ? (decimal?)null
                                            : reader.GetDecimal(reader.GetOrdinal("Total")),
                        Rank = Convert.ToInt32(reader["Rank"].ToString()),

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
        public ActionResult BatchClassTotalReport(ResultReportModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            int i = 1;
            int no;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforBatchClassTotalReport", con);
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
                    no = i++;
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["Year"].ToString();
                    row["S2"] = reader["BatchName"].ToString();
                    row["S3"] = reader["TermName"].ToString();
                    row["S4"] = reader["ExamName"].ToString();
                    row["S5"] = no;
                    row["S6"] = reader["ClassName"].ToString();
                    row["S7"] = reader["RegNo"].ToString();
                    row["S8"] = reader["FullName"].ToString();
                    row["S9"] = reader["Sinhala"].ToString();
                    row["S10"] = reader["Mathematics"].ToString();
                    row["S15"] = reader["Total"].ToString();
                    row["S16"] = reader["Rank"].ToString();

                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("BatchClassTotalReport"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/BatchClassTotalReport.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "BatchClassTotalReport.pdf");

            //Export to Excel
        //return File(stream, "application/vnd.ms-excel", "SectionDetailList.xls");


        }

        //
        // GET: /BatchClassAttendanceReport/

        public ActionResult BatchClassAttendanceReport()
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
            var model = new List<ResultReportModels>(); //  empty list on initial load
            return View(model);
        }


        //search student according to year, batch, class,term, subject, exam
        public JsonResult SearchStudentsforBatchClassAttendanceReport(int year, int batchserial, int classserial, int termserial, int examserial)
        {

            List<ResultReportModels> students = new List<ResultReportModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforBatchClassAttendanceReport", con);
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
                    students.Add(new ResultReportModels
                    {
                        No = i++,
                        ClassName = reader["ClassName"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        RegNo = reader["RegNo"].ToString(),
                        AttedanceSinhala = reader["Sinhala"].ToString(),
                        AttedanceMathematics = reader["Mathematics"].ToString(),

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
        public ActionResult BatchClassAttendanceReport(ResultReportModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            int i = 1;
            int no;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforBatchClassAttendanceReport", con);
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
                    no = i++;
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["Year"].ToString();
                    row["S2"] = reader["BatchName"].ToString();
                    row["S3"] = reader["TermName"].ToString();
                    row["S4"] = reader["ExamName"].ToString();
                    row["S5"] = no;
                    row["S6"] = reader["ClassName"].ToString();
                    row["S7"] = reader["RegNo"].ToString();
                    row["S8"] = reader["FullName"].ToString();
                    row["S9"] = reader["Sinhala"].ToString();
                    row["S10"] = reader["Mathematics"].ToString();

                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("BatchClassAttendanceReport"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/BatchClassAttendanceReport.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "BatchClassAttendanceReport.pdf");


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


        //
        // GET: /ClassSubjectReport/

        public ActionResult ClassSubjectReport()
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


            var model = new List<ResultReportModels>(); //  empty list on initial load
            return View(model);
        }


        //search student according to year, batch, class, subject
        public JsonResult SearchStudentsforClassSubjectReport(int year, int batchserial, int classserial, int subjectserial)
        {

            List<ResultReportModels> students = new List<ResultReportModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforClassSubjectReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                int i = 1;

                while (reader.Read())
                {
                    students.Add(new ResultReportModels
                    {
                        No = i++,
                        FullName = reader["StudentName"].ToString(),
                        RegNo = reader["RegNo"].ToString(),
                        T1Assignment1 = reader.IsDBNull(reader.GetOrdinal("T1Assignment1")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("T1Assignment1")),
                        T1Assignment2 = reader.IsDBNull(reader.GetOrdinal("T1Assignment2")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("T1Assignment2")),
                        T1Final = reader.IsDBNull(reader.GetOrdinal("T1Final")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("T1Final")),
                        T2Assignment1 = reader.IsDBNull(reader.GetOrdinal("T2Assignment1")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("T2Assignment1")),
                        T2Assignment2 = reader.IsDBNull(reader.GetOrdinal("T2Assignment2")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("T2Assignment2")),
                        T2Final = reader.IsDBNull(reader.GetOrdinal("T2Final")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("T2Final")),
                        T3Assignment1 = reader.IsDBNull(reader.GetOrdinal("T3Assignment1")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("T3Assignment1")),
                        T3Assignment2 = reader.IsDBNull(reader.GetOrdinal("T3Assignment2")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("T3Assignment2")),
                        T3Final = reader.IsDBNull(reader.GetOrdinal("T3Final")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("T3Final")),

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
        public ActionResult ClassSubjectReport(ResultReportModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            int i = 1;
            int no;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforClassSubjectReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                cmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    no = i++;
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["Year"].ToString();
                    row["S2"] = reader["BatchName"].ToString();
                    row["S3"] = reader["ClassName"].ToString();
                    row["S4"] = reader["SubjectName"].ToString();
                    row["S5"] = reader["TeacherName"].ToString();
                    row["S6"] = no;
                    row["S7"] = reader["RegNo"].ToString();
                    row["S8"] = reader["StudentName"].ToString();
                    row["S9"] = reader["T1Assignment1"].ToString();
                    row["S10"] = reader["T1Assignment2"].ToString();
                    row["S11"] = reader["T1Final"].ToString();
                    row["S12"] = reader["T2Assignment1"].ToString();
                    row["S13"] = reader["T2Assignment2"].ToString();
                    row["S14"] = reader["T2Final"].ToString();
                    row["S15"] = reader["T3Assignment1"].ToString();
                    row["S16"] = reader["T3Assignment2"].ToString();
                    row["S17"] = reader["T3Final"].ToString();

                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("ClassSubjectReport"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/ClassSubjectReport.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "ClassSubjectReport.pdf");


        }


        //
        // GET: /ClassSubjectHighestMarks/

        public ActionResult ClassSubjectHighestMarks()
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
            var model = new List<ResultReportModels>(); //  empty list on initial load
            return View(model);
        }



        [HttpPost]
        public ActionResult ClassSubjectHighestMarks(ResultReportModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            int i = 1;
            int no;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforClassResultHighestMarksReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
                cmd.Parameters.AddWithValue("@ExamSerial", model.ExamSerial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        no = i++;
                        row = dataTable.NewDataTable1Row();

                        // fields what used in report.rpt
                        row["S1"] = reader["Year"].ToString();
                        row["S2"] = reader["BatchName"].ToString();
                        row["S3"] = reader["ClassName"].ToString();
                        row["S4"] = reader["TeacherName"].ToString();
                        row["S5"] = reader["TermName"].ToString();
                        row["S6"] = reader["ExamName"].ToString();
                        row["S7"] = reader["SubjectName"].ToString();
                        row["S8"] = reader["HighestMark"].ToString();
                        row["S9"] = reader["HighestMarkStudent"].ToString();
                        row["S10"] = reader["LowestMark"].ToString();
                        row["S11"] = reader["LowestMarkStudent"].ToString();
                        
                        dataTable.AddDataTable1Row(row);
                    }
                }


            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("ClassSubjectHighestMarks"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/ClassSubjectHighestMarks.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "ClassSubjectHighestMarks.pdf");


        }


        //
        // GET: /ClassSubjectExamReport/

        public ActionResult ClassSubjectExamReport()
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
            var model = new List<ResultReportModels>(); //  empty list on initial load
            return View(model);
        }

        //search student according to year, batch, class, subject, term, exam
        public JsonResult SearchStudentsforClassSubjectExamReport(int year, int batchserial, int classserial, int subjectserial, int termserial, int examserial)
        {

            List<ResultReportModels> students = new List<ResultReportModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforClassResultReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@SubjectSerial", subjectserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                int i = 1;

                while (reader.Read())
                {
                    students.Add(new ResultReportModels
                    {
                        No = i++,
                        FullName = reader["FullName"].ToString(),
                        RegNo = reader["RegNo"].ToString(),
                        Attedance = reader["Attedance"].ToString(),
                        Result = reader.IsDBNull(reader.GetOrdinal("Result")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Result")),
                       
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
        public ActionResult ClassSubjectExamReport(ResultReportModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            int i = 1;
            int no;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforClassResultReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                cmd.Parameters.AddWithValue("@SubjectSerial", model.SubjectSerial);
                cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
                cmd.Parameters.AddWithValue("@ExamSerial", model.ExamSerial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    no = i++;
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["Year"].ToString();
                    row["S2"] = reader["BatchName"].ToString();
                    row["S3"] = reader["ClassName"].ToString();
                    row["S4"] = reader["SubjectName"].ToString();
                    row["S5"] = reader["TermName"].ToString();
                    row["S6"] = reader["ExamName"].ToString();
                    row["S7"] = reader["TeacherName"].ToString();
                    row["S8"] = no;
                    row["S9"] = reader["RegNo"].ToString();
                    row["S10"] = reader["FullName"].ToString();
                    row["S11"] = reader["Result"].ToString();
                    row["S12"] = reader["Attedance"].ToString();


                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
                con.Close();

            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("ClassSubjectExamReport"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/ClassSubjectExamReport.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "ClassSubjectExamReport.pdf");


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
        // GET: /StudentExamResultReport/

        public ActionResult StudentExamResultReport()
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
            var model = new List<ResultReportModels>(); //  empty list on initial load
            return View(model);
        }

        


        [HttpPost]
        public ActionResult StudentExamResultReport(ResultReportModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            int i = 1;
            int no;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforStudentExamResultReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                cmd.Parameters.AddWithValue("@StudentSerial", model.StudentSerial);
                cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);
                cmd.Parameters.AddWithValue("@ExamSerial", model.ExamSerial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    no = i++;
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["RegNo"].ToString();
                    row["S2"] = reader["FullName"].ToString();
                    row["S3"] = reader["Year"].ToString();
                    row["S4"] = reader["BatchName"].ToString();
                    row["S5"] = reader["ClassName"].ToString();
                    row["S6"] = reader["TermName"].ToString();
                    row["S7"] = reader["ExamName"].ToString();
                    row["S8"] = reader["ClassRank"].ToString();
                    row["S9"] = reader["StudentTotalMarks"].ToString();
                    row["S10"] = reader["SubjectName"].ToString();
                    row["S11"] = reader["StudentMark"].ToString();
                    row["S12"] = reader["HighestMark"].ToString();
                    row["S13"] = reader["AverageMark"].ToString();


                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
                con.Close();

            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("StudentExamResultReport"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/StudentExamResultReport.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "StudentExamResultReport.pdf");


        }


        //
        // GET: /StudentTermReport/

        public ActionResult StudentTermReport()
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
            var model = new List<ResultReportModels>(); //  empty list on initial load
            return View(model);
        }




        [HttpPost]
        public ActionResult StudentTermReport(ResultReportModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            int i = 1;
            int no;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforStudentTermReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                cmd.Parameters.AddWithValue("@StudentSerial", model.StudentSerial);
                cmd.Parameters.AddWithValue("@TermSerial", model.TermSerial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    no = i++;
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["RegNo"].ToString();
                    row["S2"] = reader["FullName"].ToString();
                    row["S3"] = reader["Year"].ToString();
                    row["S4"] = reader["BatchName"].ToString();
                    row["S5"] = reader["ClassName"].ToString();
                    row["S6"] = reader["TermName"].ToString();
                    row["S7"] = reader["SubjectName"].ToString();
                    row["S8"] = reader["Assignment1"].ToString();
                    row["S9"] = reader["Assignment2"].ToString();
                    row["S10"] = reader["FinalExam"].ToString();
                    


                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
                con.Close();

            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("StudentTermReport"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/StudentTermReport.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "StudentTermReport.pdf");


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
        
        
        public JsonResult SearchClassSubjectHighestMarksForGrid(int year, int batchserial, int classserial, int termserial, int examserial)
        {
            var results = new List<object>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforClassResultHighestMarksReport ", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        results.Add(new
                        {
                            Year = reader["Year"].ToString(),
                            BatchName = reader["BatchName"].ToString(),
                            ClassName = reader["ClassName"].ToString(),
                            TermName = reader["TermName"].ToString(),
                            ExamName = reader["ExamName"].ToString(),
                            TeacherName = reader["TeacherName"].ToString(),
                            SubjectSerial = reader["SubjectSerial"].ToString(),
                            SubjectName = reader["SubjectName"].ToString(),
                            HighestMark = reader["HighestMark"].ToString(),
                            HighestMarkStudent = reader["HighestMarkStudent"].ToString(),
                            LowestMark = reader["LowestMark"].ToString(),
                            LowestMarkStudent = reader["LowestMarkStudent"].ToString(),
                           
                        });
                    }
                }
                reader.Close();
            }
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchStudentExamResultReportForGrid(int year, int batchserial, int classserial, int studentserial, int termserial, int examserial)
        {
            var results = new List<object>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforStudentExamResultReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@StudentSerial", studentserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);
                cmd.Parameters.AddWithValue("@ExamSerial", examserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new
                    {
                        RegNo = reader["RegNo"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        Year = reader["Year"].ToString(),
                        BatchName = reader["BatchName"].ToString(),
                        ClassName = reader["ClassName"].ToString(),
                        TermName = reader["TermName"].ToString(),
                        ExamName = reader["ExamName"].ToString(),
                        Total = reader["StudentTotalMarks"].ToString(),
                        Rank = reader["ClassRank"].ToString(),
                        SubjectSerial = reader["SubjectSerial"].ToString(),
                        SubjectName = reader["SubjectName"].ToString(),
                        StudentMark = reader["StudentMark"].ToString(),
                        HighestMark = reader["HighestMark"].ToString(),
                        AverageMark = reader["AverageMark"].ToString(),
                    });
                }
                reader.Close();
            }
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchStudentTermReportForGrid(int year, int batchserial, int classserial, int studentserial, int termserial)
        {
            var results = new List<object>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentsforStudentTermReport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@StudentSerial", studentserial);
                cmd.Parameters.AddWithValue("@TermSerial", termserial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new
                    {
                        RegNo = reader["RegNo"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        Year = reader["Year"].ToString(),
                        BatchName = reader["BatchName"].ToString(),
                        ClassName = reader["ClassName"].ToString(),
                        TermName = reader["TermName"].ToString(),
                        SubjectSerial = reader["SubjectSerial"].ToString(),
                        SubjectName = reader["SubjectName"].ToString(),
                        Assignment1 = reader["Assignment1"].ToString(),
                        Assignment2 = reader["Assignment2"].ToString(),
                        FinalExam = reader["FinalExam"].ToString()
                    });
                }
                reader.Close();
            }
            return Json(results, JsonRequestBehavior.AllowGet);
        }

    }
}
