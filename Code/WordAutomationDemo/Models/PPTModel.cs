using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WordAutomationDemo.Common;

namespace WordAutomationDemo.Models
{
    public class PPTModel : AssignmentModel
    {
        public int PPTSlideID { get; set; }
        public int AssignedPPTSlideID { get; set; }
        public int AssignedTaskID { get; set; }
        public string SlideName { get; set; }
        public string SlideLink { get; set; }
        public bool IsAssigned { get; set; }
        public int? AssignedTo { get; set; }
        public string OriginalFile { get; set; }
        public string PPTRemarks { get; set; }

        public List<PPTModel> ListSlides { get; set; }
        public string strPPTComments { get; set; }

        public string strExcelComments { get; set; }

        public string Remarks { get; set; }

        public string OriginalSlideName { get; set; }

        public string ThumbnailLink { get; set; }
        public string ThumbnailLinkForOldSlide { get; set; }
        public List<string> lstThumbnailLinkForOldSlide { get; set; }
        public string TaskName { get; set; }
        public bool? IsPPTModified { get; set; }
        public bool? IsPPTApproved { get; set; }
        public bool? IsPublished { get; set; }
        public bool? ReviewRequested { get; set; }
        public bool? IsGrayedOut { get; set; }

        public string AssignedThumbnail { get; set; }
        public string AssignedToName { get; set; }


        //public string ThumbnailFileName
        //{
        //    get
        //    {
        //        return Global.SiteUrl + "ApplicationDocuments/PPTs/" + "Slides_" + this.OriginalDocumentName + "/thumb_" + this.SlideName.Split('.').First().ToString() + ".jpg";
        //    }
        //}

        public int Sequence { get; set; }
        public PPTModel()
        {
            lstThumbnailLinkForOldSlide = new List<string>();
        }

    }
}