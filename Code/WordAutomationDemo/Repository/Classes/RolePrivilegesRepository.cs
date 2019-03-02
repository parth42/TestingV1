using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Interfaces;

namespace WordAutomationDemo.Repository.Classes
{
    public class RolePrivilegesRepository : IRolePrivileges
    {
        private readonly ReadyPortalDBEntities _entities;

        public RolePrivilegesRepository(ReadyPortalDBEntities entities)
        {
            _entities = entities;
        }

        public List<tblRole> GetActiveAndNonAdminRoleList()
        {
            return _entities.tblRoles.Where(c => c.IsActive == true && !(c.IsAdminRole == true)).OrderBy(c => c.Role).ToList();
        }

        public List<SelectListItem> GetRolesByCompanyID(int companyID = 0)
        {
            if (companyID > 0)
            {
                return (from p in _entities.tblRoles
                        where p.CompanyID == companyID && p.IsActive == true && !(p.IsAdminRole == true)
                        orderby p.Role
                        select new SelectListItem()
                        {
                            Text = p.Role,
                            Value = SqlFunctions.StringConvert((double)p.RoleID).Trim(),
                        }).ToList();
            }
            else {
                return new List<SelectListItem>();
            }
        }

        public DataTable ExecuteStoredProcedure(string strSpName, int RoleId)
        {
            SqlConnection con = null;
            DataTable dt = null;
            using (con = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["connectionString"]))
            {
                con.Open();
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Connection = con;
                sqlcmd.CommandText = strSpName;
                SqlDataAdapter sqladp = new SqlDataAdapter(sqlcmd);
                sqlcmd.Parameters.Add(new SqlParameter("@RoleID", RoleId));
                dt = new DataTable();
                sqladp.Fill(dt);
            }
            return dt;
        }

        public bool UpdateRolePrivilegesByRoleID(int roleID, string[] strHdMenuItemIDArray, string[] strChkViewArray, string[] strChkAddArray, string[] strChkEditArray, string[] strChkDeleteArray, string[] strChkDetailArray)
        {
            using (var dbTransaction = _entities.Database.BeginTransaction())
            {
                    _entities.tblRolePrivilages.RemoveRange(_entities.tblRolePrivilages.Where(c => c.RoleID == roleID).AsEnumerable());
                    _entities.SaveChanges();
                    
                    int MenuItemID = 0;

                    foreach (string i in strHdMenuItemIDArray)
                    {
                        MenuItemID = Convert.ToInt32(i);
                        bool isView = false;
                        bool isAdd = false;
                        bool isEdit = false;
                        bool isDelete = false;
                        bool isDetail = false;

                        if (strChkViewArray != null && strChkViewArray.Contains("v" + MenuItemID))
                            isView = true;
                        if (strChkAddArray != null && strChkAddArray.Contains("a" + MenuItemID))
                            isAdd = true;
                        if (strChkEditArray != null && strChkEditArray.Contains("e" + MenuItemID))
                            isEdit = true;
                        if (strChkDeleteArray != null && strChkDeleteArray.Contains("d" + MenuItemID))
                            isDelete = true;
                        if (strChkDetailArray != null && strChkDetailArray.Contains("de" + MenuItemID))
                            isDetail = true;

                        if (isView || isAdd || isEdit || isDelete || isDetail)
                        {
                            tblRolePrivilage objtblRolePrivilage = new tblRolePrivilage();
                            objtblRolePrivilage.RoleID = roleID;
                            objtblRolePrivilage.MenuItemID = Convert.ToInt32(MenuItemID);
                            objtblRolePrivilage.UserID = CurrentUserSession.UserID;
                            objtblRolePrivilage.ViewPermission = isView;
                            objtblRolePrivilage.Add = isAdd;
                            objtblRolePrivilage.Edit = isEdit;
                            objtblRolePrivilage.Delete = isDelete;
                            objtblRolePrivilage.Detail = isDetail;
                            objtblRolePrivilage.IsActive = 1;
                            objtblRolePrivilage.CreatedBy = CurrentUserSession.UserID;
                            objtblRolePrivilage.CreatedDate = DateTime.Now;
                            _entities.tblRolePrivilages.Add(objtblRolePrivilage);
                            _entities.SaveChanges();
                        }
                    }
                    dbTransaction.Commit();
                    return true;
            }
        }

        public bool CreateRolePrivilegesForAdmin(int roleID)
        {
            using (var dbTransaction = _entities.Database.BeginTransaction())
            {
                try
                {
                    var menuItemIds = _entities.tblMenuItems.Select(mi => mi.MenuItemID).ToList();

                    foreach (var menuItemId in menuItemIds)
                    {
                        tblRolePrivilage objtblRolePrivilage = new tblRolePrivilage();
                        objtblRolePrivilage.RoleID = roleID;
                        objtblRolePrivilage.MenuItemID = menuItemId;
                        objtblRolePrivilage.UserID = CurrentUserSession.UserID;
                        objtblRolePrivilage.ViewPermission = true;
                        objtblRolePrivilage.Add = true;
                        objtblRolePrivilage.Edit = true;
                        objtblRolePrivilage.Delete = true;
                        objtblRolePrivilage.Detail = true;
                        objtblRolePrivilage.IsActive = 1;
                        objtblRolePrivilage.CreatedBy = CurrentUserSession.UserID;
                        objtblRolePrivilage.CreatedDate = DateTime.Now;
                        _entities.tblRolePrivilages.Add(objtblRolePrivilage);
                        _entities.SaveChanges();
                    }

                    dbTransaction.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    dbTransaction.Rollback();
                    return false;
                }
                
            }
        }

        public bool HasUpdatedDocViewPermission(int roleID)
        {
                var objRolePrivilages = (from rp in _entities.tblRolePrivilages.Where(r => r.RoleID == roleID)
                                         join mi in _entities.tblMenuItems.Where(m => m.MenuItem.Equals("Updated Document")) on rp.MenuItemID equals mi.MenuItemID
                                         select rp).ToList();

                return objRolePrivilages.Any(a => a.ViewPermission == true);
        }
    }
}