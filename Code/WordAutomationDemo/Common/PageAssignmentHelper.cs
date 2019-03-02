using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Aspose.Words;
using Aspose.Words.Layout;
using Aspose.Pdf;
using Aspose.Pdf.Facades;
using WordAutomationDemo.Models;
using SaveFormat = Aspose.Words.SaveFormat;

namespace WordAutomationDemo.Common
{
    public class PageAssignmentHelper
    {
        public bool MergePages(Aspose.Words.Document source, Aspose.Words.Document changedPage, int pageNumber, string originalFilePath)
        {
            try
            {
                using (var temp = new MemoryStream())
                {
                    var pagesDir = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, "Pages_" + source.OriginalFileName);
                    var pdf = source.Save(temp, SaveFormat.Pdf);
                    Aspose.Pdf.Document pdfDoc = new Aspose.Pdf.Document(temp);

                    PdfFileEditor pdfEditor = new PdfFileEditor();
                   // pdfEditor.SplitToEnd(originalFilePath, 3, "D://Temp/Splits.pdf");

                    var tempDoc = new Aspose.Words.Document();
                    var pageCount = source.PageCount;
                    var pageSplitter = new DocumentPageSplitter(source);

                    for (var i = 0; i < pageCount; i++)
                    {
                        var pageDoc = pageSplitter.GetDocumentOfPage(i + 1);

                        if (i == pageNumber)
                        {
                            changedPage.FirstSection.PageSetup.SectionStart = SectionStart.Continuous;
                            tempDoc.AppendDocument(changedPage, ImportFormatMode.KeepSourceFormatting);
                        }
                        else
                        {
                            pageDoc.FirstSection.PageSetup.SectionStart = SectionStart.Continuous;
                            tempDoc.AppendDocument(pageDoc, ImportFormatMode.KeepSourceFormatting);
                        }
                    }

                    tempDoc.Save(originalFilePath);

                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }

        }

        private void RemoveBlankPages(Aspose.Words.Document doc)
        {
            foreach (Section section in doc.Sections)
            {
                if (section.ToString(SaveFormat.Text).Trim() == String.Empty)
                    section.Remove();
            }

            String PageText = "";
            LayoutCollector lc = new LayoutCollector(doc);
            int pages = lc.GetStartPageIndex(doc.LastSection.Body.LastParagraph);

            for (int i = 1; i <= pages; i++)
            {

                ArrayList nodes = GetNodesByPage(i, doc);
                foreach (Paragraph para in nodes)
                {
                    PageText += para.ToString(SaveFormat.Text).Trim();
                }

                //Empty Page
                if (PageText == "")
                {
                    foreach (Node node in nodes)
                    {
                        node.Remove();
                    }
                }
                nodes.Clear();

                PageText
                    = "";
            }
        }

        //Get Paragraph nodes by page number
        private ArrayList GetNodesByPage(int page, Aspose.Words.Document document)
        {
            ArrayList nodes = new ArrayList();
            LayoutCollector lc = new LayoutCollector(document);
            foreach (Paragraph para in document.GetChildNodes(NodeType.Paragraph, true))
            {
                if (lc.GetStartPageIndex(para) == page || para.IsEndOfSection)
                    nodes.Add(para);
            }

            return nodes;
        }

        private void RemoveEmptyParagraphs(Aspose.Words.Document doc)
        {
            while (!doc.LastSection.Body.LastParagraph.HasChildNodes)
            {
                if (doc.LastSection.Body.LastParagraph.PreviousSibling != null &&
                    doc.LastSection.Body.LastParagraph.PreviousSibling.NodeType != NodeType.Paragraph)
                    break;
                doc.LastSection.Body.LastParagraph.Remove();

                // If the current section becomes empty, we should remove it.
                if (!doc.LastSection.Body.HasChildNodes)
                    doc.LastSection.Remove();

                // We should exit the loop if the document becomes empty.
                if (!doc.HasChildNodes)
                    break;
            }
        }
    }
}