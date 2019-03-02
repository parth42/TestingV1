using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;

namespace WordAutomationDemo.Models
{
    public class UserModel
    {

        public int UserId { get; set; }

        public string EncryptedUserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Full Name")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MaxAllowedLength")]
        public string FullName { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [StringLength(250, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MaxAllowedLength")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [StringLength(20, MinimumLength = 6, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MinMaxAllowedLength")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Confirm Password")]
        [StringLength(20, MinimumLength = 6, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MinMaxAllowedLength")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Department")]
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MaxAllowedLength")]
        public string Department { get; set; }

        [Display(Name = "Email ID")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [StringLength(250, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MaxAllowedLength")]
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9-]+)*\\.([a-z]{2,4})$", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "InvalidEmail")]
        public string EmailID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Role")]
        public int Role_ID { get; set; }

        public string Role { get; set; }

        public IEnumerable<SelectListItem> SelectRole { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Company")]
        public int Company_ID { get; set; }

        public string CompanyLogo { get; set; }

        public string Company { get; set; }

        public SelectList SelectCompany { get; set; }

        public bool IsActive { get; set; }

        public bool IsRoleActive { get; set; }

        public bool IsCompanyActive { get; set; }

        public bool isSelectedForProject { get; set; }

        public CommonMessagesModel ModuleName { get; set; }

        public string Active { get; set; }

        public IEnumerable<UserModel> IListUser { get; set; }

        public bool IsCurrentUser { get; set; }

        public bool IsAdminUser { get; set; }

        public string ProfileImage { get; set; }

        public bool IsSuperAdmin { get; set; }

        [Display(Name = "Exchange Server")]
        public bool IsAppointmentEnable { get; set; }

        public string DTformat { get; set; }
        public string DTFormatGrid { get; set; }
        public bool IsMessengerServiceEnable { get; set; }

        public bool CanCreateSubtasks { get; set; }
        public bool CanView { get; set; }
        public bool CanPublish { get; set; }
        public bool CanApprove { get; set; }
        public bool CanEdit { get; set; }
    }
}