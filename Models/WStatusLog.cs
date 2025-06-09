using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


namespace MyVizCollections.Models
{
    public class WStatusLog
    {

        public int ProjectID { get; set; }
        public string WorkStatus { get; set; }
        public string WorkStatusDesc { get; set; }
        public DateTime? M_date { get; set; }
        public string M_dateFormatted { get; set; }

        public string AssignedTo { get; set; }
        public string PSECode { get; set; }
        public string LogType { get; set; }
        public DateTime C_Date { get; set; }
        public int Count { get; set; }
        public string Remarks { get; set; }


     
        public string Status { get; set; }
        public string StRemarks { get; set; }
        public string SourcePixel { get; set; }
        public string SourceFileSize { get; set; }
        public string QCComments { get; set; }
        public string QCResult { get; set; }
        public string PSEID { get; set; }
        public string CPEID { get; set; }
        public string PRIPixels { get; set; }
        public string PRIFilesize { get; set; }
        public string Notes { get; set; }
        public string C_id { get; set; }
        public DateTime? C_date { get; set; }         // This maps to C_Date
        public string PSERemarks { get; set; }
        public string CPERemarks { get; set; }
        public string TimeTaken { get; set; }

      


}
}
