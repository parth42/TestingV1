using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.IO;
using System.Net;
using WordAutomationDemo.Models;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PowerPointAddIn
{
    public partial class Ribbon1
    {
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            
        }

        private void Test_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                var pres = Globals.ThisAddIn.Application.ActivePresentation;
                pres.Save();

                if (System.IO.File.Exists(pres.FullName))
                {
                    Microsoft.Office.Core.DocumentProperties properties;
                    properties = (Microsoft.Office.Core.DocumentProperties)
                        pres.BuiltInDocumentProperties;
                    Microsoft.Office.Core.DocumentProperty prop;
                    prop = properties["Manager"];

                    if (prop == null)
                        throw new Exception("This is not a valid Triyo document. Try downloading from Triyo first.");
                    //Microsoft.Office.Interop.PowerPoint.ApplicationClass pptApplication = new Microsoft.Office.Interop.PowerPoint.ApplicationClass();
                    //Microsoft.Office.Interop.PowerPoint.Presentation presentation = pptApplication.Presentations.Open(pres.FullName, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);

                    string strProperties = prop.Value;

                    var propManager = strProperties.Split('|');

                    byte[] bytes = System.IO.File.ReadAllBytes(pres.FullName);

                    //string strData = System.Convert.ToBase64String(bytes);
                    string strData = Convert.ToBase64String(bytes).Replace("+", "%2B");

                    var postData = "AssignmentID=" + propManager[1] + "&FolderName=" + propManager[2] + "&FileName=" + propManager[3] + "&UserID=" + propManager[0] + "&data=" + strData;
                    //string serviceUrl = System.Configuration.ConfigurationManager.AppSettings["ServiceUrl"];

                    var serviceUrl = "http://demo.triyosoft.com";
                    var key = Registry.LocalMachine.OpenSubKey("Software\\Triyo", false);
                    serviceUrl = key?.GetValue("Server", serviceUrl) as string ?? serviceUrl;
                    var request = (HttpWebRequest)WebRequest.Create(serviceUrl + "/AddIn/AddSlideService");
                    request.UseDefaultCredentials = true;
                    var data = Encoding.ASCII.GetBytes(postData);

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;

                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    var response = (HttpWebResponse)request.GetResponse();

                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    MessageBox.Show(responseString);
                }

                //FileStream test = (FileStream)Globals.ThisAddIn.Application.ActivePresentation;

                //Microsoft.Office.Core.DocumentProperties properties;
                //properties = (Microsoft.Office.Core.DocumentProperties)
                //    Globals.ThisAddIn.Application.ActivePresentation.BuiltInDocumentProperties;
                //Microsoft.Office.Core.DocumentProperty prop;
                //prop = properties["Manager"];
                //string strProp = prop.Value;
                //var arrProp = strProp.Split('|');
            }
            catch (Exception _ex)
            {
                MessageBox.Show(_ex.Message);
            }
        }
    }
}
