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
    
    public partial class vwRolePrivilege
    {
        public int MenuItemID { get; set; }
        public string MenuItem { get; set; }
        public string MenuItemController { get; set; }
        public string MenuItemView { get; set; }
        public Nullable<bool> ViewPermission { get; set; }
        public Nullable<bool> Add { get; set; }
        public Nullable<bool> Edit { get; set; }
        public Nullable<bool> Delete { get; set; }
        public Nullable<bool> Detail { get; set; }
        public Nullable<int> OrderID { get; set; }
        public Nullable<int> ParentID { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<int> IsActive { get; set; }
        public Nullable<int> RoleID { get; set; }
    }
}
