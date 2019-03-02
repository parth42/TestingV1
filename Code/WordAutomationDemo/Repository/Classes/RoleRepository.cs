using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Interfaces;

namespace WordAutomationDemo.Repository.Classes
{
    public class RoleRepository :IRole
    {
        private readonly ReadyPortalDBEntities _entities;
        ILogActivity iLogActivity;

        public RoleRepository(ReadyPortalDBEntities entities, ILogActivity ILogActivity)
        {
            _entities = entities;
            iLogActivity = ILogActivity;
        }

        public List<RoleModel> GetRoleList(int RoleID, string strRole, int IsActive)
        {
            List<RoleModel> RoleData = (from p in _entities.tblRoles
                                        where (string.IsNullOrEmpty(strRole) || p.Role == strRole)
                                        && (RoleID == 0 || p.RoleID != RoleID)
                                        && (IsActive == 0 ? (p.IsActive == true || p.IsActive == false) : IsActive == 1 ? (p.IsActive == true) : (p.IsActive == false))
                                        orderby p.Role
                                        select new RoleModel
                                        {
                                            RoleID = p.RoleID,
                                            Role = p.Role,
                                            Description = p.Description,
                                            Active = p.IsActive == true ? Global.ActiveText : Global.InActiveText,
                                            IsAdminRole = p.IsAdminRole
                                        }).ToList();

            if (!CurrentUserSession.User.IsSuperAdmin)
            {
                RoleData = RoleData.Where(q => q.Company_ID == CurrentUserSession.User.CompanyID).ToList();
            }

            return RoleData;
        }

        public IQueryable<RoleModel> GetRoleList()
        {
            var RoleData = (from p in _entities.tblRoles
                            orderby p.Role
                            select new RoleModel
                            {
                                RoleID = p.RoleID,
                                Role = p.Role,
                                Description = p.Description,
                                Active = p.IsActive == true ? Global.ActiveText : Global.InActiveText,
                                IsAdminRole = p.IsAdminRole,
                                Company_ID = p.CompanyID,
                                Company = p.tblCompany != null ? p.tblCompany.Name : ""
                            });

            if (!CurrentUserSession.User.IsSuperAdmin)
            {
                RoleData = RoleData.Where(q => q.Company_ID == CurrentUserSession.User.CompanyID);
            }

            return RoleData;
        }

        public RoleModel GetRoleByID(int RoleID)
        {
            using (ReadyPortalDBEntities ctx = new ReadyPortalDBEntities())
            {
                RoleModel role = (from p in ctx.tblRoles
                                  where p.RoleID == RoleID
                                  select new RoleModel
                                  {
                                      RoleID = p.RoleID,
                                      Role = p.Role,
                                      Description = p.Description,
                                      IsActive = p.IsActive,
                                      Active = (p.IsActive == true ? Global.ActiveText : Global.InActiveText),
                                      IsAdminRole = p.IsAdminRole,
                                      Company_ID = p.CompanyID,
                                      Company = p.tblCompany != null ? p.tblCompany.Name : ""
                                  }).FirstOrDefault();

                return role;
            }
        }

        public void DeleteRoles(int[] IDs)
        {
            try
            {
                using (ReadyPortalDBEntities ctx = new ReadyPortalDBEntities())
                {
                    List<RoleModel> roleList = new List<RoleModel>();
                    foreach (var id in IDs)
                    {
                        var roleModel = GetRoleByID(id);
                        if (roleModel != null)
                        {
                            roleList.Add(roleModel);
                        }
                    }

                    ctx.tblRoles.RemoveRange(ctx.tblRoles.Where(r => IDs.Contains(r.RoleID)).AsEnumerable());
                    ctx.SaveChanges();

                    foreach (var roleModel in roleList)
                    {
                        iLogActivity.AddLogActivity((byte)Global.LogActivityTypes.UserRoleDeleted, String.Format("Role <{0}> deleted successfully", roleModel.Role), new int[] { roleModel.RoleID });
                    }
                }
            }catch(Exception ex)
            {
                throw;
            }
        }

        public bool InsertRole(RoleModel objRoleModel,out tblRole tblRoleModel)
        {
            tblRole role = new tblRole();
            role.Role = objRoleModel.Role.Trim();
            role.CompanyID = objRoleModel.Company_ID;
            role.Description = !string.IsNullOrEmpty(objRoleModel.Description) ? objRoleModel.Description.Trim() : objRoleModel.Description;
            role.IsActive = objRoleModel.IsActive;
            role.IsAdminRole = objRoleModel.IsAdminRole;
            role.CreatedBy = CurrentUserSession.UserID;
            role.CreatedDate = DateTime.Now;
            _entities.tblRoles.Add(role);
            _entities.SaveChanges();
            tblRoleModel = role;
            return true;
        }

        public bool UpdateRole(RoleModel objRoleModel)
        {
                tblRole role = _entities.tblRoles.Where(r => r.RoleID == objRoleModel.RoleID).FirstOrDefault();
                role.Role = objRoleModel.Role.Trim();
                role.CompanyID = objRoleModel.Company_ID;
                role.Description = !string.IsNullOrEmpty(objRoleModel.Description) ? objRoleModel.Description.Trim() : objRoleModel.Description;
                role.IsActive = objRoleModel.IsActive;
                role.IsAdminRole = objRoleModel.IsAdminRole;
                role.ModifiedBy = CurrentUserSession.UserID;
                role.ModifiedDate = DateTime.Now;
                _entities.SaveChanges();
                return true;
        }

        public bool IsRoleExists(int RoleID, string RoleName, int companyID)
        {
            using (ReadyPortalDBEntities context = new ReadyPortalDBEntities())
            {
                bool result = (from r in context.tblRoles
                               where (RoleID == 0 || r.RoleID != RoleID)
                               && (string.Compare(r.Role.Trim(), RoleName, true) == 0)
                               && r.CompanyID == companyID
                               select r.RoleID).Any();
                return result;
            }
        }

        public List<SelectListItem> GetRolesByCompanyID(int companyID)
        {
            if (companyID > 0)
            {
                return (from p in _entities.tblRoles
                        where p.CompanyID == companyID && p.IsActive == true
                        orderby p.Role
                        select new SelectListItem()
                        {
                            Text = p.Role,
                            Value = SqlFunctions.StringConvert((double)p.RoleID).Trim(),
                        }).ToList();
            }
            else
            {
                return new List<SelectListItem>();
            }
        }
    }
}