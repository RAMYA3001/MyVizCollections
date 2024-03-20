using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


namespace MyVizCollections.Models
{
    public class ProjectDetails
    {
        public int ProjectID { get; set; }

        public string ProjectName { get; set; }

        public string UserID { get; set; }
        public string IorEorMS { get; set; }
        public string Feedback { get; set; }
        public string NeroliteName { get; set; }
        public string Depot { get; set; }
        public string DepotName { get; set; }
        public string ProjectCategory { get; set; }

        public string NeroliteNo { get; set; }
        public string NeroliteEmail { get; set; }

        public string Options { get; set; }

        public string Resolution { get; set; }

        public string Size { get; set; }

        public string caseID { get; set; }

        public string SpecialRequest { get; set; }
        public string InsyComments { get; set; }
        public string SiteAddress { get; set; }
        public string UserName { get; set; }
        public string Location { get; set; }
        public string CustomerName { get; set; }
        public string remarks { get; set; }
        public string statuscode { get; set; }


        public string imagefilename1 { get; set; }
        public string imagefilename2 { get; set; }
        public string imagefilename3 { get; set; }

        public string SIFilename { get; set; }
        public string Priority { get; set; }

        public string PSE { get; set; }
        public string QACPI { get; set; }

    }
}