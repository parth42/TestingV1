using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WordAutomationDemo.Common;

namespace WordAutomationDemo.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "In valid link or user.")]
        public int UserID { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [StringLength(20, MinimumLength = 6, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MinMaxAllowedLength")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [StringLength(20, MinimumLength = 6, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MinMaxAllowedLength")]
        [DataType(DataType.Password)]
        [DisplayName("Confirm password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string OldPassword { get; set; }
    }
}