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
       
            public ActionResult Index(string Fdate, string s1, string s2)
            {
                string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
                List<AllLevelQueueBoard> projects = new List<AllLevelQueueBoard>(); // declare here
           
            Session["ZECount"] = TatreportController.GetImgLCount("00");
            Session["TECount"] = TatreportController.GetImgLCount("10");
            Session["TWCount"] = TatreportController.GetImgLCount("20");
            Session["FFCount"] = TatreportController.GetImgLCount("45");
            try
                {
                    if (Fdate == null)
                    {
                        Fdate = DateTime.Now.ToString("yyyy-MM-dd");
                    }

                    string Username = Session["Username"]?.ToString();

                    int imode = 1;
               
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    con.Open();

                    // 🔹 MAIN DATA QUERY
                    using (MySqlCommand cmd = new MySqlCommand("SP_MyVizcollections_tatreport", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120; // ✅ reduced

                        cmd.Parameters.AddWithValue("@From_Date", Fdate);
                        cmd.Parameters.AddWithValue("@S_ID", s1);
                        cmd.Parameters.AddWithValue("@type1", s2);
                        cmd.Parameters.AddWithValue("@imode", imode);

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
                                        CPEName = rdr["CPE Name"].ToString(),
                                    WStsCount = 0 // temp
                                };

                                projects.Add(project);
                            }
                        }
                    }

                    // 🔹 SINGLE QUERY FOR ALL COUNTS (FIXED)
                    var ids = projects.Select(p => p.ProjectID).ToList();

                    if (ids.Count > 0)
                    {
                        string idList = string.Join(",", ids);

                        using (MySqlCommand countCmd = new MySqlCommand(
                            $"SELECT ProjectID, COUNT(*) AS Cnt FROM wstatuslog WHERE ProjectID IN ({idList}) AND Workstatus NOT IN (85,86) GROUP BY ProjectID",
                            con))
                        {
                            using (var reader = countCmd.ExecuteReader())
                            {
                                Dictionary<int, int> dict = new Dictionary<int, int>();

                                while (reader.Read())
                                {
                                    dict[Convert.ToInt32(reader["ProjectID"])] =
                                        Convert.ToInt32(reader["Cnt"]);
                                }

                                foreach (var p in projects)
                                {
                                    if (dict.ContainsKey(p.ProjectID))
                                        p.WStsCount = dict[p.ProjectID];
                                }
                            }
                        }
                    }
                    con.Close();
                }

                // 🔹 SORTING
                if (Username == "tatreport")
                {
                    projects = projects
                        .OrderBy(p => p.FinalStatus)
                        .ThenBy(p => p.RemainingTATHours)
                        .ToList();
                }

                ViewBag.Fdate = Fdate;
                ViewBag.s1 = s1;
                ViewBag.s2 = s2;

                return View(projects);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        }


        public ActionResult myModal(int projectid)

        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
            MySqlConnection con = new MySqlConnection(constr);
            try
            {
                DataSet ds = new DataSet();

                MySqlCommand com = new MySqlCommand("SP_ProjectQueueBoard_Details", con);
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

                ProjectDetails model = new ProjectDetails()
                {
                    ProjectID = Convert.ToInt32(ds.Tables[0].Rows[0]["ProjectID"]),
                    ProjectName = Convert.ToString(ds.Tables[0].Rows[0]["ProjectName"]),
                    UserID = Convert.ToString(ds.Tables[0].Rows[0]["UserID"]),

                    InsyComments = Convert.ToString(ds.Tables[0].Rows[0]["InSyComments"]),
                    Resolution = Convert.ToString(ds.Tables[0].Rows[0]["Resolution"]),
                    Size = Convert.ToString(ds.Tables[0].Rows[0]["FileSize"]),
                    caseID = Convert.ToString(ds.Tables[0].Rows[0]["CaseID"]),
                    Options = Convert.ToString(ds.Tables[0].Rows[0]["IorEorMS"]),
                    remarks = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]),
                    statuscode = Convert.ToString(ds.Tables[0].Rows[0]["wstatus"]),
                    Priority = Convert.ToString(ds.Tables[0].Rows[0]["Priority"]) == "Y" ? "Yes" : "No",
                    PSE = Convert.ToString(ds.Tables[0].Rows[0]["whoistheL6PSE"]),
                    QACPI = Convert.ToString(ds.Tables[0].Rows[0]["WhoistheL7QACPI"]),
                    CPBody1 = Convert.ToString(ds.Tables[0].Rows[0]["CPBody1"]),
                    CPBody2 = Convert.ToString(ds.Tables[0].Rows[0]["CPBody2"]),
                    CPBody3 = Convert.ToString(ds.Tables[0].Rows[0]["CPBody3"]),
                    CPBorder1 = Convert.ToString(ds.Tables[0].Rows[0]["CPBorder1"]),
                    CPBorder2 = Convert.ToString(ds.Tables[0].Rows[0]["CPBorder2"]),
                    CPBorder3 = Convert.ToString(ds.Tables[0].Rows[0]["CPBorder3"]),
                    CPHighlight1 = Convert.ToString(ds.Tables[0].Rows[0]["CPHighlight1"]),
                    CPHighlight2 = Convert.ToString(ds.Tables[0].Rows[0]["CPHighlight2"]),
                    CPHighlight3 = Convert.ToString(ds.Tables[0].Rows[0]["CPHighlight3"]),
                    CPSplRequest = Convert.ToString(ds.Tables[0].Rows[0]["CPSplRequest"])

                };

                if (ds.Tables[1].Rows.Count > 0)
                {
                    if (Convert.ToString(ds.Tables[1].Rows[0]["psecode"]) != string.Empty || Convert.ToString(ds.Tables[1].Rows[0]["psecode"]) != null)
                        ViewBag.QA = Convert.ToString(ds.Tables[1].Rows[0]["psecode"]);
                }

                else
                {
                    ViewBag.QA = "NA";
                }
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


        public static string GetImgLCount(string SType)
        {
            string query;
            List<AllLevelQueueBoard> items = new List<AllLevelQueueBoard>();
            AllLevelQueueBoard tSO = new AllLevelQueueBoard();
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
            MySqlConnection con = new MySqlConnection(constr);
            DateTime NextDate = DateTime.Now.AddDays(1);
            DataSet ds = new DataSet();
            if (SType == "00")
                query = "select * From fromtso where wstatus='00'";
            else if (SType == "10")
                query = "select * From fromtso where wstatus='10'";
            else if (SType == "20")
                query = "select * From fromtso where wstatus='20'";
            else if (SType == "45")
                query = "select * From fromtso where wstatus='45'";
            else
                query = "select * From fromtso where wstatus='50'";

            con.Open();
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
            sda.Fill(ds);

            //tSO.BannerMessage = ds.Tables[0].Rows.Count.ToString();

            //if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //{
            //    tSO.BannerMessage = ds.Tables[0].Rows[0]["BannerMessage"].ToString();
            //}
            //else
            //{
            //    tSO.BannerMessage = "";
            //}

            con.Close();
            // ✅ Return count as string
            return ds.Tables[0].Rows.Count.ToString();
        }
    }
}
