//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WordAutomationDemo.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblChatMessage
    {
        public long ChatMessageID { get; set; }
        public int UserFromId { get; set; }
        public int UserToId { get; set; }
        public string Message { get; set; }
        public string ClientGuid { get; set; }
        public System.DateTime MessageSentDateTime { get; set; }
        public bool IsRead { get; set; }
        public Nullable<bool> VisibleFrom { get; set; }
        public Nullable<bool> VisibleTo { get; set; }
    }
}
