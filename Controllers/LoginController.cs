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
            // Hardcoded user for demonstration purposes
            if (user.Username == "Insyadmin" && user.Password == "admin@2#4")
            {
                // Authentication successful
                return RedirectToAction("Index", "AllLevelQueueBoard");
            }
            else
            {
                // Authentication failed
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

