using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordAutomationDemo.Models
{
    public class DocModel
    {
        public int DocumentID { get; set; }
        public int ProjectID { get; set; }
        public string DisplayName { get; set; }
        public string DocumentName { get; set; }
        public int? DocumentType { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ProjectName { get; set; }
    }
}