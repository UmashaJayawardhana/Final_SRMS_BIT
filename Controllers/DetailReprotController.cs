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
    public class DetailReprotController : Controller
    {

        private readonly string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //
        // GET: /BatchDetailList/

        public ActionResult BatchDetailList()
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

            return View();
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

        //search student according to year, batch, status
        public ActionResult SearchBatchDetailsForPDF(int year, int batchserial, string status)
        {

            List<BatchModels> batches = new List<BatchModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchBatchDetailsForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                int i = 1;
                while (reader.Read())
                {
                    batches.Add(new BatchModels
                    {
                        No = i++,
                        Year = Convert.ToInt32(reader["Year"].ToString()),
                        BatchName = reader["BatchName"].ToString(),
                        GradeName = reader["GradeName"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        Status = reader["Status"].ToString()

                    });
                }
            }
            if (batches == null || !batches.Any())
            {
                TempData["Message"] = "No batches found.";
            }

            return Json(batches, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult BatchDetailList(BatchModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

              using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchBatchDetailsForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                cmd.Parameters.AddWithValue("@Status", model.Status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
             while (reader.Read())
                    {
                        row = dataTable.NewDataTable1Row();

                        // fields what used in report.rpt
                       
                        row["S1"] = reader["FullName"].ToString();
                        row["S2"] = reader["Status"].ToString();
                        row["S3"] = reader["GradeName"].ToString();
                        row["S4"] = reader["BatchName"].ToString();
                        row["S5"] = reader["Year"].ToString();
                        row["S6"] = reader["No"].ToString();

                        dataTable.AddDataTable1Row(row);
                    }

                    reader.Close();
                }

              // Validate if no data returned
              if (dataTable.Rows.Count == 0)
              {
                  TempData["Message"] = "No data found for the selected criteria.";
                  return RedirectToAction("BatchDetailList"); // redirect to view
              }

                // Load the Crystal Report
                ReportDocument report = new ReportDocument();
                report.Load(Server.MapPath("~/Report/BatchDetailList.rpt")); //  .rpt path

                // Set data source
                report.SetDataSource(dataset);

                // Export to PDF
                Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "BatchDetailList.pdf");
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


        //
        // GET: /ClassDetailList/

        public ActionResult ClassDetailList()
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

            return View();
        }


        //search student according to year, batch, status
        public ActionResult SearchClassDetailForPDF(int year, int batchserial, int classserial, string status)
        {

            List<ClassModels> classes = new List<ClassModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchClassDetailForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                int i = 1;

                while (reader.Read())
                {
                    classes.Add(new ClassModels
                    {
                        No = i++,
                        Year = Convert.ToInt32(reader["Year"].ToString()),
                        BatchName = reader["BatchName"].ToString(),
                        GradeName = reader["GradeName"].ToString(),
                        ClassName = reader["ClassName"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        Status = reader["Status"].ToString()

                    });
                }
            }
            if (classes == null || !classes.Any())
            {
                TempData["Message"] = "No classes found.";
            }

            return Json(classes, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult ClassDetailList(ClassModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchClassDetailForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                cmd.Parameters.AddWithValue("@Status", model.Status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt

                    row["S1"] = reader["Year"].ToString();
                    row["S2"] = reader["BatchName"].ToString();
                    row["S3"] = reader["GradeName"].ToString();
                    row["S4"] = reader["ClassName"].ToString();
                    row["S5"] = reader["FullName"].ToString();
                    row["S6"] = reader["Status"].ToString();
                    row["S7"] = reader["No"].ToString();

                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("ClassDetailList"); // redirect to view
            }


            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/ClassDetailList.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);

            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "ClassDetailList.pdf");
        }

        //Get section to dropdown
        public JsonResult GetSectionsByYear(int year)
        {
            List<SelectListItem> section = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetSectionsByYear", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    section.Add(new SelectListItem
                    {
                        Value = rdr["SectionSerial"].ToString(),
                        Text = rdr["SectionName"].ToString()
                    });
                }
            }

            return Json(section, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /SectionDetailList/

        public ActionResult SectionDetailList()
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

            return View();
        }


        //search sections according to year, section, status
        public ActionResult SearchSectionDetailForPDF(int year, int sectionerial,  string status)
        {

            List<SectionModels> sections = new List<SectionModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchSectionDetailForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@SectionSerial", sectionerial);
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                int i = 1;

                while (reader.Read())
                {
                    sections.Add(new SectionModels
                    {
                        No = i++,
                        Year = Convert.ToInt32(reader["Year"].ToString()),
                        SectionName = reader["SectionName"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        Status = reader["Status"].ToString()

                    });
                }
            }
            if (sections == null || !sections.Any())
            {
                TempData["Message"] = "No sections found.";
            }

            return Json(sections, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult SectionDetailList(SectionModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchSectionDetailForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@SectionSerial", model.SectionSerial);
                cmd.Parameters.AddWithValue("@Status", model.Status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["Year"].ToString();
                    row["S2"] = reader["SectionName"].ToString();
                    row["S3"] = reader["FullName"].ToString();
                    row["S4"] = reader["Status"].ToString();
                    row["S5"] = reader["No"].ToString();

                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("SectionDetailList"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/SectionDetailList.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);

            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "SectionDetailList.pdf");
        }

        
        //
        // GET: /SubjectDetailList/

        public ActionResult SubjectDetailList()
        {
            var grades = GetAllGradesActive();

            // Pass both grades to the view
            ViewBag.Grades = grades;
            return View();
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

        //search subject according to grade, medium,  status
        public ActionResult SearchSubjectDetailForPDF(int gradeserial, string medium, string status)
        {

            List<SubjectModels> subjects = new List<SubjectModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchSubjectDetailForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@GradeSerial", gradeserial);
                cmd.Parameters.AddWithValue("@Medium", medium);
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                int i = 1;

                while (reader.Read())
                {
                    subjects.Add(new SubjectModels
                    {
                        No = i++,
                        SubjectName = reader["SubjectName"].ToString(),
                        Medium = reader["Medium"].ToString(),
                        Status = reader["Status"].ToString()

                    });
                }
            }
            if (subjects == null || !subjects.Any())
            {
                TempData["Message"] = "No subjects found.";
            }

            return Json(subjects, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult SubjectDetailList(SubjectModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchSubjectDetailForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@GradeSerial", model.GradeSerial);
                cmd.Parameters.AddWithValue("@Medium", model.Medium);
                cmd.Parameters.AddWithValue("@Status", model.Status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                int i = 1;
                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["GradeName"].ToString();
                    row["S2"] = reader["SubjectName"].ToString();
                    row["S3"] = reader["Medium"].ToString();
                    row["S4"] = reader["Status"].ToString();
                    row["S5"] = i++;

                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("SubjectDetailList"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/SubjectDetailList.rpt")); //  .rpt path

            //  Set data source
            report.SetDataSource(dataset);

            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "SubjectDetailList.pdf");
        }





        //
        // GET: /StudentDetailsList/

        public ActionResult StudentDetailsList()
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

            return View();
        }


        //search students according to year, batch, class, status
        public ActionResult SearchStudentDetailsListForPDF(int year, int batchserial, int classserial, string status)
        {

            List<StudentModels> students = new List<StudentModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentDetailsListForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchserial);
                cmd.Parameters.AddWithValue("@ClassSerial", classserial);
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                int i = 1;

                while (reader.Read())
                {
                    students.Add(new StudentModels
                    {
                        No = i++,
                        Year = Convert.ToInt32(reader["Year"].ToString()),
                        RegNo = reader["RegNo"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        Status = reader["Status"].ToString(),
                        BatchName = reader["BatchName"].ToString(),
                        ClassName = reader["ClassName"].ToString()

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
        public ActionResult StudentDetailsList(StudentModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentDetailsListForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Year", model.Year);
                cmd.Parameters.AddWithValue("@BatchSerial", model.BatchSerial);
                cmd.Parameters.AddWithValue("@ClassSerial", model.ClassSerial);
                cmd.Parameters.AddWithValue("@Status", model.Status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["Year"].ToString();
                    row["S2"] = reader["BatchName"].ToString();
                    row["S3"] = reader["ClassName"].ToString();
                    row["S4"] = reader["FullName"].ToString();
                    row["S5"] = reader["Status"].ToString();
                    row["S6"] = reader["RegNo"].ToString();
                    row["S7"] = reader["No"].ToString();

                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("StudentDetailsList"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/StudentDetailsList.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "StudentDetailsList.pdf");


        }

        //
        // GET: /StudentDetails/

        public ActionResult StudentDetails()
        {
            ViewBag.AllStudents = GetAllStudentsActiveFilterUserGroup(); // for dropdown

            return View();
        }

        //Get students to dropdown
        private List<StudentModels> GetAllStudentsActive()
        {
            List<StudentModels> list = new List<StudentModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllStudentsRegNo", con);
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

        //Get students to dropdown filtering with user groupd
        private List<StudentModels> GetAllStudentsActiveFilterUserGroup()
        {
            int empserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int usergroupserial = Session["UserGroupSerial"] != null ? Convert.ToInt32(Session["UserGroupSerial"].ToString()) : 0;

            List<StudentModels> list = new List<StudentModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllStudentsRegNoFilterUserGroup", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpSerial", empserial);
                cmd.Parameters.AddWithValue("@UserGroupSerial", usergroupserial);
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

        [HttpPost]
        public ActionResult StudentDetails(StudentModels model)
        {

            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentDetailsForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RegNo", model.RegNo);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["RegNo"].ToString();
                    row["S2"] = reader["Initials"].ToString();
                    row["S3"] = reader["FirstName"].ToString();
                    row["S4"] = reader["LastName"].ToString();
                    row["S5"] = reader["FullName"].ToString();
                    row["S6"] = Convert.ToDateTime(reader["DateofBirth"]).ToString("dd/MM/yyyy");
                    row["S7"] = reader["Gender"].ToString();
                    row["S8"] = reader["Address1"].ToString();
                    row["S9"] = reader["Address2"].ToString();
                    row["S10"] = reader["Address3"].ToString();
                    row["S11"] = reader["City"].ToString();
                    row["S12"] = reader["ResidencePhone"].ToString();
                    row["DT1"] = Convert.ToDateTime(reader["AdmissionDate"]).ToString("dd/MM/yyyy");
                    row["S13"] = reader["Status"].ToString();
                    row["S14"] = reader["GuardianType"].ToString();
                    row["S15"] = reader["GuardianPhone"].ToString();
                    row["S16"] = reader["GuardianName"].ToString();
                    row["S17"] = reader["BatchName"].ToString();
                    row["S18"] = reader["ClassName"].ToString();


                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("StudentDetails"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/StudentDetails.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "StudentDetails.pdf");


        }

        //
        // GET: /TeacherDetailList/

        public ActionResult TeacherDetailList()
        {

            return View();
        }

        //search teachers according to usergroup serial, status
        public ActionResult SearchTeacherDetailsListForPDF(int userGroupSerial, string status)
        {

            List<TeacherModels> teachers = new List<TeacherModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchTeacherDetailsListForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserGroupSerial", userGroupSerial);
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                int i = 1;

                while (reader.Read())
                {
                    teachers.Add(new TeacherModels
                    {
                        No = i++,
                        RegNo = reader["RegNo"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        Status = reader["Status"].ToString(),
                        TelePhone = reader["TelePhone"].ToString(),
                        UserGroupName = reader["UserGroupName"].ToString()

                    });
                }
            }
            if (teachers == null || !teachers.Any())
            {
                TempData["Message"] = "No teachers found.";
            }

            return Json(teachers, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult TeacherDetailList(TeacherModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchTeacherDetailsListForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserGroupSerial", model.UserGroupSerial);
                cmd.Parameters.AddWithValue("@Status", model.Status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["RegNo"].ToString();
                    row["S2"] = reader["FullName"].ToString();
                    row["S3"] = reader["TelePhone"].ToString();
                    row["S4"] = reader["Status"].ToString();
                    row["S5"] = reader["UserGroupName"].ToString();
                    row["S6"] = reader["No"].ToString();

                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("TeacherDetailList"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/TeacherDetailList.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "TeacherDetailList.pdf");


        }

        //Get Teachers to dropdown
        private List<TeacherModels> GetAllTeachers()
        {
            List<TeacherModels> list = new List<TeacherModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllTeachersRegNo", con);
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

        //Get Teachers to dropdown filter using user group
        private List<TeacherModels> GetAllTeachersFilterUserGroup()
        {
            int empserial = Session["EmpSerial"] != null ? Convert.ToInt32(Session["EmpSerial"].ToString()) : 0;
            int usergroupserial = Session["UserGroupSerial"] != null ? Convert.ToInt32(Session["UserGroupSerial"].ToString()) : 0;

            List<TeacherModels> list = new List<TeacherModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("GetAllTeachersRegNoGetAllTeachersRegNo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpSerial", empserial);
                cmd.Parameters.AddWithValue("@UserGroupSerial", usergroupserial);
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
        // GET: /TeacherDetails/

        public ActionResult TeacherDetails()
        {
            ViewBag.AllTeachers = GetAllTeachersFilterUserGroup(); // for dropdown

            return View();
        }

       

        [HttpPost]
        public ActionResult TeacherDetails(TeacherModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchTeacherDetailsForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RegNo", model.RegNo);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();

                    // fields what used in report.rpt
                    row["S1"] = reader["RegNo"].ToString();
                    row["S2"] = reader["Initials"].ToString();
                    row["S3"] = reader["FirstName"].ToString();
                    row["S4"] = reader["LastName"].ToString();
                    row["S5"] = reader["FullName"].ToString();
                    row["S6"] = reader["Gender"].ToString();
                    row["S7"] = reader["NIC"].ToString();
                    row["S8"] = reader["TelePhone"].ToString();
                    row["S9"] = reader["UserGroupName"].ToString();
                    row["S10"] = reader["Status"].ToString();
                    row["DT1"] = Convert.ToDateTime(reader["DateofBirth"]).ToString("dd/MM/yyyy");
                    row["DT2"] = Convert.ToDateTime(reader["RegisterDate"]).ToString("dd/MM/yyyy");

                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("TeacherDetails"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/TeacherDetails.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to PDF
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "TeacherDetails.pdf");


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


        //
        // GET: /RegisteredStudents/

        public ActionResult RegisteredStudents()
        {

            return View();
        }


        

        [HttpPost]
        public ActionResult RegisteredStudents(StudentModels model)
        {
            DataSet1 dataset = new DataSet1();
            DataSet1.DataTable1DataTable dataTable = dataset.DataTable1;
            DataSet1.DataTable1Row row;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchRegisteredStudentsExcel", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", model.Status);

                
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    row = dataTable.NewDataTable1Row();
                    // fields what used in report.rpt
                    row["S1"] = reader["No"].ToString();
                    row["S2"] = reader["RegNo"].ToString();
                    row["S3"] = reader["FullName"].ToString();
                    row["S4"] = Convert.ToDateTime(reader["DateofBirth"]).ToString("yyyy-MM-dd");
                    row["S5"] = reader["Gender"].ToString();
                    row["S6"] = reader["Address"].ToString();
                    row["S7"] = reader["ResidencePhone"].ToString();
                    row["S8"] = Convert.ToDateTime(reader["AdmissionDate"]).ToString("yyyy-MM-dd");
                    row["S9"] = reader["Status"].ToString();

                    dataTable.AddDataTable1Row(row);
                }

                reader.Close();
            }

            // Validate if no data returned
            if (dataTable.Rows.Count == 0)
            {
                TempData["Message"] = "No data found for the selected criteria.";
                return RedirectToAction("RegisteredStudents"); // redirect to view
            }

            // Load the Crystal Report
            ReportDocument report = new ReportDocument();
            report.Load(Server.MapPath("~/Report/RegisteredStudents.rpt")); //  .rpt path

            // Set data source
            report.SetDataSource(dataset);


            // Export to Excel
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/vnd.ms-excel", "RegisteredStudents.xls");


        }

        public JsonResult SearchRegisteredStudentsForGrid(string status)
        {
            var students = new List<object>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchRegisteredStudentsExcel", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    students.Add(new
                    {
                        No = reader["No"].ToString(),
                        RegNo = reader["RegNo"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        DateofBirth = Convert.ToDateTime(reader["DateofBirth"]).ToString("yyyy-MM-dd"),
                        Status = reader["Status"].ToString()
                    });
                }
                reader.Close();
            }
            return Json(students, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchStudentDetailsForGrid(string regNo)
        {
            var students = new List<object>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchStudentDetailsForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (string.IsNullOrEmpty(regNo)) {
                    cmd.Parameters.AddWithValue("@RegNo", DBNull.Value);
                } else {
                    cmd.Parameters.AddWithValue("@RegNo", regNo);
                }

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    students.Add(new
                    {
                        RegNo = reader["RegNo"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        DateofBirth = Convert.ToDateTime(reader["DateofBirth"]).ToString("yyyy-MM-dd"),
                        Status = reader["Status"].ToString()
                    });
                }
                reader.Close();
            }
            return Json(students, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchTeacherDetailsForGrid(string regNo)
        {
            var teachers = new List<object>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SearchTeacherDetailsForPDF", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (string.IsNullOrEmpty(regNo)) {
                    cmd.Parameters.AddWithValue("@RegNo", DBNull.Value);
                } else {
                    cmd.Parameters.AddWithValue("@RegNo", regNo);
                }

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    teachers.Add(new
                    {
                        RegNo = reader["RegNo"].ToString(),
                        FullName = reader["FullName"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        NIC = reader["NIC"].ToString(),
                        UserGroupName = reader["UserGroupName"].ToString(),
                        Status = reader["Status"].ToString()
                    });
                }
                reader.Close();
            }
            return Json(teachers, JsonRequestBehavior.AllowGet);
        }

    }
}
