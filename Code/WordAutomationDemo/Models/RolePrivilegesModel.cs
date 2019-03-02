using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Models
{
    public class RolePrivilegesModel
    {
        public int RolePrivilegeID { get; set; }

        [Required(ErrorMessageResourceType = typeof(WordAutomationDemo.Common.Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name="Role")]
        public int Role_ID { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

        public SelectList MenuItemID { get; set; }

        [Required(ErrorMessageResourceType = typeof(WordAutomationDemo.Common.Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Company")]
        public int Company_ID { get; set; }

        public SelectList Company { get; set; }

        public CommonMessagesModel ModuleName { get; set; }

        public bool IsSuperAdmin { get; set; }
    }
}