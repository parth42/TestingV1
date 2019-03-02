using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordAutomationDemo.Models
{
    public class AssignmentMemberModel
    {
        public int UserID { get; set; }
        public bool CanEdit { get; set; }
        public bool CanApprove { get; set; }
        public bool CanPublish { get; set; }

    }
}