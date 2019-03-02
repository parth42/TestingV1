using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Interfaces;

namespace WordAutomationDemo.Repository.Classes
{
    /// <summary>
    /// /// <summary>
    /// Created By  : Dalvinder Singh
    /// Date : 03/08/2017
    /// </summary>
    /// </summary>
    public class CompanyRepository : ICompany
    {
        private readonly ReadyPortalDBEntities _entities;

        public CompanyRepository(ReadyPortalDBEntities entities)
        {
            _entities = entities;
        }

        public IQueryable<CompanyModel> Companylist()
        {

            var UserID = CurrentUserSession.UserID;

            var companylist = (from item in _entities.tblCompanies
                               where item.IsDeleted != true && item.CreatedBy == UserID
                               orderby item.CreatedDate descending
                               select new CompanyModel()
                               {
                                   CompanyID = item.CompanyID,
                                   Name = item.Name,
                                   CompanyLogo = item.CompanyLogo,
                                   Address = item.Address,
                                   City = item.City,
                                   Country = item.Country,
                                   State = item.State,
                                   Zip = item.Zip,
                                   IsActive = item.IsActive,
                                   CreatedDate = item.CreatedDate,
                                   IsDeleted = item.IsDeleted,
                                   WebsiteURL = item.WebsiteURL,
                                   IsAppointmentEnable = item.IsAppointmentEnable,
                                   ExchangeServerURL = item.ExchangeServerURL,
                                   ExchangeServerUserName = item.ExchangeServerUserName,
                                   ExchangeServerPassword = item.ExchangeServerPassword
                               }).ToList().AsQueryable();

            return companylist;
        }

        public IQueryable<DateFormatModel> DateFormatlist()
        {

            var UserID = CurrentUserSession.UserID;

            var dateFormatlist = (from item in _entities.tblDateFormats
                                  where item.IsActive == true
                                  select new DateFormatModel()
                                  {
                                      DateFormatID = item.DateFormatID,
                                      DateFormat = item.DateFormat
                                  }).ToList().AsQueryable();

            return dateFormatlist;
        }

        public bool IsDuplicateCompany(string Name, int CompanyId, int userId)
        {
                bool isDuplicateRec = (from items in _entities.tblCompanies
                                       where items.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)
                                       && (CompanyId == 0 || items.CompanyID != CompanyId)
                                       && items.CreatedBy == userId
                                       && items.IsDeleted != true
                                       select items).Any();
                return isDuplicateRec;

        }

        public bool InsertCompany(CompanyModel model, HttpPostedFileBase imgCompanyLogo)
        {

                tblCompany _company = new tblCompany();
                //Upload company logo
                if (imgCompanyLogo != null && imgCompanyLogo.ContentLength > 0)
                {
                    _company.CompanyLogo = UploadCompanyLogo(imgCompanyLogo);
                }
                else
                {
                    _company.CompanyLogo = string.Empty;
                }
                _company.Name = model.Name;
                _company.Address = model.Address;
                _company.City = model.City;
                _company.State = model.State;
                _company.Zip = model.Zip;
                _company.WebsiteURL = model.WebsiteURL;
                _company.Country = model.Country;
                _company.IsActive = model.IsActive;
                _company.IsAppointmentEnable = model.IsAppointmentEnable;
                _company.IsMessengerServiceEnable = model.IsMessengerServiceEnable;
                if (model.IsAppointmentEnable == true)
                {
                    _company.ExchangeServerURL = model.ExchangeServerURL;
                    _company.ExchangeServerUserName = model.ExchangeServerUserName;
                    _company.ExchangeServerPassword = WordAutomationDemo.Common.Global.Encryption.Encrypt(model.ExchangeServerPassword);
                }
                else {
                    _company.ExchangeServerURL = null;
                    _company.ExchangeServerUserName = null;
                    _company.ExchangeServerPassword = null;
                }
                _company.CreatedBy = CurrentUserSession.UserID;
                _company.CreatedDate = DateTime.Now;
                _company.IsDeleted = false;
                _company.DateFormatID = model.DateFormatID; 
                _entities.tblCompanies.Add(_company);
                _entities.SaveChanges();

                tblRole role = new tblRole();
                role.CompanyID = _company.CompanyID;
                role.Role = Global.Role;
                role.Description = "";
                role.IsActive = true;
                role.IsAdminRole = true;
                role.CreatedBy = CurrentUserSession.UserID;
                role.CreatedDate = DateTime.Now;
                _entities.tblRoles.Add(role);
                _entities.SaveChanges();

                var MenuItemID = Global.MenuItemID.Split(',');
                foreach (var item in MenuItemID)
                {
                    tblRolePrivilage objtblRolePrivilage = new tblRolePrivilage();
                    objtblRolePrivilage.RoleID = role.RoleID;
                    objtblRolePrivilage.MenuItemID = Convert.ToInt32(item);
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
                }

                _entities.SaveChanges();
                return true;

        }

