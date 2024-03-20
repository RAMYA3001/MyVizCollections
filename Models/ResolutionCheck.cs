using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MyVizCollections.Models
{
    public class ResolutionCheck
    {
        public int ProjectID { get; set; }
        public string TSO { get; set; }
        public DateTime? C_Date { get; set; }  // Nullable DateTime
        public string FileSize1 { get; set; }
        public string FileSize2 { get; set; }
        
        public string FileSize3 { get; set; }
        public string Res11 { get; set; }
        public string Res12 { get; set; }
        public string Res11_12 { get; set; }
        public string Res21 { get; set; }
        public string Res22 { get; set; }
        public string Res21_22 { get; set; }
        public string Res31 { get; set; }

        public string Res32 { get; set; }

        public string Res31_32 { get; set; }
        public string SIOption { get; set; }
        public string Status { get; set; }
        public string Remarksforrejections { get; set; }
        

    }
}
