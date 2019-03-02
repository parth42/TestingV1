using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Interfaces;

namespace WordAutomationDemo.Controllers
{
    [ValidateLogin]
    public class RolePrivilegesController : BaseController
    {
        #region Members
        IRolePrivileges iRolePrivileges;
        ICompany iCompany;
        ILogActivity iLogActivity;
        public RolePrivilegesController(IRolePrivileges IRolePrivileges, ICompany ICompany, ILogActivity ILogActivity)
        {
            iRolePrivileges = IRolePrivileges;
            iCompany = ICompany;
            iLogActivity = ILogActivity;
        }
        #endregion

        #region Index
        [HandleError]
        /// <summary>
        /// Role Privileges – This method is used show role privileges.
        /// Created By: Dipak B. Kansara
        /// Created On: 08/03/2017
        /// </summary>
        /// <returns>View</returns>
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.RolePrivileges)]
        public ActionResult Index()
        {
            RolePrivilegesModel objRolePrivilegesModel = new RolePrivilegesModel();
            objRolePrivilegesModel.Company = new SelectList(iCompany.GetCompanyList(), "CompanyID", "Name");
            if (CurrentUserSession.User.IsSuperAdmin)
            {
                objRolePrivilegesModel.Roles = iRolePrivileges.GetRolesByCompanyID(0);
                //iLogActivity.AddInformation("User is super admin");
            }
            else
            {
                objRolePrivilegesModel.Roles = iRolePrivileges.GetRolesByCompanyID(CurrentUserSession.User.CompanyID);
                //iLogActivity.AddInformation("User is not super admin");
            }
            objRolePrivilegesModel.ModuleName = new CommonMessagesModel { ModuleName = "Role Privileges" };
            return View(objRolePrivilegesModel);
        }

        [HandleError]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(int? Role_ID, int? Company_ID, string btnSubmit, RolePrivilegesModel objRolePrivilegesModel)
        {
            objRolePrivilegesModel.Company = new SelectList(iCompany.GetCompanyList(), "CompanyID", "Name");
            if (Company_ID.HasValue)
            {
                objRolePrivilegesModel.Roles = iRolePrivileges.GetRolesByCompanyID(Company_ID.Value);
               // iLogActivity.AddInformation("Company_ID has value", new int[] { Company_ID.Value });
            }
            else
            {
                objRolePrivilegesModel.Roles = iRolePrivileges.GetRolesByCompanyID(CurrentUserSession.User.CompanyID);
                //iLogActivity.AddInformation("Company_ID accessed from session", new int[] { CurrentUserSession.User.CompanyID });
            }
            objRolePrivilegesModel.ModuleName = new CommonMessagesModel { ModuleName = "Role Privileges" };

            if (Convert.ToInt32(Role_ID) > 0 && btnSubmit == null)
            {
                ViewData["RolePrivileges"] = iRolePrivileges.ExecuteStoredProcedure("RolePrev", Convert.ToInt32(Role_ID));
                //iLogActivity.AddInformation("Stored procedure\"RolePrev\" executed successfully", new int[] { Convert.ToInt32(Role_ID) });
                return View(objRolePrivilegesModel);
            }
            else if (Role_ID > 0)
            {
                string strHdMenuItemID = Request.Form["hdMenuItemID"];
                string[] strHdMenuItemIDArray = strHdMenuItemID.Split(',');

                string strChkView = Request.Form["chkView"];
                string[] strChkViewArray = null;
                if (strChkView != null)
                    strChkViewArray = strChkView.Split(',');

                string strChkAdd = Request.Form["chkAdd"];
                string[] strChkAddArray = null;
                if (strChkAdd != null)
                    strChkAddArray = strChkAdd.Split(',');

                string strChkEdit = Request.Form["chkEdit"];
                string[] strChkEditArray = null;
                if (strChkEdit != null)
                    strChkEditArray = strChkEdit.Split(',');

                string strChkDelete = Request.Form["chkDelete"];
                string[] strChkDeleteArray = null;
                if (strChkDelete != null)
                    strChkDeleteArray = strChkDelete.Split(',');

                string strChkDetail = Request.Form["chkDetail"];
                string[] strChkDetailArray = null;
                if (strChkDetail != null)
                    strChkDetailArray = strChkDetail.Split(',');

                iRolePrivileges.UpdateRolePrivilegesByRoleID(Role_ID.Value, strHdMenuItemIDArray, strChkViewArray, strChkAddArray, strChkEditArray, strChkDeleteArray, strChkDetailArray);

                iLogActivity.AddLogActivity((byte)Global.LogActivityTypes.RolePrivilegesUpdated, "RolePrivileges Updated Successfully", new int[] { Role_ID.Value });

                ViewData["MenuItem"] = null;
                return RedirectToAction("Index", "RolePrivileges", new { Msg = "added" });
            }
            else
            {
                iLogActivity.AddError("RolePrivileges Id is less than zero", new int[] { Role_ID.Value });
                ViewData["MenuItem"] = null;
                return RedirectToAction("Index", "RolePrivileges");
            }
        }
        #endregion

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetRolesByCompanyID(int companyID)
        {
            var Roles = iRolePrivileges.GetRolesByCompanyID(companyID);
         //   iLogActivity.AddInformation("Get roles by company id", new int[] { companyID });
            return Json(Roles, JsonRequestBehavior.AllowGet);
        }
    }
}