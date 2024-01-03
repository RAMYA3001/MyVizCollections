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
        public string SCA1 { get; set; }
        public string SCA2 { get; set; }
        public string SCA3 { get; set; }
        public string Preview { get; set; }

        public string SIOption { get; set; }
        
        public string ImageFileName1 { get; set; }
        public string ImageFileName2 { get; set; }
        public string ImageFileName3 { get; set; }
    }
}
