using DevExpress.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Controllers
{
    public class AddInController : Controller
    {
        public AddInController() : base()
        {
            new Aspose.Slides.License().SetLicense("Aspose.Total.lic");
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public string AddSlide(int AssignmentID, string FolderName, string FileName, int UserID)
        {
            Result objAvailabilityResult = new Result();
            using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
            {
                var fileName = FileName.Split('_');
                tblAssignment _tblAssignment = _entities.tblAssignments.Where(c => c.AssignmentID == AssignmentID).FirstOrDefault();
                if (_tblAssignment != null)
                {
                    if (_tblAssignment.Action != (int)WordAutomationDemo.Common.Global.Action.Completed)
                    {
                        Aspose.Slides.Presentation slidepresentation = new Aspose.Slides.Presentation();
                        var Path = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "\\" + FolderName + "\\AssignedDoc\\" + "New_" + FileName;

                        if (System.IO.File.Exists(Path))
                        {
                            Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(Path);
                            for (int j = 0; j < presentation.Slides.Count; j++)
                            {

                                //save the slide to Image
                                using (System.Drawing.Image image = presentation.Slides[j].GetThumbnail(1.0F, 1.0F))
                                    image.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + _tblAssignment.tblDocument.DocumentName + "/AssignedDoc/New_" + FileName.Split('.').First().ToString() + "(" + j + ")" + WordAutomationDemo.Common.Global.ImageExportExtention, System.Drawing.Imaging.ImageFormat.Png);

                                presentation.Dispose();
                            }
                            string strSlideName = FileName.Split('_').Last().ToString();
                            var pptSlide = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == AssignmentID && z.SlideName == strSlideName).FirstOrDefault();
                            if (pptSlide != null)
                            {
                                pptSlide.IsPPTModified = true;
                                _entities.Entry(pptSlide).State = System.Data.Entity.EntityState.Modified;
                            }

                            //SplitSlides(objPPTDocModel.FileName);
                            //_tblAssignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                            //_tblAssignment.CompletedDate = DateTime.Now;
                            //if (!string.IsNullOrEmpty(model.Remarks))
                            //{
                            //    doc.Remarks = model.Remarks;
                            //    doc.Comments = model.Remarks;
                            //}
                            _entities.Entry(_tblAssignment).State = System.Data.Entity.EntityState.Modified;

                            //Add Log 
                            tblAssignmentLog _log = new tblAssignmentLog();
                            _log.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                            _log.AssignmentID = _tblAssignment.AssignmentID;
                            _log.CreatedDate = DateTime.Now;
                            _log.CreatedBy = _tblAssignment.LockedByUserID;
                            _log.Description = strSlideName + " has been updated from power point";
                            //_log.DocumentName = model.OriginalFile;
                            _entities.tblAssignmentLogs.Add(_log);
                            _entities.SaveChanges();

                            //string message = string.Empty;
                            //var modifiedSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == AssignmentID && z.IsPPTModified.HasValue && z.IsPPTModified.Value == true).Count();
                            //var totalSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == AssignmentID).Count();
                            //if (modifiedSlidesCount == totalSlidesCount)
                            //{
                            //    _tblAssignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                            //    _tblAssignment.CompletedDate = DateTime.Now;
                            //    _entities.SaveChanges();
                            //    message = "Task has been completed successfully.";
                            //}
                            //else
                            //{
                            //    message = "PPT has been saved successfully in triyosoft platform.";
                            //}
                            objAvailabilityResult.Status = true;
                            objAvailabilityResult.ErrorCode = "";
                            objAvailabilityResult.ErrorMessage = "PPT has been saved successfully in triyosoft platform.";
                        }
                    }
                    else
                    {
                        objAvailabilityResult.Status = false;
                        objAvailabilityResult.ErrorCode = WordAutomationDemo.Common.Global.Action.Completed.ToString();
                        objAvailabilityResult.ErrorMessage = "Task already completed";
                    }
                }
            }
            return objAvailabilityResult.ErrorMessage;
        }

        [HttpPost]
        [AllowAnonymous]
        public string AddSlideService(int AssignmentID, string FolderName, string FileName, int UserID, string data)
        {
            Result objAvailabilityResult = new Result();
            using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
            {
                var fileName = FileName.Split('_');
                tblAssignment _tblAssignment = _entities.tblAssignments.Where(c => c.AssignmentID == AssignmentID).FirstOrDefault();
                if (_tblAssignment != null)
                {
                    if (_tblAssignment.Action != (int)WordAutomationDemo.Common.Global.Action.Completed)
                    {

                        byte[] buffer = Convert.FromBase64String(data);
                        var Path = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "\\" + FolderName + "\\AssignedDoc\\" + "New_" + FileName;
                        System.IO.File.WriteAllBytes(Path, buffer);

                        Aspose.Slides.Presentation slidepresentation = new Aspose.Slides.Presentation();

                        if (System.IO.File.Exists(Path))
                        {
                            Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(Path);
                            for (int j = 0; j < presentation.Slides.Count; j++)
                            {

                                //save the slide to Image
                                using (System.Drawing.Image image = presentation.Slides[j].GetThumbnail(1.0F, 1.0F))
                                    image.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + _tblAssignment.tblDocument.DocumentName + "/AssignedDoc/New_" + FileName.Split('.').First().ToString() + "(" + j + ")" + WordAutomationDemo.Common.Global.ImageExportExtention, System.Drawing.Imaging.ImageFormat.Png);
                                presentation.Dispose();
                            }
                            string strSlideName = FileName.Split('_').Last().ToString();
                            var pptSlide = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == AssignmentID && z.SlideName == strSlideName).FirstOrDefault();
                            if (pptSlide != null)
                            {
                                pptSlide.IsPPTModified = true;
                                _entities.Entry(pptSlide).State = System.Data.Entity.EntityState.Modified;
                            }

                            //SplitSlides(objPPTDocModel.FileName);
                            //_tblAssignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                            //_tblAssignment.CompletedDate = DateTime.Now;
                            //if (!string.IsNullOrEmpty(model.Remarks))
                            //{
                            //    doc.Remarks = model.Remarks;
                            //    doc.Comments = model.Remarks;
                            //}
                            _entities.Entry(_tblAssignment).State = System.Data.Entity.EntityState.Modified;

                            //Add Log 
                            tblAssignmentLog _log = new tblAssignmentLog();
                            _log.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                            _log.AssignmentID = _tblAssignment.AssignmentID;
                            _log.CreatedDate = DateTime.Now;
                            _log.CreatedBy = _tblAssignment.LockedByUserID;
                            _log.Description = strSlideName + " has been updated from power point";
                            //_log.DocumentName = model.OriginalFile;
                            _entities.tblAssignmentLogs.Add(_log);
                            _entities.SaveChanges();

                            //string message = string.Empty;
                            //var modifiedSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == AssignmentID && z.IsPPTModified.HasValue && z.IsPPTModified.Value == true).Count();
                            //var totalSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == AssignmentID).Count();
                            //if (modifiedSlidesCount == totalSlidesCount)
                            //{
                            //    _tblAssignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                            //    _tblAssignment.CompletedDate = DateTime.Now;
                            //    _entities.SaveChanges();
                            //    message = "Task has been completed successfully.";
                            //}
                            //else
                            //{
                            //    message = "PPT has been saved successfully in triyosoft platform.";
                            //}
                            objAvailabilityResult.Status = true;
                            objAvailabilityResult.ErrorCode = "";
                            objAvailabilityResult.ErrorMessage = "PPT has been saved successfully in triyosoft platform.";
                        }
                    }
                    else
                    {
                        objAvailabilityResult.Status = false;
                        objAvailabilityResult.ErrorCode = WordAutomationDemo.Common.Global.Action.Completed.ToString();
                        objAvailabilityResult.ErrorMessage = "Task already completed";
                    }
                }
            }
            return objAvailabilityResult.ErrorMessage;
        }

        [HttpPost]
        [AllowAnonymous]
        public string AddSheetServiceOld(int AssignedExcelSheetID, int AssignmentID, string FolderName, string FileName, int UserID, string data)
        {
            Result objAvailabilityResult = new Result();
                using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                {
                    var fileName = FileName.Split('_');
                    tblAssignment _tblAssignment = _entities.tblAssignments.Where(c => c.AssignmentID == AssignmentID).FirstOrDefault();
                    if (_tblAssignment != null)
                    {
                        if (_tblAssignment.Action != (int)WordAutomationDemo.Common.Global.Action.Completed)
                        {
                            byte[] buffer = Convert.FromBase64String(data);

                            var Path = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\Sheets_" + _tblAssignment.tblDocument.DocumentName + "\\AssignedDoc\\New_" + FileName;
                            System.IO.File.WriteAllBytes(Path, buffer);

                            if (System.IO.File.Exists(Path))
                            {
                                Workbook workbook = new Workbook();
                                workbook.LoadDocument(Path, DevExpress.Spreadsheet.DocumentFormat.OpenXml);

                                int j = 0;
                                foreach (Worksheet worksheet in workbook.Worksheets)
                                {
                                    string SheetName = string.Empty;
                                    Workbook WorkbookSplit = new Workbook();
                                    SheetName = worksheet.Name;
                                    WorkbookSplit.Worksheets[0].Name = "Sheet";
                                    WorkbookSplit.Worksheets.Add(SheetName);
                                    WorkbookSplit.Worksheets[SheetName].CopyFrom(workbook.Worksheets[j]);
                                    WorkbookSplit.Worksheets.RemoveAt(0);
                                    string tempSheetFileName = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + _tblAssignment.tblDocument.DocumentName + "/AssignedDoc/New_" + FileName.Split('.').First().ToString() + "(" + j + ")" + ".xlsx";
                                    if (System.IO.File.Exists(tempSheetFileName))
                                    {
                                        System.IO.File.Delete(tempSheetFileName);
                                    }
                                    WorkbookSplit.SaveDocument(tempSheetFileName);
                                    j++;
                                }
                            }
                                
                            string strSheetName = FileName.Split('_').Last().ToString();
                            var excelSheet = _entities.tblAssignedExcelSheets.Where(z => z.AssignedExcelSheetID == AssignedExcelSheetID).FirstOrDefault();
                            if (excelSheet != null)
                            {
                                excelSheet.IsSheetModified = true;
                                excelSheet.IsGrayedOut = true;
                                _entities.Entry(excelSheet).State = System.Data.Entity.EntityState.Modified;
                                _entities.SaveChanges();
                            }

                            _entities.Entry(_tblAssignment).State = System.Data.Entity.EntityState.Modified;

                            //Add Log 
                            tblAssignmentLog _log = new tblAssignmentLog();
                            _log.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                            _log.AssignmentID = _tblAssignment.AssignmentID;
                            _log.CreatedDate = DateTime.Now;
                            _log.CreatedBy = _tblAssignment.LockedByUserID;
                            _log.Description = strSheetName + " has been updated from excel";
                            //_log.DocumentName = model.OriginalFile;
                            _entities.tblAssignmentLogs.Add(_log);
                            _entities.SaveChanges();

                            //string message = string.Empty;
                            //var modifiedSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == AssignmentID && z.IsPPTModified.HasValue && z.IsPPTModified.Value == true).Count();
                            //var totalSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == AssignmentID).Count();
                            //if (modifiedSlidesCount == totalSlidesCount)
                            //{
                            //    _tblAssignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                            //    _tblAssignment.CompletedDate = DateTime.Now;
                            //    _entities.SaveChanges();
                            //    message = "Task has been completed successfully.";
                            //}
                            //else
                            //{
                            //    message = "PPT has been saved successfully in triyosoft platform.";
                            //}
                            objAvailabilityResult.Status = true;
                            objAvailabilityResult.ErrorCode = "";
                            objAvailabilityResult.ErrorMessage = "Excel has been saved successfully in triyosoft platform.";
                        }
                        else
                        {
                            objAvailabilityResult.Status = false;
                            objAvailabilityResult.ErrorCode = WordAutomationDemo.Common.Global.Action.Completed.ToString();
                            objAvailabilityResult.ErrorMessage = "Task already completed";
                        }
                    }
                }
            return objAvailabilityResult.ErrorMessage;
        }

        [HttpPost]
        [AllowAnonymous]
        public string AddSheetService()
        {
            Result objAvailabilityResult = new Result();
                Workbook workbook = new Workbook();
                string FileName = string.Empty;
                int AssignmentID = 0;
                int AssignedExcelSheetID = 0;
                if (Request.Files != null && Request.Files[0] != null)
                {
                    workbook.LoadDocument(Request.Files[0].InputStream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                    if (workbook != null && workbook.DocumentProperties != null && workbook.DocumentProperties.Manager != null)
                    {
                        var propManager = workbook.DocumentProperties.Manager.Split('|');
                        FileName = propManager[3];
                        AssignmentID = Convert.ToInt32(propManager[1]);
                        AssignedExcelSheetID = Convert.ToInt32(propManager[4]);
                        using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                        {
                            var fileName = FileName.Split('_');
                            tblAssignment _tblAssignment = _entities.tblAssignments.Where(c => c.AssignmentID == AssignmentID).FirstOrDefault();
                            if (_tblAssignment != null)
                            {
                                if (_tblAssignment.Action != (int)WordAutomationDemo.Common.Global.Action.Completed)
                                {
                                    var Path = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\Sheets_" + _tblAssignment.tblDocument.DocumentName + "\\AssignedDoc\\New_" + FileName;
                                    workbook.SaveDocument(Path);
                                    if (System.IO.File.Exists(Path))
                                    {
                                        int j = 0;
                                        foreach (Worksheet worksheet in workbook.Worksheets)
                                        {
                                            string SheetName = string.Empty;
                                            Workbook WorkbookSplit = new Workbook();
                                            SheetName = worksheet.Name;
                                            WorkbookSplit.Worksheets[0].Name = "Sheet";
                                            WorkbookSplit.Worksheets.Add(SheetName);
                                            WorkbookSplit.Worksheets[SheetName].CopyFrom(workbook.Worksheets[j]);
                                            WorkbookSplit.Worksheets.RemoveAt(0);
                                            string tempSheetFileName = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + _tblAssignment.tblDocument.DocumentName + "/AssignedDoc/New_" + FileName.Split('.').First().ToString() + "(" + j + ")" + ".xlsx";
                                            if (System.IO.File.Exists(tempSheetFileName))
                                            {
                                                System.IO.File.Delete(tempSheetFileName);
                                            }
                                            WorkbookSplit.SaveDocument(tempSheetFileName);
                                            j++;
                                        }
                                    }

                                    string strSheetName = FileName.Split('_').Last().ToString();
                                    var excelSheet = _entities.tblAssignedExcelSheets.Where(z => z.AssignedExcelSheetID == AssignedExcelSheetID).FirstOrDefault();
                                    if (excelSheet != null)
                                    {
                                        excelSheet.IsSheetModified = true;
                                        excelSheet.IsGrayedOut = true;
                                        _entities.Entry(excelSheet).State = System.Data.Entity.EntityState.Modified;
                                        _entities.SaveChanges();
                                    }

                                    _entities.Entry(_tblAssignment).State = System.Data.Entity.EntityState.Modified;

                                    //Add Log 
                                    tblAssignmentLog _log = new tblAssignmentLog();
                                    _log.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                                    _log.AssignmentID = _tblAssignment.AssignmentID;
                                    _log.CreatedDate = DateTime.Now;
                                    _log.CreatedBy = _tblAssignment.LockedByUserID;
                                    _log.Description = strSheetName + " has been updated from excel";
                                    //_log.DocumentName = model.OriginalFile;
                                    _entities.tblAssignmentLogs.Add(_log);
                                    _entities.SaveChanges();

                                    //string message = string.Empty;
                                    //var modifiedSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == AssignmentID && z.IsPPTModified.HasValue && z.IsPPTModified.Value == true).Count();
                                    //var totalSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == AssignmentID).Count();
                                    //if (modifiedSlidesCount == totalSlidesCount)
                                    //{
                                    //    _tblAssignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                                    //    _tblAssignment.CompletedDate = DateTime.Now;
                                    //    _entities.SaveChanges();
                                    //    message = "Task has been completed successfully.";
                                    //}
                                    //else
                                    //{
                                    //    message = "PPT has been saved successfully in triyosoft platform.";
                                    //}
                                    objAvailabilityResult.Status = true;
                                    objAvailabilityResult.ErrorCode = "";
                                    objAvailabilityResult.ErrorMessage = "Excel has been saved successfully in triyosoft platform.";
                                }
                                else
                                {
                                    objAvailabilityResult.Status = false;
                                    objAvailabilityResult.ErrorCode = WordAutomationDemo.Common.Global.Action.Completed.ToString();
                                    objAvailabilityResult.ErrorMessage = "Task already completed";
                                }
                            }
                        }
                    }
                    else
                    {
                        objAvailabilityResult.ErrorMessage = "Improper data";
                    }
                }
                else {
                    objAvailabilityResult.ErrorMessage = "File not found";
                }
            return objAvailabilityResult.ErrorMessage;
        }

        public void SendErrorToText(string message, string functionName = "")
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
                        sw.WriteLine(message);
                    }
                }
        }
        //[HttpPost]
        //public Result AddSlide()
        //{
        //    Result objAvailabilityResult = new Result();
        //    return objAvailabilityResult;
        //}

        [HttpPost]
        [AllowAnonymous]
        public JsonResult ProcessFile()
        {
            List<SelectListItem> documentFileCollection = new List<SelectListItem>();

            documentFileCollection = Directory.GetFiles(DirectoryManagmentUtils.InitialDemoFilesPathDemo, "*.*", SearchOption.TopDirectoryOnly)
                                      .Where(s => s.EndsWith(".doc", StringComparison.OrdinalIgnoreCase) || s.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                                      .Select(x => new SelectListItem
                                      {
                                          Text = x.ToString().Split('\\').Last(),
                                          Value = x.ToString().Split('\\').Last()
                                      }).ToList();
            using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
            {
                foreach (var item in documentFileCollection)
                {
                    tblDocument objtblDocument = new tblDocument();
                    objtblDocument.DisplayName = item.Text.Trim();
                    objtblDocument.DocumentName = item.Text.Trim();
                    objtblDocument.DocumentType = (int)Global.DocumentType.Word;
                    objtblDocument.CreatedBy = 6;
                    objtblDocument.CreatedDate = DateTime.Now;
                    _entities.tblDocuments.Add(objtblDocument);
                    _entities.SaveChanges();
                }

                List<SelectListItem> documentFileCollectionppt = new List<SelectListItem>();

                documentFileCollectionppt
                    = (_entities.tblPPTSlides.Where(x => !(x.IsOriginal == true)).Select(z => z.MasterDocumentName).Distinct().ToList())
                                                    .Select(x => new SelectListItem
                                                    {
                                                        Text = x.ToString(),
                                                        Value = x.ToString()
                                                    }).ToList();

                foreach (var item in documentFileCollectionppt)
                {
                    tblDocument objtblDocument = new tblDocument();
                    objtblDocument.DisplayName = item.Text.Trim();
                    objtblDocument.DocumentName = item.Text.Trim();
                    objtblDocument.DocumentType = (int)Global.DocumentType.Ppt;
                    objtblDocument.CreatedBy = 6;
                    objtblDocument.CreatedDate = DateTime.Now;
                    _entities.tblDocuments.Add(objtblDocument);
                    _entities.SaveChanges();
                }

            }

            return Json("success", JsonRequestBehavior.AllowGet);
        }

	}
}