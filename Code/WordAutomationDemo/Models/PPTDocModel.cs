using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordAutomationDemo.Models
{
    public class PPTDocModel : Result
    {
        public int AssignmentID { get; set; }
        public string FileName { get; set; }
        public HttpPostedFile objHttpPostedFile { get; set; }
    }
}