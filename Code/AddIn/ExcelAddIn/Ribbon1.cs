using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Specialized;
using System.Web;
using System.Net.Http;
using System.Net;
using Microsoft.Win32;

namespace ExcelAddIn
{
    public partial class Ribbon1
    {   
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void Save_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                var excel = Globals.ThisAddIn.Application.ActiveWorkbook;
                excel.Save();

                if (System.IO.File.Exists(excel.FullName))
                {
                    Microsoft.Office.Core.DocumentProperties properties;
                    properties = (Microsoft.Office.Core.DocumentProperties)
                        excel.BuiltinDocumentProperties;
                    Microsoft.Office.Core.DocumentProperty prop;
                    prop = properties["Manager"];
                    if (prop == null)
                        throw new Exception("This is not a valid Triyo document. Try downloading from Triyo first.");
                    string strProperties = prop.Value;

                    var propManager = strProperties.Split('|');

                    byte[] bytes = new byte[16 * 1024];
                    var stream1 = new FileStream(excel.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    var message = new HttpRequestMessage();

                    var content = new MultipartFormDataContent();
                    var fileName = System.IO.Path.GetFileName(excel.FullName);
                    content.Add(new StreamContent(stream1), "file", fileName);

                    message.Method = HttpMethod.Post;
                    message.Content = content;

                    var serviceUrl = "http://demo.triyosoft.com";
                    var key = Registry.LocalMachine.OpenSubKey("Software\\Triyo", false);
                    serviceUrl = key?.GetValue("Server", serviceUrl) as string ?? serviceUrl;
                    message.RequestUri = new Uri(serviceUrl + "/AddIn/AddSheetService");

                    var client = new HttpClient(new HttpClientHandler()
                    {
                        UseDefaultCredentials = true
                    });
                    client.SendAsync(message).ContinueWith(task =>
                    {
                        if (task.Result.IsSuccessStatusCode)
                        {
                            MessageBox.Show(task.Result.Content.ReadAsStringAsync().Result);
                            //do something with response
                        }
                    });
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
