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
            return View();
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
            else if ((user.Username == "view" && user.Password == "view@2#4") ||  (user.Username == "ActOn05" && user.Password == "@Act#$05&"))

            {
                Session["Username"] = user.Username;
                Session["Password"] = user.Password;
                return RedirectToAction("Index", "AllLevelQueueBoard");
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

