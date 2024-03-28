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
using PagedList;
using System.Globalization;

namespace MyVizCollections.Controllers
{
    public class AllLevelQueueBoardController : Controller
    {
        public ActionResult Index(int? page, string Fdate, string s1,string s2)
        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;

            try
            {

                if (Fdate == null)
                {
                    Fdate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand("SP_searchkeydropdown", con))
                    {
                        cmd.CommandTimeout = 1600;
                       
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@From_date", Fdate);
                        cmd.Parameters.AddWithValue("@S_ID", s1);
                        cmd.Parameters.AddWithValue("@type1", s2);
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
                                    SCA1 = rdr["SCA1 Link"].ToString(),
                                    SCA2 = rdr["SCA2 Link"].ToString(),
                                    SCA3 = rdr["SCA3 Link"].ToString(),
                                    SIOption = rdr["SIOption"].ToString(),
                                    ImageFileName1 = rdr["ImageFileName1"].ToString(),
                                    ImageFileName2 = rdr["ImageFileName2"].ToString(),
                                    ImageFileName3 = rdr["ImageFileName3"].ToString(),
                                    Site = rdr["Site"].ToString(),
                                    Customer = rdr["Customer"].ToString(),
                                    FeedBack= rdr["FeedBack"].ToString(),
                                    CC= rdr["CC"].ToString(),
                                    DuplicateOf=rdr["DuplicateOf"].ToString(),
                                    Priority = rdr["Priority"].ToString(),
                                    NexGenDealer = rdr["NexGenDealer"].ToString(),

                                };

                                projects.Add(project);

                            }
                        }
                        con.Close();

                        int pageSize = 10; // Adjust the page size as needed
                        int pageNumber = (page ?? 1);
                        //ViewBag.Fdate = DateTime.Now.ToString("yyyy-MM-dd");
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
            string image1= string.Empty;
            string image2= string.Empty;
            string image3= string.Empty;
            string PRI = string.Empty;
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
                    basePath = "D:\\ColourMySpace\\MyViz\\PRI\\" + ProjectID +"\\";
                    PRI = "PRI_" + $"{ProjectID}.jpg";
                    break;
                case "PDF1":
                    basePath = "D:\\ColourMySpace\\MyViz\\PDF\\" + ProjectID + "\\";
                    D1 = "1_D" + $"{ProjectID}.pdf";
                    break;
                case "PDF2":
                    basePath = "D:\\ColourMySpace\\MyViz\\PDF\\" + ProjectID + "\\";
                    D2 = "2_D" + $"{ProjectID}.pdf";
                    break;
                case "PDF3":
                    basePath = "D:\\ColourMySpace\\MyViz\\PDF\\" + ProjectID + "\\";
                    D3 = "3_D" + $"{ProjectID}.pdf";
                    break;

            }

            string imagePath = Path.Combine(basePath, filename, image1, image2, image3, PRI, D1, D2, D3);
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

        public ActionResult LaunchPreview(int Id)
        {
            string userid = Convert.ToString(Session["UserID"]);
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;

            using (MySqlConnection con = new MySqlConnection(constr))
            {
                DataSet ds = new DataSet();
                MySqlCommand com = new MySqlCommand("Sp_InsertWstatusLog", con);
                com.CommandTimeout = 1600;
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@ProjectID", Id);
                com.Parameters.AddWithValue("@TSOID", userid);

                con.Open();
                MySqlDataAdapter ad = new MySqlDataAdapter(com);
                ad.Fill(ds);

                String UrlToRedirect = "https://colourmyspace.co.in/MyViz/Login/Index";

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string PreviewCentreCode = ds.Tables[0].Rows[0]["PreviewCentreCode"].ToString();
                    string CaseIDfromKN = ds.Tables[0].Rows[0]["CaseIDfromKN"].ToString();

                    UrlToRedirect = $"https://colourmyspace.co.in/MyVizKN/CROSOLanding.aspx?caseId={CaseIDfromKN}&previewCenter={PreviewCentreCode}&source=0";
                }

                return Redirect(UrlToRedirect);
            }
        }



    }



}


