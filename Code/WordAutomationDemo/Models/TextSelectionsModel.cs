using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordAutomationDemo.Models
{
    public class TextSelectionsModel
    {
        public int PageNumber { get; set; }
        public int start { get; set; }
        public int length { get; set; }
    }
}