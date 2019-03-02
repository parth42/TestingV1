using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Repository.Interfaces
{
    /// <summary>
    /// /// <summary>
    /// Created By  : Dalvinder Singh
    /// Date : 03/08/2017
    /// </summary>
    /// </summary>
    public interface ICompany
    {
        /// <summary>
        /// Fetch all the companies list.
        /// </summary>
        /// <returns>List of companies</returns>
        /// 
        IQueryable<CompanyModel> Companylist();
        /// <summary>
        /// Fetch all the Date Format list.
        /// </summary>
        /// <returns>List of Date Format</returns>
        /// 

        IQueryable<DateFormatModel> DateFormatlist();

        /// <summary>
        /// Check if company with the same name exists.
        /// </summary>
        /// <param name="Name">Company name</param>
        /// <param name="CompanyId">zero or not equal to same name</param>
        /// <param name="userId">Current user id</param>
        /// <returns>Success/Failure</returns>
        bool IsDuplicateCompany(string Name, int CompanyId, int userId);

        /// <summary>
        /// Insert company data.
        /// </summary>
        /// <param name="model">Company model</param>
        /// <param name="imgCompanyLogo">Company Logo</param>
        /// <returns>Success/Failure</returns>
        bool InsertCompany(CompanyModel model, HttpPostedFileBase imgCompanyLogo);

        /// <summary>
        /// Upload company logo on server.
        /// </summary>
        /// <param name="imgCompanyLogo">Company Logo</param>
        /// <returns>Name of the image stored</returns>
        string UploadCompanyLogo(HttpPostedFileBase imgCompanyLogo);

        /// <summary>
        /// Get company data by id.
        /// </summary>
        /// <param name="id">Company Id</param>
        /// <returns>Company model</returns>
        CompanyModel GetCompanyById(int id);

        /// <summary>
        /// Update company data.
        /// </summary>
        /// <param name="id">Company Id</param>
        /// <param name="model">Company Model</param>
        /// <param name="imgCompanyLogo">Company Logo</param>
        /// <returns>Success/Failure</returns>
        bool UpdateCompany(int id, CompanyModel model, HttpPostedFileBase imgCompanyLogo);

        /// <summary>
        /// Delete company logo from server.
        /// </summary>
        /// <param name="companyLogo">Success/Failure</param>
        void DeleteCompanyLogo(string companyLogo);

        /// <summary>
        /// Delete multiple companies by id
        /// </summary>
        /// <param name="chkDelete">Company Ids</param>
        /// <returns>Success/Failure</returns>
        void DeleteCompanies(int[] chkDelete);

        /// <summary>
        /// Get Date format List
        /// </summary>        
        /// <returns>Date format </returns>
        List<DateFormatModel> GetDateFormatList();
        /// <summary>
        /// Get companies list
        /// </summary>        
        /// <returns>Company List</returns>
        List<CompanyModel> GetCompanyList();

        /// <summary>
        /// Check if company with the name exists
        /// </summary>
        /// <param name="fieldList">fieldList</param>
        /// <param name="valueList">valueList</param>
        /// <param name="strAddOrEditID">Add Or EditID</param>
        /// <returns>Success/Failure</returns>
        bool IsCompanyExistsByName(string fieldList, string valueList, int strAddOrEditID);

        /// <summary>
        /// Get Company Count
        /// </summary>
        /// <param name="companyName">company Name</param>
        /// <param name="companyID">Current user id</param>
        /// <returns>User count</returns>
        int GetCompanyCount(string companyName, int companyID);


        /// <summary>
        /// Get Date Format
        /// </summary>
        /// <param name="DateFormat">DateFormat</param>
        /// <param name="DateFormat Grid">DateFormat Grid</param>
        /// <returns>User count</returns>
        String GetDateFormatByCompanyId( int companyID);

        bool  GetMessangerServiceByCompanyId(int companyID);

        /// <summary>
        /// Get Bcc Email List
        /// </summary>
        /// <returns>Email list</returns>
        List<string> GetBccEmailList();
    }
}