        public void SendErrorToText(string message, string functionName = "")
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

        public void SendErrorToText(Exception ex, string functionName = "")
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

        public string UploadCompanyLogo(HttpPostedFileBase imgCompanyLogo)
        {
                string strImagePath = HttpContext.Current.Server.MapPath("~/ApplicationDocuments/CompanyLogos/");

                bool isPathExists = System.IO.Directory.Exists(strImagePath);

                if (!isPathExists)
                {
                    System.IO.Directory.CreateDirectory(strImagePath);
                }

                WebImage companyLogo = new WebImage(imgCompanyLogo.InputStream);
                if (companyLogo.Width > 400 || companyLogo.Height > 400)
                {
                    companyLogo.Resize(400, 400);
                }

                companyLogo.FileName = DateTime.UtcNow.ToLongDateString();
                var hjhj = imgCompanyLogo.GetType();
                string strImageName = DateTime.UtcNow.ToString("yyyyMMddHHmmssffff") + System.IO.Path.GetExtension(imgCompanyLogo.FileName);

                companyLogo.Save(strImagePath + strImageName);

                return strImageName;
        }

        public CompanyModel GetCompanyById(int id)
        {
            var UserID = CurrentUserSession.UserID;

            var company = (from item in _entities.tblCompanies
                           where item.IsDeleted != true && item.CreatedBy == UserID && item.CompanyID == id
                           orderby item.CreatedDate descending
                           select new CompanyModel()
                           {
                               CompanyID = item.CompanyID,
                               Name = item.Name,
                               Address = item.Address,
                               City = item.City,
                               State = item.State,
                               Zip = item.Zip,
                               Country = item.Country,
                               CompanyLogo = item.CompanyLogo,
                               CreatedBy = item.CreatedBy,
                               CreatedDate = item.CreatedDate,
                               IsActive = item.IsActive,
                               IsDeleted = item.IsDeleted,
                               IsSuperAdmin = item.IsSuperAdmin,
                               IsMessengerServiceEnable = (bool)item.IsMessengerServiceEnable ,
                               WebsiteURL = item.WebsiteURL,
                               DateFormatID= (int)item.DateFormatID ,
                               DateFormat = item.tblDateFormat.DateFormat,
							   IsAppointmentEnable = item.IsAppointmentEnable,
                               ExchangeServerURL = item.ExchangeServerURL,
                               ExchangeServerUserName = item.ExchangeServerUserName,
                               ExchangeServerPassword = item.ExchangeServerPassword 
                           }).FirstOrDefault();
            return company;
        }

        public bool UpdateCompany(int id, CompanyModel model, HttpPostedFileBase imgCompanyLogo)
        {
                tblCompany _company = _entities.tblCompanies.Where(x => x.CompanyID == id).FirstOrDefault();

                if (_company != null && _company.CompanyID > 0)
                {
                    //Upload company logo
                    if (imgCompanyLogo != null && imgCompanyLogo.ContentLength > 0)
                    {
                        _company.CompanyLogo = UploadCompanyLogo(imgCompanyLogo);

                    }
                    else if (!String.IsNullOrEmpty(_company.CompanyLogo) && String.IsNullOrEmpty(model.CompanyLogo))
                    {
                        DeleteCompanyLogo(_company.CompanyLogo); //Remove previous logo
                        _company.CompanyLogo = string.Empty;

                    }
                    _company.Name = model.Name;
                    _company.Address = model.Address;
                    _company.City = model.City;
                    _company.State = model.State;
                    _company.Zip = model.Zip;
                    _company.Country = model.Country;
                    _company.IsActive = model.IsActive;
                    _company.IsAppointmentEnable = model.IsAppointmentEnable;
                    _company.DateFormatID  = model.DateFormatID ;
                    _company.IsMessengerServiceEnable = model.IsMessengerServiceEnable; 
                    if (model.IsAppointmentEnable == true)
                    {
                        _company.ExchangeServerURL = model.ExchangeServerURL;
                        _company.ExchangeServerUserName = model.ExchangeServerUserName;
                        _company.ExchangeServerPassword = WordAutomationDemo.Common.Global.Encryption.Encrypt(model.ExchangeServerPassword);
                    }
                    else
                    {
                        _company.ExchangeServerURL = null;
                        _company.ExchangeServerUserName = null;
                        _company.ExchangeServerPassword = null;
                    }

                    _company.WebsiteURL = model.WebsiteURL;
                    _company.ModifiedBy = CurrentUserSession.UserID;
                    _company.ModifiedDate = DateTime.Now;

                    _entities.Entry(_company).State = System.Data.Entity.EntityState.Modified;
                }

                _entities.SaveChanges();

                return true;

        }

