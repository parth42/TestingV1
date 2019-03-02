using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Controllers;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Classes;
using WordAutomationDemo.Repository.Interfaces;

namespace WordAutomationDemo.Common
{
    public class CommonHelper
    {
        #region Add Documents
        public static void AddProjectDocuments(int ProjectID, string DocumentIDs, ReadyPortalDBEntities _entities, bool isCreate)
        {
            ILogActivity iLogActivity = new LogActivityRepository(_entities);

            int[] intDocumentIDs = Array.ConvertAll(DocumentIDs.Split(','), Convert.ToInt32);
            if (isCreate)
            {
                foreach (var documentID in intDocumentIDs)
                {
                    tblProjectDocument objtblProjectDocument = new tblProjectDocument();
                    objtblProjectDocument.ProjectID = ProjectID;
                    objtblProjectDocument.DocumentID = documentID;
                    objtblProjectDocument.IsActive = true;
                    _entities.tblProjectDocuments.Add(objtblProjectDocument);
                    _entities.SaveChanges();
                    iLogActivity.AddInformation("Project's(" + ProjectID + ") document added(" + documentID + ")", new int[] { objtblProjectDocument.ProjectDocumentID });
                }
            }
            else
            {
                var projectsDocuments = _entities.tblProjectDocuments.Where(pd => pd.ProjectID == ProjectID).ToList();
                if (projectsDocuments != null && projectsDocuments.Count() > 0)
                {
                    projectsDocuments.ForEach(x => x.IsActive = false);
                }

                _entities.SaveChanges();
                foreach (var documentID in intDocumentIDs)
                {
                    tblProjectDocument projectsDocument = _entities.tblProjectDocuments.Where(pd => pd.ProjectID == ProjectID && pd.DocumentID == documentID).FirstOrDefault();
                    if (projectsDocument != null)
                    {
                        projectsDocument.IsActive = true;
                        _entities.SaveChanges();
                    }
                    else
                    {
                        tblProjectDocument objtblProjectDocument = new tblProjectDocument();
                        objtblProjectDocument.ProjectID = ProjectID;
                        objtblProjectDocument.DocumentID = documentID;
                        objtblProjectDocument.IsActive = true;
                        _entities.tblProjectDocuments.Add(objtblProjectDocument);
                        _entities.SaveChanges();
                        iLogActivity.AddInformation("Project's(" + ProjectID + ") document added(" + documentID + ")", new int[] { objtblProjectDocument.ProjectDocumentID });
                    }
                }
            }
        }
        #endregion

        #region Add Documents
        public static tblDocument AddDocuments(string displayName, string newFileName, int? docType, ReadyPortalDBEntities _entities)
        {
            tblDocument objtblDocument = new tblDocument();
            objtblDocument.DisplayName = displayName;
            objtblDocument.DocumentName = newFileName;
            objtblDocument.DocumentType = docType;
            objtblDocument.CompanyID = CurrentUserSession.User.CompanyID;
            objtblDocument.CreatedBy = CurrentUserSession.UserID;
            objtblDocument.CreatedDate = DateTime.Now;
            _entities.tblDocuments.Add(objtblDocument);
            _entities.SaveChanges();
            return objtblDocument;
        }
        #endregion

