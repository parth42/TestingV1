using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordAutomationDemo.Models
{
    public class AssignmentLogModel
    {
        public int AssignmentLogID { get; set; }
        public string DocumentName { get; set; }
        public int AssignmentID { get; set; }
        public int Action { get; set; }
        public string ActionString { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
        public string UserName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedDateString { get; set; }
        public List<AssignmentLogModel> lstAssignmentLogModel { get; set; }
        public List<SlideLogModel> lstSlideLogModel { get; set; }
    }
}