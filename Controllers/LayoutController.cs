using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Mvc;
using MvcApplication1.Models;

namespace MvcApplication1.Controllers
{
    public class LayoutController : Controller
    {
        private readonly string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        [ChildActionOnly]
        public ActionResult RenderMenu()
        {
            int userGroupSerial = Session["UserGroupSerial"] != null ? Convert.ToInt32(Session["UserGroupSerial"]) : 0;
            
            List<AppMenu> allowedMenus = GetAllowedMenusForGroup(userGroupSerial);
            
            // Build hierarchical parent-child list
            List<AppMenu> sidebarMenu = new List<AppMenu>();
            Dictionary<int, AppMenu> parentMap = new Dictionary<int, AppMenu>();

            // First pass: identify all top-level menus (ParentMenuId is null)
            foreach (var menu in allowedMenus)
            {
                if (menu.ParentMenuId == null)
                {
                    parentMap[menu.MenuId] = menu;
                    sidebarMenu.Add(menu);
                }
            }

            // Second pass: attach sub-menus to parents
            foreach (var menu in allowedMenus)
            {
                if (menu.ParentMenuId.HasValue && parentMap.ContainsKey(menu.ParentMenuId.Value))
                {
                    parentMap[menu.ParentMenuId.Value].ChildMenus.Add(menu);
                }
            }

            return PartialView("_SidebarMenu", sidebarMenu);
        }