        public static List<SelectListItem> GetDocFilesInDirectory(ReadyPortalDBEntities _entities)
        {
            List<SelectListItem> documentFileCollection = new List<SelectListItem>();

            var lstDocument = (from items in _entities.tblDocuments
                               where items.CompanyID == CurrentUserSession.User.CompanyID
                               orderby items.DocumentName
                               select new DocModel
                               {
                                   DocumentID = items.DocumentID,
                                   DisplayName = items.DisplayName,
                                   DocumentName = items.DocumentName,
                                   DocumentType = items.DocumentType,
                               }).ToList();

            foreach (var document in lstDocument)
            {
                string strFilePath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathDemo, document.DocumentName);
                string strPPTFilePath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, document.DocumentName);
                string strExcelFilePath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, document.DocumentName);
                if (System.IO.File.Exists(strFilePath) || System.IO.File.Exists(strPPTFilePath) || System.IO.File.Exists(strExcelFilePath))
                {
                    documentFileCollection.Add(new SelectListItem { Text = document.DocumentName, Value = document.DocumentID.ToString() });
                }
            }

            return documentFileCollection;

        }

        public static List<SelectListItem> GetMembersList(ReadyPortalDBEntities _entities)
        {
            List<SelectListItem> memberFileCollection = new List<SelectListItem>();

            List<UserModel> lstMember = new List<UserModel>();

            if (CurrentUserSession.User.IsSuperAdmin)
            {
                var companyID = CurrentUserSession.User.CompanyID;
                lstMember = (from items in _entities.tblUserDepartments
                             where items.UserId != CurrentUserSession.UserID
                                   && items.CompanyID == companyID
                                && items.IsActive.HasValue && items.IsActive.Value
                             select new UserModel()
                             {
                                 FullName = items.FullName,
                                 UserId = items.UserId
                             }).ToList();
            }
            else
            {
                lstMember = (from items in _entities.tblUserDepartments
                             where items.UserId != CurrentUserSession.UserID
                             && items.CompanyID == CurrentUserSession.User.CompanyID
                             && items.IsActive.HasValue && items.IsActive.Value
                             select new UserModel()
                             {
                                 FullName = items.FullName,
                                 UserId = items.UserId
                             }).ToList();
            }

            foreach (var member in lstMember)
            {
                memberFileCollection.Add(new SelectListItem { Text = member.FullName, Value = member.UserId.ToString() });
            }

            return memberFileCollection;

        }

        public static ForgotPasswordModel CheckUserByUserNameEmail(string UserName, string Email)
        {
            using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
            {
                var objForgotPasswordModel = (from c in _entities.tblUserDepartments
                                              where c.UserName == UserName && c.EmailID == Email.Trim()
                                              select new ForgotPasswordModel
                                              {
                                                  UserID = c.UserId,
                                                  FirstName = c.FullName,
                                                  UserName = c.UserName,
                                                  Email = c.EmailID,
                                              }).FirstOrDefault();

                return objForgotPasswordModel;
            }
        }

        #region Get Template Content
        public static TemplateModel GetEmailTemplateContent(string TemplateName)
        {
            TemplateModel objTemplateModel = new TemplateModel();

            using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
            {
                var Templates = (from p in _entities.tblTemplateMasters
                                 where p.TemplateName.ToUpper() == TemplateName.ToUpper()
                                 && p.IsActive == true
                                 select p).FirstOrDefault();

                objTemplateModel.TemplateID = Templates.TemplateID;
                objTemplateModel.TemplateName = Templates.TemplateName;
                objTemplateModel.Subject = Templates.Subject;
                objTemplateModel.TemplateContentForEmail = Templates.TemplateContent;
            }

            return objTemplateModel;
        }
        #endregion

        public static byte[] GeneratePDF(string actionString, string AssignToName, AssignmentModel objAssignmentModel, List<AssignmentLogModel> lstAssignmentLogModel)
        {
            HomeController _HomeController = new HomeController();
                Document document = new Document();
                document.Info.Title = "Testing Email Template";

                #region DefineDocumentStyles

                const string fontName = "Arial";
                string logoPath = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/CSS/images/"), "logo.png");
                _HomeController.SendErrorToText(logoPath, "logoPath");
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

                style = document.Styles.AddStyle("TableImage", "Normal");
                style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
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
                paragraph = section.AddParagraph("Hi " + AssignToName, "Heading2");
                paragraph = section.AddParagraph(string.Empty, "Heading3");
                paragraph.Format.Borders.Bottom.Width = 0.1;

                section.AddParagraph(objAssignmentModel.TaskName, "Heading2");
                section.AddParagraph(actionString, "Heading3");
                paragraph = section.AddParagraph(string.Empty, "Heading3");
                paragraph.Format.Borders.Bottom.Width = 0.1;

                section.AddParagraph("Comment", "Heading2");
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

                foreach (AssignmentLogModel objAssignmentLogModel in lstAssignmentLogModel)
                {
                    row = table.AddRow();

                    cell = row.Cells[0];
                    cell.AddParagraph(objAssignmentLogModel.UserName);
                    cell.Borders.Width = 0.3;
                    cell.Format.Font.Size = 10;

                    cell = row.Cells[1];
                    cell.Borders.Width = 0.3;
                    cell.Format.Font.Size = 10;

                    cell.AddParagraph(objAssignmentLogModel.ActionString);

                    cell = row.Cells[2];
                    cell.AddParagraph(!string.IsNullOrEmpty(objAssignmentLogModel.Description) ? objAssignmentLogModel.Description : string.Empty);
                    cell.Borders.Width = 0.3;
                    cell.Format.Font.Size = 10;

                    cell = row.Cells[3];
                    cell.Borders.Width = 0.3;
                    cell.AddParagraph(objAssignmentLogModel.CreatedDateString);
                    cell.Format.Font.Size = 10;
                }

                document.LastSection.Add(table);
                paragraph.AddLineBreak();
                #endregion

                const short imgHeight = 300;
                const short imgWidth = 300;
                const short rowHeight = 25;

                #region "Area & Neighborhood Maps/Aerials"

                List<PPTModel> documentFileCollection = new List<PPTModel>();
                ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
                var AssignedSlides = from items in _entities.tblAssignedPPTSlides
                                     join assign in _entities.tblAssignments on items.AssignmentID equals assign.AssignmentID
                                     join doc in _entities.tblDocuments on assign.DocumentID equals doc.DocumentID
                                     //where assign.UserID == userId
                                     where assign.AssignmentID == objAssignmentModel.AssignID
                                     select new PPTModel
                                     {
                                         AssignedPPTSlideID = items.AssignedPPTSildeID,
                                         AssignedTaskID = items.AssignmentID,
                                         SlideName = items.SlideName,
                                         Action = assign.Action,
                                         OriginalDocumentName = doc.DocumentName,
                                         AssignedTo = assign.LockedByUserID,
                                         IsPPTModified = items.IsPPTModified,
                                         IsPPTApproved = items.IsPPTApproved,
                                         PPTRemarks = items.PPTRemarks != null ? items.PPTRemarks : "",
                                     };

                table = new MigraDoc.DocumentObjectModel.Tables.Table();
                table.Style = "TableContent";
                column = table.AddColumn(528);
                table.LeftPadding = 3;
                table.RightPadding = 0;

                paragraph = section.AddParagraph(string.Empty, "Heading3");

                row = table.AddRow();

                cell = row.Cells[0];
                cell.AddParagraph(string.Empty);
                cell.Shading.Color = grayBackGround;
                paragraph.AddLineBreak();
                paragraph = cell.AddParagraph("Slides");
                paragraph.Format.Font.Size = 10;

                cell.AddParagraph(string.Empty);

                document.LastSection.Add(table);

                documentFileCollection.AddRange(AssignedSlides.AsEnumerable().Select(i => new PPTModel()
                {
                    Action = i.Action,
                    AssignedPPTSlideID = i.AssignedPPTSlideID,
                    AssignedTaskID = i.AssignedTaskID,
                    OriginalDocumentName = i.OriginalDocumentName,
                    SlideName = i.SlideName,
                    //SlideLink = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + i.OriginalDocumentName + "/AssignedDoc/" + ((getAssigned) ? i.AssignedTaskID.ToString() + "_Copy_" + i.SlideName : ((i.IsPPTModified.HasValue && i.IsPPTModified == true) ? "New_" : string.Empty) + i.AssignedTaskID.ToString() + "_Copy_" + i.SlideName),
                    IsPPTModified = i.IsPPTModified,
                    IsPPTApproved = i.IsPPTApproved,
                    AssignedTo = i.AssignedTo,
                    PPTRemarks = i.PPTRemarks != null ? i.PPTRemarks : "",
                }).ToList());

                table = section.AddTable();
                table.Style = "TableImage";
                column = table.AddColumn(528);
                table.LeftPadding = 3;
                table.RightPadding = 0;

                paragraph = section.AddParagraph(string.Empty, "Heading3");

                foreach (var item in documentFileCollection)
                {
                    row = table.AddRow();
                    cell = row.Cells[0];
                    row.Height = rowHeight;
                    cell.AddParagraph(string.Empty);
                    paragraph.AddLineBreak();
                    paragraph = cell.AddParagraph(item.SlideName);
                    paragraph.Format.Font.Size = 10;

                    cell.AddParagraph(string.Empty);

                    var link = System.IO.Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, "Slides_" + item.OriginalDocumentName + "/AssignedDoc/" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName.Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention);
                    _HomeController.SendErrorToText(link, "link");

                if (System.IO.File.Exists(link))
                    {
                        row = table.AddRow();
                        cell = row.Cells[0];
                        row.Height = rowHeight;
                        cell.AddParagraph(string.Empty);
                        paragraph.AddLineBreak();
                        paragraph = cell.AddParagraph("Assigned");
                        paragraph.Format.Font.Size = 10;

                        cell.AddParagraph(string.Empty);

                        row = table.AddRow();
                        cell = row.Cells[0];
                        cell.Format.Alignment = ParagraphAlignment.Center;
                        row.Height = imgHeight;
                        Paragraph paragraph1 = cell.AddParagraph();
                        MigraDoc.DocumentObjectModel.Shapes.Image mapImage1 = paragraph1.AddImage(link);
                        paragraph1.Format.Alignment = ParagraphAlignment.Center;
                        mapImage1.Width = imgWidth;
                        mapImage1.Height = imgHeight;
                        mapImage1.WrapFormat.Style = WrapStyle.Through;
                        mapImage1.LineFormat.Color = Colors.Black;

                        cell.AddParagraph(string.Empty);

                        if (item.IsPPTModified.HasValue && item.IsPPTModified.Value == true)
                        {
                            var fileurl = System.IO.Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, "Slides_" + item.OriginalDocumentName + "/AssignedDoc/" + ("New_" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName));
                            _HomeController.SendErrorToText(fileurl, "fileurl");
                            if (System.IO.File.Exists(fileurl))
                            {
                                Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(fileurl);
                                item.ThumbnailLink += "<td class='popover-hide-xs'><div class='aniimated-thumbnials'>";

                                row = table.AddRow();
                                cell = row.Cells[0];
                                cell.Format.Alignment = ParagraphAlignment.Center;
                                row.Height = rowHeight;
                                cell.AddParagraph(string.Empty);
                                paragraph.AddLineBreak();
                                paragraph = cell.AddParagraph("Changed");
                                paragraph.Format.Font.Size = 10;

                                cell.AddParagraph(string.Empty);
                                for (int j = 0; j < presentation.Slides.Count; j++)
                                {
                                    var SlideCount = string.Empty;
                                    if (item.IsPPTModified.HasValue && item.IsPPTModified.Value == true)
                                        SlideCount = "(" + j + ")";
                                    var linkModified = System.IO.Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, "Slides_" + item.OriginalDocumentName + "/AssignedDoc/" + ("New_" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName.Split('.').FirstOrDefault() + SlideCount + WordAutomationDemo.Common.Global.ImageExportExtention));
                                    _HomeController.SendErrorToText(linkModified, "linkModified");
                                    if (System.IO.File.Exists(linkModified))
                                    {
                                        row = table.AddRow();
                                        cell = row.Cells[0];
                                        cell.Format.Alignment = ParagraphAlignment.Center;
                                        row.Height = imgHeight;
                                        Paragraph paragraph2 = cell.AddParagraph();
                                        MigraDoc.DocumentObjectModel.Shapes.Image mapImageModified = paragraph2.AddImage(linkModified);
                                        paragraph2.Format.Alignment = ParagraphAlignment.Center;
                                        mapImageModified.Width = imgWidth;
                                        mapImageModified.Height = imgHeight;
                                        mapImageModified.WrapFormat.Style = WrapStyle.Through;
                                        mapImageModified.LineFormat.Color = Colors.Black;

                                        cell.AddParagraph(string.Empty);
                                    }
                                }
                            }
                        }
                    }

                    //var fileurl = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + item.OriginalDocumentName + "/AssignedDoc/" + (item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName);
                    //if (System.IO.File.Exists(fileurl))
                    //{
                    //    Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation();
                    //    presentation.LoadFromFile(fileurl);
                    //    item.ThumbnailLink += "<td class='popover-hide-xs'><div class='aniimated-thumbnials'>";
                    //    for (int j = 0; j < presentation.Slides.Count; j++)
                    //    {
                    //        var SlideCount = string.Empty;
                    //        if (item.IsPPTModified.HasValue && item.IsPPTModified.Value == true)
                    //            SlideCount = "(" + j + ")";
                    //        //var fileurl = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + item.OriginalDocumentName + "/AssignedDoc/" + ("New_" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName);
                    //        var link = System.Web.HttpContext.Current.Server.MapPath("/ApplicationDocuments/PPTs/Slides_" + item.OriginalDocumentName + "/AssignedDoc/" + (item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName.Split('.').FirstOrDefault() + SlideCount + WordAutomationDemo.Common.Global.ImageExportExtention));
                    //        //string logoPath = System.Web.HttpContext.Current.Server.MapPath("/CSS/images/logo.png");
                    //        if (System.IO.File.Exists(link))
                    //        {

                    //            MigraDoc.DocumentObjectModel.Shapes.Image mapImage1 = cell.AddImage(link);
                    //            mapImage1.Width = imgWidth;
                    //            mapImage1.Height = imgHeight;
                    //            mapImage1.WrapFormat.Style = WrapStyle.Through;
                    //            mapImage1.LineFormat.Color = Colors.Black;
                    //        }
                    //    }
                    //}
                }

                #endregion

                //#region Add Footer
                //table = new MigraDoc.DocumentObjectModel.Tables.Table();
                //table.Style = "TableContent";
                //column = table.AddColumn(528);
                //table.LeftPadding = 3;
                //table.RightPadding = 0;
                //table.Format.Shading.Color = grayBackGround;
                //table.Format.Borders.Color = grayBackGround;

                //paragraph = section.AddParagraph(string.Empty, "Heading3");

                //row = table.AddRow();

                //cell = row.Cells[0];
                //cell.AddParagraph(string.Empty);
                //cell.Shading.Color = grayBackGround;
                //paragraph.AddLineBreak();
                //paragraph = cell.AddParagraph("©2016 Triyosoft");
                //paragraph.Format.Font.Size = 10;

                //cell.AddParagraph(string.Empty);

                //document.LastSection.Add(table);
                //#endregion

                #endregion

                #region "Generate Document"
                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                renderer.Document = document;
                renderer.RenderDocument();

                // Save the document...
                string filename = string.Format("{0}.pdf", "TaskDescription");

                string strPDFPath = System.IO.Path.Combine(DirectoryManagmentUtils.RootAppDocPath, filename);

                _HomeController.SendErrorToText(strPDFPath, "AttachFile");
                byte[] fileBytes = null;
                using (MemoryStream mStream = new MemoryStream())
                {
                    renderer.PdfDocument.Save(mStream);
                    fileBytes = mStream.ToArray();
                    //System.Web.HttpContext context = System.Web.HttpContext.Current;
                    //context.Response.Clear();
                    //context.Response.ClearHeaders();
                    //context.Response.ClearContent();
                    //context.Response.AppendHeader("content-length", fileBytes.Length.ToString());
                    //context.Response.ContentType = MimeMapping.GetMimeMapping(filename);
                    //context.Response.AppendHeader("content-disposition", "attachment; filename=" + filename);
                    //context.Response.BinaryWrite(fileBytes);
                    //context.Response.Flush();
                    //context.Response.End();
                    mStream.Dispose();
                    mStream.Flush();
                }
                return fileBytes;
            #endregion
        }

        public static bool IsTaskReassigned(int AssignmentId, int DocType, bool isAssignScreen = false)
        {
            bool isTaskChanged = false;
                using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                {
                    if (DocType == (int)Global.DocumentType.Xls)
                    {
                        isTaskChanged = _entities.tblAssignedExcelSheets.Where(e => e.AssignmentID == AssignmentId && e.IsGrayedOut == true && !(e.IsSheetApproved == true)).Any();
                    }
                    else if (DocType == (int)Global.DocumentType.Ppt)
                    {
                        isTaskChanged = _entities.tblAssignedPPTSlides.Where(e => e.AssignmentID == AssignmentId && e.IsGrayedOut.HasValue && e.IsGrayedOut.Value && !(e.IsPPTApproved.HasValue && e.IsPPTApproved.Value)).Any();
                    }
                    else if (isAssignScreen == true && DocType == 0)
                    {
                        isTaskChanged = _entities.tblAssignedWordPages.Where(e => e.AssignmentID == AssignmentId).Any();
                    }
                }
                return isTaskChanged;
        }

        public static string GetAssignmentStatus(int AssignmentId)
        {
            using(ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
            {
                int status = 0;
                if (AssignmentId != 0)
                {
                    var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == AssignmentId);
                    if (assignment != null)
                        status = (int)assignment.Status;
                }
                switch (status)
                {
                    case 1:
                        return "Not Started";
                    case 2:
                        return "In Progress";
                    case 3:
                        return "Completed";
                    case 4:
                        return "Overdue";
                    default:
                        return "";
                }
            }
        }

    }
}