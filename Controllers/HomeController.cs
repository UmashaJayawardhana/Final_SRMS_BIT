using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult DashBoard()
        {
            var notifications = new List<DashboardNotificationModel>();
            var timetables = new List<ExamScheduleModels>();
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    con.Open();

                    // Load Notifications
                    using (SqlCommand cmd = new SqlCommand("GetActiveDashboardNotifications", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                notifications.Add(new DashboardNotificationModel
                                {
                                    NotificationId = Convert.ToInt32(reader["NotificationId"]),
                                    Subject = reader["Subject"].ToString(),
                                    MessageText = reader["MessageText"].ToString(),
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                                });
                            }
                        }
                    }

                    // Load Timetables
                    using (SqlCommand cmd = new SqlCommand("GetExamTimeTables", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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

                                timetables.Add(item);
                            }
                        }
                    }

                    // Load Dashboard Stats
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Student", con))
                    {
                        ViewBag.TotalStudents = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Teacher", con))
                    {
                        ViewBag.TotalTeachers = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Exam", con))
                    {
                        ViewBag.TotalExams = (int)cmd.ExecuteScalar();
                    }
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Class", con))
                    {
                        ViewBag.TotalClasses = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                // Optionally log error
            }

            ViewBag.DashboardNotifications = notifications;
            // Get the first 4 timetables
            ViewBag.DashboardTimeTables = timetables.Take(4).ToList();
            return View();
        }

        // GET: /Home/ManageNotifications
        public ActionResult ManageNotifications()
        {
            var notifications = new List<DashboardNotificationModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    SqlCommand cmd = new SqlCommand("GetAllDashboardNotifications", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notifications.Add(new DashboardNotificationModel
                            {
                                NotificationId = Convert.ToInt32(reader["NotificationId"]),
                                Subject = reader["Subject"].ToString(),
                                MessageText = reader["MessageText"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Error loading notifications from database.";
            }

            return View(notifications);
        }

        // POST: /Home/ManageNotifications
        [HttpPost]
        public ActionResult ManageNotifications(DashboardNotificationModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(conStr))
                    {
                        if (model.NotificationId > 0)
                        {
                            SqlCommand cmd = new SqlCommand("UpdateDashboardNotification", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@NotificationId", model.NotificationId);
                            cmd.Parameters.AddWithValue("@Subject", model.Subject);
                            cmd.Parameters.AddWithValue("@MessageText", model.MessageText);
                            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            TempData["Message"] = "Notification updated successfully!";
                        }
                        else
                        {
                            SqlCommand cmd = new SqlCommand("InsertDashboardNotification", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Subject", model.Subject);
                            cmd.Parameters.AddWithValue("@MessageText", model.MessageText);
                            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            TempData["Message"] = "Notification added successfully!";
                        }
                    }
                }
                catch (Exception)
                {
                    TempData["Message"] = "Error saving notification.";
                }
            }
            return RedirectToAction("ManageNotifications");
        }

        // POST: /Home/ToggleNotificationStatus
        [HttpPost]
        public ActionResult ToggleNotificationStatus(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    SqlCommand cmd = new SqlCommand("ToggleDashboardNotificationStatus", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NotificationId", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true, message = "Status toggled successfully!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Failed to toggle status." });
            }
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
