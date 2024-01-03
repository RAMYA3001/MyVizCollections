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

namespace MyVizCollections.Controllers
{
    public class AllLevelQueueBoardController : Controller
    {
        public ActionResult Index(int? page ,string Fdate,string ProjectId)
        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;

            try
            {

                if(Fdate == null)
                {
                    Fdate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    using (MySqlCommand cmd = new MySqlCommand("SP_ProjectQueueBoard", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@From_date", Fdate);
                        cmd.Parameters.AddWithValue("@P_ID", ProjectId);

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
                                    SIOption=rdr["SIOption"].ToString(),
                                    ImageFileName1 = rdr["ImageFileName1"].ToString(),
                                    ImageFileName2 = rdr["ImageFileName2"].ToString(),
                                    ImageFileName3 = rdr["ImageFileName3"].ToString(),

                                };

                                projects.Add(project);

                            }
                        }
                        con.Close();

                        int pageSize = 10; // Adjust the page size as needed
                        int pageNumber = (page ?? 1);
                        //ViewBag.Fdate = DateTime.Now.ToString("yyyy-MM-dd");
                        ViewBag.Fdate =Fdate;
                        ViewBag.ProjectId = ProjectId;
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
                    NeroliteID = Convert.ToString(ds.Tables[0].Rows[0]["UserID"]),
                    Feedback = Convert.ToString(ds.Tables[0].Rows[0]["Feedback"]),
                    InsyComments = Convert.ToString(ds.Tables[0].Rows[0]["InSyComments"]),
                    Resolution = Convert.ToString(ds.Tables[0].Rows[0]["Resolution"]),
                    Size = Convert.ToString(ds.Tables[0].Rows[0]["FileSize"]),
                    caseID = Convert.ToString(ds.Tables[0].Rows[0]["CaseID"]),
                   
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


