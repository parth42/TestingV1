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

namespace WordAutomationDemo.Controllers
{
    [ValidateLogin]
    public class RoleController : BaseController
    {
        #region Members
        private static int Count = 0;
        IRole iRole;
        ICompanyHelper iCompanyHelper;
        IRolePrivileges iRolePrivileges;
        ILogActivity iLogActivity;
        public RoleController(IRole IRole, ICompanyHelper ICompanyHelper, ILogActivity ILogActivity, IRolePrivileges IRolePrivileges)
        {
            iRole = IRole;
            iCompanyHelper = ICompanyHelper;
            iLogActivity = ILogActivity;
            iRolePrivileges = IRolePrivileges;
        }
        #endregion

        #region Index
        /// <summary>
        /// Role Listings – This method is used to role listing.
        /// Created By: Dipak B. Kansara
        /// Created On: 07/31/2017
        /// </summary>
        /// <param name="id">ID of role</param>
        /// <returns>View</returns>
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Role)]
        public ActionResult Index(int? id)
        {
            var ViewModel = new RoleModel
            {
                ModuleName = new CommonMessagesModel { ModuleName = "Role" }
            };
            return View(ViewModel);
        }
        #endregion

        #region List

        #region Ajax Binding
        /// <summary>
        /// List ajax binding – This method is bind role in list page.
        /// Created By: Dipak B. Kansara
        /// Created On: 07/31/2017
        /// </summary>
        /// <param name="request">Kendo grid data source request object</param>
        /// <returns>json data of role</returns>
        public ActionResult Role_Read([DataSourceRequest]DataSourceRequest request)
        {
            var result = new DataSourceResult()
            {
                Data = GetRoleGridData(request), // Process data
                Total = Count // Total number of records
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Role Grid Data
        /// <summary>
        /// Get Role Grid Data - This method is used to get role data.
        /// Created By: Dipak B. Kansara
        /// Created On: 31/07/2017
        /// </summary>
        /// <param name="command">Kendo grid data source request object</param>
        /// <returns>Role data</returns>
        public IEnumerable GetRoleGridData([DataSourceRequest]DataSourceRequest command)
        {
            var result = iRole.GetRoleList();

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
            var resultEncrypted = result.ToList();
            foreach (var role in resultEncrypted)
            {
                role.EncryptedRoleID = Global.UrlEncrypt(role.RoleID.ToString());
            }
            return resultEncrypted;
        }
        #endregion

        #endregion

        #region Create
        /// <summary>
        /// Create – This method is used to create role.
        /// Create By : Dipak Kansara
        /// Created Date : 07/31/2017
        /// </summary>
        /// <returns></returns>
        [HandleError]
        [ValidateUserPermission(Global.Actions.Create, Global.Controlers.Role)]
        [HttpGet]
        public ActionResult Create()
        {
            RoleModel objRoleModel = new RoleModel();
            objRoleModel.IsActive = true;
            objRoleModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
            return View(objRoleModel);
        }

        [HandleError]
        [HttpPost]
        //[LogUserActivity(Global.LogActivityTypes.UserRoleCreated,"User Role Created")]
        public ActionResult Create(RoleModel objRoleModel)
        {
                if (ModelState.IsValid)
                {
                    tblRole tblRoleModel = null;
                    //Validate Role exists or not
                    if (iRole.IsRoleExists(0, objRoleModel.Role.Trim(), objRoleModel.Company_ID))
                    {
                        iLogActivity.AddError(string.Format(Messages.AlreadyExists, "Role "), new int[] { objRoleModel.RoleID });
                        ModelState.AddModelError(string.Empty, string.Format(Messages.AlreadyExists, "Role "));
                        objRoleModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                        return View(objRoleModel);
                    }
                    if (iRole.InsertRole(objRoleModel, out tblRoleModel))
                    {
                        iLogActivity.AddLogActivity((byte)Global.LogActivityTypes.UserRoleCreated,String.Format("Role <{0}> created Successfully",tblRoleModel.Role), new int[] { tblRoleModel.RoleID });
                        if (objRoleModel.IsAdminRole)
                        {
                            iRolePrivileges.CreateRolePrivilegesForAdmin(tblRoleModel.RoleID);
                        }
                        return RedirectToAction("Index", "Role", new { Msg = "added" });
                    }
                    else
                    {
                        iLogActivity.AddError(Messages.ProvideAllFields.ToString(), new int[] { objRoleModel.RoleID });
                        ModelState.AddModelError("", Messages.ProvideAllFields.ToString());
                        objRoleModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                        return View(objRoleModel);
                    }
                }
                else
                {
                    iLogActivity.AddError("Model State is invalid", new int[] { objRoleModel.RoleID });
                    objRoleModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                    return View(objRoleModel);
                }
        }
        #endregion

        #region Edit Role

        /// <summary>
        /// Edit – This method is used to edit role.
        /// Create By : Dipak Kansara
        /// Created Date : 07/31/2017
        /// </summary>
        /// <param name="roleID">ID of role</param>
        /// <returns></returns>
        [HandleError]
        [ValidateUserPermission(Global.Actions.Edit, Global.Controlers.Role)]
        public ActionResult Edit()
        {
            if (Request.QueryString["UID"] == null)
            {
                return RedirectToAction("Index", "Role");
            }
            else
            {
                string UID = Global.UrlDecrypt(Request.QueryString["UID"]);
                int roleID;
                if (int.TryParse(UID, out roleID))
                {
                    if (roleID == 1)
                    {
                        return RedirectToAction("Index", "Role", new { Msg = "drop" });
                    }
                    else
                    {
                        RoleModel objRoleModel = iRole.GetRoleByID(roleID);
                        if (objRoleModel == null)
                        {
                            return RedirectToAction("Index", "Role", new { Msg = "drop" });
                        }
                        objRoleModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name", objRoleModel.Company_ID);
                        return View(objRoleModel);
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Role");
                }
            }
        }
        [HttpPost]
        //[LogUserActivity(Global.LogActivityTypes.UserRoleUpdated, "User Role Updated")]
        public ActionResult Edit(RoleModel objRoleModel)
        {
                if (objRoleModel.RoleID == 1)
                {
                    iLogActivity.AddError("drop", new int[] { objRoleModel.RoleID });
                    return RedirectToAction("Index", "Role", new { Msg = "drop" });
                }
                if (ModelState.IsValid)
                {
                    //Validate Role exists or not
                    if (iRole.IsRoleExists(objRoleModel.RoleID, objRoleModel.Role.Trim(), objRoleModel.Company_ID))
                    {
                        iLogActivity.AddError(string.Format(Messages.AlreadyExists, "Role "), new int[] { objRoleModel.RoleID });
                        ModelState.AddModelError(string.Empty, string.Format(Messages.AlreadyExists, "Role "));
                        objRoleModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                        return View(objRoleModel);
                    }
                    else
                    {
                        if (iRole.UpdateRole(objRoleModel))
                        {
                            iLogActivity.AddLogActivity((byte)Global.LogActivityTypes.UserRoleUpdated, string.Format("Role <{0}> Updated Successfully", objRoleModel.Role), new int[] { objRoleModel.RoleID });
                            return RedirectToAction("Index", "Role", new { Msg = "updated" });
                        }
                        else
                        {
                            iLogActivity.AddError(Messages.ProvideAllFields.ToString(), new int[] { objRoleModel.RoleID });
                            ModelState.AddModelError("", Messages.ProvideAllFields.ToString());
                            objRoleModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                            return View(objRoleModel);
                        }
                    }
                }
                else
                {
                    iLogActivity.AddError("Model State is invalid", new int[] { objRoleModel.RoleID });
                    objRoleModel.SelectCompany = new SelectList(iCompanyHelper.GetCompanyList(), "CompanyID", "Name");
                    return View(objRoleModel);
                }
        }
        #endregion

        #region Delete

        /// <summary>
        /// Delete – This method is used to delete role.
        /// Create By : Dipak Kansara
        /// Created Date : 07/31/2017
        /// </summary>
        /// <param name="chkDelete">IDs of role</param>
        /// <returns></returns>
        [HandleError]
        [ValidateUserPermission(Global.Actions.Delete, Global.Controlers.Role)]
        [HttpPost]
        public ActionResult Delete(int[] chkDelete)
        {
                if (chkDelete.Length > 0)
                {
                    //Role logging is added in the role repository method.
                    iRole.DeleteRoles(chkDelete);
                    //iLogActivity.AddLogActivity((byte)Global.LogActivityTypes.UserRoleDeleted, "Role deleted successfully", chkDelete);
                    return RedirectToAction("Index", "Role", new { Msg = "deleted" });
                }
                else
                {
                    iLogActivity.AddError("No selected", chkDelete);
                    return RedirectToAction("Index", "Role", new { Msg = "NoSelect" });
                }
        }
        #endregion

        #region Detail
        /// <summary>
        /// Detail – This method is used to show role.
        /// Create By : Dipak Kansara
        /// Created Date : 07/31/2017
        /// </summary>
        /// <param name="roleID">ID of role</param>
        /// <returns></returns>
        [HandleError]
        [ValidateUserPermission(Global.Actions.Details, Global.Controlers.Role)]
        [HttpGet]
        public ActionResult Detail()
        {
            if (Request.QueryString["UID"] == null)
            {
                return RedirectToAction("Index", "Role");
            }
            else
            {
                string UID = Global.UrlDecrypt(Request.QueryString["UID"]);
                int roleID;
                if (int.TryParse(UID, out roleID))
                {
                    if (roleID > 0)
                    {
                        RoleModel objRoleModel = iRole.GetRoleByID(roleID);
                        if (objRoleModel == null)
                        {
                            return RedirectToAction("Index", "Role", new { Msg = "drop" });
                        }
                        else
                        {
                            return View(objRoleModel);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Role", new { Msg = "drop" });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Role", new { Msg = "drop" });
                }
            }
        }

        #endregion

        #region Validate Duplicate Role
        /// <summary>
        /// Validate Duplicate Role – This method is used to validate duplicate role.
        /// Create By : Dipak Kansara
        /// Created Date : 07/31/2017
        /// </summary>
        /// <param name="RoleID">ID of role</param>
        /// /// <param name="Role">Name of role</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public JsonResult ValidateDuplicateRole(int? RoleID, string Role, int? companyID)
        {
            if (iRole.IsRoleExists(Convert.ToInt32(RoleID), Role.Trim(), Convert.ToInt32(companyID)))
            {
                return Json(new { @status = "0" });
            }
            else
            {
                return Json(new { @status = "1" });
            }
        }
        #endregion
    }
}