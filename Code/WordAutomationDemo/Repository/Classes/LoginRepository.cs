using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Interfaces;

namespace WordAutomationDemo.Repository.Classes
{
    /// <summary>
    /// Created By - Dalvinder Singh
    /// Created Date - 10/08/2017
    /// </summary>
    public class LoginRepository : ILogin
    {
        private readonly ReadyPortalDBEntities _entities;

        public LoginRepository(ReadyPortalDBEntities entities)
        {
            _entities = entities;
        }

        public UserModel CheckLogin(string userName)
        {
            try
            {
                using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
                {
                    return (from items in _entities.tblUserDepartments
                            join role in _entities.tblRoles on items.RoleID ?? 0 equals role.RoleID into roleLeftJoin
                            from roleLJ in roleLeftJoin.DefaultIfEmpty()
                            where items.UserName == userName
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
                                CompanyLogo = items.tblCompany != null && !string.IsNullOrEmpty(items.tblCompany.CompanyLogo) ? items.tblCompany.CompanyLogo : "",
                                ProfileImage = items.ProfileImage,
                                IsActive = items.IsActive.HasValue && items.IsActive.Value ? true : false,
                                IsCompanyActive = items.tblCompany != null && items.tblCompany.IsActive.HasValue && items.tblCompany.IsActive.Value ? true : false,
                                IsRoleActive = items.tblRole != null && items.tblRole.IsActive == true ? true : false,
                                DTformat = items.tblCompany.tblDateFormat.DateFormat,
                                DTFormatGrid = items.tblCompany.tblDateFormat.DateFormat.Replace("Y", "y").Replace("D", "d"),
                                IsAdminUser = roleLJ != null ? roleLJ.IsAdminRole : false,
                                IsMessengerServiceEnable =(bool) items.tblCompany.IsMessengerServiceEnable,
                                EmailID = items.EmailID,
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

        public void SendErrorToText(Exception ex, string functionName = "")
        {
            try
            {
                string fileBasePath = HttpContext.Current.Server.MapPath("~/CSS");  //Text File Path
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

        public string EmployeePhoto(string firstName)
        {
            string Path = string.Empty;
            Path = "Images/" + firstName.Trim().Substring(0, 1).ToUpper() + ".png";
            return Path;
        }

        public List<int?> GetParentList(int role_ID)
        {
            var result = (from p in _entities.vwRolePrivileges
                          where p.RoleID == role_ID && p.ViewPermission == true
                          && p.IsActive == 1
                          select p.ParentID).ToList();
            return result;
        }

        public IList<vwRolePrivilege> GetMainItems(int role_ID)
        {
            var result = _entities.vwRolePrivileges.Where(p => p.ParentID == 0 && p.RoleID == role_ID).OrderBy(p => p.SortOrder).ToList();
            return result;
        }

        public IList<vwRolePrivilege> GetMenuItems(int role_ID)
        {
            var result = _entities.vwRolePrivileges.Where(p => p.RoleID == role_ID && p.ViewPermission == true && p.IsActive == 1).OrderBy(p => p.OrderID).ThenBy(p => p.ParentID).ThenBy(p => p.SortOrder).ThenBy(p => p.MenuItem).ToList();
            return result;
        }
       
        public string CompanyDateFormat(int CompanyID)
        {
            var result = (from p in _entities.tblCompanies
                          join d in _entities.tblDateFormats on p.DateFormatID equals d.DateFormatID into t
                          where p.CompanyID == CompanyID
                          from rt in t.DefaultIfEmpty()
                          select new { rt.DateFormat }).FirstOrDefault();
            return result.DateFormat;
        }

    }
}