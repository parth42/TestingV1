using Kendo.Mvc.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;
using WordAutomationDemo.CustomGridBinding;
using WordAutomationDemo.Repository.Interfaces;
using System.Web.Helpers;
using System.Drawing;
using System.IO;

namespace WordAutomationDemo.Controllers
{
    [ValidateLogin]
    public class UserController : BaseController
    {
        #region Members
        private static int Count = 0;
        IUser iUser;
        ICompanyHelper iCompanyHelper;
        IRole iRole;
        ILogActivity iLogActivity;
        ICompany iCompany;
        public UserController(IUser IUser, ICompanyHelper ICompanyHelper, IRole IRole, ILogActivity ILogActivity, ICompany ICompany)
        {
            iUser = IUser;
            iCompanyHelper = ICompanyHelper;
            iRole = IRole;
            iLogActivity = ILogActivity;
            iCompany = ICompany;
        }

        #endregion

        #region Index

        /// <summary>
        /// User Listings – This method is used to user listing.
        /// Created By: Dipak B. Kansara
        /// Created On: 08/02/2017
        /// </summary>
        /// <param name="id">ID of user</param>
        /// <returns>View</returns>
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.User)]
        public ActionResult Index()
        {
            var ViewModel = new UserModel
            {
                ModuleName = new CommonMessagesModel { ModuleName = "User" },
            };
            return View(ViewModel);
        }

        #endregion

        #region List

        #region Grid Event
        /// <summary>
        /// User Read – This method is bind user in list page.
        /// Created By: Dipak B. Kansara
        /// Created On: 08/02/2017
        /// </summary>
        /// <param name="request">Kendo grid data source request object</param>
        /// <returns>json data of user</returns>
        public ActionResult User_Read([DataSourceRequest]DataSourceRequest request)
        {
            var result = new DataSourceResult()
            {
                Data = GetUserGridData(request), // Process data
                Total = Count // Total number of records
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get User Grid Data
        /// <summary>
        /// Get User GridData - This method is used to get user data.
        /// Created By: Dipak B. Kansara
        /// Created On: 08/02/2017
        /// </summary>
        /// <param name="command">Kendo grid data source request object</param>
        /// <returns>User data</returns>
        public IEnumerable GetUserGridData([DataSourceRequest]DataSourceRequest command)
        {
            var result = iUser.Userlist();

            //Apply filtering
            result = result.ApplyFiltering(command.Filters);

            //Get count of total records
            Count = result.Count();

            //Apply sorting
            result = result.ApplySorting(command.Groups, command.Sorts);

            //Apply paging
            result = result.ApplyPaging(command.Page, command.PageSize);

            //Apply grouping
            if (command.Groups.Any())
            {
                return result.ApplyGrouping(command.Groups);
            }
            var lstUser = result.ToList();
            foreach (var user in lstUser)
            {
                user.EncryptedUserId = Global.UrlEncrypt(user.UserId.ToString());
            }
            return lstUser;
        }
        #endregion

        #endregion

        #region Create

        /// <summary>
        /// Create – This method is used to create user.
        /// Create By : Dipak Kansara
        /// Created Date : 08/02/2017
        /// </summary>
        /// <returns></returns>
        [HandleError]
        [HttpGet]
        [ValidateUserPermission(Global.Actions.Create, Global.Controlers.User)]
        public ActionResult Create()
        {
            UserModel objUserModel = new UserModel();
            objUserModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
            objUserModel.IsActive = true;
            return View(objUserModel);
        }

        [HandleError]
        [HttpPost]
        //[LogUserActivity(Global.LogActivityTypes.UserAdded, "New User Added")]
        public ActionResult Create(UserModel objUserModel, HttpPostedFileBase imgUserProfile)
        {
            if (ModelState.IsValid)
            {
                string strUserProfile = "";
                tblUserDepartment tblUserDepartmentModel = null;
                if (Request.Form["hdnImage"] != null)
                {
                    strUserProfile = Request.Form["hdnImage"];
                }

                if (iUser.IsUserExistsByEmail("UserName", objUserModel.UserName.Trim(), -1, 0))
                {
                    iLogActivity.AddError(string.Format(Messages.AlreadyExists, "UserName "), new int[] { objUserModel.UserId });

                    ModelState.AddModelError("UserName", string.Format(Messages.AlreadyExists, "UserName "));
                    objUserModel.SelectRole = new SelectList(iRole.GetRolesByCompanyID(objUserModel.Company_ID), "Value", "Text");
                    objUserModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                    return View(objUserModel);
                }
                else if (iUser.IsUserExistsByEmail("Email", objUserModel.EmailID.Trim(), -1, objUserModel.Company_ID))
                {
                    iLogActivity.AddError(string.Format(Messages.AlreadyExists, "Email "), new int[] { objUserModel.UserId });

                    ModelState.AddModelError("EmailID", string.Format(Messages.AlreadyExists, "Email "));
                    objUserModel.SelectRole = new SelectList(iRole.GetRolesByCompanyID(objUserModel.Company_ID), "Value", "Text");
                    objUserModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                    return View(objUserModel);
                }
                if (iUser.InsertUser(objUserModel, strUserProfile,out tblUserDepartmentModel))
                {
                    iLogActivity.AddLogActivity((byte)Global.LogActivityTypes.UserAdded,String.Format("User <{0}> added Successfully", tblUserDepartmentModel.UserName), new int[] { tblUserDepartmentModel.UserId });

                    var templates = CommonHelper.GetEmailTemplateContent("Welcome Page");

                    string EmailBody = templates.TemplateContentForEmail;

                    // Remove password from email if SSO is enabled
                    if (Global.IsADEnabled)
                        EmailBody = string.Join(Environment.NewLine, EmailBody.Replace("\r", "").Split('\n').Where(line => !line.ToLower().Contains("#password#")));

                    EmailBody = PopulateEmailBody(objUserModel, EmailBody);

                    Global.SendEmail(objUserModel.EmailID, templates.Subject, EmailBody, null, CurrentUserSession.User.EmailID);

                    return RedirectToAction("Index", "User", new { Msg = "added" });
                }
                else
                {
                    iLogActivity.AddError(Messages.ProvideAllFields.ToString(), new int[] { objUserModel.UserId });

                    ModelState.AddModelError("", Messages.ProvideAllFields.ToString());
                    objUserModel.SelectRole = new SelectList(iRole.GetRolesByCompanyID(objUserModel.Company_ID), "Value", "Text");
                    objUserModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                    return View(objUserModel);
                }
            }
            else
            {
                iLogActivity.AddError("Model State is invalid", new int[] { objUserModel.UserId });

                objUserModel.SelectRole = new SelectList(iRole.GetRolesByCompanyID(objUserModel.Company_ID), "Value", "Text");
                objUserModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                return View(objUserModel);
            }
        }

        #endregion

        #region Edit
        /// <summary>
        /// Edit – This method is used to edit user.
        /// Create By : Dipak Kansara
        /// Created Date : 08/02/2017
        /// </summary>
        /// <param name="id">ID of user</param>
        /// <returns></returns>
        [HandleError]
        [ValidateUserPermission(Global.Actions.Edit, Global.Controlers.User)]
        [HttpGet]
        public ActionResult Edit()
        {
            if (Request.QueryString["UID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                string UID = Global.UrlDecrypt(Request.QueryString["UID"]);
                int userID;
                if (int.TryParse(UID, out userID))
                {
                    UserModel objUserModel = iUser.GetUserByID(userID);
                    if (objUserModel == null)
                    {
                        return RedirectToAction("Index", "User", new { Msg = "drop" });
                    }
                    string strImagePath = System.Web.HttpContext.Current.Server.MapPath("~/ApplicationDocuments/ProfileImages/") + objUserModel.ProfileImage;

                    if (System.IO.File.Exists(strImagePath))
                    {
                        objUserModel.ProfileImage = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/ProfileImages/" + objUserModel.ProfileImage;
                    }
                    else
                    {
                        objUserModel.ProfileImage = string.Empty;
                    }

                    objUserModel.SelectRole = new SelectList(iRole.GetRolesByCompanyID(objUserModel.Company_ID), "Value", "Text", objUserModel.Role_ID);
                    objUserModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name", objUserModel.Company_ID);
                    if (!Global.IsADEnabled)
                    {
                        objUserModel.Password = Global.Encryption.Decrypt(objUserModel.Password);
                        objUserModel.ConfirmPassword = Global.Encryption.Decrypt(objUserModel.Password);
                    }

                    return View(objUserModel);
                }
                else
                {
                    return RedirectToAction("Index", "User");
                }
            }
        }

        [HandleError]
        [HttpPost]
        public ActionResult Edit(UserModel objUserModel, HttpPostedFileBase imgUserProfile)
        {
                if (ModelState.IsValid)
                {
                    string strUserProfile = "";
                    if (Request.Form["hdnImage"] != null)
                    {
                        strUserProfile = Request.Form["hdnImage"];
                    }
                    var query = iUser.GetUserByID(objUserModel.UserId);
                    if (query == null)
                    {
                        iLogActivity.AddError("drop", new int[] { objUserModel.UserId });

                        return RedirectToAction("Index", "User", new { Msg = "drop" });
                    }

                    if (iUser.IsUserExistsByEmail("UserName", objUserModel.UserName.Trim(), objUserModel.UserId, 0))
                    {
                        iLogActivity.AddError(string.Format(Messages.AlreadyExists, "UserName "), new int[] { objUserModel.UserId });

                        ModelState.AddModelError("UserName", string.Format(Messages.AlreadyExists, "UserName "));
                        objUserModel.SelectRole = new SelectList(iRole.GetRolesByCompanyID(objUserModel.Company_ID), "Value", "Text");
                        objUserModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                        return View(objUserModel);
                    }
                    else if (iUser.IsUserExistsByEmail("Email", objUserModel.EmailID.Trim(), objUserModel.UserId, objUserModel.Company_ID))
                    {
                        iLogActivity.AddError(string.Format(Messages.AlreadyExists, "Email "), new int[] { objUserModel.UserId });

                        ModelState.AddModelError("EmailID", string.Format(Messages.AlreadyExists, "Email "));
                        objUserModel.SelectRole = new SelectList(iRole.GetRolesByCompanyID(objUserModel.Company_ID), "Value", "Text");
                        objUserModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                        return View(objUserModel);
                    }
                    if (iUser.UpdateUser(objUserModel, strUserProfile, imgUserProfile))
                    {
                        //if (CurrentUserSession.UserID == objUserModel.UserId)
                        //{
                        //    CurrentUser currentUser = CurrentUserSession.User;
                        //    currentUser.FirstName = objUserModel.FullName.Trim();
                        //    currentUser.LastName = objUserModel.EmailID.Trim();
                        //    currentUser.RoleID = objUserModel.Role_ID;
                        //    currentUser.UserName = objUserModel.UserName.Trim();
                        //    CurrentUserSession.User = currentUser;
                        //}
                        iLogActivity.AddLogActivity((byte)Global.LogActivityTypes.UserUpdated, String.Format("User <{0}> Updated Successfully", objUserModel.UserName), new int[] { objUserModel.UserId });

                        return RedirectToAction("Index", "User", new { Msg = "updated" });
                    }
                    else
                    {
                        iLogActivity.AddError(Messages.ProvideAllFields.ToString(), new int[] { objUserModel.UserId });

                        ModelState.AddModelError("", Messages.ProvideAllFields.ToString());
                        objUserModel.SelectRole = new SelectList(iRole.GetRolesByCompanyID(objUserModel.Company_ID), "Value", "Text");
                        objUserModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                        return View(objUserModel);
                    }
                }
                else
                {
                    iLogActivity.AddError("Model State is invalid", new int[] { objUserModel.UserId });

                    objUserModel.SelectRole = new SelectList(iRole.GetRolesByCompanyID(objUserModel.Company_ID), "Value", "Text", objUserModel.Role_ID);
                    objUserModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name", objUserModel.Company_ID);
                    return View(objUserModel);
                }
        }

        #endregion

        #region Profile
        /// <summary>
        /// Edit – This method is used to edit user.
        /// Create By : Dipak Kansara
        /// Created Date : 08/02/2017
        /// </summary>
        /// <param name="id">ID of user</param>
        /// <returns></returns>
        [HandleError]
        [HttpGet]
        public ActionResult Profile()
        {
            if (Request.QueryString["UID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                string UID = Global.UrlDecrypt(Request.QueryString["UID"]);
                int userID;
                if (int.TryParse(UID, out userID))
                {
                    UserModel objUserModel = iUser.GetUserByID(userID);
                    if (objUserModel == null)
                    {
                        return RedirectToAction("Index", "User", new { Msg = "drop" });
                    }
                    string strImagePath = System.Web.HttpContext.Current.Server.MapPath("~/ApplicationDocuments/ProfileImages/") + objUserModel.ProfileImage;

                    if (System.IO.File.Exists(strImagePath))
                    {
                        objUserModel.ProfileImage = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/ProfileImages/" + objUserModel.ProfileImage;
                    }
                    else
                    {
                        objUserModel.ProfileImage = string.Empty;
                    }
                    objUserModel.SelectRole = new SelectList(iRole.GetRolesByCompanyID(objUserModel.Company_ID), "Value", "Text", objUserModel.Role_ID);
                    objUserModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name", objUserModel.Company_ID);
                    objUserModel.Password = Global.Encryption.Decrypt(objUserModel.Password);
                    objUserModel.ConfirmPassword = Global.Encryption.Decrypt(objUserModel.Password);
                    objUserModel.IsAppointmentEnable = objUserModel.IsAppointmentEnable;
                    return View(objUserModel);
                }
                else
                {
                    return RedirectToAction("Index", "User");
                }
            }
        }

        [HandleError]
        [HttpPost]
        public ActionResult Profile(UserModel objUserModel, HttpPostedFileBase imgUserProfile)
        {
            string strUserProfile = "";
            string UID = Global.UrlEncrypt(objUserModel.UserId.ToString());
                if (Request.Form["hdnImage"] != null)
                {
                    strUserProfile = Request.Form["hdnImage"];
                }
                var query = iUser.GetUserByID(objUserModel.UserId);
                if (query == null)
                {
                    return RedirectToAction("Index", "User", new { Msg = "drop" });
                }

                else if (iUser.IsUserExistsByEmail("Email", objUserModel.EmailID.Trim(), objUserModel.UserId, CurrentUserSession.User.CompanyID))
                {
                    ModelState.AddModelError("EmailID", string.Format(Messages.AlreadyExists, "Email "));
                    objUserModel.SelectRole = new SelectList(iRole.GetRolesByCompanyID(objUserModel.Company_ID), "Value", "Text");
                    objUserModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                    return View(objUserModel);
                }
                if (iUser.ChangeProfile(objUserModel, strUserProfile, imgUserProfile))
                {
                    //if (CurrentUserSession.UserID == objUserModel.UserId)
                    //{
                    //    CurrentUser currentUser = CurrentUserSession.User;
                    //    currentUser.FirstName = objUserModel.FullName.Trim();
                    //    currentUser.LastName = objUserModel.EmailID.Trim();
                    //    currentUser.RoleID = objUserModel.Role_ID;
                    //    currentUser.UserName = objUserModel.UserName.Trim();
                    //    CurrentUserSession.User = currentUser;
                    //}
                    return RedirectToAction("Profile", "User", new { UID = UID, Msg = "changed" });
                }
                else
                {
                    return RedirectToAction("Profile", "User", new { UID = UID, Msg = "error" });
                }
        }

        #endregion

        #region Detail
        /// <summary>
        /// Detail – This method is used to show user.
        /// Create By : Dipak Kansara
        /// Created Date :08/02/2017
        /// </summary>
        /// <param name="id">ID of user</param>
        /// <returns></returns>
        [HandleError]
        [HttpGet]
        [ValidateUserPermission(Global.Actions.Details, Global.Controlers.User)]
        public ActionResult Detail()
        {
                if (Request.QueryString["UID"] == null)
                {
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    string UID = Global.UrlDecrypt(Request.QueryString["UID"]);
                    int userID;
                    if (int.TryParse(UID, out userID))
                    {
                        UserModel objUserModel = iUser.GetUserByID(userID);
                        if (objUserModel == null)
                        {
                            return RedirectToAction("Index", "User", new { Msg = "drop" });
                        }
                        else
                        {
                            string strImagePath = System.Web.HttpContext.Current.Server.MapPath("~/ApplicationDocuments/ProfileImages/") + objUserModel.ProfileImage;

                            if (System.IO.File.Exists(strImagePath))
                            {
                                objUserModel.ProfileImage = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/ProfileImages/" + objUserModel.ProfileImage;
                            }
                            else
                            {
                                objUserModel.ProfileImage = string.Empty;
                            }
                            return View(objUserModel);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "User");
                    }
                }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Delete – This method is used to delete user.
        /// Create By : Dipak Kansara
        /// Created Date : 08/02/2017
        /// </summary>
        /// <param name="chkDelete">IDs of user</param>
        /// <returns></returns>
        [HandleError]
        [HttpPost]
        [ValidateUserPermission(Global.Actions.Delete, Global.Controlers.User)]
        public ActionResult Delete(int[] chkDelete)
        {
                if (chkDelete.Length > 0)
                {
                    //User delete log is added in the user repository delete method.
                    iUser.DeleteUsers(chkDelete);
                    
                    return RedirectToAction("Index", "User", new { Msg = "deleted" });
                }
                else
                {
                    iLogActivity.AddError("No selected", chkDelete);

                    return RedirectToAction("Index", "User", new { Msg = "NoSelect" });
                }
        }
        #endregion

        #region Validate User

        /// <summary>
        /// Validate Duplicate User – This method is used to validate duplicate user.
        /// Create By : Dipak Kansara
        /// Created Date : 08/02/2017
        /// </summary>
        /// <param name="FieldList">Field List</param>
        /// <param name="ValueList">Value List</param>
        /// <param name="strAddOrEditID">Add Or Edit ID</param>
        /// <returns>True/False</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult ValidateDuplicateUser(string FieldList, string ValueList, int strAddOrEditID)
        {
            bool IsUserExists = iUser.IsUserExistsByEmail(FieldList, ValueList, strAddOrEditID, 0);
            if (IsUserExists)
            {
                return Json("0");
            }
            else
            {
                return Json(new { @status = "1" });
            }
        }
        #endregion

        #region Validate User
        /// <summary>
        /// Get Roles By CompanyID – This method is used to get roles by companyID.
        /// Create By : Dipak Kansara
        /// Created Date : 08/08/2017
        /// </summary>
        /// <param name="companyID">companyID</param>
        /// <returns>True/False</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetRolesByCompanyID(int companyID)
        {
            var Roles = iRole.GetRolesByCompanyID(companyID);
            return Json(Roles, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Populate Email Body
        /// <summary>
        /// Populate Email Body – This method is used to populate email body.
        /// Created By: 
        /// Created On: 
        /// </summary>
        /// <param name="result">result</param>
        /// <param name="EmailBody">Email Body</param>
        /// <returns>string</returns>
        #region Email body Html
        private string PopulateEmailBody(UserModel result, string EmailBody)
        {
            var SiteUrl = Global.SiteUrl;

            EmailBody = EmailBody.Replace("#FullName#", Convert.ToString(result.UserName));
            EmailBody = EmailBody.Replace("#Login#", SiteUrl);
            EmailBody = EmailBody.Replace("#UserName#", result.UserName);
            EmailBody = EmailBody.Replace("#Password#", result.Password);
            return EmailBody;
        }
        #endregion
        #endregion
    }
}