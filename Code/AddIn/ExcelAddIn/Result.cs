using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WordAutomationDemo.Models
{
    public class Result
    {
        public string ObjectID { get; set; }
        public string DeviceID { get; set; }
        public string DeviceToken { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool Status { get; set; }
        public Guid UserID { get; set; }

        private string _outputUserID;
        public string OutputUserID
        {
            get
            {
                try
                {
                    return _outputUserID;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        byte[] strArray = Encoding.ASCII.GetBytes(value);
                        _outputUserID = Convert.ToBase64String(strArray);
                    }
                    else { _outputUserID = string.Empty; }
                }
                catch
                {
                    _outputUserID = string.Empty;
                }
            }
        }
    }
}