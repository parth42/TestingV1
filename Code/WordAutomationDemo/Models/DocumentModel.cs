using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordAutomationDemo.Models
{
    public class DocumentModel
    {

        public int? DocumentID { get; set; }
        public string Content { get; set; }

        public string ReplacementCode { get; set; }
        public int Action { get; set; }
        public string DocumentFile { get; set; }
        public DateTime? DueDate{ get; set; }

        public int ProjectID { get; set; }
        public int? UserID { get; set; }
        public int CreatedBy { get; set; }
        public string OriginalDocument { get; set; }
        public string Comment{ get; set; }
        public string Remarks{ get; set; }
        public string TaskName { get; set; }

        public string SourceFileCloneFile { get; set; }

        public int? DocumentType { get; set; }
        public bool IsEntireDocument { get; set; }
    }
}