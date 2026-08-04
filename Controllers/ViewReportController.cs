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
    public class ViewReportController : Controller
    {
        // GET: ViewReport
        public ActionResult Index(string Fdate, string Ldate, string s1, string s2)
        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;

            try
            {
                if (string.IsNullOrEmpty(Fdate))
                {
                    Fdate = DateTime.Today.ToString("yyyy-MM-dd");
                }

                if (string.IsNullOrEmpty(Ldate))
                {
                    Ldate = DateTime.Today.ToString("yyyy-MM-dd");
                }

                // Retrieve category from session
                string Username = Session["Username"]?.ToString();


                int imode = 1; // Default to 1
                List<AllLevelQueueBoard> projects = new List<AllLevelQueueBoard>();

                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand("SP_MyVizcollections_viewreport", con))
                    {
                        cmd.CommandTimeout = 1600; // ✅ reduced
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@From_Date", Fdate);
                        cmd.Parameters.AddWithValue("@To_Date", Ldate);
                        cmd.Parameters.AddWithValue("@S_ID", s1);
                        cmd.Parameters.AddWithValue("@type1", s2);
                        cmd.Parameters.AddWithValue("@imode", imode);

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            List<string> columnNames = new List<string>();
                            for (int i = 0; i < rdr.FieldCount; i++)
                                columnNames.Add(rdr.GetName(i));

                            while (rdr.Read())
                            {
                                AllLevelQueueBoard project = new AllLevelQueueBoard
                                {

                                
                                    ProjectID = rdr["ProjectID"] != DBNull.Value ? Convert.ToInt32(rdr["ProjectID"]) : 0,
                                    ProjectName = rdr["ProjectName"].ToString(),
                                    RegdOn = Convert.ToDateTime(rdr["RegdOn"]),
                                    RegdBy = rdr["RegdBy"].ToString(),
                                    FinalStatus = rdr["FinalStatus"].ToString(),
                                    FinalStatusDt = Convert.ToDateTime(rdr["FinalStatusDt"]),
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

                                if (columnNames.Contains("ActualTAT"))
                                    project.ActualTAT = rdr["ActualTAT"] != DBNull.Value ? Convert.ToDecimal(rdr["ActualTAT"]) : 0;

                                if (columnNames.Contains("EstimateTAT"))
                                    project.EstimateTAT = rdr["EstimateTAT"] != DBNull.Value ? Convert.ToDecimal(rdr["EstimateTAT"]) : 0;

                                if (columnNames.Contains("ExceededTAT"))
                                    project.ExceededTAT = rdr["ExceededTAT"] != DBNull.Value ? Convert.ToDecimal(rdr["ExceededTAT"]) : 0;

                                projects.Add(project);
                            }
                        }
                    }

                    // ✅ ONE QUERY FOR ALL COUNTS (FIX)
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
                }

                ViewBag.Fdate = Fdate;
                ViewBag.Ldate = Ldate;
                ViewBag.s1 = s1;
                ViewBag.s2 = s2;
                ViewBag.RowCount = projects.Count;

                return View(projects);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return View("Error");
            }
        
    }

        public ActionResult LaunchPreview(int Id)
        {
            string userid = Convert.ToString(Session["UserID"]);
            //string NexGenDealer = Convert.ToString(Session["LoginUser"]);
            string Shades = "";

            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                DataSet ds = new DataSet();
                MySqlCommand com = new MySqlCommand("Sp_InsertWstatusLog", con);
                com.CommandType = CommandType.StoredProcedure;
                com.CommandTimeout = 1600;
                com.Parameters.AddWithValue("@ProjectID", Id);
                com.Parameters.AddWithValue("@TSOID", userid);
                con.Open();
                //com.ExecuteNonQuery();
                MySqlDataAdapter ad = new MySqlDataAdapter(com);
                ad.Fill(ds);

                // String UrlToRedirect = String.Format(String.Concat("https://colourmyspace.co.in", "/MyVizKN/CROSOLanding.aspx?caseId={0}&previewCenter={1}&source=0"), "P521F12S3833", "PVC05");
                String PreviewCentreCode = String.Empty;
                String CaseIDfromKN = String.Empty;
                String UrlToRedirect = "https://colourmyspace.co.in/MyViz/Login/Index";
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {


                    if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    {
                        string DLR = ds.Tables[1].Rows[0]["DLR"].ToString();
                        if (DLR == "Nexgen Dealer")
                        {
                            Shades = "KN Shades;KN Combos;KN Combinations;NG Shades";
                        }
                        else
                        {
                            Shades = "KN Shades;KN Combos;KN Combinations";
                        }
                    }
                    else
                    {
                        Shades = "KN Shades;KN Combos;KN Combinations";
                    }
                    //}

                    PreviewCentreCode = !String.IsNullOrEmpty(ds.Tables[0].Rows[0]["PreviewCentreCode"].ToString()) ? (Convert.ToString(ds.Tables[0].Rows[0]["PreviewCentreCode"])) : "NA";
                    CaseIDfromKN = !String.IsNullOrEmpty(ds.Tables[0].Rows[0]["CaseIDfromKN"].ToString()) ? (Convert.ToString(ds.Tables[0].Rows[0]["CaseIDfromKN"])) : "NA";
                    string returnURL = ("https://colourmyspace.co.in/MyViz/TSODashboard?UserID=" + userid);
                    UrlToRedirect = String.Format(String.Concat("https://colourmyspace.co.in", "/MyVizKN/CROSOLanding.aspx?caseId={0}&previewCenter={1}&returnURL={2}&source=0&PL={3}"), CaseIDfromKN, PreviewCentreCode, returnURL, Shades);
                    //UrlToRedirect = String.Format(String.Concat("https://colourmyspace.co.in", "/MyVizKN/CROSOLanding.aspx?caseId={0}&previewCenter={1}&source=0"), CaseIDfromKN, PreviewCentreCode, returnURL);
                }
                //else
                //{
                //    UrlToRedirect = String.Format(String.Concat("https://colourmyspace.co.in", "/MyVizKN/CROSOLanding.aspx?caseId={0}&previewCenter={1}&source=0"), "P521F12S3833", "PVC05");

                //}
                return Redirect(UrlToRedirect);
            }
        }


        public JsonResult GetPSEList()
        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
            List<string> pse = new List<string>();

            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string q = @"SELECT DISTINCT PSECode
                     FROM lkuppsename
                     WHERE PSELevel='6'
                     AND PSEAvailability='Yes'
                     ORDER BY PSECode";

                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    con.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            pse.Add(rdr["PSECode"].ToString());
                        }
                    }
                }
            }

            return Json(pse, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCPEList()
        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
            List<string> pse = new List<string>();

            using (MySqlConnection con = new MySqlConnection(constr))
            {
                string q = @"SELECT DISTINCT PSECode
                     FROM lkuppsename
                     WHERE PSELevel='7'
                     AND PSEAvailability='Yes'
                     ORDER BY PSECode";

                using (MySqlCommand cmd = new MySqlCommand(q, con))
                {
                    con.Open();
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            pse.Add(rdr["PSECode"].ToString());
                        }
                    }
                }
            }

            return Json(pse, JsonRequestBehavior.AllowGet);
        }
    }
}



   
