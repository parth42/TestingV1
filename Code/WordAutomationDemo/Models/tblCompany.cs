//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WordAutomationDemo.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblCompany
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblCompany()
        {
            this.tblRoles = new HashSet<tblRole>();
            this.tblSectionMasters = new HashSet<tblSectionMaster>();
            this.tblUserDepartments = new HashSet<tblUserDepartment>();
        }
    
        public int CompanyID { get; set; }
        public string CompanyLogo { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsSuperAdmin { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string WebsiteURL { get; set; }
        public bool IsAppointmentEnable { get; set; }
        public string ExchangeServerURL { get; set; }
        public string ExchangeServerUserName { get; set; }
        public string ExchangeServerPassword { get; set; }
        public Nullable<int> DateFormatID { get; set; }
        public bool IsMessengerServiceEnable { get; set; }
    
        public virtual tblDateFormat tblDateFormat { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblRole> tblRoles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblSectionMaster> tblSectionMasters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblUserDepartment> tblUserDepartments { get; set; }
    }
}
