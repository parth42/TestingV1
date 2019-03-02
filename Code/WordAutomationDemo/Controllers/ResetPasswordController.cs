using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Interfaces;

namespace WordAutomationDemo.Controllers
{
    public class ResetPasswordController : Controller
    {
        #region Members

        IUser iUser;
        
        public ResetPasswordController(IUser IUser)
        {
            iUser = IUser;
        }

        #endregion

        #region Index

        /// <summary>
        /// Created By: Dipak B. Kansara
        /// Created On: 08/24/2017
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index()
        {
            string UserDetail = Request.QueryString["UID"];
            if (!string.IsNullOrEmpty(UserDetail))
            {
                UserDetail = Global.Encryption.Decrypt(UserDetail.Substring(UserDetail.LastIndexOf("~") + 1));
            }
            int UserID;
            if (int.TryParse(UserDetail, out UserID))
            {
                var objUserModel = iUser.GetUserByID(UserID);
                if (objUserModel == null)
                {
                    ModelState.AddModelError(string.Empty, Messages.NotExistsUser);
                    return View(new ResetPasswordModel { UserName = string.Empty });
                }

                var objResetPasswordModel = new ResetPasswordModel
                {
                    UserID = UserID,
                    UserName = objUserModel.UserName,
                    OldPassword = WordAutomationDemo.Common.Global.Encryption.Decrypt(objUserModel.Password),
                };
                return View(objResetPasswordModel);
            }

            ModelState.AddModelError(string.Empty, string.Format(Messages.InvalidLink, string.Empty));
            return View(new ResetPasswordModel { UserName = string.Empty });
        }

        [HttpPost]
        public ActionResult Index(ResetPasswordModel objResetPasswordModel)
        {
            if (ModelState.IsValid)
            {
                if (objResetPasswordModel.OldPassword == objResetPasswordModel.Password)
                {
                    ModelState.AddModelError("Password", Messages.NewPasswordCanNotBeSame);
                    return View(objResetPasswordModel);
                }

                UserModel objUserModel = iUser.GetUserByID(objResetPasswordModel.UserID);

                if (objUserModel == null)
                {
                    ModelState.AddModelError("Password", string.Format(Messages.InvalidLink, "Email"));
                    return View(objResetPasswordModel);
                }

                objResetPasswordModel.Password = WordAutomationDemo.Common.Global.Encryption.Encrypt(objResetPasswordModel.Password);

                iUser.ResetUserPassword(objResetPasswordModel.Password, objResetPasswordModel.UserID);
                return RedirectToAction("Index", "Login", new { Msg = "Reset" });
            }
            else
            {
                return View(objResetPasswordModel);
            }
        }
        #endregion
    }
}