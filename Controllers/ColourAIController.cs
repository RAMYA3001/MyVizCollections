using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using MyVizCollections.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing.Printing;
using System.Threading;
using System.Web.Mvc;

namespace MyVizCollections.Controllers
{
    public class ColourAIController : Controller
    {
        public ActionResult Index(int? page, string sortOrder, string imode, string q, string s, string startDate, string endDate, int? imodval)
        {
            string constr = ConfigurationManager.ConnectionStrings["ColourAI"].ConnectionString;

            try
            {

                ViewBag.searchQuery = string.IsNullOrEmpty(q) ? "" : q;
                ViewBag.searchQuery1 = string.IsNullOrEmpty(s) ? "" : s;
                ViewBag.searchQuery3 = string.IsNullOrEmpty(startDate) ? "" : startDate;
                ViewBag.searchQuery4 = string.IsNullOrEmpty(endDate) ? "" : endDate;


                //ViewBag.CurrentSort = sortOrder;

                if (string.IsNullOrEmpty(q)) q = null;
                if (string.IsNullOrEmpty(s)) s = null;
                if (string.IsNullOrEmpty(startDate)) startDate = null;
                if (string.IsNullOrEmpty(endDate)) endDate = null;

                DataSet ds = new DataSet();
                MySqlCommand com = new MySqlCommand();


                if (startDate == null)
                {
                    DateTime newDate = DateTime.Today.AddDays(-8);
                    DateTime newDate1 = DateTime.Today.AddDays(-1);
                    startDate = newDate.ToString();
                    endDate = newDate1.ToString();
                }


                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand("SP_ColourAI_test", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;


                        cmd.Parameters.AddWithValue("@CD", Convert.ToDateTime(startDate));
                        cmd.Parameters.AddWithValue("@ED", Convert.ToDateTime(endDate));
                        cmd.Parameters.AddWithValue("@P_ID", q);
                        cmd.Parameters.AddWithValue("@S", s);
                        cmd.Parameters.AddWithValue("@IMode", imodval);
                        //cmd.Parameters.AddWithValue("@IMode=2", s);
                        //cmd.Parameters.AddWithValue("@IMode=3", startDate);


                        //int mode;
                        //if (int.TryParse(imode, out mode))
                        //{
                        //    // Conversion successful, now you can compare mode with integers
                        //    if (mode == 1)
                        //    {
                        //        cmd.Parameters.AddWithValue("@IMode", q);
                        //    }
                        //    else if (mode == 2)
                        //    {
                        //        cmd.Parameters.AddWithValue("@IMode", s);
                        //    }
                        //    else if (mode == 3)
                        //    {
                        //        cmd.Parameters.AddWithValue("@IMode", 3);
                        //    }
                        //}
                        //else
                        //{
                        //    // Handle the case where imode is not a valid integer
                        //    // You may want to set a default value or show an error message
                        //}
                        con.Open();

                        List<ColourAI> projects = new List<ColourAI>();

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ColourAI project = new ColourAI
                                {
                                    ProjectID = rdr["ProjecID"].ToString(),
                                    Option = rdr["Option"].ToString(),
                                    ShadeGroup = rdr["ShadeGroup"].ToString(),
                                    ShadeCode = rdr["ShadeCode"].ToString(),
                                    ShadeName = rdr["ShadeName"].ToString(),
                                    Colour = rdr["Colour"].ToString(),
                                    StatusDt = rdr["StDate"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(rdr["StDate"])
                                };

                                projects.Add(project);
                            }
                        }

                        con.Close();

                        int pageSize = 20; // Adjust the page size as needed
                        int pageNumber = (page ?? 1);
                        ViewBag.ShadeCode = s;
                        ViewBag.ProjectId = q;
                        ViewBag.Sdate = startDate;

                        return View(projects.ToPagedList(pageNumber, pageSize));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                throw;
            }
        }

        public ActionResult myModal(int projectid)

        {
            string constr = ConfigurationManager.ConnectionStrings["ColourAI"].ConnectionString;
            MySqlConnection con = new MySqlConnection(constr);
            try
            {
                DataSet ds = new DataSet();

                MySqlCommand com = new MySqlCommand("SP_ColourAI_ProjectDetails", con);
                com.CommandTimeout = 1600;
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Mode", 1);
                com.Parameters.AddWithValue("@ProjectID", projectid);
                con.Close();

                con.Open();
                com.ExecuteNonQuery();
                MySqlDataAdapter ad = new MySqlDataAdapter(com);
                ad.Fill(ds);
                var colorchoices = ds.Tables[0].AsEnumerable();

                ColourAI model = new ColourAI()
                {
                    ProjectID = Convert.ToString(ds.Tables[0].Rows[0]["ProjectID"]),
                    ProjectName = Convert.ToString(ds.Tables[0].Rows[0]["ProjectName"]),
                    TSOID = Convert.ToString(ds.Tables[0].Rows[0]["TSO ID"]),
                    TSOName = Convert.ToString(ds.Tables[0].Rows[0]["TSO Name"]),
                    Feedback = Convert.ToString(ds.Tables[0].Rows[0]["Feedback"])

                };

                con.Close();
                return View(model);

            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                //ProjectDetails.ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }
        public ActionResult PopularShades(int? page, string sortOrder, string startDate, string endDate)
        {
            string constr = ConfigurationManager.ConnectionStrings["ColourAI"].ConnectionString;

            try
            {

                ViewBag.searchQuery3 = string.IsNullOrEmpty(startDate) ? "" : startDate;
                ViewBag.searchQuery4 = string.IsNullOrEmpty(endDate) ? "" : endDate;


                //ViewBag.CurrentSort = sortOrder;


                if (string.IsNullOrEmpty(startDate)) startDate = null;
                if (string.IsNullOrEmpty(endDate)) endDate = null;

                DataSet ds = new DataSet();
                MySqlCommand com = new MySqlCommand();


                if (startDate == null)
                {
                    DateTime newDate = DateTime.Today.AddDays(-8);
                    DateTime newDate1 = DateTime.Today.AddDays(-1);
                    startDate = newDate.ToString();
                    endDate = newDate1.ToString();
                }



                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand("SP_ColourAI_PopularShades", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;


                        cmd.Parameters.AddWithValue("@CD", Convert.ToDateTime(startDate));
                        cmd.Parameters.AddWithValue("@ED", Convert.ToDateTime(endDate));


                        con.Open();

                        List<ColourAI> projects = new List<ColourAI>();

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ColourAI project = new ColourAI
                                {


                                    ShadeGroup = rdr["ShadeGroup"].ToString(),
                                    ShadeCode = rdr["ShadeCode"].ToString(),
                                    ShadeName = rdr["ShadeName"].ToString(),
                                    Colour = rdr["Colour"].ToString(),
                                    Count = Convert.ToInt32(rdr["Count"])
                                };

                                projects.Add(project);
                            }
                        }

                        con.Close();

                        int pageSize = 20; // Adjust the page size as needed
                        int pageNumber = (page ?? 1);

                        ViewBag.Sdate = startDate;
                        return View(projects.ToPagedList(pageNumber, pageSize));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                throw;
            }
        }
    }
}