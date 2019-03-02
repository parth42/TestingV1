using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordAutomationDemo.Models
{
    
    public class MenuModel
    {
        public int MenuItemID { get; set; }
        public string MenuItem { get; set; }
        public string MenuItemController { get; set; }
        public string MenuItemView { get; set; }
        public Nullable<bool> View { get; set; }
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