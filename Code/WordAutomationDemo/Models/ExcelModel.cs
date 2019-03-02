using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordAutomationDemo.Models
{
    public class ExcelModel : AssignmentModel
    {
        public int AssignedExcelSheetID { get; set; }
        public int AssignedTaskID { get; set; }
        public string SheetName { get; set; }
        public string SheetLink { get; set; }
        public bool IsAssigned { get; set; }
        public string OriginalFile { get; set; }
        public string SheetRemarks { get; set; }
        public List<ExcelModel> ListSheets { get; set; }
        public List<string> lstModifiedSheet { get; set; }
        public string strSheetComments { get; set; }
        public string OriginalSheetName { get; set; }
        public bool? IsSheetModified { get; set; }
        public bool? IsSheetApproved { get; set; }
        public bool? IsGrayedOut { get; set; }
        public string AssignedToName { get; set; }

        public string SlideLinks { get; set; }

        //public string ThumbnailFileName
        //{
        //    get
        //    {
        //        return Global.SiteUrl + "ApplicationDocuments/PPTs/" + "Slides_" + this.OriginalDocumentName + "/thumb_" + this.SlideName.Split('.').First().ToString() + ".jpg";
        //    }
        //}

        public int Sequence { get; set; }
    }
}