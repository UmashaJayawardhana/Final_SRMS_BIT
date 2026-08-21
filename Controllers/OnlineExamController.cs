using MvcApplication1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class OnlineExamController : Controller
    {
        private readonly string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: OnlineExam/AddQuestions
        public ActionResult AddQuestions()
        {
            if (Session["EmpSerial"] == null)
            {
                return RedirectToAction("LogOn", "Account");
            }

            ViewBag.Years = GetYears();
            ViewBag.Terms = GetTerms();

            return View();
        }

        // GET: OnlineExam/TakeExam
        public ActionResult TakeExam()
        {
            if (Session["EmpSerial"] == null || Session["UserGroupName"] == null || Session["UserGroupName"].ToString() != "Student")
            {
                return RedirectToAction("LogOn", "Account");
            }

            ViewBag.Years = GetYears();
            ViewBag.Terms = GetTerms();

            return View();
        }

        // GET: OnlineExam/ExamHistory
        public ActionResult ExamHistory()
        {
            if (Session["EmpSerial"] == null || Session["UserGroupName"] == null || Session["UserGroupName"].ToString() != "Student")
            {
                return RedirectToAction("LogOn", "Account");
            }

            return View();
        }

        private SelectList GetYears()
        {
            List<SelectListItem> years = new List<SelectListItem>();
            for (int i = 2027; i >= 2020; i--)
            {
                years.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            return new SelectList(years, "Value", "Text");
        }

        private List<TermModels> GetTerms()
        {
            List<TermModels> terms = new List<TermModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT TermSerial, TermName FROM Term WHERE Status = 'Active'", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    terms.Add(new TermModels
                    {
                        TermSerial = reader.GetInt32(0),
                        TermName = reader.GetString(1)
                    });
                }
            }
            return terms;
        }

        [HttpGet]
        public JsonResult GetPendingOnlineExams(int year, int batchSerial, int classSerial, int termSerial, int subjectSerial, int examSerial)
        {
            List<SelectListItem> exams = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = @"
                    SELECT ExamScheduleAssignmentSerial, ExamName + ' - ' + Convert(varchar, DateTime, 120) as DisplayText
                    FROM ExamSceduleAssignment e
                    INNER JOIN Exam x ON e.ExamSerial = x.ExamSerial
                    WHERE e.Year = @Year AND e.BatchSerial = @BatchSerial AND e.ClassSerial = @ClassSerial 
                      AND e.TermSerial = @TermSerial AND e.SubjectSerial = @SubjectSerial AND e.ExamSerial = @ExamSerial
                      AND e.ExamType = 'Online' AND e.Status = 'Pending'";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchSerial);
                cmd.Parameters.AddWithValue("@ClassSerial", classSerial);
                cmd.Parameters.AddWithValue("@TermSerial", termSerial);
                cmd.Parameters.AddWithValue("@SubjectSerial", subjectSerial);
                cmd.Parameters.AddWithValue("@ExamSerial", examSerial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    exams.Add(new SelectListItem
                    {
                        Value = reader["ExamScheduleAssignmentSerial"].ToString(),
                        Text = reader["DisplayText"].ToString()
                    });
                }
            }
            return Json(exams, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveQuestions(List<OnlineAssignmentQuestionModels> questions)
        {
            if (questions == null || !questions.Any())
                return Json(new { success = false, message = "No questions provided." });

            int assignmentSerial = questions.First().ExamScheduleAssignmentSerial;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                // First, delete existing questions for this assignment if they exist
                using (SqlCommand cmdDel = new SqlCommand("DELETE FROM OnlineAssignmentQuestion WHERE ExamScheduleAssignmentSerial = @ExamScheduleAssignmentSerial", con))
                {
                    cmdDel.Parameters.AddWithValue("@ExamScheduleAssignmentSerial", assignmentSerial);
                    cmdDel.ExecuteNonQuery();
                }

                // Insert new questions
                foreach (var q in questions)
                {
                    string query = @"INSERT INTO OnlineAssignmentQuestion (ExamScheduleAssignmentSerial, QuestionText, Option1, Option2, Option3, Option4, CorrectOption)
                                     VALUES (@ExamScheduleAssignmentSerial, @QuestionText, @Option1, @Option2, @Option3, @Option4, @CorrectOption)";
                    using (SqlCommand cmdIns = new SqlCommand(query, con))
                    {
                        cmdIns.Parameters.AddWithValue("@ExamScheduleAssignmentSerial", q.ExamScheduleAssignmentSerial);
                        cmdIns.Parameters.AddWithValue("@QuestionText", q.QuestionText ?? "");
                        cmdIns.Parameters.AddWithValue("@Option1", q.Option1 ?? "");
                        cmdIns.Parameters.AddWithValue("@Option2", q.Option2 ?? "");
                        cmdIns.Parameters.AddWithValue("@Option3", q.Option3 ?? "");
                        cmdIns.Parameters.AddWithValue("@Option4", q.Option4 ?? "");
                        cmdIns.Parameters.AddWithValue("@CorrectOption", q.CorrectOption);
                        cmdIns.ExecuteNonQuery();
                    }
                }
            }

            return Json(new { success = true, message = "Questions saved successfully!" });
        }


        // Used by students to load an active online exam
        [HttpGet]
        public JsonResult GetActiveOnlineExams(int year, int batchSerial, int classSerial, int termSerial, int subjectSerial, int examSerial)
        {
            List<SelectListItem> exams = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                // Find online exams for this class. Can be Held or Pending depending on teacher behavior. 
                // We'll allow taking it if it's Online and today's date matches (or just allow if it's Online).
                string query = @"
                    SELECT ExamScheduleAssignmentSerial, ExamName + ' - ' + Convert(varchar, DateTime, 120) as DisplayText
                    FROM ExamSceduleAssignment e
                    INNER JOIN Exam x ON e.ExamSerial = x.ExamSerial
                    WHERE e.Year = @Year AND e.BatchSerial = @BatchSerial AND e.ClassSerial = @ClassSerial 
                      AND e.TermSerial = @TermSerial AND e.SubjectSerial = @SubjectSerial AND e.ExamSerial = @ExamSerial
                      AND e.ExamType = 'Online'";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@BatchSerial", batchSerial);
                cmd.Parameters.AddWithValue("@ClassSerial", classSerial);
                cmd.Parameters.AddWithValue("@TermSerial", termSerial);
                cmd.Parameters.AddWithValue("@SubjectSerial", subjectSerial);
                cmd.Parameters.AddWithValue("@ExamSerial", examSerial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    exams.Add(new SelectListItem
                    {
                        Value = reader["ExamScheduleAssignmentSerial"].ToString(),
                        Text = reader["DisplayText"].ToString()
                    });
                }
            }
            return Json(exams, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetStudentPastExams()
        {
            if (Session["EmpSerial"] == null)
            {
                return Json(new { success = false, message = "Session expired." }, JsonRequestBehavior.AllowGet);
            }

            int studentSerial = Convert.ToInt32(Session["EmpSerial"]);
            List<object> pastExams = new List<object>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                string query = @"
                    SELECT 
                        e.ExamScheduleAssignmentSerial, 
                        s.SubjectName, 
                        t.TermName, 
                        x.ExamName, 
                        c.ClassName, 
                        a.Result, 
                        e.DateTime
                    FROM ExamAssignmentAttedance a
                    JOIN ExamSceduleAssignment e ON a.ExamScheduleAssignmentSerial = e.ExamScheduleAssignmentSerial
                    JOIN Class c ON a.ClassSerial = c.ClassSerial
                    JOIN Subject s ON a.SubjectSerial = s.SubjectSerial
                    JOIN Term t ON a.TermSerial = t.TermSerial
                    JOIN Exam x ON a.ExamSerial = x.ExamSerial
                    WHERE a.StudentSerial = @StudentSerial AND e.ExamType = 'Online'
                    ORDER BY e.DateTime DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StudentSerial", studentSerial);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    pastExams.Add(new
                    {
                        ExamScheduleAssignmentSerial = reader["ExamScheduleAssignmentSerial"],
                        SubjectName = reader["SubjectName"].ToString(),
                        TermName = reader["TermName"].ToString(),
                        ExamName = reader["ExamName"].ToString(),
                        ClassName = reader["ClassName"].ToString(),
                        Result = Convert.ToDecimal(reader["Result"]),
                        Date = Convert.ToDateTime(reader["DateTime"]).ToString("yyyy-MM-dd HH:mm")
                    });
                }
            }
            return Json(pastExams, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetQuestions(int examScheduleAssignmentSerial)
        {
            List<OnlineAssignmentQuestionModels> questions = new List<OnlineAssignmentQuestionModels>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM OnlineAssignmentQuestion WHERE ExamScheduleAssignmentSerial = @serial", con);
                cmd.Parameters.AddWithValue("@serial", examScheduleAssignmentSerial);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    questions.Add(new OnlineAssignmentQuestionModels
                    {
                        QuestionSerial = reader.GetInt32(reader.GetOrdinal("QuestionSerial")),
                        ExamScheduleAssignmentSerial = reader.GetInt32(reader.GetOrdinal("ExamScheduleAssignmentSerial")),
                        QuestionText = reader.GetString(reader.GetOrdinal("QuestionText")),
                        Option1 = reader.GetString(reader.GetOrdinal("Option1")),
                        Option2 = reader.GetString(reader.GetOrdinal("Option2")),
                        Option3 = reader.GetString(reader.GetOrdinal("Option3")),
                        Option4 = reader.GetString(reader.GetOrdinal("Option4")),
                        CorrectOption = reader.GetInt32(reader.GetOrdinal("CorrectOption"))
                    });
                }
            }
            return Json(questions, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitExam(int examScheduleAssignmentSerial, decimal resultMarks, int year, int batchSerial, int classSerial, int termSerial, int subjectSerial, int examSerial, List<OnlineAssignmentStudentAnswerModels> studentAnswers)
        {
            if (Session["EmpSerial"] == null)
            {
                return Json(new { success = false, message = "Session expired." });
            }

            int studentSerial = Convert.ToInt32(Session["EmpSerial"]);

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                string query = @"
                    IF NOT EXISTS (SELECT 1 FROM ExamAssignmentAttedance WHERE StudentSerial = @StudentSerial AND ExamScheduleAssignmentSerial = @ExamScheduleAssignmentSerial)
                    BEGIN
                        INSERT INTO ExamAssignmentAttedance (StudentSerial, ExamScheduleAssignmentSerial, Attedance, Year, BatchSerial, ClassSerial, TermSerial, SubjectSerial, ExamSerial)
                        VALUES (@StudentSerial, @ExamScheduleAssignmentSerial, 'Present', @Year, @BatchSerial, @ClassSerial, @TermSerial, @SubjectSerial, @ExamSerial);
                    END
                    
                    UPDATE ExamAssignmentAttedance 
                    SET Result = @Result, Attedance = 'Present' 
                    WHERE StudentSerial = @StudentSerial AND ExamScheduleAssignmentSerial = @ExamScheduleAssignmentSerial;
                ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentSerial", studentSerial);
                    cmd.Parameters.AddWithValue("@ExamScheduleAssignmentSerial", examScheduleAssignmentSerial);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@BatchSerial", batchSerial);
                    cmd.Parameters.AddWithValue("@ClassSerial", classSerial);
                    cmd.Parameters.AddWithValue("@TermSerial", termSerial);
                    cmd.Parameters.AddWithValue("@SubjectSerial", subjectSerial);
                    cmd.Parameters.AddWithValue("@ExamSerial", examSerial);
                    cmd.Parameters.AddWithValue("@Result", resultMarks);
                    
                    cmd.ExecuteNonQuery();
                }

                // Delete any previous answers if somehow they retake it
                using (SqlCommand cmdDel = new SqlCommand("DELETE FROM OnlineAssignmentStudentAnswer WHERE StudentSerial = @StudentSerial AND ExamScheduleAssignmentSerial = @ExamScheduleAssignmentSerial", con))
                {
                    cmdDel.Parameters.AddWithValue("@StudentSerial", studentSerial);
                    cmdDel.Parameters.AddWithValue("@ExamScheduleAssignmentSerial", examScheduleAssignmentSerial);
                    cmdDel.ExecuteNonQuery();
                }

                if (studentAnswers != null && studentAnswers.Any())
                {
                    foreach (var ans in studentAnswers)
                    {
                        using (SqlCommand cmdIns = new SqlCommand("INSERT INTO OnlineAssignmentStudentAnswer (ExamScheduleAssignmentSerial, StudentSerial, QuestionSerial, SelectedOption) VALUES (@E, @S, @Q, @O)", con))
                        {
                            cmdIns.Parameters.AddWithValue("@E", examScheduleAssignmentSerial);
                            cmdIns.Parameters.AddWithValue("@S", studentSerial);
                            cmdIns.Parameters.AddWithValue("@Q", ans.QuestionSerial);
                            cmdIns.Parameters.AddWithValue("@O", ans.SelectedOption);
                            cmdIns.ExecuteNonQuery();
                        }
                    }
                }
            }

            return Json(new { success = true, message = "Exam submitted successfully!" });
        }

        [HttpGet]
        public JsonResult GetExamDataForStudent(int examScheduleAssignmentSerial)
        {
            if (Session["EmpSerial"] == null)
            {
                return Json(new { success = false, message = "Session expired." }, JsonRequestBehavior.AllowGet);
            }

            int studentSerial = Convert.ToInt32(Session["EmpSerial"]);

            bool isTaken = false;
            decimal result = 0;
            List<OnlineAssignmentStudentAnswerModels> submittedAnswers = new List<OnlineAssignmentStudentAnswerModels>();
            List<OnlineAssignmentQuestionModels> questions = new List<OnlineAssignmentQuestionModels>();

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                string examStatus = "";
                using (SqlCommand cmdStat = new SqlCommand("SELECT Status FROM ExamSceduleAssignment WHERE ExamScheduleAssignmentSerial = @E", con))
                {
                    cmdStat.Parameters.AddWithValue("@E", examScheduleAssignmentSerial);
                    object statRes = cmdStat.ExecuteScalar();
                    if (statRes != null && statRes != DBNull.Value)
                    {
                        examStatus = statRes.ToString();
                    }
                }

                if (!string.Equals(examStatus, "Held", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = false, message = "This exam is currently marked as '" + examStatus + "'. The teacher must change it to 'Held' before you can start or preview it." }, JsonRequestBehavior.AllowGet);
                }
                
                using (SqlCommand cmd = new SqlCommand("SELECT Result FROM ExamAssignmentAttedance WHERE StudentSerial = @S AND ExamScheduleAssignmentSerial = @E", con))
                {
                    cmd.Parameters.AddWithValue("@S", studentSerial);
                    cmd.Parameters.AddWithValue("@E", examScheduleAssignmentSerial);
                    object res = cmd.ExecuteScalar();
                    if (res != null && res != DBNull.Value)
                    {
                        isTaken = true;
                        result = Convert.ToDecimal(res);
                    }
                }

                if (isTaken)
                {
                    using (SqlCommand cmdAns = new SqlCommand("SELECT QuestionSerial, SelectedOption FROM OnlineAssignmentStudentAnswer WHERE StudentSerial = @S AND ExamScheduleAssignmentSerial = @E", con))
                    {
                        cmdAns.Parameters.AddWithValue("@S", studentSerial);
                        cmdAns.Parameters.AddWithValue("@E", examScheduleAssignmentSerial);
                        using (SqlDataReader reader = cmdAns.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                submittedAnswers.Add(new OnlineAssignmentStudentAnswerModels
                                {
                                    QuestionSerial = reader.GetInt32(0),
                                    SelectedOption = reader.GetInt32(1)
                                });
                            }
                        }
                    }
                }

                using (SqlCommand cmdQ = new SqlCommand("SELECT * FROM OnlineAssignmentQuestion WHERE ExamScheduleAssignmentSerial = @serial", con))
                {
                    cmdQ.Parameters.AddWithValue("@serial", examScheduleAssignmentSerial);
                    using (SqlDataReader reader = cmdQ.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(new OnlineAssignmentQuestionModels
                            {
                                QuestionSerial = reader.GetInt32(reader.GetOrdinal("QuestionSerial")),
                                ExamScheduleAssignmentSerial = reader.GetInt32(reader.GetOrdinal("ExamScheduleAssignmentSerial")),
                                QuestionText = reader.GetString(reader.GetOrdinal("QuestionText")),
                                Option1 = reader.GetString(reader.GetOrdinal("Option1")),
                                Option2 = reader.GetString(reader.GetOrdinal("Option2")),
                                Option3 = reader.GetString(reader.GetOrdinal("Option3")),
                                Option4 = reader.GetString(reader.GetOrdinal("Option4")),
                                CorrectOption = reader.GetInt32(reader.GetOrdinal("CorrectOption"))
                            });
                        }
                    }
                }
            }

            return Json(new { success = true, isTaken = isTaken, result = result, questions = questions, submittedAnswers = submittedAnswers }, JsonRequestBehavior.AllowGet);
        }
    }
}
