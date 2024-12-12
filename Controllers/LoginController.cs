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

        //public ActionResult Index(User user)
        //{
        //    if (user.Username == "Insyadmin" && user.Password == "admin@2#4")
        //    {
        //        Session["Username"] = user.Username;
        //        Session["Password"] = user.Password;
        //        return RedirectToAction("Index", "AllLevelQueueBoard");
        //    }
        //    else if (user.Username == "Insy1" && user.Password == "insy@1$2")
        //    {
        //        Session["Username"] = user.Username;
        //        Session["Password"] = user.Password;
        //        return RedirectToAction("Index", "AllLevelQueueBoard");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Invalid login attempt");
        //        return View();
        //    }
        //}

        //public ActionResult Index(User user)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;

        //        using (MySqlConnection con = new MySqlConnection(constr))
        //        {
        //            string query = "SELECT Category, Status FROM users WHERE Username = @Username AND Password = @Password";
        //            MySqlCommand cmd = new MySqlCommand(query, con);
        //            cmd.Parameters.AddWithValue("@Username", user.Username);
        //            cmd.Parameters.AddWithValue("@Password", user.Password); // Ensure password is hashed in production
        //            con.Open();

        //            using (MySqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    string category = reader["Category"].ToString();
        //                    string status = reader["Status"].ToString();

        //                    if (status == "Active")
        //                    {
        //                        Session["Username"] = user.Username;
        //                        Session["Category"] = category;

        //                        if (category == "Admin")
        //                        {
        //                            return RedirectToAction("Index", "AllLevelQueueBoard");
        //                        }
        //                        else if (category == "Staff")
        //                        {
        //                            return RedirectToAction("Index", "AllLevelQueueBoard");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ModelState.AddModelError(string.Empty, "User account is inactive.");
        //                    }
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
        //                }
        //            }
        //        }
        //    }

        //    return View();
        //}
       
        public ActionResult Index(User user)
        {
            if (ModelState.IsValid)
            {
                string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;

                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    string query = "SELECT Category, Status FROM users WHERE Username = @Username AND Password = @Password";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@Password", user.Password); // Ensure password is hashed in production
                    con.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string category = reader["Category"].ToString();
                            string status = reader["Status"].ToString();

                            if (status == "Active")
                            {
                                Session["Username"] = user.Username;
                                Session["Category"] = category;

                                // Redirect based on category
                                if (category == "Admin")
                                {
                                    return RedirectToAction("Index", "AllLevelQueueBoard");
                                }
                                else if (category == "Staff")
                                {
                                    return RedirectToAction("Index", "AllLevelQueueBoard");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "User account is inactive.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid username or password.");
                        }
                    }
                }
            }

            return View();
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

