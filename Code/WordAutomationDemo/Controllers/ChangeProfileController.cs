using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Interfaces;

namespace WordAutomationDemo.Controllers
{
    [ValidateLogin]
    public class ChangeProfileController : Controller
    {
        #region Members
        private static int Count = 0;
        IUser iUser;
        public ChangeProfileController(IUser IUser)
        {
            iUser = IUser;
        }
        #endregion
        #region Profile
        /// <summary>
        /// Edit – This method is used to edit user.
        /// Create By : Dipak Kansara
        /// Created Date : 08/02/2017
        /// </summary>
        /// <param name="id">ID of user</param>
        /// <returns></returns>
        [HandleError]
        [HttpGet]
        public ActionResult Profile()
        {
            if (Request.QueryString["UID"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                string UID = Global.UrlDecrypt(Request.QueryString["UID"]);
                int userID;
                if (int.TryParse(UID, out userID))
                {
                    UserModel objUserModel = iUser.GetUserByID(userID);
                    if (objUserModel == null)
                    {
                        return RedirectToAction("Index", "ChangeProfile", new { Msg = "drop" });
                    }
                    string strImagePath = System.Web.HttpContext.Current.Server.MapPath("~/ApplicationDocuments/ProfileImages/") + objUserModel.ProfileImage;

                    if (System.IO.File.Exists(strImagePath))
                    {
                        objUserModel.ProfileImage = WordAutomationDemo.Common.Global.SiteUrl + "ApplicationDocuments/ProfileImages/" + objUserModel.ProfileImage;
                    }
                    else
                    {
                        objUserModel.ProfileImage = string.Empty;
                    }
                    objUserModel.Password = Global.Encryption.Decrypt(objUserModel.Password);
                    objUserModel.ConfirmPassword = Global.Encryption.Decrypt(objUserModel.Password);
                    return View(objUserModel);
                }
                else
                {
                    return RedirectToAction("Index", "ChangeProfile");
                }
            }
        }

        [HandleError]
        [HttpPost]
        public ActionResult Profile(UserModel objUserModel, HttpPostedFileBase imgUserProfile)
        {
            string strUserProfile = "";
            string UID = Global.UrlEncrypt(objUserModel.UserId.ToString());
                if (Request.Form["hdnImage"] != null)
                {
                    strUserProfile = Request.Form["hdnImage"];
                }
                var query = iUser.GetUserByID(objUserModel.UserId);
                if (query == null)
                {
                    return RedirectToAction("Index", "ChangeProfile", new { Msg = "drop" });
                }

                else if (iUser.IsUserExistsByEmail("Email", objUserModel.EmailID.Trim(), objUserModel.UserId, CurrentUserSession.User.CompanyID))
                {
                    ModelState.AddModelError("EmailID", string.Format(Messages.AlreadyExists, "Email "));
                    return View(objUserModel);
                }
                if (iUser.ChangeProfile(objUserModel, strUserProfile, imgUserProfile))
                {
                    //if (CurrentUserSession.UserID == objUserModel.UserId)
                    //{
                    //    CurrentUser currentUser = CurrentUserSession.User;
                    //    currentUser.FirstName = objUserModel.FullName.Trim();
                    //    currentUser.LastName = objUserModel.EmailID.Trim();
                    //    currentUser.RoleID = objUserModel.Role_ID;
                    //    currentUser.UserName = objUserModel.UserName.Trim();
                    //    CurrentUserSession.User = currentUser;
                    //}
                    return RedirectToAction("Profile", "ChangeProfile", new { UID = UID, Msg = "changed" });
                }
                else
                {
                    return RedirectToAction("Profile", "ChangeProfile", new { UID = UID, Msg = "error" });
                }
        }

        #endregion
	}
}