using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using System.IO;

namespace WordAutomationDemo.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            GeneratePDF();

            return View();
        }
        private void GeneratePDF()
        {
            try
            {
                Document document = new Document();
                document.Info.Title = "Testing Email Template";

                #region DefineDocumentStyles

                const string fontName = "Arial";
                string logoPath = System.Web.HttpContext.Current.Server.MapPath("/images/logo.png");

                // Get the predefined style Normal.
                MigraDoc.DocumentObjectModel.Style style = document.Styles["Normal"];
                MigraDoc.DocumentObjectModel.Color blueText = new MigraDoc.DocumentObjectModel.Color(0, 38, 76);
                MigraDoc.DocumentObjectModel.Color whiteText = new MigraDoc.DocumentObjectModel.Color(255, 255, 255);
                MigraDoc.DocumentObjectModel.Color blueBackGround = new MigraDoc.DocumentObjectModel.Color(27, 117, 188);
                MigraDoc.DocumentObjectModel.Color grayBackGround = new MigraDoc.DocumentObjectModel.Color(221, 221, 221);
                const short paragraphMaxLength = 1000;
                const byte paragraphMinHeight = 90;
                const byte imageDescriptionMaxLength = 50;
                const byte imageLabelMaxLength = 50;

                // Because all styles are derived from Normal, the next line changes the 
                // font of the whole document. Or, more exactly, it changes the font of
                // all styles and paragraphs that do not redefine the font.
                style.Font.Name = fontName;

                // Heading1 to Heading9 are predefined styles with an outline level. An outline level
                // other than OutlineLevel.BodyText automatically creates the outline (or bookmarks) 
                // in PDF.

                style = document.Styles["Heading1"];
                style.Font.Name = fontName;
                style.Font.Size = 20;
                style.Font.Bold = true;
                style.Font.Color = blueText;
                style.ParagraphFormat.SpaceBefore = 70;

                style = document.Styles["Heading2"];
                style.Font.Name = fontName;
                style.Font.Size = 16;
                style.Font.Bold = true;
                style.ParagraphFormat.SpaceBefore = 10;

                style = document.Styles["Heading3"];
                //style.Font.Name = fontName;
                style.Font.Size = 10;
                //style.Font.Bold = true;
                style.ParagraphFormat.SpaceBefore = 5;
                

                style = document.Styles["Heading4"];
                style.Font.Size = 10;
                style.Font.Italic = true;
                style.Font.Color = whiteText;
                style.ParagraphFormat.Borders.Width = 1;
                style.ParagraphFormat.SpaceBefore = 10;
                style.ParagraphFormat.Shading.Color = blueText;
                style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                style.ParagraphFormat.FirstLineIndent = "12mm";

                style = document.Styles.AddStyle("CommonContent", "Normal");
                style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
                style.Font.Size = 8;
                style.Font.Color = blueText;
                style.ParagraphFormat.Borders.Width = 1;

                style = document.Styles.AddStyle("CommonHeading", "Normal");
                style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                style.Font.Size = 10;
                style.Font.Color = whiteText;
                style.ParagraphFormat.Borders.Width = 1;
                style.Font.Italic = true;
                style.Font.Bold = true;
                style.ParagraphFormat.Shading.Color = blueText;
                style.ParagraphFormat.SpaceBefore = 10;

                style = document.Styles.AddStyle("TableContent", "Normal");
                style.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                style.Font.Size = 8;
                style.ParagraphFormat.LeftIndent = 2;

                style = document.Styles.AddStyle("WhiteCell", "Normal");
                style.ParagraphFormat.Shading.Color = whiteText;

                style = document.Styles[StyleNames.Header];
                style.ParagraphFormat.AddTabStop("10cm", TabAlignment.Right);

                style = document.Styles[StyleNames.Footer];
                style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

                style = document.Styles.AddStyle("ImageDescriptionStyle", "Normal");
                style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                style.Font.Name = fontName;
                style.Font.Size = 8;

                style = document.Styles.AddStyle("ImageLabelStyle", "Normal");
                style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                style.Font.Italic = true;
                style.Font.Underline = Underline.Single;
                style.Font.Size = 8;

                #endregion

                #region Email Template
                Section section = document.AddSection();
                section.PageSetup = document.DefaultPageSetup.Clone();
                section.PageSetup.PageFormat = PageFormat.A4;
                section.PageSetup.StartingNumber = 1;
                section.PageSetup.TopMargin = "2.8cm";
                section.PageSetup.LeftMargin = "1.2cm";
                section.PageSetup.RightMargin = "1.2cm";
                section.PageSetup.BottomMargin = "0.5cm";

                var tableHeader = section.Headers.Primary.AddTable();
                                
                #region Add Header
                Column column = tableHeader.AddColumn(528);
                column.Format.Alignment = ParagraphAlignment.Left;

                Row row = tableHeader.AddRow();
                Cell cell = row.Cells[0];
                cell.VerticalAlignment = VerticalAlignment.Top;
                cell.Shading.Color = grayBackGround;
                cell.Borders.Bottom.Width = 1;

                Paragraph paragraph = cell.AddParagraph(string.Empty);
                paragraph.AddLineBreak();

                var imgheader = paragraph.AddImage(logoPath);
                imgheader.WrapFormat.Style = WrapStyle.Through;

                paragraph.AddLineBreak();
                #endregion

                #region Add Middle Content
                paragraph = section.AddParagraph("Hi User Name", "Heading2");
                paragraph = section.AddParagraph(string.Empty, "Heading3");
                paragraph.Format.Borders.Bottom.Width = 0.1;

                section.AddParagraph("Demo task", "Heading2");
                section.AddParagraph("A task has been assigned to you.", "Heading3");
                paragraph = section.AddParagraph(string.Empty, "Heading3");
                paragraph.Format.Borders.Bottom.Width = 0.1;

                section.AddParagraph("Description", "Heading2");
                section.AddParagraph("Description Text Here", "Heading3");
                paragraph = section.AddParagraph(string.Empty, "Heading3");
                paragraph.Format.Borders.Bottom.Width = 0.1;

                section.AddParagraph("Initial comment", "Heading2");
                section.AddParagraph("Description Text Here", "Heading3");
                paragraph = section.AddParagraph(string.Empty, "Heading3");
                paragraph.Format.Borders.Bottom.Width = 0.1;

                section.AddParagraph("Initial comment", "Heading2");
                section.AddParagraph(string.Empty, "Heading3");
                #endregion

                #region Add Table
                var table = new MigraDoc.DocumentObjectModel.Tables.Table();
                table.Style = "TableContent";
                column = table.AddColumn(130);
                table.Borders.Width = 0;
                table.LeftPadding = 0;
                table.RightPadding = 0;

                table.AddColumn(100);
                table.AddColumn(149);
                table.AddColumn(149);

                row = table.AddRow();

                cell = row.Cells[0];
                cell.AddParagraph("User");
                cell.Format.Font.Bold = true;
                cell.Borders.Width = 0.3;
                cell.Shading.Color = blueBackGround;
                cell.Format.Font.Color = new MigraDoc.DocumentObjectModel.Color(255, 255, 255);
                cell.Format.Font.Size = 14;

                cell = row.Cells[1];
                cell.Borders.Width = 0.3;
                cell.Format.Font.Bold = true;
                cell.Shading.Color = blueBackGround;
                cell.Format.Font.Color = new MigraDoc.DocumentObjectModel.Color(255, 255, 255);
                cell.Format.Font.Size = 14;

                cell.AddParagraph("Action");

                cell = row.Cells[2];

                cell.AddParagraph("Comments");
                cell.Borders.Width = 0.3;
                cell.Format.Font.Bold = true;
                cell.Shading.Color = blueBackGround;
                cell.Format.Font.Color = new MigraDoc.DocumentObjectModel.Color(255, 255, 255);
                cell.Format.Font.Size = 14;

                cell = row.Cells[3];
                cell.Borders.Width = 0.3;

                cell.AddParagraph("Date");
                cell.Format.Font.Bold = true;
                cell.Shading.Color = blueBackGround;
                cell.Format.Font.Color = new MigraDoc.DocumentObjectModel.Color(255, 255, 255);
                cell.Format.Font.Size = 14;

                int tempI = 0;
                do
                {
                    row = table.AddRow();

                    cell = row.Cells[0];
                    cell.AddParagraph("Rajiv " + tempI);
                    cell.Borders.Width = 0.3;
                    cell.Format.Font.Size = 10;

                    cell = row.Cells[1];
                    cell.Borders.Width = 0.3;
                    cell.Format.Font.Size = 10;

                    cell.AddParagraph("Approved");

                    cell = row.Cells[2];
                    cell.AddParagraph("done and changed");
                    cell.Borders.Width = 0.3;
                    cell.Format.Font.Size = 10;

                    cell = row.Cells[3];
                    cell.Borders.Width = 0.3;
                    cell.AddParagraph("08/21/2017 07:48:28");
                    cell.Format.Font.Size = 10;

                    tempI++;

                }
                while (tempI < 4);
                document.LastSection.Add(table);
                #endregion

                #region Add Footer
                table = new MigraDoc.DocumentObjectModel.Tables.Table();
                table.Style = "TableContent";
                column = table.AddColumn(528);
                table.LeftPadding = 3;
                table.RightPadding = 0;
                table.Format.Shading.Color = grayBackGround;
                table.Format.Borders.Color = grayBackGround;

                paragraph = section.AddParagraph(string.Empty, "Heading3");

                row = table.AddRow();

                cell = row.Cells[0];
                cell.AddParagraph(string.Empty);
                paragraph.AddLineBreak();
                paragraph = cell.AddParagraph("Thanks and Regards,");
                paragraph.Format.Font.Size = 10;
                paragraph.AddLineBreak();
                FormattedText formattedText = paragraph.AddFormattedText("Triyosoft");
                formattedText.Size = 16;
                formattedText.Bold = true;
                formattedText.Color = blueBackGround;

                cell.AddParagraph(string.Empty);

                document.LastSection.Add(table); 
                #endregion

                #endregion

                #region "Generate Document"
                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                renderer.Document = document;
                renderer.RenderDocument();

                // Save the document...
                string filename = string.Format("{0}.pdf", "TestEmailTemplate");

                using (MemoryStream mStream = new MemoryStream())
                {
                    renderer.PdfDocument.Save(mStream);
                    byte[] fileBytes = mStream.ToArray();
                    System.Web.HttpContext context = System.Web.HttpContext.Current;
                    context.Response.Clear();
                    context.Response.ClearHeaders();
                    context.Response.ClearContent();
                    context.Response.AppendHeader("content-length", fileBytes.Length.ToString());
                    context.Response.ContentType = MimeMapping.GetMimeMapping(filename);
                    context.Response.AppendHeader("content-disposition", "attachment; filename=" + filename);
                    context.Response.BinaryWrite(fileBytes);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            #endregion
        }
    }
}