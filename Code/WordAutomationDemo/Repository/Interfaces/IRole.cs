using System;
using System.Collections.Generic;
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
    public interface IRole
    {
        /// <summary>
        /// Method to fetch all the role list.
        /// </summary>
        /// <returns>List of role</returns>
        IQueryable<RoleModel> GetRoleList();

        /// <summary>
        /// Method to fetch all the role list for drop down.
        /// </summary>
        /// <returns>List of role</returns>
        List<RoleModel> GetRoleList(int RoleID, string strRole, int IsActive);

        /// <summary>
        /// Check if role with the name exists
        /// </summary>
        /// <param name="RoleID">RoleID</param>
        /// <param name="RoleName">RoleName</param>
        /// <param name="CompanyID">CompanyID</param>
        /// <returns>Success/Failure</returns>
        bool IsRoleExists(int RoleID, string RoleName, int CompanyID);

        /// <summary>
        /// To delete multiple role by id
        /// </summary>
        /// <param name="chkDelete">Role Ids</param>
        /// <returns>Success/Failure</returns>
        void DeleteRoles(int[] chkDelete);

        /// <summary>
        /// To get role data by id.
        /// </summary>
        /// <param name="id">Role Id</param>
        /// <returns>Role model</returns>
        RoleModel GetRoleByID(int id);

        /// <summary>
        /// Insert role data
        /// </summary>
        /// <param name="model">data model</param>
        /// <returns>Success/Failure</returns>
        bool InsertRole(RoleModel model,out tblRole tblRoleModel);

        /// <summary>
        /// To update role data
        /// </summary>
        /// <param name="model">role Model</param>
        /// <returns>Success/Failure</returns>
        bool UpdateRole(RoleModel model);

        /// <summary>
        /// To get role data by company id.
        /// </summary>
        /// <param name="companyID">company ID</param>
        /// <returns>List<SelectListItem></returns>
        List<SelectListItem> GetRolesByCompanyID(int companyID);
    }
}
