using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Common
{
    /// <summary>
    /// /// <summary>
    /// Created By  : Dalvinder Singh
    /// Date : 01/08/2017
    /// </summary>
    /// </summary>
    public interface ICompanyHelper
    {
        /// <summary>
        /// Method to fetch all the companies list.
        /// </summary>
        /// <returns>List of companies</returns>
        /// 
        IQueryable<CompanyModel> Companylist();
        
        /// <summary>
        /// Check if company with the name exists
        /// </summary>
        /// <param name="Name">Company name</param>
        /// <param name="CompanyId">zero or not equal to same name</param>
        /// <param name="userId">Current user id</param>
        /// <returns>Success/Failure</returns>
        bool IsDuplicateCompany(string Name, int CompanyId, int userId);

        /// <summary>
        /// Insert company data
        /// </summary>
        /// <param name="model">Company model</param>
        /// <param name="imgCompanyLogo">Company Logo</param>
        /// <returns>Success/Failure</returns>
        bool InsertCompany(CompanyModel model, HttpPostedFileBase imgCompanyLogo);

        /// <summary>
        /// To upload company logo on server
        /// </summary>
        /// <param name="imgCompanyLogo">Company Logo</param>
        /// <returns>Name of the image stored</returns>
        string UploadCompanyLogo(HttpPostedFileBase imgCompanyLogo);

        /// <summary>
        /// To get company data by id.
        /// </summary>
        /// <param name="id">Company Id</param>
        /// <returns>Company model</returns>
        CompanyModel GetCompanyById(int id);

        /// <summary>
        /// To update company data
        /// </summary>
        /// <param name="id">Company Id</param>
        /// <param name="model">Company Model</param>
        /// <param name="imgCompanyLogo">Company Logo</param>
        /// <returns>Success/Failure</returns>
        bool UpdateCompany(int id, CompanyModel model, HttpPostedFileBase imgCompanyLogo);

        /// <summary>
        /// To delete company logo from server
        /// </summary>
        /// <param name="companyLogo">Success/Failure</param>
        void DeleteCompanyLogo(string companyLogo);

        /// <summary>
        /// To delete multiple companies by id
        /// </summary>
        /// <param name="chkDelete">Company Ids</param>
        /// <returns>Success/Failure</returns>
        bool DeleteCompanies(int[] chkDelete);

        /// <summary>
        /// To get company list
        /// </summary>        
        /// <returns>Company List</returns>
        List<CompanyModel> GetCompanyList();
        /// <summary>
        /// To get Date Format list
        /// </summary>        
        /// <returns>Date Format List</returns>
        List<DateFormatModel> GetDateFormatList();

    }
    public class CompanyHelper : ICompanyHelper
    {
        private readonly ReadyPortalDBEntities _entities;

        public CompanyHelper(ReadyPortalDBEntities entities)
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
                                   IsDeleted = item.IsDeleted
                               });

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
                                      DateFormat = item.DateFormat,
                                  });
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
                _company.Country = model.Country;
                _company.IsActive = model.IsActive;
                _company.CreatedBy = CurrentUserSession.UserID;
                _company.CreatedDate = DateTime.Now;
                _company.IsDeleted = false;
                _company.DateFormatID = model.DateFormatID;
                _entities.tblCompanies.Add(_company);

                _entities.SaveChanges();
                return true;
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
                               DateFormatID =(int)item.DateFormatID ,
                               DateFormat = item.tblDateFormat.DateFormat 
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
                    _company.ModifiedBy = CurrentUserSession.UserID;
                    _company.ModifiedDate = DateTime.Now;
                    _company.DateFormatID = model.DateFormatID; 

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

        public bool DeleteCompanies(int[] chkDelete)
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

                return true;
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
            List<DateFormatModel> dateFormatData = new List<DateFormatModel>();

            dateFormatData = (from item in _entities.tblDateFormats
                              where item.IsActive == true
                              select new DateFormatModel
                              {
                                  DateFormatID = item.DateFormatID,
                                  DateFormat = item.DateFormat
                              }).ToList();


            return dateFormatData;
        }
    }
}