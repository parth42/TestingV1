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
    
    public partial class tblProjectMember
    {
        public int ProjectMemberID { get; set; }
        public int ProjectID { get; set; }
        public int UserID { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    
        public virtual tblProject tblProject { get; set; }
        public virtual tblUserDepartment tblUserDepartment { get; set; }
        public virtual tblUserDepartment tblUserDepartment1 { get; set; }
    }
}
