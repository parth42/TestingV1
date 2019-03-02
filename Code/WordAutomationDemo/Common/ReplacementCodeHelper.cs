using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using DevExpress.Web.Office;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Export;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Common
{
    public class ReplacementCodeHelper
    {
        public string AddReplacementCode(AssignmentModel model, RichEditDocumentServer server, Document document, string startLabel, string endLabel, int start, int length, int totalAdded, bool isEntireDoc)
        {
            DocumentPosition myStart = document.CreatePosition(start + totalAdded);
            DocumentRange myRange = document.CreateRange(myStart, length);

            if (isEntireDoc)
            {
                myStart = document.CreatePosition(0);
                myRange = document.CreateRange(myStart, document.Range.End.ToInt());
            }
            document.Selection = myRange;
            PlainTextDocumentExporterOptions d = new PlainTextDocumentExporterOptions();

            string s = document.GetHtmlText(myRange, null);
            var plainText = document.GetText(myRange, d);

            using (server)
            {
                server.Document.InsertText(myStart, startLabel + Environment.NewLine);

                //format start label section to orange
                var StartLabelRange = server.Document.CreateRange((myStart.ToInt() - (startLabel.Length + 1)), startLabel.Length);
                CharacterProperties cpLabel = server.Document.BeginUpdateCharacters(StartLabelRange);
                cpLabel.Reset();
                cpLabel.BackColor = Color.Orange;
                server.Document.EndUpdateCharacters(cpLabel);

                //format assigned section to gray;
                CharacterProperties cp = server.Document.BeginUpdateCharacters(myRange);
                cp.Reset();
                cp.BackColor = Color.LightGray;
                server.Document.EndUpdateCharacters(cp);
                server.Document.InsertText(myRange.End, Environment.NewLine + endLabel + Environment.NewLine);

                //format end label to orange
                var endlabelRange = server.Document.CreateRange((myRange.End.ToInt() - (endLabel.Length + 1)), endLabel.Length);
                CharacterProperties cpendLabel = server.Document.BeginUpdateCharacters(endlabelRange);
                cpendLabel.Reset();
                cpendLabel.BackColor = Color.Orange;
                server.Document.EndUpdateCharacters(cpendLabel);

                server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + model.DocumentName, (model.DocumentName.Split('.').Last() == "doc") ? DevExpress.XtraRichEdit.DocumentFormat.Doc : DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                DocumentManager.CloseAllDocuments();
            }

            server.Dispose();
            return s;
        }
    }
}