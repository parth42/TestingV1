using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;

namespace WordAutomationDemo.Models
{
    public class CompanyModel
    {
        public int CompanyID { get; set; }

        public string EncryptedCompanyID { get; set; }

        public string CompanyLogo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        public string Name { get; set; }

        public string Address { get; set; }

        [Display(Name = "Zip/Postal")]
        public string Zip { get; set; }

        public string City { get; set; }

        [Display(Name = "State/Province/Other")]
        public string State { get; set; }

        public string Country { get; set; }

        public int CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public Nullable<int> ModifiedBy { get; set; }

        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public Nullable<bool> IsDeleted { get; set; }        

        public string strStatus { get { return this.IsActive == true ? "Active" : "InActive"; } }

        public bool? IsSuperAdmin { get; set; }

        private string _WebsiteURL;
        [StringLength(250, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MaxAllowedLength")]
        [RegularExpression(@"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "InValidWebURL")]
        [Display(Name = "Website URL")]
        public string WebsiteURL
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_WebsiteURL))
                {
                    return (_WebsiteURL.ToLower().Contains("http:") || _WebsiteURL.Contains("https:")) ? _WebsiteURL : "http://" + _WebsiteURL;
                }
                return null;
            }
            set { _WebsiteURL = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        [Display(Name = "Date Format")]
        public int DateFormatID { get; set; }

        public string DateFormat { get; set; }

        public SelectList SelectDateFormat { get; set; }

        [Display(Name = "Exchange Server")]
        public bool IsAppointmentEnable { get; set; }

        [Display(Name = "Exchange Server URL")]
        public string ExchangeServerURL { get; set; }

        [Display(Name = "Exchange Server UserName")]
        public string ExchangeServerUserName { get; set; }

        [StringLength(20, MinimumLength = 6, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "MinMaxAllowedLength")]
        [DataType(DataType.Password)]
        public string ExchangeServerPassword { get; set; }

        [Display(Name = "Messenger Service")]
        public bool IsMessengerServiceEnable { get; set; }
    }
}