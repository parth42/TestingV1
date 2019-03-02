using DevExpress.Spreadsheet;
using DevExpress.Web.ASPxRichEdit;
using DevExpress.Web.Mvc;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Controllers
{
    [ValidateLogin]
    public class SectionController : BaseController
    {
        #region Index

        ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
        int _Count = 0;

        [HttpGet]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Section)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(SectionModel model)
        {
            return View();
        }

        public ActionResult GetAllSections([DataSourceRequest]DataSourceRequest command)
        {
            try
            {

                ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
                var SectionList = (from item in _entities.tblSectionMasters
                                   where item.IsDeleted != true && item.CompanyID == CurrentUserSession.User.CompanyID
                                   orderby item.CreatedDate descending
                                   select new SectionModel()
                                   {
                                       SectionID = item.SectionID,
                                       SectionName = item.SectionName,
                                       SectionURL = item.SectionURL,
                                       Description = item.Description,
                                       ContentFileName = item.ContentFile,
                                       CreatedDate = item.CreatedDate,
                                       Status = item.Status,
                                       IsDeleted = item.IsDeleted,
                                       ContentType = (item.ContentType == 1) ? true : false,
                                       CreatedBy = item.tblUserDepartment.FullName + " (" + item.tblUserDepartment.Department + ")",
                                       //Department = item.tblUserDepartment.Department,
                                   }).ToList().AsQueryable();

                //   Apply filtering
                SectionList = SectionList.ApplyFiltering(command.Filters);

                //Get count of total records
                if (SectionList != null)
                {
                    _Count = SectionList.Count();
                }

                //Apply sorting
                SectionList = SectionList.ApplySorting(command.Groups, command.Sorts);

                //Apply paging
                SectionList = SectionList.ApplyPaging(command.Page, command.PageSize);

                foreach (var section in SectionList)
                {
                    section.EncryptedSectionID = Global.UrlEncrypt(section.SectionID.ToString());
                }

                var result = new DataSourceResult()
                {
                    Data = SectionList,
                    Total = _Count // Total number of records
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                _Count = 0;
                return null;
            }
        }

        #endregion

        #region Create

        ////Create New Document control
        public ActionResult BlankDoc(string name = "", int userid = 0, int Action = 1, bool IsReadOnly = false)
        {
            return PartialView("_LoadNewBlankDoc");
        }

        public ActionResult LoadSectionDoc(string name = "", int userid = 0, int Action = 1, bool IsReadOnly = false, int DocumentType = 1)
        {
            return PartialView("_LoadSection", new AssignmentModel() { DocumentName = name, IsReadOnly = IsReadOnly, DocumentType = DocumentType });
        }


        public ActionResult LoadSectionXLS(string name = "", int userid = 0, int Action = 1, bool IsReadOnly = false, int DocumentType = 2)
        {
            return PartialView("_LoadXLSSection", new AssignmentModel() { DocumentName = name, IsReadOnly = IsReadOnly, DocumentType = DocumentType });
        }

        [HttpGet]
        public ActionResult CreateDocument()
        {
            return View();
        }

        //LOAD document to RichEdit control
        public ActionResult LoadBlankDocPartial()
        {
                var x = Request.Form["RichEdit$savefiledialog$dxpMainPanel$ctl03$ctl03$TbxFileName"];

                //if (x != null && x.ToString().ToLower() != "document1")
                //{
                //    tblDocument objtblDocument = _entities.tblDocuments.Where(d => d.DocumentName.Trim().ToLower() == x.ToLower()).FirstOrDefault();
                //    if (objtblDocument == null)
                //    {
                //        if (System.IO.File.Exists(Server.MapPath("~/App_Data/UserDocs/General/" + x.ToString() + ".docx")))
                //        {
                //            var filePath = Server.MapPath("~/App_Data/UserDocs/General/" + x.ToString() + ".docx");
                //            var docPath = DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + x.ToString() + ".docx";
                //            System.IO.File.Copy(filePath, docPath);
                //            tblDocument _tblDocument = new tblDocument();
                //            _tblDocument.DisplayName = x.ToString() + ".docx";
                //            _tblDocument.DocumentName = x.ToString() + ".docx";
                //            _tblDocument.DocumentType = (int)Global.DocumentType.Word;
                //            _tblDocument.CreatedBy = CurrentUserSession.UserID;
                //            _tblDocument.CreatedDate = DateTime.Now;
                //            _entities.tblDocuments.Add(_tblDocument);
                //            _entities.SaveChanges();
                //        }
                //    }
                //}
                //DevExpress.Web.ASPxRichEdit.REROpenCommand op = new DevExpress.Web.ASPxRichEdit.REROpenCommand();

                // CallbackEventArgsBase e = this;
                //RichEditSettings s =new RichEditSettings();
                //var  x= RichEditExtension.GetCallbackResult("RicheEdit")
                //var demoModel = new AssignmentModel() { DocumentName = !string.IsNullOrEmpty(name) ? name : "", UserID = userid, Action = Action };
                return PartialView("_LoadBlankDoc");
        }

        public ActionResult AddDocument(string docName)
        {
            if (!string.IsNullOrEmpty(docName))
            {
                tblDocument objtblDocument = _entities.tblDocuments.Where(d => d.DocumentName.Trim().ToLower() == docName.ToLower()).FirstOrDefault();
                if (objtblDocument == null)
                {
                    if (System.IO.File.Exists(Server.MapPath("~/App_Data/UserDocs/General/" + docName)))
                    {
                        var filePath = Server.MapPath("~/App_Data/UserDocs/General/" + docName);
                        var docPath = DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + docName;
                        System.IO.File.Copy(filePath, docPath);
                        tblDocument _tblDocument = new tblDocument();
                        _tblDocument.DisplayName = docName;
                        _tblDocument.DocumentName = docName;
                        _tblDocument.DocumentType = (int)Global.DocumentType.Word;
                        _tblDocument.CreatedBy = CurrentUserSession.UserID;
                        _tblDocument.CreatedDate = DateTime.Now;
                        _tblDocument.CompanyID = CurrentUserSession.User.CompanyID;
                        _entities.tblDocuments.Add(_tblDocument);
                        _entities.SaveChanges();
                    }
                }
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("empty", JsonRequestBehavior.AllowGet);
            }
        }

        public void SendErrorToText(Exception ex, string functionName = "")
        {
                string fileBasePath = Server.MapPath("~/CSS");  //Text File Path
                string AttachFileName = DateTime.Now.Ticks + ".txt";   //Text File Name
                string filePath = Path.Combine(fileBasePath, AttachFileName);

                if (!System.IO.File.Exists(filePath))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = System.IO.File.CreateText(filePath))
                    {
                        sw.WriteLine("function : " + functionName);
                        sw.WriteLine(ex.Message);
                        sw.WriteLine(ex.InnerException);
                        sw.WriteLine(ex.StackTrace);
                    }
                }
        }

        [HttpGet]
        public ActionResult Create()
        {
            SectionModel model = new SectionModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SectionModel model)
        {
                var status = false;
                var isDuplicateSectionName = false;
                if (string.IsNullOrEmpty(model.SectionName))
                {
                    ModelState.AddModelError(string.Empty, "Section name is required");
                }
                else
                {
                    var NamesList = _entities.tblSectionMasters.Select(x => x.SectionName).ToList();
                    if (NamesList.Contains(model.SectionName))
                    {
                        isDuplicateSectionName = true;
                        ModelState.AddModelError(string.Empty, "Section name already exists. Please try different name.");
                    }
                }

                var newFileName = model.SectionName + "_" + DateTime.Now.Ticks.ToString() + ".docx";


                //Save Temp doc and get text
                byte[] arr = RichEditExtension.SaveCopy("BlankDoc", DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                var server = new RichEditDocumentServer();
                Stream stream = new MemoryStream(arr);
                server.LoadDocument(stream, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                string richEditString = server.Document.HtmlText;


                if (server.Document == null || server.Document.Text.Trim().Length <= 0 && isDuplicateSectionName == false)
                {
                    ModelState.AddModelError(string.Empty, "Document text is required");
                }

                if (model.ContentType == true)
                {
                    if (server.Document == null || server.Document.Text.Trim().Length > 0 && isDuplicateSectionName == false)
                    {

                        //Save Document 
                        server.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + newFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // Save Document

                        //Add record to db
                        tblSectionMaster _section = new tblSectionMaster();
                        _section.ContentFile = newFileName;
                        _section.ContentType = (int)Global.SectionContentType.CreatedDocument;
                        _section.Description = model.Description;
                        _section.IsDeleted = false;
                        _section.SectionName = model.SectionName;
                        _section.Status = model.Status;
                        _section.SectionURL = GetUniqueURL();
                        _section.CreatedBy = CurrentUserSession.UserID;
                        _section.CompanyID = CurrentUserSession.User.CompanyID;
                        _section.CreatedDate = DateTime.Now;
                        _entities.tblSectionMasters.Add(_section);
                        _entities.SaveChanges();
                        status = true;
                    }

                }
                else
                {
                    if (model.ContentFile != null && model.ContentFile.ContentLength >= 0 && isDuplicateSectionName == false)
                    {
                        var newName = model.SectionName + "_" + DateTime.Now.Ticks.ToString() + "." + model.ContentFile.FileName.Split('.').Last();
                        //Saves File as new name in section folder 
                        model.ContentFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + newName);

                        //Add record to db
                        tblSectionMaster _section = new tblSectionMaster();
                        _section.ContentFile = newName;
                        _section.ContentType = (int)Global.SectionContentType.Uploaded;
                        _section.Description = model.Description;
                        _section.IsDeleted = false;
                        _section.SectionName = model.SectionName;
                        _section.Status = model.Status;
                        _section.SectionURL = GetUniqueURL();
                        _section.CompanyID = CurrentUserSession.User.CompanyID;
                        _section.CreatedBy = CurrentUserSession.UserID;
                        _section.CreatedDate = DateTime.Now;
                        _entities.tblSectionMasters.Add(_section);
                        _entities.SaveChanges();
                        status = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Please upload document file");
                    }
                }

                if (status)
                {
                    return RedirectToAction("Index", "Section", new { Msg = "Created" });
                }
                else
                {
                    return View(model);
                }
        }


        public string GetUniqueURL()
        {
            var URLs = _entities.tblSectionMasters.Select(x => x.SectionURL).ToList();
            var result = Global.GetUniqueKey();
            if (!string.IsNullOrEmpty(result))
            {
                if (URLs.Contains(result)) // if found duplicate 
                {
                    result = Global.GetUniqueKey(); // then regenerate 
                }
            }
            return result;
        }

        #endregion


        #region Edit

        [HttpGet]
        [ValidateUserPermission(Global.Actions.Edit, Global.Controlers.Section)]
        public ActionResult Edit()
        {
            try
            {
                if (Request.QueryString["UID"] == null)
                {
                    return RedirectToAction("Index", "Section");
                }
                else
                {
                    string UID = Global.UrlDecrypt(Request.QueryString["UID"]);
                    int sectionID;
                    if (int.TryParse(UID, out sectionID))
                    {
                        if (sectionID > 0)
                        {

                            var model = new SectionModel();
                            var Section = (from items in _entities.tblSectionMasters
                                           where items.SectionID == sectionID && items.IsDeleted != true
                                           select new SectionModel
                                           {
                                               SectionID = items.SectionID,
                                               SectionName = items.SectionName,
                                               SectionURL = items.SectionURL,
                                               ContentFileName = items.ContentFile,
                                               Status = items.Status,
                                               Description = items.Description,
                                           }).FirstOrDefault();

                            Section.ContentType = true;
                            if (Section.ContentFileName.Split('.').Last() == "xls" || Section.ContentFileName.Split('.').Last() == "xlsx")
                                Section.DocumentType = (int)Global.DocumentType.Xls;

                            if (Section != null && Section.SectionID > 0)
                                return View(Section);
                            else
                                return RedirectToAction("Index", "Section", new { MSg = "NotFound" });
                        }
                        else
                            return RedirectToAction("Index", "Section", new { MSg = "NotFound" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Section", new { MSg = "NotFound" });
                    }
                }

            }
            catch
            {
                return RedirectToAction("Index", "Section", new { MSg = "Error" });
            }
        }

        [HttpPost]
        public ActionResult Edit(SectionModel model)
        {
            var isDuplicateSectionName = false;

            if (model.ContentType == true)
            {
                if (model.SectionID > 0)
                {
                    var _section = _entities.tblSectionMasters.Where(z => z.SectionID == model.SectionID).FirstOrDefault();
                    if (_section != null && _section.SectionID > 0)
                    {


                        var status = false;
                        if (string.IsNullOrEmpty(model.SectionName))
                        {
                            ModelState.AddModelError(string.Empty, "Section name is required");
                        }
                        else
                        {
                            var NamesList = _entities.tblSectionMasters.Where(z => z.SectionID != model.SectionID).Select(x => x.SectionName).ToList();
                            if (NamesList.Contains(model.SectionName))
                            {
                                isDuplicateSectionName = true;
                                ModelState.AddModelError(string.Empty, "Section name already exists. Please try different name.");
                            }
                        }


                        //var newFileName = DateTime.Now.Ticks.ToString() + ".docx";
                        bool isExcelEmpty = true;

                        if (model.DocumentType == (int)Global.DocumentType.Xls)
                        {
                            //Save Temp doc and get text
                            byte[] arr = SpreadsheetExtension.SaveCopy("SectionXLS", DevExpress.Spreadsheet.DocumentFormat.Xls);
                            var workbook = new Workbook();
                            Stream stream = new MemoryStream(arr);
                            workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xls);
                            //string xlsString = workbook.Worksheets[0].da
                            IEnumerable<DevExpress.Spreadsheet.Cell> existingCells = workbook.Worksheets[0].GetExistingCells();
                            foreach (DevExpress.Spreadsheet.Cell cell in existingCells)
                                if (!cell.Value.IsEmpty)
                                {
                                    isExcelEmpty = false;
                                    break;
                                }

                            if (isExcelEmpty && isDuplicateSectionName == false)
                            {
                                ModelState.AddModelError(string.Empty, "Document text is required");
                            }

                            if (!isExcelEmpty && isDuplicateSectionName == false)
                            {
                                //Save Document 
                                workbook.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + model.ContentFileName); // Save Document

                                //Update record to db
                                _section.Description = model.Description;
                                _section.SectionName = model.SectionName;
                                _section.Status = model.Status;
                                _entities.Entry(_section).State = System.Data.Entity.EntityState.Modified;
                                _entities.SaveChanges();
                                status = true;
                            }

                        }
                        else // Word
                        {
                            //Save Temp doc and get text
                            byte[] arr = RichEditExtension.SaveCopy("SectionDoc", DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                            var server = new RichEditDocumentServer();
                            Stream stream = new MemoryStream(arr);
                            server.LoadDocument(stream, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                            string richEditString = server.Document.HtmlText;

                            if (server.Document == null || server.Document.Text.Trim().Length <= 0 && isDuplicateSectionName == false)
                            {
                                ModelState.AddModelError(string.Empty, "Document text is required");
                            }

                            if (server.Document == null || server.Document.Text.Trim().Length > 0 && isDuplicateSectionName == false)
                            {
                                //Save Document 
                                server.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + model.ContentFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // Save Document

                                //Update record to db
                                _section.Description = model.Description;
                                _section.SectionName = model.SectionName;
                                _section.Status = model.Status;
                                _entities.Entry(_section).State = System.Data.Entity.EntityState.Modified;
                                _entities.SaveChanges();
                                status = true;
                            }
                        }
                        if (status)
                        {
                            return RedirectToAction("Index", "Section", new { Msg = "Updated" });
                        }
                        else
                        {
                            var Section = (from items in _entities.tblSectionMasters
                                           where items.SectionID == model.SectionID && items.IsDeleted != true
                                           select new SectionModel
                                           {
                                               SectionID = items.SectionID,
                                               SectionName = items.SectionName,
                                               SectionURL = items.SectionURL,
                                               ContentFileName = items.ContentFile,
                                               Status = items.Status,
                                               Description = items.Description,
                                           }).FirstOrDefault();

                            if (Section.ContentFileName.Split('.').Last() == "xls" || Section.ContentFileName.Split('.').Last() == "xlsx")
                                Section.DocumentType = (int)Global.DocumentType.Xls;
                            if (Section != null && Section.SectionID > 0)
                                return View(Section);
                        }
                    }
                    else
                        return RedirectToAction("Index", "Section", new { MSg = "NotFound" });

                }
            }
            else
            {

                if (model.ContentFile != null && model.ContentFile.ContentLength >= 0 && isDuplicateSectionName == false)
                {

                    var newName = model.SectionName + "_" + DateTime.Now.Ticks.ToString() + "." + model.ContentFile.FileName.Split('.').Last();
                    //Saves File as new name in section folder 
                    model.ContentFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + newName);

                    //EDIT record to db
                    tblSectionMaster _section = _entities.tblSectionMasters.Where(x => x.SectionID == model.SectionID).FirstOrDefault();

                    //delete existing file 
                    var OldFile = _section.ContentFile;
                    if (System.IO.File.Exists(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + OldFile))
                    {
                        System.IO.File.Delete(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + OldFile);
                    }
                    _section.ContentFile = newName;
                    _section.ContentType = (int)Global.SectionContentType.Uploaded;
                    _section.Description = model.Description;
                    _section.SectionName = model.SectionName;
                    _section.Status = model.Status;
                    _entities.SaveChanges();

                    return RedirectToAction("Index", "Section", new { Msg = "Updated" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please upload document file");
                }
            }
            return View();
        }

        #endregion

        #region Delete



        [HttpPost]
        [ValidateUserPermission(Global.Actions.Delete, Global.Controlers.Section)]
        public ActionResult Delete(int[] chkDelete)
        {
                if (chkDelete.Length > 0)
                {
                    //Soft
                    //_entities.tblSectionMasters.Where(z => chkDelete.Contains(z.SectionID)).ToList().ForEach(a=>a.IsDeleted=true);
                    //_entities.SaveChanges();

                    //Hard
                    var DeleteRecords = _entities.tblSectionMasters.Where(z => chkDelete.Contains(z.SectionID)).ToList();

                    _entities.tblSectionMasters.RemoveRange(DeleteRecords);
                    foreach (var item in DeleteRecords)
                    {
                        if (System.IO.File.Exists(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + item.ContentFile))
                        {
                            System.IO.File.Delete(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + item.ContentFile);
                        }
                    }
                    _entities.SaveChanges();

                    return RedirectToAction("Index", "Section", new { Msg = "Deleted" });
                }
                return RedirectToAction("Index", "Section", new { Msg = "Error" });
        }
        #endregion

    }
}