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
    
    public partial class tblAssignmentLog
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblAssignmentLog()
        {
            this.tblAssignmentHistories = new HashSet<tblAssignmentHistory>();
        }
    
        public int AssignmentLogID { get; set; }
        public string DocumentName { get; set; }
        public Nullable<int> AssignmentID { get; set; }
        public int Action { get; set; }
        public string Description { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblAssignmentHistory> tblAssignmentHistories { get; set; }
        public virtual tblAssignment tblAssignment { get; set; }
        public virtual tblUserDepartment tblUserDepartment { get; set; }
    }
}
