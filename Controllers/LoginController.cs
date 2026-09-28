using Google.Protobuf.Collections;
using MySql.Data.MySqlClient;
using MyVizCollections.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Mvc;



namespace MyVizCollections.Controllers
{
    public class LoginController : Controller
    {
        // =========================
        // LOGIN PAGE
        // =========================
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                var bannerMessage = GetBannerMessage();

                Session["BannerMessage"] = bannerMessage;
                Session["IsCustomBanner"] =
                    bannerMessage != "Professional Preview Services";

                return View();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }


        // =========================
        // GET BANNER MESSAGE
        // =========================
        public static string GetBannerMessage()
        {
            string bannerMessage = "";

            string connectionString =
                ConfigurationManager
                .ConnectionStrings["Nerolacconstr"]
                .ConnectionString;

            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();

                using (MySqlCommand cmd =
                       new MySqlCommand("SP_BannerMessage", conn))
                {
                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.CommandTimeout = 1600;

                    object result = cmd.ExecuteScalar();

                    if (result != null &&
                        !string.IsNullOrEmpty(result.ToString()))
                    {
                        bannerMessage = result.ToString();
                    }
                    else
                    {
                        bannerMessage =
                            "Professional Preview Services";
                    }
                }
                conn.Close();

            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

                MyVizCollections.Models.ExceptionLogging.SendErrorToText(ex);

                bannerMessage = "Professional Preview Services";
            }

            return bannerMessage;
        }


        // =========================
        // LOGIN
        // =========================
        [HttpPost]
        public ActionResult Index(User user)
        {
            try
            {
                string username =
                    user.Username == null
                        ? ""
                        : user.Username.Trim();

                string password =
                    user.Password == null
                        ? ""
                        : user.Password.Trim();


                // ============================================
                // 1. INSYADMIN LOGIN
                // ============================================
                if (username == "Insyadmin" &&
                    password == "!n&dia@12$")
                {
                    Session["Username"] = username;
                    Session["UserID"] = username;
                    Session["LoginTime"] = DateTime.Now;

                    return RedirectToAction(
                        "Index",
                        "AllLevelQueueBoard");
                }


                // ============================================
                // 2. ACTON05 LOGIN
                // ============================================
                else if (username == "ActOn05" &&
                         password == "@Act#$05&")
                {
                    Session["Username"] = username;
                    Session["UserID"] = username;
                    Session["LoginTime"] = DateTime.Now;

                    return RedirectToAction(
                        "Index",
                        "Acton05");
                }


                // ============================================
                // 3. VIEW REPORT LOGIN
                // ============================================
                else if (username == "viewreport" &&
                         password == "viewreport")
                {
                    Session["Username"] = username;
                    Session["UserID"] = username;
                    Session["LoginTime"] = DateTime.Now;

                    return RedirectToAction(
                        "Index",
                        "ViewReport");
                }

                // ============================================
                // 4. pseqc check --
                // ============================================
                else if ((username == "LAWRENL5" || username == "ROGERL5") &&
           password == "admin@2#4")
                {
                    Session["Username"] = username;
                    Session["UserID"] = username;
                    Session["LoginTime"] = DateTime.Now;

                    return RedirectToAction(
                        "Index",
                        "PRIQC");
                }

                // ============================================
                // CWCOP LOGIN
                // PSEID + PSEPIN ONLY
                // ============================================
                else
                {
                    string constr =
                        ConfigurationManager
                        .ConnectionStrings["Nerolacconstr"]
                        .ConnectionString;

                    using (MySqlConnection con =
                           new MySqlConnection(constr))
                    using (MySqlCommand com =
                           new MySqlCommand(
                               "SP_CWCOP_Validateuser",
                               con))
                    {
                        com.CommandType =
                            CommandType.StoredProcedure;

                        com.CommandTimeout = 1600;


                        // PSEID
                        com.Parameters.Add(
                            "@iUsername",
                            MySqlDbType.VarChar,
                            8
                        ).Value = username;


                        // PSEPIN
                        com.Parameters.Add(
                            "@iPassword",
                            MySqlDbType.VarChar,
                            12
                        ).Value = password;


                        DataSet ds = new DataSet();

                        using (MySqlDataAdapter ad =
                               new MySqlDataAdapter(com))
                        {
                            ad.Fill(ds);
                        }


                        if (ds != null &&
                            ds.Tables.Count > 0 &&
                            ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow row =
                                ds.Tables[0].Rows[0];

                            if (row["sCode"].ToString() == "1")
                            {
                                // Only PSEID is required for session
                                Session["Username"] =
                                    row["PSEID"].ToString();

                                Session["UserID"] =
                                    row["PSEID"].ToString();

                                Session["LoginTime"] =
                                    DateTime.Now;

                                return RedirectToAction(
                                    "Index",
                                    "Tatreport");
                            }
                        }
                    }


                    ModelState.AddModelError(
                        "",
                        "Invalid username or password.");

                    return View();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);

                return View("Error");
            }
        }

        // =========================
        // LOGOUT
        // =========================
        public ActionResult Logout()
        {
            try
            {
                // Clear login session
                Session.Clear();

                // Abandon current session
                Session.Abandon();

                return RedirectToAction(
                    "Index",
                    "Login");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);

                return View("Error");
            }
        }
    }
}
