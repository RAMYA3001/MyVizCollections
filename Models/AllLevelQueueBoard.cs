using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


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

        public string ImageFileName1 { get; set; }
        public string ImageFileName2 { get; set; }
        public string ImageFileName3 { get; set; }
        public string Site { get; set; }
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



    }
}
