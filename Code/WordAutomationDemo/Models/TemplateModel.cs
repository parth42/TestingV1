using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WordAutomationDemo.Common;

namespace WordAutomationDemo.Models
{
    public class TemplateModel
    {
        public int? TemplateID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [StringLength(50, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MaxAllowedLength")]
        [Display(Name = "Template name")]
        public string TemplateName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Template type")]
        public int TemplateType { get; set; }


        public string Subject { get; set; }
        public string TemplateContentForEmail { get; set; }
        public string TemplateContentForSms { get; set; }
    }
}