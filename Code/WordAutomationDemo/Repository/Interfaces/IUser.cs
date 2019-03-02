using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Repository.Interfaces
{
    /// <summary>
    /// Created By  : Dipak Kansara
    /// Date : 08/02/2017
    /// </summary>
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Method to fetch all the user list.
        /// </summary>
        /// <returns>List of user</returns>
        IQueryable<UserModel> Userlist();

        /// <summary>
        /// Get User Count
        /// </summary>
        /// <param name="userName">user Name</param>
        /// <param name="email">email</param>
        /// <param name="userID">Current user id</param>
        /// <returns>User count</returns>
        int GetUserCount(string userName, string email, int userID, int companyID);

        /// <summary>
        /// Check if user with the email exists
        /// </summary>
        /// <param name="fieldList">fieldList</param>
        /// <param name="valueList">valueList</param>
        /// <param name="strAddOrEditID">Add Or EditID</param>
        /// <returns>Success/Failure</returns>
        bool IsUserExistsByEmail(string fieldList, string valueList, int strAddOrEditID, int companyID);

        /// <summary>
        /// Insert user data
        /// </summary>
        /// <param name="model">user model</param>
        /// <param name="strUserProfile">User Logo</param>
        /// <returns>Success/Failure</returns>
        bool InsertUser(UserModel model, string strUserProfile, out tblUserDepartment tblUserDepartmentModel);

        /// <summary>
        /// To upload user logo on server
        /// </summary>
        /// <param name="imgUserLogo">User Logo</param>
        /// <returns>Name of the image stored</returns>
        string UploadProfileImage(string strUserProfile);

        /// <summary>
        /// To get user data by id.
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>User model</returns>
        UserModel GetUserByID(int id);

        /// <summary>
        /// To update user data
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="model">User Model</param>
        /// <param name="strUserProfile">User Logo</param>
        /// <returns>Success/Failure</returns>
        bool UpdateUser(UserModel model, string strUserProfile, HttpPostedFileBase imgUserProfile);

        /// <summary>
        /// To change profile
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="model">User Model</param>
        /// <param name="strUserProfile">User Logo</param>
        /// <returns>Success/Failure</returns>
        bool ChangeProfile(UserModel model, string strUserProfile, HttpPostedFileBase imgUserProfile);

        /// <summary>
        /// To delete user logo from server
        /// </summary>
        /// <param name="companyLogo">Success/Failure</param>
        void DeleteUserProfileImage(string imgUserProfile);

        /// <summary>
        /// To delete multiple user by id
        /// </summary>
        /// <param name="chkDelete">User Ids</param>
        /// <returns>Success/Failure</returns>
        void DeleteUsers(int[] chkDelete);

        /// <summary>
        /// To set user password.
        /// </summary>
        /// <param name="password">password</param>
        /// <param name="userID">userID</param>
        /// <returns></returns>
        void ResetUserPassword(string password, int userID);

    }
}
