using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;

namespace WordAutomationDemo.Models
{

    public class AssignmentModel
    {

        //public string Status
        //{
        //    get
        //    {
        //        if (this.Action > 0)
        //            return Common.Global.GetEnumDescription((Common.Global.Action)this.Action);
        //        else
        //            return string.Empty;
        //    }
        //}
        public string AssignmentStatus
        {
            get
            {
                return CommonHelper.GetAssignmentStatus(this.AssignID);
            }
        }

        public string StatusButtonContent
        {
            get
            {
                using (var _entities = new ReadyPortalDBEntities())
                {
                    if (CommonHelper.IsTaskReassigned(this.AssignID, this.DocumentType, true))
                    {
                        return "<div>" + Common.Global.GetEnumDescription((Common.Global.Action)this.Action) + "</div><a disabled='disabled' class='tableicon graybg cursor' data-toggle='tooltip' data-placement='top' title='This task is presently being worked on. The task cannot be re-assigned at this time'><img src='" + Global.SiteUrl + "images/reassign.png'>";
                    }
                    else if (this.Action == (int)Global.Action.Assigned)
                    {
                        var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == this.AssignID);
                        
                        if (assignment != null && assignment.LockedByUserID != null)
                        {
                            //return "Task taken by " + _entities.tblUserDepartments.FirstOrDefault(u => u.UserId == assignment.LockedByUserID).FullName;
                            return "<div>Task taken by " + _entities.tblUserDepartments.FirstOrDefault(u => u.UserId == assignment.LockedByUserID).FullName + "</div><a disabled='disabled' class='tableicon graybg cursor' data-toggle='tooltip' data-placement='top' title='This task is presently being worked on. The task cannot be re-assigned at this time'><img src='" + Global.SiteUrl + "images/reassign.png'></a> " /*+ "<a class='tableicon bluebg cursor' title = 'Cancel task' data-assign-id='" + this.AssignID.ToString() + "' data-doc-type='" + this.DocumentType + "' data-doc-file='" + this.DocumentName + "' onClick='CancelTask(this)'><i class='fa fa-close'></i></a>"*/;
                        }
                        else
                        {
                            return "<div>Not Started</div><a onClick='openPopupForReassign(this," + this.AssignID + "," + this.DocumentType + "," + this.ProjectID + "," + (this.UserID != null ? this.UserID : 0) + ",\"" + (this.dtDueDate.HasValue ? this.dtDueDate.Value.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid) : "") + "\",\"" + this.DocumentName + "\")' id='" + this.DocumentName + "' class='tableicon graybg cursor' data-toggle='tooltip' data-placement='top' title='Reassign'><img src='" + Global.SiteUrl + "images/reassign.png'></a> " + "<a class='tableicon bluebg cursor' title = 'Cancel task' data-assign-id='" + this.AssignID.ToString() + "' data-doc-type='" + this.DocumentType + "' data-doc-file='" + this.DocumentName + "' onClick='CancelTask(this)'><i class='fa fa-close'></i></a>";
                        }
                    }
                    else
                    {
                        return "<div>" + Common.Global.GetEnumDescription((Common.Global.Action)this.Action) + "</div><a onClick='openPopupForReassign(this," + this.AssignID + "," + this.DocumentType + "," + this.ProjectID + "," + (this.UserID != null ? this.UserID : 0) + ",\"" + (this.dtDueDate.HasValue ? this.dtDueDate.Value.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid) : "") + "\",\"" + this.DocumentName + "\")' id='" + this.DocumentName + "' class='tableicon graybg cursor' data-toggle='tooltip' data-placement='top' title='Reassign'><img src='" + Global.SiteUrl + "images/reassign.png'></a> "+ "<a class='tableicon bluebg cursor' title = 'Cancel task' data-assign-id='" + this.AssignID.ToString() + "' data-doc-type='" + this.DocumentType + "' data-doc-file='" + this.DocumentName + "' onClick='CancelTask(this)'><i class='fa fa-close'></i></a>";
                    }
                }
            }
        }

        public string AssignmentTeam
        {
            get
            {
                using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                {
                    var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == this.AssignID);
                    if (assignment != null)
                    {
                        //if (assignment.LockedByUserID != null)
                        //{
                        //    var fullName = _entities.tblUserDepartments.FirstOrDefault(u => u.UserId == assignment.LockedByUserID).FullName;
                        //    var assignmentMember = _entities.tblAssignmentMembers.FirstOrDefault(am => am.UserID == assignment.LockedByUserID);
                        //    return GetPermissionString(assignmentMember.CanPublish, assignmentMember.CanEdit, assignmentMember.CanApprove, fullName);
                        //}
                        //else
                        //{
                            var assignmentMembers = _entities.tblAssignmentMembers.Where(am => am.AssignmentID == this.AssignID).ToList();
                            
                            var team = String.Empty;

                            foreach (var assignmentMember in assignmentMembers)
                            {
                                var fullName = _entities.tblUserDepartments.FirstOrDefault(u => u.UserId == assignmentMember.UserID).FullName;
                                var canPublish = assignmentMember.CanPublish.HasValue ? assignmentMember.CanPublish.Value : false;
                                team += GetPermissionString(canPublish, assignmentMember.CanEdit, assignmentMember.CanApprove, fullName) + "</br>";
                            }
                            return team;
                        //}
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }
        }

        private string GetPermissionString(bool canPublish, bool canEdit, bool canApprove, string fullName)
        {
            if (canPublish)
            {
                return "<div class=\"tooltip-permission\">" + fullName + "<span class=\"tooltiptext\">Publish</span></div>";
            } else if (canEdit)
            {
                return "<div class=\"tooltip-permission\">" + fullName + "<span class=\"tooltiptext\">Edit</span></div>";
            } else if (canApprove)
            {
                return "<div class=\"tooltip-permission\">" + fullName + "<span class=\"tooltiptext\">Approve</span></div>";
            } else
            {
                return "<div class=\"tooltip-permission\">" + fullName + "<span class=\"tooltiptext\">View</span></div>";
            }
        }

        public string StatusButtonContentForMerge
        {
            get
            {
                using (var _entities = new ReadyPortalDBEntities())
                {
                    if (this.AssignID == null || this.AssignID == 0)
                    {
                        return String.Empty;
                    }
                    else
                    {
                        var assignmentMember = _entities.tblAssignmentMembers.FirstOrDefault(am => am.AssignmentID == this.AssignID && am.UserID == CurrentUserSession.UserID);
                        var currentUserIsCreator = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == this.AssignID).CreatedBy == CurrentUserSession.User.UserID;

                        //tblAssignment memberAssignment;
                        //if (assignmentMember != null) { 
                        //    memberAssignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == assignmentMember.AssignmentID);
                        //}


                        if (this.Action == (int)Common.Global.Action.Approved || this.Action == (int)Common.Global.Action.Published)
                        {
                            return "<div> Completed </div>  ";
                        }

                        else if (this.Action == (int)Common.Global.Action.Completed)
                        {
                            if (currentUserIsCreator) //Let assignment creator approve/decline/reassign as well
                            {
                                return " <a class='tableicon greenbg cursor' data-toggle='tooltip' data-placement='top' title='Approve' onClick='ApproveTask(this," + this.AssignmentID + "," + this.DocumentType + ")' id='" + ((this.DocumentType == (int)Global.DocumentType.Ppt) ? this.OriginalDocumentName : this.DocumentName) + "'><i class='fa fa-thumbs-up' aria-hidden='true'></i></a> <a onClick='openPopupForDecline(this," + this.AssignmentID + "," + this.DocumentType + ")' id='" + this.OriginalDocumentName + "' class='tableicon redbg cursor' data-toggle='tooltip' data-placement='top' title='Decline'><i class='fa fa-thumbs-down' aria-hidden='true'></i></a> <a onClick='openPopupForReassign(this," + this.AssignmentID + "," + this.DocumentType + "," + this.ProjectID + "," + this.UserID + ",\"" + (this.dtDueDate.HasValue ? this.dtDueDate.Value.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid) : "") + "\")' id='" + this.DocumentName + "' class='tableicon graybg cursor' data-toggle='tooltip' data-placement='top' title='Reassign'><img src='" + Global.SiteUrl + "images/reassign.png'>";
                            }
                            else
                            {
                                var memberAssignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == assignmentMember.AssignmentID);
                                if (memberAssignment != null && assignmentMember.CanApprove && memberAssignment.LockedByUserID != CurrentUserSession.User.UserID)
                                {
                                    return " <a class='tableicon greenbg cursor' data-toggle='tooltip' data-placement='top' title='Approve' onClick='ApproveTask(this," + this.AssignmentID + "," + this.DocumentType + ")' id='" + ((this.DocumentType == (int)Global.DocumentType.Ppt) ? this.OriginalDocumentName : this.DocumentName) + "'><i class='fa fa-thumbs-up' aria-hidden='true'></i></a> <a onClick='openPopupForDecline(this," + this.AssignmentID + "," + this.DocumentType + ")' id='" + this.OriginalDocumentName + "' class='tableicon redbg cursor' data-toggle='tooltip' data-placement='top' title='Decline'><i class='fa fa-thumbs-down' aria-hidden='true'></i></a> <a onClick='openPopupForReassign(this," + this.AssignmentID + "," + this.DocumentType + "," + this.ProjectID + "," + this.UserID + ",\"" + (this.dtDueDate.HasValue ? this.dtDueDate.Value.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid) : "") + "\")' id='" + this.DocumentName + "' class='tableicon graybg cursor' data-toggle='tooltip' data-placement='top' title='Reassign'><img src='" + Global.SiteUrl + "images/reassign.png'>";
                                }
                                else
                                {
                                    return "<span>Waiting for Approval</span>";
                                }
                            }
                        }

                        else if (this.Action == (int)Common.Global.Action.Declined)
                        {
                            if (this.LockedByUserID != CurrentUserSession.UserID)
                            {
                                if (CommonHelper.IsTaskReassigned(this.AssignID, this.DocumentType))
                                {
                                    return "<div class='cursor' title='" + this.DeclinedReason + "'> Declined</div><a disabled='disabled' class='tableicon graybg cursor' data-toggle='tooltip' data-placement='top' title='This task is presently being worked on. The task cannot be re-assigned at this time'><img src='" + Global.SiteUrl + "images/reassign.png'>";
                                }
                                else
                                {
                                    return "<div class='cursor' title='" + this.DeclinedReason + "'> Declined</div><a onClick='openPopupForReassign(this," + this.AssignmentID + "," + this.DocumentType + "," + this.ProjectID + "," + this.UserID + ",\"" + (this.dtDueDate.HasValue ? this.dtDueDate.Value.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid) : "") + "\")' id='" + this.DocumentName + "' class='tableicon graybg cursor' data-toggle='tooltip' data-placement='top' title='Reassign'><img src='" + Global.SiteUrl + "images/reassign.png'>";
                                }
                            }
                            else
                            {
                                return "<div class='cursor' title='" + this.DeclinedReason + "'> Declined</div>  ";
                            }
                        }

                        else if (this.Action == (int)Common.Global.Action.Assigned)
                        {
                            if (currentUserIsCreator && assignmentMember == null)
                            {
                                var assignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == this.AssignID);
                                if (assignment.LockedByUserID != null)
                                {
                                    return /*"<a class='tableicon bluebg cursor' title = 'Cancel task' data-assign-id='" + this.AssignID.ToString() + "' data-doc-type='" + this.DocumentType + "' data-doc-file='" + this.DocumentName + "' onClick='CancelTask(this)'><i class='fa fa-close'></i></a><br/>"+*/"In Progress";
                                }
                                else
                                {
                                    return /*"<a class='tableicon bluebg cursor' title = 'Cancel task' data-assign-id='" + this.AssignID.ToString() + "' data-doc-type='" + this.DocumentType + "' data-doc-file='" + this.DocumentName + "' onClick='CancelTask(this)'><i class='fa fa-close'></i></a><br/>"+*/"Not started";
                                }
                            }
                            else
                            {
                                var memberAssignment = _entities.tblAssignments.FirstOrDefault(a => a.AssignmentID == assignmentMember.AssignmentID);
                                if (assignmentMember.CanEdit || (assignmentMember.CanPublish.HasValue && assignmentMember.CanPublish.Value))
                                {
                                    var actionBtns = String.Empty;
                                    if (memberAssignment.LockedByUserID != null && memberAssignment.LockedByUserID == CurrentUserSession.User.UserID)
                                    {
                                        //return " <a class='tableicon bluebg cursor' title = 'Edit' data-toggle='tooltip' data-placement='top' onClick='EditContent(this," + this.DocumentType + "," + this.AssignmentID + ")' id='" + Global.UrlEncrypt(this.AssignmentID) + "'  ><i class='fa fa-pencil' aria-hidden='true'></i></a>";
                                        actionBtns = " <a class='tableicon bluebg cursor' title = 'Edit' data-toggle='tooltip' data-placement='top' onClick='EditContent(this," + this.DocumentType + "," + this.AssignmentID + ")' id='" + Global.UrlEncrypt(this.AssignmentID) + "'  ><i class='fa fa-pencil' aria-hidden='true'></i></a> <a onClick='openPopupForReassign(this," + this.AssignmentID + "," + this.DocumentType + "," + this.ProjectID + "," + this.UserID + ",\"" + (this.dtDueDate.HasValue ? this.dtDueDate.Value.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid) : "") + "\")' id='" + this.DocumentName + "' class='tableicon graybg cursor' data-toggle='tooltip' data-placement='top' title='Reassign'><img src='" + Global.SiteUrl + "images/reassign.png'></a>";
                                    }
                                    else if (memberAssignment.LockedByUserID == null)
                                    {
                                        //return " <a class='tableicon bluebg cursor' title = 'Lock Task and Edit' data-toggle='tooltip' data-placement='top' onClick='EditContent(this," + this.DocumentType + "," + this.AssignmentID + ")' id='" + Global.UrlEncrypt(this.AssignmentID) + "'  ><i class='fa fa-pencil' aria-hidden='true'></i></a>";
                                        actionBtns = " <a class='tableicon bluebg cursor' title = 'Lock Task and Edit' data-toggle='tooltip' data-placement='top' onClick='EditContent(this," + this.DocumentType + "," + this.AssignmentID + ")' id='" + Global.UrlEncrypt(this.AssignmentID) + "'  ><i class='fa fa-pencil' aria-hidden='true'></i></a> <a onClick='openPopupForReassign(this," + this.AssignmentID + "," + this.DocumentType + "," + this.ProjectID + "," + this.UserID + ",\"" + (this.dtDueDate.HasValue ? this.dtDueDate.Value.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid) : "") + "\")' id='" + this.DocumentName + "' class='tableicon graybg cursor' data-toggle='tooltip' data-placement='top' title='Reassign'><img src='" + Global.SiteUrl + "images/reassign.png'></a>";
                                        //if (currentUserIsCreator)
                                        //{
                                        //    actionBtns += " <a class='tableicon bluebg cursor' title = 'Cancel task' data-assign-id='" + this.AssignID.ToString() + "' data-doc-type='" + this.DocumentType + "' data-doc-file='" + this.DocumentName + "' onClick='CancelTask(this)'><i class='fa fa-close'></i></a>";
                                        //}
                                    }
                                    else
                                    {
                                        var lockedby = _entities.tblUserDepartments.FirstOrDefault(u => u.UserId == memberAssignment.LockedByUserID).FullName;
                                        //return "<p>Task taken by " + lockedby + "</p><a class='tableicon bluebg cursor' title = 'Reassign To Me' data-toggle='tooltip' data-placement='top' onClick='EditContent(this," + this.DocumentType + "," + this.AssignmentID + ")' id='" + Global.UrlEncrypt(this.AssignmentID) + "'  ><i class='fa fa-pencil' aria-hidden='true'></i></a>";
                                        actionBtns = "<p>Task taken by " + lockedby + "</p><a class='tableicon bluebg cursor' title = 'Reassign To Me' data-toggle='tooltip' data-placement='top' onClick='EditContent(this," + this.DocumentType + "," + this.AssignmentID + ")' id='" + Global.UrlEncrypt(this.AssignmentID) + "'  ><i class='fa fa-pencil' aria-hidden='true'></i></a> <a onClick='openPopupForReassign(this," + this.AssignmentID + "," + this.DocumentType + "," + this.ProjectID + "," + this.UserID + ",\"" + (this.dtDueDate.HasValue ? this.dtDueDate.Value.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid) : "") + "\")' id='" + this.DocumentName + "' class='tableicon graybg cursor' data-toggle='tooltip' data-placement='top' title='Reassign'><img src='" + Global.SiteUrl + "images/reassign.png'></a>";
                                    }

                                    return actionBtns;
                                }
                                else if (assignmentMember.CanApprove)
                                {
                                    if (CommonHelper.IsTaskReassigned(this.AssignID, this.DocumentType))
                                    {
                                        return " <a class='tableicon greenbg cursor' data-toggle='tooltip' data-placement='top' title='Approve' onClick='ApproveTask(this," + this.AssignmentID + "," + this.DocumentType + ")' id='" + ((this.DocumentType == (int)Global.DocumentType.Ppt) ? this.OriginalDocumentName : this.DocumentName) + "'><i class='fa fa-thumbs-up' aria-hidden='true'></i></a> <a onClick='openPopupForDecline(this," + this.AssignmentID + "," + this.DocumentType + ")' id='" + this.OriginalDocumentName + "' class='tableicon redbg cursor' data-toggle='tooltip' data-placement='top' title='Decline'><i class='fa fa-thumbs-down' aria-hidden='true'></i></a> <a disabled='disabled' class='tableicon graybg cursor' data-toggle='tooltip' data-placement='top' title='This task is presently being worked on. The task cannot be re-assigned at this time'><img src='" + Global.SiteUrl + "/images/reassign.png'>";
                                    }
                                    else if (memberAssignment.LockedByUserID == null)
                                    {
                                        return "Not Started";
                                    }
                                    else
                                    {
                                        return "In Progress";
                                    }
                                }
                                else
                                {
                                    return "In Progress";
                                }

                            }
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }


                }
            }
        }




        public int AssignID { get; set; }

        public string AssignmentID { get; set; }
        public int DocumentID { get; set; }
        [Display(Name = "Task Name")]
        public string TaskName { get; set; }
        public int? LockedByUserID { get; set; }
        public string UserName { get; set; }
        public bool IsReadOnly { get; set; }

        public string Comments { get; set; }
        public string Remarks { get; set; }
        public string DeclinedReason { get; set; }
        public string Remarks2 { get; set; }
        public string ReassignRemarks { get; set; }
        public string BulkDeclineRemarks { get; set; }

        public string DeclinePPTReason { get; set; }

        public string DeclineSheetReason { get; set; }

        public bool? IsPPTModified { get; set; }

        public bool IsExcelSheetModified { get; set; }

        public bool IsExcelSheetApproved { get; set; }

        public bool? IsPPTApproved { get; set; }

        public string Content { get; set; }

        public string ReplacementCode { get; set; }

        public DateTime? DateTime { get; set; }

        public int DocumentType { get; set; }
        public string SelectedRows { get; set; }
        public bool IsEntireDocument { get; set; }

        public int DocumentSubType { get; set; }

        public List<SelectListItem> PPTList { get; set; }
        public List<SelectListItem> WordList { get; set; }

        public string strDateTime { get { return (this.DateTime.HasValue) ? this.DateTime.Value.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid + " hh:mm tt") : "N/A"; } }

        public int? UserID { get; set; }
        public int ProjectID { get; set; }
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }
        public List<SelectListItem> myProjectList { get; set; }

        public DateTime? dtDueDate { get; set; }
        public string DueDate { get; set; }

        public DateTime? dtCompletedDate { get; set; }
        public string strCompletedDate { get { return (dtCompletedDate.HasValue) ? this.dtCompletedDate.Value.ToString(WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid + " hh:mm tt") : "N/A"; } }


        public IEnumerable<string> Department { get; set; }

        public string AssignedBy { get; set; }

        //public string UserName { get; set; }

        public IEnumerable<UserModel> UserList { get; set; }
        public IEnumerable<SelectListItem> UserListItems
        {
            get
            {
                return UserList == null ? new List<SelectListItem>() : UserList.Select(u => new SelectListItem() { Text = u.FullName, Value = Convert.ToString(u.UserId) });
            }
        }

        public List<int> MemberIDList { get; set; }

        public List<int> DocumentIDList { get; set; }

        public List<SelectListItem> MemberList { get; set; }

        public string AssignmentMemberList { get; set; }

        public List<string> AssignedMembers { get; set; }

        public List<AssignmentMemberModel> AssignmentMembers { get; set; }

        public int Action { get; set; }

        public string DocumentName { get; set; }

        public CommonMessagesModel ModuleName { get; set; }

        public string ViewIcon
        {
            get
            {
                if (this.DocumentType == (int)Global.DocumentType.Word)
                {
                    return "<img  src=" + Global.SiteUrl + "Css/Images/word.png  style='cursor:pointer' /> ";
                }
                else if (this.DocumentType == (int)Global.DocumentType.Ppt)
                {
                    return "<img src=" + Global.SiteUrl + "Css/Images/ppt.png style='cursor:pointer' />";
                }
                else if (this.DocumentType == (int)Global.DocumentType.Xls)
                {
                    return "<img src=" + Global.SiteUrl + "Css/Images/xls.png style='cursor:pointer' />";
                }
                else
                    return "<img src=" + Global.SiteUrl + "Css/Images/view-icon.png style='cursor:pointer' />";

            }
        }

        public string OriginalDocumentName { get; set; }

        public string SourceFileCloneFile { get; set; }
        public IEnumerable<AssignmentModel> SnippetList { get; set; }



        public int start { get; set; }
        public int length { get; set; }
        public string selectedText { get; set; }
        public string selectedPages { get; set; }
        public string selectedRanges { get; set; }
        public List<WordPageModel> AssignedPages { get; set; }
        public List<string> SelectedSlides { get; set; }

        public List<string> SelectedSheets { get; set; }

        public int CurrentUserID { get { return CurrentUserSession.UserID; } }

        public IEnumerable<string> AssignedTo { get; set; }

        public int? CreatedBy { get; set; }

        public string CheckBoxApproveDecline
        {
            get
            {
                if (this.Action == (int)Common.Global.Action.Completed)
                {
                    if (this.UserID != CurrentUserSession.UserID)
                        //return "<button class='btn btn-primary ApproveButton' title = 'Approve'  style='cursor:pointer;padding: 3px 12px;margin-bottom: 3px;'  onClick='ApproveTask(this," + this.AssignmentID + "," + this.DocumentType + ")' id='" + ((this.DocumentType == (int)Global.DocumentType.Ppt) ? this.OriginalDocumentName : this.DocumentName) + "' >Approve</button> <button class='btn btn-primary danger' title = 'Decline' style='cursor:pointer;padding: 3px 12px;margin-bottom: 3px;background-color: indianred;cursor:pointer;'   onClick='openPopupForDecline(this," + this.AssignmentID + "," + this.DocumentType + ")' id='" + this.DocumentName + "'  >Decline</button>";
                        return "<input type='checkbox' id='chkDelete' name='chkDelete' class='singleCheckBox' value='" + this.AssignmentID + "' onclick='return initializeSingleCheckBox(this);' />";
                    else
                        return string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public DateTime? ProjectDateTime { get; set; }

        [Display(Name = "Browse File")]
        public HttpPostedFileBase[] files { get; set; }

        public List<UploadedDocModel> lstUploadedDocModel { get; set; }

        public string ProjectTaskID { get; set; }
        public List<SelectListItem> ProjectDocumentList { get; set; }

        public int DateFormatID { get; set; }

        public string DateFormat { get; set; }

        public SelectList SelectDateFormat { get; set; }
        public string Path { get; internal set; }
        public int Status { get; set; }
        public bool IsFullscreen { get; set; }
    }
}
