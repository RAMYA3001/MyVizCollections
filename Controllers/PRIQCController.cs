using MyVizCollections.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyVizCollections.Controllers
{
    public class PRIQCController : Controller
    {
        // GET: PRIQC

        public ActionResult Index(string s1, string s2)
        {
            string constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
            List<AllLevelQueueBoard> projects = new List<AllLevelQueueBoard>(); // declare here
            try
            {



                // Retrieve category from session
                string Username = Session["Username"]?.ToString();


                int imode = 1; // Default to 1


                using (MySqlConnection con = new MySqlConnection(constr))
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand("SP_Myvizcollections_PRIQCheck", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 1600; // ✅ reduced timeout



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
                                    PSEName = rdr["PSE Name"].ToString(),
                                    CPEName = rdr["CPE Name"].ToString(),
                                  
                                    EmailStatus = rdr["EmailStatus"].ToString(),
                                    EMailValues = rdr["EMailValues"].ToString(),

                                    Priority = rdr["Priority"].ToString(),
                                    NexGenDealer = rdr["NexGenDealer"].ToString(),


                                    // ❌ TEMP FIX (no extra DB call)
                                    WStsCount = 0
                                };

                                projects.Add(project);
                            }
                        }
                    }

                    // ✅ OPTIONAL: Get all counts in ONE query
                    var projectIds = projects.Select(p => p.ProjectID).ToList();

                    if (projectIds.Count > 0)
                    {
                        string idList = string.Join(",", projectIds);

                        using (MySqlCommand countCmd = new MySqlCommand(
                            $"SELECT ProjectID, COUNT(*) AS Cnt FROM wstatuslog WHERE ProjectID IN ({idList}) AND Workstatus NOT IN (85,86) GROUP BY ProjectID", con))
                        {
                            using (var reader = countCmd.ExecuteReader())
                            {
                                var dict = new Dictionary<int, int>();

                                while (reader.Read())
                                {
                                    dict[Convert.ToInt32(reader["ProjectID"])] = Convert.ToInt32(reader["Cnt"]);
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
        public ActionResult Getsourceid(string ProjectID1, string ProjectID2, string Type1, string Type2)
        {
            try
            {

                string constr = "";
                constr = ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;
                using (MySqlConnection cn = new MySqlConnection(constr))
                {

                    cn.Open();
                    DataSet ds = new DataSet();
                    MySqlCommand cmd = new MySqlCommand("SP_GetSourceimage", cn);
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

                    cn.Close();

                    return Json(projects, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return null;
            }


        }




        public ActionResult Help()
        {
            ViewBag.SISubmittedPath = @"D:\WEB_APP\MyViz\SISubmitted";
            ViewBag.PRIPath = @"D:\WEB_APP\MyViz\PRI";
            ViewBag.PRITestPath = @"D:\WEB_APP\MyViz\PRITest";
            ViewBag.PDFPath = @"D:\ColoursGalore\CW\User_Data\PDF_Files";

            return View();
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
                    basePath = "D:\\WEB_APP\\MyViz\\SISubmitted";
                    //filename = $"{ProjectID}.jpeg";
                    filename = sourcefile;
                    break;
                case "img1":
                    basePath = "D:\\WEB_APP\\MyViz\\SISubmitted";
                    //filename = $"{ProjectID}.jpeg";
                    image1 = sourcefile;
                    break;
                case "img2":
                    basePath = "D:\\WEB_APP\\MyViz\\SISubmitted";
                    //filename = $"{ProjectID}.jpeg";
                    image2 = sourcefile;
                    break;
                case "img3":
                    basePath = "D:\\WEB_APP\\MyViz\\SISubmitted";
                    //filename = $"{ProjectID}.jpeg";
                    image3 = sourcefile;
                    break;

                case "PRI":
                    basePath = "D:\\WEB_APP\\MyViz\\PRI\\" + ProjectID + "\\";
                    PRI = "PRI_" + $"{ProjectID}.jpg";
                    break;

                case "PRI_Test":
                    basePath = "D:\\WEB_APP\\MyViz\\PRITest\\" + ProjectID + "\\";
                    PRITest = "PRITest_" + $"{ProjectID}.jpg";
                    break;


                case "PDF1":
                    basePath = "D:\\ColoursGalore\\CW\\User_Data\\PDF_Files\\" + ProjectID + "\\";
                    D1 = "1_D" + $"{ProjectID}.pdf";
                    break;
                case "PDF2":
                    basePath = "D:\\ColoursGalore\\CW\\User_Data\\PDF_Files\\" + ProjectID + "\\";
                    D2 = "2_D" + $"{ProjectID}.pdf";
                    break;
                case "PDF3":
                    basePath = "D:\\ColoursGalore\\CW\\User_Data\\PDF_Files\\" + ProjectID + "\\";
                    D3 = "3_D" + $"{ProjectID}.pdf";
                    break;

            }

            string imagePath = Path.Combine(basePath, filename, image1, image2, image3, PRI, PRITest, D1, D2, D3);
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
                con.Open();
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

        // =========================================================
        // SAVE PRI QC
        // Calls SP_Save_PRI_QC
        // =========================================================
        [HttpPost]
        public JsonResult SaveQC(
            string ProjectID,
            string QCStatus,
            string QCRemark)
        {
            string constr =
                ConfigurationManager.ConnectionStrings["Nerolacconstr"].ConnectionString;

            try
            {
                // Get logged-in QC user
                string qcBy = "";

                if (Session["UserID"] != null)
                {
                    qcBy = Session["UserID"].ToString();
                }
                else if (Session["Username"] != null)
                {
                    qcBy = Session["Username"].ToString();
                }

                // Validation
                if (string.IsNullOrEmpty(ProjectID))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Project ID is required."
                    });
                }

                if (string.IsNullOrEmpty(QCStatus))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please select QC Status."
                    });
                }

                if (QCStatus != "Pass" && QCStatus != "Fail")
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid QC Status."
                    });
                }

                using (MySqlConnection con =
                    new MySqlConnection(constr))
                {
                    con.Open();

                    using (MySqlCommand cmd =
                        new MySqlCommand("SP_Save_PRI_QC  ", con))
                    {
                        cmd.CommandType =
                            CommandType.StoredProcedure;

                        cmd.CommandTimeout = 1600;

                        cmd.Parameters.AddWithValue(
                            "@p_ProjectID",
                            ProjectID);

                        cmd.Parameters.AddWithValue(
                            "@p_QCBY",
                            qcBy);

                        cmd.Parameters.AddWithValue(
                            "@p_QCStatus",
                            QCStatus);

                        cmd.Parameters.AddWithValue(
                            "@p_QCRemark",
                            QCRemark ?? "");

                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }

                return Json(new
                {
                    success = true,
                    message = "PRI QC status saved successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =========================================================
        // CHECK COLUMN EXISTS
        // =========================================================
        private bool HasColumn(
            MySqlDataReader reader,
            string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i)
                    .Equals(
                        columnName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}


