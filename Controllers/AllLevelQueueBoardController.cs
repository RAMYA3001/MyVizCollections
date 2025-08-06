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
    public class AllLevelQueueBoardController : Controller
    {
        public ActionResult Index(int? page, string Fdate, string s1, string s2)
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


                int imode = 1; // Default to 1
                if (Username == "view") // Check if Username is "view"
                {
                    imode = 2; // Use imode 2 for "view"
                }
                else if (Username == "ActOn05") // Check if Username is "ActOn05"
                {
                    imode = 3; // Use imode 3 for "ActOn05"
                }
                else if (Username == "admin") // Check if Username is "admin"
                {
                    imode = 4; // Use imode 3 for "ActOn05"
                }

                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand("SP_MyVizcollections_searchkey_test", con))
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
                                 
                                    //WStatusCount = rdr["WStatusCount"] != DBNull.Value ? Convert.ToInt32(rdr["WStatusCount"]) : 0

                                };

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

                        int pageSize = 10; // Adjust the page size as needed
                        int pageNumber = (page ?? 1);
                       
                        ViewBag.Fdate = Fdate;
                        ViewBag.s1 = s1;
                        ViewBag.s2 = s2;

                        return View(projects.ToPagedList(pageNumber, pageSize));
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                throw;
            }
        }

        //public ActionResult Help()
        //{
        //    ViewBag.SISubmittedPath = @"D:\ColourMySpace\MyViz\SISubmitted";
        //    ViewBag.PRIPath = @"D:\ColourMySpace\MyViz\PRI";
        //    ViewBag.PRITestPath = @"D:\ColourMySpace\MyViz\PRITest";
        //    ViewBag.PDFPath = @"D:\ColourMySpace\MyViz\PDF";          
        //    ViewBag.Json= @"D:\ColourMySpace\MyViz\JSON_Files";

        //    return View();
        //}


        public ActionResult Help()
        {
            // MyViz Paths
            ViewBag.MV_SISubmittedPath = @"D:\WEB_APP\MyViz\SISubmitted";
            ViewBag.MV_PRIPath = @"D:\WEB_APP\MyViz\PRI";
            ViewBag.MV_PRITestPath = @"D:\WEB_APP\MyViz\PRITest";
            ViewBag.MV_PDFPath = @"D:\ColoursGalore\CW\User_Data\PDF_Files";
            ViewBag.MV_Json = @"D:\ColoursGalore\CW\User_Data\JSON_Files";

            // CW Paths
            ViewBag.CW_SISubmittedPath = @"D:\ColourMySpace\MyViz\SISubmitted";
            ViewBag.CW_PRIPath = @"D:\ColourMySpace\MyViz\PRI";
            ViewBag.CW_PRITestPath = @"D:\ColourMySpace\MyViz\PRITest";
            ViewBag.CW_PDFPath = @"D:\ColourMySpace\MyViz\PDF";
            ViewBag.CW_Json = @"D:\ColourMySpace\MyViz\JSON_Files";

            // JK Paths
            ViewBag.JK_SISubmittedPath = @"D:\WEB_APP\JkPaints\jkpaints\SISubmitted";
            ViewBag.JK_SISelectedPath = @"D:\WEB_APP\JkPaints\jkpaints\SISelected";
            ViewBag.JK_PRIPath = @"D:\WEB_APP\JkPaints\jkpaints\PRI";
            ViewBag.JK_PRITestPath = @"D:\WEB_APP\JkPaints\jkpaints\PRITest";
            ViewBag.JK_PDFPath = @"D:\WEB_APP\JkPaints\JkpaintsKN\User_Data\PDF_Files";
            ViewBag.JK_Json = @"D:\WEB_APP\JkPaints\JkpaintsKN\User_Data\JSON_Files";

            return View();
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
        public ActionResult Getsourceid(string ProjectID1, string ProjectID2, string Type1, string Type2)
        {
            try
            {

                string constr = "";
                constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
                MySqlConnection cn = new MySqlConnection(constr);

                cn.Open();
                DataSet ds = new DataSet();
                MySqlCommand cmd = new MySqlCommand("SP_GetSourceimage", cn);
                cmd.CommandTimeout = 1600;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@P_ID1", ProjectID1);
                cmd.Parameters.AddWithValue("@P_ID2", ProjectID2);
                cmd.Parameters.AddWithValue("@Stype1", Type1);
                cmd.Parameters.AddWithValue("@Stype2", Type2);

                MySqlDataAdapter ad = new MySqlDataAdapter(cmd);
                ad.Fill(ds);
                string source1 = null;
                string source2 = null;




                string pid1 = ds.Tables[0].Rows[0][0].ToString();
                string pid2 = ds.Tables[1].Rows[0][0].ToString();
                List<Getsourceimage> projects = new List<Getsourceimage>();
                List<string> sources = new List<string>();
                sources.Add(source1);
                sources.Add(source2);
                Getsourceimage project = new Getsourceimage
                {

                    ProjectID1 = pid1,
                    ProjectID2 = pid2,


                };

                projects.Add(project);



                return Json(projects, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        public ActionResult ResolutionCheck(int? page, string Sdate, string Edate, string t1)
        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;

            try
            {
                if (Sdate == null || Edate == null)
                {
                    // Set default values to current date
                    DateTime currentDate = DateTime.Now;
                    Sdate = currentDate.ToString("yyyy-MM-dd");
                    Edate = currentDate.ToString("yyyy-MM-dd");
                }
                string formattedStartDate = Convert.ToDateTime(Sdate).ToString("dd-MM-yyyy");
                string formattedEndDate = Convert.ToDateTime(Edate).ToString("dd-MM-yyyy");

                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand("SP_ResolutionCheckData", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 1600;
                        cmd.Parameters.AddWithValue("@From_Date", Convert.ToDateTime(Sdate));
                        cmd.Parameters.AddWithValue("@To_Date", Convert.ToDateTime(Edate));
                        cmd.Parameters.AddWithValue("@type1", t1);

                        con.Open();

                        // Fetching data for the main query
                        List<ResolutionCheck> projects = new List<ResolutionCheck>();
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                ResolutionCheck project = new ResolutionCheck
                                {
                                    ProjectID = rdr["ProjectID"] != DBNull.Value ? Convert.ToInt32(rdr["ProjectID"]) : 0,
                                    TSO = rdr["TSO"].ToString(),
                                    C_Date = Convert.ToDateTime(rdr["C_Date"]),
                                    FileSize1 = rdr["FileSize1"].ToString(),
                                    FileSize2 = rdr["FileSize2"].ToString(),
                                    FileSize3 = rdr["FileSize3"].ToString(),
                                    Res11 = rdr["Res11"].ToString(),
                                    Res12 = rdr["Res12"].ToString(),
                                    Res11_12 = rdr["Res11_12"].ToString(),
                                    Res21 = rdr["Res21"].ToString(),
                                    Res22 = rdr["Res22"].ToString(),
                                    Res21_22 = rdr["Res21_22"].ToString(),
                                    Res31 = rdr["Res31"].ToString(),
                                    Res32 = rdr["Res32"].ToString(),
                                    Res31_32 = rdr["Res31_32"].ToString(),
                                    SIOption = rdr["SIOption"].ToString(),
                                    Status = rdr["Status"].ToString(),
                                    Remarksforrejections = rdr["Remarksforrejections"].ToString(),
                                };

                                projects.Add(project);
                            }
                        }

                        // Adding data from the additional query
                        string additionalQuery = "select DATE_FORMAT(Date(C_Date),'%d-%m-%Y')As CDate FROM resolutioncheck ORDER BY C_Date DESC LIMIT 0,1";

                        using (MySqlCommand additionalCmd = new MySqlCommand(additionalQuery, con))
                        {
                            using (MySqlDataReader additionalReader = additionalCmd.ExecuteReader())
                            {
                                while (additionalReader.Read())
                                {

                                    ViewBag.Lpcom = Convert.ToString(additionalReader["CDate"]);

                                }



                            }
                        }

                        con.Close();

                        int pageSize = 10; // Adjust the page size as needed
                        int pageNumber = (page ?? 1);
                        ViewBag.Sdate = formattedStartDate;
                        ViewBag.Edate = formattedEndDate;
                        ViewBag.t1 = t1; // Ensure t1 is properly passed to ViewBag

                        return View(projects.ToPagedList(pageNumber, pageSize));
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                throw;
            }
        }



        public ActionResult ResolutionModal(int projectid)

        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
            MySqlConnection con = new MySqlConnection(constr);
            try
            {
                DataSet ds = new DataSet();

                MySqlCommand com = new MySqlCommand("SP_Resolution_ProjDetails", con);
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
                    UserName = Convert.ToString(ds.Tables[0].Rows[0]["UserName"]),
                    DepotName = Convert.ToString(ds.Tables[0].Rows[0]["DepotName"]),
                    SiteAddress = Convert.ToString(ds.Tables[0].Rows[0]["SiteAddress"]),
                    Location = Convert.ToString(ds.Tables[0].Rows[0]["Location"]),
                    CustomerName = Convert.ToString(ds.Tables[0].Rows[0]["CustomerName"]),
                    remarks = Convert.ToString(ds.Tables[0].Rows[0]["Remarks"]),
                    statuscode = Convert.ToString(ds.Tables[0].Rows[0]["wstatus"]),
                    InsyComments = Convert.ToString(ds.Tables[0].Rows[0]["InSyComments"]),
                    Resolution = Convert.ToString(ds.Tables[0].Rows[0]["Resolution"]),


                    PSE = Convert.ToString(ds.Tables[0].Rows[0]["whoistheL6PSE"]),
                    QACPI = Convert.ToString(ds.Tables[0].Rows[0]["WhoistheL7QACPI"]),







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
        public ActionResult DisplayImage(int ProjectID, string linkType, string sourcefile)
        {

            string imagePath = GetImagePathByProjectID(ProjectID, linkType, sourcefile);


            if (System.IO.File.Exists(imagePath))
            {
                // Determine the file extension dynamically based on the image path
                string fileExtension = Path.GetExtension(imagePath);

                // Set the content type based on the file extension
                string contentType = GetContentType(fileExtension);

                // Read the file bytes and return the file
                byte[] imageData = System.IO.File.ReadAllBytes(imagePath);
                return File(imageData, contentType);
            }

            // If the file does not exist, you might want to handle this case differently
            return HttpNotFound();
        }
       
        private string GetImagePathByProjectID(int ProjectID, string linkType, string sourcefile)
        {

            string basePath = string.Empty;
            string filename = string.Empty;
            
           
            string image1 = string.Empty;
            string image2 = string.Empty;
            string image3 = string.Empty;
            string PRI = string.Empty;
            string PRITest = string.Empty;
            string D1 = string.Empty;
            string D2 = string.Empty;
            string D3 = string.Empty;
            //string sifile = string.Empty;



            switch (linkType)
            {
                case "Source":
                    basePath = "D:\\ColourMySpace\\MyViz\\SISubmitted";
                    //filename = $"{ProjectID}.jpeg";
                    filename = sourcefile;
                    break;
                case "img1":
                    basePath = "D:\\ColourMySpace\\MyViz\\SISubmitted";
                    //filename = $"{ProjectID}.jpeg";
                    image1 = sourcefile;
                    break;
                case "img2":
                    basePath = "D:\\ColourMySpace\\MyViz\\SISubmitted";
                    //filename = $"{ProjectID}.jpeg";
                    image2 = sourcefile;
                    break;
                case "img3":
                    basePath = "D:\\ColourMySpace\\MyViz\\SISubmitted";
                    //filename = $"{ProjectID}.jpeg";
                    image3 = sourcefile;
                    break;
                case "PRI":
                    basePath = "D:\\ColourMySpace\\MyViz\\PRI\\" + ProjectID + "\\";
                    PRI = "PRI_" + $"{ProjectID}.jpg";
                    break;

                 case "PRI_Test":
                    basePath = "D:\\ColourMySpace\\MyViz\\PRITest\\" + ProjectID + "\\";
                    PRITest = "PRITest_" + $"{ProjectID}.jpg";
                    break;
                  
                case "PDF1":
                    basePath = "D:\\ColourMySpace\\MyViz\\PDF\\" + ProjectID + "\\";
                    string checkfilename1 = basePath + "1_D" + $"{ProjectID}.pdf";
                    if (System.IO.File.Exists(checkfilename1))
                    {
                        D1 = "1_D" + $"{ProjectID}.pdf";
                    }
                    else
                    {
                        D1 = "1_D" + $"{ProjectID}.jpg";
                    }
                    //D1 = Path.GetExtension(sourcefile)?.ToLower() == ".jpg" ?
                    //    "1_D" + $"{ProjectID}.jpg" : "1_D" + $"{ProjectID}.pdf";
                    break;
                case "PDF2":
                    basePath = "D:\\ColourMySpace\\MyViz\\PDF\\" + ProjectID + "\\";
                    string checkfilename2 = basePath + "2_D" + $"{ProjectID}.pdf";
                    if (System.IO.File.Exists(checkfilename2))
                    {
                        D1 = "2_D" + $"{ProjectID}.pdf";
                    }
                    else
                    {
                        D1 = "2_D" + $"{ProjectID}.jpg";
                    }
                    //D1 = Path.GetExtension(sourcefile)?.ToLower() == ".jpg" ?
                    //    "1_D" + $"{ProjectID}.jpg" : "1_D" + $"{ProjectID}.pdf";
                    break;


                   
                case "PDF3":
                    basePath = "D:\\ColourMySpace\\MyViz\\PDF\\" + ProjectID + "\\";
                    string checkfilename3 = basePath + "3_D" + $"{ProjectID}.pdf";
                    if (System.IO.File.Exists(checkfilename3))
                    {
                        D1 = "3_D" + $"{ProjectID}.pdf";
                    }
                    else
                    {
                        D1 = "3_D" + $"{ProjectID}.jpg";
                    }
                  
                    break;




                default:
                    return null; // Return null for invalid linkType
            }

        

        string imagePath = Path.Combine(basePath, filename,  image1, image2, image3, PRI, PRITest, D1, D2, D3);
            return imagePath;
        }


    private string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".jpg":
                    return "image/jpg";
                case ".jpeg":
                    return "image/jpeg";
                case ".pdf":
                    return "application/pdf";
                // Add more cases for other file types if needed
                default:
                    return "application/octet-stream"; // Default to generic binary content
            }
        }






        //public JsonResult Qualitycheck(string projectID, string result, string comments)
        //{
        //    string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;

        //    try
        //    {
        //        using (MySqlConnection con = new MySqlConnection(constr))
        //        {


        //            // If no feedback exists, save the new feedback
        //            using (MySqlCommand cmd = new MySqlCommand("SP_SCAQA", con))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;

        //                // Add parameters
        //                cmd.Parameters.AddWithValue("@ProjectID_P", projectID);
        //                cmd.Parameters.AddWithValue("@QCResult", result);
        //                cmd.Parameters.AddWithValue("@QCComments", comments);

        //                con.Open();
        //                cmd.ExecuteNonQuery();
        //                con.Close();
        //            }
        //        }

        //        return Json(new { success = true, message = "Quality Check updated successfully!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"Error: {ex.Message}" });
        //    }
        //}



        [HttpPost]
        public JsonResult Qualitycheck(string projectID, string result, string comments)
        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;

            try
            {
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand("SP_SCAQA", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.AddWithValue("@ProjectID_P", projectID);
                        cmd.Parameters.AddWithValue("@QCResult", result);
                        cmd.Parameters.AddWithValue("@QCComments", comments);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }

                return Json(new { success = true, message = "Quality Check updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }


      [HttpGet]
public JsonResult GetSCAQAData(string projectID)
{
    AllLevelQueueBoard data = null;

    // Fetch the connection string for MySQL
    string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
    
    // Create MySQL connection
    MySqlConnection con = new MySqlConnection(constr);

            // Prepare the query
            //string query = "SELECT PSEID, CPEID, PSERemarks, CPERemarks FROM scaqa WHERE ProjectID = @ProjectID";
            string query = @"SELECT fromtso.WhoistheL6PSE, scaqa.CPEID, scaqa.PSERemarks, scaqa.CPERemarks, fromtso.ProjectName 
                         FROM scaqa 
                         JOIN fromtso ON scaqa.ProjectID = fromtso.ProjectID 
                         WHERE scaqa.ProjectID = @ProjectID";
            try
    {
        // Use MySqlCommand for MySQL queries
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            // Add the parameter for ProjectID
            cmd.Parameters.AddWithValue("@ProjectID", projectID);
            
            // Open the connection
            con.Open();

            // Execute the query and read the results
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    // Populate the data model
                    data = new AllLevelQueueBoard
                    {

                        WhoistheL6PSE = reader["WhoistheL6PSE"].ToString(),
                        CPEID = reader["CPEID"].ToString(),
                        PSERemarks = reader["PSERemarks"].ToString(),
                        CPERemarks = reader["CPERemarks"].ToString(),
                        ProjectName = reader["ProjectName"].ToString()
                    };
                }
            }
        }
    }
    catch (Exception ex)
    {
        // Log the exception (Optional, for debugging)
        Console.WriteLine(ex.Message);
        return Json(new { success = false, message = "An error occurred while retrieving data." }, JsonRequestBehavior.AllowGet);
    }
    finally
    {
        // Close the connection (if it's open)
        if (con.State == System.Data.ConnectionState.Open)
        {
            con.Close();
        }
    }

    // Return JSON response
    if (data != null)
    {
        return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
    }
    else
    {
        return Json(new { success = false, message = "No data found." }, JsonRequestBehavior.AllowGet);
    }
}






        public ActionResult Statuslog(int projectid)
        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
            MySqlConnection con = new MySqlConnection(constr);
            try
            {
                DataSet ds = new DataSet();

                MySqlCommand com = new MySqlCommand("SP_Statuslog", con);
                com.CommandTimeout = 1600;
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Mode", 1);
                com.Parameters.AddWithValue("@ProjectID", projectid);

                con.Open();
                MySqlDataAdapter ad = new MySqlDataAdapter(com);

                ad.Fill(ds);

                List<WStatusLog> statusLogs = new List<WStatusLog>();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        WStatusLog model = new WStatusLog()
                        {
                            ProjectID = Convert.ToInt32(row["ProjectID"]),
                            WorkStatus = Convert.ToString(row["WorkStatus"]),
                            WorkStatusDesc = Convert.ToString(row["WorkStatusDesc"]),
                            AssignedTo = Convert.ToString(row["AssignedTo"]),
                            M_date = row["M_date"] != DBNull.Value ? Convert.ToDateTime(row["M_date"]) : (DateTime?)null
                        };

                        // Format M_date before adding to the list
                        if (model.M_date.HasValue)
                        {
                            model.M_dateFormatted = model.M_date.Value.ToString("dd/MM/yyyy hh:mm:ss tt");
                        }

                        statusLogs.Add(model);
                    }
                }


                //// Fetch feedback from scaqa table
                //MySqlCommand feedbackCommand = new MySqlCommand("SELECT Result FROM scaqa WHERE ProjectID = @ProjectID", con);
                //feedbackCommand.Parameters.AddWithValue("@ProjectID", projectid);
                //string feedback = feedbackCommand.ExecuteScalar() as string;

                //ViewBag.Feedback = feedback; // Pass feedback to the view

                return View("Statuslog", statusLogs);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return null;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }







        public ActionResult Myvizinsylog(int projectid)
        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
            MySqlConnection con = new MySqlConnection(constr);

            try
            {
                DataSet ds = new DataSet();

                MySqlCommand com = new MySqlCommand("SP_myvizinsylog_Collections", con);
                com.CommandTimeout = 1600;
                com.CommandType = CommandType.StoredProcedure;

                // Only pass the one parameter that matches the stored procedure
                com.Parameters.AddWithValue("Project_ID", projectid);

                MySqlDataAdapter ad = new MySqlDataAdapter(com);
                ad.Fill(ds);

                List<WStatusLog> MyVizinsyLogs = new List<WStatusLog>();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        WStatusLog model = new WStatusLog()
                        {
                            ProjectID = Convert.ToInt32(row["ProjectID"]),
                            PSECode = Convert.ToString(row["PSECode"]),
                            LogType = Convert.ToString(row["LogType"]),
                            Remarks = Convert.ToString(row["Remarks"]),
                            Count = row["Count"] != DBNull.Value ? Convert.ToInt32(row["Count"]) : 0,
                            M_date = row["C_date"] != DBNull.Value ? Convert.ToDateTime(row["C_date"]) : (DateTime?)null
                        
                        };
                   

                        model.M_dateFormatted = model.M_date?.ToString("dd/MM/yyyy hh:mm:ss tt");

                        MyVizinsyLogs.Add(model);
                    }
                }

                return View("Myvizinsylog", MyVizinsyLogs);
            }
            catch (Exception ex)
             {
                // Optionally log the error
                return Content("Error: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }




        public ActionResult SCAQALog(int projectid)
        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
            MySqlConnection con = new MySqlConnection(constr);

            try
            {
                DataSet ds = new DataSet();

                MySqlCommand com = new MySqlCommand("SP_SCAQA_log", con);
                com.CommandTimeout = 1600;
                com.CommandType = CommandType.StoredProcedure;

                // Match parameter name exactly as in the procedure
                com.Parameters.AddWithValue("Project_ID", projectid);

                MySqlDataAdapter ad = new MySqlDataAdapter(com);
                ad.Fill(ds);

                List<WStatusLog> SCAQALog = new List<WStatusLog>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        WStatusLog model = new WStatusLog()
                        {
                            ProjectID = Convert.ToInt32(row["ProjectID"]),
                            Status = Convert.ToString(row["Status"]),
                            StRemarks = Convert.ToString(row["StRemarks"]),
                            SourcePixel = Convert.ToString(row["SourcePixel"]),
                            SourceFileSize = Convert.ToString(row["SourceFileSize"]),
                            QCComments = Convert.ToString(row["QCComments"]),
                            QCResult = Convert.ToString(row["QCResult"]),
                            PSEID = Convert.ToString(row["WhoistheL6PSE"]),
                            CPEID = Convert.ToString(row["CPEID"]),
                            PRIPixels = Convert.ToString(row["PRIPixels"]),
                            PRIFilesize = Convert.ToString(row["PRIFilesize"]),
                            Notes = Convert.ToString(row["Notes"]),
                            //C_id = Convert.ToString(row["C_id"]),
                            //C_date = row["C_Date"] != DBNull.Value ? Convert.ToDateTime(row["C_Date"]) : (DateTime?)null,
                            PSERemarks = Convert.ToString(row["PSERemarks"]),
                            CPERemarks = Convert.ToString(row["CPERemarks"]),
                            TimeTaken = Convert.ToString(row["TimeTaken"])
                        };


                        SCAQALog.Add(model);
                    }
                }

                return View("SCAQALog", SCAQALog);
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
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







    }



}