using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Repository.Interfaces
{
    /// <summary>
    /// Created By - Dalvinder Singh
    /// Created Date - 10/08/2017
    /// </summary>
    public interface ILogin
    {
        UserModel CheckLogin(string userName);
        void SendErrorToText(Exception ex, string functionName = "");
        string EmployeePhoto(string firstName);
        List<int?> GetParentList(int role_ID);
        IList<vwRolePrivilege> GetMainItems(int role_ID);
        IList<vwRolePrivilege> GetMenuItems(int role_ID);
        string CompanyDateFormat(int CompanyID);


    }
}