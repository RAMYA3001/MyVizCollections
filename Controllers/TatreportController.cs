using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Mvc;
using MyVizCollections.Models;
using Google.Protobuf.Collections;
using System.Collections;
using System.Drawing.Printing;
using System.Web.UI;
using PagedList.Mvc;
using System.IO;
using System.Web.UI.WebControls;
using System.Web;
using System.Linq;
using PagedList;
using System.Globalization;
using System.Diagnostics;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System.Web.DynamicData;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data.SqlClient;


namespace MyVizCollections.Controllers
{
    public class TatreportController : Controller
    {
        // GET: Tat
       
            public ActionResult Index(int? page, string Fdate, string s1, string s2)
            {
                string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
                List<AllLevelQueueBoard> projects = new List<AllLevelQueueBoard>(); // declare here

                try
                {
                    if (Fdate == null)
                    {
                        Fdate = DateTime.Now.ToString("yyyy-MM-dd");
                    }

                    string Username = Session["Username"]?.ToString();

                    int imode = 4;
                    //if (Username == "view")
                    //    imode = 2;
                    //else if (Username == "ActOn05")
                    //    imode = 3;
                    //else if (Username == "Admin")
                    //    imode = 4;

                    using (MySqlConnection con = new MySqlConnection(constr))
                    {
                        using (MySqlCommand cmd = new MySqlCommand("SP_MyVizcollections_searchkey", con))
                        {
                            cmd.CommandTimeout = 1600;
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@From_Date", Fdate);
                            cmd.Parameters.AddWithValue("@S_ID", s1);
                            cmd.Parameters.AddWithValue("@type1", s2);
                            cmd.Parameters.AddWithValue("@imode", imode);

                            con.Open();

                            using (MySqlDataReader rdr = cmd.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    AllLevelQueueBoard project = new AllLevelQueueBoard
                                    {
                                        ProjectID = rdr["ProjectID"] != DBNull.Value ? Convert.ToInt32(rdr["ProjectID"]) : 0,
                                        ProjectName = rdr["ProjectName"].ToString(),
                                        RegdOn = rdr["RegdOn"] != DBNull.Value ? Convert.ToDateTime(rdr["RegdOn"]) : (DateTime?)null,
                                        RegdBy = rdr["RegdBy"].ToString(),
                                        FinalStatus = rdr["FinalStatus"].ToString(),
                                        FinalStatusDt = rdr["FinalStatusDt"] != DBNull.Value ? Convert.ToDateTime(rdr["FinalStatusDt"]) : (DateTime?)null,
                                        Type = rdr["Type"].ToString(),
                                        Category = rdr["Category"].ToString(),
                                        SelectedImageLink = rdr["SelectedImageLink"].ToString(),
                                        PRILink = rdr["PRILink"].ToString(),
                                        PRITest = rdr["PRITest"].ToString(),
                                        SCA1 = rdr["SCA1 Link"].ToString(),
                                        SCA2 = rdr["SCA2 Link"].ToString(),
                                        SCA3 = rdr["SCA3 Link"].ToString(),
                                        SIOption = rdr["SIOption"].ToString(),
                                        ImageFileName1 = rdr["ImageFileName1"].ToString(),
                                        ImageFileName2 = rdr["ImageFileName2"].ToString(),
                                        ImageFileName3 = rdr["ImageFileName3"].ToString(),
                                        Site = rdr["Site"].ToString(),
                                        Customer = rdr["Customer"].ToString(),
                                        FeedBack = rdr["FeedBack"].ToString(),
                                        CC = rdr["CC"].ToString(),
                                        DuplicateOf = rdr["DuplicateOf"].ToString(),
                                        Priority = rdr["Priority"].ToString(),
                                        NexGenDealer = rdr["NexGenDealer"].ToString(),
                                        EmailStatus = rdr["EmailStatus"].ToString(),
                                        EMailValues = rdr["EMailValues"].ToString(),
                                        PSEName = rdr["PSE Name"].ToString(),
                                        CPEName = rdr["CPE Name"].ToString()
                                    };

                                    // Get WStsCount
                                    using (MySqlConnection countCon = new MySqlConnection(constr))
                                    {
                                        countCon.Open();
                                        using (MySqlCommand countCmd = new MySqlCommand("SELECT COUNT(*) FROM wstatuslog WHERE ProjectID = @pid AND Workstatus NOT IN (85, 86)", countCon))
                                        {
                                            countCmd.Parameters.AddWithValue("@pid", project.ProjectID);
                                            project.WStsCount = Convert.ToInt32(countCmd.ExecuteScalar());
                                        }
                                    }

                                    projects.Add(project);
                                }
                            }
                        }
                    }

                    // Calculate Remaining TAT Hours for each project
                    foreach (var proj in projects)
                    {
                        if (proj.RegdOn.HasValue)
                        {
                            proj.DeliveryOn = proj.RegdOn.Value.AddHours(24);
                            DateTime refTime = (proj.FinalStatus == "60" && proj.FinalStatusDt.HasValue)
                                ? proj.FinalStatusDt.Value
                                : DateTime.Now;

                            proj.RemainingTATHours = (proj.DeliveryOn - refTime).TotalHours;
                        }
                        else
                        {
                            proj.RemainingTATHours = double.MaxValue;
                        }
                    }

                    // Sort by RemainingTATHours if Admin
                    if (Username == "tatreport")
                    {
                        projects = projects.OrderBy(p => p.RemainingTATHours).ToList();
                    }

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    ViewBag.Fdate = Fdate;
                    ViewBag.s1 = s1;
                    ViewBag.s2 = s2;

                    return View(projects.ToPagedList(pageNumber, pageSize));
                }
                catch (Exception ex)
                {
                    // Log or handle error
                    throw;
                }
            }
        }
    }
