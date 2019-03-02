using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordAutomationDemo.Models
{
    public class UploadedDocModel
    {
        public int AssignedDocsID { get; set; }
        public int AssignmentID { get; set; }
        public string DocName { get; set; }
        public string DocURL { get; set; }
        public string DownloadImagePath { get; set; }
        public int Sequence { get; set; }
        public bool IsDocFile { get; set; }
        public bool IsPPTFile { get; set; }
    }
    public class ProjectDocumentModel
    {
        public string ProjectTaskID { get; set; }
        public string ProjectName { get; set; }
        public string DocName { get; set; }
    }
}