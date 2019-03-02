using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Repository.Interfaces
{
    /// <summary>
    /// Created By  : Dipak Kansara
    /// Date : 08/03/2017
    /// </summary>
    public interface IRolePrivileges
    {
        /// <summary>
        /// Method to fetch active and non admin role list.
        /// </summary>
        /// <returns>List of role</returns>
        List<tblRole> GetActiveAndNonAdminRoleList();

        /// <summary>
        /// Method to fetch active role by company id.
        /// </summary>
        /// <returns>List of role</returns>
        List<SelectListItem> GetRolesByCompanyID(int companyID);

        /// <summary>
        /// Method to fetch role privileges.
        /// </summary>
        /// <returns>List of role privileges</returns>
        DataTable ExecuteStoredProcedure(string strSpName, int RoleId);

        /// <summary>
        /// Method to update role privileges by id.
        /// </summary>
        /// <returns></returns>
        bool UpdateRolePrivilegesByRoleID(int roleID, string[] strHdMenuItemIDArray, string[] strChkViewArray, string[] strChkAddArray, string[] strChkEditArray, string[] strChkDeleteArray, string[] strChkDetailArray);

        /// <summary>
        /// Check if user has Updated Doc View Permission
        /// </summary>
        /// <param name="userID">User id</param>
        /// <returns></returns>
        bool HasUpdatedDocViewPermission(int userID);

        /// <summary>
        /// Create role privileges for a new Admin user.
        /// </summary>
        /// <param name="userID">User id</param>
        /// <returns></returns>
        bool CreateRolePrivilegesForAdmin(int roleID);
    }
}