        private List<AppMenu> GetAllowedMenusForGroup(int userGroupSerial)
        {
            List<AppMenu> list = new List<AppMenu>();
            using (SqlConnection con = new SqlConnection(conStr))
            {
                // Super Admin (Group 1) gets everything. Others check RolePrivilege
                string query = @"
                    SELECT m.* 
                    FROM AppMenu m
                    WHERE m.Status = 'Active'
                      AND (@UserGroupSerial = 1 OR EXISTS (
                          SELECT 1 FROM RolePrivilege rp 
                          WHERE rp.MenuId = m.MenuId 
                            AND rp.UserGroupSerial = @UserGroupSerial 
                            AND rp.CanView = 1
                      ))
                    ORDER BY m.DisplayOrder ASC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserGroupSerial", userGroupSerial);
                
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new AppMenu
                        {
                            MenuId = Convert.ToInt32(reader["MenuId"]),
                            MenuName = reader["MenuName"].ToString(),
                            ControllerName = reader["ControllerName"] == DBNull.Value ? null : reader["ControllerName"].ToString(),
                            ActionName = reader["ActionName"] == DBNull.Value ? null : reader["ActionName"].ToString(),
                            Url = reader["Url"] == DBNull.Value ? null : reader["Url"].ToString(),
                            Icon = reader["Icon"] == DBNull.Value ? null : reader["Icon"].ToString(),
                            ParentMenuId = reader["ParentMenuId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["ParentMenuId"]),
                            DisplayOrder = Convert.ToInt32(reader["DisplayOrder"]),
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        [HttpGet]
        public JsonResult GetNotifications()
        {
            var notifications = new List<NotificationItem>();

            if (Session["UserGroupName"] == null)
                return Json(notifications, JsonRequestBehavior.AllowGet);

            string userGroup = Session["UserGroupName"].ToString();
            
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    con.Open();

                    if (userGroup == "Teacher" && Session["EmpSerial"] != null)
                    {
                        int empSerial = Convert.ToInt32(Session["EmpSerial"]);

                        // Class Teacher Notifications
                        SqlCommand cmdClass = new SqlCommand("SELECT COUNT(*) FROM ExamResult er INNER JOIN Class c ON er.ClassSerial = c.ClassSerial WHERE c.TeacherSerial = @EmpSerial AND er.ApprovedClassTeacher = 'Pending'", con);
                        cmdClass.Parameters.AddWithValue("@EmpSerial", empSerial);
                        int classPending = (int)cmdClass.ExecuteScalar();
                        if (classPending > 0)
                            notifications.Add(new NotificationItem { Message = classPending + " Results pending Class Teacher Approval", Url = "/Result/ApprovedResultClassTeacher", Icon = "clipboard-outline" });

                        // Batch Head Notifications
                        SqlCommand cmdBatch = new SqlCommand("SELECT COUNT(*) FROM ExamResult er INNER JOIN Batch b ON er.BatchSerial = b.BatchSerial WHERE b.TeacherSerial = @EmpSerial AND er.ApprovedClassTeacher = 'Approved' AND er.ApprovedByBatchHead = 'Pending'", con);
                        cmdBatch.Parameters.AddWithValue("@EmpSerial", empSerial);
                        int batchPending = (int)cmdBatch.ExecuteScalar();
                        if (batchPending > 0)
                            notifications.Add(new NotificationItem { Message = batchPending + " Results pending Batch Head Approval", Url = "/Result/ApprovedResultBatchHead", Icon = "folder-open-outline" });

                        // Section Head Notifications
                        SqlCommand cmdSection = new SqlCommand("SELECT COUNT(*) FROM ExamResult er INNER JOIN Section s ON er.SectionSerial = s.SectionSerial WHERE s.TeacherSerial = @EmpSerial AND er.ApprovedByBatchHead = 'Approved' AND er.ApprovedBySectionHead = 'Pending'", con);
                        cmdSection.Parameters.AddWithValue("@EmpSerial", empSerial);
                        int sectionPending = (int)cmdSection.ExecuteScalar();
                        if (sectionPending > 0)
                            notifications.Add(new NotificationItem { Message = sectionPending + " Results pending Section Head Approval", Url = "/Result/ApprovedResultSectionHead", Icon = "library-outline" });

                        // Online Exams missing questions
                        SqlCommand cmdOnline = new SqlCommand(@"
                            SELECT COUNT(*) 
                            FROM ExamScheduleAssignment 
                            WHERE Status = 'Pending' AND ExamType = 'Online'
                              AND ClassSerial IN (SELECT ClassSerial FROM ClassSubjectTeacher WHERE TeacherSerial = @EmpSerial)", con);
                        cmdOnline.Parameters.AddWithValue("@EmpSerial", empSerial);
                        int onlinePending = (int)cmdOnline.ExecuteScalar();
                        if (onlinePending > 0)
                            notifications.Add(new NotificationItem { Message = onlinePending + " Online Assignments missing questions", Url = "/OnlineExam/AddQuestions", Icon = "create-outline" });
                    }
                    else if (userGroup == "Principal")
                    {
                        // Principal Approvals
                        SqlCommand cmdPrincipal = new SqlCommand("SELECT COUNT(*) FROM ExamResult WHERE ApprovedBySectionHead = 'Approved' AND ApprovedByPrincipal = 'Pending'", con);
                        int principalPending = (int)cmdPrincipal.ExecuteScalar();
                        if (principalPending > 0)
                            notifications.Add(new NotificationItem { Message = principalPending + " Results pending Principal Approval", Url = "/Result/ApprovedResultPrincipal", Icon = "shield-checkmark-outline" });
                    }
                    else if ((userGroup == "Student" || userGroup == "Parent") && Session["EmpSerial"] != null)
                    {
                        int studentSerial = Convert.ToInt32(Session["EmpSerial"]);
                        
                        SqlCommand cmdStudent = new SqlCommand("SELECT ClassSerial, BatchSerial FROM Student WHERE StudentSerial = @StudentSerial", con);
                        cmdStudent.Parameters.AddWithValue("@StudentSerial", studentSerial);
                        using (SqlDataReader reader = cmdStudent.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int classSerial = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                int batchSerial = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                reader.Close();

                                SqlCommand cmdActiveExams = new SqlCommand(@"
                                    SELECT COUNT(*) 
                                    FROM ExamScheduleAssignment esa
                                    WHERE esa.ExamType = 'Online' 
                                      AND esa.Status = 'Pending' 
                                      AND esa.ClassSerial = @ClassSerial 
                                      AND esa.BatchSerial = @BatchSerial
                                      AND NOT EXISTS (SELECT 1 FROM ExamAssignmentAttedance eaa WHERE eaa.ExamScheduleAssignmentSerial = esa.ExamScheduleAssignmentSerial AND eaa.StudentSerial = @StudentSerial)", con);
                                
                                cmdActiveExams.Parameters.AddWithValue("@ClassSerial", classSerial);
                                cmdActiveExams.Parameters.AddWithValue("@BatchSerial", batchSerial);
                                cmdActiveExams.Parameters.AddWithValue("@StudentSerial", studentSerial);
                                
                                int activeExams = (int)cmdActiveExams.ExecuteScalar();
                                if (activeExams > 0)
                                    notifications.Add(new NotificationItem { Message = activeExams + " New Online Exams Available", Url = "/OnlineExam/TakeExam", Icon = "laptop-outline" });
                            }
                            else
                            {
                                reader.Close();
                            }
                        }

                        // Final Exam Results Approved by Principal
                        SqlCommand cmdFinal = new SqlCommand(@"
                            SELECT DISTINCT t.TermName, e.ExamName
                            FROM ExamResult er
                            INNER JOIN Term t ON er.TermSerial = t.TermSerial
                            INNER JOIN Exam e ON er.ExamSerial = e.ExamSerial
                            WHERE er.StudentSerial = @StudentSerial 
                              AND er.ApprovedByPrincipal = 'Approved'", con);
                        cmdFinal.Parameters.AddWithValue("@StudentSerial", studentSerial);
                        using (SqlDataReader readerFinal = cmdFinal.ExecuteReader())
                        {
                            while (readerFinal.Read())
                            {
                                string termName = readerFinal["TermName"].ToString();
                                string examName = readerFinal["ExamName"].ToString();
                                notifications.Add(new NotificationItem { 
                                    Message = "Final Exam Results Released: " + termName + " - " + examName, 
                                    Url = "/ResultReport/StudentExamResultReport", 
                                    Icon = "document-text-outline" 
                                });
                            }
                        }

                        // Assignment Results Approved by Class Teacher
                        SqlCommand cmdAssignment = new SqlCommand(@"
                            SELECT DISTINCT t.TermName, e.ExamName, s.SubjectName
                            FROM ExamAssignmentAttedance eaa
                            INNER JOIN Term t ON eaa.TermSerial = t.TermSerial
                            INNER JOIN Exam e ON eaa.ExamSerial = e.ExamSerial
                            INNER JOIN Subject s ON eaa.SubjectSerial = s.SubjectSerial
                            WHERE eaa.StudentSerial = @StudentSerial 
                              AND eaa.ApprovedClassTeacher = 'Approved'", con);
                        cmdAssignment.Parameters.AddWithValue("@StudentSerial", studentSerial);
                        using (SqlDataReader readerAssignment = cmdAssignment.ExecuteReader())
                        {
                            while (readerAssignment.Read())
                            {
                                string termName = readerAssignment["TermName"].ToString();
                                string examName = readerAssignment["ExamName"].ToString();
                                string subjectName = readerAssignment["SubjectName"].ToString();
                                notifications.Add(new NotificationItem { 
                                    Message = "Assignment Results Released: " + termName + " - " + examName + " (" + subjectName + ")", 
                                    Url = "/ResultReport/StudentExamResultReport", 
                                    Icon = "clipboard-outline" 
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Ignore DB errors during notification fetch to prevent page load failures
            }

            return Json(notifications, JsonRequestBehavior.AllowGet);
        }
    }

    public class NotificationItem
    {
        public string Message { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
    }
}
