using Aspose.Cells;
using DevExpress.Web.Office;
using DevExpress.XtraRichEdit;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Controllers
{
    /// <summary>
    /// Created By  : Ajay Vekariya
    /// Date : 17/02/2017
    /// </summary>
    [ValidateLogin]
    public class ProjectsController : BaseController
    {

        public ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
        int _Count = 0;

        public ProjectsController()
        {
            new Aspose.Cells.License().SetLicense("Aspose.Total.lic");
            new Aspose.Slides.License().SetLicense("Aspose.Total.lic");
        }

        #region Index

        [HttpGet]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Projects)]
        public ActionResult Index()
        {
            return View();
        }

        #endregion

        #region Archive

        [HttpGet]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Projects)]
        public ActionResult Archive()
        {
            return View();
        }

        #endregion

        #region Complete

        [HttpGet]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Projects)]
        public ActionResult Complete()
        {
            return View();
        }

        #endregion

        #region Create

        [HttpGet]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Projects)]
        public ActionResult Create()
        {
            var model = new ProjectModel();
            var companyID = CurrentUserSession.User.CompanyID;
            model.MemberList = (from items in _entities.tblUserDepartments
                                where items.UserId != CurrentUserSession.UserID
                                      && items.CompanyID == companyID
                                select new UserModel()
                                {
                                    FullName = items.FullName,
                                    UserId = items.UserId
                                }).ToList();
            model.StartDate = DateTime.Now;
            model.Status = (int)Global.ProjectStatus.Active;
            model.EndDate = DateTime.Now.AddDays(1);
            model.WordList = CommonHelper.GetDocFilesInDirectory(_entities);
            model.Members = CommonHelper.GetMembersList(_entities);
            return View(model);
        }

        [HttpPost]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Projects)]
        public ActionResult Create(ProjectModel model)
        {
            AssignmentModel assignmentModel = new AssignmentModel();
            HomeController _homeController = new HomeController();
            model.DocumentIDs = Convert.ToString(Request.Form["DocumentID"]);
            if (model.DocumentIDList != null)
            {
                model.DocumentIDs = string.Join(",", model.DocumentIDList);
            }

            HttpFileCollectionBase objHttpFileCollectionBase = Request.Files;
            model.StartDate = DateTime.ParseExact(model.strStartDate, WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid, CultureInfo.InvariantCulture);
            model.EndDate = DateTime.ParseExact(model.strEndDate, WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid, CultureInfo.InvariantCulture);
            if (ModelState.IsValid)
            {

                if (!IsDuplicateProject(model.Name, 0, CurrentUserSession.UserID))
                {

                    tblProject _project = new tblProject();
                    _project.Name = model.Name;
                    _project.StartDate = model.StartDate;
                    _project.EndDate = model.EndDate;
                    _project.Status = model.Status;
                    //_project.FileName = NewFileName;
                    _project.Description = model.Description;
                    _project.CreatedBy = CurrentUserSession.UserID;
                    _project.CreatedDate = DateTime.Now;
                    _entities.tblProjects.Add(_project);
                    if (model.MemberIDList != null)
                    {
                        foreach (var item in model.MemberIDList)
                        {
                            if (item > 0)
                            {
                                if (item != CurrentUserSession.UserID)
                                {
                                    tblProjectMember _member = new tblProjectMember();
                                    _member.ProjectID = _project.ProjectID;
                                    _member.UserID = item;
                                    _member.CreatedBy = CurrentUserSession.UserID;
                                    _member.CreatedDate = DateTime.Now;
                                    _entities.tblProjectMembers.Add(_member);
                                }
                            }
                        }
                    }
                    _entities.SaveChanges();
                    if (!string.IsNullOrEmpty(model.DocumentIDs))
                    {
                        //Add Contact Keywords
                        CommonHelper.AddProjectDocuments(_project.ProjectID, model.DocumentIDs, _entities, true);
                    }
                    assignmentModel.ProjectID = _project.ProjectID;
                }
                else
                {
                    model.MemberList = (from items in _entities.tblUserDepartments
                                        where items.UserId != CurrentUserSession.UserID
                                        select new UserModel()
                                        {
                                            FullName = items.FullName,
                                            UserId = items.UserId
                                        }).ToList();
                    model.StartDate = DateTime.Now;
                    model.Status = (int)Global.ProjectStatus.Active;
                    model.EndDate = DateTime.Now.AddDays(1);
                    model.WordList = CommonHelper.GetDocFilesInDirectory(_entities);
                    model.Members = CommonHelper.GetMembersList(_entities);
                    ModelState.AddModelError("Name", string.Format(Messages.AlreadyExists, "Project"));
                    return View(model);
                }
            }

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase docFile = Request.Files[i];
                var ts = DateTime.Now.Ticks.ToString();
                var FileExtention = (docFile != null && docFile.ContentLength > 0) ? docFile.FileName.Split('.').Last().ToLower() : string.Empty;
                var NewFileName = Path.GetFileName((docFile != null && docFile.ContentLength > 0) ? docFile.FileName.Substring(0, docFile.FileName.Length - 5) + "_" + ts + '.' + FileExtention : string.Empty);
                if (FileExtention == "ppt" || FileExtention == "xls" || FileExtention == "doc")
                    NewFileName = NewFileName + "x";
                //Save document if uploaded
                if (docFile != null && docFile.ContentLength > 0)
                {
                    string[] pptFiles = { "pptx", "ppt" };
                    string[] docFiles = { "docx", "doc" };
                    string[] xlsFiles = { "xlsx", "xls" };
                    System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.ProjectsDocumentFilesPath, assignmentModel.ProjectID.ToString()));
                    var ProjectFolderPath = Path.Combine(DirectoryManagmentUtils.ProjectsDocumentFilesPath, assignmentModel.ProjectID.ToString());
                    if (pptFiles.Contains(FileExtention))
                    {
                        var NewName = "Slides_" + Path.GetFileNameWithoutExtension(docFile.FileName) + "_" + ts + '.' + FileExtention;
                        var Foldername = NewName;

                        if (FileExtention == "ppt")
                        {
                            //crate directory
                            System.IO.Directory.CreateDirectory(Path.Combine(ProjectFolderPath, Foldername + "x"));
                            System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, Foldername + "x"));

                            Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(docFile.InputStream);
                            presentation.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + NewFileName, Aspose.Slides.Export.SaveFormat.Pptx); //Save File for PPT Folder 
                            presentation.Save(ProjectFolderPath + "/" + NewFileName, Aspose.Slides.Export.SaveFormat.Pptx);//Save File for Projects Folder 
                        }
                        else
                        {
                            //crate directory
                            System.IO.Directory.CreateDirectory(Path.Combine(ProjectFolderPath, Foldername));
                            System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, Foldername));

                            docFile.SaveAs(ProjectFolderPath + "/" + NewFileName); //Save File for Projects Folder 
                            docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + NewFileName); //Save File for PPT Folder 
                        }
                        _homeController.SplitSlides(NewFileName); //Split Sldies for PPT Folder 

                        SplitProjetcFileSlides(NewFileName, assignmentModel.ProjectID, true); //Split Sldies for Projects Folder 
                        tblDocument objtblDocument = CommonHelper.AddDocuments(docFile.FileName, NewFileName, (int)Global.DocumentType.Ppt, _entities);
                        CommonHelper.AddProjectDocuments(assignmentModel.ProjectID, objtblDocument.DocumentID.ToString(), _entities, true);
                        //TempData.Add("Document", NewFileName);
                    }
                    else if (xlsFiles.Contains(FileExtention))
                    {
                        var NewName = Path.GetFileNameWithoutExtension(docFile.FileName) + "_" + ts + '.' + FileExtention;
                        var Foldername = "Sheets_" + NewName;
                        Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, Foldername));

                        // PID the rows with row identifiers
                        using (var workbook = new Workbook(docFile.InputStream))
                        {
                            new HomeController().PidExcelRows(workbook, NewName);
                            workbook.Save(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, NewName), SaveFormat.Xlsx);
                        }

                        _homeController.SplitSheets(NewName);
                        tblDocument objtblDocument = CommonHelper.AddDocuments(Path.GetFileName(docFile.FileName), NewName, (int)Global.DocumentType.Xls, _entities);
                        CommonHelper.AddProjectDocuments(assignmentModel.ProjectID, objtblDocument.DocumentID.ToString(), _entities, true);
                    }
                    else if (docFiles.Contains(FileExtention))
                    {
                        docFile.SaveAs(ProjectFolderPath + "/" + NewFileName); //Save File for Projects Folder 


                        if (FileExtention == "doc") // if old document file  then convert it to docx 
                        {
                            NewFileName = NewFileName + "x";
                            RichEditDocumentServer server = new RichEditDocumentServer();
                            server.Document.LoadDocument(docFile.InputStream, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                            server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + NewFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                            server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + NewFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                            DocumentManager.CloseAllDocuments();
                        }
                        else
                        {
                            docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + NewFileName); //Save File for General Folder 
                            docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + NewFileName); //Save File Copy for General Folder 
                        }
                        tblDocument objtblDocument = CommonHelper.AddDocuments(docFile.FileName, NewFileName, (int)Global.DocumentType.Word, _entities);
                        CommonHelper.AddProjectDocuments(assignmentModel.ProjectID, objtblDocument.DocumentID.ToString(), _entities, true);
                        //TempData.Add("Document", NewFileName);
                    }
                }
            }

            TempData.Add("ProjectCreated", Messages.ProjectCreated.ToString());
            return RedirectToAction("CreateAssignment", "Home", new { assignmentModel.ProjectID });
        }


        #endregion

        [HttpPost]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Projects)]
        public ActionResult UploadDocuments()
        {
            int projectID = Convert.ToInt32(Request.Form["hdnProjectID"]);
            if (projectID > 0)
            {
                HomeController _homeController = new HomeController();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase docFile = Request.Files[i];
                    var ts = DateTime.Now.Ticks.ToString();
                    var FileExtention = (docFile != null && docFile.ContentLength > 0) ? docFile.FileName.Split('.').Last().ToLower() : string.Empty;
                    var NewFileName = (docFile != null && docFile.ContentLength > 0) ? docFile.FileName.Substring(0, docFile.FileName.Length - 5) + "_" + ts + '.' + FileExtention : string.Empty;
                    if (FileExtention == "ppt")
                        NewFileName = NewFileName + "x";
                    //Save document if uploaded
                    if (docFile != null && docFile.ContentLength > 0)
                    {
                        string[] pptFiles = { "pptx", "ppt" };
                        string[] docFiles = { "docx", "doc" };
                        string[] xlsFiles = { "xlsx", "xls" };
                        System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.ProjectsDocumentFilesPath, projectID.ToString()));
                        var ProjectFolderPath = Path.Combine(DirectoryManagmentUtils.ProjectsDocumentFilesPath, projectID.ToString());
                        if (pptFiles.Contains(FileExtention))
                        {
                            var NewName = "Slides_" + docFile.FileName.Substring(0, docFile.FileName.Length - 5) + "_" + ts + '.' + FileExtention;
                            var Foldername = NewName;

                            if (FileExtention == "ppt")
                            {
                                //crate directory
                                System.IO.Directory.CreateDirectory(Path.Combine(ProjectFolderPath, Foldername + "x"));
                                System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, Foldername + "x"));

                                Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(docFile.InputStream);
                                presentation.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + NewFileName, Aspose.Slides.Export.SaveFormat.Pptx); //Save File for PPT Folder 
                                presentation.Save(ProjectFolderPath + "/" + NewFileName, Aspose.Slides.Export.SaveFormat.Pptx);//Save File for Projects Folder 
                            }
                            else
                            {
                                //crate directory
                                System.IO.Directory.CreateDirectory(Path.Combine(ProjectFolderPath, Foldername));
                                System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, Foldername));

                                docFile.SaveAs(ProjectFolderPath + "/" + NewFileName); //Save File for Projects Folder 
                                docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + NewFileName); //Save File for PPT Folder 
                            }
                            _homeController.SplitSlides(NewFileName); //Split Sldies for PPT Folder 

                            SplitProjetcFileSlides(NewFileName, projectID, true); //Split Sldies for Projects Folder 
                            tblDocument objtblDocument = CommonHelper.AddDocuments(docFile.FileName, NewFileName, (int)Global.DocumentType.Ppt, _entities);
                            CommonHelper.AddProjectDocuments(projectID, objtblDocument.DocumentID.ToString(), _entities, true);
                            //TempData.Add("Document", NewFileName);
                        }
                        else if (xlsFiles.Contains(FileExtention))
                        {
                            var NewName = "Sheets_" + docFile.FileName.Substring(0, docFile.FileName.Length - 5) + "_" + ts + '.' + FileExtention;
                            var Foldername = NewName;

                            using (var workbook = new Workbook(docFile.InputStream))
                            {
                                new HomeController().PidExcelRows(workbook, NewName);
                                System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, Foldername));
                                docFile.SaveAs(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, NewName));
                            }
                            _homeController.SplitSheets(NewFileName);
                            tblDocument objtblDocument = CommonHelper.AddDocuments(docFile.FileName, NewFileName, (int)Global.DocumentType.Xls, _entities);
                            CommonHelper.AddProjectDocuments(projectID, objtblDocument.DocumentID.ToString(), _entities, true);
                        }
                        else if (docFiles.Contains(FileExtention))
                        {
                            docFile.SaveAs(ProjectFolderPath + "/" + NewFileName); //Save File for Projects Folder 


                            if (FileExtention == "doc") // if old document file  then convert it to docx 
                            {
                                NewFileName = NewFileName + "x";
                                RichEditDocumentServer server = new RichEditDocumentServer();
                                server.Document.LoadDocument(docFile.InputStream, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                                server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + NewFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                                server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + NewFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                                DocumentManager.CloseAllDocuments();
                            }
                            else
                            {
                                docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + NewFileName); //Save File for General Folder 
                                docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + NewFileName); //Save File Copy for General Folder 
                            }
                            tblDocument objtblDocument = CommonHelper.AddDocuments(docFile.FileName, NewFileName, (int)Global.DocumentType.Word, _entities);
                            CommonHelper.AddProjectDocuments(projectID, objtblDocument.DocumentID.ToString(), _entities, true);
                            //TempData.Add("Document", NewFileName);
                        }
                    }
                }
                return RedirectToAction("Index", "Projects", new { Msg = "Added" });
            }
            else
            {
                return RedirectToAction("Index", "Projects", new { Msg = "Error" });
            }
        }

        #region Edit

        [HttpGet]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Projects)]
        public ActionResult Edit()
        {

            if (Request.QueryString["UID"] == null)
            {
                return RedirectToAction("Index", "Projects");
            }
            else
            {
                string UID = Global.UrlDecrypt(Request.QueryString["UID"]);
                int projectID;
                if (int.TryParse(UID, out projectID))
                {
                    if (projectID > 0)
                    {
                        var model = new ProjectModel();
                        var UserID = CurrentUserSession.UserID;
                        ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
                        var Project = (from item in _entities.tblProjects
                                       where item.IsDeleted != true /*&& item.CreatedBy == UserID*/ && item.ProjectID == projectID
                                       orderby item.CreatedDate descending
                                       select new ProjectModel()
                                       {
                                           ProjectID = item.ProjectID,
                                           Name = item.Name,
                                           StartDate = item.StartDate,
                                           EndDate = item.EndDate,
                                           CreatedBy = item.CreatedBy,
                                           CreatedByName = item.tblUserDepartment1.FullName,
                                           Description = item.Description,
                                           CreatedDate = item.CreatedDate,
                                           FileName = item.FileName,
                                           Status = item.Status,
                                           IsDeleted = item.IsDeleted,
                                       }).FirstOrDefault();
                        if (Project != null && Project.ProjectID > 0)
                        {
                            Project.strStartDate = Project.StartDate.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid);
                            Project.strEndDate = Project.EndDate.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid);
                            Project.MemberList = (from items in _entities.tblUserDepartments
                                                  where items.UserId != CurrentUserSession.UserID
                                                  select new UserModel()
                                                  {
                                                      FullName = items.FullName,
                                                      UserId = items.UserId,
                                                      isSelectedForProject = _entities.tblProjectMembers.Where(x => x.ProjectID == Project.ProjectID && x.UserID == items.UserId).Any()
                                                  }).ToList();
                            Project.WordList = CommonHelper.GetDocFilesInDirectory(_entities);
                            Project.Members = CommonHelper.GetMembersList(_entities);

                            var lstDocument = (from items in _entities.tblDocuments
                                               join proj in _entities.tblProjectDocuments on items.DocumentID equals proj.DocumentID
                                               where proj.ProjectID == Project.ProjectID && proj.IsActive == true
                                               select items).ToList();

                            Project.ProjectDocuments = lstDocument.Select(d => new SelectListItem
                            {
                                Value = d.DocumentID.ToString(),
                                Text = d.DocumentName
                            }).ToList();

                            Project.MembersSelected = Project.MemberList.Where(a => a.isSelectedForProject).Select(d => new SelectListItem
                            {
                                Value = d.UserId.ToString(),
                                Text = d.FullName
                            }).ToList();



                            return View(Project);
                        }
                        else
                            return RedirectToAction("Index", "Projects", new { Msg = "ProjectNotFound" });

                    }
                    else
                        return RedirectToAction("Index", "Projects", new { Msg = "ProjectNotFound" });
                }
                else
                {
                    return RedirectToAction("Index", "Projects", new { Msg = "ProjectNotFound" });
                }
            }

        }

        [HttpPost]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Projects)]
        public ActionResult Edit(ProjectModel model)
        {
            HomeController _homeController = new HomeController();
            HttpFileCollectionBase objHttpFileCollectionBase = Request.Files;
            model.DocumentIDs = Convert.ToString(Request.Form["DocumentID"]);
                model.StartDate = DateTime.ParseExact(model.strStartDate, WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid, CultureInfo.InvariantCulture);
                model.EndDate = DateTime.ParseExact(model.strEndDate, WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid, CultureInfo.InvariantCulture);

                if (ModelState.IsValid && model.ProjectID > 0)
                {

                    if (!IsDuplicateProject(model.Name, model.ProjectID, CurrentUserSession.UserID))
                    {
                        tblProject _project = _entities.tblProjects.Where(x => x.ProjectID == model.ProjectID).FirstOrDefault();
                        if (_project != null && _project.ProjectID > 0)
                        {
                            _project.Name = model.Name;
                            _project.Description = model.Description;
                            _project.StartDate = model.StartDate;
                            _project.EndDate = model.EndDate;
                            //_project.FileName = (!string.IsNullOrEmpty(NewFileName)) ? NewFileName : _project.FileName;
                            _project.ModifiedBy = CurrentUserSession.UserID;
                            _project.ModifiedDate = DateTime.Now;
                            _project.Status = model.Status;
                            _entities.Entry(_project).State = System.Data.Entity.EntityState.Modified;
                        }
                        var _members = _entities.tblProjectMembers.Where(x => x.ProjectID == model.ProjectID).ToList();
                        if (_members != null && _members.Count > 0)
                        {
                            _entities.tblProjectMembers.RemoveRange(_members);
                        }
                        if (model.MemberIDList != null)
                        {
                            foreach (var item in model.MemberIDList)
                            {
                                if (item > 0)
                                {
                                    if (item != CurrentUserSession.UserID)
                                    {
                                        tblProjectMember _member = new tblProjectMember();
                                        _member.ProjectID = model.ProjectID;
                                        _member.UserID = item;
                                        _member.CreatedBy = CurrentUserSession.UserID;
                                        _member.CreatedDate = DateTime.Now;
                                        _entities.tblProjectMembers.Add(_member);
                                    }
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(model.DocumentIDs))
                        {
                            CommonHelper.AddProjectDocuments(model.ProjectID, model.DocumentIDs, _entities, false);
                        }
                        _entities.SaveChanges();
                    }
                    else
                    {
                        model.strStartDate = model.StartDate.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid);
                        model.strEndDate = model.EndDate.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid);
                        model.MemberList = (from items in _entities.tblUserDepartments
                                              where items.UserId != CurrentUserSession.UserID
                                              select new UserModel()
                                              {
                                                  FullName = items.FullName,
                                                  UserId = items.UserId,
                                                  isSelectedForProject = _entities.tblProjectMembers.Where(x => x.ProjectID == model.ProjectID && x.UserID == items.UserId).Any()
                                              }).ToList();
                        model.WordList = CommonHelper.GetDocFilesInDirectory(_entities);
                        model.Members = CommonHelper.GetMembersList(_entities);

                        var lstDocument = (from items in _entities.tblDocuments
                                           join proj in _entities.tblProjectDocuments on items.DocumentID equals proj.DocumentID
                                           where proj.ProjectID == model.ProjectID && proj.IsActive == true
                                           select items).ToList();

                        model.ProjectDocuments = lstDocument.Select(d => new SelectListItem
                        {
                            Value = d.DocumentID.ToString(),
                            Text = d.DocumentName
                        }).ToList();

                        model.MembersSelected = model.MemberList.Where(a => a.isSelectedForProject).Select(d => new SelectListItem
                        {
                            Value = d.UserId.ToString(),
                            Text = d.FullName
                        }).ToList();
                        ModelState.AddModelError("Name", string.Format(Messages.AlreadyExists, "Project"));
                        return View(model);
                    }

                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase docFile = Request.Files[i];
                        var ts = DateTime.Now.Ticks.ToString();
                        var FileExtention = (docFile != null && docFile.ContentLength > 0) ? docFile.FileName.Split('.').Last().ToLower() : string.Empty;
                        var NewFileName = (docFile != null && docFile.ContentLength > 0) ? docFile.FileName.Substring(0, docFile.FileName.Length - 5) + "_" + ts + '.' + FileExtention : string.Empty;
                        if (FileExtention == "ppt")
                            NewFileName = NewFileName + "x";
                        //Save document if uploaded
                        if (docFile != null && docFile.ContentLength > 0)
                        {
                            string[] pptFiles = { "pptx", "ppt" };
                            string[] docFiles = { "docx", "doc" };
                            string[] xlsFiles = { "xlsx", "xls" };
                            System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.ProjectsDocumentFilesPath, model.ProjectID.ToString()));
                            var ProjectFolderPath = Path.Combine(DirectoryManagmentUtils.ProjectsDocumentFilesPath, model.ProjectID.ToString());
                            if (pptFiles.Contains(FileExtention))
                            {
                                var NewName = "Slides_" + docFile.FileName.Substring(0, docFile.FileName.Length - 5) + "_" + ts + '.' + FileExtention;
                                var Foldername = NewName;

                                if (FileExtention == "ppt")
                                {
                                    //crate directory
                                    System.IO.Directory.CreateDirectory(Path.Combine(ProjectFolderPath, Foldername + "x"));
                                    System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, Foldername + "x"));

                                    Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(docFile.InputStream);
                                    presentation.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + NewFileName, Aspose.Slides.Export.SaveFormat.Pptx); //Save File for PPT Folder 
                                    presentation.Save(ProjectFolderPath + "/" + NewFileName, Aspose.Slides.Export.SaveFormat.Pptx);//Save File for Projects Folder 
                                }
                                else
                                {
                                    //crate directory
                                    System.IO.Directory.CreateDirectory(Path.Combine(ProjectFolderPath, Foldername));
                                    System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, Foldername));

                                    docFile.SaveAs(ProjectFolderPath + "/" + NewFileName); //Save File for Projects Folder 
                                    docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + NewFileName); //Save File for PPT Folder 
                                }
                                _homeController.SplitSlides(NewFileName); //Split Sldies for PPT Folder 

                                SplitProjetcFileSlides(NewFileName, model.ProjectID, true); //Split Sldies for Projects Folder 
                                tblDocument objtblDocument = CommonHelper.AddDocuments(docFile.FileName, NewFileName, (int)Global.DocumentType.Ppt, _entities);
                                CommonHelper.AddProjectDocuments(model.ProjectID, objtblDocument.DocumentID.ToString(), _entities, false);
                                //TempData.Add("Document", NewFileName);
                            }
                            else if (xlsFiles.Contains(FileExtention))
                            {
                                var NewName = "Sheets_" + docFile.FileName.Substring(0, docFile.FileName.Length - 5) + "_" + ts + '.' + FileExtention;
                                var Foldername = NewName;


                                using (var workbook = new Workbook(docFile.InputStream))
                                {
                                    new HomeController().PidExcelRows(workbook, NewName);
                                    Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, Foldername + "x"));
                                    workbook.Save(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, NewFileName), SaveFormat.Xlsx);
                                }
                                _homeController.SplitSheets(NewFileName);
                                tblDocument objtblDocument = CommonHelper.AddDocuments(docFile.FileName, NewFileName, (int)Global.DocumentType.Xls, _entities);
                                CommonHelper.AddProjectDocuments(model.ProjectID, objtblDocument.DocumentID.ToString(), _entities, true);
                            }
                            else if (docFiles.Contains(FileExtention))
                            {
                                string newName = string.Empty;
                                var newFile = docFile.FileName.Split('\\');
                                if (newFile != null && newFile.Length > 0)
                                {
                                    newName = newFile.Last();
                                    NewFileName = newName.Substring(0, newName.Length - 5) + "_" + ts + '.' + FileExtention;
                                }

                                docFile.SaveAs(ProjectFolderPath + "/" + NewFileName); //Save File for Projects Folder 


                                if (FileExtention == "doc") // if old document file  then convert it to docx 
                                {
                                    NewFileName = NewFileName + "x";
                                    RichEditDocumentServer server = new RichEditDocumentServer();
                                    server.Document.LoadDocument(docFile.InputStream, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                                    server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + NewFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                                    server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + NewFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                                    DocumentManager.CloseAllDocuments();
                                }
                                else
                                {
                                    docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + NewFileName); //Save File for General Folder 
                                    docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + newName); //Save File Copy for General Folder 
                                }
                                tblDocument objtblDocument = CommonHelper.AddDocuments(newName, NewFileName, (int)Global.DocumentType.Word, _entities);
                                CommonHelper.AddProjectDocuments(model.ProjectID, objtblDocument.DocumentID.ToString(), _entities, false);
                                //TempData.Add("Document", NewFileName);
                            }
                        }
                    }

                    //Save document if uploaded
                    //if (docFile != null && docFile.ContentLength > 0)
                    //{
                    //    string[] pptFiles = { "pptx", "ppt" };
                    //    string[] docFiles = { "docx", "doc" };
                    //    System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.ProjectsDocumentFilesPath, id.ToString()));
                    //    var ProjectFolderPath = Path.Combine(DirectoryManagmentUtils.ProjectsDocumentFilesPath, id.ToString());
                    //    //clean previous files
                    //    try
                    //    {
                    //        if (Directory.Exists(ProjectFolderPath))
                    //        {
                    //            Directory.Delete(ProjectFolderPath, true);
                    //        }
                    //    }
                    //    catch
                    //    {
                    //    }

                    //    Directory.CreateDirectory(ProjectFolderPath);

                    //    if (pptFiles.Contains(FileExtention))
                    //    {
                    //        var NewName = "Slides_" + docFile.FileName.Substring(0, docFile.FileName.Length - 5) + "_" + ts + '.' + FileExtention;
                    //        var Foldername = NewName;

                    //        if (FileExtention == "ppt")
                    //        {
                    //            //crate directory
                    //            System.IO.Directory.CreateDirectory(Path.Combine(ProjectFolderPath, Foldername + "x"));

                    //            Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation();
                    //            presentation.LoadFromStream(docFile.InputStream, FileFormat.Auto);
                    //            presentation.SaveToFile(ProjectFolderPath + "/" + NewFileName, Aspose.Slides.Export.SaveFormat.Pptx);
                    //            SplitProjetcFileSlides(NewFileName, id, false);
                    //        }
                    //        else
                    //        {
                    //            //crate directory
                    //            System.IO.Directory.CreateDirectory(Path.Combine(ProjectFolderPath, Foldername));
                    //            docFile.SaveAs(ProjectFolderPath + "/" + NewFileName);
                    //        }
                    //        TempData.Add("Document", NewFileName);
                    //    }
                    //    else if (docFiles.Contains(FileExtention))
                    //    {
                    //        docFile.SaveAs(ProjectFolderPath + "/" + NewFileName);
                    //    }
                    //}
                    return RedirectToAction("Index", "Projects", new { Msg = "ProjectEdited" });
                }
                else
                {
                    ModelState.AddModelError("", Messages.ProvideAllFields.ToString());
                    return View(model);
                }
        }

        #endregion

        #region Delete

        [HttpPost]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Projects)]
        public ActionResult Delete(int[] chkDelete)
        {
            if (chkDelete.Length > 0)
            {

                foreach (var item in chkDelete)
                {
                    var assignmentIds = _entities.tblAssignments.Where(a => a.ProjectID == item).Select(a => a.AssignmentID);

                    _entities.tblAssignmentLogs.RemoveRange(_entities.tblAssignmentLogs.Where(al => assignmentIds.Contains(al.AssignmentID.Value)).AsEnumerable());

                    _entities.tblAssignedPPTSlides.RemoveRange(_entities.tblAssignedPPTSlides.Where(al => assignmentIds.Contains(al.AssignmentID)).AsEnumerable());

                    _entities.tblAssignedExcelSheets.RemoveRange(_entities.tblAssignedExcelSheets.Where(al => assignmentIds.Contains(al.AssignmentID)).AsEnumerable());

                    _entities.tblExcelRowMaps.RemoveRange(_entities.tblExcelRowMaps.Where(ex => assignmentIds.Contains(ex.AssignmentID)).AsEnumerable());

                    _entities.tblAssignedWordPages.RemoveRange(_entities.tblAssignedWordPages.Where(al => assignmentIds.Contains(al.AssignmentID.Value)).AsEnumerable());

                    _entities.tblAssignmentMembers.RemoveRange(_entities.tblAssignmentMembers.Where(am => assignmentIds.Contains(am.AssignmentID.Value)).AsEnumerable());

                    _entities.tblAssignments.RemoveRange(_entities.tblAssignments.Where(a => a.ProjectID == item));

                    _entities.tblProjectMembers.RemoveRange(_entities.tblProjectMembers.Where(a => a.ProjectID == item));

                    _entities.tblProjectDocuments.RemoveRange(_entities.tblProjectDocuments.Where(a => a.ProjectID == item));

                    var ProjectFolderPath = Path.Combine(DirectoryManagmentUtils.ProjectsDocumentFilesPath, item.ToString());
                    //clean previous files
                    try
                    {
                        if (Directory.Exists(ProjectFolderPath))
                        {
                            Directory.Delete(ProjectFolderPath, true);
                        }
                    }
                    catch
                    {
                    }

                    _entities.tblProjects.RemoveRange(_entities.tblProjects.Where(a => a.ProjectID == item));

                    //itemTodelete.IsDeleted = true;
                    //_entities.Entry(itemTodelete).State = System.Data.Entity.EntityState.Modified;
                }
                _entities.SaveChanges();
                return RedirectToAction("Index", "Projects", new { Msg = "deleted" });
            }
            else
            {
                return RedirectToAction("Index", "Projects", new { Msg = "NoSelect" });
            }
        }

        #endregion

        #region Details

        [HttpGet]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Projects)]
        public ActionResult Details()
        {
            if (Request.QueryString["UID"] == null)
            {
                return RedirectToAction("Index", "Projects");
            }
            else
            {
                string UID = Global.UrlDecrypt(Request.QueryString["UID"]);
                int projectID;
                if (int.TryParse(UID, out projectID))
                {
                    if (projectID > 0)
                    {
                        var model = new ProjectModel();
                        var UserID = CurrentUserSession.UserID;
                        ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
                        var Project = (from item in _entities.tblProjects
                                       join user in _entities.tblUserDepartments on item.CreatedBy equals user.UserId
                                       where item.IsDeleted != true && item.CreatedBy == UserID && item.ProjectID == projectID
                                       orderby item.CreatedDate descending
                                       select new ProjectModel()
                                       {
                                           ProjectID = item.ProjectID,
                                           Name = item.Name,
                                           StartDate = item.StartDate,
                                           EndDate = item.EndDate,
                                           CreatedBy = item.CreatedBy,
                                           CreatedByName = user.FullName,
                                           Description = item.Description,
                                           CreatedDate = item.CreatedDate,
                                           Status = item.Status,
                                           FileName = item.FileName,
                                           IsDeleted = item.IsDeleted,
                                       }).FirstOrDefault();
                        if (Project != null && Project.ProjectID > 0)
                        {
                            Project.strStartDate = Project.StartDate.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid);
                            Project.strEndDate = Project.EndDate.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid);
                            Project.MemberList = (from items in _entities.tblUserDepartments
                                                  select new UserModel()
                                                  {
                                                      FullName = items.FullName,
                                                      UserId = items.UserId,
                                                      isSelectedForProject = _entities.tblProjectMembers.Where(x => x.ProjectID == Project.ProjectID && x.UserID == items.UserId).Any()
                                                  }).Where(x => x.isSelectedForProject).ToList();

                            return View(Project);
                        }
                        else
                            return RedirectToAction("Index", "Projects", new { Msg = "ProjectNotFound" });

                    }
                    else
                        return RedirectToAction("Index", "Projects", new { Msg = "ProjectNotFound" });
                }
                else
                {
                    return RedirectToAction("Index", "Projects", new { Msg = "ProjectNotFound" });
                }
            }
        }

        #endregion

        #region Methods

        //Load Document to Popup 
        public ActionResult LoadProjectDoc(int ProjectID = 0, string FileName = "")
        {
            var demoModel = new ProjectModel() { FileName = FileName, ProjectID = ProjectID };
            return PartialView("_LoadProjectDoc", demoModel);
        }


        public ActionResult GetAllSlidesForProjectDoc(string FileName = "", int ProjectID = 0)
        {
            List<PPTModel> documentFileCollection = new List<PPTModel>();
            if (!string.IsNullOrEmpty(FileName.Trim()) && ProjectID > 0)
            {
                var AllSlides = from items in _entities.tblPPTSlides
                                where items.MasterDocumentName == FileName && items.IsOriginal == true
                                orderby items.Sequence
                                select items;

                documentFileCollection.AddRange(AllSlides.AsEnumerable().Select(i => new PPTModel()
                {
                    Sequence = i.Sequence,
                    OriginalDocumentName = i.MasterDocumentName,
                    SlideName = i.SlideName,
                    SlideLink = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/ProjectDocuments/" + ProjectID.ToString() + "/Slides_" + FileName + "/" + i.SlideName.ToString(),
                }).ToList());


                foreach (var item in documentFileCollection)
                {

                    var link = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/ProjectDocuments/" + ProjectID.ToString() + "/Slides_" + FileName + "/" + item.SlideName.Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention;
                    item.ThumbnailLink += "   <img onmouseover='ShowImg(this);' onmouseout='HideImg(this);' style='border : 1px solid;' class='names' src='" + link + "' height=50px width=50px data-id='" + link + "'/>";

                }
            }


            var result = new DataSourceResult()
            {
                Data = documentFileCollection,
                Total = documentFileCollection.Count // Total number of records
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveNewProject(string Name, string StartDate, string EndDate, string Description, string[] Members)
        {
            if (!string.IsNullOrEmpty(Name.Trim()) && !string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {


                if (!IsDuplicateProject(Name, 0, CurrentUserSession.UserID))
                {
                    var dtStartDate = DateTime.ParseExact(StartDate, WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid, CultureInfo.InvariantCulture);
                    var dtEndDate = DateTime.ParseExact(EndDate, WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid, CultureInfo.InvariantCulture);

                    tblProject _project = new tblProject();
                    _project.Name = Name.Trim();
                    _project.StartDate = dtStartDate;
                    _project.EndDate = dtEndDate;
                    _project.Status = (int)Global.ProjectStatus.Active;
                    _project.Description = Description;
                    _project.CreatedBy = CurrentUserSession.UserID;
                    _project.CreatedDate = DateTime.Now;
                    _entities.tblProjects.Add(_project);
                    if (Members != null)
                    {
                        foreach (var item in Members)
                        {
                            var Intitem = int.Parse(item);
                            //var Intitem = item;
                            if (Intitem > 0)
                            {
                                if (Intitem != CurrentUserSession.UserID)
                                {
                                    tblProjectMember _member = new tblProjectMember();
                                    _member.UserID = Intitem;
                                    _member.CreatedBy = CurrentUserSession.UserID;
                                    _member.CreatedDate = DateTime.Now;
                                    _entities.tblProjectMembers.Add(_member);
                                }
                            }
                        }
                    }
                    _entities.SaveChanges();
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(2, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(3, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckDuplication(string Name, int ProjectId)
        {
            try
            {
                var userId = CurrentUserSession.UserID;
                if (!string.IsNullOrEmpty(Name))
                {
                    var response = IsDuplicateProject(Name, ProjectId, userId);
                    return Json((response) ? 1 : 0, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(2, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(3, JsonRequestBehavior.AllowGet);

            }
        }

        public bool IsDuplicateProject(string Name, int ProjectId, int userId)
        {
            bool isDuplicateRec = (from items in _entities.tblProjects
                                    where items.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)
                                    && (ProjectId == 0 || items.ProjectID != ProjectId)
                                    && items.CreatedBy == userId
                                    && items.IsDeleted != true
                                    select items).Any();
            return isDuplicateRec;
        }

        public ActionResult GetAllProjects([DataSourceRequest]DataSourceRequest command)
        {
            var UserID = CurrentUserSession.UserID;
            ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
            var myProjectIds = _entities.tblProjectMembers.Where(pm => pm.UserID == UserID).Select(p => p.ProjectID).ToList();
            var ProjectList = _entities.tblProjects.Where(p => !p.IsDeleted 
                                                            && p.Status == (int)Global.ProjectStatus.Active 
                                                            && (myProjectIds.Contains(p.ProjectID) || p.CreatedBy == UserID))
                .OrderBy( p => p.CreatedDate)
                .Select(item => new ProjectModel()
                {
                    ProjectID = item.ProjectID,
                    Name = item.Name,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    FileName = item.FileName,
                    CreatedBy = item.CreatedBy,
                    CreatedByName = item.tblUserDepartment1.FullName,
                    Description = item.Description,
                    CreatedDate = item.CreatedDate,
                    Status = item.Status,
                    IsDeleted = item.IsDeleted,
                }).ToList().AsQueryable();

            //   Apply filtering
            ProjectList = ProjectList.ApplyFiltering(command.Filters);

            //Get count of total records
            if (ProjectList != null)
            {
                _Count = ProjectList.Count();
            }

            //Apply sorting
            ProjectList = ProjectList.ApplySorting(command.Groups, command.Sorts);

            //Apply paging
            ProjectList = ProjectList.ApplyPaging(command.Page, command.PageSize);

            foreach (var project in ProjectList)
            {
                project.EncryptedProjectID = Global.UrlEncrypt(project.ProjectID.ToString());
            }

            var result = new DataSourceResult()
            {
                Data = ProjectList,
                Total = _Count // Total number of records
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetArchiveProjects([DataSourceRequest]DataSourceRequest command)
        {
            var UserID = CurrentUserSession.UserID;
            ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
            var myProjectIds = _entities.tblProjectMembers.Where(pm => pm.UserID == UserID).Select(p => p.ProjectID).ToList();
            var ProjectList = _entities.tblProjects.Where(p => !p.IsDeleted
                                                               && p.Status == (int)Global.ProjectStatus.InActive
                                                               && (myProjectIds.Contains(p.ProjectID) || p.CreatedBy == UserID))
                .OrderBy(p => p.CreatedDate)
                .Select(item => new ProjectModel()
                {
                    ProjectID = item.ProjectID,
                    Name = item.Name,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    FileName = item.FileName,
                    CreatedBy = item.CreatedBy,
                    CreatedByName = item.tblUserDepartment1.FullName,
                    Description = item.Description,
                    CreatedDate = item.CreatedDate,
                    Status = item.Status,
                    IsDeleted = item.IsDeleted,
                }).ToList().AsQueryable();

            //   Apply filtering
            ProjectList = ProjectList.ApplyFiltering(command.Filters);

            //Get count of total records
            if (ProjectList != null)
            {
                _Count = ProjectList.Count();
            }

            //Apply sorting
            ProjectList = ProjectList.ApplySorting(command.Groups, command.Sorts);

            //Apply paging
            ProjectList = ProjectList.ApplyPaging(command.Page, command.PageSize);

            foreach (var project in ProjectList)
            {
                project.EncryptedProjectID = Global.UrlEncrypt(project.ProjectID.ToString());
            }

            var result = new DataSourceResult()
            {
                Data = ProjectList,
                Total = _Count // Total number of records
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCompletedProjects([DataSourceRequest]DataSourceRequest command)
        {
            var UserID = CurrentUserSession.UserID;
            ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
            var ProjectList = (from item in _entities.tblProjects
                                where item.Status == (int)Global.ProjectStatus.Completed && item.CreatedBy == UserID
                                orderby item.CreatedDate descending
                                select new ProjectModel()
                                {
                                    ProjectID = item.ProjectID,
                                    Name = item.Name,
                                    StartDate = item.StartDate,
                                    EndDate = item.EndDate,
                                    FileName = item.FileName,
                                    CreatedBy = item.CreatedBy,
                                    CreatedByName = item.tblUserDepartment1.FullName,
                                    Description = item.Description,
                                    CreatedDate = item.CreatedDate,
                                    Status = item.Status,
                                    IsDeleted = item.IsDeleted,
                                }).ToList().AsQueryable();

            //   Apply filtering
            ProjectList = ProjectList.ApplyFiltering(command.Filters);

            //Get count of total records
            if (ProjectList != null)
            {
                _Count = ProjectList.Count();
            }

            //Apply sorting
            ProjectList = ProjectList.ApplySorting(command.Groups, command.Sorts);

            //Apply paging
            ProjectList = ProjectList.ApplyPaging(command.Page, command.PageSize);

            foreach (var project in ProjectList)
            {
                project.EncryptedProjectID = Global.UrlEncrypt(project.ProjectID.ToString());
            }

            var result = new DataSourceResult()
            {
                Data = ProjectList,
                Total = _Count // Total number of records
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult BulkStausChange(string ProjectIDs, int IsActive)
        {
            if (!string.IsNullOrEmpty(ProjectIDs))
            {
                int[] IntProjectIds = Array.ConvertAll(ProjectIDs.Split(','), int.Parse);

                var projects = _entities.tblProjects.Where(x => IntProjectIds.Contains(x.ProjectID)).ToList();
                projects.ForEach(x => x.Status = IsActive);

                _entities.SaveChanges();
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(2, JsonRequestBehavior.AllowGet);
            }
        }




        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetMembers(int ProjectID)
        {
            var model = new List<UserModel>();
            if (ProjectID > 0)
            {
                using (_entities = new ReadyPortalDBEntities())
                {

                    model = (from member in _entities.tblProjectMembers
                                //join projects in _entities.tblProjects on member.ProjectID equals projects.ProjectID
                                join user in _entities.tblUserDepartments on member.UserID equals user.UserId
                                where member.ProjectID == ProjectID /*&& member.CreatedBy == CurrentUserSession.UserID*/
                             select new UserModel
                                {
                                    FullName = user.FullName,
                                    Department = user.Department,
                                    UserId = user.UserId,
                                    UserName = user.UserName,
                                    CanCreateSubtasks = user.CanCreateSubtasks.HasValue ? user.CanCreateSubtasks.Value : false,
                                    CanEdit = user.CanEdit.HasValue ? user.CanEdit.Value : false,
                                    CanApprove = user.CanApprove.HasValue ? user.CanApprove.Value : false
                                }).ToList();
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }



        public string SplitProjetcFileSlides(string FileName, int TaskID, bool isProjectCreated = false)
        {
            string TempSlideName = string.Empty;
            string TempFileName = FileName;
            var TempThumbFileName = string.Empty;

            var savepptid = 1;
            string returnString = "success";
            string SlideName = string.Empty;
            string fileextenstion = '.' + FileName.Split('.').Last();

            int totalslidecount = 0;

            MemoryStream MainStreamSlideCount = new MemoryStream(System.IO.File.ReadAllBytes(DirectoryManagmentUtils.ProjectsDocumentFilesPath + "/" + TaskID.ToString() + "/" + FileName));

            MainStreamSlideCount.Seek(0, SeekOrigin.Begin);
            PresentationDocument presentationDocumentSlideCount = PresentationDocument.Open(MainStreamSlideCount, true);
            MainStreamSlideCount.Dispose();
            totalslidecount = CountSlides(presentationDocumentSlideCount);
            presentationDocumentSlideCount.Dispose();

            //Spire
            //Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation();
            //presentation.LoadFromFile(DirectoryManagmentUtils.ProjectsDocumentFilesPath + "/" + TaskID.ToString() + "/" + FileName);
            //totalslidecount = presentation.Slides.Count;

            var exceptionMessage = string.Empty;

            for (int j = 0; j < totalslidecount; j++)
            {
                GC.Collect();

                MemoryStream MainStreamActualFile = new MemoryStream(System.IO.File.ReadAllBytes(DirectoryManagmentUtils.ProjectsDocumentFilesPath + "/" + TaskID.ToString() + "/" + FileName));
                MainStreamActualFile.Seek(0, SeekOrigin.Begin);
                DeleteSlide(MainStreamActualFile, j);
                MainStreamActualFile.Seek(0, SeekOrigin.Begin);

                SlideName = savepptid + fileextenstion;
                var thumbfileName = savepptid + WordAutomationDemo.Common.Global.ImageExportExtention;


                if (j == 0)
                {
                    TempSlideName = SlideName;
                    TempThumbFileName = thumbfileName;
                }

                string fileurl = System.IO.Path.Combine(DirectoryManagmentUtils.ProjectsDocumentFilesPath + "/" + TaskID.ToString() + "/" + "Slides_" + FileName + "/" + SlideName);

                string tempSlideFileName = DirectoryManagmentUtils.ProjectsDocumentFilesPath + "/" + TaskID.ToString() + "/" + "Slides_" + FileName + '/' + SlideName;
                if (System.IO.File.Exists(tempSlideFileName))
                {
                    System.IO.File.Delete(tempSlideFileName);
                }

                var fs1 = new FileStream(tempSlideFileName, FileMode.OpenOrCreate);

                MainStreamActualFile.CopyTo(fs1);
                MainStreamActualFile.Close();

                fs1.Close();
                fs1.Dispose();
                MemoryStream MainStreamSaveFile = new MemoryStream(System.IO.File.ReadAllBytes(tempSlideFileName), true);
                System.IO.File.WriteAllBytes(fileurl, MainStreamSaveFile.ToArray());
                MainStreamSaveFile.Close();
                MainStreamSaveFile.Dispose();


                GeneratePPTImage(SlideName, FileName, thumbfileName, TaskID.ToString(), isProjectCreated);

                if (!isProjectCreated)
                {
                    tblPPTSlide _slide = new tblPPTSlide();
                    _slide.SlideName = SlideName;
                    _slide.Sequence = savepptid;
                    _slide.MasterDocumentName = FileName;
                    _entities.tblPPTSlides.Add(_slide);
                    tblPPTSlide _slideOriginal = new tblPPTSlide();
                    _slideOriginal.SlideName = SlideName;
                    _slideOriginal.Sequence = savepptid;
                    _slideOriginal.MasterDocumentName = FileName;
                    _slideOriginal.IsOriginal = true;
                    _entities.tblPPTSlides.Add(_slideOriginal);
                }
                savepptid++;

                GC.Collect();
            }

            _entities.SaveChanges();



            return returnString;
        }

        public static void DeleteSlide(MemoryStream MemoryStream, int slideIndex)
        {
            using (PresentationDocument presentationDocument = PresentationDocument.Open(MemoryStream, true))
            {
                DeleteSlide(presentationDocument, slideIndex);
            }
        }

        // Delete the specified slide from the presentation.
        public static void DeleteSlide(PresentationDocument presentationDocument, int slideIndex)
        {
            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            // Use the CountSlides sample to get the number of slides in the presentation.
            int slidesCount = CountSlides(presentationDocument);
            for (int i = 0; i < slidesCount; i++)
            {
                if (i != slideIndex)
                {

                    //if (slideIndex < 0 || slideIndex >= slidesCount)
                    //{
                    //    throw new ArgumentOutOfRangeException("slideIndex");
                    //}

                    // Get the presentation part from the presentation document. 
                    PresentationPart presentationPart = presentationDocument.PresentationPart;

                    // Get the presentation from the presentation part.
                    DocumentFormat.OpenXml.Presentation.Presentation presentation = presentationPart.Presentation;

                    // Get the list of slide IDs in the presentation.
                    SlideIdList slideIdList = presentation.SlideIdList;

                    // Get the slide ID of the specified slide
                    //SlideId slideId = slideIdList.ChildElements[slideIndex] as SlideId;
                    SlideId slideId = slideIdList.ChildElements[i] as SlideId;

                    // Get the relationship ID of the slide.
                    string slideRelId = slideId.RelationshipId;

                    // Remove the slide from the slide list.
                    slideIdList.RemoveChild(slideId);

                    //
                    // Remove references to the slide from all custom shows.
                    if (presentation.CustomShowList != null)
                    {
                        // Iterate through the list of custom shows.
                        foreach (var customShow in presentation.CustomShowList.Elements<CustomShow>())
                        {
                            if (customShow.SlideList != null)
                            {
                                // Declare a link list of slide list entries.
                                LinkedList<SlideListEntry> slideListEntries = new LinkedList<SlideListEntry>();
                                foreach (SlideListEntry slideListEntry in customShow.SlideList.Elements())
                                {
                                    // Find the slide reference to remove from the custom show.
                                    if (slideListEntry.Id != null && slideListEntry.Id == slideRelId)
                                    {
                                        slideListEntries.AddLast(slideListEntry);
                                    }
                                }

                                // Remove all references to the slide from the custom show.
                                foreach (SlideListEntry slideListEntry in slideListEntries)
                                {
                                    customShow.SlideList.RemoveChild(slideListEntry);
                                }
                            }
                        }
                    }

                    // Save the modified presentation.
                    presentation.Save();

                    // Get the slide part for the specified slide.
                    SlidePart slidePart = presentationPart.GetPartById(slideRelId) as SlidePart;

                    // Remove the slide part.
                    presentationPart.DeletePart(slidePart);

                    i--;

                    if (slideIndex > 0)
                    {
                        slideIndex--;
                    }
                    slidesCount = CountSlides(presentationDocument);
                }
            }
        }


        public static int CountSlides(PresentationDocument presentationDocument)
        {
            // Check for a null document object.
            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            int slidesCount = 0;

            // Get the presentation part of document.
            PresentationPart presentationPart = presentationDocument.PresentationPart;

            // Get the slide count from the SlideParts.
            if (presentationPart != null)
            {
                slidesCount = presentationPart.SlideParts.Count();
            }

            // Return the slide count to the previous method.
            return slidesCount;
        }


        public void GeneratePPTImage(string SlideName, string FileName, string thumbfileName, string TaskID, bool isProjectCreated)
        {
            string exceptionMessage = string.Empty;
            string fileurl = string.Empty;
            if (isProjectCreated)
            {
                fileurl = System.IO.Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "\\" + "Slides_" + FileName + "\\" + SlideName);
            }
            else
            {
                fileurl = System.IO.Path.Combine(DirectoryManagmentUtils.ProjectsDocumentFilesPath + "\\" + TaskID + "\\" + "Slides_" + FileName + "\\" + SlideName);
            }

            Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(fileurl);

            //save the slide to Image
            using (System.Drawing.Image image = presentation.Slides[0].GetThumbnail(1.0F, 1.0F))
                image.Save(DirectoryManagmentUtils.ProjectsDocumentFilesPath + "\\" + TaskID + "\\" + "Slides_" + FileName + "\\" + thumbfileName, System.Drawing.Imaging.ImageFormat.Png);
            presentation.Dispose();
        }
        #endregion
    }
}
