using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WordAutomationDemo.Common;

namespace WordAutomationDemo.Models
{
    public class WordPageModel : AssignmentModel
    {
        public int AssignedWordPageID { get; set; }
        public int AssignedTaskID { get; set; }
        public bool IsAssigned { get; set; }
        public string AssignedTo { get; set; }
        public int PageNumber { get; set; }
        public string OriginalFile { get; set; }
        public string Ticks { get; set; }
        public string AssignedFile { get; set; }
        public string WordPageRemarks { get; set; }
        public List<WordPageModel> Listpages { get; set; }
        public string ThumbnailLink { get; set; }
        public string ThumbnailLinkForOldWordPage { get; set; }
        public bool? IsModified { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsPublished { get; set; }
        public bool? ReviewRequested { get; set; }
        public bool? IsGrayedOut { get; set; }
        public string AssignedThumbnail { get; set; }
        public string AssignedContent { get; set; }
        public string ChangedThumbnail { get; set; }
        public string AssignedToName { get; set; }
        public int Sequence { get; set; }

    }
}