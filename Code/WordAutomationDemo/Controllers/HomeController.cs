using DevExpress.Office;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Web.Office;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Export;
using HtmlAgilityPack;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DevExpress.Spreadsheet;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml;
using System.Drawing;
using System.Data.Entity.Validation;
using System.Collections.ObjectModel;
using System.Collections;
using System.Drawing.Imaging;
using WordAutomationDemo.SignalR;
using Microsoft.Exchange.WebServices.Data;
using System.Net;
using Z.EntityFramework.Extensions;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Aspose.Pdf.Devices;
using Aspose.Pdf.Facades;
using Aspose.Words;
using Aspose.Words.Layout;
using Aspose.Words.Rendering;
using Aspose.Words.Replacing;
using Aspose.Words.Saving;
using Newtonsoft.Json;
using HeaderFooter = Aspose.Words.HeaderFooter;
using HeaderFooterType = Aspose.Words.HeaderFooterType;
using Image = System.Drawing.Image;
using Paragraph = Aspose.Words.Paragraph;
using Section = Aspose.Words.Section;
using Shape = DocumentFormat.OpenXml.Presentation.Shape;

namespace WordAutomationDemo.Controllers
{
    public class HomeController : Controller
    {
        int _Count = 0;
        public static ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
        List<string> colors = new List<string>();

        public HomeController()
        {
            Aspose.Words.License wordsLicense = new Aspose.Words.License();
            Aspose.Cells.License cellsLicense = new Aspose.Cells.License();
            Aspose.Slides.License slidesLicense = new Aspose.Slides.License();
            Aspose.Pdf.License pdfLicense = new Aspose.Pdf.License();
            wordsLicense.SetLicense("Aspose.Total.lic");
            cellsLicense.SetLicense("Aspose.Total.lic");
            slidesLicense.SetLicense("Aspose.Total.lic");
            pdfLicense.SetLicense("Aspose.Total.lic");
        }

        #region Login

        [HttpGet]
        public ActionResult Login()
        {
            if (CurrentUserSession.UserID > 0)
                return RedirectToAction("Approval", "Home");

            else
                return View();
        }

        [HttpPost]
        public ActionResult Login(int? id)
        {
            string UserName = Convert.ToString(Request.Form["UserName"]);
            string Password = Convert.ToString(Request.Form["UserPassword"]);
            var resp = CheckLogin(UserName, Password);
            if (resp != null && resp.UserId > 0)
            {
                if (Global.Encryption.Decrypt(resp.Password) == Password)
                {
                    CurrentUser User = new CurrentUser();
                    User.UserName = resp.UserName;
                    User.FirstName = resp.FullName;
                    User.strUserID = resp.UserId.ToString();
                    User.UserID = resp.UserId;
                    User.RoleID = resp.Role_ID;
                    User.CompanyID = resp.Company_ID;
                    User.IsSuperAdmin = resp.IsSuperAdmin;
                    User.ProfileImage = resp.ProfileImage;
                    User.CanEdit = resp.CanEdit;
                    User.CanApprove = resp.CanApprove;

                    CurrentUserSession.User = User;
                    Session["EPT"] = EmployeePhoto(User.FirstName);
                    IList<vwRolePrivilege> MenuItems = null;
                    IList<vwRolePrivilege> MainItems = null;

                    var ParentList = (from p in _entities.vwRolePrivileges
                                      where p.RoleID == resp.Role_ID && p.ViewPermission == true
                                      && p.IsActive == 1
                                      select p.ParentID).ToList();

                    MainItems = _entities.vwRolePrivileges.Where(p => p.ParentID == 0
                                                                         && p.RoleID == resp.Role_ID).OrderBy(p => p.SortOrder).ToList();

                    MenuItems = _entities.vwRolePrivileges.Where(p => p.RoleID == resp.Role_ID
                                                                         && p.ViewPermission == true
                                                                         && p.IsActive == 1)
                                                                         .OrderBy(p => p.OrderID)
                                                                         .ThenBy(p => p.ParentID)
                                                                         .ThenBy(p => p.SortOrder)
                                                                         .ThenBy(p => p.MenuItem).ToList();

                    Session["MainItems"] = MainItems;
                    Session["MenuItems"] = MenuItems;


                    //// REDIRECT ON THE BASES OF RIGHTS 
                    //var permitted = CurrentUserSession.Permission.Where(x => x.ViewPermission == true).ToList();
                    //var observable = new ObservableCollection<MenuModel>();
                    //foreach (var item in permitted)
                    //{
                    //    observable.Add(item);
                    //}

                    //var MyTaskItem = permitted.Where(x => x.Action == "Approval" && x.Controller == "Home").FirstOrDefault();
                    //if (MyTaskItem != null && MyTaskItem.MenuItemID > 0)
                    //{
                    //    int MyTaskItemIndex = permitted.IndexOf(MyTaskItem);
                    //    observable.Move(MyTaskItemIndex, 0);
                    //}
                    //var CreateTaskItem = observable.Where(x => x.Action == "CreateAssignment" && x.Controller == "Home").FirstOrDefault();
                    //if (CreateTaskItem != null && CreateTaskItem.MenuItemID > 0)
                    //{
                    //    int CreateTaskItemIndex = observable.IndexOf(CreateTaskItem);
                    //    observable.Move(CreateTaskItemIndex, (observable.Count - 1));
                    //}
                    //var OrderedPermittedMenus = observable.ToList();
                    //if (OrderedPermittedMenus != null && OrderedPermittedMenus.Count > 0)
                    //{
                    //    return RedirectToAction(OrderedPermittedMenus.FirstOrDefault().Action, OrderedPermittedMenus.FirstOrDefault().Controller);
                    //}
                    //else
                    //{
                    //    return RedirectToAction("Approval", "Home");
                    //}
                    return RedirectToAction("Approval", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Messages.WrongCredentials);
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.WrongCredentials);
                return View();
            }
        }

        public UserModel CheckLogin(string userName, string Password)
        {
            try
            {
                using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                {
                    return (from items in _entities.tblUserDepartments
                            where items.UserName == userName && items.IsActive == true
                            select new UserModel
                            {
                                Department = items.Department,
                                FullName = items.FullName,
                                Password = items.Password,
                                UserId = items.UserId,
                                UserName = items.UserName,
                                Role_ID = items.RoleID.HasValue ? items.RoleID.Value : 0,
                                Company_ID = items.tblCompany != null ? items.tblCompany.CompanyID : 0,
                                IsSuperAdmin = items.tblCompany != null && items.tblCompany.IsSuperAdmin.HasValue && items.tblCompany.IsSuperAdmin.Value ? true : false,
                                ProfileImage = items.ProfileImage,
                                CanCreateSubtasks = items.CanCreateSubtasks.HasValue ? items.CanCreateSubtasks.Value : false,
                                CanEdit = items.CanEdit.HasValue ? items.CanEdit.Value : false,
                                CanApprove = items.CanApprove.HasValue ? items.CanApprove.Value : false
                            }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

                SendErrorToText(ex, "CheckLogin");
                return null;
            }
        }
        #endregion

        #region Get Users List for Instant Message
        [HttpPost]
        public JsonResult GetUsersListForInstantMessage()
        {
            List<ChatUser> chatUsers = ChatHub.GetAllUsers();
            int UserID = CurrentUserSession.UserID;
            var objValidateUserList = (from items in _entities.tblUserDepartments
                                       where items.UserId != CurrentUserSession.User.UserID
                                       select new UserModel
                                       {
                                           Department = items.Department,
                                           FullName = items.FullName,
                                           Password = items.Password,
                                           UserId = items.UserId,
                                           UserName = items.UserName,
                                           EmailID = items.EmailID

                                       }).ToList();
            int C = 0;
            string HTML = "<table width='100%'>";
            HTML += "<tr><td style='width:50%;'><input type='checkbox' style='margin-right:5px;' id='chkChatSelAll' name='chkChatSelAll' onclick='SetChatUserChkbox();'/>Select All</td></tr>";
            foreach (var user in objValidateUserList)
            {
                string UserImage = Url.Content("~/" + EmployeePhoto(user.FullName));
                string RandomColor = GetColor(C);
                C += 1;
                if (C % 2 == 0)
                {
                    HTML += "<td style='width:50%;height:35px;'><input type='checkbox' style='margin-right:5px;' id='" + user.UserId + "' name='cbIMUser'/><img src='" + UserImage + "' class='profile-picture' style='height:25px;width:25px;background: none repeat scroll 0 0 " + RandomColor + ";padding-right: 0px;' />&nbsp;&nbsp;" + user.FullName + "</td></tr>";
                }
                else
                {
                    if (objValidateUserList.Count() == C)
                    {
                        HTML += "<tr><td style='width:50%;height:35px;'><input type='checkbox' style='margin-right:5px;' id='" + user.UserId + "' name='cbIMUser'/><img src='" + UserImage + "' class='profile-picture' style='height:25px;width:25px;background: none repeat scroll 0 0 " + RandomColor + ";padding-right: 0px;' />&nbsp;&nbsp;" + user.FullName + "</td></tr>";
                    }
                    else
                    {
                        HTML += "<tr><td style='width:50%;height:35px;'><input type='checkbox' style='margin-right:5px;' id='" + user.UserId + "' name='cbIMUser'/><img src='" + UserImage + "' class='profile-picture' style='height:25px;width:25px;background: none repeat scroll 0 0 " + RandomColor + ";padding-right: 0px;' />&nbsp;&nbsp;" + user.FullName + "</td>";
                    }
                }
            }
            HTML += "</table>";
            return Json(new
            {
                @UsersHTML = HTML
            });
        }
        #endregion

        #region Get Users List for Instant Message
        [HttpPost]
        public JsonResult GetUsersList()
        {
            List<ChatUser> chatUsers = ChatHub.GetAllUsers();
            int UserID = CurrentUserSession.UserID;
            //var objValidateUserList = (from items in _entities.tblUserDepartments
            //                           where items.UserId != CurrentUserSession.User.UserID && items.CompanyID == CurrentUserSession.User.CompanyID
            //                           select new UserModel
            //                           {
            //                               FullName = items.FullName,
            //                               UserId = items.UserId,
            //                           }).ToList();

            var objValidateUserList = _entities.tblUserDepartments.Where(u => u.UserId != CurrentUserSession.User.UserID && u.CompanyID == CurrentUserSession.User.CompanyID)
                .Select(um => new UserModel
                {
                    FullName = um.FullName,
                    UserId = um.UserId,

                }).ToList();


            string HTML = "<select multiple id='instantUserID' style='width:300px'>";
            foreach (var user in objValidateUserList)
            {
                HTML += "<option value=" + user.UserId + ">" + user.FullName + "</option>";
            }
            HTML += "</select>";
            return Json(new
            {
                @UsersHTML = HTML
            });
        }
        #endregion

        #region Send Instant Message
        [HttpPost]
        public string SendInstantMessageToUsers(string SelectedUserIDs, string InstantMessage)
        {
            string Status = "0";
            if (CurrentUserSession.User.UserID > 0)
            {
                int CompanyID = CurrentUserSession.User.CompanyID;
                int UserID = CurrentUserSession.User.UserID;
                string[] IDs = Convert.ToString(SelectedUserIDs) == null ? null : SelectedUserIDs.Split(',');
                foreach (var id in IDs)
                {
                    ChatHub CH = new ChatHub();
                    int SelectedID = Convert.ToInt32(id);
                    CH.SendInstantMessage(SelectedID, InstantMessage, Guid.NewGuid().ToString(), UserID, CompanyID);
                }
                Status = "1";
            }
            else
            {
                Status = "-3"; // Session Expired
            }
            return "{ \"Status\" :" + Status + "}";
        }
        #endregion

        #region Create Assignment

        [HttpGet]
        [ValidateLogin]
        [ValidateUserPermission(Global.Actions.CreateAssignment, Global.Controlers.Home)]
        public ActionResult CreateAssignment(int? ProjectID)
        {
            var Model = new AssignmentModel();
            Model.DocumentName = TempData.ContainsKey("Document") ? TempData["Document"].ToString() : string.Empty;
            if (Model.DocumentName.Split('.').Last().ToLower() == "ppt" || Model.DocumentName.Split('.').Last().ToLower() == "pptx")
            {
                Model.DocumentType = (int)WordAutomationDemo.Common.Global.DocumentType.Ppt;
            }
            else if (Model.DocumentName.Split('.').Last().ToLower() == "doc" || Model.DocumentName.Split('.').Last().ToLower() == "docx")
            {
                Model.DocumentType = (int)WordAutomationDemo.Common.Global.DocumentType.Word;
            }
            else if (Model.DocumentName.Split('.').Last().ToLower() == "xls" || Model.DocumentName.Split('.').Last().ToLower() == "xlsx")
            {
                Model.DocumentType = (int)WordAutomationDemo.Common.Global.DocumentType.Xls;
            }
            else
            {
                Model.DocumentType = (int)WordAutomationDemo.Common.Global.DocumentType.SimpleTask;
            }

            //Model.MemberList = GetAllMembers();
            Model.myProjectList = GetAllProjects(CurrentUserSession.UserID);
            Model.WordList = CommonHelper.GetDocFilesInDirectory(_entities);
            //Model.PPTList = GetPPTFilesInDirectory();
            //Model.WordList = GetDocFilesInDirectory();
            Model.MemberList = GetAllMembers();

            if (ProjectID.HasValue && ProjectID > 0)
            {
                Model.ProjectID = ProjectID.Value;
                Model.UserList = GetMembersByProjectID(ProjectID.Value);
            }



            return View(Model);
        }

        [HttpPost]
        [ValidateLogin]
        [ValidateUserPermission(Global.Actions.CreateAssignment, Global.Controlers.Home)]
        public ActionResult CreateAssignment(AssignmentModel AssignmentModel)
        {
            var assignmentMembers = JsonConvert.DeserializeObject<List<AssignmentMemberModel>>(AssignmentModel.AssignmentMemberList);
            if (assignmentMembers.Count > 0)
            {
                //REMOVE WHEN USERID FIELD IS DELETED
                var assignmentMember = assignmentMembers.FirstOrDefault();

                DateTime? MyDueDate = string.IsNullOrEmpty(AssignmentModel.DueDate) ? (DateTime?)null : DateTime.ParseExact(Convert.ToString(AssignmentModel.DueDate), WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid, null, System.Globalization.DateTimeStyles.AssumeLocal);
                var newModel = new AssignmentModel()
                {
                    myProjectList = GetAllProjects(CurrentUserSession.UserID),
                    DocumentType = AssignmentModel.DocumentType,
                    WordList = CommonHelper.GetDocFilesInDirectory(_entities),
                    //AssignmentMembers = assignmentMembers,
                    MemberList = GetAllMembers(),
                    ProjectID = AssignmentModel.ProjectID,
                    UserList = GetMembersByProjectID(AssignmentModel.ProjectID),
                    DueDate = AssignmentModel.DueDate,
                    IsFullscreen = AssignmentModel.IsFullscreen,
                    Comments = ""
                };

                if (AssignmentModel.DocumentType == (int)WordAutomationDemo.Common.Global.DocumentType.Ppt)
                {
                    #region  PPT Assignment
                    AssignmentModel.SelectedSlides = AssignmentModel.SelectedSlides.FirstOrDefault().Split(',').ToList();
                    if (AssignmentModel.SelectedSlides.Count > 0 && !string.IsNullOrEmpty(AssignmentModel.DocumentName))
                    {
                        ReadyPortalDBEntities _context = new ReadyPortalDBEntities();

                        DocumentModel model = new DocumentModel()
                        {
                            //Content = plainText,
                            //DocumentFile = AssignmentModel.DocumentName,
                            //ReplacementCode = lblToInsertInDB,
                            DocumentType = (int)WordAutomationDemo.Common.Global.DocumentType.Ppt,
                            Action = (int)Common.Global.Action.Assigned,
                            UserID = AssignmentModel.UserID,
                            CreatedBy = CurrentUserSession.UserID,
                            OriginalDocument = AssignmentModel.DocumentName,
                            Comment = AssignmentModel.Comments,
                            DueDate = MyDueDate,
                            ProjectID = AssignmentModel.ProjectID,
                            TaskName = AssignmentModel.TaskName,
                            DocumentID = AssignmentModel.DocumentID
                        };
                        var assignementId = SaveSnippet(model);

                        if (assignementId > 0)
                        {
                            for (int i = 0; i < AssignmentModel.SelectedSlides.Count; i++)
                            {
                                tblAssignedPPTSlide ppt = new tblAssignedPPTSlide();
                                ppt.SlideName = AssignmentModel.SelectedSlides[i];
                                ppt.Sequence = i + 1;
                                ppt.AssignmentID = assignementId;
                                _context.tblAssignedPPTSlides.Add(ppt);
                                _context.SaveChanges();
                                //save copy of a file
                                var Path = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "\\Slides_" + AssignmentModel.DocumentName + "\\";
                                if (!System.IO.Directory.Exists(Server.MapPath("~/ApplicationDocuments/PPTs/Slides_" + AssignmentModel.DocumentName + "/AssignedDoc/")))
                                    System.IO.Directory.CreateDirectory(Server.MapPath("~/ApplicationDocuments/PPTs/Slides_" + AssignmentModel.DocumentName + "/AssignedDoc/"));

                                using (FileStream fs = new FileStream(Path + AssignmentModel.SelectedSlides[i], FileMode.OpenOrCreate))
                                {

                                    Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(fs);
                                    presentation.DocumentProperties.Manager = CurrentUserSession.UserID + "|" + assignementId + "|" + "Slides_" + AssignmentModel.DocumentName + "|" + assignementId + "_Copy_" + AssignmentModel.SelectedSlides[i] + "|" + ppt.AssignedPPTSildeID;
                                    presentation.Save(Path + "/AssignedDoc/" + assignementId + "_Copy_" + AssignmentModel.SelectedSlides[i], Aspose.Slides.Export.SaveFormat.Pptx);

                                    System.IO.File.Copy(Path + AssignmentModel.SelectedSlides[i].Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention, Path + "/AssignedDoc/" + assignementId + "_Copy_" + AssignmentModel.SelectedSlides[i].Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention); //Copy Thumbnails also 
                                }
                            }

                            //Add new assignment members
                            var newMembers = new List<tblAssignmentMember>();
                            foreach (var member in assignmentMembers)
                            {
                                var newMember = new tblAssignmentMember()
                                {
                                    UserID = member.UserID,
                                    CanEdit = member.CanEdit,
                                    CanApprove = member.CanApprove,
                                    CanPublish = member.CanPublish,
                                    AssignmentID = assignementId,
                                };
                                newMembers.Add(newMember);
                            }
                            _entities.BulkInsert(newMembers);
                        }
                        _context.SaveChanges();

                        NotifyMembersOfAssignment(assignmentMembers, AssignmentModel, assignementId);

                        ViewData.Add("Success", "Task Assigned Successfully");
                        return View("CreateAssignment", newModel);
                    }

                    AssignmentModel.UserList = GetAllUsers(new DataSourceRequest());
                    ViewData.Add("Failure", "Error, please try again later.");
                    return View("CreateAssignment", AssignmentModel);
                    #endregion

                }
                else if (AssignmentModel.DocumentType == (int)WordAutomationDemo.Common.Global.DocumentType.Xls)
                {
                    #region  Excel

                    // Share the rows
                    if (!string.IsNullOrEmpty(AssignmentModel.SelectedRows))
                    {

                        DocumentModel model = new DocumentModel()
                        {
                            //Content = plainText,
                            //DocumentFile = AssignmentModel.DocumentName,
                            //ReplacementCode = lblToInsertInDB,
                            DocumentType = (int)WordAutomationDemo.Common.Global.DocumentType.Xls,
                            Action = (int)Common.Global.Action.Assigned,
                            UserID = assignmentMember.UserID,
                            CreatedBy = CurrentUserSession.UserID,
                            OriginalDocument = AssignmentModel.DocumentName,
                            Comment = AssignmentModel.Comments,
                            DueDate = MyDueDate,
                            ProjectID = AssignmentModel.ProjectID,
                            TaskName = AssignmentModel.TaskName,
                            DocumentID = AssignmentModel.DocumentID
                        };
                        var selection = AssignmentModel.SelectedRows.Split(':');
                        (int topRow, int bottomRow, int sheetIndex) = (selection[0] == "*" ? -1 : int.Parse(selection[0]), selection[1] == "*" ? -1 : int.Parse(selection[1]), selection[2] == "*" ? -1 : int.Parse(selection[2]));

                        var assignementId = SaveSnippet(model);

                        // Share the rows
                        using (var doc = new Aspose.Cells.Workbook(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, AssignmentModel.DocumentName)))
                        {
                            // Parth: Bellowe line commented becasue we need all the sheets with the same selection.
                            //var sheets = topRow == -1 || sheetIndex < 1 ? doc.Worksheets.ToArray() : new Aspose.Cells.Worksheet[] { doc.Worksheets[sheetIndex - 1] };
                            var sheets = doc.Worksheets.ToArray();
                            foreach (var sheet in sheets)
                            {
                                // Skip empty sheets
                                if (sheet.Cells.Rows.Count == 0 || sheet.Cells.MaxDataColumn == 0)
                                    continue;

                                var startRow = topRow == -1 ? 0 : topRow;
                                var endRow = bottomRow == -1 ? sheet.Cells.Rows.Count - 1 : bottomRow;

                                var newRowMappings = new List<tblExcelRowMap>();

                                if (startRow != 0 && endRow != 0)
                                {
                                    using (var guids = sheet.Cells.ExportDataTable(0, 0, 1, 1))
                                    {
                                        guids.Rows[0][0] = guids.Rows[0][0].ToString().Replace("(Do Not Modify) Triyo ", "");

                                        foreach (System.Data.DataRow row in guids.Rows)
                                            newRowMappings.Add(new tblExcelRowMap()
                                            {
                                                DateLastModified = DateTime.Now,
                                                IsActive = true,
                                                IsRemoved = false,
                                                MasterDocumentName = AssignmentModel.DocumentName,
                                                RowId = Guid.Parse(row[0].ToString()),
                                                AssignmentID = assignementId
                                            });
                                        _entities.BulkInsert(newRowMappings);
                                    }
                                }
                                
                                using (var guids = sheet.Cells.ExportDataTable(startRow, 0, endRow - startRow + 1, 1))
                                {
                                    guids.Rows[0][0] = guids.Rows[0][0].ToString().Replace("(Do Not Modify) Triyo ", "");
                                    
                                    foreach (System.Data.DataRow row in guids.Rows)
                                        newRowMappings.Add(new tblExcelRowMap()
                                        {
                                            DateLastModified = DateTime.Now,
                                            IsActive = true,
                                            IsRemoved = false,
                                            MasterDocumentName = AssignmentModel.DocumentName,
                                            RowId = Guid.Parse(row[0].ToString()),
                                            AssignmentID = assignementId
                                        });
                                    _entities.BulkInsert(newRowMappings);
                                }
                            }
                        }


                        //Add new assignment members
                        var newMembers = new List<tblAssignmentMember>();
                        foreach (var member in assignmentMembers)
                        {
                            var newMember = new tblAssignmentMember()
                            {
                                UserID = member.UserID,
                                CanEdit = member.CanEdit,
                                CanApprove = member.CanApprove,
                                CanPublish = member.CanPublish,
                                AssignmentID = assignementId
                            };
                            newMembers.Add(newMember);
                        }
                        _entities.BulkInsert(newMembers);
                        newModel.AssignedMembers = new List<string>();
                        foreach (var UserToNotify in _entities.tblAssignmentMembers.Where(x => x.AssignmentID == assignementId).Join(_entities.tblUserDepartments, x => x.UserID, y => y.UserId, (x, y) => y))
                        {
                            newModel.AssignedMembers.Add(UserToNotify.FullName);
                        }
                        NotifyMembersOfAssignment(assignmentMembers, AssignmentModel, assignementId);
                        ViewData.Add("Success", "Task Assigned Successfully");
                        return View("CreateAssignment", newModel);
                    }
                    else
                    {
                        AssignmentModel.SelectedSheets = AssignmentModel.SelectedSheets.FirstOrDefault().Split(',').ToList();
                        if (AssignmentModel.SelectedSheets.Count > 0 && !string.IsNullOrEmpty(AssignmentModel.DocumentName))
                        {
                            ReadyPortalDBEntities _context = new ReadyPortalDBEntities();

                            DocumentModel model = new DocumentModel()
                            {
                                //Content = plainText,
                                //DocumentFile = AssignmentModel.DocumentName,
                                //ReplacementCode = lblToInsertInDB,
                                DocumentType = (int)WordAutomationDemo.Common.Global.DocumentType.Xls,
                                Action = (int)Common.Global.Action.Assigned,
                                UserID = AssignmentModel.UserID,
                                CreatedBy = CurrentUserSession.UserID,
                                OriginalDocument = AssignmentModel.DocumentName,
                                Comment = AssignmentModel.Comments,
                                DueDate = MyDueDate,
                                ProjectID = AssignmentModel.ProjectID,
                                TaskName = AssignmentModel.TaskName,
                                DocumentID = AssignmentModel.DocumentID
                            };
                            var assignementId = SaveSnippet(model);

                            if (assignementId > 0)
                            {
                                for (int i = 0; i < AssignmentModel.SelectedSheets.Count; i++)
                                {
                                    if (!string.IsNullOrEmpty(AssignmentModel.SelectedSheets[i]))
                                    {
                                        tblAssignedExcelSheet xls = new tblAssignedExcelSheet();
                                        xls.SheetName = AssignmentModel.SelectedSheets[i];
                                        xls.Sequence = i + 1;
                                        xls.AssignmentID = assignementId;
                                        _context.tblAssignedExcelSheets.Add(xls);
                                        _context.SaveChanges();
                                        //save copy of a file
                                        var Path = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\Sheets_" + AssignmentModel.DocumentName + "\\";
                                        if (!System.IO.Directory.Exists(Server.MapPath("~/ApplicationDocuments/Excels/Sheets_" + AssignmentModel.DocumentName + "/AssignedDoc/")))
                                            System.IO.Directory.CreateDirectory(Server.MapPath("~/ApplicationDocuments/Excels/Sheets_" + AssignmentModel.DocumentName + "/AssignedDoc/"));

                                        Workbook workbook = new Workbook();
                                        if (System.IO.File.Exists(Path + AssignmentModel.SelectedSheets[i].ToString()))
                                        {
                                            // Load a workbook from the stream. 
                                            using (FileStream stream = new FileStream(Path + AssignmentModel.SelectedSheets[i].ToString(), FileMode.Open))
                                            {
                                                workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                                                workbook.DocumentProperties.Manager = CurrentUserSession.UserID + "|" + assignementId + "|" + "Sheets_" + AssignmentModel.DocumentName + "|" + assignementId + "_Copy_" + AssignmentModel.SelectedSheets[i] + "|" + xls.AssignedExcelSheetID;
                                                workbook.SaveDocument(Path + "/AssignedDoc/" + assignementId + "_Copy_" + AssignmentModel.SelectedSheets[i], DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                                            }
                                        }
                                    }
                                }
                                //Add new assignment members
                                var newMembers = new List<tblAssignmentMember>();
                                foreach (var member in assignmentMembers)
                                {
                                    var newMember = new tblAssignmentMember()
                                    {
                                        UserID = member.UserID,
                                        CanEdit = member.CanEdit,
                                        CanApprove = member.CanApprove,
                                        AssignmentID = assignementId,
                                        CanPublish = member.CanPublish
                                    };
                                    newMembers.Add(newMember);
                                }
                                _entities.BulkInsert(newMembers);
                            }
                            _context.SaveChanges();

                            newModel.AssignedMembers = new List<string>();
                            foreach (var UserToNotify in _entities.tblAssignmentMembers.Where(x => x.AssignmentID == assignementId).Join(_entities.tblUserDepartments, x => x.UserID, y => y.UserId, (x, y) => y))
                            {
                                AssignmentModel.AssignedMembers.Add(UserToNotify.FullName);
                            }

                            NotifyMembersOfAssignment(assignmentMembers, AssignmentModel, assignementId);

                            AssignmentModel.UserList = GetMembersByProjectID(model.ProjectID);
                            AssignmentModel.PPTList = GetPPTFilesInDirectory();
                            AssignmentModel.myProjectList = GetAllProjects(CurrentUserSession.UserID);
                            AssignmentModel.WordList = CommonHelper.GetDocFilesInDirectory(_entities);
                            AssignmentModel.MemberList = GetAllMembers();
                            AssignmentModel.Comments = string.Empty;
                            AssignmentModel.TaskName = string.Empty;
                            AssignmentModel.DocumentType = (int)WordAutomationDemo.Common.Global.DocumentType.Ppt;
                            ViewData.Add("Success", "Task Assigned Successfully");

                            return View("CreateAssignment", newModel);
                        }
                    }

                    AssignmentModel.UserList = GetAllUsers(new DataSourceRequest());
                    ViewData.Add("Failure", "Error, please try later.");
                    return View("CreateAssignment", AssignmentModel);
                    #endregion

                }
                else if (AssignmentModel.DocumentType == (int)WordAutomationDemo.Common.Global.DocumentType.Word)
                {
                    #region Word Assignment

                    string plainText = String.Empty;
                    string newFileName = String.Empty;
                    var Ticks = DateTime.Now.Ticks.ToString();
                    //string lbl = Characters.Space.ToString() + "#" + Ticks + "#" + Characters.Space.ToString();
                    string lblToInsertInDB = "#" + Ticks + "#";

                    ReadyPortalDBEntities e = new ReadyPortalDBEntities();
                    var memberIds = assignmentMembers.Select(am => am.UserID).ToList();
                    var usernames = e.tblUserDepartments.Where(u => memberIds.Contains(u.UserId))
                        .Select(u => u.FullName).ToList();
                    var fullnames = String.Join(", ", usernames);

                    var newPgAssignments = new List<tblAssignedWordPage>();
                    var assignedPages = new List<WordPageModel>();
                    var docsToSave = new List<Aspose.Words.Document>();
                    var doc = new Aspose.Words.Document(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" +
                                                        AssignmentModel.DocumentName);

                    var replacementCodeHelper = new ReplacementCodeHelper();
                    //Page assignment
                    if ((AssignmentModel.selectedPages != null &&
                        !String.IsNullOrEmpty(AssignmentModel.selectedPages)) ||
                        (AssignmentModel.selectedRanges != null && !String.IsNullOrEmpty(AssignmentModel.selectedRanges)))
                    {
                        List<TextSelectionsModel> selections = new List<TextSelectionsModel>();
                        if (AssignmentModel.selectedPages != null)
                        {
                            selections = JsonConvert
                                .DeserializeObject<List<TextSelectionsModel>>(AssignmentModel.selectedPages)
                                .OrderBy(sp => sp.PageNumber).ToList();
                        }

                        if (AssignmentModel.selectedRanges != null)
                        {
                            selections = JsonConvert
                                .DeserializeObject<List<TextSelectionsModel>>(AssignmentModel.selectedRanges)
                                .OrderBy(sp => sp.PageNumber).ToList();
                        }

                        var totalAdded = 0;
                        foreach (var selection in selections)
                        {
                            var ticks = DateTime.Now.Ticks.ToString();
                            var label = "#" + ticks + "#";
                            var sLabel = "#S" + label + "  [This section has been assigned to: " + fullnames +
                                         "] Please do not edit the below grayed out content.";
                            var eLabel = label + "E#\n";

                            RichEditDocumentServer server = new RichEditDocumentServer();
                            server.LoadDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + AssignmentModel.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                            DevExpress.XtraRichEdit.API.Native.Document document = server.Document;
                            var s = replacementCodeHelper.AddReplacementCode(AssignmentModel, server, document, sLabel, eLabel,
                                selection.start, selection.length, totalAdded, false);

                            totalAdded += sLabel.Length + eLabel.Length + 3;
                            var newPgAssignment = new tblAssignedWordPage
                            {
                                DocName = AssignmentModel.DocumentName,
                                Sequence = selection.PageNumber,
                            };
                            if (AssignmentModel.selectedRanges != null)
                            {
                                newPgAssignment.Ticks = ticks;
                            }
                            newPgAssignments.Add(newPgAssignment);

                            var assignedPage = new WordPageModel
                            {
                                OriginalFile = AssignmentModel.DocumentName,
                                PageNumber = selection.PageNumber,
                                AssignedContent = s,
                                Ticks = ticks
                            };
                            assignedPages.Add(assignedPage);
                        }
                    }
                    else
                    {
                        var Ts = DateTime.Now.Ticks.ToString();
                        newFileName = "_" + Ts + ".docx";
                        var newFileNameCopy = "Copy_" + Ts + ".docx";

                        RichEditDocumentServer server = new RichEditDocumentServer();
                        server.LoadDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + AssignmentModel.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                        DevExpress.XtraRichEdit.API.Native.Document document = server.Document;

                        if (AssignmentModel.IsEntireDocument)
                        {
                            doc.Save(DirectoryManagmentUtils.InitialDemoFilesPath + "/" + newFileName);
                            doc.Save(DirectoryManagmentUtils.InitialDemoFilesPath + "/" + newFileNameCopy);
                        }

                        //Add replacement code here
                        var startLabel = "#S" + lblToInsertInDB + "  [This section has been assigned to: " +
                                             fullnames + "] Please do not edit the below grayed out content.";
                        var endLabel = lblToInsertInDB + "E#\n";
                        var s = replacementCodeHelper.AddReplacementCode(AssignmentModel, server, document, startLabel, endLabel,
                            AssignmentModel.start, AssignmentModel.length, 0, AssignmentModel.IsEntireDocument);

                        var serverUpdated = new RichEditDocumentServer();
                        serverUpdated.CreateNewDocument(false);

                        var docUpdated = serverUpdated.Document;
                        docUpdated.BeginUpdate();
                        docUpdated.AppendHtmlText(s, InsertOptions.KeepSourceFormatting);
                        //docUpdated.DefaultCharacterProperties.ForeColor = System.Drawing.Color.Blue;
                        docUpdated.EndUpdate();

                        if (!AssignmentModel.IsEntireDocument)
                        {
                            serverUpdated.Document.SaveDocument(
                                DirectoryManagmentUtils.InitialDemoFilesPath + "/" + newFileName,
                                DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                            serverUpdated.Document.SaveDocument(
                                DirectoryManagmentUtils.InitialDemoFilesPath + "/" + newFileNameCopy,
                                DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                        }

                        DocumentManager.CloseAllDocuments();
                        serverUpdated.Dispose();
                    }

                    string[] xls = { "xls", "xlsx" };
                    int DocType = xls.Contains(AssignmentModel.DocumentName.Split('.').Last().ToString())
                        ? (int)WordAutomationDemo.Common.Global.DocumentType.Xls
                        : (int)WordAutomationDemo.Common.Global.DocumentType.Word;
                    //save to database 
                    DocumentModel model = new DocumentModel()
                    {
                        Content = plainText,
                        DocumentFile = newFileName,
                        ReplacementCode = lblToInsertInDB,
                        Action = (int)Common.Global.Action.Assigned,
                        UserID = AssignmentModel.UserID,
                        CreatedBy = CurrentUserSession.UserID,
                        OriginalDocument = AssignmentModel.DocumentName,
                        Comment = AssignmentModel.Comments,
                        DueDate = MyDueDate,
                        DocumentType = DocType,
                        ProjectID = AssignmentModel.ProjectID,
                        TaskName = AssignmentModel.TaskName,
                        DocumentID = AssignmentModel.DocumentID,
                        IsEntireDocument = AssignmentModel.IsEntireDocument
                    };

                    var assignementId = SaveSnippet(model);

                    if (newPgAssignments.Count > 0)
                    {
                        var assignedPgsDir = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords,
                            "Pages_" + AssignmentModel.DocumentName, "assignedDocs");
                        var changedPgsDir = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords,
                            "Pages_" + AssignmentModel.DocumentName, "changedDocs");
                        var assignedThumbsDir = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords,
                            "Pages_" + AssignmentModel.DocumentName, "assignedThumbs");
                        var changedThumbsDir = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords,
                            "Pages_" + AssignmentModel.DocumentName, "changedThumbs");

                        if (!Directory.Exists(assignedPgsDir)) Directory.CreateDirectory(assignedPgsDir);
                        if (!Directory.Exists(changedPgsDir)) Directory.CreateDirectory(changedPgsDir);
                        if (!Directory.Exists(assignedThumbsDir)) Directory.CreateDirectory(assignedThumbsDir);
                        if (!Directory.Exists(changedThumbsDir)) Directory.CreateDirectory(changedThumbsDir);

                        //var pageSplitter = new DocumentPageSplitter(doc);
                        foreach (var assignedPage in assignedPages)
                        {
                            var server = new RichEditDocumentServer();
                            server.CreateNewDocument(false);
                            var fileName = assignementId.ToString() + "-" + assignedPage.Ticks + "-" +
                                           assignedPage.PageNumber.ToString() + ".docx";
                            var document = server.Document;
                            document.BeginUpdate();
                            document.AppendHtmlText(assignedPage.AssignedContent,
                                InsertOptions.KeepSourceFormatting);
                            document.DefaultCharacterProperties.ForeColor = System.Drawing.Color.Black;
                            document.EndUpdate();

                            using (var tempStream = new MemoryStream())
                            {
                                server.Document.SaveDocument(tempStream,
                                    DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                                DocumentManager.CloseAllDocuments();
                                server.Dispose();
                                Aspose.Words.Document dstDoc = new Aspose.Words.Document(tempStream);
                                dstDoc.Save(Path.Combine(assignedPgsDir, fileName));
                                dstDoc.Save(Path.Combine(changedPgsDir, fileName));
                                GeneratePageImage(assignedPage.PageNumber, doc, assignedThumbsDir, assignementId);
                                GeneratePageImage(assignedPage.PageNumber, doc, changedThumbsDir, assignementId);
                            }
                        }

                        foreach (var newPgAssignment in newPgAssignments)
                        {
                            newPgAssignment.AssignmentID = assignementId;
                        }

                        _entities.BulkInsert(newPgAssignments);
                    }

                    //Generate new thumbnails
                    SplitPages(AssignmentModel.DocumentName);



                    var newMembers = new List<tblAssignmentMember>();
                    foreach (var member in assignmentMembers)
                    {
                        var newMember = new tblAssignmentMember()
                        {
                            UserID = member.UserID,
                            CanEdit = member.CanEdit,
                            CanApprove = member.CanApprove,
                            CanPublish = member.CanPublish,
                            AssignmentID = assignementId
                        };
                        newMembers.Add(newMember);
                    }

                    _entities.BulkInsert(newMembers);


                    var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                    AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                    objAssignmentModelEmail.TaskName = AssignmentModel.TaskName;
                    objAssignmentModelEmail.ReassignRemarks = AssignmentModel.Comments;
                    List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                    lstAssignmentLogModel = GetAssignmentLog(assignementId);
                    NotifyMembersOfAssignment(assignmentMembers, AssignmentModel, assignementId);

                    ViewData.Add("Success", "Task Assigned Successfully");
                    return View("CreateAssignment", newModel);

                    #endregion
                }
                else
                {
                    DocumentModel model = new DocumentModel()
                    {
                        Action = (int)Common.Global.Action.Assigned,
                        UserID = AssignmentModel.UserID,
                        CreatedBy = CurrentUserSession.UserID,
                        Comment = AssignmentModel.Comments,
                        DueDate = MyDueDate,
                        ProjectID = AssignmentModel.ProjectID,
                        TaskName = AssignmentModel.TaskName,
                        DocumentID = null,
                        IsEntireDocument = AssignmentModel.IsEntireDocument
                    };

                    var assignementId = SaveSnippet(model);

                    var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                    AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                    objAssignmentModelEmail.TaskName = AssignmentModel.TaskName;
                    objAssignmentModelEmail.Comments = AssignmentModel.Comments;
                    List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                    lstAssignmentLogModel = GetAssignmentLog(assignementId);
                    NotifyMembersOfAssignment(assignmentMembers, AssignmentModel, assignementId);
                    ViewData.Add("Success", "Task Assigned Successfully");
                    return View("CreateAssignment", newModel);
                }
            }
            else
            {
                AssignmentModel.UserList = GetMembersByProjectID(AssignmentModel.ProjectID);
                AssignmentModel.PPTList = GetPPTFilesInDirectory();
                AssignmentModel.myProjectList = GetAllProjects(CurrentUserSession.UserID);
                AssignmentModel.WordList = CommonHelper.GetDocFilesInDirectory(_entities);
                AssignmentModel.MemberList = GetAllMembers();
                AssignmentModel.Status = 1;
                ViewData.Add("Failure", "Error, your task could not be assigned.");
                return View("CreateAssignment", AssignmentModel);
            }
        }

        private void NotifyMembersOfAssignment(List<AssignmentMemberModel> assignmentMembers, AssignmentModel model, int assignmentId)
        {
            var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

            AssignmentModel objAssignmentModelEmail = new AssignmentModel();
            objAssignmentModelEmail.TaskName = model.TaskName;
            objAssignmentModelEmail.ReassignRemarks = model.Comments;
            objAssignmentModelEmail.AssignID = assignmentId;
            List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
            lstAssignmentLogModel = GetAssignmentLog(assignmentId);
            string EmailBody = templates.TemplateContentForEmail;
            using (ReadyPortalDBEntities _edx = new ReadyPortalDBEntities())
            {
                foreach (var UserToNotify in _edx.tblAssignmentMembers.Where(x => x.AssignmentID == assignmentId).Join(_edx.tblUserDepartments, x => x.UserID, y => y.UserId, (x, y) => y))
                {
                    templates.Subject = templates.Subject.Replace("#Subject#", "Task Assignment: New task has been assigned to you.");
                    EmailBody = PopulateEmailBody("A task has been Assigned to you", UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody);

                    byte[] fileBytes = CommonHelper.GeneratePDF("A task has been assigned to you", UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel);

                    Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody, fileBytes);

                    if (UserToNotify.IsAppointmentEnable == true)
                    {
                        tblCompany objCompany = _edx.tblCompanies.Where(s => s.CompanyID == UserToNotify.CompanyID).FirstOrDefault();
                        if (objCompany != null && objCompany.IsAppointmentEnable == true)
                        {
                            tblUserDepartment User = _edx.tblUserDepartments.Where(x => x.UserId == CurrentUserSession.User.UserID).FirstOrDefault();
                            CreateAppointment(DateTime.Parse(model.DueDate), User, UserToNotify, model.TaskName);
                        }
                    }
                }
            }
        }

        public void CreateAppointment(DateTime dueDate, tblUserDepartment objUser, tblUserDepartment objUserToNotify, string taskName)
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013);

            service.Url = new Uri(objUserToNotify.tblCompany.ExchangeServerURL);
            string strPassword = Global.Encryption.Decrypt(objUserToNotify.tblCompany.ExchangeServerPassword);
            service.Credentials = new NetworkCredential(objUserToNotify.tblCompany.ExchangeServerUserName, strPassword);
            service.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, objUser.EmailID);

            Appointment meeting = new Appointment(service);

            var templatesAppointment = CommonHelper.GetEmailTemplateContent("Appointment Notification");

            templatesAppointment.Subject = templatesAppointment.Subject.Replace("#Subject#", "Task Appointment: New appointment has been assigned to you.");
            templatesAppointment.TemplateContentForEmail = templatesAppointment.TemplateContentForEmail.Replace("#TaskName#", taskName);
            meeting.Subject = templatesAppointment.Subject;
            meeting.Body = templatesAppointment.TemplateContentForEmail;
            meeting.Start = dueDate;
            meeting.End = dueDate;
            meeting.RequiredAttendees.Add(objUserToNotify.EmailID);
            meeting.ReminderMinutesBeforeStart = 60;

            // Save the meeting to the Calendar folder and send the meeting request.
            meeting.Save(SendInvitationsMode.SendOnlyToAll);
            //meeting.Save(SendInvitationsMode.SendOnlyToAll);
        }

        #endregion


        #region GenerateTemplate

        [HttpPost]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult GenerateTemplate(AssignmentModel AssignmentModel)
        {
            var DocumentName = AssignmentModel.OriginalDocumentName;
            if (!string.IsNullOrEmpty(DocumentName))
            {
                //Save pending changes and get text
                byte[] arr = RichEditExtension.SaveCopy("RichEdit", DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                DocumentSaveOptions ds = new DocumentSaveOptions();
                var zz = ds.CurrentFileName;
                Stream stream = new MemoryStream(arr);

                var server = new RichEditDocumentServer();
                server.LoadDocument(stream, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);

                string richEditString = server.Document.HtmlText;
                if (server.Document.Text.Trim().Length > 0)
                    server.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathTemplates + "/" + DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);

                DocumentManager.CloseAllDocuments();
                server.Dispose();

                server = new RichEditDocumentServer();
                server.LoadDocument(DirectoryManagmentUtils.InitialDemoFilesPathTemplates + "/" + DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                //Get all Sections from database 
                var sections = _entities.tblSectionMasters.Where(z => z.IsDeleted != true && z.Status == true).ToList();
                if (sections != null && sections.Count > 0)
                {
                    foreach (var item in sections)
                    {
                        x:
                        var URL = item.SectionURL;
                        DocumentRange URLrange = server.Document.FindAll(URL, DevExpress.XtraRichEdit.API.Native.SearchOptions.None).FirstOrDefault();
                        if (URLrange != null)
                        {
                            if (item.ContentFile.Split('.').Last() == "xls" || item.ContentFile.Split('.').Last() == "xlsx") // if xls file 
                            {
                                // Load a workbook from the stream. 
                                Workbook workbook = new Workbook();
                                var tempHtmlFile = "temp_" + DateTime.Now.Ticks.ToString() + ".html";
                                workbook.LoadDocument(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + item.ContentFile);

                                Worksheet worksheet = workbook.Worksheets[0];
                                workbook.Worksheets.ActiveWorksheet = worksheet;
                                DevExpress.XtraSpreadsheet.Export.HtmlDocumentExporterOptions options = new DevExpress.XtraSpreadsheet.Export.HtmlDocumentExporterOptions();
                                // Specify the part of the document to be exported to HTML.
                                options.SheetIndex = worksheet.Index;
                                // Export the active worksheet to a stream as HTML with the specified options.
                                workbook.ExportToHtml(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + tempHtmlFile, options);

                                server.Document.Replace(URLrange, ""); // remove timestamp

                                //Beautify HTML 
                                HtmlDocument docs = new HtmlDocument();
                                docs.Load(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + tempHtmlFile);
                                foreach (HtmlNode link in docs.DocumentNode.SelectNodes("//table[@style]"))
                                {
                                    HtmlAttribute att = link.Attributes["style"];
                                    if (att != null)
                                        att.Value += "width:100%;table-layout:auto;";
                                }

                                var FinalHtmlText = docs.DocumentNode.WriteTo();

                                server.Document.InsertHtmlText(URLrange.Start, FinalHtmlText, InsertOptions.MatchDestinationFormatting); // insert html to document

                                //delete temp file
                                if (System.IO.File.Exists(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + tempHtmlFile))
                                    System.IO.File.Delete(DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + tempHtmlFile);

                            }
                            else
                            {
                                server.Document.Replace(URLrange, ""); // remove timestamp
                                server.Document.InsertDocumentContent(URLrange.Start, DirectoryManagmentUtils.InitialDemoFilesPathSection + "/" + item.ContentFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Snippet to the temp document
                            }
                            goto x;
                        }
                    }
                }
                string[] split = DocumentName.Split('.');
                string firstPart = string.Join(".", split.Take(split.Length - 1));
                var NewDocName = firstPart + "_" + DateTime.Now.Ticks.ToString() + ".docx";
                server.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + NewDocName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                server.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + NewDocName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // Save copy

                DocumentManager.CloseAllDocuments();
                //server.Dispose();
                TempData.Add("Document", NewDocName);
                return RedirectToAction("CreateAssignment", "Home", new { @Msg = "DocCreated" });
            }
            else
                return RedirectToAction("CreateAssignment", "Home", new { @Msg = "Error" });
        }


        #endregion


        [HttpGet]
        [ValidateLogin]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult Assignment()
        {
            var model = new AssignmentModel();
            return View(model);
        }


        [HttpGet]
        [ValidateLogin]
        [ValidateUserPermission(Global.Actions.Approval, Global.Controlers.Home)]
        public ActionResult Approval()
        {
            var user = ChatHub.FindUserByUserID(CurrentUserSession.User.UserID, CurrentUserSession.User.CompanyID.ToString());
            if (user == null)
            {
                var userDetail = (from items in _entities.tblUserDepartments
                                  where items.UserId == CurrentUserSession.User.UserID
                                  select new UserModel
                                  {
                                      Department = items.Department,
                                      FullName = items.FullName,
                                      Password = items.Password,
                                      UserId = items.UserId,
                                      UserName = items.UserName,
                                      EmailID = items.EmailID,
                                      CanCreateSubtasks = items.CanCreateSubtasks.HasValue ? items.CanCreateSubtasks.Value : false,
                                      CanEdit = items.CanEdit.HasValue ? items.CanEdit.Value : false,
                                      CanApprove = items.CanApprove.HasValue ? items.CanApprove.Value : false
                                  }).FirstOrDefault();
                user = new ChatUser()
                {
                    Name = userDetail.FullName,
                    UserName = userDetail.UserName,
                    ProfilePictureUrl = EmployeePhoto(userDetail.FullName),
                    Id = userDetail.UserId,
                    Status = ChatUser.StatusType.Online,
                    RoomId = CurrentUserSession.User.CompanyID.ToString(),
                    //Mobile = userDetail.Telephone,
                    Email = userDetail.EmailID,
                    RandomColor = GetColor(0),
                };
            }
            bool Check;
            Check = ChatHub.CheckAvailability(CurrentUserSession.User.UserID, CurrentUserSession.User.CompanyID.ToString());
            if (!Check)
            {
                ChatHub.RegisterNewUser(user);
            }
            ChatHub.ChangeStatus(CurrentUserSession.User.CompanyID.ToString(), CurrentUserSession.User.UserID, ChatUser.StatusType.Online);
            var objValidateUserList = (from items in _entities.tblUserDepartments
                                       where items.UserId != CurrentUserSession.User.UserID && items.CompanyID == CurrentUserSession.User.CompanyID
                                       select new UserModel
                                       {
                                           Department = items.Department,
                                           FullName = items.FullName,
                                           Password = items.Password,
                                           UserId = items.UserId,
                                           UserName = items.UserName,
                                           EmailID = items.EmailID,
                                           CanCreateSubtasks = items.CanCreateSubtasks.HasValue ? items.CanCreateSubtasks.Value : false,
                                           CanEdit = items.CanEdit.HasValue ? items.CanEdit.Value : false,
                                           CanApprove = items.CanApprove.HasValue ? items.CanApprove.Value : false

                                       }).ToList();
            ChatUser TempCh = new ChatUser();
            ChatHub ChtHub = new ChatHub();
            bool IsRegistered = false;
            int i = 1;
            foreach (var subuser in objValidateUserList)
            {
                IsRegistered = ChatHub.CheckAvailability(subuser.UserId, CurrentUserSession.User.CompanyID.ToString());
                if (!IsRegistered)
                {
                    ChatUser TempCh1 = new ChatUser();
                    TempCh1.Id = subuser.UserId;
                    TempCh1.UserName = subuser.UserName;
                    TempCh1.Name = subuser.FullName;
                    TempCh1.ProfilePictureUrl = EmployeePhoto(subuser.FullName);
                    TempCh1.RandomColor = GetColor(i);
                    TempCh1.Status = ChatUser.StatusType.Offline;
                    TempCh1.RoomId = CurrentUserSession.User.CompanyID.ToString();
                    //TempCh1.Mobile = subuser.Telephone;
                    TempCh1.Email = subuser.EmailID;
                    ChatHub.RegisterNewUser(TempCh1);
                    i++;
                    if (i == colors.Count)
                        i = 1;
                }
            }
            user.Status = ChatUser.StatusType.Online;
            ChatHelper.CreateNewUserCookie(this.Response, user);
            var model = new AssignmentModel();
            return View(model);
        }

        #region Employee Photo
        public string EmployeePhoto(string FirstName)
        {
            string Path = string.Empty;
            Path = "Images/" + FirstName.Trim().Substring(0, 1).ToUpper() + ".png";
            return Path;
        }
        #endregion

        #region Get Color
        private string GetColor(int i)
        {
            colors.Add("#1bb794");
            colors.Add("#c82222");
            colors.Add("#1b85b7");
            colors.Add("#b7a61b");
            colors.Add("#1bb5b7");
            colors.Add("#a5784b");
            colors.Add("#78a51f");
            colors.Add("#ff781f");
            colors.Add("#d2d24b");
            colors.Add("#8FB657");
            if (i == 0)
            {
                Random random = new Random();
                int index = random.Next(colors.Count);
                var name = colors[index];
                return name;
            }
            else
            {
                return colors[i];
            }
        }
        #endregion

        #region  Edit Content

        [HttpGet]
        [ValidateLogin]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult EditPPT(string id, int Action = 1)
        {
            if (id == null)
            {
                return RedirectToAction("Approval", "Home");
            }
            else
            {
                string UID = Global.UrlDecrypt(id);
                int assignmentID;
                if (int.TryParse(UID, out assignmentID))
                {
                    using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                    {

                        List<PPTModel> documentFileCollection = new List<PPTModel>();

                        var AssignedSlides = from items in _entities.tblAssignedPPTSlides
                                             join assign in _entities.tblAssignments on items.AssignmentID equals assign.AssignmentID
                                             join document in _entities.tblDocuments on assign.DocumentID equals document.DocumentID
                                             join prjct in _entities.tblProjects on assign.ProjectID equals prjct.ProjectID
                                             where assign.AssignmentID == assignmentID
                                             select new PPTModel
                                             {
                                                 AssignedPPTSlideID = items.AssignedPPTSildeID,
                                                 AssignedTaskID = items.AssignmentID,
                                                 SlideName = items.SlideName,
                                                 AssignedTo = assign.LockedByUserID,
                                                 ProjectName = prjct.Name,
                                                 OriginalFile = document.DocumentName,
                                                 TaskName = !string.IsNullOrEmpty(assign.TaskName) ? assign.TaskName : "",
                                                 IsPPTModified = items.IsPPTModified,
                                                 PPTRemarks = items.PPTRemarks,
                                                 IsPPTApproved = items.IsPPTApproved,
                                                 IsGrayedOut = items.IsGrayedOut,
                                                 IsPublished = items.IsPublished,
                                                 ReviewRequested = items.ReviewRequested
                                             };

                        documentFileCollection.AddRange(AssignedSlides.AsEnumerable().Select(i => new PPTModel()
                        {
                            AssignedPPTSlideID = i.AssignedPPTSlideID,
                            AssignedTaskID = i.AssignedTaskID,
                            OriginalFile = i.OriginalFile,
                            SlideName = i.SlideName,
                            IsPPTModified = i.IsPPTModified,
                            IsGrayedOut = i.IsGrayedOut,
                            IsPPTApproved = i.IsPPTApproved,
                            PPTRemarks = i.PPTRemarks,
                            IsPublished = i.IsPublished,
                            ReviewRequested = i.ReviewRequested,
                            SlideLink = Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + i.OriginalFile + "/AssignedDoc/" + i.AssignedTaskID.ToString() + "_Copy_" + i.SlideName,
                            ThumbnailLink = Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + i.OriginalFile + "/AssignedDoc/" + i.AssignedTaskID.ToString() + "_Copy_" + i.SlideName.Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention,
                        }).ToList());

                        foreach (var item in documentFileCollection)
                        {
                            //if (item.IsPPTModified.HasValue && item.IsPPTModified.Value == true)
                            //{
                            var fileurl = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + item.OriginalFile + "/AssignedDoc/" + ("New_" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName);
                            //Spire
                            if (System.IO.File.Exists(fileurl))
                            {
                                Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(fileurl);
                                for (int j = 0; j < presentation.Slides.Count; j++)
                                {
                                    var SlideCount = string.Empty;
                                    //if (item.IsPPTModified.HasValue && item.IsPPTModified.Value == true)
                                    SlideCount = "(" + j + ")";

                                    var link = Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + item.OriginalFile + "/AssignedDoc/" + ("New_" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName.Split('.').FirstOrDefault() + SlideCount + Global.ImageExportExtention);
                                    //item.ThumbnailLinkForOldSlide += "<img onmouseover='ShowImg(this);' onmouseout='HideImg(this);' style='border : 1px solid;' class='img-responsive img-thumbnail' src='" + link + "' height=50px width=50px data-id='" + link + "'/>&nbsp;&nbsp;";
                                    //item.ThumbnailLinkForOldSlide += "<img class='img-responsive img-thumbnail' src='" + link + "' height=50px width=50px data-id='" + link + "'/>&nbsp;&nbsp;";
                                    item.lstThumbnailLinkForOldSlide.Add(link);
                                    var linkOld = Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + item.OriginalFile + "/AssignedDoc/" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName.Split('.').FirstOrDefault() + Global.ImageExportExtention;
                                    //item.AssignedThumbnail = "<img onmouseover='ShowImg(this);' onmouseout='HideImg(this);' style='border : 1px solid;' class='img-responsive img-thumbnail' src='" + linkOld + "' height=50px width=50px data-id='" + linkOld + "'/>";
                                    item.AssignedThumbnail = linkOld;//"<img class='img-responsive img-thumbnail' src='" + linkOld + "' height=50px width=50px data-id='" + linkOld + "'/>";
                                }
                            }
                            else
                            {
                                var linkOld = Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + item.OriginalFile + "/AssignedDoc/" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName.Split('.').FirstOrDefault() + Global.ImageExportExtention;
                                //item.AssignedThumbnail = "<img onmouseover='ShowImg(this);' onmouseout='HideImg(this);' style='border : 1px solid;' class='img-responsive img-thumbnail' src='" + linkOld + "' height=50px width=50px data-id='" + linkOld + "'/>";
                                item.AssignedThumbnail = linkOld; //"<img class='img-responsive img-thumbnail' src='" + linkOld + "' height=50px width=50px data-id='" + linkOld + "'/>";
                                                                  //item.lstThumbnailLinkForOldSlide.Add("No changes found.");
                            }
                            //}
                        }
                        var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == assignmentID);
                        assignment.LockedByUserID = CurrentUserSession.User.UserID;
                        _entities.SaveChanges();

                        var demoModel = new PPTModel() { ListSlides = documentFileCollection, OriginalFile = AssignedSlides.FirstOrDefault().OriginalFile, AssignedTaskID = assignmentID, ProjectName = AssignedSlides.FirstOrDefault().ProjectName, TaskName = AssignedSlides.FirstOrDefault().TaskName };

                        //return PartialView("_LoadSnippet", demoModel);
                        return View("EditPPT", demoModel);
                    }
                }
                else
                {
                    return RedirectToAction("Approval", "Home");
                }
            }
        }


        [HttpPost]
        [ValidateLogin]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult EditPPT(PPTModel model, List<HttpPostedFileBase> newfiles)
        {
            if (newfiles != null && newfiles.Count > 0 && !string.IsNullOrEmpty(model.OriginalFile))
            {
                //Save Uploaded file 
                var doc = (from itm in _entities.tblAssignments
                           join document in _entities.tblDocuments on itm.DocumentID equals document.DocumentID
                           //where itm.OrginalSourceFile == model.OriginalFile
                           where itm.AssignmentID == model.AssignedTaskID
                           select itm).FirstOrDefault();

                var slides = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == model.AssignedTaskID && !(z.IsPPTApproved.HasValue && z.IsPPTApproved.Value == true)).ToList();

                if (slides.Count == newfiles.Count)
                {
                    for (int i = 0; i < newfiles.Count; i++)
                    {
                        if (newfiles[i] != null && newfiles[i].ContentLength > 0) // if new file is uploaded  then save it,
                        {
                            string fileExt = newfiles[i].FileName.Split('.').Last().ToLower();

                            if (fileExt == "ppt")
                            {
                                Aspose.Slides.Presentation slidepresentation = new Aspose.Slides.Presentation(newfiles[i].InputStream);
                                slidepresentation.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.tblDocument.DocumentName + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName, Aspose.Slides.Export.SaveFormat.Pptx);
                            }
                            else
                            {
                                newfiles[i].SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.tblDocument.DocumentName + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName);
                            }

                            //save thumnails 
                            var fileurl = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.tblDocument.DocumentName + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName;

                            Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(fileurl);
                            for (int j = 0; j < presentation.Slides.Count; j++)
                            {

                                //save the slide to Image
                                using (System.Drawing.Image image = presentation.Slides[j].GetThumbnail(1.0F, 1.0F))
                                    image.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.tblDocument.DocumentName + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName.Split('.').First().ToString() + "(" + j + ")" + WordAutomationDemo.Common.Global.ImageExportExtention, System.Drawing.Imaging.ImageFormat.Png);
                                presentation.Dispose();
                            }
                            string strSlideName = slides[i].SlideName;
                            var pptSlide = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == model.AssignedTaskID && z.SlideName == strSlideName).FirstOrDefault();
                            if (pptSlide != null)
                            {
                                pptSlide.IsPPTModified = true;
                                pptSlide.IsGrayedOut = true;
                                _entities.Entry(pptSlide).State = System.Data.Entity.EntityState.Modified;
                                _entities.SaveChanges();
                            }
                        }
                        else //else copy the same file as changes file 
                        {

                            //if (System.IO.File.Exists(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.OrginalSourceFile + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName))
                            //{
                            //    System.IO.File.Delete(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.OrginalSourceFile + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName);
                            //}

                            //System.IO.File.Copy(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.OrginalSourceFile + "/AssignedDoc/" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName, DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.OrginalSourceFile + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName);

                            ////save thumnails 
                            //var fileurl = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.OrginalSourceFile + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName;

                            ////Spire

                            //Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation();
                            //presentation.LoadFromFile(fileurl);
                            //for (int j = 0; j < presentation.Slides.Count; j++)
                            //{

                            //    //save the slide to Image
                            //    System.Drawing.Image image = presentation.Slides[j].SaveAsImage();
                            //    image.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.OrginalSourceFile + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName.Split('.').First().ToString() + "(" + j + ")" + WordAutomationDemo.Common.Global.ImageExportExtention, System.Drawing.Imaging.ImageFormat.Png);
                            //    presentation.Dispose();
                            //}
                        }
                    }

                    doc.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                    doc.CompletedDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(model.Remarks))
                    {
                        doc.Remarks = model.Remarks;
                        doc.Comments = model.Remarks;
                    }
                    _entities.Entry(doc).State = System.Data.Entity.EntityState.Modified;
                    doc.Status = (int)Global.AssignmentStatus.Completed;
                    _entities.SaveChanges();

                    if (!string.IsNullOrEmpty(model.strPPTComments))
                    {
                        var strRemarks = model.strPPTComments.Split('~');
                        foreach (var item in strRemarks)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                var AssignedPPTSlideID = item.Split('`').FirstOrDefault();
                                if (!string.IsNullOrEmpty(AssignedPPTSlideID))
                                {
                                    int PPTSlideID = Convert.ToInt32(AssignedPPTSlideID);
                                    var pptSlide = _entities.tblAssignedPPTSlides.Where(z => z.AssignedPPTSildeID == PPTSlideID).FirstOrDefault();
                                    if (pptSlide != null)
                                    {
                                        pptSlide.PPTRemarks = item.Split('`').LastOrDefault();
                                        _entities.Entry(pptSlide).State = System.Data.Entity.EntityState.Modified;

                                        tblAssignmentLog _logRemarks = new tblAssignmentLog();
                                        _logRemarks.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                                        _logRemarks.AssignmentID = doc.AssignmentID;
                                        _logRemarks.CreatedDate = DateTime.Now;
                                        _logRemarks.CreatedBy = CurrentUserSession.UserID;
                                        _logRemarks.Description = item.Split('`').LastOrDefault();
                                        _logRemarks.DocumentName = model.OriginalFile;
                                        _entities.tblAssignmentLogs.Add(_logRemarks);
                                        _entities.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    //Add Log 
                    tblAssignmentLog _log = new tblAssignmentLog();
                    _log.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                    _log.AssignmentID = doc.AssignmentID;
                    _log.CreatedDate = DateTime.Now;
                    _log.CreatedBy = CurrentUserSession.UserID;
                    _log.Description = model.Remarks;
                    _log.DocumentName = model.OriginalFile;
                    _entities.tblAssignmentLogs.Add(_log);
                    _entities.SaveChanges();

                    var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                    AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                    objAssignmentModelEmail.TaskName = doc.TaskName;
                    objAssignmentModelEmail.Comments = doc.Comments;
                    objAssignmentModelEmail.AssignID = doc.AssignmentID;
                    List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                    lstAssignmentLogModel = GetAssignmentLog(doc.AssignmentID);
                    string EmailBody = templates.TemplateContentForEmail;
                    var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == doc.CreatedBy).FirstOrDefault();
                    templates.Subject = templates.Subject.Replace("#Subject#", "Task Completed: Task has been completed by " + CurrentUserSession.User.FirstName);
                    EmailBody = PopulateEmailBody("A task has been Completed by " + CurrentUserSession.User.FirstName, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody, true);

                    byte[] fileBytes = CommonHelper.GeneratePDF("A task has been Completed by " + CurrentUserSession.User.FirstName, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel);

                    Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody, fileBytes);

                    return RedirectToAction("Approval", "Home", new { @Msg = "Completed" });
                }
                else
                    return RedirectToAction("Approval", "Home", new { @Msg = "Error" });

            }

            return View();
        }

        [HttpGet]
        [ValidateLogin]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult EditPageContent(string id, int Action = 1)
        {
            if (id == null)
            {
                return RedirectToAction("Approval", "Home");
            }
            else
            {
                string UID = Global.UrlDecrypt(id);
                int assignmentID;
                if (int.TryParse(UID, out assignmentID))
                {
                    using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                    {
                        var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == assignmentID);
                        var pages = _entities.tblAssignedWordPages.Where(awp =>
                            awp.AssignmentID == assignment.AssignmentID).ToList();

                        if (assignment != null && pages.Count > 0)
                        {
                            var documentName =
                                _entities.tblDocuments.FirstOrDefault(d => d.DocumentID == assignment.DocumentID)
                                    .DocumentName;
                            var projectName =
                                _entities.tblProjects.FirstOrDefault(p => p.ProjectID == assignment.ProjectID).Name;

                            var files = new DirectoryInfo(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, "Pages_" + documentName, "changedDocs"));
                            List<WordPageModel> pageModels = new List<WordPageModel>();

                            int cnt = 0;
                            foreach (var page in pages)
                            {
                                var pageNumber = ((int)page.Sequence).ToString();
                                var pgFiles = files.GetFiles(assignmentID + "-*-" + pageNumber + ".docx");
                                var fileName = "";

                                if (pgFiles.Length > 1)
                                {
                                    fileName = pgFiles[cnt].Name;
                                }
                                else
                                {
                                    fileName = pgFiles[0].Name;
                                }
                                //var fileName = files.GetFiles(assignmentID + "-*-" + pageNumber + ".docx")[0].Name;
                                var ticks = fileName.Split('-')[1];

                                WordPageModel wpm = new WordPageModel
                                {
                                    PageNumber = (int)page.Sequence,
                                    OriginalFile = documentName,
                                    AssignedThumbnail = Global.SiteUrl + "ApplicationDocuments/Words/Pages_" + documentName + "/assignedThumbs/" + assignmentID + "-" + pageNumber + ".png",
                                    ChangedThumbnail = Global.SiteUrl + "ApplicationDocuments/Words/Pages_" + documentName + "/changedThumbs/" + assignmentID + "-" + pageNumber + ".png",
                                    IsPublished = page.IsPublished.HasValue ? page.IsPublished.Value : false,
                                    ReviewRequested = page.ReviewRequested.HasValue ? page.ReviewRequested.Value : false,
                                    Remarks = page.PageRemarks,
                                    Ticks = ticks
                                };
                                pageModels.Add(wpm);
                                cnt++;
                            }

                            var demoModel = new AssignmentModel()
                            {
                                DocumentName = documentName,
                                ProjectName = projectName,
                                TaskName = assignment.TaskName,
                                UserID = CurrentUserSession.User.UserID,
                                Action = Action,
                                AssignID = assignment.AssignmentID,
                                OriginalDocumentName = documentName,
                                AssignedPages = pageModels
                            };

                            //Lock assignment
                            assignment.LockedByUserID = CurrentUserSession.User.UserID;
                            _entities.SaveChanges();

                            return View(demoModel);
                        }
                        else
                        {
                            return RedirectToAction("Approval", "Home", new { Msg = "Error" });
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Approval", "Home");
                }
            }
        }

        [HttpPost]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult EditPageContent(string docName, int AssignmentID, string remarks, RichEditDocumentServer server)
        {
            var isPublished = Convert.ToBoolean(Request.Form["published"]);
            int action = isPublished ? (int)Global.Action.Published : (int)Global.Action.Completed;

            if (UpdateTask(docName, action.ToString(), server, remarks, "", -1, AssignmentID, true))
            {
                return RedirectToAction("Approval", "Home", new { @Msg = "Completed" });
            }

            var demoModel = new AssignmentModel() { DocumentName = docName, UserID = CurrentUserSession.UserID, Action = action };
            return View("EditPageContent", demoModel);

        }

        [HttpGet]
        [ValidateLogin]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult EditContent(string id, int Action = 1)
        {
            if (id == null)
            {
                return RedirectToAction("Approval", "Home");
            }
            else
            {
                string UID = Global.UrlDecrypt(id);
                int assignmentID;
                if (int.TryParse(UID, out assignmentID))
                {
                    using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                    {
                        var doc = (from items in _entities.tblAssignments
                                   where items.AssignmentID == assignmentID
                                   join prjct in _entities.tblProjects on items.ProjectID equals prjct.ProjectID
                                   select new AssignmentModel
                                   {
                                       DocumentName = items.DocumentFile,
                                       ProjectName = prjct.Name,
                                       TaskName = !string.IsNullOrEmpty(items.TaskName) ? items.TaskName : "",
                                       AssignID = items.AssignmentID
                                   }).FirstOrDefault();

                        if (doc != null)
                        {
                            var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == assignmentID);
                            assignment.LockedByUserID = CurrentUserSession.User.UserID;
                            _entities.SaveChanges();

                            var document = _entities.tblDocuments.FirstOrDefault(d => d.DocumentID == assignment.DocumentID);

                            var demoModel = new AssignmentModel() { DocumentName = doc.DocumentName, ProjectName = doc.ProjectName, TaskName = doc.TaskName, UserID = CurrentUserSession.UserID, Action = Action, AssignID = doc.AssignID, OriginalDocumentName = document.DocumentName };
                            //return PartialView("_LoadSnippet", demoModel);
                            return View(demoModel);
                        }
                        else
                            return RedirectToAction("Approval", "Home", new { Msg = "Error" });
                    }
                }
                else
                {
                    return RedirectToAction("Approval", "Home");
                }
            }
        }

        [HttpPost]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult EditContent(AssignmentModel model, RichEditDocumentServer server)
        {
            using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
            {
                //server.Document.SaveDocument("D://Temp/TestDoc.docx",DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                //server.SaveDocument(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPath,model.DocumentName), DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                var isPublished = Convert.ToBoolean(Request.Form["published"]);
                int action = isPublished ? (int)Global.Action.Published : (int)Global.Action.Completed;

                if (UpdateTask(model.DocumentName, action.ToString(), server, model.Remarks, "", -1, model.AssignID))
                {
                    return RedirectToAction("Approval", "Home", new { @Msg = "Completed" });
                }

                var demoModel = new AssignmentModel() { DocumentName = model.DocumentName, UserID = CurrentUserSession.UserID, Action = model.Action };
                return View("EditContent", demoModel);
            }
        }

        [HttpGet]
        [ValidateLogin]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult EditTask(string id, int Action = 0)
        {
            if (id == null)
            {
                return RedirectToAction("Approval", "Home");
            }
            else
            {
                string UID = Global.UrlDecrypt(id);
                int assignmentID;
                if (int.TryParse(UID, out assignmentID))
                {
                    using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                    {
                        var doc = (from items in _entities.tblAssignments
                                   where items.AssignmentID == assignmentID
                                   join prjct in _entities.tblProjects on items.ProjectID equals prjct.ProjectID
                                   select new AssignmentModel
                                   {
                                       AssignID = items.AssignmentID,
                                       ProjectID = prjct.ProjectID,
                                       ProjectName = prjct.Name,
                                       UserID = CurrentUserSession.UserID,
                                       TaskName = !string.IsNullOrEmpty(items.TaskName) ? items.TaskName : ""
                                   }).FirstOrDefault();

                        if (doc != null)
                        {
                            doc.lstUploadedDocModel = new List<UploadedDocModel>();
                            var ProjectFolderPath = Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, id.ToString());
                            if (Directory.Exists(ProjectFolderPath))
                            {
                                List<UploadedDocModel> lstUploadedDocModel = new List<UploadedDocModel>();

                                var AssignedDocs = from items in _entities.tblAssignedWordPages
                                                   join assign in _entities.tblAssignments on items.AssignmentID equals assign.AssignmentID
                                                   where assign.AssignmentID == assignmentID
                                                   select new UploadedDocModel
                                                   {
                                                       AssignedDocsID = items.AssignedWordPageID,
                                                       DocName = items.DocName,
                                                       AssignmentID = items.AssignmentID.HasValue ? items.AssignmentID.Value : 0,
                                                       Sequence = items.Sequence.HasValue ? items.Sequence.Value : 0
                                                   };

                                lstUploadedDocModel.AddRange(AssignedDocs.AsEnumerable().Select(i => new UploadedDocModel()
                                {
                                    AssignedDocsID = i.AssignedDocsID,
                                    AssignmentID = i.AssignmentID,
                                    DocName = i.DocName,
                                    DocURL = Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, i.AssignmentID.ToString() + "/" + i.DocName),
                                    DownloadImagePath = Global.SiteUrl + "Css/Images/download.png",
                                    Sequence = i.Sequence,
                                    IsDocFile = i.DocName.Split('.').Last().ToLower() == "doc" || i.DocName.Split('.').Last().ToLower() == "docx" ? true : false,
                                }).ToList().OrderBy(s => s.Sequence));
                                doc.lstUploadedDocModel = lstUploadedDocModel;
                            }
                            var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == assignmentID);
                            assignment.LockedByUserID = CurrentUserSession.User.UserID;
                            _entities.SaveChanges();

                            return View(doc);
                        }
                        else
                            return RedirectToAction("Approval", "Home", new { Msg = "Error" });
                    }
                }
                else
                {
                    return RedirectToAction("Approval", "Home");
                }
            }
        }

        [HttpPost]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult EditTask(AssignmentModel model, HttpPostedFileBase[] files)
        {
            //foreach (HttpPostedFileBase file in files)
            //{
            //    //Checking file is available to save.  
            //    if (file != null)
            //    {
            //        if (file.FileName.Contains(".doc") || file.FileName.Contains(".docx") || file.FileName.Contains(".ppt") || file.FileName.Contains(".xls") || file.FileName.Contains(".xlsx") || file.FileName.Contains(".pdf"))
            //        {
            //            System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, model.AssignID.ToString()));
            //            var ProjectFolderPath = Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, model.AssignID.ToString());
            //            var InputFileName = Path.GetFileName(file.FileName);
            //            var ServerSavePath = Path.Combine(ProjectFolderPath, InputFileName);
            //            //Save file to server folder  
            //            file.SaveAs(ServerSavePath);
            //        }
            //        else {
            //            var demoModel = new AssignmentModel() { ProjectID = model.ProjectID, AssignID = model.AssignID, Remarks =model.Remarks, UserID = CurrentUserSession.UserID, Action = model.Action };
            //            return View("EditTask", demoModel);
            //        }
            //    }
            //}
            var doc = (from itm in _entities.tblAssignments
                       where itm.AssignmentID == model.AssignID
                       select itm).FirstOrDefault();

            doc.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
            doc.CompletedDate = DateTime.Now;

            if (!string.IsNullOrEmpty(model.Remarks))
            {
                doc.Remarks = model.Remarks;
                doc.Comments = model.Remarks;
            }
            _entities.Entry(doc).State = System.Data.Entity.EntityState.Modified;

            //Add Log 
            tblAssignmentLog _log = new tblAssignmentLog();
            _log.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
            _log.AssignmentID = doc.AssignmentID;
            _log.CreatedDate = DateTime.Now;
            _log.CreatedBy = CurrentUserSession.UserID;
            _log.Description = model.Remarks;

            _entities.tblAssignmentLogs.Add(_log);
            _entities.SaveChanges();

            var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

            AssignmentModel objAssignmentModelEmail = new AssignmentModel();
            objAssignmentModelEmail.TaskName = doc.TaskName;
            objAssignmentModelEmail.Comments = doc.Comments;
            objAssignmentModelEmail.AssignID = doc.AssignmentID;
            List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
            lstAssignmentLogModel = GetAssignmentLog(doc.AssignmentID);
            string EmailBody = templates.TemplateContentForEmail;
            var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == doc.CreatedBy).FirstOrDefault();
            templates.Subject = templates.Subject.Replace("#Subject#", "Task Completed: Task has been completed by " + CurrentUserSession.User.FirstName);
            EmailBody = PopulateEmailBody("A task has been Completed by " + CurrentUserSession.User.FirstName, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody, true);

            Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody);

            //var Currentuser = _entities.tblUserDepartments.Where(x => x.UserId == CurrentUserSession.UserID).FirstOrDefault();
            //Task.Factory.StartNew(() =>
            //{
            //    var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == doc.CreatedBy).FirstOrDefault();
            //    if (UserToNotify != null && !string.IsNullOrEmpty(UserToNotify.EmailID))
            //    {
            //        StringBuilder _sb = new StringBuilder();
            //        _sb.AppendLine("Hello " + UserToNotify.FullName + ",<br/><br/>");
            //        _sb.AppendLine(Currentuser.FullName + " has " + Common.Global.GetEnumDescription((Common.Global.Action)doc.Action) + " his task in Triyo. Please login to your account to check it.<br/><br/>");
            //        _sb.AppendLine("Regards,<br/><b>Triyo</b>");
            //        WordAutomationDemo.Common.Global.SendEmail(UserToNotify.EmailID, "Task " + Common.Global.GetEnumDescription((Common.Global.Action)doc.Action) + " by  " + Currentuser.FullName, _sb.ToString());
            //    }
            //});

            return RedirectToAction("Approval", "Home", new { @Msg = "Completed" });
        }

        [HttpPost]
        public JsonResult UpdateFile(int sequence, int id, int AssignedDocsID, string oldFileName)
        {
            var file = Request.Files[0];
            var docID = 0;
            if (file != null && file.ContentLength > 0)
            {
                var InputFileName = Path.GetFileName(file.FileName);
                string fileName = InputFileName.Replace("." + InputFileName.Split('.').Last().ToLower(), "") + "_" + DateTime.Now.Ticks.ToString() + "." + InputFileName.Split('.').Last().ToLower();
                //var _doc = _entities.tblAssignedDocs.Where(z => z.AssignmentID == id).OrderByDescending(s => s.Sequence).FirstOrDefault();
                //if (_doc != null)
                //{
                //    sequence = _doc.Sequence.HasValue ? _doc.Sequence.Value + 1 : 1;
                //}
                //else {
                //    sequence = 1;
                //}
                tblAssignedWordPage objtblAssignedDoc = new tblAssignedWordPage();
                objtblAssignedDoc.AssignmentID = id;
                objtblAssignedDoc.DocName = fileName;
                objtblAssignedDoc.Sequence = sequence;
                _entities.tblAssignedWordPages.Add(objtblAssignedDoc);
                _entities.SaveChanges();
                var TaskFolderPath = Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, id.ToString());
                var ServerSavePath = Path.Combine(TaskFolderPath, fileName);

                if (!string.IsNullOrEmpty(oldFileName))
                {
                    var _docToDelete = _entities.tblAssignedWordPages.Where(z => z.AssignedWordPageID == AssignedDocsID).FirstOrDefault();
                    if (_docToDelete != null)
                    {
                        docID = objtblAssignedDoc.AssignedWordPageID;
                        _entities.tblAssignedWordPages.Remove(_docToDelete);
                        _entities.SaveChanges();
                    }
                    var OldFilePath = Path.Combine(TaskFolderPath, oldFileName);
                    //Save file to server folder  
                    if (System.IO.File.Exists(OldFilePath))
                    {
                        System.IO.File.Delete(OldFilePath);
                    }
                }

                if (docID == 0)
                {
                    docID = objtblAssignedDoc.AssignedWordPageID;
                }

                System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, id.ToString()));
                file.SaveAs(ServerSavePath);
                if (InputFileName.Split('.').Last().ToLower() == "ppt" || InputFileName.Split('.').Last().ToLower() == "pptx")
                {
                    if (!string.IsNullOrEmpty(oldFileName))
                    {
                        var docToDelete = _entities.tblAssignedPPTSlides.Where(z => z.AssignedDocsID == AssignedDocsID);
                        foreach (var doc in docToDelete)
                        {
                            _entities.tblAssignedPPTSlides.Remove(doc);
                        }
                        _entities.SaveChanges();
                        string directoryName = Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, id.ToString() + "/PPT/" + oldFileName + "/");
                        if (System.IO.Directory.Exists(directoryName))
                        {
                            System.IO.DirectoryInfo di = new DirectoryInfo(directoryName);
                            foreach (FileInfo fi in di.GetFiles())
                            {
                                fi.Delete();
                            }
                            System.IO.Directory.Delete(directoryName);
                        }
                    }
                    SplitTaskSlides(fileName, id, objtblAssignedDoc.AssignedWordPageID);
                }

                return Json(new { IsSuccess = true, FileName = fileName, AssignedDocsID = docID });
            }
            return Json(new { IsSuccess = false });
        }

        [HttpPost]
        public JsonResult DeleteDocument(int id, int AssignedDocsID, string oldFileName)
        {
            if (id > 0)
            {
                var ProjectFolderPath = Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, id.ToString());
                var OldFilePath = Path.Combine(ProjectFolderPath, oldFileName);

                var _docToDelete = _entities.tblAssignedWordPages.Where(z => z.AssignedWordPageID == AssignedDocsID).FirstOrDefault();
                if (_docToDelete != null)
                {
                    _entities.tblAssignedWordPages.Remove(_docToDelete);
                    _entities.SaveChanges();
                }

                //Save file to server folder  
                if (System.IO.File.Exists(OldFilePath))
                {
                    System.IO.File.Delete(OldFilePath);
                    if (oldFileName.Split('.').Last().ToLower() == "ppt" || oldFileName.Split('.').Last().ToLower() == "pptx")
                    {
                        var docToDelete = _entities.tblAssignedPPTSlides.Where(z => z.AssignedDocsID == AssignedDocsID);
                        foreach (var doc in docToDelete)
                        {
                            _entities.tblAssignedPPTSlides.Remove(doc);
                        }
                        _entities.SaveChanges();
                        string directoryName = Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, id.ToString() + "/PPT/" + oldFileName + "/");
                        if (System.IO.Directory.Exists(directoryName))
                        {
                            System.IO.DirectoryInfo di = new DirectoryInfo(directoryName);
                            foreach (FileInfo fi in di.GetFiles())
                            {
                                fi.Delete();
                            }
                            System.IO.Directory.Delete(directoryName);
                        }
                    }
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                return Json("notexist", JsonRequestBehavior.AllowGet);
            }
            return Json("fail", JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpPost]
        public JsonResult GetReadOnlyDocument(int assignmentID, string docName)
        {
            var isDocExist = (from items in _entities.tblAssignedWordPages
                              where items.AssignmentID == assignmentID && items.DocName == docName
                              select items).Any();
            if (isDocExist)
            {
                return Json(new { IsSuccess = true, FileName = Path.Combine(DirectoryManagmentUtils.ReadOnlyFilesPath, docName), DownloadImagePath = Global.SiteUrl + "Css/Images/download.png" });
            }
            else
            {
                return Json(new { IsSuccess = false });
            }
        }

        [HttpPost]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult UploadDocDropzone()
        {
            HttpPostedFileBase docFile = Request.Files[0];
            var fileName = docFile.FileName;
            var FileExtension = fileName.Split('.').Last().ToLower();
            int docType = 0;

            var ts = DateTime.Now.Ticks.ToString();
            var NewFileName = fileName.Substring(0, fileName.Length - 5) + "_" + ts + '.' + FileExtension;
            if (FileExtension == "ppt" || FileExtension == "pptx")
            {
                var NewName = "Slides_" + fileName.Substring(0, fileName.Length - 5) + "_" + ts + '.' + FileExtension;
                var Foldername = NewName;

                if (FileExtension == "ppt")
                {
                    //crate directory
                    System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, Foldername + "x"));

                    Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(docFile.InputStream);
                    NewFileName = NewFileName + "x";
                    presentation.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + NewFileName, Aspose.Slides.Export.SaveFormat.Pptx);
                }
                else
                {
                    //crate directory
                    System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, Foldername));
                    docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + NewFileName);

                }
                SplitSlides(NewFileName);
                TempData.Add("hdnDocTaskType", (int)Global.DocumentType.Ppt);
                TempData.Add("Document", NewFileName);
                docType = (int)Global.DocumentType.Ppt;
            }
            else if (FileExtension == "xls" || FileExtension == "xlsx")
            {

                var NewName = "Sheets_" + fileName.Substring(0, fileName.Length - 5) + "_" + ts + '.' + FileExtension;
                var Foldername = NewName;

                using (var workbook = new Aspose.Cells.Workbook(docFile.InputStream))
                {
                    PidExcelRows(workbook, NewFileName);
                    Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, Foldername));
                    workbook.Save(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, NewFileName));
                }

                SplitSheets(NewFileName);
                TempData.Add("hdnDocTaskType", (int)Global.DocumentType.Xls);
                TempData.Add("Document", NewFileName);
                docType = (int)Global.DocumentType.Xls;
            }
            else
            {
                if (FileExtension == "doc") // if old document file  then convert it to docx 
                {
                    NewFileName = NewFileName + "x";
                    RichEditDocumentServer server = new RichEditDocumentServer();
                    server.Document.LoadDocument(docFile.InputStream, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                    server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + NewFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + NewFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathWords + "/Original_" + NewFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    DocumentManager.CloseAllDocuments();
                }
                else
                {
                    docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + NewFileName);
                    docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + NewFileName);
                    docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathWords + "/Original_" + NewFileName);
                    //docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + docFile.FileName);
                    //docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + docFile.FileName);
                }
                SplitPages(NewFileName);
                TempData.Add("hdnDocTaskType", (int)Global.DocumentType.Word);
                TempData.Add("Document", NewFileName);
                docType = (int)Global.DocumentType.Word;
            }

            tblDocument objtblDocument = CommonHelper.AddDocuments(fileName, NewFileName, docType, _entities);
            CommonHelper.AddProjectDocuments(Convert.ToInt32(Request.Form["projectId"]), objtblDocument.DocumentID.ToString(), _entities, true);

            TempData.Add("hdnDocProjectID", Request.Form["projectId"]);
            //TempData.Add("hdnDocTaskName", objFormCollection["hdnDocTaskName"]);
            //TempData.Add("hdnDocDueDate", objFormCollection["hdnDocDueDate"]);
            //TempData.Add("hdnDocAssignTo", objFormCollection["hdnDocAssignTo"]);
            //TempData.Add("hdnDocComments", objFormCollection["hdnDocComments"]);
            TempData.Add("hdnDocName", NewFileName);
            TempData.Add("hdnDocumentID", objtblDocument.DocumentID);
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("CreateAssignment", "Home", new { @Msg = "Uploaded" });
            return Json(new { Url = redirectUrl });

        }

        [HttpPost]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult UploadDoc(HttpPostedFileBase docFile, FormCollection objFormCollection)
        {
            TempData.Remove("hdnDocTaskType");
            TempData.Remove("hdnDocProjectID");
            TempData.Remove("hdnDocTaskName");
            TempData.Remove("hdnDocDueDate");
            TempData.Remove("hdnDocAssignTo");
            TempData.Remove("hdnDocComments");
            TempData.Remove("hdnDocName");
            TempData.Remove("hdnDocumentID");
            var fileName = string.Empty;
            int docType = 0;
            if (docFile != null && docFile.ContentLength > 0)
            {
                if (objFormCollection["hdnFileName"] != null)
                {
                    fileName = objFormCollection["hdnFileName"];
                }
                else
                {
                    fileName = docFile.FileName;
                }
                var FileExtention = fileName.Split('.').Last().ToLower();

                var ts = DateTime.Now.Ticks.ToString();
                var NewFileName = fileName.Substring(0, fileName.Length - 5) + "_" + ts + '.' + FileExtention;
                if (FileExtention == "ppt" || FileExtention == "pptx")
                {
                    var NewName = "Slides_" + fileName.Substring(0, fileName.Length - 5) + "_" + ts + '.' + FileExtention;
                    var Foldername = NewName;

                    if (FileExtention == "ppt")
                    {
                        //crate directory
                        System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, Foldername + "x"));

                        Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(docFile.InputStream);
                        NewFileName = NewFileName + "x";
                        presentation.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + NewFileName, Aspose.Slides.Export.SaveFormat.Pptx);
                    }
                    else
                    {
                        //crate directory
                        System.IO.Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, Foldername));
                        docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + NewFileName);

                    }
                    //SplitSlides(NewFileName);
                    SplitSlidesNew(NewFileName);
                    TempData.Add("hdnDocTaskType", (int)Global.DocumentType.Ppt);
                    TempData.Add("Document", NewFileName);
                    docType = (int)Global.DocumentType.Ppt;
                }
                else if (FileExtention == "xls" || FileExtention == "xlsx")
                {

                    var NewName = "Sheets_" + fileName.Substring(0, fileName.Length - 5) + "_" + ts + '.' + FileExtention;
                    var Foldername = NewName;

                    using (var workbook = new Aspose.Cells.Workbook(docFile.InputStream))
                    {
                        PidExcelRows(workbook, NewFileName);
                        Directory.CreateDirectory(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, Foldername));
                        workbook.Save(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, NewFileName));
                    }

                    SplitSheets(NewFileName);
                    TempData.Add("hdnDocTaskType", (int)Global.DocumentType.Xls);
                    TempData.Add("Document", NewFileName);
                    docType = (int)Global.DocumentType.Xls;
                }
                else
                {
                    if (FileExtention == "doc") // if old document file  then convert it to docx 
                    {
                        NewFileName = NewFileName + "x";
                        RichEditDocumentServer server = new RichEditDocumentServer();
                        server.Document.LoadDocument(docFile.InputStream, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                        server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + NewFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                        server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + NewFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                        server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathWords + "/Original_" + NewFileName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                        DocumentManager.CloseAllDocuments();
                    }
                    else
                    {
                        docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + NewFileName);
                        docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + NewFileName);
                        docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathWords + "/Original_" + NewFileName);
                        //docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + docFile.FileName);
                        //docFile.SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + docFile.FileName);
                    }

                    SplitPages(NewFileName);
                    TempData.Add("hdnDocTaskType", (int)Global.DocumentType.Word);
                    TempData.Add("Document", NewFileName);
                    docType = (int)Global.DocumentType.Word;
                }

                tblDocument objtblDocument = CommonHelper.AddDocuments(fileName, NewFileName, docType, _entities);
                CommonHelper.AddProjectDocuments(Convert.ToInt32(objFormCollection["hdnDocProjectID"]), objtblDocument.DocumentID.ToString(), _entities, true);

                TempData.Add("hdnDocProjectID", objFormCollection["hdnDocProjectID"]);
                TempData.Add("hdnDocTaskName", objFormCollection["hdnDocTaskName"]);
                TempData.Add("hdnDocDueDate", objFormCollection["hdnDocDueDate"]);
                TempData.Add("hdnDocAssignTo", objFormCollection["hdnDocAssignTo"]);
                TempData.Add("hdnDocComments", objFormCollection["hdnDocComments"]);
                TempData.Add("hdnDocName", NewFileName);
                TempData.Add("hdnDocumentID", objtblDocument.DocumentID);
                return RedirectToAction("CreateAssignment", "Home", new { @Msg = "Uploaded" });
            }
            return RedirectToAction("CreateAssignment", "Home");
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Index", "Login", new { Msg = "Logout" });
        }


        [HttpGet]
        [ValidateLogin]
        public ActionResult UnAuthorised()
        {
            return View();
        }

        #region MEthods

        [HttpGet]
        public ActionResult DownloadOriginalDoc(string filename)
        {
            var originalPath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathDemo, filename);
            var copyPath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, filename);

            Aspose.Words.Document originalDoc = new Aspose.Words.Document(originalPath);
            originalDoc.Save(copyPath);

            return File(copyPath, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", filename);
        }

        [HttpPost]
        public string UpdateSlide(FormCollection objFormCollection)
        {
            Result objAvailabilityResult = new Result();
            if (objFormCollection != null && objFormCollection["assignmentID"] != null && objFormCollection["assignedPPTSlideID"] != null && objFormCollection["originalFile"] != null && objFormCollection["slideName"] != null)
            {
                int assignmentID = Convert.ToInt32(objFormCollection["assignmentID"]);
                int assignedPPTSlideID = Convert.ToInt32(objFormCollection["assignedPPTSlideID"]);
                string originalFile = objFormCollection["originalFile"];
                string slideName = objFormCollection["slideName"];
                if (Request.Files != null && Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                        {
                            var fileName = assignmentID + "_Copy_" + slideName;
                            tblAssignment _tblAssignment = _entities.tblAssignments.Where(c => c.AssignmentID == assignmentID).FirstOrDefault();
                            if (_tblAssignment != null)
                            {
                                if (_tblAssignment.Action != (int)WordAutomationDemo.Common.Global.Action.Completed)
                                {
                                    Aspose.Slides.Presentation slidepresentation = new Aspose.Slides.Presentation();
                                    var Path = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "\\Slides_" + originalFile + "\\AssignedDoc\\" + fileName;

                                    if (System.IO.File.Exists(Path))
                                    {
                                        Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(file.InputStream);
                                        presentation.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "\\Slides_" + originalFile + "\\AssignedDoc\\New_" + fileName, Aspose.Slides.Export.SaveFormat.Pptx); //Save File for Projects Folder 
                                        for (int j = 0; j < presentation.Slides.Count; j++)
                                        {

                                            //save the slide to Image
                                            using (System.Drawing.Image image = presentation.Slides[j].GetThumbnail(1.0F, 1.0F))
                                                image.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + _tblAssignment.tblDocument.DocumentName + "/AssignedDoc/New_" + fileName.Split('.').First().ToString() + "(" + j + ")" + WordAutomationDemo.Common.Global.ImageExportExtention, System.Drawing.Imaging.ImageFormat.Png);
                                            presentation.Dispose();
                                        }
                                        var pptSlide = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == assignmentID && z.SlideName == slideName).FirstOrDefault();
                                        if (pptSlide != null)
                                        {
                                            if (objFormCollection["PPTRemarks"] != null)
                                            {
                                                pptSlide.PPTRemarks = objFormCollection["PPTRemarks"];
                                            }
                                            pptSlide.IsPPTModified = true;
                                            //pptSlide.IsGrayedOut = true;
                                            _entities.Entry(pptSlide).State = System.Data.Entity.EntityState.Modified;
                                        }

                                        _entities.Entry(_tblAssignment).State = System.Data.Entity.EntityState.Modified;
                                        _entities.SaveChanges();
                                        string message = string.Empty;
                                        string strEmailMessage = string.Empty;
                                        string strEmailSubject = string.Empty;
                                        var modifiedSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == assignmentID && z.IsPPTModified.HasValue && z.IsPPTModified.Value == true).Count();
                                        var totalSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == assignmentID).Count();
                                        message = "2";

                                        var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                                        objAvailabilityResult.Status = true;
                                        objAvailabilityResult.ErrorCode = "";
                                        objAvailabilityResult.ErrorMessage = message;
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
                    }
                }
            }
            return objAvailabilityResult.ErrorMessage;
        }


        [HttpPost]
        public string CompleteSlide(FormCollection objFormCollection)
        {
            Result objAvailabilityResult = new Result();
            if (objFormCollection != null && objFormCollection["assignmentID"] != null && objFormCollection["assignedPPTSlideID"] != null && objFormCollection["originalFile"] != null && objFormCollection["slideName"] != null)
            {
                int assignmentID = Convert.ToInt32(objFormCollection["assignmentID"]);
                int assignedPPTSlideID = Convert.ToInt32(objFormCollection["assignedPPTSlideID"]);
                string originalFile = objFormCollection["originalFile"];
                string slideName = objFormCollection["slideName"];
                if (Request.Files != null && Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                        {
                            var fileName = assignmentID + "_Copy_" + slideName;
                            tblAssignment _tblAssignment = _entities.tblAssignments.Where(c => c.AssignmentID == assignmentID).FirstOrDefault();
                            if (_tblAssignment != null)
                            {
                                if (_tblAssignment.Action != (int)WordAutomationDemo.Common.Global.Action.Completed)
                                {
                                    Aspose.Slides.Presentation slidepresentation = new Aspose.Slides.Presentation();
                                    var Path = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "\\Slides_" + originalFile + "\\AssignedDoc\\" + fileName;

                                    if (System.IO.File.Exists(Path))
                                    {
                                        Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(file.InputStream);
                                        presentation.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "\\Slides_" + originalFile + "\\AssignedDoc\\New_" + fileName, Aspose.Slides.Export.SaveFormat.Pptx); //Save File for Projects Folder 
                                        for (int j = 0; j < presentation.Slides.Count; j++)
                                        {

                                            //save the slide to Image
                                            using (System.Drawing.Image image = presentation.Slides[j].GetThumbnail(1.0F, 1.0F))
                                                image.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + _tblAssignment.tblDocument.DocumentName + "/AssignedDoc/New_" + fileName.Split('.').First().ToString() + "(" + j + ")" + WordAutomationDemo.Common.Global.ImageExportExtention, System.Drawing.Imaging.ImageFormat.Png);
                                            presentation.Dispose();
                                        }
                                        var pptSlide = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == assignmentID && z.SlideName == slideName).FirstOrDefault();
                                        if (pptSlide != null)
                                        {
                                            if (objFormCollection["PPTRemarks"] != null)
                                            {
                                                pptSlide.PPTRemarks = objFormCollection["PPTRemarks"];
                                            }
                                            pptSlide.IsPPTModified = true;
                                            pptSlide.IsGrayedOut = true;
                                            _entities.Entry(pptSlide).State = System.Data.Entity.EntityState.Modified;
                                        }

                                        //SplitSlides(objPPTDocModel.FileName);
                                        //_tblAssignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                                        _tblAssignment.CompletedDate = DateTime.Now;
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
                                        _log.CreatedBy = CurrentUserSession.UserID;
                                        if (objFormCollection["PPTRemarks"] != null)
                                        {
                                            _log.Description = objFormCollection["PPTRemarks"];
                                        }
                                        //_log.Description = slideName + " has been updated from power point";
                                        //_log.DocumentName = model.OriginalFile;
                                        _entities.tblAssignmentLogs.Add(_log);
                                        _entities.SaveChanges();

                                        string message = string.Empty;
                                        string strEmailMessage = string.Empty;
                                        string strEmailSubject = string.Empty;
                                        var modifiedSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == assignmentID && z.IsPPTModified.HasValue && z.IsPPTModified.Value == true).Count();
                                        var totalSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == assignmentID).Count();
                                        if (modifiedSlidesCount == totalSlidesCount)
                                        {
                                            _tblAssignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                                            _tblAssignment.CompletedDate = DateTime.Now;
                                            _entities.SaveChanges();
                                            strEmailMessage = "A task has been Completed by " + CurrentUserSession.User.FirstName;
                                            strEmailSubject = "Task Completed: Task has been completed by " + CurrentUserSession.User.FirstName;
                                            message = "1";
                                        }
                                        else
                                        {
                                            strEmailMessage = "A slide has been Completed by " + CurrentUserSession.User.FirstName;
                                            strEmailSubject = "Slide Completed: slide has been completed by " + CurrentUserSession.User.FirstName;
                                            message = "2";
                                        }

                                        var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                                        AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                                        objAssignmentModelEmail.TaskName = _tblAssignment.TaskName;
                                        objAssignmentModelEmail.Comments = _tblAssignment.Comments;
                                        objAssignmentModelEmail.AssignID = _tblAssignment.AssignmentID;
                                        List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                                        lstAssignmentLogModel = GetAssignmentLog(_tblAssignment.AssignmentID);
                                        string EmailBody = templates.TemplateContentForEmail;

                                        var assignmentMemberIDs = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == _tblAssignment.AssignmentID && am.UserID != CurrentUserSession.User.UserID).Select(u => u.UserID).ToList();
                                        var UsersToNotify = _entities.tblUserDepartments.Where(x => assignmentMemberIDs.Contains(x.UserId)).ToList();

                                        foreach (var UserToNotify in UsersToNotify)
                                        {
                                            templates.Subject = templates.Subject.Replace("#Subject#", strEmailSubject);
                                            EmailBody = PopulateEmailBody(strEmailMessage, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody, true);

                                            byte[] fileBytes = CommonHelper.GeneratePDF(strEmailMessage, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel);

                                            Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody, fileBytes);
                                        }

                                        objAvailabilityResult.Status = true;
                                        objAvailabilityResult.ErrorCode = "";
                                        objAvailabilityResult.ErrorMessage = message;
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
                    }
                }
                else
                {
                    using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                    {
                        tblAssignment _tblAssignment = _entities.tblAssignments.Where(c => c.AssignmentID == assignmentID).FirstOrDefault();
                        if (_tblAssignment != null)
                        {
                            var pptSlide = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == assignmentID && z.SlideName == slideName).FirstOrDefault();
                            if (pptSlide != null)
                            {
                                if (objFormCollection["PPTRemarks"] != null)
                                {
                                    pptSlide.PPTRemarks = objFormCollection["PPTRemarks"];
                                }
                                pptSlide.IsPPTModified = true;
                                pptSlide.IsGrayedOut = true;
                                _entities.Entry(pptSlide).State = System.Data.Entity.EntityState.Modified;
                            }

                            //Add Log 
                            tblAssignmentLog _log = new tblAssignmentLog();
                            _log.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                            _log.AssignmentID = _tblAssignment.AssignmentID;
                            _log.CreatedDate = DateTime.Now;
                            _log.CreatedBy = CurrentUserSession.UserID;
                            if (objFormCollection["PPTRemarks"] != null)
                            {
                                _log.Description = objFormCollection["PPTRemarks"];
                            }
                            //_log.DocumentName = model.OriginalFile;
                            _entities.tblAssignmentLogs.Add(_log);
                            _entities.SaveChanges();

                            string message = string.Empty;
                            string strEmailMessage = string.Empty;
                            string strEmailSubject = string.Empty;
                            var modifiedSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == assignmentID && z.IsPPTModified.HasValue && z.IsPPTModified.Value == true).Count();
                            var totalSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == assignmentID).Count();
                            if (modifiedSlidesCount == totalSlidesCount)
                            {
                                _tblAssignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                                _tblAssignment.CompletedDate = DateTime.Now;
                                _entities.SaveChanges();
                                strEmailMessage = "A task has been Completed by " + CurrentUserSession.User.FirstName;
                                strEmailSubject = "Task Completed: Task has been completed by " + CurrentUserSession.User.FirstName;
                                message = "1";
                            }
                            else
                            {
                                strEmailMessage = "A slide has been Completed by " + CurrentUserSession.User.FirstName;
                                strEmailSubject = "Slide Completed: slide has been completed by " + CurrentUserSession.User.FirstName;
                                message = "2";
                            }

                            var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                            AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                            objAssignmentModelEmail.TaskName = _tblAssignment.TaskName;
                            objAssignmentModelEmail.Comments = _tblAssignment.Comments;
                            objAssignmentModelEmail.AssignID = _tblAssignment.AssignmentID;
                            List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                            lstAssignmentLogModel = GetAssignmentLog(_tblAssignment.AssignmentID);
                            string EmailBody = templates.TemplateContentForEmail;
                            var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == _tblAssignment.CreatedBy).FirstOrDefault();
                            templates.Subject = templates.Subject.Replace("#Subject#", strEmailSubject);
                            EmailBody = PopulateEmailBody(strEmailMessage, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody, true);

                            byte[] fileBytes = CommonHelper.GeneratePDF(strEmailMessage, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel);

                            Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody, fileBytes);

                            objAvailabilityResult.Status = true;
                            objAvailabilityResult.ErrorCode = "";
                            objAvailabilityResult.ErrorMessage = message;
                        }
                    }
                }
            }
            return objAvailabilityResult.ErrorMessage;
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetPendingTaskCount()
        {
            return Json(GetSnippetsActionCount(), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllProjectsJson()
        {
            var data = GetAllProjects(CurrentUserSession.UserID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> GetAllProjects(int userId)
        {
            try
            {
                var myProjectIds = _entities.tblProjectMembers.Where(pm => pm.UserID == userId).Select(p => p.ProjectID).ToList();
                var projects = _entities.tblProjects.Where(p => !p.IsDeleted && p.Status == (int)Global.ProjectStatus.Active && (myProjectIds.Contains(p.ProjectID) || p.CreatedBy == userId)).ToList();
                var list = new List<SelectListItem>();

                list = (projects.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.ProjectID.ToString()
                }).ToList());
                return list;
            }
            catch
            {
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult HasAssignedPages(int AssignmentID)
        {
            var assignedPages = _entities.tblAssignedWordPages.Where(wp => wp.AssignmentID == AssignmentID).ToList();
            return Json(assignedPages.Count > 0, JsonRequestBehavior.AllowGet);
        }

        //Load Document to Popup 
        public ActionResult LoadSnippet(string name = "", int userid = 0, int Action = 1, bool IsReadOnly = false)
        {
            var demoModel = new AssignmentModel() { DocumentName = name, UserID = CurrentUserSession.UserID, Action = Action, IsReadOnly = IsReadOnly };
            return PartialView("_LoadSnippet", demoModel);
        }

        public ActionResult LoadPageSnippet(string name = "", int userid = 0, int Action = 1, bool IsReadOnly = false, string originalFile = "")
        {
            var demoModel = new AssignmentModel()
            {
                DocumentName = name,
                UserID = CurrentUserSession.UserID,
                Action = Action,
                IsReadOnly = IsReadOnly,
                OriginalDocumentName = originalFile
            };
            return PartialView("_LoadPageSnippet", demoModel);
        }

        public ActionResult AddComment(int userId, int assignmentID, string comment)
        {
            if (userId > 0 && assignmentID > 0)
            {
                var name = _entities.tblUserDepartments.FirstOrDefault(u => u.UserId == userId).FullName;
                tblAssignmentLog newAssignmentLog = new tblAssignmentLog
                {
                    AssignmentID = assignmentID,
                    Action = (int)Global.Action.Comment,
                    CreatedDate = DateTime.Now,
                    Description = comment,
                    CreatedBy = userId
                };

                _entities.tblAssignmentLogs.Add(newAssignmentLog);
                _entities.SaveChanges();

                AssignmentLogModel newAssignmentLogModel = new AssignmentLogModel
                {
                    AssignmentID = assignmentID,
                    AssignmentLogID = newAssignmentLog.AssignmentLogID,
                    Description = comment,
                    Action = (int)Global.Action.Comment,
                    UserName = name,
                    CreatedDate = newAssignmentLog.CreatedDate,
                    CreatedDateString = newAssignmentLog.CreatedDate.HasValue ? newAssignmentLog.CreatedDate.Value.ToString("MM/dd/yyyy hh:mm:ss") : "",
                    ActionString = Enum.GetName(typeof(Global.Action), Global.Action.Comment)
                };

                return Json(newAssignmentLogModel);

            }
            return Json("failed");
        }

        //Load Document to Popup 
        public ActionResult LoadCommentHistory(int assignmentID)
        {
            AssignmentLogModel objAssignmentLogModel = new AssignmentLogModel();
            var lstAssignmentLog = (from items in _entities.tblAssignmentLogs
                                    where items.AssignmentID == assignmentID
                                    select new AssignmentLogModel
                                    {
                                        AssignmentID = items.AssignmentID.HasValue ? items.AssignmentID.Value : 0,
                                        AssignmentLogID = items.AssignmentLogID,
                                        Description = items.Description,
                                        Action = items.Action,
                                        UserName = items.CreatedBy.HasValue ? items.tblUserDepartment.FullName : "",
                                        CreatedDate = items.CreatedDate
                                    }).OrderByDescending(a => a.CreatedDate).ToList();
            objAssignmentLogModel.AssignmentID = assignmentID;
            foreach (var item in lstAssignmentLog)
            {
                item.ActionString = item.Action > 0 ? Enum.GetName(typeof(Global.Action), item.Action) : "";

                item.CreatedDateString = item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("MM/dd/yyyy hh:mm:ss") : "";
                //if (WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid != null)
                //{
                //    item.CreatedDateString = item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid + " hh:mm:ss") : "";
                //}
                //else {
                //    item.CreatedDateString = item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("MM/dd/yyyy hh:mm:ss") : "";
                //}

            }

            //var lstAssignedSlideComments = _entities.tblAssignedPPTSlides.Where(appt =>
            //    appt.AssignmentID == assignmentID && !String.IsNullOrEmpty(appt.PPTRemarks)).ToList();

            //var slideComments = new List<SlideLogModel>();
            //if (lstAssignedSlideComments.Count > 0)
            //{
            //    foreach (var slide in lstAssignedSlideComments)
            //    {
            //        var slideLogModel = new SlideLogModel
            //        {
            //            SlideNumber = slide.Sequence,
            //            Comment = slide.PPTRemarks
            //        };
            //        slideComments.Add(slideLogModel);
            //    }

            //}

            //objAssignmentLogModel.lstSlideLogModel = slideComments;
            objAssignmentLogModel.lstAssignmentLogModel = lstAssignmentLog;
            return PartialView("_LoadCommentHistory", objAssignmentLogModel);
        }

        //Load Document to Popup 
        public ActionResult LoadOrginalDoc(string name = "", int userid = 0, int Action = 1, bool IsReadOnly = false)
        {
            var demoModel = new AssignmentModel() { DocumentName = name, UserID = CurrentUserSession.UserID, Action = Action, IsReadOnly = IsReadOnly };

            return PartialView("_LoadOriginalDoc", demoModel);

        }

        public ActionResult SendToReview(int assignmentID, string remarks = "")
        {
            var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == assignmentID);
            assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
            assignment.Comments = remarks;

            //Add log
            tblAssignmentLog _log = new tblAssignmentLog();
            _log.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
            _log.AssignmentID = assignment.AssignmentID;
            _log.CreatedDate = DateTime.Now;
            _log.Description = remarks;
            _log.CreatedBy = CurrentUserSession.UserID;
            _entities.tblAssignmentLogs.Add(_log);
            _entities.SaveChanges();


            //Send notification to approvers except to current user (if approver)
            string message = string.Empty;
            string strEmailMessage = string.Empty;
            string strEmailSubject = string.Empty;
            strEmailMessage = CurrentUserSession.User.FirstName + " has submitted a task for review";
            strEmailSubject = "Task Submitted for review by " + CurrentUserSession.User.FirstName;
            message = "1";

            var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

            AssignmentModel objAssignmentModelEmail = new AssignmentModel();
            objAssignmentModelEmail.TaskName = assignment.TaskName;
            objAssignmentModelEmail.Comments = assignment.Comments;
            objAssignmentModelEmail.AssignID = assignment.AssignmentID;
            List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
            lstAssignmentLogModel = GetAssignmentLog(assignment.AssignmentID);
            string EmailBody = templates.TemplateContentForEmail;

            var currentUserID = CurrentUserSession.User.UserID;
            var assignmentApproverIDs = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == assignment.AssignmentID && am.CanApprove && am.UserID != currentUserID).Select(u => u.UserID).ToList();
            var usersToNotify = _entities.tblUserDepartments.Where(x => assignmentApproverIDs.Contains(x.UserId)).ToList();

            foreach (var userToNotify in usersToNotify)
            {
                templates.Subject = templates.Subject.Replace("#Subject#", strEmailSubject);
                EmailBody = PopulateEmailBody(strEmailMessage, userToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody, true);

                Global.SendEmail(userToNotify.EmailID, templates.Subject, EmailBody);
            }
            //var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == assignment.CreatedBy).FirstOrDefault();

            return RedirectToAction("Approval", "Home");
        }

        //Load Document to Popup 
        public ActionResult LoadDownloadedDoc(string name = "", string assignmentID = "", int userid = 0, int Action = 1, bool IsReadOnly = false)
        {
            var demoModel = new AssignmentModel() { DocumentName = name, UserID = CurrentUserSession.UserID, Action = Action, IsReadOnly = IsReadOnly, AssignmentID = assignmentID };

            return PartialView("_LoadDownloadedDoc", demoModel);

        }

        public ActionResult UserCanApprove(int userId, int assignmentId)
        {
            using (var _entities = new ReadyPortalDBEntities())
            {
                var member = _entities.tblAssignmentMembers.FirstOrDefault(am => am.UserID == userId && am.AssignmentID == assignmentId);
                return Json(member != null ? member.CanApprove : false, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetChangedWordPages(string FileName, int AssignmentID)
        {
            var assignedPages = _entities.tblAssignedWordPages.Where(awp => awp.AssignmentID == AssignmentID).Select(p => p.Sequence).ToList();
            var changedPages = GetChangedWordPageModels(FileName, AssignmentID);

            //Dictionary<String, WordPageModel> dict = new Dictionary<string, WordPageModel>();
            //for (var i = 0; i < assignedPages.Count; i++)
            //{
            //    var pageNum = (int)assignedPages[i];
            //    dict[pageNum.ToString()] = changedPages.FirstOrDefault(cp => cp.PageNumber == assignedPages[i]);
            //}

            return Json(changedPages, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAssignedWordPages(string FileName, int AssignmentID)
        {
            var assignedPages = _entities.tblAssignedWordPages.Where(awp => awp.AssignmentID == AssignmentID).Select(p => p.Sequence).ToList();
            List<WordPageModel> aPages = new List<WordPageModel>();

            var docPath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, "Pages_" + FileName,
                "assignedDocs");
            var thumbPath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, "Pages_" + FileName,
                "assignedThumbs");
            if (!Directory.Exists(thumbPath)) Directory.CreateDirectory(thumbPath);

            var files = Directory.GetFiles(docPath).Select(Path.GetFileName).ToList();

            for (var i = 0; i < files.Count; i++)
            {
                var pageAssignmentID = Int32.Parse(files[i].Split('-')[0]);
                var ticks = files[i].Split('-')[1];
                var pageNum = Int32.Parse((files[i].Split('-')[2]).Split('.')[0]);
                if (assignedPages.Contains(pageNum) && pageAssignmentID == AssignmentID)
                {
                    var thumbnailLink = Global.SiteUrl + "ApplicationDocuments/Words/Pages_" + FileName + "/assignedThumbs/" + files[i].ToString();
                    WordPageModel model = new WordPageModel
                    {
                        IsAssigned = true,
                        PageNumber = pageNum,
                        AssignedTo = "",
                        OriginalFile = FileName,
                        ThumbnailLink = thumbnailLink,
                        Ticks = ticks
                    };
                    aPages.Add(model);
                }
            }

            return Json(aPages, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetWordPages(string FileName)
        {
            List<WordPageModel> pages = new List<WordPageModel>();
            if (!string.IsNullOrEmpty(FileName))
            {
                pages = GetWordPageModels(FileName);
            }

            return Json(pages, JsonRequestBehavior.AllowGet);
        }

        private List<WordPageModel> GetChangedWordPageModels(string FileName, int AssignmentID)
        {
            var assignedPages = _entities.tblAssignedWordPages.Where(awp => awp.AssignmentID == AssignmentID).ToList();
            var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == AssignmentID);
            // var assignedPageNumbers = assignedPages.Select(p => p.Sequence).ToList();
            List<WordPageModel> pages = new List<WordPageModel>();

            var docPath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, "Pages_" + FileName, "changedDocs");
            var thumbPath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, "Pages_" + FileName,
                "changedThumbs");

            if (!Directory.Exists(docPath)) Directory.CreateDirectory(docPath);
            if (!Directory.Exists(thumbPath)) Directory.CreateDirectory(thumbPath);

            var cnt = 0;
            foreach (var assignedPage in assignedPages)
            {
                var pageNum = ((int)assignedPage.Sequence).ToString();
                var pageDoc = "";
                var dirFiles = new DirectoryInfo(docPath).GetFiles(AssignmentID.ToString() + "-*-" + pageNum + ".docx");
                if (dirFiles.Length > 1)
                {
                    pageDoc = dirFiles[cnt].Name;
                }
                else
                {
                    pageDoc = dirFiles[0].Name;
                }
                //var pageDoc = new DirectoryInfo(docPath).GetFiles(AssignmentID.ToString() + "-*-" + pageNum + ".docx")[0].Name;
                //var pageDoc = Directory.GetFiles(docPath +"\\"+ AssignmentID.ToString() + "-*-" + pageNum + ".docx").Select(Path.GetFileName).ToList()[0];
                var ticks = pageDoc.Split('-')[1];
                var thumbnailLink = Global.SiteUrl + "ApplicationDocuments/Words/Pages_" + FileName + "/changedThumbs/" + AssignmentID.ToString() + "-" + pageNum + ".png";
                WordPageModel model = new WordPageModel
                {
                    IsAssigned = true,
                    PageNumber = Int32.Parse(pageNum),
                    AssignedTo = "",
                    LockedByUserID = assignment.LockedByUserID,
                    OriginalFile = FileName,
                    ThumbnailLink = thumbnailLink,
                    ReviewRequested = assignedPage.ReviewRequested,
                    IsPublished = assignedPage.IsPublished,
                    Ticks = ticks
                };
                pages.Add(model);
                cnt++;
            }


            //var pageThumbs = Directory.GetFiles(thumbPath).Select(Path.GetFileName).ToList();
            //var pageDocs = Directory.GetFiles(docPath).Select(Path.GetFileName).ToList();

            //for (var i = 0; i < pageDocs.Count; i++)
            //{
            //    var pageAssignmentID = Int32.Parse(pageDocs[i].Split('-')[0]);
            //    var ticks = (pageDocs[i].Split('-')[1]);
            //    var pageNum = Int32.Parse(pageDocs[i].Split('-')[2].Split('.')[0]);

            //    //GeneratePageImage(0,new Aspose.Words.Document(docPath + "/" + AssignmentID.ToString() + "-" + ticks + "-"+ pageNum.ToString()+ ".docx"), thumbPath, AssignmentID);
            //    if (assignedPages.Contains(pageNum) && AssignmentID == pageAssignmentID)
            //    {
            //        var thumbnailLink = Global.SiteUrl + "ApplicationDocuments/Words/Pages_" + FileName + "/changedThumbs/" + AssignmentID.ToString() + "-" + pageNum.ToString() + ".png";
            //        WordPageModel model = new WordPageModel
            //        {
            //            IsAssigned = true,
            //            PageNumber = pageNum,
            //            AssignedTo = "",
            //            OriginalFile = FileName,
            //            ThumbnailLink = thumbnailLink,

            //        };
            //        pages.Add(model);
            //    }
            //}

            return pages;
        }
        private List<WordPageModel> GetWordPageModels(string FileName)
        {
            List<WordPageModel> pages = new List<WordPageModel>();

            var docPath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, "Pages_" + FileName);
            var originalWordPath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathDemo, FileName);
            if (!Directory.Exists(docPath)) //thumbnails don't exist so create new ones
            {
                SplitPages(FileName);
            }

            //var server = new RichEditDocumentServer();
            //server.Document.InsertDocumentContent(server.Document.Range.Start, originalWordPath, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Snippet to the temp document

            var pageCount = new Aspose.Words.Document(originalWordPath).PageCount;
            //var pagesWithAssignedContent = GetPagesWithAssignedContent(FileName).Select(x => x - 1);
            var pagesWithAssignedContent = GetPagesWithAssignedContent(FileName);

            List<int> assignedPages = new List<int>();
            if (pagesWithAssignedContent != null && pagesWithAssignedContent.Count > 0)
            {
                assignedPages = pagesWithAssignedContent.Select(x => x - 1).ToList();
            }
            //var pageCount = Enumerable.Range(0, Directory.GetFiles(docPath).Length).Select(x => x.ToString() + ".png");
            //var assignedPages = _entities.tblAssignedWordPages.Where(awp => awp.DocName == FileName).ToList();

            for (var i = 0; i < pageCount; i++)
            {
                string assignedTo = "";
                bool isAssigned = false;
                var thumbnailLink = Global.SiteUrl + "ApplicationDocuments/Words/Pages_" + FileName + "/" + i.ToString() + ".png";

                WordPageModel model = new WordPageModel
                {
                    IsAssigned = assignedPages.Contains(i),
                    PageNumber = i,
                    AssignedTo = assignedTo,
                    OriginalFile = FileName,
                    ThumbnailLink = thumbnailLink
                };
                pages.Add(model);
            }

            return pages;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetSlides(string FileName)
        {
            List<PPTModel> documentFileCollection = new List<PPTModel>();
            List<PPTModel> docFileCollection = new List<PPTModel>();
            if (!string.IsNullOrEmpty(FileName))
            {
                var Assigned = from p in _entities.tblAssignments
                               join q in _entities.tblAssignedPPTSlides on p.AssignmentID equals q.AssignmentID
                               join document in _entities.tblDocuments on p.DocumentID equals document.DocumentID
                               join member in _entities.tblAssignmentMembers on q.AssignmentID equals member.AssignmentID
                               where document.DocumentName == FileName && p.Action != (int)WordAutomationDemo.Common.Global.Action.Declined
                               orderby q.Sequence
                               select new { q.SlideName, p.LockedBy.FullName, p.Action, q.AssignedPPTSildeID, p.AssignmentID };

                var AssignedSlides = Assigned.Where(a => a.Action != (int)Global.Action.Approved && a.Action != (int)Global.Action.Published).Select(a => a.SlideName).Distinct().ToList();


                var pptSlides = _entities.tblPPTSlides.AsEnumerable().Where(c => c.MasterDocumentName == FileName && !(c.IsOriginal == true)).ToList();

                foreach (var slide in pptSlides)
                {
                    var assignedTo = String.Empty;
                    var assigned = Assigned.FirstOrDefault(a => a.SlideName == slide.SlideName);
                    if (assigned != null)
                    {
                        var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == assigned.AssignmentID);
                        var assignmentMembers = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == assigned.AssignmentID).ToList();

                        if (assignmentMembers.Count() > 0)
                        {
                            foreach (var am in assignmentMembers)
                            {
                                if (am.UserID != null)
                                {
                                    var fullName = _entities.tblUserDepartments.FirstOrDefault(u => u.UserId == am.UserID).FullName;
                                    var canPublish = am.CanPublish.HasValue ? am.CanPublish.Value : false;
                                    assignedTo += Global.GetPermissionString(canPublish, am.CanEdit, am.CanApprove, fullName) + "</br>";
                                }

                            }
                        }
                    }


                    var model = new PPTModel()
                    {
                        Sequence = slide.Sequence,
                        OriginalDocumentName = slide.MasterDocumentName,
                        SlideName = slide.SlideName,
                        SlideLink = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "\\" + slide.SlideName,
                        ThumbnailLink = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "\\" + slide.SlideName.Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention,
                        IsAssigned = AssignedSlides.Contains(slide.SlideName),
                        AssignedToName = assignedTo,
                        AssignedPPTSlideID = Assigned.Where(a => a.SlideName == slide.SlideName).Select(a => a.FullName).FirstOrDefault() != null ? Assigned.Where(a => a.SlideName == slide.SlideName).Select(a => a.AssignedPPTSildeID).FirstOrDefault() : 0,
                        PPTSlideID = slide.PPTSlidesID
                    };

                    docFileCollection.Add(model);
                }

                docFileCollection = docFileCollection.OrderBy(x => x.Sequence).ToList();
            }
            return Json(docFileCollection, JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetMySlides(string FileName, int userId, bool getAssigned, int AssignmentId)
        {
            List<PPTModel> documentFileCollection = new List<PPTModel>();
            if (!string.IsNullOrEmpty(FileName) && userId > 0)
            {
                var AssignedSlides = from items in _entities.tblAssignedPPTSlides
                                     join assign in _entities.tblAssignments on items.AssignmentID equals assign.AssignmentID
                                     join document in _entities.tblDocuments on assign.DocumentID equals document.DocumentID
                                     //where assign.UserID == userId
                                     where assign.AssignmentID == AssignmentId
                                     select new PPTModel
                                     {
                                         AssignedPPTSlideID = items.AssignedPPTSildeID,
                                         AssignedTaskID = items.AssignmentID,
                                         SlideName = items.SlideName,
                                         Action = assign.Action,
                                         OriginalDocumentName = document.DocumentName,
                                         AssignedTo = assign.LockedByUserID,
                                         IsPPTModified = items.IsPPTModified,
                                         IsPPTApproved = items.IsPPTApproved,
                                         LockedByUserID = assign.LockedByUserID,
                                         IsPublished = items.IsPublished,
                                         ReviewRequested = items.ReviewRequested,
                                         PPTRemarks = items.PPTRemarks != null ? items.PPTRemarks : "",
                                     };

                var completed = (int)WordAutomationDemo.Common.Global.Action.Completed;
                var assigned = (int)WordAutomationDemo.Common.Global.Action.Assigned;

                documentFileCollection.AddRange(AssignedSlides.AsEnumerable().Select(i => new PPTModel()
                {
                    Action = i.Action,
                    AssignedPPTSlideID = i.AssignedPPTSlideID,
                    AssignedTaskID = i.AssignedTaskID,
                    OriginalDocumentName = i.OriginalDocumentName,
                    SlideName = i.SlideName,
                    SlideLink = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/AssignedDoc/" + ((getAssigned) ? i.AssignedTaskID.ToString() + "_Copy_" + i.SlideName : ((i.IsPPTModified.HasValue && i.IsPPTModified == true) ? "New_" : string.Empty) + i.AssignedTaskID.ToString() + "_Copy_" + i.SlideName),
                    IsPPTModified = i.IsPPTModified,
                    IsPPTApproved = i.IsPPTApproved,
                    LockedByUserID = i.LockedByUserID,
                    AssignedTo = i.AssignedTo,
                    IsPublished = i.IsPublished,
                    ReviewRequested = i.ReviewRequested,
                    PPTRemarks = i.PPTRemarks != null ? i.PPTRemarks : "",
                }).ToList());

                foreach (var item in documentFileCollection)
                {
                    if (!getAssigned && (item.IsPPTModified.HasValue && item.IsPPTModified.Value == true))
                    {
                        var fileurl = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + FileName + "/AssignedDoc/" + ("New_" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName);
                        //Application pptApplication = new Application();
                        //Microsoft.Office.Interop.PowerPoint.Presentation pptPresentation = pptApplication.Presentations.Open(fileurl, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);
                        //for (int j = 1; j <= pptPresentation.Slides.Count; j++)
                        //{
                        //    var SlideCount = string.Empty;
                        //    if (item.Action != assigned)
                        //        SlideCount = "(" + j + ")";
                        //    var link = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/AssignedDoc/" + ("New_" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName.Split('.').FirstOrDefault() + SlideCount + WordAutomationDemo.Common.Global.ImageExportExtention);
                        //    item.ThumbnailLink += "&nbsp;&nbsp;  <img onmouseover='ShowImg(this);' onmouseout='HideImg(this);' style='border : 1px solid;' class='names' src='" + link + "' height=50px width=50px data-id='" + link + "'/>";
                        //}
                        //pptPresentation.Close();


                        //Spire
                        if (System.IO.File.Exists(fileurl))
                        {
                            Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(fileurl);
                            item.ThumbnailLink += "<td class='popover-hide-xs'><div class='aniimated-thumbnials'>";
                            for (int j = 0; j < presentation.Slides.Count; j++)
                            {
                                var SlideCount = string.Empty;
                                //if (item.IsPPTModified.HasValue && item.IsPPTModified.Value == true)
                                SlideCount = "(" + j + ")";
                                var link = Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/AssignedDoc/" + ("New_" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName.Split('.').FirstOrDefault() + SlideCount + WordAutomationDemo.Common.Global.ImageExportExtention);
                                //item.ThumbnailLink += "&nbsp;&nbsp;  <img onmouseover='ShowImg(this);' onmouseout='HideImg(this);' style='border : 1px solid;' class='names' src='" + link + "' height=50px width=50px data-id='" + link + "'/>";
                                //item.ThumbnailLink += "" + link + ">";
                                item.ThumbnailLink += " <a href='" + link + "' data-toggle='popover' data-trigger='hover' data-placement='bottom' id='ppt" + item.AssignedPPTSlideID + "_" + j + "' class='gridthumb' data-original-title=''><img src='" + link + "' class='img-responsive img-thumbnail'><div id='popover-content-ppt" + item.AssignedPPTSlideID + "_" + j + "' class='hide'><img src='" + link + "' class='img-responsive'></div></a>";

                                item.lstThumbnailLinkForOldSlide.Add(link);
                                var linkOld = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/AssignedDoc/" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName.Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention;
                                //item.ThumbnailLinkForOldSlide = "&nbsp;&nbsp;  <img onmouseover='ShowImg(this);' onmouseout='HideImg(this);' style='border : 1px solid;' class='names' src='" + linkOld + "' height=50px width=50px data-id='" + linkOld + "'/>";
                                item.ThumbnailLinkForOldSlide = "<td class='popover-hide-xs'><a href='javascript:;' data-toggle='popover' data-trigger='hover' data-placement='bottom' id='ppt" + item.AssignedPPTSlideID + "' class='gridthumb' data-original-title=''><img src='" + linkOld + "' class='img-responsive img-thumbnail'></a><div id='popover-content-ppt" + item.AssignedPPTSlideID + "' class='hide'><img src='" + linkOld + "' class='img-responsive'></div></td>";
                            }
                            item.ThumbnailLink += "</div></td>";
                        }
                    }
                    else
                    {
                        item.ThumbnailLink += "<td class='popover-hide-xs'><div class='aniimated-thumbnials'>";
                        var link = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/AssignedDoc/" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName.Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention;
                        //item.ThumbnailLink += "&nbsp;&nbsp;  <img onmouseover='ShowImg(this);' onmouseout='HideImg(this);' style='border : 1px solid;' class='names' src='" + link + "' height=50px width=50px data-id='" + link + "'/>";
                        //item.ThumbnailLink += "" + link + ">";
                        item.ThumbnailLink += "<a href='" + link + "' data-toggle='popover' data-trigger='hover' data-placement='bottom' id='ppt" + item.AssignedPPTSlideID + "' class='gridthumb' data-original-title=''><img src='" + link + "' class='img-responsive img-thumbnail'><div id='popover-content-ppt" + item.AssignedPPTSlideID + "' class='hide'><img src='" + link + "' class='img-responsive'></div></a></div></td>";

                        var linkOld = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/AssignedDoc/" + item.AssignedTaskID.ToString() + "_Copy_" + item.SlideName.Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention;
                        //item.ThumbnailLinkForOldSlide = "&nbsp;&nbsp;  <img onmouseover='ShowImg(this);' onmouseout='HideImg(this);' style='border : 1px solid;' class='names' src='" + linkOld + "' height=50px width=50px data-id='" + linkOld + "'/>";
                        item.ThumbnailLinkForOldSlide = "<td class='popover-hide-xs'><a href='javascript:;' data-toggle='popover' data-trigger='hover' data-placement='bottom' id='ppt" + item.AssignedPPTSlideID + "' class='gridthumb' data-original-title=''><img src='" + linkOld + "' class='img-responsive img-thumbnail'></a><div id='popover-content-ppt" + item.AssignedPPTSlideID + "' class='hide'><img src='" + linkOld + "' class='img-responsive'></div></td>";
                    }
                }
            }
            return Json(documentFileCollection, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetUploadedDoc(int userId, bool getAssigned, int AssignmentId)
        {
            List<UploadedDocModel> lstUploadedDocModel = new List<UploadedDocModel>();
            var ProjectFolderPath = Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, AssignmentId.ToString());
            if (Directory.Exists(ProjectFolderPath))
            {

                var AssignedDocs = from items in _entities.tblAssignedWordPages
                                   join assign in _entities.tblAssignments on items.AssignmentID equals assign.AssignmentID
                                   where assign.AssignmentID == AssignmentId
                                   select new UploadedDocModel
                                   {
                                       AssignedDocsID = items.AssignedWordPageID,
                                       DocName = items.DocName,
                                       AssignmentID = items.AssignmentID.HasValue ? items.AssignmentID.Value : 0,
                                       Sequence = items.Sequence.HasValue ? items.Sequence.Value : 0
                                   };

                lstUploadedDocModel.AddRange(AssignedDocs.AsEnumerable().Select(i => new UploadedDocModel()
                {
                    AssignedDocsID = i.AssignedDocsID,
                    AssignmentID = i.AssignmentID,
                    DocName = i.DocName,
                    DocURL = Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, i.AssignmentID.ToString() + "/" + i.DocName),
                    DownloadImagePath = Global.SiteUrl + "Css/Images/download.png",
                    Sequence = i.Sequence,
                    IsDocFile = i.DocName.Split('.').Last().ToLower() == "doc" || i.DocName.Split('.').Last().ToLower() == "docx" ? true : false,
                    IsPPTFile = i.DocName.Split('.').Last().ToLower() == "ppt" || i.DocName.Split('.').Last().ToLower() == "pptx" ? true : false,
                }).ToList().OrderBy(s => s.Sequence));

            }

            return Json(lstUploadedDocModel, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetBlackline(string filename)
        {
            DocumentComparisonUtil docCompUtil = new DocumentComparisonUtil();
            int added = 0, deleted = 0;

            var originalDoc = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, "Original_" + filename);
            var updatedDoc = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathDemo, filename);
            var comparisonDoc = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, "Blackline_" + filename.Split('.')[0] + ".pdf");

            if (!System.IO.File.Exists(originalDoc))
            {
                var newOriginalDoc = new Aspose.Words.Document(updatedDoc);
                newOriginalDoc.Save(originalDoc);
            }

            docCompUtil.Compare(originalDoc, updatedDoc, comparisonDoc, ref added, ref deleted);

            return Json(comparisonDoc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllSlides(string FileName)
        {
            List<PPTModel> documentFileCollection = new List<PPTModel>();
            if (!string.IsNullOrEmpty(FileName))
            {
                var AllSlides = from items in _entities.tblPPTSlides
                                where items.MasterDocumentName == FileName
                                orderby items.Sequence
                                select items;

                documentFileCollection.AddRange(AllSlides.AsEnumerable().Where(a => !(a.IsOriginal == true)).Select(i => new PPTModel()
                {
                    Sequence = i.Sequence,
                    OriginalDocumentName = i.MasterDocumentName,
                    SlideName = i.SlideName,
                    SlideLink = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/" + i.SlideName.ToString(),
                }).ToList());


                int k = 0;
                foreach (var item in documentFileCollection)
                {
                    var link = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/" + item.SlideName.Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention;
                    item.ThumbnailLink += " <a href='" + link + "' data-toggle='popover' data-trigger='hover' data-placement='bottom' id='ppt" + item.AssignedPPTSlideID + "_" + k + "' class='gridthumb' data-original-title=''><img src='" + link + "' class='img-responsive img-thumbnail'><div id='popover-content-ppt" + item.AssignedPPTSlideID + "_" + k + "' class='hide'><img src='" + link + "' class='img-responsive'></div></a>";
                    k++;
                }
            }


            return Json(documentFileCollection, JsonRequestBehavior.AllowGet);



        }

        public ActionResult GetAllTaskSlides(int? assignedDocsID)
        {
            List<PPTModel> documentFileCollection = new List<PPTModel>();
            if (assignedDocsID > 0)
            {
                var AllSlides = from items in _entities.tblAssignedPPTSlides
                                join assign in _entities.tblAssignedWordPages on items.AssignedDocsID equals assign.AssignedWordPageID
                                where items.AssignedDocsID == assignedDocsID
                                orderby items.Sequence
                                select new
                                {
                                    AssignmentID = assign.AssignmentID,
                                    Sequence = items.Sequence,
                                    OriginalDocumentName = items.SlideName,
                                    SlideName = items.SlideName,
                                    DocName = assign.DocName
                                };

                documentFileCollection.AddRange(AllSlides.AsEnumerable().Select(i => new PPTModel()
                {
                    AssignmentID = i.AssignmentID.ToString(),
                    Sequence = i.Sequence,
                    OriginalDocumentName = i.SlideName,
                    SlideName = i.SlideName,
                    OriginalFile = i.DocName,
                    SlideLink = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/TaskDocuments/" + i.AssignmentID + "/PPT/" + i.DocName + "/" + i.SlideName.ToString(),
                }).ToList());


                foreach (var item in documentFileCollection)
                {

                    var link = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/TaskDocuments/" + item.AssignmentID + "/PPT/" + item.OriginalFile + "/" + item.SlideName.ToString().Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention;
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




        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetSlidesAssignedByMe(string FileName, int userId, int AssignmentId)
        {
            List<PPTModel> documentFileCollection = new List<PPTModel>();
            if (!string.IsNullOrEmpty(FileName) && userId > 0)
            {
                var AssignedSlides = from items in _entities.tblAssignedPPTSlides
                                     join assign in _entities.tblAssignments on items.AssignmentID equals assign.AssignmentID
                                     join document in _entities.tblDocuments on assign.DocumentID equals document.DocumentID
                                     where assign.AssignmentID == AssignmentId
                                     select new PPTModel
                                     {
                                         OriginalDocumentName = document.DocumentName,
                                         AssignedTaskID = items.AssignmentID,
                                         SlideName = items.SlideName,
                                         AssignedTo = assign.LockedByUserID,
                                         AssignedPPTSlideID = items.AssignedPPTSildeID
                                     };

                documentFileCollection.AddRange(AssignedSlides.AsEnumerable().Select(i => new PPTModel()
                {
                    OriginalDocumentName = i.OriginalDocumentName,
                    SlideName = i.SlideName,
                    SlideLink = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/AssignedDoc/" + i.AssignedTaskID.ToString() + "_Copy_" + i.SlideName,
                    //ThumbnailLink = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/AssignedDoc/" + i.AssignedTaskID.ToString() + "_Copy_" + i.SlideName.Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention,
                    //ThumbnailLink = "&nbsp;&nbsp;  <img onmouseover='ShowImg(this);' onmouseout='HideImg(this);' style='border : 1px solid;' class='names' src='" + WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/AssignedDoc/" + i.AssignedTaskID.ToString() + "_Copy_" + i.SlideName.Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention + "' height=50px width=50px data-id='" + WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/AssignedDoc/" + i.AssignedTaskID.ToString() + "_Copy_" + i.SlideName.Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention + "'/>"
                    ThumbnailLink = "<td class='popover-hide-xs'><a href='javascript:;' data-toggle='popover' data-trigger='hover' data-placement='bottom' id='ppt" + i.AssignedPPTSlideID + "' class='gridthumb' data-original-title=''><img src='" + WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/AssignedDoc/" + i.AssignedTaskID.ToString() + "_Copy_" + i.SlideName.Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention + "' class='img-responsive img-thumbnail'></a><div id='popover-content-ppt" + i.AssignedPPTSlideID + "' class='hide'><img src='" + WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/PPTs/Slides_" + FileName + "/AssignedDoc/" + i.AssignedTaskID.ToString() + "_Copy_" + i.SlideName.Split('.').FirstOrDefault() + WordAutomationDemo.Common.Global.ImageExportExtention + "' class='img-responsive'></div></td>"
                }).ToList());

            }
            return Json(documentFileCollection, JsonRequestBehavior.AllowGet);
        }

        public static List<SelectListItem> GetPPTFilesInDirectory()
        {
            //string[] allowedExtensions = { "*.ppt", "*.pptx" };
            List<SelectListItem> documentFileCollection = new List<SelectListItem>();
            //foreach (string extension in allowedExtensions)
            //{
            //    var x = Directory.GetFiles(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, extension, SearchOption.TopDirectoryOnly);
            //    documentFileCollection.AddRange(x.Select(i => new SelectListItem()
            //                                             {
            //                                                 Text = i.ToString().Split('\\').Last(),
            //                                                 Value = i.ToString().Split('\\').Last()
            //                                             }).ToList());
            //}


            documentFileCollection = (_entities.tblPPTSlides.Where(x => !(x.IsOriginal == true)).Select(z => z.MasterDocumentName).Distinct().ToList())
                                                .Select(x => new SelectListItem
                                                {
                                                    Text = x.ToString(),
                                                    Value = x.ToString()
                                                }).ToList();

            return documentFileCollection;

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetDocFilesInDirectoryJson()
        {
            var result = CommonHelper.GetDocFilesInDirectory(_entities);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string RequestReviewPage(FormCollection objFormCollection)
        {
            Result objAvailabilityResult = new Result();
            if (objFormCollection != null && objFormCollection["assignmentID"] != null &&
                objFormCollection["PageNumber"] != null && objFormCollection["originalFile"] != null)
            {
                int assignmentID = Int32.Parse(objFormCollection["assignmentID"]);
                int pageNumber = Int32.Parse(objFormCollection["PageNumber"]);
                string remarks = objFormCollection["PageRemarks"];
                string originalFile = objFormCollection["originalFile"];
                string ticks = objFormCollection["ticks"];

                var assignment = _entities.tblAssignments.Where(c => c.AssignmentID == assignmentID).FirstOrDefault();
                if (assignment != null)
                {
                    var assignedPages = _entities.tblAssignedWordPages.Where(awp =>
                        awp.AssignmentID == assignment.AssignmentID && awp.Sequence == pageNumber).ToList();
                    tblAssignedWordPage assignedPage;
                    if (ticks != null && !String.IsNullOrEmpty(ticks) && assignedPages.Count > 1)
                    {
                        assignedPage = assignedPages.FirstOrDefault(awp => awp.Ticks == ticks);
                    }
                    else
                    {
                        assignedPage = _entities.tblAssignedWordPages.FirstOrDefault(awp =>
                            awp.AssignmentID == assignment.AssignmentID && awp.Sequence == pageNumber);
                    }

                    assignedPage.ReviewRequested = true;

                    tblAssignmentLog _log = new tblAssignmentLog();
                    _log.Action = (int)Global.Action.Completed;
                    _log.AssignmentID = assignment.AssignmentID;
                    _log.CreatedDate = DateTime.Now;
                    _log.CreatedDate = DateTime.Now;
                    _log.Description = remarks;
                    _log.CreatedBy = CurrentUserSession.User.UserID;
                    _entities.tblAssignmentLogs.Add(_log);
                    _entities.SaveChanges();

                    var allPages = _entities.tblAssignedWordPages.Where(sl => sl.AssignmentID == assignment.AssignmentID).ToList();

                    var publishedCount = allPages.Where(sl => sl.IsPublished.HasValue && sl.IsPublished.Value).Count();
                    var pendingApprovalCount = allPages.Where(sl => !sl.IsPublished.HasValue && sl.ReviewRequested.HasValue && sl.ReviewRequested.Value).Count();

                    bool taskfinished = false;
                    bool taskCompleted = false;
                    if (publishedCount == allPages.Count)
                    {
                        taskfinished = true;
                    }
                    if (pendingApprovalCount + publishedCount == allPages.Count && pendingApprovalCount > 0)
                    {
                        taskCompleted = true;
                    }

                    string message = string.Empty;
                    string strEmailMessage = string.Empty;
                    string strEmailSubject = string.Empty;

                    if (taskfinished)
                    {
                        assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
                        assignment.CompletedDate = DateTime.Now;
                        _entities.SaveChanges();
                        strEmailMessage = "A task has been Finished by " + CurrentUserSession.User.FirstName;
                        strEmailSubject = "Task Completed: Task has been finished by " + CurrentUserSession.User.FirstName;
                        message = "1";
                    }
                    else if (taskCompleted)
                    {
                        assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                        assignment.CompletedDate = DateTime.Now;
                        _entities.SaveChanges();
                        strEmailMessage = "A task has been Completed by " + CurrentUserSession.User.FirstName;
                        strEmailSubject = "Task Completed: Task has been completed by " + CurrentUserSession.User.FirstName;
                        message = "1";
                    }
                    else
                    {
                        message = string.Empty;
                        strEmailMessage = "A page has been finished by " + CurrentUserSession.User.FirstName;
                        strEmailSubject = "Page Completed: Slide has been finished by " + CurrentUserSession.User.FirstName;
                        message = "2";
                    }

                    //Send email to approvers
                    var assignmentMembers = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == assignment.AssignmentID && am.CanApprove).Select(a => new AssignmentMemberModel()
                    {
                        UserID = (int)a.UserID,
                        CanApprove = a.CanApprove,
                        CanEdit = a.CanEdit
                    }).ToList();

                    var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                    AssignmentModel modelEmail = new AssignmentModel();
                    modelEmail.TaskName = assignment.TaskName;
                    modelEmail.Comments = assignment.Comments;
                    modelEmail.AssignID = assignment.AssignmentID;
                    List<AssignmentLogModel> logModels = new List<AssignmentLogModel>();
                    logModels = GetAssignmentLog(assignment.AssignmentID);
                    string EmailBody = templates.TemplateContentForEmail;
                    var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == assignment.CreatedBy).FirstOrDefault();
                    templates.Subject = templates.Subject.Replace("#Subject#", strEmailSubject);
                    EmailBody = PopulateEmailBody(strEmailMessage, UserToNotify.FullName, modelEmail, logModels, EmailBody, true);

                    byte[] fileBytes = CommonHelper.GeneratePDF(strEmailMessage, UserToNotify.FullName, modelEmail, logModels);

                    //NotifyMembersOfAssignment(assignmentMembers, modelEmail, assignment.AssignmentID);

                    objAvailabilityResult.Status = true;
                    objAvailabilityResult.ErrorCode = "";
                    objAvailabilityResult.ErrorMessage = message;
                }
            }

            return objAvailabilityResult.ErrorMessage;

        }

        [HttpPost]
        public string RequestReviewSlide(FormCollection objFormCollection)
        {
            Result objAvailabilityResult = new Result();
            if (objFormCollection != null && objFormCollection["assignmentID"] != null && objFormCollection["assignedPPTSlideID"] != null && objFormCollection["originalFile"] != null && objFormCollection["slideName"] != null)
            {
                int assignmentID = Int32.Parse(objFormCollection["assignmentID"]);
                int assignedPPTSlideID = Int32.Parse(objFormCollection["assignedPPTSlideID"]);
                string remarks = objFormCollection["PPTRemarks"];
                string originalFile = objFormCollection["originalFile"];
                string slideName = objFormCollection["slideName"];

                var assignment = _entities.tblAssignments.Where(c => c.AssignmentID == assignmentID).FirstOrDefault();
                if (assignment != null)
                {
                    var pptSlide = _entities.tblAssignedPPTSlides.FirstOrDefault(sl => sl.AssignmentID == assignmentID && sl.AssignedPPTSildeID == assignedPPTSlideID);
                    pptSlide.IsPPTModified = true;
                    pptSlide.ReviewRequested = true;
                    pptSlide.PPTRemarks = remarks;
                    //pptSlide.IsGrayedOut = true;
                    _entities.Entry(pptSlide).State = EntityState.Modified;

                    tblAssignmentLog _log = new tblAssignmentLog();
                    _log.Action = (int)Global.Action.Completed;
                    _log.AssignmentID = assignment.AssignmentID;
                    _log.CreatedDate = DateTime.Now;
                    _log.CreatedDate = DateTime.Now;
                    _log.Description = "Slide " + pptSlide.Sequence.ToString() + " - " + remarks;
                    _log.CreatedBy = CurrentUserSession.User.UserID;
                    _entities.tblAssignmentLogs.Add(_log);
                    _entities.SaveChanges();

                    var allSlides = _entities.tblAssignedPPTSlides.Where(sl => sl.AssignmentID == assignment.AssignmentID).ToList();
                    var allPendingApproval = allSlides.Count == allSlides.Where(sl => sl.ReviewRequested.HasValue && sl.ReviewRequested.Value).Count();

                    string message = string.Empty;
                    string strEmailMessage = string.Empty;
                    string strEmailSubject = string.Empty;

                    var publishedCount = allSlides.Where(sl => sl.IsPublished.HasValue && sl.IsPublished.Value).Count();
                    var approvedCount = allSlides.Where(sl => sl.IsPPTApproved.HasValue && sl.IsPPTApproved.Value).Count();
                    var pendingApprovalCount = allSlides.Where(sl => !sl.IsPPTApproved.HasValue && sl.ReviewRequested.HasValue && sl.ReviewRequested.Value).Count();

                    bool taskCompleted = false;

                    if (pendingApprovalCount + publishedCount == allSlides.Count)
                    {
                        taskCompleted = true;
                    }

                    if (taskCompleted)
                    {
                        assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                        assignment.CompletedDate = DateTime.Now;
                        _entities.SaveChanges();
                        strEmailMessage = "A task has been Completed by " + CurrentUserSession.User.FirstName;
                        strEmailSubject = "Task Completed: Task has been completed by " + CurrentUserSession.User.FirstName;
                        message = "1";
                    }
                    else
                    {
                        message = string.Empty;
                        strEmailMessage = "A slide has been completed by " + CurrentUserSession.User.FirstName;
                        strEmailSubject = "Slide Completed: Slide has been completed by " + CurrentUserSession.User.FirstName;
                        message = "2";
                    }

                    //Send email to approvers
                    var assignmentMembers = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == assignment.AssignmentID && am.CanApprove).Select(a => new AssignmentMemberModel()
                    {
                        UserID = (int)a.UserID,
                        CanApprove = a.CanApprove,
                        CanEdit = a.CanEdit
                    }).ToList();

                    var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                    AssignmentModel modelEmail = new AssignmentModel();
                    modelEmail.TaskName = assignment.TaskName;
                    modelEmail.Comments = assignment.Comments;
                    modelEmail.AssignID = assignment.AssignmentID;
                    List<AssignmentLogModel> logModels = new List<AssignmentLogModel>();
                    logModels = GetAssignmentLog(assignment.AssignmentID);
                    string EmailBody = templates.TemplateContentForEmail;
                    var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == assignment.CreatedBy).FirstOrDefault();
                    templates.Subject = templates.Subject.Replace("#Subject#", strEmailSubject);
                    EmailBody = PopulateEmailBody(strEmailMessage, UserToNotify.FullName, modelEmail, logModels, EmailBody, true);

                    byte[] fileBytes = CommonHelper.GeneratePDF(strEmailMessage, UserToNotify.FullName, modelEmail, logModels);

                    //NotifyMembersOfAssignment(assignmentMembers, modelEmail, assignment.AssignmentID);

                    objAvailabilityResult.Status = true;
                    objAvailabilityResult.ErrorCode = "";
                    objAvailabilityResult.ErrorMessage = message;

                }

            }
            return objAvailabilityResult.ErrorMessage;
        }

        public bool ApprovePages(string doc, int assignmentID)
        {
            var allApproved = true;
            var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == assignmentID);
            var document = _entities.tblDocuments.FirstOrDefault(d => d.DocumentID == assignment.DocumentID);
            var fileDir = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, "Pages_" + document.DocumentName, "changedDocs");
            var files = new DirectoryInfo(fileDir).GetFiles(assignmentID + "-*.docx");

            foreach (var file in files)
            {
                int pageNum = Int32.Parse(file.Name.Split('-')[2].Split('.')[0]);
                string ticks = file.Name.Split('-')[1];

                tblAssignedWordPage assignedPage;
                var ap = _entities.tblAssignedWordPages.FirstOrDefault(awp =>
                    awp.AssignmentID == assignmentID && awp.Ticks == ticks);
                if (ap != null)
                {
                    assignedPage = ap;
                }
                else
                {
                    assignedPage = _entities.tblAssignedWordPages.FirstOrDefault(awp => awp.AssignmentID == assignmentID && awp.Sequence == pageNum);
                }

                if (assignedPage.IsPublished == null)
                {
                    if (!UpdateTask(file.Name, ((int)Global.Action.Approved).ToString(), new RichEditDocumentServer(), "",
                        "", -1, assignmentID, true))
                    {
                        allApproved = false;
                    }
                    else
                    {
                        assignedPage.IsPublished = true;
                        _entities.SaveChanges();
                    }
                }
            }

            if (allApproved)
            {
                tblAssignmentLog _log = new tblAssignmentLog();
                _log.Action = (int)Global.Action.Approved;
                _log.AssignmentID = assignment.AssignmentID;
                _log.CreatedDate = DateTime.Now;
                _log.CreatedDate = DateTime.Now;
                _log.Description = "";
                _log.CreatedBy = CurrentUserSession.User.UserID;
                _entities.tblAssignmentLogs.Add(_log);
                _entities.SaveChanges();

                assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
                assignment.CompletedDate = DateTime.Now;
                _entities.SaveChanges();
                var strEmailMessage = "A task has been Finished by " + CurrentUserSession.User.FirstName;
                var strEmailSubject = "Task Completed: Task has been finished by " + CurrentUserSession.User.FirstName;

                //Send email to approvers
                var assignmentMembers = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == assignment.AssignmentID && am.CanApprove).Select(a => new AssignmentMemberModel()
                {
                    UserID = (int)a.UserID,
                    CanApprove = a.CanApprove,
                    CanEdit = a.CanEdit
                }).ToList();

                var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                AssignmentModel modelEmail = new AssignmentModel();
                modelEmail.TaskName = assignment.TaskName;
                modelEmail.Comments = assignment.Comments;
                modelEmail.AssignID = assignment.AssignmentID;
                List<AssignmentLogModel> logModels = new List<AssignmentLogModel>();
                logModels = GetAssignmentLog(assignment.AssignmentID);
                string EmailBody = templates.TemplateContentForEmail;
                var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == assignment.CreatedBy).FirstOrDefault();
                templates.Subject = templates.Subject.Replace("#Subject#", strEmailSubject);
                EmailBody = PopulateEmailBody(strEmailMessage, UserToNotify.FullName, modelEmail, logModels, EmailBody, true);

                byte[] fileBytes = CommonHelper.GeneratePDF(strEmailMessage, UserToNotify.FullName, modelEmail, logModels);
            }

            return allApproved;
        }
        public string PublishPage(FormCollection objFormCollection)
        {
            Result objAvailabilityResult = new Result();
            if (objFormCollection != null && objFormCollection["assignmentID"] != null &&
                objFormCollection["ticks"] != null &&
                objFormCollection["PageNumber"] != null && objFormCollection["originalFile"] != null)
            {
                int assignmentID = Int32.Parse(objFormCollection["assignmentID"]);
                int pageNumber = Int32.Parse(objFormCollection["PageNumber"]);
                string ticks = objFormCollection["ticks"];
                string remarks = objFormCollection["PageRemarks"];
                string originalFile = objFormCollection["originalFile"];

                var assignment = _entities.tblAssignments.Where(c => c.AssignmentID == assignmentID).FirstOrDefault();
                if (assignment != null)
                {
                    tblAssignedWordPage assignedPage;

                    var ap =
                        _entities.tblAssignedWordPages.FirstOrDefault(
                            awp => awp.AssignmentID == assignment.AssignmentID && awp.Ticks == ticks);

                    if (ap != null)
                    {
                        assignedPage = ap;
                    }
                    else
                    {
                        assignedPage = _entities.tblAssignedWordPages.FirstOrDefault(awp =>
                            awp.AssignmentID == assignment.AssignmentID && awp.Sequence == pageNumber);
                    }

                    var filename = assignmentID.ToString() + "-" + ticks + "-" + pageNumber.ToString() + ".docx";

                    //Update original doc
                    if (!UpdateTask(filename, ((int)Global.Action.Published).ToString(), new RichEditDocumentServer(),
                        remarks, "", -1, assignmentID, true))
                    {
                        return objAvailabilityResult.ErrorMessage;
                    }

                    assignedPage.IsPublished = true;

                    tblAssignmentLog _log = new tblAssignmentLog();
                    _log.Action = (int)Global.Action.Published;
                    _log.AssignmentID = assignment.AssignmentID;
                    _log.CreatedDate = DateTime.Now;
                    _log.CreatedDate = DateTime.Now;
                    _log.Description = remarks;
                    _log.CreatedBy = CurrentUserSession.User.UserID;
                    _entities.tblAssignmentLogs.Add(_log);
                    _entities.SaveChanges();

                    var allPages = _entities.tblAssignedWordPages.Where(sl => sl.AssignmentID == assignment.AssignmentID).ToList();

                    var publishedCount = allPages.Where(sl => sl.IsPublished.HasValue && sl.IsPublished.Value).Count();
                    var pendingApprovalCount = allPages.Where(sl => !sl.IsPublished.HasValue && sl.ReviewRequested.HasValue && sl.ReviewRequested.Value).Count();

                    bool taskfinished = false;
                    bool taskCompleted = false;
                    if (publishedCount == allPages.Count)
                    {
                        taskfinished = true;
                    }
                    if (pendingApprovalCount + publishedCount == allPages.Count && pendingApprovalCount > 0)
                    {
                        taskCompleted = true;
                    }

                    string message = string.Empty;
                    string strEmailMessage = string.Empty;
                    string strEmailSubject = string.Empty;

                    if (taskfinished)
                    {
                        assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Published;
                        assignment.CompletedDate = DateTime.Now;
                        _entities.SaveChanges();
                        strEmailMessage = "A task has been Finished by " + CurrentUserSession.User.FirstName;
                        strEmailSubject = "Task Completed: Task has been finished by " + CurrentUserSession.User.FirstName;
                        message = "1";
                    }
                    else if (taskCompleted)
                    {
                        assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                        assignment.CompletedDate = DateTime.Now;
                        _entities.SaveChanges();
                        strEmailMessage = "A task has been Completed by " + CurrentUserSession.User.FirstName;
                        strEmailSubject = "Task Completed: Task has been completed by " + CurrentUserSession.User.FirstName;
                        message = "1";
                    }
                    else
                    {
                        message = string.Empty;
                        strEmailMessage = "A page has been finished by " + CurrentUserSession.User.FirstName;
                        strEmailSubject = "Page Completed: Page has been finished by " + CurrentUserSession.User.FirstName;
                        message = "2";
                    }

                    //Send email to approvers
                    var assignmentMembers = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == assignment.AssignmentID && am.CanApprove).Select(a => new AssignmentMemberModel()
                    {
                        UserID = (int)a.UserID,
                        CanApprove = a.CanApprove,
                        CanEdit = a.CanEdit
                    }).ToList();

                    var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                    AssignmentModel modelEmail = new AssignmentModel();
                    modelEmail.TaskName = assignment.TaskName;
                    modelEmail.Comments = assignment.Comments;
                    modelEmail.AssignID = assignment.AssignmentID;
                    List<AssignmentLogModel> logModels = new List<AssignmentLogModel>();
                    logModels = GetAssignmentLog(assignment.AssignmentID);
                    string EmailBody = templates.TemplateContentForEmail;
                    var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == assignment.CreatedBy).FirstOrDefault();
                    templates.Subject = templates.Subject.Replace("#Subject#", strEmailSubject);
                    EmailBody = PopulateEmailBody(strEmailMessage, UserToNotify.FullName, modelEmail, logModels, EmailBody, true);

                    byte[] fileBytes = CommonHelper.GeneratePDF(strEmailMessage, UserToNotify.FullName, modelEmail, logModels);

                    //NotifyMembersOfAssignment(assignmentMembers, modelEmail, assignment.AssignmentID);

                    objAvailabilityResult.Status = true;
                    objAvailabilityResult.ErrorCode = "";
                    objAvailabilityResult.ErrorMessage = message;
                }
            }

            return objAvailabilityResult.ErrorMessage;
        }

        public string PublishSlide(FormCollection objFormCollection)
        {
            Result objAvailabilityResult = new Result();
            if (objFormCollection != null && objFormCollection["assignmentID"] != null && objFormCollection["assignedPPTSlideID"] != null && objFormCollection["originalFile"] != null && objFormCollection["slideName"] != null)
            {
                int assignmentID = Int32.Parse(objFormCollection["assignmentID"]);
                int assignedPPTSlideID = Int32.Parse(objFormCollection["assignedPPTSlideID"]);
                string remarks = objFormCollection["PPTRemarks"];
                string originalFile = objFormCollection["originalFile"];
                string slideName = objFormCollection["slideName"];

                var assignment = _entities.tblAssignments.Where(c => c.AssignmentID == assignmentID).FirstOrDefault();
                if (assignment != null)
                {
                    var pptSlide = _entities.tblAssignedPPTSlides.FirstOrDefault(sl => sl.AssignmentID == assignmentID && sl.AssignedPPTSildeID == assignedPPTSlideID);
                    pptSlide.IsPPTModified = true;
                    pptSlide.IsPublished = true;
                    pptSlide.PPTRemarks = remarks;
                    //pptSlide.IsGrayedOut = true;
                    _entities.Entry(pptSlide).State = EntityState.Modified;

                    var AssignedSlide = new PPTModel()
                    {
                        AssignedTaskID = pptSlide.AssignmentID,
                        OriginalFile = originalFile,
                        OriginalSlideName = pptSlide.SlideName,
                        SlideName = "New_" + pptSlide.AssignmentID.ToString() + "_Copy_" + pptSlide.SlideName,
                        IsPPTApproved = pptSlide.IsPPTApproved,
                        AssignedPPTSlideID = pptSlide.AssignedPPTSildeID
                    };

                    var LastFile = _entities.tblPPTSlides.Where(c => c.MasterDocumentName == originalFile && !(c.IsOriginal == true)).OrderByDescending(c => c.PPTSlidesID).FirstOrDefault();
                    var MyOriFile = _entities.tblPPTSlides.Where(c => c.MasterDocumentName == originalFile && c.SlideName == AssignedSlide.OriginalSlideName && !(c.IsOriginal == true)).FirstOrDefault();
                    int MyFileSeq = Convert.ToInt32(MyOriFile.Sequence);
                    int LastSlide = Convert.ToInt32(LastFile.SlideName.Split('.')[0]);
                    //Reinitiate Sequence
                    var InBetweenLst = _entities.tblPPTSlides.Where(c => c.MasterDocumentName == originalFile && !(c.IsOriginal == true) && (c.Sequence > MyFileSeq)).OrderBy(a => a.Sequence).ToList();
                    int FirstEntryID = SplitCompletedSlides(AssignedSlide.OriginalFile, AssignedSlide.SlideName, LastSlide + 1, MyFileSeq, AssignedSlide.OriginalSlideName);

                    var newPptSlide = (from itm in _entities.tblAssignedPPTSlides
                                       where itm.AssignedPPTSildeID == AssignedSlide.AssignedPPTSlideID
                                       select itm).FirstOrDefault();

                    if (newPptSlide != null)
                    {
                        newPptSlide.IsPPTApproved = true;
                        newPptSlide.IsPublished = true;
                        _entities.Entry(newPptSlide).State = System.Data.Entity.EntityState.Modified;
                    }

                    _entities.SaveChanges();

                    // Merge PPTs to Master  Doc & Regenrate PPT file 
                    var allSlideNames = (from p in _entities.tblPPTSlides
                                         where p.MasterDocumentName == originalFile && !(p.IsOriginal == true)
                                         orderby p.Sequence
                                         select p.SlideName).ToList();


                    MergeFiles(allSlideNames, originalFile);

                    tblAssignmentLog _log = new tblAssignmentLog();
                    _log.Action = (int)Global.Action.Approved;
                    _log.AssignmentID = assignment.AssignmentID;
                    _log.Description = "Slide " + pptSlide.Sequence.ToString() + " - " + remarks;
                    _log.CreatedDate = DateTime.Now;
                    _log.CreatedDate = DateTime.Now;
                    _log.CreatedBy = CurrentUserSession.User.UserID;
                    _entities.tblAssignmentLogs.Add(_log);
                    _entities.SaveChanges();

                    var allSlides = _entities.tblAssignedPPTSlides.Where(sl => sl.AssignmentID == assignment.AssignmentID).ToList();

                    var publishedCount = allSlides.Where(sl => sl.IsPublished.HasValue && sl.IsPublished.Value).Count();
                    var approvedCount = allSlides.Where(sl => sl.IsPPTApproved.HasValue && sl.IsPPTApproved.Value && !sl.IsPublished.HasValue).Count();
                    var pendingApprovalCount = allSlides.Where(sl => !sl.IsPPTApproved.HasValue && sl.ReviewRequested.HasValue && sl.ReviewRequested.Value).Count();

                    bool taskfinished = false;
                    bool taskCompleted = false;
                    if (publishedCount + approvedCount == allSlides.Count)
                    {
                        taskfinished = true;
                    }
                    if (pendingApprovalCount + publishedCount == allSlides.Count)
                    {
                        taskCompleted = true;
                    }
                    string message = string.Empty;
                    string strEmailMessage = string.Empty;
                    string strEmailSubject = string.Empty;

                    if (taskfinished)
                    {
                        assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
                        assignment.CompletedDate = DateTime.Now;
                        _entities.SaveChanges();
                        strEmailMessage = "A task has been Finished by " + CurrentUserSession.User.FirstName;
                        strEmailSubject = "Task Completed: Task has been finished by " + CurrentUserSession.User.FirstName;
                        message = "1";
                    }
                    else if (taskCompleted)
                    {
                        assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                        assignment.CompletedDate = DateTime.Now;
                        _entities.SaveChanges();
                        strEmailMessage = "A task has been Completed by " + CurrentUserSession.User.FirstName;
                        strEmailSubject = "Task Completed: Task has been completed by " + CurrentUserSession.User.FirstName;
                        message = "1";
                    }
                    else
                    {
                        message = string.Empty;
                        strEmailMessage = "A slide has been finished by " + CurrentUserSession.User.FirstName;
                        strEmailSubject = "Slide Completed: Slide has been finished by " + CurrentUserSession.User.FirstName;
                        message = "2";
                    }

                    //Send email to approvers
                    var assignmentMembers = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == assignment.AssignmentID && am.CanApprove).Select(a => new AssignmentMemberModel()
                    {
                        UserID = (int)a.UserID,
                        CanApprove = a.CanApprove,
                        CanEdit = a.CanEdit
                    }).ToList();

                    var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                    AssignmentModel modelEmail = new AssignmentModel();
                    modelEmail.TaskName = assignment.TaskName;
                    modelEmail.Comments = assignment.Comments;
                    modelEmail.AssignID = assignment.AssignmentID;
                    List<AssignmentLogModel> logModels = new List<AssignmentLogModel>();
                    logModels = GetAssignmentLog(assignment.AssignmentID);
                    string EmailBody = templates.TemplateContentForEmail;
                    var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == assignment.CreatedBy).FirstOrDefault();
                    templates.Subject = templates.Subject.Replace("#Subject#", strEmailSubject);
                    EmailBody = PopulateEmailBody(strEmailMessage, UserToNotify.FullName, modelEmail, logModels, EmailBody, true);

                    byte[] fileBytes = CommonHelper.GeneratePDF(strEmailMessage, UserToNotify.FullName, modelEmail, logModels);

                    //NotifyMembersOfAssignment(assignmentMembers, modelEmail, assignment.AssignmentID);

                    objAvailabilityResult.Status = true;
                    objAvailabilityResult.ErrorCode = "";
                    objAvailabilityResult.ErrorMessage = message;
                }

            }
            return objAvailabilityResult.ErrorMessage;
        }
        public bool DeclinePPT(string document, int AssignID, string Remarks)
        {
            //Mark Task as Merged 
            var doc = (from itm in _entities.tblAssignments
                       where itm.AssignmentID == AssignID
                       select itm).FirstOrDefault();
            doc.Action = (int)WordAutomationDemo.Common.Global.Action.Declined;
            doc.CompletedDate = DateTime.Now;
            if (!string.IsNullOrEmpty(Remarks))
            {
                doc.Comments = Remarks;
            }
            _entities.Entry(doc).State = System.Data.Entity.EntityState.Modified;

            var slides = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == AssignID && !(z.IsPPTApproved.HasValue && z.IsPPTApproved.Value == true));
            if (slides != null && slides.Count() > 0)
            {
                foreach (var slide in slides)
                {
                    slide.IsPPTModified = null;
                    slide.IsPPTApproved = null;
                    slide.IsGrayedOut = null;
                    _entities.Entry(slide).State = System.Data.Entity.EntityState.Modified;
                }
                _entities.SaveChanges();
            }

            //Add Log 
            tblAssignmentLog _log = new tblAssignmentLog();
            _log.Action = (int)WordAutomationDemo.Common.Global.Action.Declined;
            _log.AssignmentID = doc.AssignmentID;
            _log.CreatedDate = DateTime.Now;
            _log.CreatedBy = CurrentUserSession.UserID;
            _log.Description = Remarks;
            _log.DocumentName = document;
            _entities.tblAssignmentLogs.Add(_log);
            _entities.SaveChanges();
            return true;
        }


        public bool ApprovePPT(string document, int AssignID, string Remarks)
        {
            using (ReadyPortalDBEntities _readyPortalDBEntities = new ReadyPortalDBEntities())
            {
                var AssignedSlides = (from items in _readyPortalDBEntities.tblAssignedPPTSlides.AsEnumerable()
                                      join q in _readyPortalDBEntities.tblAssignments.AsEnumerable() on items.AssignmentID equals q.AssignmentID
                                      join docu in _readyPortalDBEntities.tblDocuments on q.DocumentID equals docu.DocumentID
                                      where items.AssignmentID == AssignID && items.IsPPTModified.HasValue && items.IsPPTModified.Value
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

                int LastEntryForAll = Convert.ToInt32(_readyPortalDBEntities.tblPPTSlides.Where(c => c.MasterDocumentName == document && !(c.IsOriginal == true)).OrderByDescending(c => c.Sequence).Select(c => c.PPTSlidesID).FirstOrDefault());

                foreach (var item in AssignedSlides)
                {
                    var LastFile = _readyPortalDBEntities.tblPPTSlides.Where(c => c.MasterDocumentName == document && !(c.IsOriginal == true)).OrderByDescending(c => c.PPTSlidesID).FirstOrDefault();

                    var MyOriFile = _readyPortalDBEntities.tblPPTSlides.Where(c => c.MasterDocumentName == document && c.SlideName == item.OriginalSlideName && !(c.IsOriginal == true)).FirstOrDefault();

                    int MyFileSeq = Convert.ToInt32(MyOriFile.Sequence);

                    int LastSlide = Convert.ToInt32(LastFile.SlideName.Split('.')[0]);

                    //Reinitiate Sequence
                    var InBetweenLst = _readyPortalDBEntities.tblPPTSlides.Where(c => c.MasterDocumentName == document && !(c.IsOriginal == true) && (c.Sequence > MyFileSeq)).OrderBy(a => a.Sequence).ToList();

                    int FirstEntryID = SplitCompletedSlides(item.OriginalFile, item.SlideName, LastSlide + 1, MyFileSeq, item.OriginalSlideName);

                    int GetLastSeq = Convert.ToInt32(_readyPortalDBEntities.tblPPTSlides.Where(c => c.MasterDocumentName == document && !(c.IsOriginal == true)).OrderByDescending(c => c.PPTSlidesID).Select(c => c.Sequence).FirstOrDefault());

                    foreach (var inbet in InBetweenLst)
                    {
                        GetLastSeq++;

                        inbet.Sequence = GetLastSeq;
                    }

                    var pptSlide = (from itm in _readyPortalDBEntities.tblAssignedPPTSlides
                                    where itm.AssignedPPTSildeID == item.AssignedPPTSlideID
                                    select itm).FirstOrDefault();

                    if (pptSlide != null)
                    {
                        pptSlide.IsPPTApproved = true;
                        _readyPortalDBEntities.Entry(pptSlide).State = System.Data.Entity.EntityState.Modified;
                    }

                    _readyPortalDBEntities.SaveChanges();
                }

                //Mark Task as Merged 
                var doc = (from itm in _readyPortalDBEntities.tblAssignments
                           where itm.AssignmentID == AssignID
                           select itm).FirstOrDefault();
                doc.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
                doc.Status = (int)Global.AssignmentStatus.Completed;
                doc.CompletedDate = DateTime.Now;
                if (!string.IsNullOrEmpty(Remarks))
                {
                    doc.Remarks = Remarks;
                    doc.Comments = Remarks;
                }
                _readyPortalDBEntities.Entry(doc).State = System.Data.Entity.EntityState.Modified;

                //Add Log 
                tblAssignmentLog _log = new tblAssignmentLog();
                _log.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
                _log.AssignmentID = doc.AssignmentID;
                _log.CreatedDate = DateTime.Now;
                _log.CreatedBy = CurrentUserSession.UserID;
                _log.Description = Remarks;
                _log.DocumentName = document;
                _readyPortalDBEntities.tblAssignmentLogs.Add(_log);
                _readyPortalDBEntities.SaveChanges();

                // Merge PPTs to Master  Doc & Regenrate PPT file 
                var allSlides = (from p in _readyPortalDBEntities.tblPPTSlides
                                 where p.MasterDocumentName == document && !(p.IsOriginal == true)
                                 orderby p.Sequence
                                 select p.SlideName).ToList();


                MergeFiles(allSlides, document);
                return true;
            }
        }

        [HttpPost]
        public bool ApprovePage(int pageNumber, int assignmentID, string ticks)
        {
            var assignedPages = _entities.tblAssignedWordPages.Where(awp => awp.AssignmentID == assignmentID).ToList();

            var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == assignmentID);
            var document = _entities.tblDocuments.FirstOrDefault(d => d.DocumentID == assignment.DocumentID);
            var docPath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, "Pages_" + document.DocumentName, "changedDocs");
            tblAssignedWordPage assignedPage;
            string filename = "";
            if (ticks != null && !String.IsNullOrEmpty(ticks))
            {
                assignedPage = assignedPages.FirstOrDefault(ap => ap.Ticks == ticks);
                filename = new DirectoryInfo(docPath).GetFiles(assignmentID.ToString() + "-" + ticks + "-" + pageNumber.ToString() + ".docx")[0].Name;
            }
            else
            {
                assignedPage = assignedPages.FirstOrDefault(ap => ap.Sequence == pageNumber);
                filename = new DirectoryInfo(docPath).GetFiles(assignmentID.ToString() + "-*-" + pageNumber.ToString() + ".docx")[0].Name;
            }
            //var assignedPage = assignedPages.FirstOrDefault(ap => ap.Sequence == pageNumber);


            //var ticks = filename.Split('-')[1];

            //Update original doc
            if (!UpdateTask(filename, ((int)Global.Action.Approved).ToString(), new RichEditDocumentServer(),
                "", "", -1, assignmentID, true))
            {
                return false;
            }

            assignedPage.IsPublished = true;

            tblAssignmentLog _log = new tblAssignmentLog();
            _log.Action = (int)Global.Action.Approved;
            _log.AssignmentID = assignment.AssignmentID;
            _log.CreatedDate = DateTime.Now;
            _log.CreatedDate = DateTime.Now;
            _log.Description = "";
            _log.CreatedBy = CurrentUserSession.User.UserID;
            _entities.tblAssignmentLogs.Add(_log);
            _entities.SaveChanges();

            var allPages = _entities.tblAssignedWordPages.Where(sl => sl.AssignmentID == assignment.AssignmentID).ToList();

            var publishedCount = allPages.Where(sl => sl.IsPublished.HasValue && sl.IsPublished.Value).Count();
            var pendingApprovalCount = allPages.Where(sl => !sl.IsPublished.HasValue && sl.ReviewRequested.HasValue && sl.ReviewRequested.Value).Count();

            bool taskfinished = false;
            bool taskCompleted = false;
            if (publishedCount == allPages.Count)
            {
                taskfinished = true;
            }
            if (pendingApprovalCount + publishedCount == allPages.Count && pendingApprovalCount > 0)
            {
                taskCompleted = true;
            }

            string strEmailMessage = string.Empty;
            string strEmailSubject = string.Empty;

            if (taskfinished)
            {
                assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
                assignment.CompletedDate = DateTime.Now;
                _entities.SaveChanges();
                strEmailMessage = "A task has been Finished by " + CurrentUserSession.User.FirstName;
                strEmailSubject = "Task Completed: Task has been finished by " + CurrentUserSession.User.FirstName;
            }
            else if (taskCompleted)
            {
                assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                assignment.CompletedDate = DateTime.Now;
                _entities.SaveChanges();
                strEmailMessage = "A task has been Completed by " + CurrentUserSession.User.FirstName;
                strEmailSubject = "Task Completed: Task has been completed by " + CurrentUserSession.User.FirstName;
            }
            else
            {
                strEmailMessage = "A page has been finished by " + CurrentUserSession.User.FirstName;
                strEmailSubject = "Page Completed: Page has been finished by " + CurrentUserSession.User.FirstName;
            }

            //Send email to approvers
            var assignmentMembers = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == assignment.AssignmentID && am.CanApprove).Select(a => new AssignmentMemberModel()
            {
                UserID = (int)a.UserID,
                CanApprove = a.CanApprove,
                CanEdit = a.CanEdit
            }).ToList();

            var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

            AssignmentModel modelEmail = new AssignmentModel();
            modelEmail.TaskName = assignment.TaskName;
            modelEmail.Comments = assignment.Comments;
            modelEmail.AssignID = assignment.AssignmentID;
            List<AssignmentLogModel> logModels = new List<AssignmentLogModel>();
            logModels = GetAssignmentLog(assignment.AssignmentID);
            string EmailBody = templates.TemplateContentForEmail;
            var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == assignment.CreatedBy).FirstOrDefault();
            templates.Subject = templates.Subject.Replace("#Subject#", strEmailSubject);
            EmailBody = PopulateEmailBody(strEmailMessage, UserToNotify.FullName, modelEmail, logModels, EmailBody, true);

            byte[] fileBytes = CommonHelper.GeneratePDF(strEmailMessage, UserToNotify.FullName, modelEmail, logModels);

            //NotifyMembersOfAssignment(assignmentMembers, modelEmail, assignment.AssignmentID);
            return true;
        }

        [HttpPost]
        public bool ApproveSlide(int assignedPPTSlideID, int assignID, string docName)
        {
            using (ReadyPortalDBEntities _readyPortalDBEntities = new ReadyPortalDBEntities())
            {
                var AssignedSlides = (from items in _readyPortalDBEntities.tblAssignedPPTSlides.AsEnumerable()
                                      join q in _readyPortalDBEntities.tblAssignments.AsEnumerable() on items.AssignmentID equals q.AssignmentID
                                      join docu in _readyPortalDBEntities.tblDocuments on q.DocumentID equals docu.DocumentID
                                      where items.AssignmentID == assignID && items.IsPPTModified.HasValue && items.IsPPTModified.Value && items.AssignedPPTSildeID == assignedPPTSlideID
                                      orderby items.Sequence
                                      select new PPTModel()
                                      {
                                          AssignedTaskID = items.AssignmentID,
                                          OriginalFile = docu.DocumentName,
                                          OriginalSlideName = items.SlideName,
                                          SlideName = "New_" + items.AssignmentID.ToString() + "_Copy_" + items.SlideName
                                      }).FirstOrDefault();

                int LastEntryForAll = Convert.ToInt32(_readyPortalDBEntities.tblPPTSlides.Where(c => c.MasterDocumentName == docName && !(c.IsOriginal == true)).OrderByDescending(c => c.Sequence).Select(c => c.PPTSlidesID).FirstOrDefault());

                var LastFile = _readyPortalDBEntities.tblPPTSlides.Where(c => c.MasterDocumentName == docName && !(c.IsOriginal == true)).OrderByDescending(c => c.PPTSlidesID).FirstOrDefault();

                var MyOriFile = _readyPortalDBEntities.tblPPTSlides.Where(c => c.MasterDocumentName == docName && c.SlideName == AssignedSlides.OriginalSlideName && !(c.IsOriginal == true)).FirstOrDefault();

                int MyFileSeq = Convert.ToInt32(MyOriFile.Sequence);

                int LastSlide = Convert.ToInt32(LastFile.SlideName.Split('.')[0]);

                //Reinitiate Sequence
                var InBetweenLst = _readyPortalDBEntities.tblPPTSlides.Where(c => c.MasterDocumentName == docName && !(c.IsOriginal == true) && (c.Sequence > MyFileSeq)).OrderBy(a => a.Sequence).ToList();

                int FirstEntryID = SplitCompletedSlides(AssignedSlides.OriginalFile, AssignedSlides.SlideName, LastSlide + 1, MyFileSeq, AssignedSlides.OriginalSlideName);

                int GetLastSeq = Convert.ToInt32(_readyPortalDBEntities.tblPPTSlides.Where(c => c.MasterDocumentName == docName && !(c.IsOriginal == true)).OrderByDescending(c => c.PPTSlidesID).Select(c => c.Sequence).FirstOrDefault());

                foreach (var inbet in InBetweenLst)
                {
                    GetLastSeq++;

                    inbet.Sequence = GetLastSeq;
                }
                _readyPortalDBEntities.SaveChanges();


                var ppt = (from itm in _readyPortalDBEntities.tblAssignedPPTSlides
                           where itm.AssignedPPTSildeID == assignedPPTSlideID
                           select itm).FirstOrDefault();

                if (ppt != null)
                {
                    ppt.IsPPTApproved = true;
                    _readyPortalDBEntities.Entry(ppt).State = System.Data.Entity.EntityState.Modified;
                    _readyPortalDBEntities.SaveChanges();
                }

                // Merge PPTs to Master  Doc & Regenrate PPT file 
                var allSlides = (from p in _readyPortalDBEntities.tblPPTSlides
                                 where p.MasterDocumentName == docName && !(p.IsOriginal == true)
                                 orderby p.Sequence
                                 select p.SlideName).ToList();

                MergeFiles(allSlides, docName);

                ////Mark Task as Merged 
                var doc = (from itm in _readyPortalDBEntities.tblAssignments
                           where itm.AssignmentID == assignID
                           select itm).FirstOrDefault();
                //doc.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
                //doc.CompletedDate = DateTime.Now;
                //if (!string.IsNullOrEmpty(Remarks))
                //{
                //    doc.Remarks = Remarks;
                //    doc.Comments = Remarks;
                //}
                //_readyPortalDBEntities.Entry(doc).State = System.Data.Entity.EntityState.Modified;

                var modifiedSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == assignID && z.IsPPTApproved.HasValue && z.IsPPTApproved.Value == true).Count();
                var totalSlidesCount = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == assignID).Count();
                if (modifiedSlidesCount == totalSlidesCount)
                {
                    doc.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
                    doc.CompletedDate = DateTime.Now;
                    _readyPortalDBEntities.Entry(doc).State = System.Data.Entity.EntityState.Modified;
                    _entities.SaveChanges();
                }

                //Add Log 
                tblAssignmentLog _log = new tblAssignmentLog();
                _log.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
                _log.AssignmentID = doc.AssignmentID;
                _log.CreatedDate = DateTime.Now;
                _log.CreatedBy = CurrentUserSession.UserID;
                _log.Description = AssignedSlides != null && AssignedSlides.OriginalSlideName != null ? AssignedSlides.OriginalSlideName + " has been approved!" : "";
                _log.DocumentName = docName;
                _readyPortalDBEntities.tblAssignmentLogs.Add(_log);
                _readyPortalDBEntities.SaveChanges();

                var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                objAssignmentModelEmail.TaskName = doc.TaskName;
                objAssignmentModelEmail.Comments = doc.Comments;
                objAssignmentModelEmail.AssignID = doc.AssignmentID;
                List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                lstAssignmentLogModel = GetAssignmentLog(doc.AssignmentID);
                string EmailBody = templates.TemplateContentForEmail;
                var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == doc.LockedByUserID).FirstOrDefault();
                templates.Subject = templates.Subject.Replace("#Subject#", "Slide Approved: Slide has been approved by " + CurrentUserSession.User.FirstName);
                EmailBody = PopulateEmailBody("A slide has been approved by " + CurrentUserSession.User.FirstName, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody);

                byte[] fileBytes = CommonHelper.GeneratePDF("A slide has been approved by " + CurrentUserSession.User.FirstName, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel);

                Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody, fileBytes);

                return true;
            }
        }

        public bool DeclineSlide(int assignedPPTSlideID, int assignID, string docName, string remarks)
        {
            using (ReadyPortalDBEntities _readyPortalDBEntities = new ReadyPortalDBEntities())
            {
                var ppt = (from itm in _readyPortalDBEntities.tblAssignedPPTSlides
                           where itm.AssignedPPTSildeID == assignedPPTSlideID
                           select itm).FirstOrDefault();

                if (ppt != null)
                {
                    ppt.IsPPTModified = null;
                    ppt.IsPPTApproved = null;
                    ppt.IsGrayedOut = null;
                    ppt.PPTRemarks = remarks;
                    _readyPortalDBEntities.Entry(ppt).State = System.Data.Entity.EntityState.Modified;
                    _readyPortalDBEntities.SaveChanges();
                }

                //Mark Task as Merged 
                var doc = (from itm in _entities.tblAssignments
                           where itm.AssignmentID == assignID
                           select itm).FirstOrDefault();
                //doc.Action = (int)WordAutomationDemo.Common.Global.Action.Declined;
                //doc.CompletedDate = DateTime.Now;
                //if (!string.IsNullOrEmpty(Remarks))
                //{
                //    doc.Comments = Remarks;
                //}
                //_entities.Entry(doc).State = System.Data.Entity.EntityState.Modified;

                //Add Log 
                tblAssignmentLog _log = new tblAssignmentLog();
                _log.Action = (int)WordAutomationDemo.Common.Global.Action.Declined;
                _log.AssignmentID = assignID;
                _log.CreatedDate = DateTime.Now;
                _log.CreatedBy = CurrentUserSession.UserID;
                _log.Description = remarks;
                _log.DocumentName = docName;
                _entities.tblAssignmentLogs.Add(_log);
                _entities.SaveChanges();

                var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                objAssignmentModelEmail.TaskName = doc.TaskName;
                objAssignmentModelEmail.Comments = doc.Comments;
                objAssignmentModelEmail.AssignID = doc.AssignmentID;
                List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                lstAssignmentLogModel = GetAssignmentLog(doc.AssignmentID);
                string EmailBody = templates.TemplateContentForEmail;
                var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == doc.LockedByUserID).FirstOrDefault();
                templates.Subject = templates.Subject.Replace("#Subject#", "Slide Declined: Slide has been declined by " + CurrentUserSession.User.FirstName);
                EmailBody = PopulateEmailBody("A slide has been declined by " + CurrentUserSession.User.FirstName, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody);

                byte[] fileBytes = CommonHelper.GeneratePDF("A slide has been declined by " + CurrentUserSession.User.FirstName, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel);

                Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody, fileBytes);

                return true;
            }
        }

        public bool ReassignTask(string document, string DeclineReason = "", int AssignID = 0, string assignedTo = "", DateTime? dueDate = null)
        {
            using (var _entities = new ReadyPortalDBEntities())
            {
                var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == AssignID);
                assignment.CompletedDate = null;
                _entities.Entry(assignment).State = EntityState.Modified;
                assignment.Action = (int)Global.Action.Assigned;

                tblAssignmentLog _log = new tblAssignmentLog();
                _log.Action = (int)WordAutomationDemo.Common.Global.Action.Reassign;
                _log.AssignmentID = AssignID;
                _log.CreatedBy = CurrentUserSession.UserID;
                _log.CreatedDate = DateTime.Now;
                assignment.LockedByUserID = null;
                assignment.DueDate = dueDate;
                _entities.tblAssignmentLogs.Add(_log);
                _entities.SaveChanges();
                _entities.Entry(assignment).State = System.Data.Entity.EntityState.Modified;

                //Remove existing assignment members
                var currentMembers = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == AssignID);
                _entities.tblAssignmentMembers.RemoveRange(currentMembers);
                _entities.SaveChanges();
                //Create new assignment members
                var assignedMembers = JsonConvert.DeserializeObject<List<AssignmentMemberModel>>(assignedTo);
                var newMembers = new List<tblAssignmentMember>();
                foreach (var member in assignedMembers)
                {
                    var newMember = new tblAssignmentMember()
                    {
                        UserID = member.UserID,
                        CanEdit = member.CanEdit,
                        CanApprove = member.CanApprove,
                        CanPublish = member.CanPublish,
                        AssignmentID = AssignID
                    };
                    newMembers.Add(newMember);
                }
                _entities.BulkInsert(newMembers);
                _entities.SaveChanges();

                return true;
            }
        }

        public bool ReassignTaskPPT(string document, string DeclineReason = "", int AssignID = 0, string assignedTo = "", DateTime? dueDate = null)
        {
            using (var _entities = new ReadyPortalDBEntities())
            {
                var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == AssignID);
                assignment.CompletedDate = null;
                _entities.Entry(assignment).State = EntityState.Modified;
                assignment.Action = (int)Global.Action.Assigned;

                tblAssignmentLog _log = new tblAssignmentLog();
                _log.Action = (int)WordAutomationDemo.Common.Global.Action.Reassign;
                assignment.LockedByUserID = null;
                assignment.DueDate = dueDate;
                _entities.SaveChanges();
                _entities.Entry(assignment).State = System.Data.Entity.EntityState.Modified;

                //Remove existing assignment members
                var currentMembers = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == AssignID);
                _entities.tblAssignmentMembers.RemoveRange(currentMembers);
                _entities.SaveChanges();
                //Create new assignment members
                var assignedMembers = JsonConvert.DeserializeObject<List<AssignmentMemberModel>>(assignedTo);
                var newMembers = new List<tblAssignmentMember>();
                foreach (var member in assignedMembers)
                {
                    var newMember = new tblAssignmentMember()
                    {
                        UserID = member.UserID,
                        CanEdit = member.CanEdit,
                        CanApprove = member.CanApprove,
                        CanPublish = member.CanPublish,
                        AssignmentID = AssignID
                    };
                    newMembers.Add(newMember);
                }
                _entities.BulkInsert(newMembers);

                var slides = _entities.tblAssignedPPTSlides.Where(sl => sl.AssignmentID == AssignID).ToList();

                foreach (var slide in slides)
                {
                    //slide.PPTRemarks = "";
                    slide.IsPPTApproved = null;
                    slide.IsGrayedOut = null;
                    slide.ReviewRequested = null;
                    slide.IsPublished = null;
                }

                _entities.SaveChanges();

                return true;
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CancelTask(int AssignmentID, string OriginalFilename, int DocType)
        {
            try
            {
                var docPath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, "Pages_" + OriginalFilename, "assignedDocs");

                //Revert any changes on the updated doc
                var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == AssignmentID);
                switch (DocType)
                {
                    case (int)Global.DocumentType.Ppt:
                        _entities.tblAssignedPPTSlides.RemoveRange(_entities.tblAssignedPPTSlides
                            .Where(aps => aps.AssignmentID == AssignmentID).ToList());
                        break;
                    case (int)Global.DocumentType.Xls:
                        _entities.tblExcelRowMaps.RemoveRange(_entities.tblExcelRowMaps
                            .Where(erm => erm.AssignmentID == AssignmentID).ToList());
                        break;
                    case (int)Global.DocumentType.Word:
                        var pageAssignments = _entities.tblAssignedWordPages.Where(awp => awp.AssignmentID == AssignmentID)
                            .ToList();

                        if (pageAssignments.Count > 0)
                        {
                            int cnt = 0;
                            foreach (var pageAssignment in pageAssignments)
                            {
                                var dirFiles = new DirectoryInfo(docPath).GetFiles(AssignmentID.ToString() + "-*-" + pageAssignment.Sequence + ".docx");
                                var filename = String.Empty;
                                if (!String.IsNullOrEmpty(pageAssignment.Ticks))
                                {
                                    filename = AssignmentID.ToString() + "-" + pageAssignment.Ticks + "-" + pageAssignment.Sequence + ".docx";
                                }
                                else
                                {
                                    filename = dirFiles[cnt].Name;
                                }
                                cnt++;

                                RevertWordTask(filename, new RichEditDocumentServer(), "", AssignmentID, true);
                                _entities.tblAssignedWordPages.Remove(pageAssignment);
                            }
                        }
                        else
                        {
                            RevertWordTask(assignment.DocumentFile, new RichEditDocumentServer(), "", AssignmentID, false);
                        }
                        break;
                    default:
                        break;
                };

                //Remove assignment members
                var assignmentMembers = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == AssignmentID).ToList();
                _entities.tblAssignmentMembers.RemoveRange(assignmentMembers);

                //Remove assignment logs
                _entities.tblAssignmentLogs.RemoveRange(_entities.tblAssignmentLogs
                    .Where(al => al.AssignmentID == AssignmentID).ToList());

                //Remove the assignment
                _entities.tblAssignments.Remove(assignment);

                //Audit trail
                tblLogActivity _logActivity = new tblLogActivity();
                _logActivity.ActivityDetails = String.Format("User <{0}> has cancelled task <{1}>.", CurrentUserSession.User.UserName, AssignmentID.ToString());
                _logActivity.LogActivityTypeID = (int)Global.LogActivityTypes.TaskDeleted;
                _logActivity.CreatedBy = CurrentUserSession.User.UserID;
                _logActivity.CreatedDate = DateTime.Now;
                _logActivity.IPAddress = HttpContext.Request.UserHostAddress;

                _entities.tblLogActivities.Add(_logActivity);
                _entities.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public bool RevertWordTask(string document, RichEditDocumentServer server, string Remarks = "", int AssignID = 0, bool isPage = false)
        {
            if (!string.IsNullOrEmpty(document))
            {
                if (AssignID > 0)
                {
                    var assignment = (from itm in _entities.tblAssignments
                                      join docu in _entities.tblDocuments on itm.DocumentID equals docu.DocumentID
                                      where itm.AssignmentID == AssignID
                                      select itm).FirstOrDefault();
                    var docName = _entities.tblDocuments.FirstOrDefault(d => d.DocumentID == assignment.DocumentID).DocumentName;

                    if (isPage)
                    {
                        server.Document.InsertDocumentContent(server.Document.Range.Start, DirectoryManagmentUtils.InitialDemoFilesPathWords + "/Pages_" + docName + "/assignedDocs/" + document, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    }
                    else
                    {
                        server.Document.InsertDocumentContent(server.Document.Range.Start, DirectoryManagmentUtils.InitialDemoFilesPath + "/Copy" + document, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Snippet to the temp document
                    }
                    DevExpress.XtraRichEdit.API.Native.Document newdocuments = server.Document;

                    CharacterProperties cp = server.Document.BeginUpdateCharacters(server.Document.Range);
                    cp.ForeColor = System.Drawing.Color.Black; // convert all text to black color
                    server.Document.EndUpdateCharacters(cp);

                    HtmlDocumentExporterOptions htm = new HtmlDocumentExporterOptions();
                    htm.CssPropertiesExportType = DevExpress.XtraRichEdit.Export.Html.CssPropertiesExportType.Inline;
                    var HTML = server.Document.GetHtmlText(server.Document.Range, null, htm);

                    HtmlDocument docs = new HtmlDocument();
                    docs.LoadHtml(HTML);
                    foreach (HtmlNode link in docs.DocumentNode.SelectNodes("//span[@style]"))
                    {
                        HtmlAttribute att = link.Attributes["style"];
                        if (att.Value.Contains("text-decoration: line-through;"))
                        {
                            link.Remove();
                        }
                    }

                    var textToReplace = docs.DocumentNode.WriteTo();
                    DocumentManager.CloseAllDocuments();
                    server.Dispose();
                    var ReplacementCode = "";
                    if (isPage)
                    {
                        ReplacementCode = "#" + document.Split('-')[1] + "#";
                    }
                    else
                    {
                        ReplacementCode = assignment.ReplacementCode;
                    }

                    server = new RichEditDocumentServer();
                    server.LoadDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + assignment.tblDocument.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    DevExpress.XtraRichEdit.API.Native.Document documents = server.Document;
                    //DocumentRange ranges = server.Document.FindAll(doc.ReplacementCode, SearchOptions.WholeWord).FirstOrDefault();
                    var CodeToFindWithSpace = Characters.Space.ToString() + "#S" + ReplacementCode + Characters.Space.ToString();
                    DocumentRange rangesStart = server.Document.FindAll(CodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                    var EndCodeToFindWithSpace = Characters.Space.ToString() + ReplacementCode + "E#" + Characters.Space.ToString();
                    DocumentRange rangesEnd = server.Document.FindAll(EndCodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                    DocumentRange ranges = null;
                    if (rangesStart != null && rangesEnd != null)
                    {
                        var totalLength = rangesEnd.End.ToInt() - rangesStart.Start.ToInt();
                        ranges = server.Document.CreateRange(rangesStart.Start, totalLength);
                    }
                    if (ranges != null)
                    {
                        //server.Document.ReplaceAll(doc.ReplacementCode, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord); // remove timestamp
                        server.Document.Replace(ranges, ""); // remove timestamp
                        server.Document.InsertHtmlText(ranges.Start, textToReplace, InsertOptions.MatchDestinationFormatting); // insert Snippet to the marker position
                        server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + assignment.tblDocument.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // save the doc 
                    }
                    else
                    {


                        var CodeToFindWithNoSpace = "#S" + ReplacementCode;
                        DocumentRange NoSpacerangesStart = server.Document.FindAll(CodeToFindWithNoSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                        var EndCodeToFindWithNoSpace = ReplacementCode + "E#";
                        DocumentRange NoSpacerangesEnd = server.Document.FindAll(EndCodeToFindWithNoSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                        DocumentRange rangesNoSpace = null;
                        if (NoSpacerangesStart != null && NoSpacerangesEnd != null)
                        {
                            var totalLengthNoSpace = NoSpacerangesEnd.End.ToInt() - NoSpacerangesStart.Start.ToInt();
                            rangesNoSpace = server.Document.CreateRange(NoSpacerangesStart.Start, totalLengthNoSpace);
                        }
                        if (rangesNoSpace != null)
                        {
                            //server.Document.ReplaceAll(doc.ReplacementCode, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord); // remove timestamp
                            server.Document.Replace(rangesNoSpace, ""); // remove timestamp
                            server.Document.InsertHtmlText(rangesNoSpace.Start, textToReplace, InsertOptions.MatchDestinationFormatting); // insert Snippet to the marker position
                            server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + assignment.tblDocument.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // save the doc 
                        }
                    }

                    DocumentManager.CloseAllDocuments();
                    server.Dispose();


                    //Update Master Copy

                    //Make a Temp Document
                    server = new RichEditDocumentServer();
                    server.CreateNewDocument();
                    server.Document.InsertDocumentContent(server.Document.Range.Start, DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + assignment.tblDocument.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Orignal Document to the temp document

                    var ListofAssignments = GetPendingAssignedTasks(assignment.tblDocument.DocumentName);
                    if (ListofAssignments != null && ListofAssignments.Count > 0)
                    {
                        foreach (var item in ListofAssignments)
                        {
                            //var CodeToFindWithSpace2 = Characters.Space.ToString() + item.ReplacementCode + Characters.Space.ToString();
                            //DocumentRange ranges2 = server.Document.FindAll(item.ReplacementCode, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();



                            var CodeToFindWithSpace2 = Characters.Space.ToString() + "#S" + ReplacementCode + Characters.Space.ToString();
                            DocumentRange rangesStart2 = server.Document.FindAll(CodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                            var EndCodeToFindWithSpace2 = Characters.Space.ToString() + ReplacementCode + "E#" + Characters.Space.ToString();
                            DocumentRange rangesEnd2 = server.Document.FindAll(EndCodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                            DocumentRange ranges2 = null;
                            if (rangesStart2 != null && rangesEnd2 != null)
                            {
                                var totalLength2 = rangesEnd2.End.ToInt() - rangesStart2.Start.ToInt();
                                ranges2 = server.Document.CreateRange(rangesStart2.Start, totalLength2);
                            }

                            if (ranges2 != null)
                            {
                                //server.Document.ReplaceAll(item.ReplacementCode, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord); // remove timestamp
                                server.Document.Replace(ranges2, ""); // remove timestamp
                                server.Document.InsertDocumentContent(ranges2.Start, DirectoryManagmentUtils.InitialDemoFilesPath + "/" + "Copy_" + item.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Snippet to the temp document
                            }
                            else
                            {

                                var CodeToFindWithNoSpace2 = "#S" + ReplacementCode;
                                DocumentRange NoSpacerangesStart2 = server.Document.FindAll(CodeToFindWithNoSpace2, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                                var EndCodeToFindWithNoSpace2 = ReplacementCode + "E#";
                                DocumentRange NoSpacerangesEnd2 = server.Document.FindAll(EndCodeToFindWithNoSpace2, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                                DocumentRange rangesNoSpace2 = null;
                                if (NoSpacerangesStart2 != null && NoSpacerangesEnd2 != null)
                                {
                                    var totalLengthNoSpace2 = NoSpacerangesEnd2.End.ToInt() - NoSpacerangesStart2.Start.ToInt();
                                    rangesNoSpace2 = server.Document.CreateRange(NoSpacerangesStart2.Start, totalLengthNoSpace2);
                                }

                                if (rangesNoSpace2 != null)
                                {
                                    //server.Document.ReplaceAll(item.ReplacementCode, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord); // remove timestamp
                                    server.Document.Replace(rangesNoSpace2, ""); // remove timestamp
                                    server.Document.InsertDocumentContent(rangesNoSpace2.Start, DirectoryManagmentUtils.InitialDemoFilesPath + "/" + "Copy_" + item.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Snippet to the temp document
                                }
                            }

                        }
                    }
                    var FinalContent = server.Document.HtmlText;
                    server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + assignment.tblDocument.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // Saves with same name causes the replacement
                    DocumentManager.CloseAllDocuments();
                    server.Dispose();

                    return true;
                }
            }

            return false;
        }

        public bool UpdateTask(string document, string actionId, RichEditDocumentServer server, string Remarks = "", string DeclineReason = "", int DocType = -1, int AssignID = 0, bool isPage = false, bool IsReassign = false, int assignedTo = 0, DateTime? dueDate = null)
        {
            if (!string.IsNullOrEmpty(document))
            {
                ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
                tblAssignment doc = new tblAssignment();
                string docName = String.Empty;
                if (AssignID > 0)
                {
                    doc = (from itm in _entities.tblAssignments
                           join docu in _entities.tblDocuments on itm.DocumentID equals docu.DocumentID
                           where itm.AssignmentID == AssignID
                           select itm).FirstOrDefault();
                    docName = _entities.tblDocuments.FirstOrDefault(d => d.DocumentID == doc.DocumentID).DocumentName;
                }
                else
                {
                    doc = (from itm in _entities.tblAssignments
                           join docu in _entities.tblDocuments on itm.DocumentID equals docu.DocumentID
                           where docu.DocumentName == document
                           select itm).FirstOrDefault();
                }
                //if (IsReassign && doc.Action == 1)
                //{
                //    return false;
                //}

                if (!string.IsNullOrEmpty(Remarks))
                {
                    doc.Remarks = Remarks;
                }

                if (actionId == Convert.ToString((int)Global.Action.Completed) || actionId == Convert.ToString((int)Global.Action.Approved) || actionId == Convert.ToString((int)Global.Action.Published))
                    doc.CompletedDate = DateTime.Now;
                _entities.Entry(doc).State = System.Data.Entity.EntityState.Modified;

                if (int.Parse(actionId) == (int)Global.Action.Approved || int.Parse(actionId) == (int)Global.Action.Published)
                {
                    #region Approved

                    // extract text from snippet
                    server.CreateNewDocument(); // create new temp document virtually

                    if (isPage)
                    {
                        server.Document.InsertDocumentContent(server.Document.Range.Start, DirectoryManagmentUtils.InitialDemoFilesPathWords + "/Pages_" + docName + "/changedDocs/" + document, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    }
                    else
                    {
                        server.Document.InsertDocumentContent(server.Document.Range.Start, DirectoryManagmentUtils.InitialDemoFilesPath + "/" + document, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Snippet to the temp document
                    }
                    DevExpress.XtraRichEdit.API.Native.Document newdocuments = server.Document;

                    CharacterProperties cp = server.Document.BeginUpdateCharacters(server.Document.Range);
                    cp.ForeColor = System.Drawing.Color.Black; // convert all text to black color
                    server.Document.EndUpdateCharacters(cp);

                    HtmlDocumentExporterOptions htm = new HtmlDocumentExporterOptions();
                    htm.CssPropertiesExportType = DevExpress.XtraRichEdit.Export.Html.CssPropertiesExportType.Inline;
                    var HTML = server.Document.GetHtmlText(server.Document.Range, null, htm);

                    HtmlDocument docs = new HtmlDocument();
                    docs.LoadHtml(HTML);
                    foreach (HtmlNode link in docs.DocumentNode.SelectNodes("//span[@style]"))
                    {
                        HtmlAttribute att = link.Attributes["style"];
                        if (att.Value.Contains("text-decoration: line-through;"))
                        {
                            link.Remove();
                        }
                    }

                    var textToReplace = docs.DocumentNode.WriteTo();
                    DocumentManager.CloseAllDocuments();
                    server.Dispose();
                    var ReplacementCode = "";
                    if (isPage)
                    {
                        ReplacementCode = "#" + document.Split('-')[1] + "#";
                    }
                    else
                    {
                        ReplacementCode = doc.ReplacementCode;
                    }

                    server = new RichEditDocumentServer();
                    server.LoadDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + doc.tblDocument.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    DevExpress.XtraRichEdit.API.Native.Document documents = server.Document;
                    //DocumentRange ranges = server.Document.FindAll(doc.ReplacementCode, SearchOptions.WholeWord).FirstOrDefault();
                    var CodeToFindWithSpace = Characters.Space.ToString() + "#S" + ReplacementCode + Characters.Space.ToString();
                    DocumentRange rangesStart = server.Document.FindAll(CodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                    var EndCodeToFindWithSpace = Characters.Space.ToString() + ReplacementCode + "E#" + Characters.Space.ToString();
                    DocumentRange rangesEnd = server.Document.FindAll(EndCodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                    DocumentRange ranges = null;
                    if (rangesStart != null && rangesEnd != null)
                    {
                        var totalLength = rangesEnd.End.ToInt() - rangesStart.Start.ToInt();
                        ranges = server.Document.CreateRange(rangesStart.Start, totalLength);
                    }
                    if (ranges != null)
                    {
                        //server.Document.ReplaceAll(doc.ReplacementCode, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord); // remove timestamp
                        server.Document.Replace(ranges, ""); // remove timestamp
                        server.Document.InsertHtmlText(ranges.Start, textToReplace, InsertOptions.MatchDestinationFormatting); // insert Snippet to the marker position
                        server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + doc.tblDocument.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // save the doc 
                    }
                    else
                    {


                        var CodeToFindWithNoSpace = "#S" + ReplacementCode;
                        DocumentRange NoSpacerangesStart = server.Document.FindAll(CodeToFindWithNoSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                        var EndCodeToFindWithNoSpace = ReplacementCode + "E#";
                        DocumentRange NoSpacerangesEnd = server.Document.FindAll(EndCodeToFindWithNoSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                        DocumentRange rangesNoSpace = null;
                        if (NoSpacerangesStart != null && NoSpacerangesEnd != null)
                        {
                            var totalLengthNoSpace = NoSpacerangesEnd.End.ToInt() - NoSpacerangesStart.Start.ToInt();
                            rangesNoSpace = server.Document.CreateRange(NoSpacerangesStart.Start, totalLengthNoSpace);
                        }
                        if (rangesNoSpace != null)
                        {
                            //server.Document.ReplaceAll(doc.ReplacementCode, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord); // remove timestamp
                            server.Document.Replace(rangesNoSpace, ""); // remove timestamp
                            server.Document.InsertHtmlText(rangesNoSpace.Start, textToReplace, InsertOptions.MatchDestinationFormatting); // insert Snippet to the marker position
                            server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + doc.tblDocument.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // save the doc 
                        }
                    }

                    DocumentManager.CloseAllDocuments();
                    server.Dispose();


                    //Update Master Copy

                    //Make a Temp Document
                    server = new RichEditDocumentServer();
                    server.CreateNewDocument();
                    server.Document.InsertDocumentContent(server.Document.Range.Start, DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + doc.tblDocument.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Orignal Document to the temp document

                    var ListofAssignments = GetPendingAssignedTasks(doc.tblDocument.DocumentName);
                    if (ListofAssignments != null && ListofAssignments.Count > 0)
                    {
                        foreach (var item in ListofAssignments)
                        {
                            //var CodeToFindWithSpace2 = Characters.Space.ToString() + item.ReplacementCode + Characters.Space.ToString();
                            //DocumentRange ranges2 = server.Document.FindAll(item.ReplacementCode, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();



                            var CodeToFindWithSpace2 = Characters.Space.ToString() + "#S" + ReplacementCode + Characters.Space.ToString();
                            DocumentRange rangesStart2 = server.Document.FindAll(CodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                            var EndCodeToFindWithSpace2 = Characters.Space.ToString() + ReplacementCode + "E#" + Characters.Space.ToString();
                            DocumentRange rangesEnd2 = server.Document.FindAll(EndCodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                            DocumentRange ranges2 = null;
                            if (rangesStart2 != null && rangesEnd2 != null)
                            {
                                var totalLength2 = rangesEnd2.End.ToInt() - rangesStart2.Start.ToInt();
                                ranges2 = server.Document.CreateRange(rangesStart2.Start, totalLength2);
                            }

                            if (ranges2 != null)
                            {
                                //server.Document.ReplaceAll(item.ReplacementCode, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord); // remove timestamp
                                server.Document.Replace(ranges2, ""); // remove timestamp
                                server.Document.InsertDocumentContent(ranges2.Start, DirectoryManagmentUtils.InitialDemoFilesPath + "/" + "Copy_" + item.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Snippet to the temp document
                            }
                            else
                            {

                                var CodeToFindWithNoSpace2 = "#S" + ReplacementCode;
                                DocumentRange NoSpacerangesStart2 = server.Document.FindAll(CodeToFindWithNoSpace2, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                                var EndCodeToFindWithNoSpace2 = ReplacementCode + "E#";
                                DocumentRange NoSpacerangesEnd2 = server.Document.FindAll(EndCodeToFindWithNoSpace2, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                                DocumentRange rangesNoSpace2 = null;
                                if (NoSpacerangesStart2 != null && NoSpacerangesEnd2 != null)
                                {
                                    var totalLengthNoSpace2 = NoSpacerangesEnd2.End.ToInt() - NoSpacerangesStart2.Start.ToInt();
                                    rangesNoSpace2 = server.Document.CreateRange(NoSpacerangesStart2.Start, totalLengthNoSpace2);
                                }

                                if (rangesNoSpace2 != null)
                                {
                                    //server.Document.ReplaceAll(item.ReplacementCode, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord); // remove timestamp
                                    server.Document.Replace(rangesNoSpace2, ""); // remove timestamp
                                    server.Document.InsertDocumentContent(rangesNoSpace2.Start, DirectoryManagmentUtils.InitialDemoFilesPath + "/" + "Copy_" + item.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Snippet to the temp document
                                }
                            }

                        }
                    }
                    var FinalContent = server.Document.HtmlText;
                    server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + doc.tblDocument.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // Saves with same name causes the replacement
                    DocumentManager.CloseAllDocuments();
                    server.Dispose();
                    #endregion

                }
                else if (int.Parse(actionId) == (int)WordAutomationDemo.Common.Global.Action.Declined)
                {
                    //#region Decline
                    //server = new RichEditDocumentServer();
                    //server.LoadDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + doc.OrginalSourceFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    //DevExpress.XtraRichEdit.API.Native.Document documents = server.Document;

                    ////var CodeToFindWithSpace = Characters.Space.ToString() + doc.ReplacementCode + Characters.Space.ToString();
                    ////DocumentRange ranges = server.Document.FindAll(CodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();


                    //var CodeToFindWithSpace = Characters.Space.ToString() + "#S" + doc.ReplacementCode + Characters.Space.ToString();
                    //DocumentRange rangesStart = server.Document.FindAll(CodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                    //var EndCodeToFindWithSpace = Characters.Space.ToString() + doc.ReplacementCode + "E#" + Characters.Space.ToString();
                    //DocumentRange rangesEnd = server.Document.FindAll(EndCodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                    //DocumentRange ranges = null;
                    //if (rangesStart != null && rangesEnd != null)
                    //{
                    //    var totalLength = rangesEnd.End.ToInt() - rangesStart.Start.ToInt();
                    //    ranges = server.Document.CreateRange(rangesStart.Start, totalLength);
                    //}

                    //if (ranges != null)
                    //{
                    //    server.Document.Replace(ranges, ""); // remove timestamp
                    //    server.Document.InsertDocumentContent(ranges.Start, DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + doc.DocumentFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert assigned copy to main doc
                    //    server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + doc.OrginalSourceFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // Saves with same name causes the replacement
                    //}
                    //else
                    //{
                    //    //DocumentRange rangesNoSpace = server.Document.FindAll(doc.ReplacementCode, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                    //    var CodeToFindWithNoSpace = "#S" + doc.ReplacementCode;
                    //    DocumentRange NoSpacerangesStart = server.Document.FindAll(CodeToFindWithNoSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                    //    var EndCodeToFindWithNoSpace = doc.ReplacementCode + "E#";
                    //    DocumentRange NoSpacerangesEnd = server.Document.FindAll(EndCodeToFindWithNoSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                    //    DocumentRange rangesNoSpace = null;
                    //    if (NoSpacerangesStart != null && NoSpacerangesEnd != null)
                    //    {
                    //        var totalLengthNoSpace = NoSpacerangesEnd.End.ToInt() - NoSpacerangesStart.Start.ToInt();
                    //        rangesNoSpace = server.Document.CreateRange(NoSpacerangesStart.Start, totalLengthNoSpace);
                    //    }
                    //    if (rangesNoSpace != null)
                    //    {
                    //        server.Document.Replace(rangesNoSpace, ""); // remove timestamp
                    //        server.Document.InsertDocumentContent(rangesNoSpace.Start, DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + doc.DocumentFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert assigned copy to main doc
                    //        server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + doc.OrginalSourceFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // Saves with same name causes the replacement
                    //    }
                    //}

                    //DocumentManager.CloseAllDocuments();
                    //server.Dispose();

                    //DocumentManager.CloseAllDocuments();
                    //server.Dispose();
                    //#endregion
                }

                tblAssignment docAssignment = new tblAssignment();

                if (AssignID > 0)
                {
                    docAssignment = (from itm in _entities.tblAssignments
                                     join docu in _entities.tblDocuments on itm.DocumentID equals docu.DocumentID
                                     where itm.AssignmentID == AssignID
                                     select itm).FirstOrDefault();
                }
                else
                {
                    docAssignment = (from itm in _entities.tblAssignments
                                     join docu in _entities.tblDocuments on itm.DocumentID equals docu.DocumentID
                                     where docu.DocumentName == document
                                     select itm).FirstOrDefault();
                }

                if (!isPage)
                {
                    docAssignment.Action = int.Parse(actionId);
                    doc.Status = (int)Global.AssignmentStatus.InProgress;
                    _entities.SaveChanges();
                }

                if (docAssignment.Action == (int)WordAutomationDemo.Common.Global.Action.Declined)
                {
                    if (!string.IsNullOrEmpty(DeclineReason))
                    {
                        docAssignment.Comments = DeclineReason;
                    }
                }

                if (docAssignment.Action != (int)WordAutomationDemo.Common.Global.Action.Approved && docAssignment.Action != (int)Global.Action.Published)
                {
                    doc.Status = (int)Global.AssignmentStatus.Completed;
                    if (!string.IsNullOrEmpty(Remarks))
                    {
                        docAssignment.Comments = Remarks;
                    }
                }
                _entities.Entry(docAssignment).State = System.Data.Entity.EntityState.Modified;

                //Add Log 



                if (!isPage)
                {
                    tblAssignmentLog _log = new tblAssignmentLog();

                    if (IsReassign)
                    {
                        _log.Action = (int)WordAutomationDemo.Common.Global.Action.Reassign;
                        docAssignment.LockedByUserID = assignedTo;
                        docAssignment.DueDate = dueDate;
                        _entities.Entry(docAssignment).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        _log.Action = int.Parse(actionId);
                    }
                    _log.AssignmentID = doc.AssignmentID;
                    _log.CreatedDate = DateTime.Now;
                    _log.CreatedBy = CurrentUserSession.UserID;
                    if (docAssignment.Action == (int)WordAutomationDemo.Common.Global.Action.Declined)
                        _log.Description = DeclineReason;
                    else
                        _log.Description = Remarks;

                    _log.DocumentName = document;
                    _entities.tblAssignmentLogs.Add(_log);
                    _entities.SaveChanges();

                    //if (IsReassign)
                    //{
                    //    var slides = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == doc.AssignmentID);
                    //    foreach (var slide in slides)
                    //    {
                    //        slide.IsPPTModified = null;
                    //        _entities.Entry(slide).State = System.Data.Entity.EntityState.Modified;
                    //    }
                    //    _entities.SaveChanges();
                    //}

                    var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                    AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                    objAssignmentModelEmail.TaskName = doc.TaskName;
                    objAssignmentModelEmail.AssignID = doc.AssignmentID;
                    List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                    lstAssignmentLogModel = GetAssignmentLog(doc.AssignmentID);
                    string EmailBody = templates.TemplateContentForEmail;
                    var userIDs = new List<int>();
                    if (actionId == Convert.ToString((int)WordAutomationDemo.Common.Global.Action.Completed))
                    {
                        userIDs.Add(doc.CreatedBy.Value);
                    }
                    else
                    {
                        userIDs.AddRange(doc.tblAssignment1.tblAssignmentMembers.Select(m => (int)m.UserID).ToList());
                    }
                    foreach (var userToNotify in _entities.tblUserDepartments.Where(x => userIDs.Contains(x.UserId)))
                    {
                        templates.Subject = templates.Subject.Replace("#Subject#", "Task " + (IsReassign ? Common.Global.GetEnumDescription(WordAutomationDemo.Common.Global.Action.Reassign) : Common.Global.GetEnumDescription((Common.Global.Action)doc.Action)) + ": Task has been " + (IsReassign ? Common.Global.GetEnumDescription(WordAutomationDemo.Common.Global.Action.Reassign) : Common.Global.GetEnumDescription((Common.Global.Action)doc.Action)) + " by " + CurrentUserSession.User.FirstName);
                        EmailBody = PopulateEmailBody("A task has been " + (IsReassign ? Common.Global.GetEnumDescription(WordAutomationDemo.Common.Global.Action.Reassign) : Common.Global.GetEnumDescription((Common.Global.Action)doc.Action)) + " by " + CurrentUserSession.User.FirstName, userToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody, (actionId == Convert.ToString((int)WordAutomationDemo.Common.Global.Action.Completed) ? true : false));
                        Global.SendEmail(userToNotify.EmailID, templates.Subject, EmailBody);
                    }
                }

                SplitPages(docName);
                //var Currentuser = _entities.tblUserDepartments.Where(x => x.UserId == CurrentUserSession.UserID).FirstOrDefault();
                //Task.Factory.StartNew(() =>
                //  {
                //      var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == doc.CreatedBy).FirstOrDefault();
                //      if (UserToNotify != null && !string.IsNullOrEmpty(UserToNotify.EmailID))
                //      {
                //          StringBuilder _sb = new StringBuilder();
                //          _sb.AppendLine("Hello " + UserToNotify.FullName + ",<br/><br/>");
                //          _sb.AppendLine(Currentuser.FullName + " has " + Common.Global.GetEnumDescription((Common.Global.Action)doc.Action) + " his task in Triyo. Please login to your account to check it.<br/><br/>");
                //          _sb.AppendLine("Regards,<br/><b>Triyo</b>");
                //          WordAutomationDemo.Common.Global.SendEmail(UserToNotify.EmailID, "Task " + Common.Global.GetEnumDescription((Common.Global.Action)doc.Action) + " by  " + Currentuser.FullName, _sb.ToString());
                //      }
                //  });

                return true;

            }
            else if (DocType == 0)
            {
                var doc = (from itm in _entities.tblAssignments
                           where itm.AssignmentID == AssignID
                           select itm).FirstOrDefault();

                if (IsReassign && doc.DocumentType.HasValue)
                {
                    if (CommonHelper.IsTaskReassigned(doc.AssignmentID, doc.DocumentType.Value))
                    {
                        return false;
                    }
                }
                else
                {
                    if (IsReassign && CommonHelper.IsTaskReassigned(doc.AssignmentID, 0))
                    {
                        return false;
                    }
                }

                doc.Action = int.Parse(actionId);

                if (!string.IsNullOrEmpty(Remarks))
                    doc.Remarks = Remarks;

                if (actionId == Convert.ToString((int)WordAutomationDemo.Common.Global.Action.Completed))
                    doc.CompletedDate = DateTime.Now;
                _entities.Entry(doc).State = System.Data.Entity.EntityState.Modified;

                if (doc.Action == (int)WordAutomationDemo.Common.Global.Action.Declined)
                {
                    if (!string.IsNullOrEmpty(DeclineReason))
                    {
                        doc.Comments = DeclineReason;
                    }
                }

                if (doc.Action != (int)WordAutomationDemo.Common.Global.Action.Approved && doc.Action != (int)Global.Action.Published)
                {
                    if (!string.IsNullOrEmpty(Remarks))
                    {
                        doc.Comments = Remarks;
                    }
                }
                _entities.Entry(doc).State = System.Data.Entity.EntityState.Modified;

                //Add Log 
                tblAssignmentLog _log = new tblAssignmentLog();
                if (IsReassign)
                {
                    _log.Action = (int)WordAutomationDemo.Common.Global.Action.Reassign;
                    doc.LockedByUserID = assignedTo;
                    doc.DueDate = dueDate;
                    _entities.Entry(doc).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    _log.Action = int.Parse(actionId);
                }
                _log.AssignmentID = doc.AssignmentID;
                _log.CreatedDate = DateTime.Now;
                _log.CreatedBy = CurrentUserSession.UserID;
                if (doc.Action == (int)WordAutomationDemo.Common.Global.Action.Declined)
                    _log.Description = DeclineReason;
                else
                    _log.Description = Remarks;

                _log.DocumentName = document;
                _entities.tblAssignmentLogs.Add(_log);
                _entities.SaveChanges();

                if (IsReassign || doc.Action == (int)WordAutomationDemo.Common.Global.Action.Declined)
                {
                    if (doc.DocumentType == (int)Global.DocumentType.Ppt)
                    {
                        var slides = _entities.tblAssignedPPTSlides.Where(z => z.AssignmentID == doc.AssignmentID && !(z.IsPPTApproved.HasValue && z.IsPPTApproved.Value == true));
                        foreach (var slide in slides)
                        {
                            slide.IsPPTModified = null;
                            slide.IsPPTApproved = null;
                            slide.IsGrayedOut = null;
                            _entities.Entry(slide).State = System.Data.Entity.EntityState.Modified;
                        }
                        _entities.SaveChanges();
                    }
                    else if (doc.DocumentType == (int)Global.DocumentType.Xls)
                    {
                        var sheets = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == doc.AssignmentID && !(z.IsSheetApproved == true));
                        foreach (var sheet in sheets)
                        {
                            sheet.IsSheetModified = false;
                            sheet.IsSheetApproved = false;
                            sheet.IsGrayedOut = false;
                            _entities.Entry(sheet).State = System.Data.Entity.EntityState.Modified;
                        }
                        _entities.SaveChanges();
                    }
                }

                if (doc.Action == (int)WordAutomationDemo.Common.Global.Action.Approved || doc.Action == (int)WordAutomationDemo.Common.Global.Action.Published)
                {
                    var assignedDocs = GetAssignedDocs(AssignID);
                    if (assignedDocs != null && assignedDocs.Count() > 0)
                    {
                        foreach (var assignedDoc in assignedDocs)
                        {
                            var FileExtention = assignedDoc.DocName.Split('.').Last().ToLower();
                            var NewFileName = assignedDoc.DocName;
                            if (FileExtention == "doc" || FileExtention == "docx")
                            {
                                if (FileExtention == "doc") // if old document file  then convert it to docx 
                                {
                                    NewFileName = NewFileName + "x";
                                }
                                string sourceFile = System.IO.Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, AssignID.ToString() + "/" + assignedDoc.DocName);
                                string destFile = System.IO.Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + NewFileName);
                                string destCopyFile = System.IO.Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + "Copy_" + NewFileName);
                                if (System.IO.File.Exists(sourceFile))
                                {
                                    System.IO.File.Copy(sourceFile, destFile, true);
                                    System.IO.File.Copy(sourceFile, destCopyFile, true);
                                }
                            }
                            else if (FileExtention == "ppt" || FileExtention == "pptx")
                            {
                                NewFileName = "Slides_" + NewFileName;
                                if (FileExtention == "ppt")
                                {
                                    NewFileName = NewFileName + "x";
                                }
                                string directoryName = Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, AssignID.ToString() + "/PPT/" + assignedDoc.DocName);
                                if (System.IO.Directory.Exists(directoryName))
                                {
                                    string destDirName = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, NewFileName);
                                    System.IO.Directory.CreateDirectory(destDirName);
                                    string sourceFile = System.IO.Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, AssignID.ToString() + "/" + assignedDoc.DocName);
                                    string destFileName = FileExtention == "ppt" ? assignedDoc.DocName + "x" : assignedDoc.DocName;
                                    string destFile = System.IO.Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + destFileName);
                                    if (System.IO.File.Exists(sourceFile))
                                    {
                                        System.IO.File.Copy(sourceFile, destFile, true);
                                    }
                                    DirectoryInfo dir = new DirectoryInfo(directoryName);
                                    FileInfo[] files = dir.GetFiles();
                                    int sequence = 1;
                                    foreach (FileInfo file in files)
                                    {
                                        if (file.Name.Split('.').Last().ToLower() == "ppt" || file.Name.Split('.').Last().ToLower() == "pptx")
                                        {
                                            tblPPTSlide _slide = new tblPPTSlide();
                                            _slide.SlideName = file.Name;
                                            _slide.Sequence = sequence;
                                            _slide.MasterDocumentName = assignedDoc.DocName;
                                            _entities.tblPPTSlides.Add(_slide);
                                            tblPPTSlide _slideOriginal = new tblPPTSlide();
                                            _slideOriginal.SlideName = file.Name;
                                            _slideOriginal.Sequence = sequence;
                                            _slideOriginal.MasterDocumentName = assignedDoc.DocName;
                                            _slideOriginal.IsOriginal = true;
                                            _entities.tblPPTSlides.Add(_slideOriginal);
                                            sequence++;
                                        }
                                        string temppath = Path.Combine(destDirName, file.Name);
                                        file.CopyTo(temppath, true);
                                    }
                                    _entities.SaveChanges();
                                }
                            }
                            else
                            {
                                string sourceFile = System.IO.Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, AssignID.ToString() + "/" + assignedDoc.DocName);
                                string destFile = System.IO.Path.Combine(DirectoryManagmentUtils.ReadOnlyFilesPath + "/" + NewFileName);
                                if (System.IO.File.Exists(sourceFile))
                                {
                                    System.IO.File.Copy(sourceFile, destFile, true);
                                }
                            }
                        }
                    }
                }

                var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                objAssignmentModelEmail.TaskName = doc.TaskName;
                objAssignmentModelEmail.AssignID = doc.AssignmentID;
                List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                lstAssignmentLogModel = GetAssignmentLog(doc.AssignmentID);
                string EmailBody = templates.TemplateContentForEmail;
                foreach (var userToNotify in _entities.tblUserDepartments.Where(x => doc.tblAssignmentMembers.Where(u => u.UserID == x.UserId).Count() > 0))
                {
                    templates.Subject = templates.Subject.Replace("#Subject#", "Task " + (IsReassign ? Common.Global.GetEnumDescription(WordAutomationDemo.Common.Global.Action.Reassign) : Common.Global.GetEnumDescription((Common.Global.Action)doc.Action)) + ": Task has been " + (IsReassign ? Common.Global.GetEnumDescription(WordAutomationDemo.Common.Global.Action.Reassign) : Common.Global.GetEnumDescription((Common.Global.Action)doc.Action)) + " by " + CurrentUserSession.User.FirstName);
                    EmailBody = PopulateEmailBody("A task has been " + (IsReassign ? Common.Global.GetEnumDescription(WordAutomationDemo.Common.Global.Action.Reassign) : Common.Global.GetEnumDescription((Common.Global.Action)doc.Action)) + " by " + CurrentUserSession.User.FirstName, userToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody);

                    byte[] fileBytes = null;
                    if (IsReassign && doc.DocumentType == (int)Global.DocumentType.Ppt)
                        fileBytes = CommonHelper.GeneratePDF("A task has been Reassigned by " + CurrentUserSession.User.FirstName, userToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel);

                    Global.SendEmail(userToNotify.EmailID, templates.Subject, EmailBody, fileBytes);
                }
                //var Currentuser = _entities.tblUserDepartments.Where(x => x.UserId == CurrentUserSession.UserID).FirstOrDefault();
                //Task.Factory.StartNew(() =>
                //{
                //    var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == doc.CreatedBy).FirstOrDefault();
                //    if (UserToNotify != null && !string.IsNullOrEmpty(UserToNotify.EmailID))
                //    {
                //        StringBuilder _sb = new StringBuilder();
                //        _sb.AppendLine("Hello " + UserToNotify.FullName + ",<br/><br/>");
                //        _sb.AppendLine(Currentuser.FullName + " has " + Common.Global.GetEnumDescription((Common.Global.Action)doc.Action) + " his task in Triyo. Please login to your account to check it.<br/><br/>");
                //        _sb.AppendLine("Regards,<br/><b>Triyo</b>");
                //        WordAutomationDemo.Common.Global.SendEmail(UserToNotify.EmailID, "Task " + Common.Global.GetEnumDescription((Common.Global.Action)doc.Action) + " by  " + Currentuser.FullName, _sb.ToString());
                //    }
                //});
                return true;
            }
            else
                return false;

        }

        #region Populate Email Body
        /// <summary>
        /// Populate Email Body – This method is used to populate email body.
        /// Created By: Dipak B. Kansara
        /// Created On: 09/06/2017
        /// </summary>
        /// <param name="result">result</param>
        /// <param name="EmailBody">Email Body</param>
        /// <returns>string</returns>
        private string PopulateEmailBody(string actionString, string AssignToName, AssignmentModel objAssignmentModel, List<AssignmentLogModel> lstAssignmentLogModel, string EmailBody, bool IsApproveDecline = false, int ActionID = 0)
        {
            var SiteUrl = Global.SiteUrl;
            var Currentuser = _entities.tblUserDepartments.Where(x => x.UserId == CurrentUserSession.UserID).FirstOrDefault();
            EmailBody = EmailBody.Replace("#FullName#", AssignToName);
            EmailBody = EmailBody.Replace("#TaskName#", objAssignmentModel.TaskName);
            EmailBody = EmailBody.Replace("#Action#", actionString);
            if (IsApproveDecline)
            {
                EmailBody = EmailBody.Replace("#Link#", "click to approve or decline task");
                EmailBody = EmailBody.Replace("#Button#", "<br><a title='Approve' href='" + SiteUrl + "Common/ProcessTask?AID=" + Global.Encryption.Encrypt(objAssignmentModel.AssignID.ToString()) + "&Action=" + Global.Encryption.Encrypt("3") + "'><img src='http://qamvc.alliancetek.org/triyosoft/CSS/images/approve-btn.png' alt='Approve' /></a>&nbsp;&nbsp;<a title='Decline' href='" + SiteUrl + "Common/ProcessTask?AID=" + Global.Encryption.Encrypt(objAssignmentModel.AssignID.ToString()) + "&Action=" + Global.Encryption.Encrypt("4") + "' alt='Decline'><img src='http://qamvc.alliancetek.org/triyosoft/CSS/images/decline-btn.png' alt='Decline' border='0'/></a>&nbsp;&nbsp;<a title='Reassign' href='" + SiteUrl + "Home/Approval' alt='Reassign'><img src='http://qamvc.alliancetek.org/triyosoft/CSS/images/reassign-btn.png' alt='Reassign' border='0'/></a>");
            }
            else
            {
                EmailBody = EmailBody.Replace("#Link#", "");
                EmailBody = EmailBody.Replace("#Button#", "");
            }

            if (lstAssignmentLogModel != null && lstAssignmentLogModel.Count > 0)
            {
                var AssignmentLogModel = lstAssignmentLogModel.OrderBy(s => s.CreatedDate).FirstOrDefault();
                if (AssignmentLogModel != null)
                {
                    EmailBody = EmailBody.Replace("#InitialComment#", AssignmentLogModel.Description);
                }
                EmailBody = EmailBody.Replace("#TaskDescription#", lstAssignmentLogModel.FirstOrDefault().Description);
            }
            string strCommentHistory = string.Empty;
            StringBuilder _sb = new StringBuilder();
            _sb.AppendLine("<thead><tr style='background-color: #4ca0dd; color: #fff; text-align: left;'>");
            _sb.AppendLine("<th style='padding:3px 8px; border-right:solid 1px #fff; border-bottom:solid 1px #fff; font-family:Arial, Helvetica, sans-serif;text-align: left;'>User</th>");
            _sb.AppendLine("<th style='padding:3px 8px; border-right:solid 1px #fff; border-bottom:solid 1px #fff; font-family:Arial, Helvetica, sans-serif;text-align: left;'>Action</th>");
            _sb.AppendLine("<th style='padding:3px 8px; border-right:solid 1px #fff; border-bottom:solid 1px #fff; font-family:Arial, Helvetica, sans-serif;text-align: left;'>Comments</th>");
            _sb.AppendLine("<th style='padding:3px 8px; border-right:solid 1px #fff; border-bottom:solid 1px #fff; font-family:Arial, Helvetica, sans-serif;text-align: left;'>Date</th>");
            _sb.AppendLine("</tr></thead><tbody>");
            foreach (AssignmentLogModel objAssignmentLogModel in lstAssignmentLogModel)
            {
                _sb.AppendLine("<tr style='font-family:Arial, Helvetica, sans-serif;'>");
                _sb.AppendLine("<td style='padding:3px 8px; border-left:solid 1px #dedede; border-right:solid 1px #dedede; border-bottom:solid 1px #dedede;'><span>" + objAssignmentLogModel.UserName + "</span></td>");
                _sb.AppendLine("<td style='padding:3px 8px; border-right:solid 1px #dedede; border-bottom:solid 1px #dedede;'><span>" + objAssignmentLogModel.ActionString + "</span></td>");
                _sb.AppendLine("<td style='padding:3px 8px; border-right:solid 1px #dedede; border-bottom:solid 1px #dedede;'><span>" + objAssignmentLogModel.Description + "</span></td>");
                _sb.AppendLine("<td style='padding:3px 8px; border-right:solid 1px #dedede; border-bottom:solid 1px #dedede;'><span>" + objAssignmentLogModel.CreatedDateString + "</span></td></tr>");
            }
            _sb.AppendLine("</tbody>");
            EmailBody = EmailBody.Replace("#CommentHistory#", _sb.ToString());

            return EmailBody;
        }
        #endregion

        #region Get Assignment Log
        /// <summary>
        /// Get Assignment Log – This method is used to get assignment log.
        /// Created By: Dipak B. Kansara
        /// Created On: 09/06/2017
        /// </summary>
        /// <param name="assignmentID">assignmentID</param>
        /// <returns>List<AssignmentLogModel></returns>
        private List<AssignmentLogModel> GetAssignmentLog(int assignmentID)
        {
            var lstAssignmentLog = (from items in _entities.tblAssignmentLogs
                                    where items.AssignmentID == assignmentID
                                    select new AssignmentLogModel
                                    {
                                        AssignmentID = items.AssignmentID.HasValue ? items.AssignmentID.Value : 0,
                                        AssignmentLogID = items.AssignmentLogID,
                                        Description = items.Description,
                                        Action = items.Action,
                                        UserName = items.CreatedBy.HasValue ? items.tblUserDepartment.FullName : "",
                                        CreatedDate = items.CreatedDate
                                    }).OrderByDescending(a => a.CreatedDate).ToList();
            foreach (var item in lstAssignmentLog)
            {
                item.ActionString = item.Action > 0 ? Enum.GetName(typeof(Global.Action), item.Action) : "";

                if (WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid != null)
                {
                    item.CreatedDateString = item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid + " hh:mm:ss") : "";
                }

                //item.CreatedDateString = item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString(Session["DTFormatGrid"].ToString() + " hh:mm:ss") : "";

                //if (Session != null && Session["DTFormatGrid"] != null)
                //{
                //    item.CreatedDateString = item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString(Session["DTFormatGrid"].ToString() + " hh:mm:ss") : "";
                //}
                //else {
                //    item.CreatedDateString = item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("MM/dd/yyyy hh:mm:ss") : "";
                //}

            }
            return lstAssignmentLog;
        }
        #endregion

        public List<UploadedDocModel> GetAssignedDocs(int assignmentID)
        {
            var AssignedDocs = (from items in _entities.tblAssignedWordPages
                                where items.AssignmentID == assignmentID
                                select new UploadedDocModel
                                {
                                    AssignedDocsID = items.AssignedWordPageID,
                                    DocName = items.DocName,
                                    AssignmentID = items.AssignmentID.HasValue ? items.AssignmentID.Value : 0,
                                    Sequence = items.Sequence.HasValue ? items.Sequence.Value : 0
                                }).ToList();
            return AssignedDocs;
        }

        public JsonResult GetProjectDocumentList(int? projectID, int? documentType)
        {
            List<ProjectDocumentModel> lstProjectDocumentModel = new List<ProjectDocumentModel>();

            var currUserId = CurrentUserSession.User.UserID;
            var currUserCompanyId = CurrentUserSession.User.CompanyID;

            List<DocModel> projDocs = (from proj in _entities.tblProjects
                                       join member in _entities.tblProjectMembers on proj.ProjectID equals member.ProjectID
                                       join projDoc in _entities.tblProjectDocuments on proj.ProjectID equals projDoc.ProjectID
                                       join doc in _entities.tblDocuments on projDoc.DocumentID equals doc.DocumentID
                                       where (proj.CreatedBy == currUserId || member.UserID == currUserId)
                                             && !proj.IsDeleted
                                             && proj.Status == (int)Global.ProjectStatus.Active
                                             && doc.CompanyID == currUserCompanyId
                                       select new DocModel
                                       {
                                           DocumentID = doc.DocumentID,
                                           DisplayName = doc.DisplayName,
                                           DocumentName = doc.DocumentName,
                                           DocumentType = doc.DocumentType,
                                           ProjectID = proj.ProjectID,
                                           ProjectName = proj.Name,
                                       }).Distinct().ToList();

            if (projectID > 0)
            {
                projDocs = projDocs.Where(pd => pd.ProjectID == projectID).ToList();
            }

            if (projDocs.Count > 0)
            {
                lstProjectDocumentModel = projDocs
                    .Select(pd => new ProjectDocumentModel
                    {
                        ProjectName = pd.ProjectName,
                        DocName = pd.DocumentName,
                        ProjectTaskID = pd.ProjectID.ToString() + "_0_" + pd.DocumentType.ToString() + "_" + pd.DocumentID.ToString()
                    }).ToList();
            }

            return Json(lstProjectDocumentModel, JsonRequestBehavior.AllowGet);
        }

        public List<AssignmentModel> GetPendingAssignedTasks(string FileName)
        {
            var pendingTasks = new List<AssignmentModel>();
            if (!string.IsNullOrEmpty(FileName))
            {
                using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                {
                    pendingTasks = (from items in _entities.tblAssignments
                                    join doc in _entities.tblDocuments on items.DocumentID equals doc.DocumentID
                                    where items.Action == (int)WordAutomationDemo.Common.Global.Action.Completed
                                     && doc.DocumentName == FileName
                                    select new AssignmentModel
                                    {
                                        ProjectID = items.ProjectID.Value,
                                        DocumentName = items.DocumentFile,
                                        OriginalDocumentName = doc.DocumentName,
                                        ReplacementCode = items.ReplacementCode,
                                        ProjectName = _entities.tblProjects.Where(z => z.ProjectID == items.ProjectID).Select(z => z.Name).FirstOrDefault()
                                    }).ToList();

                }
            }
            return pendingTasks;
        }


        public void SendErrorToText(Exception ex, string functionName = "")
        {
            try
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
            catch (Exception e)
            {
                e.ToString();
            }
        }


        public void SendErrorToText(string message, string functionName = "")
        {
            try
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
                        sw.WriteLine(message);
                    }
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
        }


        //LOAD document to RichEdit control
        public ActionResult LoadAndSavePartial(string name = "", int userid = 0, int Action = 1)
        {
            var x = Request.Form["DXCallbackArgument"];


            //DevExpress.Web.ASPxRichEdit.REROpenCommand op = new DevExpress.Web.ASPxRichEdit.REROpenCommand();

            // CallbackEventArgsBase e = this;
            //RichEditSettings s =new RichEditSettings();
            //var  x= RichEditExtension.GetCallbackResult("RicheEdit")
            var demoModel = new AssignmentModel() { DocumentName = !string.IsNullOrEmpty(name) ? name : "", UserID = userid, Action = Action };
            return PartialView("_LoadAndSavePartial", demoModel);
        }

        public ActionResult LoadAndSavePagePartial(string name = "", int userid = 0, int Action = 1)
        {
            var x = Request.Form["DXCallbackArgument"];


            //DevExpress.Web.ASPxRichEdit.REROpenCommand op = new DevExpress.Web.ASPxRichEdit.REROpenCommand();

            // CallbackEventArgsBase e = this;
            //RichEditSettings s =new RichEditSettings();
            //var  x= RichEditExtension.GetCallbackResult("RicheEdit")
            var demoModel = new AssignmentModel() { DocumentName = !string.IsNullOrEmpty(name) ? name : "", UserID = userid, Action = Action };
            return PartialView("_LoadAndSavePagePartial", demoModel);
        }

        //Gets all snippets from db
        public ActionResult GetAllSnippets([DataSourceRequest] DataSourceRequest request)
        {
            var result = new DataSourceResult()
            {
                Data = GetAllSnippet(request), // Process data
                Total = _Count // Total number of records
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public IEnumerable GetAllSnippet([DataSourceRequest]DataSourceRequest command)
        {
            ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
            var SnippetList = (from item in _entities.tblAssignments
                               join proj in _entities.tblProjects on item.ProjectID equals proj.ProjectID
                               join document in _entities.tblDocuments on item.DocumentID equals document.DocumentID into gj
                               from doc in gj.DefaultIfEmpty()
                               where item.CreatedBy == CurrentUserSession.UserID && item.Action == (int)WordAutomationDemo.Common.Global.Action.Assigned && proj.Status == (int)Global.ProjectStatus.Active && proj.IsDeleted != true
                               orderby proj.CreatedDate descending
                               select new AssignmentModel()
                               {
                                   Action = item.Action,
                                   Content = item.Content,
                                   DateTime = item.CreatedDate,
                                   DocumentName = item.DocumentFile,
                                   UserID = item.LockedByUserID,
                                   ReplacementCode = item.ReplacementCode,
                                   UserName = item.LockedBy.FullName,
                                   AssignedTo = item.tblAssignmentMembers.Select(m => m.tblUserDepartment.FullName),
                                   Department = item.tblAssignmentMembers.Select(m => m.tblUserDepartment.Department),
                                   //SourceFileCloneFile = item.SourceFileCloneFile,
                                   OriginalDocumentName = doc.DocumentName,
                                   Comments = !string.IsNullOrEmpty(item.Comments) ? item.Comments : "",
                                   TaskName = item.TaskName,
                                   Remarks = item.Remarks,
                                   dtDueDate = item.DueDate,
                                   DocumentType = item.DocumentType.HasValue && item.DocumentType.Value > 0 ? item.DocumentType.Value : 0,
                                   AssignID = item.AssignmentID,
                                   ProjectID = (item.ProjectID.HasValue) ? item.ProjectID.Value : 0,

                                   ProjectName = _entities.tblProjects.Where(z => z.ProjectID == item.ProjectID).Select(z => z.Name).FirstOrDefault(),
                                   ProjectDateTime = proj.CreatedDate

                               }).ToList();

            IQueryable<AssignmentModel> SnippetListData = SnippetList.AsQueryable().OrderByDescending(s => s.ProjectDateTime);

            ////Apply filtering
            //result = result.ApplyFiltering(command.Filters);

            ////Get count of total records
            //_Count = result.Count();

            ////Apply sorting
            //result = result.ApplySorting(command.Groups, command.Sorts);

            ////Apply paging
            //result = result.ApplyPaging(command.Page, command.PageSize);


            ////Apply grouping
            //if (command.Groups.Any())
            //{
            //    return result.ApplyGrouping(command.Groups);
            //}
            //var data = result.ToList();

            //return data;

            //Apply filtering
            SnippetListData = SnippetListData.ApplyFiltering(command.Filters);

            //Get count of total records
            _Count = SnippetList.Count();

            //Apply sorting
            SnippetListData = SnippetListData.ApplySorting(command.Groups, command.Sorts);

            //Apply paging
            SnippetListData = SnippetListData.ApplyPaging(command.Page, command.PageSize);

            //Apply grouping
            if (command.Groups.Any())
            {
                return SnippetListData.ApplyGrouping(command.Groups);
            }
            return SnippetListData.ToList();
        }

        public List<SelectListItem> GetAllMembers()
        {
            if (CurrentUserSession.User.IsSuperAdmin)
            {
                return (from items in _entities.tblUserDepartments
                        where items.UserId != CurrentUserSession.UserID
                        && items.IsActive.HasValue && items.IsActive.Value
                        select new SelectListItem()
                        {
                            Text = items.FullName,
                            Value = SqlFunctions.StringConvert((double)items.UserId).Trim(),
                        }).ToList();
            }
            else
            {
                return (from items in _entities.tblUserDepartments
                        where items.UserId != CurrentUserSession.UserID
                        && items.CompanyID == CurrentUserSession.User.CompanyID
                        && items.IsActive.HasValue && items.IsActive.Value
                        select new SelectListItem()
                        {
                            Text = items.FullName,
                            Value = SqlFunctions.StringConvert((double)items.UserId).Trim(),
                        }).ToList();
            }
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetMembersByProject(int ProjectID)
        {
            var resp = GetMembersByProjectID(ProjectID);
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public List<UserModel> GetMembersByProjectID(int ProjectID)
        {
            var currentUserCompanyID = (int)CurrentUserSession.User.CompanyID;
            var ret = (from p in _entities.tblProjectMembers
                       join u in _entities.tblUserDepartments on p.UserID equals u.UserId
                       where p.ProjectID == ProjectID && /*p.UserID != CurrentUserSession.UserID &&*/ u.CompanyID == currentUserCompanyID
                       && u.IsActive.HasValue && u.IsActive.Value
                       select new UserModel()
                       {
                           FullName = u.FullName,
                           Company = u.tblCompany.Name,
                           CompanyLogo = u.tblCompany.CompanyLogo,
                           Company_ID = u.tblCompany.CompanyID,
                           Department = u.Department,
                           IsActive = u.IsActive == true,
                           UserName = u.UserName,
                           ProfileImage = u.ProfileImage,
                           CanApprove = u.CanApprove == true,
                           CanEdit = u.CanEdit == true,
                           UserId = u.UserId
                       }).ToList();

            //Include project creator as well
            var projCreator = _entities.tblProjects.FirstOrDefault(p => p.ProjectID == ProjectID);
            if (projCreator != null/* && projCreator.CreatedBy != CurrentUserSession.UserID*/)
            {
                var projCreatorUser = _entities.tblUserDepartments.FirstOrDefault(u => u.UserId == projCreator.CreatedBy);
                var projCreatorCompany =
                    _entities.tblCompanies.FirstOrDefault(c => c.CompanyID == projCreatorUser.CompanyID);

                var userModel = new UserModel()
                {
                    FullName = projCreatorUser.FullName,
                    Company = projCreatorCompany.Name,
                    CompanyLogo = projCreatorCompany.CompanyLogo,
                    Department = projCreatorUser.Department,
                    IsActive = projCreatorUser.IsActive == true,
                    UserName = projCreatorUser.UserName,
                    ProfileImage = projCreatorUser.ProfileImage,
                    CanApprove = projCreatorUser.CanApprove == true,
                    CanEdit = projCreatorUser.CanEdit == true,
                    UserId = projCreator.CreatedBy,
                    Company_ID = (int)projCreatorUser.CompanyID,

                };

                if (ret.FirstOrDefault(um => um.UserId == projCreator.CreatedBy) == null)
                {
                    ret.Add(userModel);
                }
            }


            foreach (var u in ret)
                u.ProfileImage = string.IsNullOrWhiteSpace(u.ProfileImage) ? "/CSS/images/no-photo.jpg" : "/ApplicationDocuments/ProfileImages/" + u.ProfileImage;
            return ret;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetMembersByProjectForReassign(int ProjectID, int AssignmentID)
        {
            var projectUsers = GetMembersByProjectID(ProjectID);
            var assignmentMembers = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == AssignmentID).ToList();
            var assignmentMemberUserIDs = assignmentMembers.Select(am => am.UserID).ToList();

            foreach (var projectUser in projectUsers)
            {
                if (assignmentMemberUserIDs.Contains(projectUser.UserId))
                {
                    var assignmentMember = assignmentMembers.FirstOrDefault(am => am.UserID == projectUser.UserId);
                    projectUser.CanPublish = assignmentMember.CanPublish.HasValue ? assignmentMember.CanPublish.Value : false;
                    projectUser.CanApprove = assignmentMember.CanApprove;
                    projectUser.CanEdit = assignmentMember.CanEdit;
                    projectUser.CanView = !projectUser.CanPublish && !projectUser.CanApprove && !projectUser.CanEdit;
                }
                else
                {
                    projectUser.CanPublish = false;
                    projectUser.CanApprove = false;
                    projectUser.CanEdit = false;
                    projectUser.CanView = false;
                }
            }
            return Json(projectUsers, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAllMembersExcept(int[] exclude)
        {
            var resp = GetAllUsers(null).Where(u => exclude == null || exclude.Length == 0 || !exclude.Contains(u.UserId)).ToList();
            return Json(resp, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetMembersByTask(int? taskId)
        {
            var resp = (taskId == null || taskId == 0) ? new List<UserModel>() : GetMembersByTaskID((int)taskId);
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public List<UserModel> GetMembersByTaskID(int taskId)
        {
            return (from p in _entities.tblAssignmentMembers
                    join u in _entities.tblUserDepartments on p.UserID equals u.UserId
                    where u.IsActive.HasValue && u.IsActive.Value
                    select new UserModel()
                    {
                        FullName = u.FullName,
                        Company = u.tblCompany.Name,
                        CompanyLogo = u.tblCompany.CompanyLogo,
                        Company_ID = u.tblCompany.CompanyID,
                        Department = u.Department,
                        IsActive = u.IsActive == true,
                        UserName = u.UserName,
                        ProfileImage = string.IsNullOrWhiteSpace(u.ProfileImage) ? "/CSS/images/no-photo.jpg" : "/ApplicationDocuments/ProfileImages/" + u.ProfileImage,
                        CanApprove = p.CanApprove,
                        CanEdit = p.CanEdit,
                        UserId = u.UserId
                    }).ToList();
        }
        public IEnumerable<UserModel> GetAllUsers([DataSourceRequest]DataSourceRequest command)
        {
            var demo = new AssignmentModel();
            ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
            demo.UserList = (from u in _entities.tblUserDepartments
                                 //where u.UserId != CurrentUserSession.UserID
                             select new UserModel()
                             {
                                 FullName = u.FullName,
                                 Company = u.tblCompany.Name,
                                 CompanyLogo = u.tblCompany.CompanyLogo,
                                 Company_ID = u.tblCompany.CompanyID,
                                 Department = u.Department,
                                 IsActive = u.IsActive == true,
                                 UserName = u.UserName,
                                 ProfileImage = u.ProfileImage,
                                 CanApprove = u.CanApprove == true,
                                 CanEdit = u.CanEdit == true,
                                 UserId = u.UserId
                             }).ToList();
            foreach (var u in demo.UserList)
                u.ProfileImage = string.IsNullOrWhiteSpace(u.ProfileImage) ? "/CSS/images/no-photo.jpg" : "/ApplicationDocuments/ProfileImages/" + u.ProfileImage;

            _Count = demo.UserList.Count();
            return demo.UserList;
        }

        public ActionResult GetMySnippets([DataSourceRequest]DataSourceRequest command, int userid)
        {
            if (userid > 0)
            {
                var result = new DataSourceResult()
                {
                    Data = GetMySnippetsData(command, userid),
                    Total = _Count // Total number of records
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable GetMySnippetsData([DataSourceRequest]DataSourceRequest command, int UserID)
        {
            var result = GetSnippetsByUser(UserID).GroupBy(p => p.AssignID)
                                                  .Select(g => g.First());

            //Apply filtering
            result = result.ApplyFiltering(command.Filters);

            //Get count of total records
            _Count = result.Count();

            //Apply sorting
            result = result.ApplySorting(command.Groups, command.Sorts);

            //Apply paging
            result = result.ApplyPaging(command.Page, command.PageSize);


            //Apply grouping
            if (command.Groups.Any())
            {
                return result.ApplyGrouping(command.Groups);
            }
            var data = result.ToList();

            return data;
        }

        public int GetSnippetsActionCount()
        {
            using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
            {
                return _entities.tblAssignments.Where(item => item.Action == (int)WordAutomationDemo.Common.Global.Action.Completed && item.CreatedBy == CurrentUserSession.UserID).Count();
            }
        }


        public IQueryable<AssignmentModel> GetSnippetsByUser(int userid)
        {
            var demo = new AssignmentModel();
            var Declined = (int)WordAutomationDemo.Common.Global.Action.Declined;
            ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
            var SnippetList = (from membership in _entities.tblAssignmentMembers
                               join item in _entities.tblAssignments on membership.AssignmentID equals item.AssignmentID
                               join document in _entities.tblDocuments on item.DocumentID equals document.DocumentID into gj
                               from doc in gj.DefaultIfEmpty()
                               join dept in _entities.tblUserDepartments on item.CreatedBy equals dept.UserId
                               join Assigneddept in _entities.tblUserDepartments on membership.UserID equals Assigneddept.UserId
                               join proj in _entities.tblProjects on item.ProjectID equals proj.ProjectID
                               where (((membership.UserID == userid) || (item.CreatedBy == userid && item.Action != (int)WordAutomationDemo.Common.Global.Action.Assigned)) && proj.Status == (int)Global.ProjectStatus.Active && proj.IsDeleted != true && (item.DocumentType != (int)WordAutomationDemo.Common.Global.DocumentType.Ppt || item.DocumentType != (int)WordAutomationDemo.Common.Global.DocumentType.Xls))
                               orderby item.CreatedDate descending
                               select new AssignmentModel()
                               {
                                   AssignID = item.AssignmentID,
                                   AssignmentID = SqlFunctions.StringConvert((double)item.AssignmentID).Trim(),
                                   Action = item.Action,
                                   Content = item.Content,
                                   DateTime = item.CreatedDate,
                                   DocumentName = item.DocumentFile,
                                   UserID = membership.UserID,
                                   Department = new string[] { membership.tblUserDepartment.Department },
                                   ReplacementCode = item.ReplacementCode,
                                   OriginalDocumentName = doc.DocumentName,
                                   //SourceFileCloneFile = item.SourceFileCloneFile,
                                   AssignedBy = string.IsNullOrEmpty(dept.Department) ? dept.FullName : dept.FullName + " (" + dept.Department + ")",
                                   CreatedBy = item.CreatedBy,
                                   AssignedTo = new string[] { string.IsNullOrEmpty(Assigneddept.Department) ? Assigneddept.FullName : Assigneddept.FullName + " (" + Assigneddept.Department + ")" },
                                   Comments = !string.IsNullOrEmpty(item.Comments) ? item.Comments : "",
                                   TaskName = item.TaskName,
                                   DeclinedReason = item.tblAssignmentLogs.Where(x => x.Action == Declined).OrderByDescending(x => x.AssignmentLogID).FirstOrDefault().Description,
                                   Remarks = item.Remarks,
                                   DocumentType = (item.DocumentType.HasValue) ? item.DocumentType.Value : 0,
                                   dtDueDate = item.DueDate,
                                   dtCompletedDate = item.CompletedDate,
                                   ProjectID = (item.ProjectID.HasValue) ? item.ProjectID.Value : 0,
                                   ProjectName = _entities.tblProjects.Where(x => x.ProjectID == item.ProjectID).Select(z => z.Name).FirstOrDefault()
                               }).OrderByDescending(z => z.DateTime);

            var SnippetListPPT = (from membership in _entities.tblAssignmentMembers
                                  join item in _entities.tblAssignments on membership.AssignmentID equals item.AssignmentID
                                  join document in _entities.tblDocuments on item.DocumentID equals document.DocumentID into gj
                                  from doc in gj.DefaultIfEmpty()
                                  join dept in _entities.tblUserDepartments on item.CreatedBy equals dept.UserId
                                  join Assigneddept in _entities.tblUserDepartments on membership.UserID equals Assigneddept.UserId
                                  join proj in _entities.tblProjects on item.ProjectID equals proj.ProjectID
                                  join AssignedPPT in _entities.tblAssignedPPTSlides on item.AssignmentID equals AssignedPPT.AssignmentID
                                  where (((membership.UserID == userid) || (item.CreatedBy == userid)) && proj.Status == (int)Global.ProjectStatus.Active && proj.IsDeleted != true && item.DocumentType == (int)WordAutomationDemo.Common.Global.DocumentType.Ppt)
                                  orderby item.CreatedDate descending
                                  select new AssignmentModel()
                                  {
                                      AssignID = item.AssignmentID,
                                      AssignmentID = SqlFunctions.StringConvert((double)item.AssignmentID).Trim(),
                                      Action = item.Action,
                                      Content = item.Content,
                                      DateTime = item.CreatedDate,
                                      DocumentName = item.DocumentFile,
                                      UserID = membership.UserID,
                                      Department = new string[] { membership.tblUserDepartment.Department },
                                      ReplacementCode = item.ReplacementCode,
                                      OriginalDocumentName = doc.DocumentName,
                                      //SourceFileCloneFile = item.SourceFileCloneFile,
                                      AssignedBy = string.IsNullOrEmpty(dept.Department) ? dept.FullName : dept.FullName + " (" + dept.Department + ")",
                                      CreatedBy = item.CreatedBy,
                                      AssignedTo = new string[] { string.IsNullOrEmpty(Assigneddept.Department) ? Assigneddept.FullName : Assigneddept.FullName + " (" + Assigneddept.Department + ")" },
                                      Comments = !string.IsNullOrEmpty(item.Comments) ? item.Comments : "",
                                      TaskName = item.TaskName,
                                      DeclinedReason = item.tblAssignmentLogs.Where(x => x.Action == Declined).OrderByDescending(x => x.AssignmentLogID).FirstOrDefault().Description,
                                      Remarks = item.Remarks,
                                      DocumentType = (item.DocumentType.HasValue) ? item.DocumentType.Value : 0,
                                      dtDueDate = item.DueDate,
                                      dtCompletedDate = item.CompletedDate,
                                      ProjectID = (item.ProjectID.HasValue) ? item.ProjectID.Value : 0,
                                      ProjectName = _entities.tblProjects.Where(x => x.ProjectID == item.ProjectID).Select(z => z.Name).FirstOrDefault(),
                                      IsPPTModified = AssignedPPT.IsPPTModified
                                  }).OrderByDescending(z => z.DateTime);

            var SnippetListExcel = (from membership in _entities.tblAssignmentMembers
                                    join item in _entities.tblAssignments on membership.AssignmentID equals item.AssignmentID
                                    join document in _entities.tblDocuments on item.DocumentID equals document.DocumentID into gj
                                    from doc in gj.DefaultIfEmpty()
                                    join dept in _entities.tblUserDepartments on item.CreatedBy equals dept.UserId
                                    join Assigneddept in _entities.tblUserDepartments on membership.UserID equals Assigneddept.UserId
                                    join proj in _entities.tblProjects on item.ProjectID equals proj.ProjectID
                                    join AssignedExcel in _entities.tblAssignedExcelSheets on item.AssignmentID equals AssignedExcel.AssignmentID
                                    where (((membership.UserID == userid) || (item.CreatedBy == userid)) && proj.Status == (int)Global.ProjectStatus.Active && proj.IsDeleted != true && item.DocumentType == (int)WordAutomationDemo.Common.Global.DocumentType.Xls)
                                    orderby item.CreatedDate descending
                                    select new AssignmentModel()
                                    {
                                        AssignID = item.AssignmentID,
                                        AssignmentID = SqlFunctions.StringConvert((double)item.AssignmentID).Trim(),
                                        Action = item.Action,
                                        Content = item.Content,
                                        DateTime = item.CreatedDate,
                                        DocumentName = item.DocumentFile,
                                        UserID = membership.UserID,
                                        Department = new string[] { membership.tblUserDepartment.Department },
                                        ReplacementCode = item.ReplacementCode,
                                        OriginalDocumentName = doc.DocumentName,
                                        //SourceFileCloneFile = item.SourceFileCloneFile,
                                        AssignedBy = string.IsNullOrEmpty(dept.Department) ? dept.FullName : dept.FullName + " (" + dept.Department + ")",
                                        CreatedBy = item.CreatedBy,
                                        AssignedTo = new string[] { string.IsNullOrEmpty(Assigneddept.Department) ? Assigneddept.FullName : Assigneddept.FullName + " (" + Assigneddept.Department + ")" },
                                        Comments = !string.IsNullOrEmpty(item.Comments) ? item.Comments : "",
                                        TaskName = item.TaskName,
                                        DeclinedReason = item.tblAssignmentLogs.Where(x => x.Action == Declined).OrderByDescending(x => x.AssignmentLogID).FirstOrDefault().Description,
                                        Remarks = item.Remarks,
                                        DocumentType = (item.DocumentType.HasValue) ? item.DocumentType.Value : 0,
                                        dtDueDate = item.DueDate,
                                        dtCompletedDate = item.CompletedDate,
                                        ProjectID = (item.ProjectID.HasValue) ? item.ProjectID.Value : 0,
                                        ProjectName = _entities.tblProjects.Where(x => x.ProjectID == item.ProjectID).Select(z => z.Name).FirstOrDefault(),
                                        IsExcelSheetModified = AssignedExcel.IsSheetModified,
                                        IsExcelSheetApproved = AssignedExcel.IsSheetApproved
                                    }).OrderByDescending(z => z.DateTime);

            var SnippetListWordPages = (from membership in _entities.tblAssignmentMembers
                                        join item in _entities.tblAssignments on membership.AssignmentID equals item.AssignmentID
                                        join document in _entities.tblDocuments on item.DocumentID equals document.DocumentID into gj
                                        from doc in gj.DefaultIfEmpty()
                                        join dept in _entities.tblUserDepartments on item.CreatedBy equals dept.UserId
                                        join Assigneddept in _entities.tblUserDepartments on membership.UserID equals Assigneddept.UserId
                                        join proj in _entities.tblProjects on item.ProjectID equals proj.ProjectID
                                        join AssignedWordPage in _entities.tblAssignedWordPages on item.AssignmentID equals AssignedWordPage.AssignmentID
                                        where (((membership.UserID == userid) || (item.CreatedBy == userid)) && proj.Status == (int)Global.ProjectStatus.Active && proj.IsDeleted != true && item.DocumentType == (int)WordAutomationDemo.Common.Global.DocumentType.Word)
                                        orderby item.CreatedDate descending
                                        select new AssignmentModel()
                                        {
                                            AssignID = item.AssignmentID,
                                            AssignmentID = SqlFunctions.StringConvert((double)item.AssignmentID).Trim(),
                                            Action = item.Action,
                                            Content = item.Content,
                                            DateTime = item.CreatedDate,
                                            DocumentName = item.DocumentFile,
                                            UserID = membership.UserID,
                                            Department = new string[] { membership.tblUserDepartment.Department },
                                            ReplacementCode = item.ReplacementCode,
                                            OriginalDocumentName = doc.DocumentName,
                                            //SourceFileCloneFile = item.SourceFileCloneFile,
                                            AssignedBy = string.IsNullOrEmpty(dept.Department) ? dept.FullName : dept.FullName + " (" + dept.Department + ")",
                                            CreatedBy = item.CreatedBy,
                                            AssignedTo = new string[] { string.IsNullOrEmpty(Assigneddept.Department) ? Assigneddept.FullName : Assigneddept.FullName + " (" + Assigneddept.Department + ")" },
                                            Comments = !string.IsNullOrEmpty(item.Comments) ? item.Comments : "",
                                            TaskName = item.TaskName,
                                            DeclinedReason = item.tblAssignmentLogs.Where(x => x.Action == Declined).OrderByDescending(x => x.AssignmentLogID).FirstOrDefault().Description,
                                            Remarks = item.Remarks,
                                            DocumentType = (item.DocumentType.HasValue) ? item.DocumentType.Value : 0,
                                            dtDueDate = item.DueDate,
                                            dtCompletedDate = item.CompletedDate,
                                            ProjectID = (item.ProjectID.HasValue) ? item.ProjectID.Value : 0,
                                            ProjectName = _entities.tblProjects.Where(x => x.ProjectID == item.ProjectID).Select(z => z.Name).FirstOrDefault(),
                                            IsPPTModified = AssignedWordPage.IsPublished.HasValue || AssignedWordPage.ReviewRequested.HasValue
                                        }).OrderByDescending(z => z.DateTime);


            var lstSnippet = SnippetList.ToList();

            foreach (var item in SnippetListWordPages)
            {
                if (item.IsPPTModified.HasValue && item.IsPPTModified.Value == true)
                {
                    if (!lstSnippet.Where(s => s.AssignmentID == item.AssignmentID).Any() && !lstSnippet.Contains(item))
                    {
                        lstSnippet.Add(item);
                    }
                }
                else if (item.UserID == userid)
                {
                    if (!lstSnippet.Where(s => s.AssignmentID == item.AssignmentID).Any() && !lstSnippet.Contains(item))
                    {
                        lstSnippet.Add(item);
                    }
                }
                else if (item.Action == (int)Global.Action.Completed || item.Action == (int)Global.Action.Approved || item.Action == (int)Global.Action.Published)
                {
                    if (!lstSnippet.Where(s => s.AssignmentID == item.AssignmentID).Any() && !lstSnippet.Contains(item))
                    {
                        lstSnippet.Add(item);
                    }
                }
            }
            foreach (var item in SnippetListPPT)
            {
                if (item.IsPPTModified.HasValue && item.IsPPTModified.Value == true)
                {
                    if (!lstSnippet.Where(s => s.AssignmentID == item.AssignmentID).Any() && !lstSnippet.Contains(item))
                    {
                        lstSnippet.Add(item);
                    }
                }
                else if (item.UserID == userid)
                {
                    if (!lstSnippet.Where(s => s.AssignmentID == item.AssignmentID).Any() && !lstSnippet.Contains(item))
                    {
                        lstSnippet.Add(item);
                    }
                }
                else if (item.Action == (int)Global.Action.Completed || item.Action == (int)Global.Action.Approved || item.Action == (int)Global.Action.Published)
                {
                    if (!lstSnippet.Where(s => s.AssignmentID == item.AssignmentID).Any() && !lstSnippet.Contains(item))
                    {
                        lstSnippet.Add(item);
                    }
                }
            }

            foreach (var item in SnippetListExcel)
            {
                if (item.IsExcelSheetModified == true && !(item.IsExcelSheetApproved == true))
                {
                    if (!lstSnippet.Where(s => s.AssignmentID == item.AssignmentID).Any() && !lstSnippet.Contains(item))
                    {
                        lstSnippet.Add(item);
                    }
                }
                else if (item.UserID == userid)
                {
                    if (!lstSnippet.Where(s => s.AssignmentID == item.AssignmentID).Any() && !lstSnippet.Contains(item))
                    {
                        lstSnippet.Add(item);
                    }
                }
                else if (item.Action == (int)Global.Action.Completed || item.Action == (int)Global.Action.Approved || item.Action == (int)Global.Action.Published)
                {
                    if (!lstSnippet.Where(s => s.AssignmentID == item.AssignmentID).Any() && !lstSnippet.Contains(item))
                    {
                        lstSnippet.Add(item);
                    }
                }
            }

            var lstSnippetData = lstSnippet.AsQueryable().OrderByDescending(z => z.DateTime);

            return lstSnippetData;
        }

        //Saves Snippet entry to db
        public int SaveSnippet(DocumentModel model)
        {
            ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();
            tblAssignment _newentry = new tblAssignment();
            _newentry.Content = model.Content;
            _newentry.ReplacementCode = model.ReplacementCode;
            _newentry.ProjectID = model.ProjectID;
            _newentry.DocumentID = model.DocumentID;
            _newentry.DocumentFile = model.DocumentFile;
            _newentry.CreatedDate = DateTime.Now;
            _newentry.Action = model.Action;
            _newentry.CreatedBy = model.CreatedBy;
            _newentry.DocumentType = model.DocumentType;
            _newentry.Status = 1;
            //_newentry.OrginalSourceFile = model.OriginalDocument;
            _newentry.Comments = model.Comment;
            _newentry.Remarks = model.Remarks;
            _newentry.DueDate = model.DueDate;
            _newentry.TaskName = model.TaskName;
            _newentry.IsEntireDocument = model.IsEntireDocument;
            //_newentry.tblAssignmentMembers = new tblAssignmentMember[] { new tblAssignmentMember() { UserID = model.UserID, tblAssignment = _newentry } };
            _entities.tblAssignments.Add(_newentry);
            _entities.SaveChanges();

            //Add Log 
            tblAssignmentLog _log = new tblAssignmentLog();
            _log.Action = model.Action;
            _log.AssignmentID = _newentry.AssignmentID;
            _log.CreatedDate = DateTime.Now;
            _log.CreatedBy = CurrentUserSession.UserID;
            _log.Description = model.Comment;
            _log.DocumentName = model.OriginalDocument;
            _entities.tblAssignmentLogs.Add(_log);
            _entities.SaveChanges();

            return _newentry.AssignmentID;
        }

        public void SplitSlidesNew(string FileName)
        {
            var presentationPath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, FileName);
            Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(presentationPath);

            int totalSlideCount = presentation.Slides.Count;

            string slidesPath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, "Slides_" + FileName);
            if (!System.IO.Directory.Exists(slidesPath))
            {
                System.IO.Directory.CreateDirectory(slidesPath);
            }

            for (int i = 0; i < totalSlideCount; i++)
            {
                var slideName = (i + 1).ToString() + ".pptx";
                var thumbName = (i + 1).ToString() + Global.ImageExportExtention;

                var slide = presentation.Slides[i];

                using (var newSlidePres = new Aspose.Slides.Presentation())
                {
                    newSlidePres.Slides.AddClone(slide);
                    if (newSlidePres.Slides.Count > 1)
                    {
                        newSlidePres.Slides.RemoveAt(0);
                    }
                    newSlidePres.Save(Path.Combine(slidesPath, slideName), Aspose.Slides.Export.SaveFormat.Pptx);
                }

                using (System.Drawing.Image image = presentation.Slides[i].GetThumbnail(1.0F, 1.0F))
                    image.Save(Path.Combine(slidesPath, thumbName), System.Drawing.Imaging.ImageFormat.Png);

                tblPPTSlide _slide = new tblPPTSlide();
                _slide.SlideName = slideName;
                _slide.Sequence = i + 1;
                _slide.MasterDocumentName = FileName;
                _entities.tblPPTSlides.Add(_slide);
                tblPPTSlide _slideOriginal = new tblPPTSlide();
                _slideOriginal.SlideName = slideName;
                _slideOriginal.Sequence = i + 1;
                _slideOriginal.MasterDocumentName = FileName;
                _slideOriginal.IsOriginal = true;
                _entities.tblPPTSlides.Add(_slideOriginal);
            }
            presentation.Dispose();
            _entities.SaveChanges();
        }

        public string SplitSlides(string FileName)
        {
            #region NO 




            string TempSlideName = string.Empty;
            string TempFileName = FileName;
            var TempThumbFileName = string.Empty;

            var savepptid = 1;
            string returnString = "success";
            string SlideName = string.Empty;
            string fileextenstion = '.' + FileName.Split('.').Last();

            int totalslidecount = 0;

            MemoryStream MainStreamSlideCount = new MemoryStream(System.IO.File.ReadAllBytes(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + FileName));

            MainStreamSlideCount.Seek(0, SeekOrigin.Begin);
            PresentationDocument presentationDocumentSlideCount = PresentationDocument.Open(MainStreamSlideCount, true);
            MainStreamSlideCount.Dispose();
            totalslidecount = CountSlides(presentationDocumentSlideCount);
            presentationDocumentSlideCount.Dispose();

            ////Spire
            //Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation();
            //presentation.LoadFromFile(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + FileName);
            //totalslidecount = presentation.Slides.Count;

            var exceptionMessage = string.Empty;


            for (int j = 0; j < totalslidecount; j++)
            {
                GC.Collect();

                MemoryStream MainStreamActualFile = new MemoryStream(System.IO.File.ReadAllBytes(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + FileName));
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

                string fileurl = System.IO.Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + FileName + "/" + SlideName);

                string tempSlideFileName = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + FileName + '/' + SlideName;
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


                GeneratePPTImage(SlideName, FileName, thumbfileName);

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
                savepptid++;

                GC.Collect();
            }

            _entities.SaveChanges();
            #endregion

            return returnString;
        }

        public string SplitTaskSlides(string FileName, int assignmentID, int assignedDocsID)
        {
            string TempSlideName = string.Empty;
            string TempFileName = FileName;
            var TempThumbFileName = string.Empty;

            var savepptid = 1;
            string returnString = "success";
            string SlideName = string.Empty;
            string fileextenstion = '.' + FileName.Split('.').Last();

            int totalslidecount = 0;

            string directoryName = Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, assignmentID.ToString() + "/PPT/" + FileName + "/");
            if (!System.IO.Directory.Exists(directoryName))
            {
                System.IO.Directory.CreateDirectory(directoryName);
            }

            MemoryStream MainStreamSlideCount = new MemoryStream(System.IO.File.ReadAllBytes(DirectoryManagmentUtils.TaskDocumentFilesPath + "/" + assignmentID.ToString() + "/" + FileName));

            MainStreamSlideCount.Seek(0, SeekOrigin.Begin);
            PresentationDocument presentationDocumentSlideCount = PresentationDocument.Open(MainStreamSlideCount, true);
            MainStreamSlideCount.Dispose();
            totalslidecount = CountSlides(presentationDocumentSlideCount);
            presentationDocumentSlideCount.Dispose();

            ////Spire
            //Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation();
            //presentation.LoadFromFile(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + FileName);
            //totalslidecount = presentation.Slides.Count;

            var exceptionMessage = string.Empty;

            for (int j = 0; j < totalslidecount; j++)
            {
                GC.Collect();

                MemoryStream MainStreamActualFile = new MemoryStream(System.IO.File.ReadAllBytes(DirectoryManagmentUtils.TaskDocumentFilesPath + "/" + assignmentID.ToString() + "/" + FileName));
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

                string fileurl = System.IO.Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, assignmentID.ToString() + "/PPT/" + FileName + "/" + SlideName);

                string tempSlideFileName = DirectoryManagmentUtils.TaskDocumentFilesPath + "/" + assignmentID.ToString() + "/PPT/" + FileName + '/' + SlideName;
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


                GenerateTaskPPTImage(SlideName, FileName, thumbfileName, assignmentID);



                tblAssignedPPTSlide objtblAssignedPPTSlide = new tblAssignedPPTSlide();
                objtblAssignedPPTSlide.AssignmentID = assignmentID;
                objtblAssignedPPTSlide.Sequence = savepptid;
                objtblAssignedPPTSlide.SlideName = SlideName;
                objtblAssignedPPTSlide.AssignedDocsID = assignedDocsID;
                objtblAssignedPPTSlide.IsTaskPPT = true;
                _entities.tblAssignedPPTSlides.Add(objtblAssignedPPTSlide);
                //tblPPTSlide _slideOriginal = new tblPPTSlide();
                //_slideOriginal.SlideName = SlideName;
                //_slideOriginal.Sequence = savepptid;
                //_slideOriginal.MasterDocumentName = FileName;
                //_slideOriginal.IsOriginal = true;
                //_entities.tblPPTSlides.Add(_slideOriginal);
                savepptid++;

                GC.Collect();
            }

            _entities.SaveChanges();


            return returnString;
        }

        public void GenerateTaskPPTImage(string SlideName, string FileName, string thumbfileName, int assignmentID)
        {
            string exceptionMessage = string.Empty;

            string fileurl = System.IO.Path.Combine(DirectoryManagmentUtils.TaskDocumentFilesPath, assignmentID.ToString() + "/PPT/" + FileName + "/" + SlideName);

            string tempSlideFileName = DirectoryManagmentUtils.TaskDocumentFilesPath + "/" + assignmentID.ToString() + "/PPT/" + FileName + '/' + SlideName;

            Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(fileurl);

            //save the slide to Image
            using (System.Drawing.Image image = presentation.Slides[0].GetThumbnail(1.0F, 1.0F))
                image.Save(DirectoryManagmentUtils.TaskDocumentFilesPath + "/" + assignmentID.ToString() + "/PPT/" + FileName + "/" + thumbfileName, System.Drawing.Imaging.ImageFormat.Png);
            presentation.Dispose();
        }

        public List<int> GetPagesWithAssignedContent(string FileName)
        {
            List<int> pages = new List<int>();
            var document = _entities.tblDocuments.FirstOrDefault(d => d.DocumentName == FileName);
            //var assignments = _entities.tblAssignments.Where(a => a.DocumentID == document.DocumentID && a.Action == 1).ToList();

            //if (assignments.Count > 0)
            //{
            //var replacementCodes = assignments.Select(a => a.ReplacementCode).ToList();

            var docPath = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathDemo, FileName);
            var doc = new Aspose.Words.Document(docPath);

            FindReplaceOptions options = new FindReplaceOptions();

            var replacingCallback = new ReplaceEvaluatorFindPage();
            options.ReplacingCallback = replacingCallback;

            Regex regex = new Regex(@"#S#\d+#", RegexOptions.IgnoreCase);
            doc.Range.Replace(regex, "", options);

            Regex regex1 = new Regex(@"#\d+#E#", RegexOptions.IgnoreCase);
            doc.Range.Replace(regex1, "", options);
            var pageNum = replacingCallback.pageNumbers;
            return pageNum;
            //if (!pages.Contains(pageNum)) pages.Add(pageNum);
            //}


            //return pages;
        }

        public class ReplaceEvaluatorFindPage : IReplacingCallback
        {
            /// <summary>
            /// This method is called by the Aspose.Words find and replace engine for each match.
            /// This method highlights the match string, even if it spans multiple runs.
            /// </summary>
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
            {
                if (pageNumbers == null)
                {
                    pageNumbers = new List<int>();
                }
                // This is a Run node that contains either the beginning or the complete match.
                Aspose.Words.Node currentNode = e.MatchNode;
                Aspose.Words.Document doc = (Aspose.Words.Document)currentNode.Document;
                LayoutCollector layout = new LayoutCollector(doc);
                pageNumber = layout.GetStartPageIndex(currentNode);
                if (!pageNumbers.Contains(pageNumber)) pageNumbers.Add(pageNumber);

                return ReplaceAction.Replace;
            }

            public int pageNumber;
            public List<int> pageNumbers;
        }

        public void SplitPages(string FileName)
        {
            var thumbnailDir = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathWords, "Pages_" + FileName);
            if (!Directory.Exists(thumbnailDir))
            {
                Directory.CreateDirectory(thumbnailDir);
            }

            string fileurl = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathDemo, FileName);
            //var currentThumbs = new DirectoryInfo(thumbnailDir).GetFiles("*.png").Where(f => f.Extension == ".png");
            //foreach (var thumb in currentThumbs)
            //{
            //    System.IO.File.Delete(thumb.FullName);
            //}

            //Aspose.Words.Document document = new Aspose.Words.Document(fileurl);

            //var pageCount = document.PageCount;

            //for (var i = 0; i < pageCount; i++)
            //{
            //    GeneratePageImage(i, document, thumbnailDir);
            //}

        }

        public void GeneratePageImage(int pageNumber, Aspose.Words.Document document, string thumbDir, int AssignmentID = 0)
        {
            ImageSaveOptions options = new ImageSaveOptions(Aspose.Words.SaveFormat.Png);
            options.PageIndex = pageNumber;
            options.PageCount = 1;

            using (var tempStream = new MemoryStream())
            {
                //Split pdf pages and then save them as png files
                document.Save(tempStream, SaveFormat.Pdf);
                PdfFileEditor pdfEditor = new PdfFileEditor();

                MemoryStream[] outBuffer = pdfEditor.SplitToPages(tempStream);

                //int pageCount = 1;
                MemoryStream aStream = outBuffer[pageNumber];

                var pagePdf = new Aspose.Pdf.Document(aStream);

                Aspose.Pdf.Devices.Resolution resolution = new Resolution(300);
                Aspose.Pdf.Devices.PngDevice device = new Aspose.Pdf.Devices.PngDevice(resolution);

                var page = pagePdf.Pages[1];

                using (var msImage = new MemoryStream())
                {
                    device.Process(page, msImage);
                    Image img = System.Drawing.Image.FromStream(msImage);
                    if (AssignmentID > 0)
                    {
                        img.Save(Path.Combine(thumbDir, AssignmentID.ToString() + "-" + pageNumber.ToString() + ".png"), ImageFormat.Png);
                    }
                    else
                    {
                        img.Save(Path.Combine(thumbDir, pageNumber.ToString() + ".png"), ImageFormat.Png);
                    }
                }
            }

            //using (MemoryStream imgStream = new MemoryStream())
            //{
            //    document.Save(imgStream, options);

            //    // Insert the image stream into a temporary Document instance.
            //    Aspose.Words.Document temp = new Aspose.Words.Document();
            //    Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(temp);
            //    Aspose.Words.Drawing.Shape img = builder.InsertImage(imgStream);

            //    // Save the individual image to disk using ShapeRenderer class
            //    ShapeRenderer renderer = img.GetShapeRenderer();

            //    if (AssignmentID != 0)
            //    {
            //        renderer.Save(Path.Combine(thumbDir, AssignmentID.ToString() + "-" + pageNumber.ToString() + ".png"), new ImageSaveOptions(Aspose.Words.SaveFormat.Png));
            //    }
            //    else
            //    {
            //        renderer.Save(Path.Combine(thumbDir, pageNumber.ToString() + ".png"), new ImageSaveOptions(Aspose.Words.SaveFormat.Png));
            //    }
            //}
        }

        public void GeneratePPTImage(string SlideName, string FileName, string thumbfileName)
        {
            string exceptionMessage = string.Empty;
            string fileurl = System.IO.Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "\\" + "Slides_" + FileName + "\\" + SlideName);
            //Spire

            Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(fileurl);

            //save the slide to Image
            using (System.Drawing.Image image = presentation.Slides[0].GetThumbnail(1.0F, 1.0F))
                image.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "\\" + "Slides_" + FileName + "\\" + thumbfileName, System.Drawing.Imaging.ImageFormat.Png);
            presentation.Dispose();
        }

        public int SplitCompletedSlides(string OriginalFile, string FileName, int StartIndex, int StartSeq, string originalSlideName)
        {
            int FirstEntryID = 0;

            var savepptid = StartIndex;
            string fileName = string.Empty;
            string fileextenstion = '.' + FileName.Split('.').Last();
            int totalslidecount = 0;

            using (MemoryStream MainStreamSlideCount = new MemoryStream(System.IO.File.ReadAllBytes(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + OriginalFile + "/AssignedDoc/" + FileName)))
            {
                MainStreamSlideCount.Seek(0, SeekOrigin.Begin);
                using (PresentationDocument presentationDocumentSlideCount = PresentationDocument.Open(MainStreamSlideCount, true))
                {
                    totalslidecount = CountSlides(presentationDocumentSlideCount);
                }
            }

            for (int j = 0; j < totalslidecount; j++)
            {
                //using (MemoryStream MainStreamActualFile = new MemoryStream(System.IO.File.ReadAllBytes(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + FileName + '/' + FileName)))
                using (MemoryStream MainStreamActualFile = new MemoryStream(System.IO.File.ReadAllBytes(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + OriginalFile + "/AssignedDoc/" + FileName)))
                {
                    MainStreamActualFile.Seek(0, SeekOrigin.Begin);
                    DeleteSlide(MainStreamActualFile, j);
                    MainStreamActualFile.Seek(0, SeekOrigin.Begin);

                    fileName = savepptid + fileextenstion;
                    var thumbfileName = savepptid + WordAutomationDemo.Common.Global.ImageExportExtention;

                    //fileName = FileName.Substring(0, FileName.Length - minusfilelength) + "-" + savepptid + fileextenstion;
                    string fileurl = System.IO.Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + OriginalFile + "/" + fileName).Replace(@"\\\", "/");

                    string tempSlideFileName = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + OriginalFile + '/' + fileName;
                    if (System.IO.File.Exists(tempSlideFileName))
                    {
                        System.IO.File.Delete(tempSlideFileName);
                    }
                    using (var fs1 = new FileStream(tempSlideFileName, FileMode.OpenOrCreate))
                    {
                        MainStreamActualFile.CopyTo(fs1);
                    }


                    using (MemoryStream MainStreamSaveFile = new MemoryStream(System.IO.File.ReadAllBytes(tempSlideFileName)))
                    {

                        System.IO.File.WriteAllBytes(fileurl, MainStreamSaveFile.ToArray());


                        //get thumbnails 


                        //Application pptApplication = new Application();
                        //Microsoft.Office.Interop.PowerPoint.Presentation pptPresentation = pptApplication.Presentations.Open(fileurl, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);
                        //pptPresentation.Slides[1].Export(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + OriginalFile + "/" + thumbfileName, WordAutomationDemo.Common.Global.ImageExportExtention, 800, 600);
                        //pptPresentation.Close();

                        //Spire 


                        Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation(fileurl);
                        //save the slide to Image
                        using (System.Drawing.Image image = presentation.Slides[0].GetThumbnail(1.0F, 1.0F))
                            image.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + OriginalFile + "/" + thumbfileName, System.Drawing.Imaging.ImageFormat.Png);
                        presentation.Dispose();


                        tblPPTSlide _slide = new tblPPTSlide();
                        _slide.SlideName = fileName;
                        _slide.Sequence = StartSeq;
                        _slide.MasterDocumentName = OriginalFile;
                        _entities.tblPPTSlides.Add(_slide);
                        _entities.SaveChanges();

                        if (j == 0)
                        {
                            FirstEntryID = _slide.PPTSlidesID;
                        }

                        savepptid++;
                        StartSeq++;
                    }


                }
            }
            var OriginalFilePath = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + OriginalFile + '/' + originalSlideName;
            var ThumbFilePath = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + OriginalFile + "/" + originalSlideName.Split('.').FirstOrDefault().ToString() + WordAutomationDemo.Common.Global.ImageExportExtention;

            if (System.IO.File.Exists(OriginalFilePath))
                System.IO.File.Delete(OriginalFilePath); // delete old slides 

            if (System.IO.File.Exists(ThumbFilePath))
                System.IO.File.Delete(ThumbFilePath); // delete thumbnails of old slides 

            var _slideToDelete = _entities.tblPPTSlides.Where(z => z.SlideName == originalSlideName && z.MasterDocumentName == OriginalFile && !(z.IsOriginal == true)).FirstOrDefault();
            _entities.tblPPTSlides.Remove(_slideToDelete);
            _entities.SaveChanges();
            return FirstEntryID;
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


        private bool GrantAccess(string fullPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(fullPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
            return true;
        }

        uint uniqueId = 0;
        uint id = 0;
        int mergedSideCount = 0;
        bool OldslidesExists = true;

        public string MergeFiles(List<string> FileList, string OriginalFileName)
        {
            string fileName = string.Empty;
            var fileInfoMain = System.IO.File.Open(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + OriginalFileName, FileMode.Open);


            using (MemoryStream MainStream = new MemoryStream())
            {
                fileInfoMain.CopyTo(MainStream);
                foreach (var item in FileList)
                {
                    var fileInfoChild = System.IO.File.Open(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + OriginalFileName + "/" + item, FileMode.Open);
                    using (MemoryStream ChildStream = new MemoryStream())
                    {
                        fileInfoChild.CopyTo(ChildStream);

                        ChildStream.Seek(0, SeekOrigin.Begin);

                        MainStream.Seek(0, SeekOrigin.Begin);

                        MergeSlidesStream(ChildStream, MainStream);

                        //fileInfoChild.Close();
                    }
                }


                MainStream.Seek(0, SeekOrigin.Begin);

                fileName = OriginalFileName;
                fileInfoMain.Close();
                string fileurl = System.IO.Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs, OriginalFileName).Replace(@"\\\", "/");
                if (System.IO.File.Exists(fileurl))
                {
                    System.IO.File.Delete(fileurl);
                }
                System.IO.File.WriteAllBytes(fileurl, MainStream.ToArray());



                fileInfoMain.Close();

            }

            //}

            return fileName;
        }

        public void MergeSlidesStream(MemoryStream MemoryStream2, MemoryStream MemoryStream1)
        {
            using (PresentationDocument destinationPresentationDoc = PresentationDocument.Open(MemoryStream1, true))
            {
                PresentationPart destPresPart = destinationPresentationDoc.PresentationPart;
                // If the merged presentation doesn't have a SlideIdList element yet then add it.
                if (destPresPart.Presentation.SlideIdList == null)
                    destPresPart.Presentation.SlideIdList = new SlideIdList();

                if (OldslidesExists)
                {
                    destPresPart.Presentation.SlideIdList.RemoveAllChildren();
                    OldslidesExists = false;
                }
                // Open the source presentation. This will throw an exception if the source presentation does not exist.
                using (PresentationDocument sourcePresentationDoc = PresentationDocument.Open(MemoryStream2, false))
                {
                    PresentationPart sourcePresPart = sourcePresentationDoc.PresentationPart;

                    // Get unique ids for the slide master and slide lists for use later.
                    uniqueId = GetMaxSlideMasterId(destPresPart.Presentation.SlideMasterIdList);
                    uint maxSlideId = GetMaxSlideId(destPresPart.Presentation.SlideIdList);

                    // Copy each slide in the source presentation in order to the destination presentation.
                    foreach (SlideId slideId in sourcePresPart.Presentation.SlideIdList)
                    {
                        SlidePart sp;
                        SlidePart destSp;
                        SlideMasterPart destMasterPart;
                        string relId;
                        SlideMasterId newSlideMasterId;
                        SlideId newSlideId;

                        //increase the slide count
                        mergedSideCount++;

                        // Create a unique relationship id.
                        id = maxSlideId + 1;
                        id++;
                        sp = (SlidePart)sourcePresPart.GetPartById(slideId.RelationshipId);
                        //relId = "uniq" + id;
                        relId = "uniq" + DateTime.Now.Ticks.ToString();
                        // Add the slide part to the destination presentation.
                        destSp = destPresPart.AddPart<SlidePart>(sp, relId);

                        // The master part was added. Make sure the relationship is in place.
                        destMasterPart = destSp.SlideLayoutPart.SlideMasterPart;
                        destPresPart.AddPart(destMasterPart);

                        // Add slide master to slide master list.
                        uniqueId++;
                        newSlideMasterId = new SlideMasterId();
                        newSlideMasterId.RelationshipId = destPresPart.GetIdOfPart(destMasterPart);
                        newSlideMasterId.Id = uniqueId;

                        // Add slide to slide list.
                        maxSlideId++;
                        newSlideId = new SlideId();
                        newSlideId.RelationshipId = relId;
                        newSlideId.Id = maxSlideId;

                        destPresPart.Presentation.SlideMasterIdList.Append(newSlideMasterId);
                        destPresPart.Presentation.SlideIdList.Append(newSlideId);

                    }

                    // Make sure all slide ids are unique.
                    ModifySlideLayoutIds(destPresPart);
                }

                // Save the changes to the destination presentation.
                destPresPart.Presentation.Save();

            }


            //string fileUrl = "/SPdemo2/Shared Documents/pptTest1.pptx";
            //MemoryStream1.Seek(0, SeekOrigin.Begin);
            //Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, fileUrl, MemoryStream1, true);

        }

        public uint GetMaxSlideMasterId(SlideMasterIdList slideMasterIdList)
        {
            // Slide master identifiers have a minimum value of greater than or equal to 2147483648. 
            uint max = 2147483648;

            if (slideMasterIdList != null)
                // Get the maximum id value from the current set of children.
                foreach (SlideMasterId child in slideMasterIdList.Elements<SlideMasterId>())
                {
                    uint id = child.Id;

                    if (id > max)
                        max = id;
                }

            return max;
        }

        public uint GetMaxSlideId(SlideIdList slideIdList)
        {
            // Slide identifiers have a minimum value of greater than or equal to 256
            // and a maximum value of less than 2147483648. 
            uint max = 256;

            if (slideIdList != null)
                // Get the maximum id value from the current set of children.
                foreach (SlideId child in slideIdList.Elements<SlideId>())
                {
                    uint id = child.Id;

                    if (id > max)
                        max = id;
                }

            return max;
        }

        public void ModifySlideLayoutIds(PresentationPart presPart)
        {
            // Make sure all slide layouts have unique ids.
            foreach (SlideMasterPart slideMasterPart in presPart.SlideMasterParts)
            {
                foreach (SlideLayoutId slideLayoutId in slideMasterPart.SlideMaster.SlideLayoutIdList)
                {
                    uniqueId++;
                    slideLayoutId.Id = (uint)uniqueId;
                }

                slideMasterPart.SlideMaster.Save();
            }
        }

        public ActionResult MyTaskGrid_Actions()
        {
            List<string> Actions = new List<string>();
            Actions.Add("Edit Action");
            Actions.Add("Completed");
            Actions.Add("Approved/Decline Action");
            Actions.Add("Approved");
            Actions.Add("Published");
            Actions.Add("Reassign");
            Actions.Add("Decline");
            return Json(Actions, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpPost]
        public int BulkApproveTasks(string IDs)
        {
            int Status = 0;
            int UserID = CurrentUserSession.UserID;
            int Completed = Convert.ToInt32(Global.Action.Completed);
            int Approved = Convert.ToInt32(Global.Action.Approved);
            int Published = Convert.ToInt32(Global.Action.Published);
            int DocumentType = 0;
            var OrginalSourceFile = "";
            int Word = Convert.ToInt32(Global.DocumentType.Word);
            int Ppt = Convert.ToInt32(Global.DocumentType.Ppt);
            int Xls = Convert.ToInt32(Global.DocumentType.Xls);

            int[] TaskIDs = Array.ConvertAll(IDs.Split(','), int.Parse);

            foreach (var AssignmentID in TaskIDs)
            {
                try
                {
                    tblAssignment _tblAssignment = _entities.tblAssignments.Where(c => c.AssignmentID == AssignmentID && c.CreatedBy == UserID && c.Action == Completed).FirstOrDefault();
                    if (_tblAssignment != null)
                    {
                        if (_tblAssignment.tblDocument != null)
                        {
                            OrginalSourceFile = _tblAssignment.tblDocument.DocumentName;
                        }
                        DocumentType = _tblAssignment.DocumentType.HasValue ? _tblAssignment.DocumentType.Value : 0;
                        if (Convert.ToInt32(_tblAssignment.DocumentType) == Word)
                        {
                            var document = _tblAssignment.DocumentFile;
                            var ReplacementCode = _tblAssignment.ReplacementCode;

                            _tblAssignment.Action = Approved;
                            _entities.Entry(_tblAssignment).State = EntityState.Modified;

                            //Add Log 
                            tblAssignmentLog _log = new tblAssignmentLog();
                            _log.Action = Approved;
                            _log.AssignmentID = _tblAssignment.AssignmentID;
                            _log.CreatedDate = DateTime.Now;
                            _log.CreatedBy = UserID;
                            _log.DocumentName = document;
                            _entities.tblAssignmentLogs.Add(_log);
                            //
                            _entities.SaveChanges();

                            // extract text from snippet
                            RichEditDocumentServer server = new RichEditDocumentServer();
                            server.CreateNewDocument(); // create new temp document virtually
                            server.Document.InsertDocumentContent(server.Document.Range.Start, DirectoryManagmentUtils.InitialDemoFilesPath + "/" + document, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Snippet to the temp document
                            DevExpress.XtraRichEdit.API.Native.Document newdocuments = server.Document;

                            CharacterProperties cp = server.Document.BeginUpdateCharacters(server.Document.Range);
                            cp.ForeColor = System.Drawing.Color.Black; // convert all text to black color
                            server.Document.EndUpdateCharacters(cp);

                            HtmlDocumentExporterOptions htm = new HtmlDocumentExporterOptions();
                            htm.CssPropertiesExportType = DevExpress.XtraRichEdit.Export.Html.CssPropertiesExportType.Inline;
                            var HTML = server.Document.GetHtmlText(server.Document.Range, null, htm);

                            HtmlDocument docs = new HtmlDocument();
                            docs.LoadHtml(HTML);
                            foreach (HtmlNode link in docs.DocumentNode.SelectNodes("//span[@style]"))
                            {
                                HtmlAttribute att = link.Attributes["style"];
                                if (att.Value.Contains("text-decoration: line-through;"))
                                {
                                    link.Remove();
                                }
                            }

                            var textToReplace = docs.DocumentNode.WriteTo();
                            DocumentManager.CloseAllDocuments();
                            server.Dispose();

                            server = new RichEditDocumentServer();
                            server.LoadDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + OrginalSourceFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                            DevExpress.XtraRichEdit.API.Native.Document documents = server.Document;
                            //DocumentRange ranges = server.Document.FindAll(doc.ReplacementCode, SearchOptions.WholeWord).FirstOrDefault();
                            //var CodeToFindWithSpace = Characters.Space.ToString() + ReplacementCode + Characters.Space.ToString();
                            //DocumentRange ranges = server.Document.FindAll(CodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();


                            var CodeToFindWithSpace = Characters.Space.ToString() + "#S" + ReplacementCode + Characters.Space.ToString();
                            DocumentRange rangesStart = server.Document.FindAll(CodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                            var EndCodeToFindWithSpace = Characters.Space.ToString() + ReplacementCode + "E#" + Characters.Space.ToString();
                            DocumentRange rangesEnd = server.Document.FindAll(EndCodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                            DocumentRange ranges = null;
                            if (rangesStart != null && rangesEnd != null)
                            {
                                var totalLength = rangesEnd.End.ToInt() - rangesStart.Start.ToInt();
                                ranges = server.Document.CreateRange(rangesStart.Start, totalLength);
                            }

                            if (ranges != null)
                            {
                                server.Document.Replace(ranges, ""); // remove All Strings in range
                                server.Document.InsertHtmlText(ranges.Start, textToReplace, InsertOptions.MatchDestinationFormatting); // insert Snippet to the marker position
                                server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + OrginalSourceFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // save the doc 
                            }
                            else
                            {
                                //DocumentRange rangesNoSpace = server.Document.FindAll(ReplacementCode, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                                var CodeToFindWithNoSpace = "#S" + ReplacementCode;
                                DocumentRange NoSpacerangesStart = server.Document.FindAll(CodeToFindWithNoSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                                var EndCodeToFindWithNoSpace = ReplacementCode + "E#";
                                DocumentRange NoSpacerangesEnd = server.Document.FindAll(EndCodeToFindWithNoSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                                DocumentRange rangesNoSpace = null;
                                if (NoSpacerangesStart != null && NoSpacerangesEnd != null)
                                {
                                    var totalLengthNoSpace = NoSpacerangesEnd.End.ToInt() - NoSpacerangesStart.Start.ToInt();
                                    rangesNoSpace = server.Document.CreateRange(NoSpacerangesStart.Start, totalLengthNoSpace);
                                }

                                if (rangesNoSpace != null)
                                {
                                    server.Document.Replace(rangesNoSpace, ""); // remove 
                                    server.Document.InsertHtmlText(rangesNoSpace.Start, textToReplace, InsertOptions.MatchDestinationFormatting); // insert Snippet to the marker position
                                    server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + OrginalSourceFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // save the doc 
                                }
                            }

                            DocumentManager.CloseAllDocuments();
                            server.Dispose();


                            //Update Master Copy

                            //Make a Temp Document
                            server = new RichEditDocumentServer();
                            server.CreateNewDocument();
                            server.Document.InsertDocumentContent(server.Document.Range.Start, DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + OrginalSourceFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Orignal Document to the temp document

                            var ListofAssignments = GetPendingAssignedTasks(OrginalSourceFile);
                            if (ListofAssignments != null && ListofAssignments.Count > 0)
                            {
                                foreach (var item in ListofAssignments)
                                {
                                    //var CodeToFindWithSpace2 = Characters.Space.ToString() + item.ReplacementCode + Characters.Space.ToString();
                                    //DocumentRange ranges2 = server.Document.FindAll(item.ReplacementCode, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();




                                    var CodeToFindWithSpace2 = Characters.Space.ToString() + "#S" + ReplacementCode + Characters.Space.ToString();
                                    DocumentRange rangesStart2 = server.Document.FindAll(CodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                                    var EndCodeToFindWithSpace2 = Characters.Space.ToString() + ReplacementCode + "E#" + Characters.Space.ToString();
                                    DocumentRange rangesEnd2 = server.Document.FindAll(EndCodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                                    DocumentRange ranges2 = null;
                                    if (rangesStart2 != null && rangesEnd2 != null)
                                    {
                                        var totalLength2 = rangesEnd2.End.ToInt() - rangesStart2.Start.ToInt();
                                        ranges2 = server.Document.CreateRange(rangesStart2.Start, totalLength2);
                                    }

                                    if (ranges2 != null)
                                    {
                                        server.Document.ReplaceAll(item.ReplacementCode, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord); // remove timestamp
                                        server.Document.InsertDocumentContent(ranges2.Start, DirectoryManagmentUtils.InitialDemoFilesPath + "/" + "Copy_" + item.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Snippet to the temp document
                                    }
                                    else
                                    {
                                        //DocumentRange rangesNoSpace2 = server.Document.FindAll(item.ReplacementCode, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                                        var CodeToFindWithNoSpace2 = "#S" + ReplacementCode;
                                        DocumentRange NoSpacerangesStart2 = server.Document.FindAll(CodeToFindWithNoSpace2, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                                        var EndCodeToFindWithNoSpace2 = ReplacementCode + "E#";
                                        DocumentRange NoSpacerangesEnd2 = server.Document.FindAll(EndCodeToFindWithNoSpace2, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                                        DocumentRange rangesNoSpace2 = null;
                                        if (NoSpacerangesStart2 != null && NoSpacerangesEnd2 != null)
                                        {
                                            var totalLengthNoSpace2 = NoSpacerangesEnd2.End.ToInt() - NoSpacerangesStart2.Start.ToInt();
                                            rangesNoSpace2 = server.Document.CreateRange(NoSpacerangesStart2.Start, totalLengthNoSpace2);
                                        }

                                        if (rangesNoSpace2 != null)
                                        {
                                            server.Document.ReplaceAll(item.ReplacementCode, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord); // remove timestamp
                                            server.Document.InsertDocumentContent(rangesNoSpace2.Start, DirectoryManagmentUtils.InitialDemoFilesPath + "/" + "Copy_" + item.DocumentName, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert Snippet to the temp document
                                        }
                                    }

                                }
                            }
                            var FinalContent = server.Document.HtmlText;
                            server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + OrginalSourceFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // Saves with same name causes the replacement
                            DocumentManager.CloseAllDocuments();
                            server.Dispose();
                        }
                        else if (Convert.ToInt32(_tblAssignment.DocumentType) == Ppt)
                        {
                            var AssignedSlides = (from items in _entities.tblAssignedPPTSlides.AsEnumerable()
                                                  join q in _entities.tblAssignments.AsEnumerable() on items.AssignmentID equals q.AssignmentID
                                                  join doc in _entities.tblDocuments on q.DocumentID equals doc.DocumentID
                                                  where items.AssignmentID == AssignmentID && items.IsPPTApproved == null
                                                  orderby items.Sequence
                                                  select new PPTModel()
                                                  {
                                                      AssignedPPTSlideID = items.AssignedPPTSildeID,
                                                      AssignedTaskID = items.AssignmentID,
                                                      OriginalFile = doc.DocumentName,
                                                      OriginalSlideName = items.SlideName,
                                                      SlideName = "New_" + items.AssignmentID.ToString() + "_Copy_" + items.SlideName
                                                  }).ToList();

                            int LastEntryForAll = Convert.ToInt32(_entities.tblPPTSlides.Where(c => c.MasterDocumentName == OrginalSourceFile && !(c.IsOriginal == true)).OrderByDescending(c => c.PPTSlidesID).Select(c => c.PPTSlidesID).FirstOrDefault());

                            foreach (var item in AssignedSlides)
                            {
                                var LastFile = _entities.tblPPTSlides.Where(c => c.MasterDocumentName == OrginalSourceFile && !(c.IsOriginal == true)).OrderByDescending(c => c.PPTSlidesID).FirstOrDefault();

                                var MyOriFile = _entities.tblPPTSlides.Where(c => c.MasterDocumentName == OrginalSourceFile && c.SlideName == item.OriginalSlideName && !(c.IsOriginal == true)).FirstOrDefault();
                                int MyFileSeq = Convert.ToInt32(MyOriFile.Sequence);

                                int LastSlide = Convert.ToInt32(LastFile.SlideName.Split('.')[0]);

                                int FirstEntryID = SplitCompletedSlides(item.OriginalFile, item.SlideName, LastSlide + 1, MyFileSeq, item.OriginalSlideName);

                                //Reinitiate Sequence
                                var InBetweenLst = _entities.tblPPTSlides.Where(c => c.MasterDocumentName == OrginalSourceFile && !(c.IsOriginal == true) && (c.PPTSlidesID < FirstEntryID && c.PPTSlidesID > MyOriFile.PPTSlidesID) && c.PPTSlidesID <= LastEntryForAll).ToList();

                                int GetLastSeq = Convert.ToInt32(_entities.tblPPTSlides.Where(c => c.MasterDocumentName == OrginalSourceFile && !(c.IsOriginal == true)).OrderByDescending(c => c.PPTSlidesID).Select(c => c.Sequence).FirstOrDefault());

                                foreach (var inbet in InBetweenLst)
                                {
                                    GetLastSeq++;

                                    inbet.Sequence = GetLastSeq;
                                }
                                _entities.SaveChanges();

                                var ppt = (from itm in _entities.tblAssignedPPTSlides
                                           where itm.AssignedPPTSildeID == item.AssignedPPTSlideID
                                           select itm).FirstOrDefault();

                                if (ppt != null)
                                {
                                    ppt.IsPPTApproved = true;
                                    _entities.Entry(ppt).State = System.Data.Entity.EntityState.Modified;
                                    _entities.SaveChanges();
                                }
                            }

                            //Mark Task as Merged 
                            _tblAssignment.Action = Approved;
                            _tblAssignment.CompletedDate = DateTime.Now;
                            _entities.Entry(_tblAssignment).State = EntityState.Modified;

                            //Add Log 
                            tblAssignmentLog _log = new tblAssignmentLog();
                            _log.Action = Approved;
                            _log.AssignmentID = AssignmentID;
                            _log.CreatedDate = DateTime.Now;
                            _log.CreatedBy = CurrentUserSession.UserID;
                            _log.DocumentName = OrginalSourceFile;
                            _entities.tblAssignmentLogs.Add(_log);

                            //// Merge PPTs to Master  Doc & Regenrate PPT file 
                            //var allSlides = (from p in _entities.tblPPTSlides
                            //                 where p.MasterDocumentName == OrginalSourceFile
                            //                 orderby p.Sequence
                            //                 select p.SlideName).ToList();

                            //MergeFiles(allSlides, OrginalSourceFile);

                            _entities.SaveChanges();
                        }
                        else if (Convert.ToInt32(_tblAssignment.DocumentType) == Xls)
                        {
                            var AssignedSheets = (from items in _entities.tblAssignedExcelSheets.AsEnumerable()
                                                  join q in _entities.tblAssignments.AsEnumerable() on items.AssignmentID equals q.AssignmentID
                                                  join docu in _entities.tblDocuments on q.DocumentID equals docu.DocumentID
                                                  where items.AssignmentID == AssignmentID && items.IsSheetModified == true
                                                  && !(items.IsSheetApproved == true)
                                                  orderby items.Sequence
                                                  select new ExcelModel()
                                                  {
                                                      AssignedTaskID = items.AssignmentID,
                                                      OriginalFile = docu.DocumentName,
                                                      OriginalSheetName = items.SheetName,
                                                      SheetName = "New_" + items.AssignmentID.ToString() + "_Copy_" + items.SheetName,
                                                      IsSheetApproved = items.IsSheetApproved,
                                                      AssignedExcelSheetID = items.AssignedExcelSheetID,
                                                  }).ToList();


                            int LastEntryForAll = Convert.ToInt32(_entities.tblExcelSheets.Where(c => c.MasterDocumentName == OrginalSourceFile && !(c.IsOriginal == true)).OrderByDescending(c => c.Sequence).Select(c => c.ExcelSheetID).FirstOrDefault());

                            Workbook WorkbookSplit = new Workbook();

                            //foreach (var item in AssignedSheets)
                            //{
                            //var LastFile = _readyPortalDBEntities.tblExcelSheets.Where(c => c.MasterDocumentName == document && !(c.IsOriginal == true)).OrderByDescending(c => c.ExcelSheetID).FirstOrDefault();

                            //var MyOriFile = _readyPortalDBEntities.tblExcelSheets.Where(c => c.MasterDocumentName == document && c.SheetName == item.OriginalSheetName && !(c.IsOriginal == true)).FirstOrDefault();

                            //int MyFileSeq = Convert.ToInt32(MyOriFile.Sequence);

                            //int LastSlide = Convert.ToInt32(LastFile.SheetName.Replace("Sheet", "").Split('.')[0]);

                            ////Reinitiate Sequence
                            //var InBetweenLst = _readyPortalDBEntities.tblExcelSheets.Where(c => c.MasterDocumentName == document && !(c.IsOriginal == true) && (c.Sequence > MyFileSeq)).OrderBy(a => a.Sequence).ToList();

                            Workbook workbook = new Workbook();
                            using (FileStream stream = new FileStream(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + OrginalSourceFile, FileMode.Open))
                            {
                                workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                                //int worksheetIndex = Convert.ToInt32(item.OriginalSheetName.Replace("Sheet", "").Split('.')[0]) - 1;
                                int workSheetCount = 0;
                                for (int k = 0; k < workbook.Worksheets.Count(); k++)
                                {
                                    var sheet = AssignedSheets.Where(s => s.OriginalSheetName.ToLower() == workbook.Worksheets[k].Name.ToLower() + ".xlsx").FirstOrDefault();
                                    if (sheet != null)
                                    {
                                        var fileurl = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + OrginalSourceFile + "/AssignedDoc/" + ("New_" + AssignmentID.ToString() + "_Copy_" + sheet.OriginalSheetName);
                                        if (System.IO.File.Exists(fileurl))
                                        {
                                            using (FileStream streamNew = new FileStream(fileurl, FileMode.Open))
                                            {
                                                Workbook WorkbookNew = new Workbook();
                                                WorkbookNew.LoadDocument(streamNew, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                                                for (int j = 0; j < WorkbookNew.Worksheets.Count(); j++)
                                                {
                                                    if (workSheetCount == 0)
                                                    {
                                                        WorkbookSplit.Worksheets[workSheetCount].Name = "Sheet";
                                                    }
                                                    WorkbookSplit.Worksheets.Add("Sheet" + (workSheetCount + 1));
                                                    WorkbookSplit.Worksheets["Sheet" + (workSheetCount + 1)].CopyFrom(WorkbookNew.Worksheets[j]);
                                                    workSheetCount++;
                                                    //tblExcelSheet _sheet = new tblExcelSheet();
                                                    //_sheet.SheetName = "Sheet" + (workSheetCount + 1) + ".xlsx";
                                                    //_sheet.Sequence = StartSeq;
                                                    //_sheet.MasterDocumentName = OriginalFile;
                                                    //_entities.tblExcelSheets.Add(_sheet);
                                                    //_entities.SaveChanges();

                                                    //if (j == 0)
                                                    //{
                                                    //    FirstEntryID = _sheet.ExcelSheetID;
                                                    //}

                                                    //savepptid++;
                                                    //StartSeq++;
                                                }
                                            }
                                            int assignedExcelSheetID = sheet.AssignedExcelSheetID;
                                            var excel = (from itm in _entities.tblAssignedExcelSheets
                                                         where itm.AssignedExcelSheetID == assignedExcelSheetID
                                                         select itm).FirstOrDefault();

                                            if (excel != null)
                                            {
                                                excel.IsSheetApproved = true;
                                                _entities.Entry(excel).State = System.Data.Entity.EntityState.Modified;
                                                _entities.SaveChanges();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (workSheetCount == 0)
                                        {
                                            WorkbookSplit.Worksheets[workSheetCount].Name = "Sheet";
                                        }
                                        WorkbookSplit.Worksheets.Add("Sheet" + (workSheetCount + 1));
                                        WorkbookSplit.Worksheets["Sheet" + (workSheetCount + 1)].CopyFrom(workbook.Worksheets[k]);
                                        workSheetCount++;
                                    }
                                }
                                stream.Dispose();
                                stream.Close();
                                if (System.IO.File.Exists(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + OrginalSourceFile))
                                {
                                    System.IO.File.Delete(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + OrginalSourceFile);
                                }
                                WorkbookSplit.Worksheets.RemoveAt(0);
                                WorkbookSplit.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + OrginalSourceFile);

                                //int FirstEntryID = SplitCompletedSheets(item.OriginalFile, item.SheetName, LastSlide + 1, MyFileSeq, item.OriginalSheetName, AssignID);

                                //int GetLastSeq = Convert.ToInt32(_readyPortalDBEntities.tblExcelSheets.Where(c => c.MasterDocumentName == document && !(c.IsOriginal == true)).OrderByDescending(c => c.ExcelSheetID).Select(c => c.Sequence).FirstOrDefault());

                                //foreach (var inbet in InBetweenLst)
                                //{
                                //    GetLastSeq++;

                                //    inbet.Sequence = GetLastSeq;
                                //}
                            }

                            //Mark Task as Merged 
                            _tblAssignment.Action = Approved;
                            _tblAssignment.CompletedDate = DateTime.Now;
                            _entities.Entry(_tblAssignment).State = EntityState.Modified;

                            //Add Log 
                            tblAssignmentLog _log = new tblAssignmentLog();
                            _log.Action = Approved;
                            _log.AssignmentID = AssignmentID;
                            _log.CreatedDate = DateTime.Now;
                            _log.CreatedBy = CurrentUserSession.UserID;
                            _log.DocumentName = OrginalSourceFile;
                            _entities.tblAssignmentLogs.Add(_log);

                            //// Merge PPTs to Master  Doc & Regenrate PPT file 
                            //var allSlides = (from p in _entities.tblPPTSlides
                            //                 where p.MasterDocumentName == OrginalSourceFile
                            //                 orderby p.Sequence
                            //                 select p.SlideName).ToList();

                            //MergeFiles(allSlides, OrginalSourceFile);

                            _entities.SaveChanges();
                        }
                        else
                        {
                            //Mark Task as Merged 
                            _tblAssignment.Action = Approved;
                            _tblAssignment.CompletedDate = DateTime.Now;
                            _entities.Entry(_tblAssignment).State = EntityState.Modified;

                            //Add Log 
                            tblAssignmentLog _log = new tblAssignmentLog();
                            _log.Action = Approved;
                            _log.AssignmentID = AssignmentID;
                            _log.CreatedDate = DateTime.Now;
                            _log.CreatedBy = CurrentUserSession.UserID;
                            _log.DocumentName = OrginalSourceFile;
                            _entities.tblAssignmentLogs.Add(_log);

                            _entities.SaveChanges();
                        }
                        var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                        AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                        objAssignmentModelEmail.TaskName = _tblAssignment.TaskName;
                        objAssignmentModelEmail.AssignID = _tblAssignment.AssignmentID;
                        List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                        lstAssignmentLogModel = GetAssignmentLog(_tblAssignment.AssignmentID);
                        string EmailBody = templates.TemplateContentForEmail;
                        foreach (var userToNotify in _tblAssignment.tblAssignmentMembers.Select(m => m.tblUserDepartment))
                        {
                            templates.Subject = templates.Subject.Replace("#Subject#", "Task Approved: Task has been approved by " + CurrentUserSession.User.FirstName);
                            EmailBody = PopulateEmailBody("A task has been approved by " + CurrentUserSession.User.FirstName, userToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody);
                            byte[] fileBytes = null;

                            if (Convert.ToInt32(_tblAssignment.DocumentType) == Ppt)
                            {
                                fileBytes = CommonHelper.GeneratePDF("A task has been Approved by " + CurrentUserSession.User.FirstName, userToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel);
                            }
                            Global.SendEmail(userToNotify.EmailID, templates.Subject, EmailBody, fileBytes);
                        }
                    }
                }
                catch
                {
                }
            }
            if (DocumentType > 0 && DocumentType == Ppt)
            {
                // Merge PPTs to Master  Doc & Regenrate PPT file 
                var allSlides = (from p in _entities.tblPPTSlides
                                 where p.MasterDocumentName == OrginalSourceFile && !(p.IsOriginal == true)
                                 orderby p.Sequence
                                 select p.SlideName).ToList();

                MergeFiles(allSlides, OrginalSourceFile);
            }

            Status = 1;
            return Status;
        }

        [HttpPost]
        public int BulkDeclineTasks(string IDs, string Remarks)
        {
            int Status = 0;

            int UserID = CurrentUserSession.UserID;
            int Completed = Convert.ToInt32(Global.Action.Completed);
            int Approved = Convert.ToInt32(Global.Action.Approved);
            int Declined = Convert.ToInt32(Global.Action.Declined);

            int Word = Convert.ToInt32(Global.DocumentType.Word);
            int Ppt = Convert.ToInt32(Global.DocumentType.Ppt);
            int Xls = Convert.ToInt32(Global.DocumentType.Xls);
            int DocumentType = 0;
            var OrginalSourceFile = "";

            int[] TaskIDs = Array.ConvertAll(IDs.Split(','), int.Parse);

            foreach (var AssignmentID in TaskIDs)
            {
                try
                {
                    tblAssignment _tblAssignment = _entities.tblAssignments.Where(c => c.AssignmentID == AssignmentID && c.CreatedBy == UserID && c.Action == Completed).FirstOrDefault();
                    if (_tblAssignment != null)
                    {
                        if (_tblAssignment.tblDocument != null)
                        {
                            OrginalSourceFile = _tblAssignment.tblDocument.DocumentName;
                        }
                        DocumentType = _tblAssignment.DocumentType.HasValue ? _tblAssignment.DocumentType.Value : 0;

                        if (DocumentType == Word)
                        {
                            var document = _tblAssignment.DocumentFile;
                            var ReplacementCode = _tblAssignment.ReplacementCode;

                            _tblAssignment.Action = Declined;
                            _tblAssignment.Remarks = Remarks;
                            _entities.Entry(_tblAssignment).State = EntityState.Modified;

                            //Add Log 
                            tblAssignmentLog _log = new tblAssignmentLog();
                            _log.Action = Declined;
                            _log.AssignmentID = _tblAssignment.AssignmentID;
                            _log.CreatedDate = DateTime.Now;
                            _log.CreatedBy = UserID;
                            _log.DocumentName = document;
                            if (!string.IsNullOrEmpty(Remarks))
                            {
                                _log.Description = Remarks;
                            }
                            _entities.tblAssignmentLogs.Add(_log);
                            //
                            _entities.SaveChanges();



                            var server = new RichEditDocumentServer();
                            server.LoadDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + OrginalSourceFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                            DevExpress.XtraRichEdit.API.Native.Document documents = server.Document;

                            //var CodeToFindWithSpace = Characters.Space.ToString() + ReplacementCode + Characters.Space.ToString();
                            //DocumentRange ranges = server.Document.FindAll(CodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                            var CodeToFindWithSpace = Characters.Space.ToString() + "#S" + ReplacementCode + Characters.Space.ToString();
                            DocumentRange rangesStart = server.Document.FindAll(CodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                            var EndCodeToFindWithSpace = Characters.Space.ToString() + ReplacementCode + "E#" + Characters.Space.ToString();
                            DocumentRange rangesEnd = server.Document.FindAll(EndCodeToFindWithSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                            DocumentRange ranges = null;
                            if (rangesStart != null && rangesEnd != null)
                            {
                                var totalLength = rangesEnd.End.ToInt() - rangesStart.Start.ToInt();
                                ranges = server.Document.CreateRange(rangesStart.Start, totalLength);
                            }

                            if (ranges != null)
                            {
                                //server.Document.ReplaceAll(ReplacementCode, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord); // remove timestamp
                                server.Document.Replace(ranges, ""); // remove timestamp
                                server.Document.InsertDocumentContent(ranges.Start, DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + _tblAssignment.DocumentFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert assigned copy to main doc
                                server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + OrginalSourceFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // Saves with same name causes the replacement
                            }
                            else
                            {
                                //DocumentRange rangesNoSpace = server.Document.FindAll(ReplacementCode, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                                var CodeToFindWithNoSpace = "#S" + ReplacementCode;
                                DocumentRange NoSpacerangesStart = server.Document.FindAll(CodeToFindWithNoSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();

                                var EndCodeToFindWithNoSpace = ReplacementCode + "E#";
                                DocumentRange NoSpacerangesEnd = server.Document.FindAll(EndCodeToFindWithNoSpace, DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord).FirstOrDefault();
                                DocumentRange rangesNoSpace = null;
                                if (NoSpacerangesStart != null && NoSpacerangesEnd != null)
                                {
                                    var totalLengthNoSpace = NoSpacerangesEnd.End.ToInt() - NoSpacerangesStart.Start.ToInt();
                                    rangesNoSpace = server.Document.CreateRange(NoSpacerangesStart.Start, totalLengthNoSpace);
                                }
                                if (rangesNoSpace != null)
                                {
                                    server.Document.Replace(rangesNoSpace, ""); // remove timestamp
                                    server.Document.ReplaceAll(ReplacementCode, "", DevExpress.XtraRichEdit.API.Native.SearchOptions.WholeWord); // remove timestamp
                                    server.Document.InsertDocumentContent(rangesNoSpace.Start, DirectoryManagmentUtils.InitialDemoFilesPathDocument + "/" + "Copy_" + _tblAssignment.DocumentFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // insert assigned copy to main doc
                                    server.Document.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathDemo + "/" + OrginalSourceFile, DevExpress.XtraRichEdit.DocumentFormat.OpenXml); // Saves with same name causes the replacement
                                }
                            }

                            DocumentManager.CloseAllDocuments();
                            server.Dispose();

                            DocumentManager.CloseAllDocuments();
                            server.Dispose();
                        }
                        else if (DocumentType == Ppt || DocumentType == Xls || DocumentType == 0)
                        {
                            _tblAssignment.Action = Declined;
                            _tblAssignment.Remarks = Remarks;
                            _entities.Entry(_tblAssignment).State = EntityState.Modified;

                            if (DocumentType == Ppt)
                            {
                                var ppt = (from itm in _entities.tblAssignedPPTSlides
                                           where itm.AssignmentID == AssignmentID && !(itm.IsPPTApproved.HasValue && itm.IsPPTApproved.Value == true)
                                           select itm);

                                if (ppt != null && ppt.Count() > 0)
                                {
                                    foreach (var item in ppt)
                                    {
                                        item.IsPPTModified = null;
                                        item.IsGrayedOut = null;
                                    }
                                    _entities.SaveChanges();
                                }
                            }

                            if (DocumentType == Xls)
                            {
                                var excel = (from itm in _entities.tblAssignedExcelSheets
                                             where itm.AssignmentID == AssignmentID && !(itm.IsSheetApproved == true)
                                             select itm);

                                if (excel != null && excel.Count() > 0)
                                {
                                    foreach (var item in excel)
                                    {
                                        item.IsSheetModified = false;
                                        item.IsGrayedOut = false;
                                    }
                                    _entities.SaveChanges();
                                }
                            }

                            //Add Log 
                            tblAssignmentLog _log = new tblAssignmentLog();
                            _log.Action = Declined;
                            _log.AssignmentID = AssignmentID;
                            _log.CreatedDate = DateTime.Now;
                            _log.CreatedBy = UserID;
                            if (!string.IsNullOrEmpty(Remarks))
                            {
                                _log.Description = Remarks;
                            }
                            _log.DocumentName = _tblAssignment.DocumentFile;
                            _entities.tblAssignmentLogs.Add(_log);
                            _entities.SaveChanges();
                        }

                        var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                        AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                        objAssignmentModelEmail.TaskName = _tblAssignment.TaskName;
                        objAssignmentModelEmail.AssignID = _tblAssignment.AssignmentID;
                        List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                        lstAssignmentLogModel = GetAssignmentLog(_tblAssignment.AssignmentID);
                        string EmailBody = templates.TemplateContentForEmail;
                        foreach (var userToNotify in _tblAssignment.tblAssignmentMembers.Select(m => m.tblUserDepartment))
                        {
                            templates.Subject = templates.Subject.Replace("#Subject#", "Task Declined: Task has been declined by " + CurrentUserSession.User.FirstName);
                            EmailBody = PopulateEmailBody("A task has been declined by " + CurrentUserSession.User.FirstName, userToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody);

                            byte[] fileBytes = null;
                            if (DocumentType == Ppt)
                                fileBytes = CommonHelper.GeneratePDF("A task has been declined by " + CurrentUserSession.User.FirstName, userToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel);

                            Global.SendEmail(userToNotify.EmailID, templates.Subject, EmailBody, fileBytes);
                        }

                        //var Currentuser = _entities.tblUserDepartments.Where(x => x.UserId == UserID).FirstOrDefault();
                        //Task.Factory.StartNew(() =>
                        //{
                        //    var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == _tblAssignment.CreatedBy).FirstOrDefault();
                        //    if (UserToNotify != null && !string.IsNullOrEmpty(UserToNotify.EmailID))
                        //    {
                        //        string DEC = Common.Global.GetEnumDescription(Common.Global.Action.Declined);

                        //        StringBuilder _sb = new StringBuilder();
                        //        _sb.AppendLine("Hello " + UserToNotify.FullName + ",<br/><br/>");
                        //        _sb.AppendLine(Currentuser.FullName + " has " + DEC + " his task in Triyo. Please login to your account to check it.<br/><br/>");
                        //        _sb.AppendLine("Regards,<br/><b>Triyo</b>");
                        //        WordAutomationDemo.Common.Global.SendEmail(UserToNotify.EmailID, "Task " + DEC + " by  " + Currentuser.FullName, _sb.ToString());
                        //    }
                        //});
                    }
                }
                catch
                {
                }
            }

            Status = 1;

            return Status;
        }

        #region Excel
        public void SaveExcelForUser(Aspose.Cells.Workbook workbook, int assignmentID, string originalFilename, string snippetFilename)
        {
            var masterWb = new Aspose.Cells.Workbook(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\" + originalFilename);

            var sheetDict = new Dictionary<string, Dictionary<Guid, Aspose.Cells.Row>>();

            foreach (var masterSheet in masterWb.Worksheets)
            {
                var dict = new Dictionary<Guid, Aspose.Cells.Row>();
                var sheetName = masterSheet.Name;

                for (var ind = 0; ind < masterSheet.Cells.Rows.Count; ind++)
                {
                    var val = masterSheet.Cells.Rows[ind].FirstCell.Value as String;
                    if (!String.IsNullOrEmpty(val))
                    {
                        var guidVal = val.StartsWith("(Do Not Modify) Triyo") ? val.Replace("(Do Not Modify) Triyo ", "") : val;
                        Guid guid = Guid.Parse(guidVal);
                        var row = masterSheet.Cells.Rows[ind];
                        dict.Add(guid, row);
                    }
                    else
                    {
                        continue;
                    }
                }

                sheetDict.Add(sheetName, dict);
            }

            foreach (var sheet in workbook.Worksheets)
            {
                // Skip empty sheets
                if (sheet.Cells.Rows.Count == 0 || sheet.Cells.MaxDataColumn == -1)
                    continue;

                //throw new Exception("Please upload Triyo generated SpreadSheet");

                var data = sheet.Cells.ExportDataTable(0, 0, sheet.Cells.MaxDataRow + 1, sheet.Cells.MaxDataColumn + 1);

                // Possibly the fastest way of pidding the Excel doc with guids and assigning ownership in the DB
                var formatter = new BinaryFormatter();
                var newGuids = new List<Guid>();
                var newRows = new List<System.Data.DataRow>();
                Func<Guid, Guid> addNewGuid = delegate (Guid g) { newGuids.Add(g); return g; };
                Func<int, string> getChecksum = delegate (int row)
                {
                    using (var md5 = MD5.Create())
                    using (var ms = new MemoryStream())
                    {
                        formatter.Serialize(ms, sheet.Cells.ExportArray(row, 2, 1, sheet.Cells.MaxDataColumn - 1));
                        return Convert.ToBase64String(md5.ComputeHash(ms.ToArray()));
                    }
                };

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var guid = data.Rows[i][0].ToString().StartsWith("(Do Not Modify) Triyo") ? data.Rows[i][0].ToString().Replace("(Do Not Modify) Triyo ", "") : data.Rows[i][0].ToString();
                    var checksum = data.Rows[i][1].ToString().StartsWith("(Do Not Modify) Checksum") ? data.Rows[i][1].ToString().Replace("(Do Not Modify) Checksum ", "") : data.Rows[i][1].ToString();

                    if (String.IsNullOrEmpty(data.Rows[i][0] as String))
                    {
                        (data.Rows[i][0], data.Rows[i][1]) = data.Rows[i][0] is DBNull ? (addNewGuid(Guid.NewGuid()), getChecksum(i)) : (data.Rows[i][0], data.Rows[i][1]);
                        newRows.Add(data.Rows[i]);

                        //Update guids and checksums in assigned content
                        for (var c = 0; c < data.Rows[i].ItemArray.Length; c++)
                        {
                            sheet.Cells.Rows[i][c].Value = data.Rows[i][c];
                        }

                    }
                    else if (String.Compare(getChecksum(i), checksum as String) != 0)
                    {
                        Guid updRowGuid = Guid.Parse(guid);
                        var index = sheetDict[sheet.Name][updRowGuid].Index;
                        var masterRow = masterWb.Worksheets[sheet.Name].Cells.Rows[index];

                        for (int j = 0; j < sheet.Cells.MaxDataColumn + 1; j++)
                        {
                            var cell = masterRow[j];
                            //Update checksum
                            if (j == 1)
                                cell.Value = getChecksum(i);
                            else
                                cell.Value = data.Rows[i][j];
                        }
                    }
                }

                //Insert new rows to end of master
                var lastRow = masterWb.Worksheets[sheet.Name].Cells.GetLastDataRow(2);
                foreach (var newRow in newRows)
                {
                    masterWb.Worksheets[sheet.Name].Cells.InsertRow(lastRow + 1);
                    var lastMasterRow = masterWb.Worksheets[sheet.Name].Cells.Rows[lastRow + 1];

                    for (int k = 0; k < newRow.ItemArray.Length; k++)
                    {
                        lastMasterRow[k].Value = newRow.ItemArray[k];
                    }
                    lastRow += 1;
                }

                var newRowMappings = newGuids.Select(rowid => new tblExcelRowMap()
                {
                    DateLastModified = DateTime.Now,
                    IsActive = true,
                    IsRemoved = false,
                    MasterDocumentName = originalFilename,
                    RowId = rowid,
                    AssignmentID = assignmentID
                }).ToArray();
                _entities.BulkInsert(newRowMappings);

                workbook.Save(snippetFilename);
                masterWb.Save(masterWb.FileName);
            }
        }

        //Load Document to Popup 
        public ActionResult LoadAssignedExcel(string originalDocumentName, string name, int userid, bool IsReadOnly, int assignmentId, int Action = 1)
        {
            var demoModel = new AssignmentModel() { OriginalDocumentName = originalDocumentName, DocumentName = name, UserID = CurrentUserSession.UserID, Action = Action, IsReadOnly = IsReadOnly };
            //demoModel.Path = "/Home/GetAssignedExcel?FileName=" + name + "&userId=" + userid + "&AssignmentId=" + assignmentId;
            // For some reason DevExpress requires a physical file path. We can't load from a URL. Stupid but okay let's do it.
            var tmp = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, "tmp");
            var assignment = _entities.tblAssignments.Where(a => a.AssignmentID == assignmentId).First();
            //var assignedUser = assignment.tblUserDepartment.UserId;
            if (!Directory.Exists(tmp))
                Directory.CreateDirectory(tmp);

            //Create a copy of the assigned content
            string copyFilePath = Path.Combine(tmp, assignmentId.ToString() + "-c-" + name);
            if (!System.IO.File.Exists(copyFilePath))
                System.IO.File.WriteAllBytes(copyFilePath, GetExcelForAssignment(name, assignmentId, viewInGrid: true));

            string filePath = Path.Combine(tmp, assignmentId.ToString() + "-" + name);

            if (!System.IO.File.Exists(filePath)) System.IO.File.WriteAllBytes(filePath, GetExcelForAssignment(name, assignmentId, viewInGrid: true));

            demoModel.Path = "/" + filePath.Replace(Request.ServerVariables["APPL_PHYSICAL_PATH"], "").Replace(@"\", "/");
            return PartialView("_LoadInlineExcel", demoModel);
        }

        //Load Document to Popup 
        public ActionResult LoadAssignedChangedExcel(string originalDocumentName, string name, int userid, bool IsReadOnly, int assignmentId, int Action = 1)
        {
            var demoModel = new AssignmentModel() { OriginalDocumentName = originalDocumentName, DocumentName = name, UserID = CurrentUserSession.UserID, Action = Action, IsReadOnly = IsReadOnly, AssignID = assignmentId };
            //demoModel.Path = "/Home/GetAssignedExcel?FileName=" + name + "&userId=" + userid + "&AssignmentId=" + assignmentId;
            // For some reason DevExpress requires a physical file path. We can't load from a URL. Stupid but okay let's do it.
            var tmp = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, "tmp");
            var assignment = _entities.tblAssignments.Where(a => a.AssignmentID == assignmentId).First();
            name = assignment.tblDocument.DocumentName;
            if (!Directory.Exists(tmp))
                Directory.CreateDirectory(tmp);

            string copyFilePath = Path.Combine(tmp, assignmentId.ToString() + "-c-" + name);
            if (!System.IO.File.Exists(copyFilePath))
                System.IO.File.WriteAllBytes(copyFilePath, GetExcelForAssignment(name, assignmentId, viewInGrid: true));

            string filePath = Path.Combine(tmp, assignmentId.ToString() + "-" + name);

            if (!System.IO.File.Exists(filePath)) System.IO.File.WriteAllBytes(filePath, GetExcelForAssignment(name, assignmentId, true, viewInGrid: true));

            demoModel.Path = "/" + copyFilePath.Replace(Request.ServerVariables["APPL_PHYSICAL_PATH"], "").Replace(@"\", "/");
            return PartialView("_LoadExcel", demoModel);
        }

        //Load Document to Popup 
        public ActionResult LoadInlineExcel(string originalDocumentName = "", string name = "", int userid = 0, int Action = 1, bool IsReadOnly = false, int AssignmentID = 0)
        {
            var demoModel = new AssignmentModel() { OriginalDocumentName = "Sheets_" + originalDocumentName, DocumentName = name, UserID = CurrentUserSession.UserID, Action = Action, IsReadOnly = IsReadOnly, AssignID = AssignmentID };

            return PartialView("_LoadInlineExcel", demoModel);
        }

        //Load Document to Popup 
        public ActionResult LoadChangedExcel(string originalDocumentName = "", string name = "", int userid = 0, int Action = 1, bool IsReadOnly = false)
        {
            var demoModel = new AssignmentModel() { OriginalDocumentName = "Sheets_" + originalDocumentName, DocumentName = name, UserID = CurrentUserSession.UserID, Action = Action, IsReadOnly = IsReadOnly };

            return PartialView("_LoadChangedExcel", demoModel);

        }

        public ActionResult LoadInlineAssigned(string originalDocumentName = "", string name = "", int userid = 0, int Action = 1, bool IsReadOnly = false, int AssignmentID = 0)
        {
            var demoModel = new AssignmentModel() { OriginalDocumentName = "Sheets_" + originalDocumentName, DocumentName = name, UserID = CurrentUserSession.UserID, Action = Action, IsReadOnly = IsReadOnly, AssignID = AssignmentID };

            return PartialView("_LoadInlineAssigned", demoModel);
        }

        //Load Document to Popup 
        public ActionResult LoadOriginalExcel(string originalDocumentName = "", int userid = 0, int Action = 1, bool IsReadOnly = false)
        {

            using (MemoryStream stream = new MemoryStream())
            {
                DocumentManager.CloseDocument(Path.Combine(WordAutomationDemo.Models.DirectoryManagmentUtils.InitialDemoFilesPathExcels, originalDocumentName));
            }

            var demoModel = new AssignmentModel() { OriginalDocumentName = originalDocumentName, UserID = CurrentUserSession.UserID, Action = Action, IsReadOnly = IsReadOnly };

            return PartialView("_LoadOriginalExcel", demoModel);

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public FileResult GetAssignedExcel(string FileName, int userId, int AssignmentId)
        {
            var assignment = _entities.tblAssignments.Where(a => a.AssignmentID == AssignmentId).First();
            return File(GetExcelForAssignment(FileName, AssignmentId), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public FileResult GetAssignedUnchangedExcel(string FileName, int userId, int AssignmentId)
        {
            //var docName = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\tmp", userId.ToString() + "-" + AssignmentId.ToString() + "-" + FileName);
            var docName = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\tmp", AssignmentId.ToString() + "-" + FileName);

            using (var doc = new Aspose.Cells.Workbook(docName))
            {
                using (var ms = new MemoryStream())
                {
                    doc.Save(ms, Aspose.Cells.SaveFormat.Xlsx);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public FileResult GetAssignedChangedExcel(string FileName, int userId, int AssignmentId)
        {
            var docName = Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\tmp", AssignmentId.ToString() + "-c-" + FileName);

            using (var doc = new Aspose.Cells.Workbook(docName))
            {
                using (var ms = new MemoryStream())
                {
                    doc.Save(ms, Aspose.Cells.SaveFormat.Xlsx);
                    return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                }
            }
        }

        private byte[] GetExcelForAssignment(string docName, int assignmentId, bool excludeUnmodifiedRows = false, bool viewInGrid = false)
        {
            using (var doc = new Aspose.Cells.Workbook(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, docName)))
            {
                foreach (var sheet in doc.Worksheets)
                {
                    if (sheet.Cells.Rows.Count == 0 || sheet.Cells.MaxDataColumn == -1)
                        continue;
                    var formatter = new BinaryFormatter();
                    Func<int, string> getChecksum = delegate (int row)
                    {
                        using (var md5 = MD5.Create())
                        using (var ms = new MemoryStream())
                        {
                            formatter.Serialize(ms, sheet.Cells.ExportArray(row, 2, 1, sheet.Cells.MaxDataColumn - 1));
                            return Convert.ToBase64String(md5.ComputeHash(ms.ToArray()));
                        }
                    };

                    // Skip empty sheets
                    if (sheet.Cells.Rows.Count == 0 || sheet.Cells.MaxDataColumn == -1)
                        continue;
                    List<int> hitlist = new List<int>();
                    int counter = 0;
                    var index = new HashSet<Guid>(_entities.tblExcelRowMaps.Where(m => m.MasterDocumentName == docName && m.AssignmentID == assignmentId).Select(m => m.RowId));
                    string checksum = string.Empty;
                    foreach (Aspose.Cells.Row row in sheet.Cells.Rows)
                    {
                        string guid = counter++ == 0 ? (row[0].Value as string ?? string.Empty).Replace("(Do Not Modify) Triyo ", "") : row[0].Value as string;
                        if (excludeUnmodifiedRows)
                            checksum = (row[1].Value as string ?? string.Empty).Replace("(Do Not Modify) Checksum ", "");
                        Guid og = Guid.Empty;
                        if (!(Guid.TryParse(guid, out og) && index.Contains(og)))
                            hitlist.Add(row.Index);
                        else if (excludeUnmodifiedRows && !getChecksum(row.Index).Equals(checksum))
                            hitlist.Add(row.Index);
                    }
                    foreach (var row in hitlist.OrderByDescending(c => c))
                        sheet.Cells.DeleteRow(row);
                }
                using (var ms = new MemoryStream())
                {
                    if (viewInGrid == true)
                    {
                        doc.Save(ms, Aspose.Cells.SaveFormat.Auto);
                    }
                    else
                    {
                        doc.Save(ms, Aspose.Cells.SaveFormat.Xlsx);
                    }

                    return ms.ToArray();
                }
            }
        }

        public bool DeclineExcel(string document, int AssignID, string Remarks)
        {
            //Mark Task as Merged 
            var doc = (from itm in _entities.tblAssignments
                       where itm.AssignmentID == AssignID
                       select itm).FirstOrDefault();
            doc.Action = (int)WordAutomationDemo.Common.Global.Action.Declined;
            doc.CompletedDate = DateTime.Now;
            if (!string.IsNullOrEmpty(Remarks))
            {
                doc.Comments = Remarks;
            }
            _entities.Entry(doc).State = System.Data.Entity.EntityState.Modified;

            var sheets = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == AssignID && !(z.IsSheetApproved == true));
            if (sheets != null && sheets.Count() > 0)
            {
                foreach (var sheet in sheets)
                {

                    var fileurl = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + document + "/AssignedDoc/New_" + AssignID.ToString() + "_Copy_" + sheet.SheetName;
                    var newfileurl = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + document + "/AssignedDoc/" + AssignID.ToString() + "_Copy_" + sheet.SheetName;
                    if (System.IO.File.Exists(fileurl) && System.IO.File.Exists(newfileurl))
                    {
                        System.IO.File.Delete(fileurl);
                        System.IO.File.Delete(newfileurl);
                        var oldfileurl = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + document + "/" + sheet.SheetName;
                        if (System.IO.File.Exists(oldfileurl))
                        {
                            Workbook workbook = new Workbook();
                            using (FileStream stream = new FileStream(oldfileurl, FileMode.Open))
                            {
                                workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                                workbook.DocumentProperties.Manager = CurrentUserSession.UserID + "|" + AssignID + "|" + "Sheets_" + document + "|" + AssignID + "_Copy_" + sheet.SheetName;
                                workbook.SaveDocument(newfileurl, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                            }
                        }
                    }
                    sheet.IsSheetModified = false;
                    sheet.IsSheetApproved = false;
                    sheet.IsGrayedOut = false;
                    _entities.Entry(sheet).State = System.Data.Entity.EntityState.Modified;
                }
                _entities.SaveChanges();
            }

            //Add Log 
            tblAssignmentLog _log = new tblAssignmentLog();
            _log.Action = (int)WordAutomationDemo.Common.Global.Action.Declined;
            _log.AssignmentID = doc.AssignmentID;
            _log.CreatedDate = DateTime.Now;
            _log.CreatedBy = CurrentUserSession.UserID;
            _log.Description = Remarks;
            _log.DocumentName = document;
            _entities.tblAssignmentLogs.Add(_log);
            _entities.SaveChanges();

            var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

            AssignmentModel objAssignmentModelEmail = new AssignmentModel();
            objAssignmentModelEmail.TaskName = doc.TaskName;
            objAssignmentModelEmail.Comments = doc.Comments;
            objAssignmentModelEmail.AssignID = doc.AssignmentID;
            List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
            lstAssignmentLogModel = GetAssignmentLog(doc.AssignmentID);
            string EmailBody = templates.TemplateContentForEmail;
            var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == doc.LockedByUserID).FirstOrDefault();
            templates.Subject = templates.Subject.Replace("#Subject#", "Task Declined: Sheet has been declined by " + CurrentUserSession.User.FirstName);
            EmailBody = PopulateEmailBody("A task has been declined by " + CurrentUserSession.User.FirstName, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody);

            Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody);

            return true;
        }

        public bool DeclineSheet(int assignedExeclSheetID, int assignID, string docName, string remarks)
        {
            using (ReadyPortalDBEntities _readyPortalDBEntities = new ReadyPortalDBEntities())
            {
                var excel = (from itm in _readyPortalDBEntities.tblAssignedExcelSheets
                             where itm.AssignedExcelSheetID == assignedExeclSheetID
                             select itm).FirstOrDefault();

                if (excel != null)
                {
                    var fileurl = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + docName + "/AssignedDoc/New_" + assignID.ToString() + "_Copy_" + excel.SheetName;
                    var newfileurl = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + docName + "/AssignedDoc/" + assignID.ToString() + "_Copy_" + excel.SheetName;
                    if (System.IO.File.Exists(fileurl) && System.IO.File.Exists(newfileurl))
                    {
                        System.IO.File.Delete(fileurl);
                        System.IO.File.Delete(newfileurl);
                        var oldfileurl = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + docName + "/" + excel.SheetName;
                        if (System.IO.File.Exists(oldfileurl))
                        {
                            Workbook workbook = new Workbook();
                            using (FileStream stream = new FileStream(oldfileurl, FileMode.Open))
                            {
                                workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                                workbook.DocumentProperties.Manager = CurrentUserSession.UserID + "|" + assignID + "|" + "Sheets_" + docName + "|" + assignID + "_Copy_" + excel.SheetName;
                                workbook.SaveDocument(newfileurl, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                            }
                        }
                    }
                    excel.IsSheetModified = false;
                    excel.IsSheetApproved = false;
                    excel.IsGrayedOut = false;
                    excel.SheetRemarks = remarks;
                    _readyPortalDBEntities.Entry(excel).State = System.Data.Entity.EntityState.Modified;
                    _readyPortalDBEntities.SaveChanges();
                }

                //Mark Task as Merged 
                var doc = (from itm in _entities.tblAssignments
                           where itm.AssignmentID == assignID
                           select itm).FirstOrDefault();
                //doc.Action = (int)WordAutomationDemo.Common.Global.Action.Declined;
                //doc.CompletedDate = DateTime.Now;
                //if (!string.IsNullOrEmpty(Remarks))
                //{
                //    doc.Comments = Remarks;
                //}
                //_entities.Entry(doc).State = System.Data.Entity.EntityState.Modified;

                //Add Log 
                tblAssignmentLog _log = new tblAssignmentLog();
                _log.Action = (int)WordAutomationDemo.Common.Global.Action.Declined;
                _log.AssignmentID = assignID;
                _log.CreatedDate = DateTime.Now;
                _log.CreatedBy = CurrentUserSession.UserID;
                _log.Description = remarks;
                _log.DocumentName = docName;
                _entities.tblAssignmentLogs.Add(_log);
                _entities.SaveChanges();

                var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                objAssignmentModelEmail.TaskName = doc.TaskName;
                objAssignmentModelEmail.Comments = doc.Comments;
                objAssignmentModelEmail.AssignID = doc.AssignmentID;
                List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                lstAssignmentLogModel = GetAssignmentLog(doc.AssignmentID);
                string EmailBody = templates.TemplateContentForEmail;
                var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == doc.LockedByUserID).FirstOrDefault();
                templates.Subject = templates.Subject.Replace("#Subject#", "Sheet Declined: Sheet has been declined by " + CurrentUserSession.User.FirstName);
                EmailBody = PopulateEmailBody("A sheet has been declined by " + CurrentUserSession.User.FirstName, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody);

                Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody);

                return true;
            }
        }

        public bool ValidateUploadedExcel(FormCollection objFormCollection)
        {
            Result objAvailabilityResult = new Result();
            if (objFormCollection != null && objFormCollection["assignmentID"] != null && objFormCollection["originalFile"] != null)
            {
                int assignmentID = Convert.ToInt32(objFormCollection["assignmentID"]);
                string originalFile = objFormCollection["originalFile"];
                if (Request.Files != null && Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var origChangedPath = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\tmp\\" +
                                              assignmentID.ToString() + "-c-" + originalFile;
                        var origChangedFile = new Aspose.Cells.Workbook(origChangedPath);
                        var uploadedFile = new Aspose.Cells.Workbook(file.InputStream);

                        //Look for assigned rows in new uploaded document
                        using (var _entities = new ReadyPortalDBEntities())
                        {
                            var assignedRows = _entities.tblExcelRowMaps.Where(r => r.AssignmentID == assignmentID)
                                .ToList();
                            List<Guid> assignedGuids = assignedRows.Select(r => r.RowId).ToList();

                            //Check if sheets match
                            var origWorksheetNames = origChangedFile.Worksheets.Select(w => w.Name);
                            var uploadedWorksheetNames = uploadedFile.Worksheets.Select(w => w.Name);
                            bool sheetsMatch = origWorksheetNames.Intersect(uploadedWorksheetNames).Count() ==
                                               origWorksheetNames.Count() && origChangedFile.Worksheets.Count() == uploadedFile.Worksheets.Count();

                            if (sheetsMatch)
                            {
                                List<Guid> guidsInFile = new List<Guid>();
                                foreach (var sheet in uploadedFile.Worksheets)
                                {
                                    var data = sheet.Cells.ExportDataTable(0, 0, sheet.Cells.MaxDataRow + 1, sheet.Cells.MaxDataColumn + 1, new Aspose.Cells.ExportTableOptions()
                                    {
                                        CheckMixedValueType = false,
                                        ExportColumnName = true,
                                        ExportAsString = true
                                    });

                                    for (var i = 0; i < data.Rows.Count; i++)
                                    {
                                        Guid guid = Guid.Empty;
                                        var guidCell = data.Rows[i][0].ToString().StartsWith("(Do Not Modify) Triyo") ? data.Rows[i][0].ToString().Replace("(Do Not Modify) Triyo ", "") : data.Rows[i][0].ToString();

                                        if (Guid.TryParse(guidCell, out guid) && guid != Guid.Empty)
                                        {
                                            guidsInFile.Add(guid);
                                        }
                                    }

                                }

                                //Check if uploaded has Guids and all guids in original are in uploaded doc
                                //if (guidsInFile.Count > 0 &&
                                //    assignedGuids.Intersect(guidsInFile).Count() == assignedGuids.Count)
                                //{
                                //    //Update content of changed file with uploaded file
                                //    UpdateChangedExcel(uploadedFile, origChangedPath);
                                //    return true;
                                //}
                                // Need to comment upper code because that code never received correct guild in the downloaded file.
                                UpdateChangedExcel(uploadedFile, origChangedPath);
                                return true;

                            }
                            else
                            {
                                return false;
                            }


                        }

                    }
                }
            }

            return false;
        }

        private void UpdateChangedExcel(Aspose.Cells.Workbook uploaded, string originalChangedFilename)
        {
            //Just replacing the document for now (might need to go through rows and replace values later)
            uploaded.Save(originalChangedFilename);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public bool ApproveExcel(string document, int AssignID, string Remarks)
        {
            var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == AssignID);
            var originalFile = _entities.tblDocuments.FirstOrDefault(d => d.DocumentID == assignment.DocumentID).DocumentName;
            var path = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\tmp\\" + assignment.AssignmentID.ToString() + "-c-" + originalFile;
            var workbook = new Aspose.Cells.Workbook(path);
            SaveExcelForUser(workbook, AssignID, originalFile, path);
            assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
            assignment.Status = (int)Global.AssignmentStatus.Completed;

            //Add log
            tblAssignmentLog _log = new tblAssignmentLog();
            _log.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
            _log.AssignmentID = assignment.AssignmentID;
            _log.CreatedDate = DateTime.Now;
            _log.CreatedBy = CurrentUserSession.UserID;
            _entities.tblAssignmentLogs.Add(_log);

            //Remove mapped rows
            var excelRowMaps = _entities.tblExcelRowMaps.Where(rm => rm.AssignmentID == assignment.AssignmentID).ToList();
            _entities.tblExcelRowMaps.RemoveRange(excelRowMaps);
            _entities.SaveChanges();

            return true;
        }

        [HttpPost]
        public bool ApproveSheet(int assignedExcelSheetID, int assignID, string docName)
        {
            int FirstEntryID = 0;
            using (ReadyPortalDBEntities _readyPortalDBEntities = new ReadyPortalDBEntities())
            {
                var AssignedSheets = (from items in _readyPortalDBEntities.tblAssignedExcelSheets.AsEnumerable()
                                      join q in _readyPortalDBEntities.tblAssignments.AsEnumerable() on items.AssignmentID equals q.AssignmentID
                                      join docu in _readyPortalDBEntities.tblDocuments on q.DocumentID equals docu.DocumentID
                                      where items.AssignmentID == assignID && items.IsSheetModified == true && items.AssignedExcelSheetID == assignedExcelSheetID
                                      orderby items.Sequence
                                      select new ExcelModel()
                                      {
                                          AssignedExcelSheetID = items.AssignedExcelSheetID,
                                          AssignedTaskID = items.AssignmentID,
                                          OriginalFile = docu.DocumentName,
                                          OriginalSheetName = items.SheetName,
                                          SheetName = "New_" + items.AssignmentID.ToString() + "_Copy_" + items.SheetName
                                      }).FirstOrDefault();
                Workbook WorkbookSplit = new Workbook();
                if (AssignedSheets != null)
                {
                    var MyOriFile = _readyPortalDBEntities.tblExcelSheets.Where(c => c.MasterDocumentName == docName && c.SheetName == AssignedSheets.OriginalSheetName && !(c.IsOriginal == true)).FirstOrDefault();

                    int MyFileSeq = Convert.ToInt32(MyOriFile.Sequence);

                    List<tblExcelSheet> lsttblExcelSheetNew = new List<tblExcelSheet>();

                    List<tblExcelSheet> lsttblExcelSheets = new List<tblExcelSheet>();
                    lsttblExcelSheets = _readyPortalDBEntities.tblExcelSheets.Where(c => c.MasterDocumentName == docName && c.Sequence < MyFileSeq && !(c.IsOriginal == true)).ToList();
                    foreach (var item in lsttblExcelSheets)
                    {
                        lsttblExcelSheetNew.Add(item);
                    }

                    Workbook workbook = new Workbook();
                    int NewFileSeq = 0;
                    using (FileStream stream = new FileStream(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + docName, FileMode.Open))
                    {
                        workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);

                        var fileurl = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + docName + "/AssignedDoc/" + AssignedSheets.SheetName;
                        if (System.IO.File.Exists(fileurl))
                        {
                            using (FileStream streamNew = new FileStream(fileurl, FileMode.Open))
                            {
                                Workbook WorkbookNew = new Workbook();
                                WorkbookNew.LoadDocument(streamNew, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                                var strSheetName = workbook.Worksheets.Select(s => s.Name).ToArray();
                                var oldSheet = _readyPortalDBEntities.tblExcelSheets.Where(c => c.MasterDocumentName == docName && c.SheetName == AssignedSheets.OriginalSheetName && !(c.IsOriginal == true)).FirstOrDefault();
                                if (oldSheet != null)
                                {
                                    var excelSheet = workbook.Worksheets.Where(s => s.Name.ToLower() == AssignedSheets.OriginalSheetName.Replace(".xlsx", "").ToLower()).FirstOrDefault();
                                    if (excelSheet != null)
                                    {
                                        workbook.Worksheets.Remove(excelSheet);
                                    }

                                    NewFileSeq = MyFileSeq;
                                    for (int j = 0; j < WorkbookNew.Worksheets.Count(); j++)
                                    {
                                        string strName = string.Empty;
                                        if (strSheetName.Contains(WorkbookNew.Worksheets[j].Name))
                                        {
                                            strName = WorkbookNew.Worksheets[j].Name;
                                            workbook.Worksheets.Add(strName);
                                            workbook.Worksheets[strName].CopyFrom(WorkbookNew.Worksheets[j]);
                                        }
                                        else
                                        {
                                            strName = WorkbookNew.Worksheets[j].Name;
                                            workbook.Worksheets.Add(WorkbookNew.Worksheets[j].Name);
                                            workbook.Worksheets[WorkbookNew.Worksheets[j].Name].CopyFrom(WorkbookNew.Worksheets[j]);
                                        }


                                        tblExcelSheet _sheet = new tblExcelSheet();
                                        strName = strName.Replace(".xlsx", "");
                                        _sheet.SheetName = strName + ".xlsx";
                                        //_sheet.SheetName = strName;
                                        _sheet.Sequence = NewFileSeq;
                                        _sheet.MasterDocumentName = docName;
                                        lsttblExcelSheetNew.Add(_sheet);
                                        NewFileSeq++;

                                        //tblExcelSheet _sheet = new tblExcelSheet();
                                        //_sheet.SheetName = "Sheet" + (workSheetCount + 1) + ".xlsx";
                                        //_sheet.Sequence = NewFileSeq;
                                        //_sheet.MasterDocumentName = docName;
                                        //_readyPortalDBEntities.tblExcelSheets.Add(_sheet);
                                        //_readyPortalDBEntities.SaveChanges();
                                        //workSheetCount++;
                                        //NewFileSeq++;
                                        //if (j == 0)
                                        //{
                                        //    FirstEntryID = _sheet.ExcelSheetID;
                                        //}
                                    }
                                    _readyPortalDBEntities.tblExcelSheets.Remove(oldSheet);

                                }
                            }
                        }

                        stream.Dispose();
                        stream.Close();
                        lsttblExcelSheets = _readyPortalDBEntities.tblExcelSheets.Where(c => c.MasterDocumentName == docName && c.Sequence > MyFileSeq && !(c.IsOriginal == true)).ToList();
                        foreach (var item in lsttblExcelSheets)
                        {
                            item.Sequence = NewFileSeq;
                            lsttblExcelSheetNew.Add(item);
                            NewFileSeq++;
                        }
                        var sheet = _readyPortalDBEntities.tblExcelSheets.Where(s => s.MasterDocumentName.ToLower() == docName.ToLower() && !(s.IsOriginal == true)).ToList();
                        foreach (var item in sheet)
                        {
                            _readyPortalDBEntities.tblExcelSheets.Remove(item);
                        }
                        _readyPortalDBEntities.SaveChanges();
                        foreach (var item in lsttblExcelSheetNew)
                        {
                            tblExcelSheet _sheet = new tblExcelSheet();
                            _sheet.SheetName = item.SheetName;
                            _sheet.Sequence = item.Sequence;
                            _sheet.MasterDocumentName = docName;
                            _readyPortalDBEntities.tblExcelSheets.Add(_sheet);
                        }
                        _readyPortalDBEntities.SaveChanges();
                    }
                    if (System.IO.File.Exists(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + docName))
                    {
                        System.IO.File.Delete(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + docName);
                    }
                    workbook.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + docName);

                    Workbook WorkbookSplitOriginal = new Workbook();

                    using (FileStream stream = new FileStream(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + docName, FileMode.Open))
                    {
                        WorkbookSplit.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                        _readyPortalDBEntities.SaveChanges();
                        bool firstSheet = false;
                        foreach (var item in lsttblExcelSheetNew)
                        {
                            Workbook WorkbookSplitNew = new Workbook();
                            WorkbookSplitNew.Worksheets[0].Name = "Sheet";
                            WorkbookSplitNew.Worksheets.Add(item.SheetName.Replace(".xlsx", ""));
                            var worksheet = WorkbookSplit.Worksheets.Where(s => s.Name.ToLower() == item.SheetName.Replace(".xlsx", "").ToLower()).FirstOrDefault();
                            WorkbookSplitNew.Worksheets[item.SheetName.Replace(".xlsx", "")].CopyFrom(worksheet);
                            WorkbookSplitNew.Worksheets.RemoveAt(0);
                            string tempSheetFileName = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + "Sheets_" + docName + '/' + item.SheetName;
                            if (System.IO.File.Exists(tempSheetFileName))
                            {
                                System.IO.File.Delete(tempSheetFileName);
                            }
                            WorkbookSplitNew.SaveDocument(tempSheetFileName);
                            if (firstSheet == false)
                            {
                                WorkbookSplitOriginal.Worksheets[0].Name = "Sheet";
                            }
                            WorkbookSplitOriginal.Worksheets.Add(item.SheetName.Replace(".xlsx", ""));
                            WorkbookSplitOriginal.Worksheets[item.SheetName.Replace(".xlsx", "")].CopyFrom(worksheet);
                            if (firstSheet == false)
                            {
                                WorkbookSplitOriginal.Worksheets.RemoveAt(0);
                                firstSheet = true;
                            }
                        }

                        //int worksheetIndex = Convert.ToInt32(item.OriginalSheetName.Replace("Sheet", "").Split('.')[0]) - 1;
                        //for (int k = 0; k < WorkbookSplit.Worksheets.Count(); k++)
                        //{
                        //    //tblExcelSheet _sheet = new tblExcelSheet();
                        //    //_sheet.SheetName = workbook.Worksheets[k].Name + ".xlsx";
                        //    //_sheet.Sequence = k + 1;
                        //    //_sheet.MasterDocumentName = docName;
                        //    //_readyPortalDBEntities.tblExcelSheets.Add(_sheet);

                        //    Workbook WorkbookSplitNew = new Workbook();
                        //    WorkbookSplitNew.Worksheets[0].Name = "Sheet";
                        //    WorkbookSplitNew.Worksheets.Add(_sheet.SheetName);
                        //    WorkbookSplitNew.Worksheets[_sheet.SheetName].CopyFrom(workbook.Worksheets[k]);
                        //    WorkbookSplitNew.Worksheets.RemoveAt(0);
                        //    string tempSheetFileName = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + "Sheets_" + docName + '/' + _sheet.SheetName;
                        //    if (System.IO.File.Exists(tempSheetFileName))
                        //    {
                        //        System.IO.File.Delete(tempSheetFileName);
                        //    }
                        //    WorkbookSplitNew.SaveDocument(tempSheetFileName);

                        //}
                        _readyPortalDBEntities.SaveChanges();
                    }
                    if (System.IO.File.Exists(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + docName))
                    {
                        System.IO.File.Delete(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + docName);
                    }
                    WorkbookSplitOriginal.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + docName);

                }
                _readyPortalDBEntities.SaveChanges();

                var excel = (from itm in _readyPortalDBEntities.tblAssignedExcelSheets
                             where itm.AssignedExcelSheetID == assignedExcelSheetID
                             select itm).FirstOrDefault();

                if (excel != null)
                {
                    excel.IsSheetApproved = true;
                    _readyPortalDBEntities.Entry(excel).State = System.Data.Entity.EntityState.Modified;
                    _readyPortalDBEntities.SaveChanges();
                }
                ////Mark Task as Merged 
                var doc = (from itm in _readyPortalDBEntities.tblAssignments
                           where itm.AssignmentID == assignID
                           select itm).FirstOrDefault();

                var modifiedSlidesCount = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == assignID && z.IsSheetApproved == true).Count();
                var totalSlidesCount = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == assignID).Count();
                if (modifiedSlidesCount == totalSlidesCount)
                {
                    doc.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
                    doc.CompletedDate = DateTime.Now;
                    _readyPortalDBEntities.Entry(doc).State = System.Data.Entity.EntityState.Modified;
                    _entities.SaveChanges();
                }

                //Add Log 
                tblAssignmentLog _log = new tblAssignmentLog();
                _log.Action = (int)WordAutomationDemo.Common.Global.Action.Approved;
                _log.AssignmentID = doc.AssignmentID;
                _log.CreatedDate = DateTime.Now;
                _log.CreatedBy = CurrentUserSession.UserID;
                _log.Description = AssignedSheets != null && AssignedSheets.OriginalSheetName != null ? AssignedSheets.OriginalSheetName + " has been approved!" : "";
                _log.DocumentName = docName;
                _readyPortalDBEntities.tblAssignmentLogs.Add(_log);
                _readyPortalDBEntities.SaveChanges();

                var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                objAssignmentModelEmail.TaskName = doc.TaskName;
                objAssignmentModelEmail.Comments = doc.Comments;
                objAssignmentModelEmail.AssignID = doc.AssignmentID;
                List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                lstAssignmentLogModel = GetAssignmentLog(doc.AssignmentID);
                string EmailBody = templates.TemplateContentForEmail;
                var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == doc.LockedByUserID).FirstOrDefault();
                templates.Subject = templates.Subject.Replace("#Subject#", "Sheet Approved: Sheet has been approved by " + CurrentUserSession.User.FirstName);
                EmailBody = PopulateEmailBody("A sheet has been approved by " + CurrentUserSession.User.FirstName, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody);

                Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody);

                return true;
            }
        }

        [HttpGet]
        [ValidateLogin]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult EditExcel(string id, int Action = 1)
        {
            if (id == null)
            {
                return RedirectToAction("Approval", "Home");
            }
            else
            {
                string AID = Global.UrlDecrypt(id);
                int assignmentID;
                if (int.TryParse(AID, out assignmentID))
                {
                    List<ExcelModel> documentFileCollection = new List<ExcelModel>();
                    using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                    {
                        var assignment = _entities.tblAssignments.Where(a => a.AssignmentID == assignmentID).First();
                        if (Action == 2)
                        {
                            assignment.Status = (int)Global.AssignmentStatus.InProgress;
                        }
                        else
                        {
                            assignment.Status = (int)Global.AssignmentStatus.Completed;
                        }

                        _entities.SaveChanges();
                        string docname = assignment.tblDocument.DocumentName;
                        var sheets = new List<Aspose.Cells.Worksheet>();
                        using (var ms = new MemoryStream(GetExcelForAssignment(docname, assignmentID)))
                        //using (var doc = new Aspose.Cells.Workbook(ms, new Aspose.Cells.LoadOptions(Aspose.Cells.LoadFormat.Xlsx)))
                        using (var doc = new Aspose.Cells.Workbook(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, docname)))
                        {
                            foreach (var sheet in doc.Worksheets)
                            {
                                if (sheet.Cells.Rows.Count == 0 || sheet.Cells.MaxDataColumn == -1)
                                    continue;
                                sheets.Add(sheet);
                            }

                            if (sheets.Count == 0)
                                throw new Exception("Sorry, something went wrong. Please contact your administrator. ERR:4201");

                            var AssignedSlides = sheets.Select(sheet => new ExcelModel
                            {
                                AssignedExcelSheetID = sheet.TabId,
                                AssignedTaskID = assignment.AssignmentID,
                                SheetName = sheet.Name,
                                //AssignedTo = assign.UserID,
                                ProjectName = assignment.tblProject.Name,
                                OriginalFile = docname,
                                TaskName = assignment.TaskName,
                                //IsSheetModified = !ValidateChecksum(sheet),
                                SheetRemarks = assignment.Remarks,
                                IsSheetApproved = false,
                                IsGrayedOut = false
                            });

                            //Lock task to user.
                            assignment.LockedByUserID = CurrentUserSession.User.UserID;
                            _entities.SaveChanges();

                            documentFileCollection.AddRange(AssignedSlides.AsEnumerable().Select(i => new ExcelModel()
                            {
                                AssignedExcelSheetID = i.AssignedExcelSheetID,
                                AssignedTaskID = assignment.AssignmentID,
                                UserID = (int)assignment.LockedByUserID,
                                OriginalFile = i.OriginalFile,
                                SheetName = i.SheetName,
                                //IsSheetModified = i.IsSheetModified,
                                IsGrayedOut = i.IsGrayedOut,
                                IsSheetApproved = i.IsSheetApproved,
                                SheetRemarks = i.SheetRemarks,
                                SheetLink = "/Home/GetAssignedExcel?FileName=" + i.OriginalFile + "&userId=" + (int)assignment.LockedByUserID + "&AssignmentId=" + assignment.AssignmentID,
                            }).ToList());
                            foreach (var item in documentFileCollection)
                            {
                                item.lstModifiedSheet = new List<string>();
                                var linkOld = "/Home/GetAssignedExcel?FileName=" + item.OriginalFile + "&userId=" + item.UserID + "&AssignmentId=" + item.AssignmentID;
                                item.lstModifiedSheet.Add("No changes found");
                            }



                            var demoModel = new ExcelModel() { ListSheets = documentFileCollection, OriginalFile = AssignedSlides.FirstOrDefault().OriginalFile, AssignedTaskID = assignmentID, ProjectName = AssignedSlides.FirstOrDefault().ProjectName, TaskName = AssignedSlides.FirstOrDefault().TaskName };

                            //return PartialView("_LoadSnippet", demoModel);
                            return View("EditExcel", demoModel);
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Approval", "Home");
                }
            }
        }

        private bool? ValidateChecksum(Aspose.Cells.Worksheet sheet)
        {
            if (sheet.Cells.Rows.Count == 0 || sheet.Cells.MaxDataColumn == -1)
                return true;

            var formatter = new BinaryFormatter();
            Func<int, string> getChecksum = delegate (int row)
            {
                using (var md5 = MD5.Create())
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, sheet.Cells.ExportArray(row, 2, 1, sheet.Cells.MaxDataColumn - 2));
                    return Convert.ToBase64String(md5.ComputeHash(ms.ToArray()));
                }
            };

            foreach (Aspose.Cells.Row row in sheet.Cells.Rows)
            {
                string checksum = (row[1].Value as string ?? string.Empty).Replace("(Do Not Modify) Checksum ", "");
                if (!getChecksum(row.Index).Equals(checksum))
                    return false;
            }
            return true;
        }

        [HttpPost]
        [ValidateLogin]
        [ValidateUserPermission(Global.Actions.Assignment, Global.Controlers.Home)]
        public ActionResult EditExcel(ExcelModel model, List<HttpPostedFileBase> newfiles)
        {
            if (newfiles != null && newfiles.Count > 0 && !string.IsNullOrEmpty(model.OriginalFile))
            {
                //Save Uploaded file 
                var doc = (from itm in _entities.tblAssignments
                           join document in _entities.tblDocuments on itm.DocumentID equals document.DocumentID
                           //where itm.OrginalSourceFile == model.OriginalFile
                           where itm.AssignmentID == model.AssignedTaskID
                           select itm).FirstOrDefault();

                var Sheets = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == model.AssignedTaskID && !(z.IsSheetApproved == true)).ToList();

                if (Sheets.Count == newfiles.Count)
                {
                    for (int i = 0; i < newfiles.Count; i++)
                    {
                        if (newfiles[i] != null && newfiles[i].ContentLength > 0) // if new file is uploaded  then save it,
                        {
                            string fileExt = newfiles[i].FileName.Split('.').Last().ToLower();

                            if (fileExt == "xls")
                            {
                                Workbook workbook = new Workbook();

                                workbook.LoadDocument(newfiles[i].InputStream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                                workbook.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + doc.tblDocument.DocumentName + "/AssignedDoc/New_" + newfiles[i].FileName.Split('.').First().ToString() + ".xlsx");

                                int j = 0;
                                foreach (Worksheet worksheet in workbook.Worksheets)
                                {
                                    string SheetName = string.Empty;
                                    Workbook WorkbookSplit = new Workbook();
                                    SheetName = "Sheet" + (j + 1);
                                    WorkbookSplit.Worksheets[0].Name = "Sheet";
                                    WorkbookSplit.Worksheets.Add(SheetName);
                                    WorkbookSplit.Worksheets[SheetName].CopyFrom(workbook.Worksheets[j]);
                                    WorkbookSplit.Worksheets.RemoveAt(0);
                                    string tempSheetFileName = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + doc.tblDocument.DocumentName + "/AssignedDoc/New_" + newfiles[j].FileName.Split('.').First().ToString() + "(" + j + ")" + ".xlsx";
                                    if (System.IO.File.Exists(tempSheetFileName))
                                    {
                                        System.IO.File.Delete(tempSheetFileName);
                                    }
                                    WorkbookSplit.SaveDocument(tempSheetFileName);
                                    j++;
                                }
                            }
                            else
                            {
                                newfiles[i].SaveAs(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + doc.tblDocument.DocumentName + "/AssignedDoc/New_" + Sheets[i].AssignmentID + "_Copy_" + Sheets[i].SheetName);

                                Workbook workbook = new Workbook();

                                workbook.LoadDocument(newfiles[i].InputStream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);

                                int j = 0;
                                foreach (Worksheet worksheet in workbook.Worksheets)
                                {
                                    string SheetName = string.Empty;
                                    Workbook WorkbookSplit = new Workbook();
                                    SheetName = "Sheet" + (j + 1);
                                    WorkbookSplit.Worksheets[0].Name = "Sheet";
                                    WorkbookSplit.Worksheets.Add(SheetName);
                                    WorkbookSplit.Worksheets[SheetName].CopyFrom(workbook.Worksheets[j]);
                                    WorkbookSplit.Worksheets.RemoveAt(0);
                                    string tempSheetFileName = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + doc.tblDocument.DocumentName + "/AssignedDoc/New_" + Sheets[i].AssignmentID + "_Copy_" + Sheets[i].SheetName.Split('.').First().ToString() + "(" + j + ")" + ".xlsx";
                                    if (System.IO.File.Exists(tempSheetFileName))
                                    {
                                        System.IO.File.Delete(tempSheetFileName);
                                    }
                                    WorkbookSplit.SaveDocument(tempSheetFileName);
                                    j++;
                                }

                            }

                            string strSheetName = Sheets[i].SheetName;
                            var excelSheet = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == model.AssignedTaskID && z.SheetName == strSheetName).FirstOrDefault();
                            if (excelSheet != null)
                            {
                                excelSheet.IsSheetModified = true;
                                excelSheet.IsGrayedOut = true;
                                _entities.Entry(excelSheet).State = System.Data.Entity.EntityState.Modified;
                                _entities.SaveChanges();
                            }
                        }
                        else //else copy the same file as changes file 
                        {

                            //if (System.IO.File.Exists(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.OrginalSourceFile + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName))
                            //{
                            //    System.IO.File.Delete(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.OrginalSourceFile + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName);
                            //}

                            //System.IO.File.Copy(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.OrginalSourceFile + "/AssignedDoc/" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName, DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.OrginalSourceFile + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName);

                            ////save thumnails 
                            //var fileurl = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.OrginalSourceFile + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName;

                            ////Spire

                            //Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation();
                            //presentation.LoadFromFile(fileurl);
                            //for (int j = 0; j < presentation.Slides.Count; j++)
                            //{

                            //    //save the slide to Image
                            //    System.Drawing.Image image = presentation.Slides[j].SaveAsImage();
                            //    image.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + doc.OrginalSourceFile + "/AssignedDoc/New_" + slides[i].AssignmentID + "_Copy_" + slides[i].SlideName.Split('.').First().ToString() + "(" + j + ")" + WordAutomationDemo.Common.Global.ImageExportExtention, System.Drawing.Imaging.ImageFormat.Png);
                            //    presentation.Dispose();
                            //}
                        }
                    }

                    doc.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                    doc.Status = (int)Global.AssignmentStatus.Completed;
                    doc.CompletedDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(model.Remarks))
                    {
                        doc.Remarks = model.Remarks;
                        doc.Comments = model.Remarks;
                    }
                    _entities.Entry(doc).State = System.Data.Entity.EntityState.Modified;
                    _entities.SaveChanges();

                    //if (!string.IsNullOrEmpty(model.strExcelComments))
                    //{
                    //    var strRemarks = model.strExcelComments.Split('~');
                    //    foreach (var item in strRemarks)
                    //    {
                    //        if (!string.IsNullOrEmpty(item))
                    //        {
                    //            var AssignedExcelSheetID = item.Split('`').FirstOrDefault();
                    //            if (!string.IsNullOrEmpty(AssignedExcelSheetID))
                    //            {
                    //                int ExcelSheetID = Convert.ToInt32(AssignedExcelSheetID);
                    //                var excelSheet = _entities.tblAssignedExcelSheets.Where(z => z.AssignedExcelSheetID == ExcelSheetID).FirstOrDefault();
                    //                if (excelSheet != null)
                    //                {
                    //                    excelSheet.SheetRemarks = item.Split('`').LastOrDefault();
                    //                    _entities.Entry(excelSheet).State = System.Data.Entity.EntityState.Modified;

                    //                    tblAssignmentLog _logRemarks = new tblAssignmentLog();
                    //                    _logRemarks.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                    //                    _logRemarks.AssignmentID = doc.AssignmentID;
                    //                    _logRemarks.CreatedDate = DateTime.Now;
                    //                    _logRemarks.CreatedBy = CurrentUserSession.UserID;
                    //                    _logRemarks.Description = item.Split('`').LastOrDefault();
                    //                    _logRemarks.DocumentName = model.OriginalFile;
                    //                    _entities.tblAssignmentLogs.Add(_logRemarks);
                    //                    _entities.SaveChanges();
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    //Add Log 
                    tblAssignmentLog _log = new tblAssignmentLog();
                    _log.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                    _log.AssignmentID = doc.AssignmentID;
                    _log.CreatedDate = DateTime.Now;
                    _log.CreatedBy = CurrentUserSession.UserID;
                    _log.Description = model.Remarks;
                    _log.DocumentName = model.OriginalFile;
                    _entities.tblAssignmentLogs.Add(_log);
                    _entities.SaveChanges();

                    var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                    AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                    objAssignmentModelEmail.TaskName = doc.TaskName;
                    objAssignmentModelEmail.Comments = doc.Comments;
                    objAssignmentModelEmail.AssignID = doc.AssignmentID;
                    List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                    lstAssignmentLogModel = GetAssignmentLog(doc.AssignmentID);
                    string EmailBody = templates.TemplateContentForEmail;
                    var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == doc.CreatedBy).FirstOrDefault();
                    templates.Subject = templates.Subject.Replace("#Subject#", "Task Completed: Task has been completed by " + CurrentUserSession.User.FirstName);
                    EmailBody = PopulateEmailBody("A task has been Completed by " + CurrentUserSession.User.FirstName, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody, true);

                    Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody);

                    return RedirectToAction("Approval", "Home", new { @Msg = "Completed" });
                }
                else
                    return RedirectToAction("Approval", "Home", new { @Msg = "Error" });

            }

            return View();
        }

        //Load Document to Popup 
        public ActionResult LoadExcel(string originalDocumentName = "", string name = "", int userid = 0, int Action = 1, bool IsReadOnly = false)
        {
            var demoModel = new AssignmentModel() { OriginalDocumentName = "Sheets_" + originalDocumentName, DocumentName = name, UserID = CurrentUserSession.UserID, Action = Action, IsReadOnly = IsReadOnly };
            demoModel.Path = "/" + Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, name).Replace(Request.ServerVariables["APPL_PHYSICAL_PATH"], "").Replace(@"\", "/");
            return PartialView("_LoadExcel", demoModel);
        }

        public ActionResult SaveInlineExcel(object Spreadsheet)
        {
            var devExpressWb = SpreadsheetExtension.GetCurrentDocument("Spreadsheet");
            var filename = Path.GetFileName(devExpressWb.Options.Save.CurrentFileName);
            var filenameSplt = filename.Split('-');
            var assignmentID = Int32.Parse(filenameSplt[0]);
            var assignment = _entities.tblAssignments.Where(a => a.AssignmentID == assignmentID).First();

            if (assignment.Status == (int)Common.Global.AssignmentStatus.NotStarted)
                assignment.Status = (int)Common.Global.AssignmentStatus.InProgress;
            _entities.SaveChanges();

            var assignedUserID = Int32.Parse(filenameSplt[0]);
            var originalFilename = filenameSplt[2];
            using (var ms = new MemoryStream(SpreadsheetExtension.GetCurrentDocument("Spreadsheet").SaveDocument(DevExpress.Spreadsheet.DocumentFormat.Xlsx)))
            {
                var workbook = new Aspose.Cells.Workbook(ms, new Aspose.Cells.LoadOptions(Aspose.Cells.LoadFormat.Xlsx));
                workbook.Save(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\tmp\\" + filename);
                //SaveExcelForUser(workbook, assignmentID, originalFilename, filename);

            }

            List<ExcelModel> documentFileCollection = new List<ExcelModel>();
            using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
            {
                string docname = assignment.tblDocument.DocumentName;
                var sheets = new List<Aspose.Cells.Worksheet>();
                using (var ms = new MemoryStream(GetExcelForAssignment(docname, assignmentID)))
                using (var doc = new Aspose.Cells.Workbook(ms, new Aspose.Cells.LoadOptions(Aspose.Cells.LoadFormat.Xlsx)))
                {
                    foreach (var sheet in doc.Worksheets)
                    {
                        if (sheet.Cells.Rows.Count == 0 || sheet.Cells.MaxDataColumn == -1)
                            continue;
                        sheets.Add(sheet);
                    }

                    var AssignedSlides = sheets.Select(sheet => new ExcelModel
                    {
                        AssignedExcelSheetID = sheet.TabId,
                        AssignedTaskID = assignment.AssignmentID,
                        SheetName = sheet.Name,
                        //AssignedTo = assign.UserID,
                        ProjectName = assignment.tblProject.Name,
                        OriginalFile = docname,
                        TaskName = assignment.TaskName,
                        //IsSheetModified = !ValidateChecksum(sheet),
                        SheetRemarks = assignment.Remarks,
                        IsSheetApproved = false,
                        IsGrayedOut = false
                    });

                    documentFileCollection.AddRange(AssignedSlides.AsEnumerable().Select(i => new ExcelModel()
                    {
                        AssignedExcelSheetID = i.AssignedExcelSheetID,
                        AssignedTaskID = assignment.AssignmentID,
                        UserID = assignment.LockedByUserID,
                        OriginalFile = i.OriginalFile,
                        SheetName = i.SheetName,
                        IsSheetModified = i.IsSheetModified,
                        IsGrayedOut = i.IsGrayedOut,
                        IsSheetApproved = i.IsSheetApproved,
                        SheetRemarks = i.SheetRemarks,
                        SheetLink = "/Home/GetAssignedExcel?FileName=" + i.OriginalFile + "&userId=" + assignment.LockedByUserID + "&AssignmentId=" + assignment.AssignmentID,
                    }).ToList());
                    foreach (var item in documentFileCollection)
                    {
                        item.lstModifiedSheet = new List<string>();
                        var linkOld = "/Home/GetAssignedExcel?FileName=" + item.OriginalFile + "&userId=" + item.UserID + "&AssignmentId=" + item.AssignmentID;
                        item.lstModifiedSheet.Add("No changes found");
                    }

                    var demoModel = new ExcelModel() { ListSheets = documentFileCollection, OriginalFile = AssignedSlides.FirstOrDefault().OriginalFile, AssignedTaskID = assignmentID, ProjectName = AssignedSlides.FirstOrDefault().ProjectName, TaskName = AssignedSlides.FirstOrDefault().TaskName };

                    //return PartialView("_LoadSnippet", demoModel);
                    return View("EditExcel", demoModel);
                }
            }
        }

        public ActionResult PublishExcel(int assignmentID, string originalFile, string remarks = "")
        {
            var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == assignmentID);
            assignment.Comments = remarks;
            var path = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\tmp\\" + assignment.AssignmentID.ToString() + "-c-" + originalFile;
            var workbook = new Aspose.Cells.Workbook(path);
            SaveExcelForUser(workbook, assignmentID, originalFile, path);

            //Update task status
            _entities.Entry(assignment).State = System.Data.Entity.EntityState.Modified;
            assignment.Action = (int)WordAutomationDemo.Common.Global.Action.Published;
            assignment.CompletedDate = DateTime.Now;

            //Add log
            tblAssignmentLog _log = new tblAssignmentLog();
            _log.Action = (int)WordAutomationDemo.Common.Global.Action.Published;
            _log.AssignmentID = assignment.AssignmentID;
            _log.Description = assignment.Comments;
            _log.CreatedDate = DateTime.Now;
            _log.CreatedBy = CurrentUserSession.UserID;
            _entities.tblAssignmentLogs.Add(_log);

            //Remove assigned rows
            var excelRowMaps = _entities.tblExcelRowMaps.Where(rm => rm.AssignmentID == assignment.AssignmentID).ToList();
            _entities.tblExcelRowMaps.RemoveRange(excelRowMaps);
            _entities.SaveChanges();

            //Send notification
            string message = string.Empty;
            string strEmailMessage = string.Empty;
            string strEmailSubject = string.Empty;
            strEmailMessage = "A task has been Finished by " + CurrentUserSession.User.FirstName;
            strEmailSubject = "Task Finished: Task has been finished by " + CurrentUserSession.User.FirstName;
            message = "1";

            var templates = CommonHelper.GetEmailTemplateContent("Finished Task");

            AssignmentModel objAssignmentModelEmail = new AssignmentModel();
            objAssignmentModelEmail.TaskName = assignment.TaskName;
            objAssignmentModelEmail.Comments = assignment.Comments;
            objAssignmentModelEmail.AssignID = assignment.AssignmentID;
            List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
            lstAssignmentLogModel = GetAssignmentLog(assignment.AssignmentID);
            string EmailBody = templates.TemplateContentForEmail;

            var assignmentMemberIDs = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == assignment.AssignmentID && am.UserID != CurrentUserSession.User.UserID).Select(u => u.UserID).ToList();
            var UsersToNotify = _entities.tblUserDepartments.Where(x => assignmentMemberIDs.Contains(x.UserId)).ToList();

            foreach (var UserToNotify in UsersToNotify)
            {
                templates.Subject = templates.Subject.Replace("#Subject#", strEmailSubject);
                EmailBody = PopulateEmailBody(strEmailMessage, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody, true);

                Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody);
            }

            return RedirectToAction("Approval", "Home");
        }

        [HttpPost]
        public string CompleteSheet(FormCollection objFormCollection)
        {
            Result objAvailabilityResult = new Result();
            if (objFormCollection != null && objFormCollection["assignmentID"] != null && objFormCollection["assignedExcelSheetID"] != null && objFormCollection["originalFile"] != null && objFormCollection["sheetName"] != null)
            {
                int assignmentID = Convert.ToInt32(objFormCollection["assignmentID"]);
                int assignedExcelSheetID = Convert.ToInt32(objFormCollection["assignedExcelSheetID"]);
                string originalFile = objFormCollection["originalFile"];
                string sheetName = objFormCollection["sheetName"];
                if (Request.Files != null && Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                        {
                            var fileName = assignmentID + "_Copy_" + sheetName;
                            tblAssignment _tblAssignment = _entities.tblAssignments.Where(c => c.AssignmentID == assignmentID).FirstOrDefault();
                            if (_tblAssignment != null)
                            {
                                if (_tblAssignment.Action != (int)WordAutomationDemo.Common.Global.Action.Completed)
                                {
                                    Aspose.Slides.Presentation slidepresentation = new Aspose.Slides.Presentation();
                                    var Path = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\Sheets_" + originalFile + "\\AssignedDoc\\" + fileName;

                                    if (System.IO.File.Exists(Path))
                                    {

                                        Workbook workbook = new Workbook();

                                        workbook.LoadDocument(file.InputStream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                                        workbook.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + _tblAssignment.tblDocument.DocumentName + "/AssignedDoc/New_" + fileName.Split('.').First().ToString() + ".xlsx");

                                        int i = 0;
                                        foreach (Worksheet worksheet in workbook.Worksheets)
                                        {
                                            string SheetName = string.Empty;
                                            Workbook WorkbookSplit = new Workbook();
                                            SheetName = worksheet.Name;
                                            WorkbookSplit.Worksheets[0].Name = "Sheet";
                                            WorkbookSplit.Worksheets.Add(SheetName);
                                            WorkbookSplit.Worksheets[SheetName].CopyFrom(workbook.Worksheets[i]);
                                            WorkbookSplit.Worksheets.RemoveAt(0);
                                            string tempSheetFileName = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + _tblAssignment.tblDocument.DocumentName + "/AssignedDoc/New_" + fileName.Split('.').First().ToString() + "(" + i + ")" + ".xlsx";
                                            if (System.IO.File.Exists(tempSheetFileName))
                                            {
                                                System.IO.File.Delete(tempSheetFileName);
                                            }
                                            WorkbookSplit.SaveDocument(tempSheetFileName);
                                            i++;
                                        }

                                        var excelSheet = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == assignmentID && z.SheetName == sheetName).FirstOrDefault();
                                        if (excelSheet != null)
                                        {
                                            if (objFormCollection["SheetRemarks"] != null)
                                            {
                                                excelSheet.SheetRemarks = objFormCollection["SheetRemarks"];
                                            }
                                            excelSheet.IsSheetModified = true;
                                            excelSheet.IsGrayedOut = true;
                                            _entities.Entry(excelSheet).State = System.Data.Entity.EntityState.Modified;
                                        }

                                        //SplitSlides(objPPTDocModel.FileName);
                                        //_tblAssignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                                        _tblAssignment.CompletedDate = DateTime.Now;
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
                                        _log.CreatedBy = CurrentUserSession.UserID;
                                        if (objFormCollection["SheetRemarks"] != null)
                                        {
                                            _log.Description = objFormCollection["SheetRemarks"];
                                        }
                                        //_log.Description = slideName + " has been updated from power point";
                                        //_log.DocumentName = model.OriginalFile;
                                        _entities.tblAssignmentLogs.Add(_log);
                                        _entities.SaveChanges();

                                        string message = string.Empty;
                                        string strEmailMessage = string.Empty;
                                        string strEmailSubject = string.Empty;
                                        var modifiedSheetsCount = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == assignmentID && z.IsSheetModified == true).Count();
                                        var totalSheetsCount = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == assignmentID).Count();
                                        if (modifiedSheetsCount == totalSheetsCount)
                                        {
                                            _tblAssignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                                            _tblAssignment.CompletedDate = DateTime.Now;
                                            _entities.SaveChanges();
                                            strEmailMessage = "A task has been Completed by " + CurrentUserSession.User.FirstName;
                                            strEmailSubject = "Task Completed: Task has been completed by " + CurrentUserSession.User.FirstName;
                                            message = "1";
                                        }
                                        else
                                        {
                                            strEmailMessage = "A sheet has been Completed by " + CurrentUserSession.User.FirstName;
                                            strEmailSubject = "Sheet Completed: sheet has been completed by " + CurrentUserSession.User.FirstName;
                                            message = "2";
                                        }

                                        var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                                        AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                                        objAssignmentModelEmail.TaskName = _tblAssignment.TaskName;
                                        objAssignmentModelEmail.Comments = _tblAssignment.Comments;
                                        objAssignmentModelEmail.AssignID = _tblAssignment.AssignmentID;
                                        List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                                        lstAssignmentLogModel = GetAssignmentLog(_tblAssignment.AssignmentID);
                                        string EmailBody = templates.TemplateContentForEmail;
                                        var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == _tblAssignment.CreatedBy).FirstOrDefault();
                                        templates.Subject = templates.Subject.Replace("#Subject#", strEmailSubject);
                                        EmailBody = PopulateEmailBody(strEmailMessage, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody, true);

                                        Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody);

                                        objAvailabilityResult.Status = true;
                                        objAvailabilityResult.ErrorCode = "";
                                        objAvailabilityResult.ErrorMessage = message;
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
                    }
                }
                else
                {
                    using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                    {
                        tblAssignment _tblAssignment = _entities.tblAssignments.Where(c => c.AssignmentID == assignmentID).FirstOrDefault();
                        if (_tblAssignment != null)
                        {
                            var excelSheet = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == assignmentID && z.SheetName == sheetName).FirstOrDefault();
                            if (excelSheet != null)
                            {
                                if (objFormCollection["SheetRemarks"] != null)
                                {
                                    excelSheet.SheetRemarks = objFormCollection["SheetRemarks"];
                                }
                                excelSheet.IsSheetModified = true;
                                excelSheet.IsGrayedOut = true;
                                _entities.Entry(excelSheet).State = System.Data.Entity.EntityState.Modified;
                            }

                            //Add Log 
                            tblAssignmentLog _log = new tblAssignmentLog();
                            _log.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                            _log.AssignmentID = _tblAssignment.AssignmentID;
                            _log.CreatedDate = DateTime.Now;
                            _log.CreatedBy = CurrentUserSession.UserID;
                            if (objFormCollection["SheetRemarks"] != null)
                            {
                                _log.Description = objFormCollection["SheetRemarks"];
                            }
                            //_log.DocumentName = model.OriginalFile;
                            _entities.tblAssignmentLogs.Add(_log);
                            _entities.SaveChanges();

                            string message = string.Empty;
                            string strEmailMessage = string.Empty;
                            string strEmailSubject = string.Empty;
                            var modifiedSheetCount = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == assignmentID && z.IsSheetModified == true).Count();
                            var totalSheetCount = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == assignmentID).Count();
                            if (modifiedSheetCount == totalSheetCount)
                            {
                                _tblAssignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                                _tblAssignment.CompletedDate = DateTime.Now;
                                _entities.SaveChanges();
                                strEmailMessage = "A task has been Completed by " + CurrentUserSession.User.FirstName;
                                strEmailSubject = "Task Completed: Task has been completed by " + CurrentUserSession.User.FirstName;
                                message = "1";
                            }
                            else
                            {
                                strEmailMessage = "A sheet has been Completed by " + CurrentUserSession.User.FirstName;
                                strEmailSubject = "Sheet Completed: sheet has been completed by " + CurrentUserSession.User.FirstName;
                                message = "2";
                            }

                            var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                            AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                            objAssignmentModelEmail.TaskName = _tblAssignment.TaskName;
                            objAssignmentModelEmail.Comments = _tblAssignment.Comments;
                            objAssignmentModelEmail.AssignID = _tblAssignment.AssignmentID;
                            List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                            lstAssignmentLogModel = GetAssignmentLog(_tblAssignment.AssignmentID);
                            string EmailBody = templates.TemplateContentForEmail;
                            var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == _tblAssignment.CreatedBy).FirstOrDefault();
                            templates.Subject = templates.Subject.Replace("#Subject#", strEmailSubject);
                            EmailBody = PopulateEmailBody(strEmailMessage, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody, true);

                            Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody);

                            objAvailabilityResult.Status = true;
                            objAvailabilityResult.ErrorCode = "";
                            objAvailabilityResult.ErrorMessage = message;
                        }
                    }
                }
            }
            return objAvailabilityResult.ErrorMessage;
        }

        [HttpPost]
        public string CompleteInlineSheet(int assignmentID)
        {
            Result objAvailabilityResult = new Result();
            //sheetName = assignmentID + "_Copy_" + sheetName;
            tblAssignment _tblAssignment = _entities.tblAssignments.Where(c => c.AssignmentID == assignmentID).FirstOrDefault();
            if (_tblAssignment != null)
            {
                //var Path = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\Sheets_" + _tblAssignment.tblDocument.DocumentName + "\\AssignedDoc\\" + sheetName;
                //if (System.IO.File.Exists(Path))
                //{
                //    Workbook workbook = new Workbook();
                //    workbook.LoadDocument(Path, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                //    workbook.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "\\Sheets_" + _tblAssignment.tblDocument.DocumentName + "\\AssignedDoc\\New_" + sheetName, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                //    int j = 0;
                //    foreach (Worksheet worksheet in workbook.Worksheets)
                //    {
                //        string SheetName = string.Empty;
                //        Workbook WorkbookSplit = new Workbook();
                //        SheetName = worksheet.Name;
                //        WorkbookSplit.Worksheets[0].Name = "Sheet";
                //        WorkbookSplit.Worksheets.Add(SheetName);
                //        WorkbookSplit.Worksheets[SheetName].CopyFrom(workbook.Worksheets[j]);
                //        WorkbookSplit.Worksheets.RemoveAt(0);
                //        string tempSheetFileName = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + _tblAssignment.tblDocument.DocumentName + "/AssignedDoc/New_" + sheetName.Split('.').First().ToString() + "(" + j + ")" + ".xlsx";
                //        if (System.IO.File.Exists(tempSheetFileName))
                //        {
                //            System.IO.File.Delete(tempSheetFileName);
                //        }
                //        WorkbookSplit.SaveDocument(tempSheetFileName);
                //        j++;
                //    }
                //}
                //var excelSheet = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == assignmentID && z.AssignedExcelSheetID == assignedExcelSheetID).FirstOrDefault();
                //if (excelSheet != null)
                //{
                //    if (!string.IsNullOrEmpty(SheetRemarks))
                //    {
                //        excelSheet.SheetRemarks = SheetRemarks;
                //    }
                //    excelSheet.IsSheetModified = true;
                //    excelSheet.IsGrayedOut = true;
                //    _entities.Entry(excelSheet).State = System.Data.Entity.EntityState.Modified;
                //}
                _tblAssignment.CompletedDate = DateTime.Now;
                _entities.Entry(_tblAssignment).State = System.Data.Entity.EntityState.Modified;
                //Add Log 
                tblAssignmentLog _log = new tblAssignmentLog();
                _log.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                _log.AssignmentID = _tblAssignment.AssignmentID;
                _log.CreatedDate = DateTime.Now;
                _log.CreatedBy = CurrentUserSession.UserID;
                //if (!string.IsNullOrEmpty(SheetRemarks))
                //{
                //    _log.Description = SheetRemarks;
                //}
                _entities.tblAssignmentLogs.Add(_log);
                _entities.SaveChanges();

                string message = string.Empty;
                string strEmailMessage = string.Empty;
                string strEmailSubject = string.Empty;
                var modifiedSheetsCount = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == assignmentID && z.IsSheetModified == true).Count();
                var totalSheetsCount = _entities.tblAssignedExcelSheets.Where(z => z.AssignmentID == assignmentID).Count();
                if (modifiedSheetsCount == totalSheetsCount)
                {
                    _tblAssignment.Action = (int)WordAutomationDemo.Common.Global.Action.Completed;
                    _tblAssignment.CompletedDate = DateTime.Now;
                    _entities.SaveChanges();
                    strEmailMessage = "A task has been Completed by " + CurrentUserSession.User.FirstName;
                    strEmailSubject = "Task Completed: Task has been completed by " + CurrentUserSession.User.FirstName;
                    message = "1";
                }
                else
                {
                    strEmailMessage = "A sheet has been Completed by " + CurrentUserSession.User.FirstName;
                    strEmailSubject = "Sheet Completed: sheet has been completed by " + CurrentUserSession.User.FirstName;
                    message = "2";
                }

                var templates = CommonHelper.GetEmailTemplateContent("Task Notification");

                AssignmentModel objAssignmentModelEmail = new AssignmentModel();
                objAssignmentModelEmail.TaskName = _tblAssignment.TaskName;
                objAssignmentModelEmail.Comments = _tblAssignment.Comments;
                objAssignmentModelEmail.AssignID = _tblAssignment.AssignmentID;
                List<AssignmentLogModel> lstAssignmentLogModel = new List<AssignmentLogModel>();
                lstAssignmentLogModel = GetAssignmentLog(_tblAssignment.AssignmentID);
                string EmailBody = templates.TemplateContentForEmail;
                var UserToNotify = _entities.tblUserDepartments.Where(x => x.UserId == _tblAssignment.CreatedBy).FirstOrDefault();
                templates.Subject = templates.Subject.Replace("#Subject#", strEmailSubject);
                EmailBody = PopulateEmailBody(strEmailMessage, UserToNotify.FullName, objAssignmentModelEmail, lstAssignmentLogModel, EmailBody, true);

                Global.SendEmail(UserToNotify.EmailID, templates.Subject, EmailBody);

                objAvailabilityResult.Status = true;
                objAvailabilityResult.ErrorCode = "";
                objAvailabilityResult.ErrorMessage = message;
            }
            return objAvailabilityResult.ErrorMessage;
        }

        public string SplitSheets(string FileName)
        {
            string returnString = "success";
            string SheetName = string.Empty;
            var exceptionMessage = string.Empty;
            Workbook workbook = new Workbook();

            // Load a workbook from the stream. 
            using (FileStream stream = new FileStream(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + FileName, FileMode.Open))
            {
                // workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                workbook.LoadDocument(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + FileName);
            }

            int i = 0;
            foreach (Worksheet worksheet in workbook.Worksheets)
            {
                Workbook WorkbookSplit = new Workbook();
                SheetName = worksheet.Name;
                WorkbookSplit.Worksheets[0].Name = "Sheet";
                WorkbookSplit.Worksheets.Add(SheetName);
                WorkbookSplit.Worksheets[SheetName].CopyFrom(workbook.Worksheets[i]);
                try
                {
                    WorkbookSplit.Worksheets.RemoveAt(0);
                }
                catch (InvalidOperationException)
                {
                    // Fails gracefully on the last sheet
                }
                string tempSheetFileName = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + "Sheets_" + FileName + '/' + SheetName + ".xlsx";
                if (System.IO.File.Exists(tempSheetFileName))
                {
                    System.IO.File.Delete(tempSheetFileName);
                }
                WorkbookSplit.SaveDocument(tempSheetFileName);

                tblExcelSheet _Sheet = new tblExcelSheet();
                _Sheet.SheetName = SheetName + ".xlsx";
                _Sheet.Sequence = i + 1;
                _Sheet.MasterDocumentName = FileName;
                _entities.tblExcelSheets.Add(_Sheet);
                tblExcelSheet _sheetOriginal = new tblExcelSheet();
                _sheetOriginal.SheetName = SheetName;
                _sheetOriginal.Sequence = i + 1;
                _sheetOriginal.MasterDocumentName = FileName;
                _sheetOriginal.IsOriginal = true;
                _entities.tblExcelSheets.Add(_sheetOriginal);
                i++;

                GC.Collect();
                _entities.SaveChanges();
            }

            if (!string.IsNullOrEmpty(exceptionMessage))
                SendErrorToText(exceptionMessage, "SplitSheet");

            return returnString;
        }

        internal void PidExcelRows(Aspose.Cells.Workbook workbook, string docName)
        {
            foreach (var sheet in workbook.Worksheets)
            {
                // Skip empty sheets
                if (sheet.Cells.Rows.Count == 0 || sheet.Cells.MaxDataColumn == -1)
                    continue;

                // Create tracker columns
                if (!((sheet.Cells["A1"].Value as string)?.StartsWith("(Do Not Modify) Triyo") ?? false))
                {
                    sheet.Cells.InsertColumn(0, true);
                    sheet.Cells.InsertColumn(1, true);
                }
                var data = sheet.Cells.ExportDataTable(0, 0, sheet.Cells.MaxDataRow + 1, 2);

                // Possibly the fastest way of pidding the Excel doc with guids and assigning ownership in the DB
                var formatter = new BinaryFormatter();
                var newGuids = new List<Guid>();
                Func<Guid, Guid> addNewGuid = delegate (Guid g) { newGuids.Add(g); return g; };
                Func<int, string> getChecksum = delegate (int row)
                {
                    using (var md5 = MD5.Create())
                    using (var ms = new MemoryStream())
                    {
                        formatter.Serialize(ms, sheet.Cells.ExportArray(row, 2, 1, sheet.Cells.MaxDataColumn - 1));
                        return Convert.ToBase64String(md5.ComputeHash(ms.ToArray()));
                    }
                };
                var existingRowIds = new HashSet<Guid>(_entities.tblExcelRowMaps.Where(r => r.MasterDocumentName == docName).Select(r => r.RowId));
                for (int i = 0; i < data.Rows.Count; i++)
                    (data.Rows[i][0], data.Rows[i][1]) = data.Rows[i][0] is DBNull ? (addNewGuid(Guid.NewGuid()), getChecksum(i)) : (data.Rows[i][0], data.Rows[i][1]);
                sheet.Cells.ImportDataTable(data, false, 0, 0, false);
                sheet.Cells["A1"].Value = "(Do Not Modify) Triyo " + data.Rows[0][0];
                sheet.Cells["B1"].Value = "(Do Not Modify) Checksum " + data.Rows[0][1];
                //var newRowMappings = newGuids.Select(rowid => new tblExcelRowMap()
                //{
                //    DateLastModified = DateTime.Now,
                //    IsActive = true,
                //    IsRemoved = false,
                //    MasterDocumentName = docName,
                //    RowId = rowid,
                //    UserId = CurrentUserSession.UserID
                //}).ToArray();
                //_entities.BulkInsert(newRowMappings);

                // Hide tracker columns
                sheet.Cells.Columns[0].IsHidden = true;
                sheet.Cells.Columns[1].IsHidden = true;
                data.Dispose();
            }
        }

        public ActionResult UpdateSpreadSheet(int AssignmentID, string documentID)
        {
            //pwPDrive _obj = new pwPDrive();
            IWorkbook book = SpreadsheetExtension.GetCurrentDocument("spreadsheetInline");
            byte[] _docData = book.SaveDocument(DevExpress.Spreadsheet.DocumentFormat.Xlsx);

            Workbook workbook = new Workbook();
            workbook.LoadDocument(_docData, DevExpress.Spreadsheet.DocumentFormat.Xlsx);

            if (System.IO.File.Exists(documentID))
            {
                System.IO.File.Delete(documentID);
                workbook.SaveDocument(documentID);

                //Workbook workbook = new Workbook();
                //workbook.LoadDocument(_docData, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                //// Load a workbook from the stream. 
                //using (FileStream stream = new FileStream(_docData, FileMode.Open))
                //{
                //    workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                //    string strManager = workbook.DocumentProperties.Manager;
                //    //workbook.DocumentProperties.Manager = CurrentUserSession.UserID + "|" + assignementId + "|" + "Sheets_" + AssignmentModel.DocumentName + "|" + assignementId + "_Copy_" + AssignmentModel.SelectedSheets[i];
                //    //workbook.SaveDocument(documentID + "/AssignedDoc/" + assignementId + "_Copy_" + AssignmentModel.SelectedSheets[i], DevExpress.Spreadsheet.DocumentFormat.Xlsx);
                //}
            }

            //Workbook workbook = new Workbook();
            //workbook.LoadDocument(_docData, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
            //string strManager = workbook.DocumentProperties.Manager;
            //workbook.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + workbook.DocumentProperties.Title, DevExpress.Spreadsheet.DocumentFormat.Xlsx);

            ////_obj.updateDocument(_docData, DOCID, _ownerID);
            ViewBag.DocTitle = AssignmentID;
            return PartialView("_SpreadsheetPartial");
        }

        public int SplitCompletedSheets(string OriginalFile, string FileName, int StartIndex, int StartSeq, string originalSlideName, int assignID)
        {
            int FirstEntryID = 0;

            var savepptid = StartIndex;
            string fileName = string.Empty;
            string fileextenstion = '.' + FileName.Split('.').Last();
            int totalSheetCount = 0;
            Workbook workbook = new Workbook();
            string tempSheetFileName = "";

            //var fileInfoMain = System.IO.File.Open(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + OriginalFile, FileMode.Open);

            //using (MemoryStream MainStream = new MemoryStream())
            //{
            //    fileInfoMain.CopyTo(MainStream);
            //    foreach (var item in FileList)
            //    {
            //        var fileInfoChild = System.IO.File.Open(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/Slides_" + OriginalFile + "/" + item, FileMode.Open);
            //        using (MemoryStream ChildStream = new MemoryStream())
            //        {
            //            fileInfoChild.CopyTo(ChildStream);

            //            ChildStream.Seek(0, SeekOrigin.Begin);

            //            MainStream.Seek(0, SeekOrigin.Begin);

            //            MergeSlidesStream(ChildStream, MainStream);

            //            //fileInfoChild.Close();
            //        }
            //    }
            //}

            using (FileStream stream = new FileStream(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + OriginalFile, FileMode.Open))
            {
                workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                Workbook WorkbookSplit = new Workbook();
                int worksheetIndex = Convert.ToInt32(originalSlideName.Replace("Sheet", "").Split('.')[0]) - 1;
                int workSheetCount = 0;
                for (int k = 0; k < workbook.Worksheets.Count(); k++)
                {
                    if (k == worksheetIndex)
                    {
                        var fileurl = DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Sheets_" + OriginalFile + "/AssignedDoc/" + ("New_" + assignID.ToString() + "_Copy_Sheet" + (worksheetIndex + 1) + ".xlsx");
                        if (System.IO.File.Exists(fileurl))
                        {
                            using (FileStream streamNew = new FileStream(fileurl, FileMode.Open))
                            {
                                Workbook WorkbookNew = new Workbook();
                                WorkbookNew.LoadDocument(streamNew, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                                for (int j = 0; j < WorkbookNew.Worksheets.Count(); j++)
                                {
                                    fileName = "Sheet" + savepptid + fileextenstion;
                                    if (workSheetCount == 0)
                                    {
                                        WorkbookSplit.Worksheets[workSheetCount].Name = "Sheet";
                                    }
                                    WorkbookSplit.Worksheets.Add("Sheet" + (workSheetCount + 1));
                                    WorkbookSplit.Worksheets["Sheet" + (workSheetCount + 1)].CopyFrom(WorkbookNew.Worksheets[j]);
                                    workSheetCount++;

                                    var oldSheet = _entities.tblExcelSheets.Where(f => f.MasterDocumentName == OriginalFile && f.SheetName == "Sheet" + (workSheetCount + 1) + ".xlsx" && f.IsOriginal == null).FirstOrDefault();
                                    if (oldSheet != null)
                                    {
                                        _entities.tblExcelSheets.Remove(oldSheet);
                                        _entities.SaveChanges();
                                    }

                                    tblExcelSheet _sheet = new tblExcelSheet();
                                    _sheet.SheetName = "Sheet" + (workSheetCount + 1) + ".xlsx";
                                    _sheet.Sequence = StartSeq;
                                    _sheet.MasterDocumentName = OriginalFile;
                                    _entities.tblExcelSheets.Add(_sheet);
                                    _entities.SaveChanges();

                                    if (j == 0)
                                    {
                                        FirstEntryID = _sheet.ExcelSheetID;
                                    }

                                    savepptid++;
                                    StartSeq++;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (workSheetCount == 0)
                        {
                            WorkbookSplit.Worksheets[workSheetCount].Name = "Sheet";
                        }
                        WorkbookSplit.Worksheets.Add("Sheet" + (workSheetCount + 1));
                        WorkbookSplit.Worksheets["Sheet" + (workSheetCount + 1)].CopyFrom(workbook.Worksheets[k]);
                        workSheetCount++;
                    }
                }
                stream.Dispose();
                stream.Close();
                if (System.IO.File.Exists(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + OriginalFile))
                {
                    System.IO.File.Delete(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + OriginalFile);
                }
                WorkbookSplit.Worksheets.RemoveAt(0);
                WorkbookSplit.SaveDocument(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/" + OriginalFile);
            }

            //for (int j = 0; j < totalSheetCount; j++)
            //{
            //    //using (MemoryStream MainStreamActualFile = new MemoryStream(System.IO.File.ReadAllBytes(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + FileName + '/' + FileName)))
            //    using (MemoryStream MainStreamActualFile = new MemoryStream(System.IO.File.ReadAllBytes(DirectoryManagmentUtils.InitialDemoFilesPathExcels + "/Slides_" + OriginalFile + "/AssignedDoc/" + FileName)))
            //    {
            //        MainStreamActualFile.Seek(0, SeekOrigin.Begin);
            //        DeleteSlide(MainStreamActualFile, j);

            //        MainStreamActualFile.Seek(0, SeekOrigin.Begin);

            //        fileName = savepptid + fileextenstion;
            //        var thumbfileName = savepptid + WordAutomationDemo.Common.Global.ImageExportExtention;

            //        //fileName = FileName.Substring(0, FileName.Length - minusfilelength) + "-" + savepptid + fileextenstion;
            //        string fileurl = System.IO.Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + OriginalFile + "/" + fileName).Replace(@"\\\", "/");

            //        string tempSlideFileName = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + OriginalFile + '/' + fileName;
            //        if (System.IO.File.Exists(tempSlideFileName))
            //        {
            //            System.IO.File.Delete(tempSlideFileName);
            //        }
            //        using (var fs1 = new FileStream(tempSlideFileName, FileMode.OpenOrCreate))
            //        {
            //            MainStreamActualFile.CopyTo(fs1);
            //        }


            //        using (MemoryStream MainStreamSaveFile = new MemoryStream(System.IO.File.ReadAllBytes(tempSlideFileName)))
            //        {

            //            System.IO.File.WriteAllBytes(fileurl, MainStreamSaveFile.ToArray());


            //            //get thumbnails 


            //            //Application pptApplication = new Application();
            //            //Microsoft.Office.Interop.PowerPoint.Presentation pptPresentation = pptApplication.Presentations.Open(fileurl, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);
            //            //pptPresentation.Slides[1].Export(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + OriginalFile + "/" + thumbfileName, WordAutomationDemo.Common.Global.ImageExportExtention, 800, 600);
            //            //pptPresentation.Close();

            //            //Spire 


            //            Aspose.Slides.Presentation presentation = new Aspose.Slides.Presentation();
            //            presentation.LoadFromFile(fileurl);
            //            //save the slide to Image
            //            System.Drawing.Image image = presentation.Slides[0].SaveAsImage();
            //            image.Save(DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + OriginalFile + "/" + thumbfileName, System.Drawing.Imaging.ImageFormat.Png);
            //            presentation.Dispose();


            //            tblPPTSlide _slide = new tblPPTSlide();
            //            _slide.SlideName = fileName;
            //            _slide.Sequence = StartSeq;
            //            _slide.MasterDocumentName = OriginalFile;
            //            _entities.tblPPTSlides.Add(_slide);
            //            _entities.SaveChanges();

            //            if (j == 0)
            //            {
            //                FirstEntryID = _slide.PPTSlidesID;
            //            }

            //            savepptid++;
            //            StartSeq++;
            //        }


            //    }
            //}
            //var OriginalFilePath = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + OriginalFile + '/' + originalSlideName;
            //var ThumbFilePath = DirectoryManagmentUtils.InitialDemoFilesPathPPTs + "/" + "Slides_" + OriginalFile + "/" + originalSlideName.Split('.').FirstOrDefault().ToString() + WordAutomationDemo.Common.Global.ImageExportExtention;

            //if (System.IO.File.Exists(OriginalFilePath))
            //    System.IO.File.Delete(OriginalFilePath); // delete old slides 

            //if (System.IO.File.Exists(ThumbFilePath))
            //    System.IO.File.Delete(ThumbFilePath); // delete thumbnails of old slides 

            //var _slideToDelete = _entities.tblPPTSlides.Where(z => z.SlideName == originalSlideName && z.MasterDocumentName == OriginalFile && !(z.IsOriginal == true)).FirstOrDefault();
            //_entities.tblPPTSlides.Remove(_slideToDelete);
            //_entities.SaveChanges();
            return FirstEntryID;
        }

        public static int CountSheets(Workbook objWorkbook)
        {
            // Check for a null document object.
            if (objWorkbook == null)
            {
                throw new ArgumentNullException("excelDocument");
            }

            int sheetCount = 0;

            sheetCount = objWorkbook.Worksheets.Count();

            // Return the slide count to the previous method.
            return sheetCount;
        }

        #endregion

    }
}