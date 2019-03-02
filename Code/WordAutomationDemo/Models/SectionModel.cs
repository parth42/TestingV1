using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WordAutomationDemo.Common;

namespace WordAutomationDemo.Models
{
    public class SectionModel
    {

        public int SectionID { get; set; }

        public string EncryptedSectionID { get; set; }

        [Display(Name = "Section Name ")]
        [Required(ErrorMessage = "Section name is required")]
        [DataType(DataType.Text)]
        public string SectionName { get; set; }

        public string SectionURL { get; set; }

        [DataType(DataType.Text)]
        public string Description { get; set; }

        [Display(Name = "Content Type")]
        public bool ContentType { get; set; }

        [Display(Name = "Content File")]
        [Required(ErrorMessage = "Please upload document")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ContentFile { get; set; }

        public string ContentFileName { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<bool> Status { get; set; }

        public string strStatus { get { return this.Status.Value == true ? "Active" : "InActive"; } }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int DocumentType { get; set; }

        public string BlankDoc { get; set; }

        public IEnumerable<SectionModel> SectionList { get; set; }


        public string ViewIcon
        {
            get
            {
                if (this.ContentFileName.Split('.').Last() == "docx" || this.ContentFileName.Split('.').Last() == "doc")
                {
                    return "<img  src= " + Global.SiteUrl + "Css/Images/word.png  style='cursor:pointer' /> ";
                }
                if (this.ContentFileName.Split('.').Last() == "ppt" || this.ContentFileName.Split('.').Last() == "pptx")
                {
                    return "<img src= " + Global.SiteUrl + "Css/Images/ppt.png style='cursor:pointer' />";
                }
                else if (this.ContentFileName.Split('.').Last() == "xls" || this.ContentFileName.Split('.').Last() == "xlsx")
                {
                    return "<img src= " + Global.SiteUrl + "Css/Images/xls.png style='cursor:pointer' />";
                }
                else
                    return "View";

            }
        }

    }
}