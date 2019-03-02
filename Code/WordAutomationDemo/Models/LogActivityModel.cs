using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordAutomationDemo.Models
{
    public class LogActivityModel
    {
        public int LogActivityID { get; set; }
        public int LogActivityTypeID { get; set; }
        public string ActivityDetails { get; set; }
        public string IPAddress { get; set; }
        public int? ChangedID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByName { get; set; }
        public string LogActivityTypeName { get; set; }
        public string ChangedRoleNameOrID { get; set; }

        public IEnumerable<LogActivityModel> IListLogActivity { get; set; }
        public CommonMessagesModel ModuleName { get; set; }
    }
}