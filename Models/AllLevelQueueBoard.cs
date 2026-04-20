using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using context = System.Web.HttpContext;

namespace MyVizCollections.Models
{
    public class AllLevelQueueBoard
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public DateTime? RegdOn { get; set; }  // Nullable DateTime
        public string RegdBy { get; set; }
        public string FinalStatus { get; set; }
        public DateTime? FinalStatusDt { get; set; }  // Nullable DateTime
        public string Type { get; set; }
        public string Category { get; set; }
        public string SelectedImageLink { get; set; }
        public string PRILink { get; set; }
        public string PRITest { get; set; }
        public string SCA1 { get; set; }
        public string SCA2 { get; set; }
        public string SCA3 { get; set; }
        public string Preview { get; set; }
        public int WStatusCount { get; set; }
        public int WStsCount { get; set; }
        public string SIOption { get; set; }
        public decimal ActualTAT { get; set; }
        public decimal EstimateTAT { get; set; }
        public decimal ExceededTAT { get; set; }
        public string ImageFileName1 { get; set; }
        public string ImageFileName2 { get; set; }
        public string ImageFileName3 { get; set; }
        public string Site { get; set; }
        public string depotname { get; set; }
        public string Customer { get; set; }
        public string PSEName { get; set; }
        public string CPEName { get; set; }

        public string FeedBack { get; set; }
        public string CC { get; set; }
        public string DuplicateOf { get; set; }
        public string NexGenDealer { get; set; }
        public string Appliedcolour { get; set; }
        public string Priority { get; set; }
        // Added for TAT
        public double RemainingTATHours { get; set; }
        public DateTime DeliveryOn { get; set; }
        public string EmailStatus { get; set; }
        
            public string EMailValues { get; set; }
        public string QCResult { get; set; }
        public string QCComments { get; set; }
        public string CPERemarks { get; set; }
        public string WhoistheL6PSE { get; set; }
        public string PSERemarks { get; set; }

        // Add this property if missing
        public string CPEID { get; set; }

        public string BannerMessage { get; set; }


    }


    public static class ExceptionLogging
    {

        private static String ErrorlineNo, Errormsg, extype, exurl, hostIp, ErrorLocation, HostAdd;

        public static void SendErrorToText(Exception ex)
        {
            var line = Environment.NewLine + Environment.NewLine;
            ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7);
            Errormsg = ex.GetType().Name.ToString();
            extype = ex.GetType().ToString();
            exurl = context.Current.Request.Url.ToString();
            ErrorLocation = ex.Message.ToString();
            try
            {
                string filepath = context.Current.Server.MapPath("~/ExceptionDetailsFile/");  //Text File Path
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                filepath = filepath + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {
                    File.Create(filepath).Dispose();
                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line + "Error Line No :" + " " + ErrorlineNo + line + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " " + extype + line + "Error Location :" + " " + ErrorLocation + line + " Error Page Url:" + " " + exurl + line + "User Host IP:" + " " + hostIp + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();

                }

            }
            catch (Exception e)
            {
                e.ToString();

            }

        }
    }
    }