using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication1.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace MvcApplication1.Controllers
{
    public class BankAccountController : Controller
    {

        private readonly string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //
        // GET: /BankAccount/

        public ActionResult Cretae()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(BankAccountModel bankaccount)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    SqlCommand cmd = new SqlCommand("addnewaccount", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@name", bankaccount.Name);
                    cmd.Parameters.AddWithValue("@accountno", bankaccount.AccountNo);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return View();
        }




    }
}
