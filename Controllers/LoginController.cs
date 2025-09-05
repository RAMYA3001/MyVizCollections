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
        public ActionResult Index()
        {
           
            var bannerMessage = GetBannerMessage();
            Session["BannerMessage"] = bannerMessage;
            Session["IsCustomBanner"] = bannerMessage != "Professional Preview Services";
            return View();
        }

        public static string GetBannerMessage()
        {
            string bannerMessage = "";
            string connectionString = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SP_BannerMessage", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var result = cmd.ExecuteScalar();
                    if (result != null && !string.IsNullOrEmpty(result.ToString()))
                        bannerMessage = result.ToString();
                    else
                        bannerMessage = "Professional Preview Services"; // fallback
                }
            }

            return bannerMessage;
        }
        [HttpPost]
        public ActionResult Index(User user)
        {

            if (user.Username == "Insyadmin" && user.Password == "!n&dia@12$")
            {
                Session["Username"] = user.Username;
                Session["Password"] = user.Password;
                return RedirectToAction("Index", "AllLevelQueueBoard");
            }
            else if ((user.Username == "viewreport" && user.Password == "viewreport") || (user.Username == "ActOn05" && user.Password == "@Act#$05&"))


            {
                Session["Username"] = user.Username;
                Session["Password"] = user.Password;
                return RedirectToAction("Index", "AllLevelQueueBoard");
            }
            else if (user.Username == "tatreport" && user.Password == "tatreport")

            {
                Session["Username"] = user.Username;
                Session["Password"] = user.Password;
                return RedirectToAction("Index", "Tatreport");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View();
            }
        }


        public ActionResult Logout()
        {
            try
            {

                string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;

                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately, for example, log it.
                return null;
            }
        }



    }


}

        
   

