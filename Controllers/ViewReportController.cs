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
        public ActionResult Index( string Fdate,  string s1, string s2)
        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;

            try
            {
                if (Fdate == null)
                {
                    Fdate = DateTime.Now.ToString("yyyy-MM-dd");
                }
            
                // Retrieve category from session
                string Username = Session["Username"]?.ToString();


                int imode = 2; // Default to 1
             

                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand("SP_MyVizcollections_searchkey_Index", con))
                    {
                        cmd.CommandTimeout = 1600;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@From_Date", Fdate);
                       
                        cmd.Parameters.AddWithValue("@S_ID", s1);
                        cmd.Parameters.AddWithValue("@type1", s2);
                        cmd.Parameters.AddWithValue("@imode", imode); // Pass imode value

                        con.Open();

                        List<AllLevelQueueBoard> projects = new List<AllLevelQueueBoard>();

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {

                            // ✅ Build a list of returned column names to check existence
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
                                  

                                    //WStatusCount = rdr["WStatusCount"] != DBNull.Value ? Convert.ToInt32(rdr["WStatusCount"]) : 0

                                };


                                // ✅ Only add TAT columns if they exist in the result
                                if (columnNames.Contains("ActualTAT"))
                                    project.ActualTAT = rdr["ActualTAT"] != DBNull.Value ? Convert.ToDecimal(rdr["ActualTAT"]) : 0;

                                if (columnNames.Contains("EstimateTAT"))
                                    project.EstimateTAT = rdr["EstimateTAT"] != DBNull.Value ? Convert.ToDecimal(rdr["EstimateTAT"]) : 0;

                                if (columnNames.Contains("ExceededTAT"))
                                    project.ExceededTAT = rdr["ExceededTAT"] != DBNull.Value ? Convert.ToDecimal(rdr["ExceededTAT"]) : 0;
                                // After adding project to the list
                                using (MySqlConnection countCon = new MySqlConnection(constr))
                                {
                                    countCon.Open();
                                    using (MySqlCommand countCmd = new MySqlCommand("SELECT COUNT(*) FROM wstatuslog WHERE ProjectID = @pid AND Workstatus NOT IN (85, 86)", countCon))
                                    {
                                        countCmd.Parameters.AddWithValue("@pid", project.ProjectID);
                                        int count = Convert.ToInt32(countCmd.ExecuteScalar());
                                        project.WStsCount = count; // Assuming you have a property in your model for this
                                    }
                                }


                                projects.Add(project);
                            }
                        }

                        con.Close();

                       //int pageSize = 10; // Adjust the page size as needed
                       //int pageNumber = (page ?? 1);

                        ViewBag.Fdate = Fdate;
                       
                        ViewBag.s1 = s1;
                        ViewBag.s2 = s2;
                        ViewBag.RowCount = projects.Count; // ✅ Total rows displayed

                        //return View(projects.ToPagedList(pageNumber, pageSize));
                        return View(projects);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                throw;
            }
        }

    }
}