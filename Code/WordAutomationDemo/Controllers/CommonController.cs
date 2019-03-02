using DevExpress.Spreadsheet;
using DevExpress.XtraRichEdit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Controllers
{
    public class CommonController : Controller
    {

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ProcessTask()
        {
            bool IsLogin = true;
                if (Request.QueryString["AID"] != null && Request.QueryString["Action"] != null)
                {
                    string strAssignmentID = Global.Encryption.Decrypt(Request.QueryString["AID"]);
                    int AssignmentID;
                    string strActionID = Global.Encryption.Decrypt(Request.QueryString["Action"]);
                    int ActionID;
                    if (int.TryParse(strAssignmentID, out AssignmentID))
                    {
                        if (int.TryParse(strActionID, out ActionID))
                        {
                            using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                            {
                                var doc = (from items in _entities.tblAssignments
                                           where items.AssignmentID == AssignmentID
                                           select items).FirstOrDefault();

                                if (doc != null)
                                {
                                    if (CurrentUserSession.User == null)
                                    {
                                        IsLogin = false;
                                        var user = (from u in _entities.tblUserDepartments
                                                    join r in _entities.tblRoles on u.RoleID equals r.RoleID
                                                    where u.UserId == doc.CreatedBy
                                                    select new UserModel
                                                    {
                                                        UserId = u.UserId,
                                                        UserName = u.UserName,
                                                        FullName = u.FullName,
                                                        Role_ID = u.RoleID.HasValue ? u.RoleID.Value : 0,
                                                        Role = r.Role,
                                                        CanCreateSubtasks = u.CanCreateSubtasks.HasValue ? u.CanCreateSubtasks.Value : false,
                                                        CanApprove = u.CanApprove.HasValue ? u.CanApprove.Value : false,
                                                        CanEdit = u.CanEdit.HasValue ? u.CanEdit.Value : false
                                                    }).FirstOrDefault();

                                        CurrentUser User = new CurrentUser();
                                        User.UserName = user.UserName;
                                        User.FirstName = user.FullName;
                                        User.strUserID = user.UserId.ToString();
                                        User.UserID = user.UserId;
                                        User.RoleID = user.Role_ID;
                                        CurrentUserSession.User = User;
                                    }
                                    if (CurrentUserSession.User != null && CurrentUserSession.User.UserID == doc.CreatedBy)
                                    {
                                        if (doc.Action != ActionID)
                                        {
                                            if (doc.Action == (int)WordAutomationDemo.Common.Global.Action.Declined || doc.Action == (int)WordAutomationDemo.Common.Global.Action.Approved)
                                            {
                                                if (IsLogin)
                                                {
                                                    return RedirectToAction("Approval", "Home", new { @Msg = "Processed" });
                                                }
                                                else
                                                {
                                                    Session.Clear();
                                                    Session.RemoveAll();
                                                    Session.Abandon();
                                                    return Redirect(Global.SiteUrl + "Common/Index?Msg=Processed");
                                                }
                                            }
                                            if (ActionID != (int)WordAutomationDemo.Common.Global.Action.Declined)
                                            {
                                                HomeController _homeController = new HomeController();
                                                if (ActionID == (int)WordAutomationDemo.Common.Global.Action.Approved && doc.DocumentType == (int)Global.DocumentType.Xls)
                                                {
                                                    var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == AssignmentID);
                                                    var originalFile = _entities.tblDocuments.FirstOrDefault(d => d.DocumentID == assignment.DocumentID).DocumentName;
                                                    var path = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\tmp\\" + assignment.AssignmentID.ToString() + "-c-" + originalFile;
                                                    var workbook = new Aspose.Cells.Workbook(path);
                                                    _homeController.SaveExcelForUser(workbook, AssignmentID, originalFile, path);
                                                    assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;

                                                    //Add log
                                                    tblAssignmentLog _log = new tblAssignmentLog();
                                                    _log.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
                                                    _log.AssignmentID = assignment.AssignmentID;
                                                    _log.CreatedDate = DateTime.Now;
                                                    _log.CreatedBy = CurrentUserSession.UserID;
                                                    _entities.tblAssignmentLogs.Add(_log);
                                                    _entities.SaveChanges();
                                                }
                                                else if (ActionID == (int)WordAutomationDemo.Common.Global.Action.Approved && doc.DocumentType == (int)Global.DocumentType.Ppt)
                                                {
                                                    var AssignedSlides = (from items in _entities.tblAssignedPPTSlides.AsEnumerable()
                                                                          join q in _entities.tblAssignments.AsEnumerable() on items.AssignmentID equals q.AssignmentID
                                                                          join docu in _entities.tblDocuments on q.DocumentID equals docu.DocumentID
                                                                          where items.AssignmentID == AssignmentID && items.IsPPTModified.HasValue && items.IsPPTModified.Value
                                                                          && !(items.IsPPTApproved.HasValue && items.IsPPTApproved.Value == true)
                                                                          orderby items.Sequence
                                                                          select new PPTModel()
                                                                          {
                                                                              AssignedTaskID = items.AssignmentID,
                                                                              OriginalFile = docu.DocumentName,
                                                                              OriginalSlideName = items.SlideName,
                                                                              SlideName = "New_" + items.AssignmentID.ToString() + "_Copy_" + items.SlideName,
                                                                              IsPPTApproved = items.IsPPTApproved,
                                                                              AssignedPPTSlideID = items.AssignedPPTSildeID,
                                                                          }).ToList();

                                                    string OrginalSourceFile = AssignedSlides.FirstOrDefault().OriginalFile;

                                                    int LastEntryForAll = Convert.ToInt32(_entities.tblPPTSlides.Where(c => c.MasterDocumentName == OrginalSourceFile && !(c.IsOriginal == true)).OrderByDescending(c => c.Sequence).Select(c => c.PPTSlidesID).FirstOrDefault());

                                                    foreach (var item in AssignedSlides)
                                                    {
                                                        var LastFile = _entities.tblPPTSlides.Where(c => c.MasterDocumentName == OrginalSourceFile && !(c.IsOriginal == true)).OrderByDescending(c => c.PPTSlidesID).FirstOrDefault();

                                                        var MyOriFile = _entities.tblPPTSlides.Where(c => c.MasterDocumentName == OrginalSourceFile && c.SlideName == item.OriginalSlideName && !(c.IsOriginal == true)).FirstOrDefault();

                                                        int MyFileSeq = Convert.ToInt32(MyOriFile.Sequence);

                                                        int LastSlide = Convert.ToInt32(LastFile.SlideName.Split('.')[0]);

                                                        //Reinitiate Sequence
                                                        var InBetweenLst = _entities.tblPPTSlides.Where(c => c.MasterDocumentName == OrginalSourceFile && !(c.IsOriginal == true) && (c.Sequence > MyFileSeq)).OrderBy(a => a.Sequence).ToList();

                                                        int FirstEntryID = _homeController.SplitCompletedSlides(item.OriginalFile, item.SlideName, LastSlide + 1, MyFileSeq, item.OriginalSlideName);

                                                        int GetLastSeq = Convert.ToInt32(_entities.tblPPTSlides.Where(c => c.MasterDocumentName == OrginalSourceFile && !(c.IsOriginal == true)).OrderByDescending(c => c.PPTSlidesID).Select(c => c.Sequence).FirstOrDefault());

                                                        foreach (var inbet in InBetweenLst)
                                                        {
                                                            GetLastSeq++;

                                                            inbet.Sequence = GetLastSeq;
                                                        }

                                                        var pptSlide = (from itm in _entities.tblAssignedPPTSlides
                                                                        where itm.AssignedPPTSildeID == item.AssignedPPTSlideID
                                                                        select itm).FirstOrDefault();

                                                        if (pptSlide != null)
                                                        {
                                                            pptSlide.IsPPTApproved = true;
                                                            _entities.Entry(pptSlide).State = System.Data.Entity.EntityState.Modified;
                                                        }

                                                        _entities.SaveChanges();
                                                    }
                                                }
                                                RichEditDocumentServer server = new RichEditDocumentServer();
                                                if (_homeController.UpdateTask(doc.DocumentFile, ActionID.ToString(), server, "", "", 0, doc.AssignmentID, false, false, 0))
                                                {
                                                    if (ActionID == (int)WordAutomationDemo.Common.Global.Action.Approved)
                                                    {
                                                        if (IsLogin)
                                                        {
                                                            return RedirectToAction("Approval", "Home", new { @Msg = "Approved" });
                                                        }
                                                        else
                                                        {
                                                            Session.Clear();
                                                            Session.RemoveAll();
                                                            Session.Abandon();
                                                            return Redirect(Global.SiteUrl + "Common/Index?Msg=Approved");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (IsLogin)
                                                        {
                                                            return RedirectToAction("Approval", "Home", new { @Msg = "Declined" });
                                                        }
                                                        else
                                                        {
                                                            Session.Clear();
                                                            Session.RemoveAll();
                                                            Session.Abandon();
                                                            return Redirect(Global.SiteUrl + "Common/Index?Msg=Declined");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (IsLogin)
                                                    {
                                                        return RedirectToAction("Approval", "Home", new { @Msg = "Error" });
                                                    }
                                                    else
                                                    {
                                                        Session.Clear();
                                                        Session.RemoveAll();
                                                        Session.Abandon();
                                                        return Redirect(Global.SiteUrl + "Common/Index?Msg=Error");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (!IsLogin)
                                                {
                                                    Session.Clear();
                                                    Session.RemoveAll();
                                                    Session.Abandon();
                                                }
                                                var demoModel = new AssignmentModel() { AssignID = doc.AssignmentID };
                                                return View("ProcessTask", demoModel);
                                            }
                                        }
                                        else
                                        {
                                            if (IsLogin)
                                            {
                                                return RedirectToAction("Approval", "Home", new { @Msg = "Processed" });
                                            }
                                            else
                                            {
                                                Session.Clear();
                                                Session.RemoveAll();
                                                Session.Abandon();
                                                return Redirect(Global.SiteUrl + "Common/Index?Msg=Processed");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (IsLogin)
                                        {
                                            return RedirectToAction("Approval", "Home", new { @Msg = "NoFound" });
                                        }
                                        else
                                        {
                                            Session.Clear();
                                            Session.RemoveAll();
                                            Session.Abandon();
                                            return Redirect(Global.SiteUrl + "Common/Index?Msg=NoFound");
                                        }
                                    }
                                }
                                else
                                {
                                    if (IsLogin)
                                    {
                                        return RedirectToAction("Approval", "Home", new { @Msg = "NoFound" });
                                    }
                                    else
                                    {
                                        Session.Clear();
                                        Session.RemoveAll();
                                        Session.Abandon();
                                        return Redirect(Global.SiteUrl + "Common/Index?Msg=NoFound");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (IsLogin)
                            {
                                return RedirectToAction("Approval", "Home", new { @Msg = "Error" });
                            }
                            else
                            {
                                Session.Clear();
                                Session.RemoveAll();
                                Session.Abandon();
                                return Redirect(Global.SiteUrl + "Common/Index?Msg=Error");
                            }
                        }
                    }
                    else {
                        if (IsLogin)
                        {
                            return RedirectToAction("Approval", "Home", new { @Msg = "NoFound" });
                        }
                        else
                        {
                            Session.Clear();
                            Session.RemoveAll();
                            Session.Abandon();
                            return Redirect(Global.SiteUrl + "Common/Index?Msg=NoFound");
                        }
                    }
                }
                else
                {
                    if (IsLogin)
                    {
                        return RedirectToAction("Approval", "Home", new { @Msg = "UrlNotFound" });
                    }
                    else
                    {
                        Session.Clear();
                        Session.RemoveAll();
                        Session.Abandon();
                        return Redirect(Global.SiteUrl + "Common/Index?Msg=UrlNotFound");
                    }
                }
        }

        [HttpPost]
        public ActionResult ProcessTask(AssignmentModel objAssignmentModel)
        {
            bool IsLogin = true;
                if (objAssignmentModel != null && objAssignmentModel.AssignID > 0)
                {
                    using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                    {
                        var doc = (from items in _entities.tblAssignments
                                   where items.AssignmentID == objAssignmentModel.AssignID
                                   select items).FirstOrDefault();

                        if (doc != null)
                        {

                            if (CurrentUserSession.User == null)
                            {
                                IsLogin = false;
                                var user = (from u in _entities.tblUserDepartments
                                            join r in _entities.tblRoles on u.RoleID equals r.RoleID
                                            where u.UserId == doc.CreatedBy
                                            select new UserModel
                                            {
                                                UserId = u.UserId,
                                                UserName = u.UserName,
                                                FullName = u.FullName,
                                                Role_ID = u.RoleID.HasValue ? u.RoleID.Value : 0,
                                                Role = r.Role,
                                                CanCreateSubtasks = u.CanCreateSubtasks.HasValue ? u.CanCreateSubtasks.Value : false,
                                                CanEdit = u.CanEdit.HasValue ? u.CanEdit.Value : false,
                                                CanApprove = u.CanApprove.HasValue ? u.CanApprove.Value : false
                                            }).FirstOrDefault();

                                CurrentUser User = new CurrentUser();
                                User.UserName = user.UserName;
                                User.FirstName = user.FullName;
                                User.strUserID = user.UserId.ToString();
                                User.UserID = user.UserId;
                                User.RoleID = user.Role_ID;
                                CurrentUserSession.User = User;
                            }

                            RichEditDocumentServer server = new RichEditDocumentServer();
                            HomeController _homeController = new HomeController();
                            if (_homeController.UpdateTask(doc.DocumentFile, Convert.ToString((int)Global.Action.Declined), server, "", objAssignmentModel.Remarks2, 0, doc.AssignmentID, false, false, 0))
                            {
                                if (IsLogin)
                                {
                                    return RedirectToAction("Approval", "Home", new { Msg = "Declined" });
                                }
                                else
                                {
                                    Session.Clear();
                                    Session.RemoveAll();
                                    Session.Abandon();
                                    return Redirect(Global.SiteUrl + "Common/Index?Msg=Declined");
                                }
                            }
                            else
                            {
                                if (IsLogin)
                                {
                                    return RedirectToAction("Approval", "Home", new { Msg = "Error" });
                                }
                                else
                                {
                                    Session.Clear();
                                    Session.RemoveAll();
                                    Session.Abandon();
                                    return Redirect(Global.SiteUrl + "Common/Index?Msg=Error");
                                }
                            }
                        }
                        else
                        {
                            if (IsLogin)
                            {
                                return RedirectToAction("Approval", "Home", new { Msg = "NoFound" });
                            }
                            else
                            {
                                Session.Clear();
                                Session.RemoveAll();
                                Session.Abandon();
                                return Redirect(Global.SiteUrl + "Common/Index?Msg=NoFound");
                            }
                        }
                    }
                }
                else
                {
                    if (IsLogin)
                    {
                        return RedirectToAction("Approval", "Home", new { @Msg = "UrlNotFound" });
                    }
                    else
                    {
                        Session.Clear();
                        Session.RemoveAll();
                        Session.Abandon();
                        return Redirect(Global.SiteUrl + "Common/Index?Msg=UrlNotFound");
                    }
                }
        }

        public void SendErrorToText(Exception ex, string functionName = "")
        {
                string fileBasePath = System.Web.HttpContext.Current.Server.MapPath("~/CSS");  //Text File Path
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
    }
}