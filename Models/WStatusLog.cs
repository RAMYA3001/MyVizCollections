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

        
        public string PSECode { get; set; }
        public string LogType { get; set; }
        public DateTime C_Date { get; set; }
        public int Count { get; set; }
        public string Remarks { get; set; }
    }
}