        public void DeleteCompanyLogo(string companyLogo)
        {
                string strImagePath = HttpContext.Current.Server.MapPath("~/ApplicationDocuments/CompanyLogos/") + companyLogo;

                bool isPathExists = System.IO.File.Exists(strImagePath);

                if (isPathExists)
                {
                    System.IO.File.Delete(strImagePath);
                }
        }

        public void DeleteCompanies(int[] chkDelete)
        {
            foreach (var item in chkDelete)
            {
                var companyEntity = _entities.tblCompanies.Where(a => a.CompanyID == item);
                if (companyEntity != null && !String.IsNullOrEmpty(companyEntity.FirstOrDefault().CompanyLogo))
                {
                    DeleteCompanyLogo(companyEntity.FirstOrDefault().CompanyLogo);
                }
                _entities.tblCompanies.RemoveRange(companyEntity);
            }
            _entities.SaveChanges();
        }

        public List<CompanyModel> GetCompanyList()
        {
            List<CompanyModel> companyData = new List<CompanyModel>();
            if (CurrentUserSession.User.IsSuperAdmin == true)
            {
                companyData = (from item in _entities.tblCompanies
                               where item.IsDeleted != true && item.IsActive == true
                               orderby item.Name ascending
                               select new CompanyModel
                               {
                                   CompanyID = item.CompanyID,
                                   Name = item.Name
                               }).ToList();
            }
            else
            {
                companyData = (from item in _entities.tblCompanies
                               where item.IsDeleted != true && item.IsActive == true
                               && item.CompanyID == CurrentUserSession.User.CompanyID
                               orderby item.Name ascending
                               select new CompanyModel
                               {
                                   CompanyID = item.CompanyID,
                                   Name = item.Name
                               }).ToList();
            }
            return companyData;
        }

        public List<DateFormatModel> GetDateFormatList()
        {
            List<DateFormatModel> dateformatData = new List<DateFormatModel>();
            dateformatData = (from item in _entities.tblDateFormats
                           select new DateFormatModel
                           {
                               DateFormatID = item.DateFormatID,
                               DateFormat = item.DateFormat
                           }).ToList();
            return dateformatData;
        }

        public bool IsCompanyExistsByName(string FieldList, string ValueList, int strAddOrEditID)
        {
            bool IsExists = false;
            string strCompanyName = string.Empty;
            if (!string.IsNullOrEmpty(FieldList))
            {
                if (strAddOrEditID == -1 && strAddOrEditID != 0)
                {

                    strCompanyName = this.GetCompanyCount(ValueList.Trim(), 0) == 0 ? null : "1";

                }
                else if (strAddOrEditID != 0)
                {

                    strCompanyName = this.GetCompanyCount(ValueList.Trim(), strAddOrEditID) == 0 ? null : "1";

                }

                if (!string.IsNullOrEmpty(strCompanyName))
                {
                    IsExists = true;
                }
            }
            return IsExists;
        }

        public int GetCompanyCount(string companyName, int companyID)
        {
            return (from p in _entities.tblCompanies
                    where
                    (string.IsNullOrEmpty(companyName) || p.Name == companyName)
                    && (companyID == 0 || p.CompanyID != companyID)
                    select p.CompanyID).Count();
        }

        public String  GetDateFormatByCompanyId(int id)
        {
            var UserID = CurrentUserSession.UserID;

            var DateFormat = (from item in _entities.tblCompanies
                           where item.IsDeleted != true && item.CreatedBy == UserID && item.CompanyID == id
                           orderby item.CreatedDate descending
                           select 
                               item.tblDateFormat.DateFormat
                           ).FirstOrDefault().ToString ();
            return DateFormat;
        }

        public Boolean  GetMessangerServiceByCompanyId(int id)
        {
            var UserID = CurrentUserSession.UserID;

            var MessengerService = (from item in _entities.tblCompanies
                              where item.IsDeleted != true && item.CreatedBy == UserID && item.CompanyID == id
                              orderby item.CreatedDate descending
                              select
                                  item.IsMessengerServiceEnable 
                           ).FirstOrDefault().ToString();
            return Convert.ToBoolean (MessengerService);
        }

        public List<string> GetBccEmailList()
        {
            var lstBccEmail = (from c in _entities.tblCompanies
                               join u in _entities.tblUserDepartments on c.CompanyID equals u.CompanyID
                               join r in _entities.tblRoles on u.RoleID equals r.RoleID
                               where u.IsActive == true && c.IsActive == true && c.IsSuperAdmin.HasValue && c.IsSuperAdmin.Value == true
                               && r.IsAdminRole == true
                               select new
                               {
                                   u.EmailID
                               }
                           ).Select(s => s.EmailID).ToList();

            return lstBccEmail;
        }
    }
}