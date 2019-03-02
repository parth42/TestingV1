using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;

namespace WordAutomationDemo.Models
{
    public class ProjectModel
    {
        public int ProjectID { get; set; }

        public string EncryptedProjectID { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "MM/DD/YYYY")]
        public DateTime StartDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "MM/DD/YYYY")]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        public int Status { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public Nullable<int> ModifiedBy { get; set; }

        public string CreatedByName { get; set; }

        public List<ProjectModel> ProjectList { get; set; }

        public List<UserModel> MemberList { get; set; }

        public List<SelectListItem> myProjectList { get; set; }

        public List<SelectListItem> PPTList { get; set; }
        public List<SelectListItem> WordList { get; set; }
        public List<SelectListItem> ProjectDocuments { get; set; }
        public List<SelectListItem> Members { get; set; }
        public List<SelectListItem> MembersSelected { get; set; }

        public List<int> MemberIDList { get; set; }

        public List<int> DocumentIDList { get; set; }

        public string FileName { get; set; }

        public string DocumentID { get; set; }

        public string DocumentIDs { get; set; }

        public int DocumentType
        {
            get
            {
                if (!string.IsNullOrEmpty(this.FileName))
                    if (this.FileName.Split('.').Last().ToLower() == "ppt" || this.FileName.Split('.').Last().ToLower() == "pptx")
                        return (int)Global.DocumentType.Ppt;
                    else
                        return (int)Global.DocumentType.Word;
                else
                    return 0;
            }
        }


        public string ViewIcon
        {
            get
            {
                if (this.DocumentType == (int)Global.DocumentType.Word)
                {
                    return "<img  src=" + Global.SiteUrl + "Css/Images/word.png  style='cursor:pointer' title='View'/> ";
                }
                else if (this.DocumentType == (int)Global.DocumentType.Ppt)
                {
                    return "<img src=" + Global.SiteUrl + "Css/Images/ppt.png style='cursor:pointer' title='View' />";
                }
                else if (this.DocumentType == (int)Global.DocumentType.Xls)
                {
                    return "<img src=" + Global.SiteUrl + "Css/Images/xls.png style='cursor:pointer' title='View' />";
                }
                else
                    return "N/A";

            }
        }
        public string strStartDate { get; set; }
        public string strEndDate { get; set; }
        public string strStatus {
            get
            {
                if (this.Status == (int)Global.ProjectStatus.Active)
                {
                    return "Active";
                }
                else if (this.Status == (int)Global.ProjectStatus.InActive)
                {
                    return "InActive";
                }
                else if (this.Status == (int)Global.ProjectStatus.Completed)
                {
                    return "Completed";
                }
                    return "";
            }
        }
    }
}