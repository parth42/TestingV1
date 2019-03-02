using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using WordAutomationDemo.Common;
using System.Web.Mvc;

namespace WordAutomationDemo.Models
{
    public class RoleModel
    {
        public int RoleID { get; set; }

        public string EncryptedRoleID { get; set; }

        public static int PageSizeRole { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MaxAllowedLength")]
        public string Role { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MaxAllowedLength")]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Company")]
        public int Company_ID { get; set; }

        public string Company { get; set; }

        public SelectList SelectCompany { get; set; }

        public IEnumerable<RoleModel> IListRole { get; set; }
        public string Active { get; set; }
        public CommonMessagesModel ModuleName { get; set; }
        public bool IsAdminRole { get; set; }

        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}