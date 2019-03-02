using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Controllers
{
    public class ForgotPasswordController : Controller
    {
        #region Index

        /// <summary>
        /// Created By: Dipak B. Kansara
        /// Created On: 08/24/2017
        /// </summary>
        /// <returns>View</returns>
        [HandleError]
        public ActionResult Index()
        {
            return View();
        }

        [HandleError]
        [HttpPost]
        public ActionResult Index(ForgotPasswordModel objForgotPasswordModel)
        {
            if (ModelState.IsValid)
            {
                ForgotPasswordModel _ForgotPasswordModel = CommonHelper.CheckUserByUserNameEmail(objForgotPasswordModel.UserName, objForgotPasswordModel.Email);
                if (_ForgotPasswordModel != null)
                {
                    var templates = CommonHelper.GetEmailTemplateContent("Forgot Password");

                    string EmailBody = templates.TemplateContentForEmail;
                    EmailBody = PopulateEmailBody(_ForgotPasswordModel, EmailBody);

                    Global.SendEmail(objForgotPasswordModel.Email, templates.Subject, EmailBody);

                    return RedirectToAction("Index", "Login", new { Msg = "Forgot" });
                }
                else
                {
                    return RedirectToAction("Index", "ForgotPassword", new { MSg = "NotFound" });
                }
            }
            return View(objForgotPasswordModel);
        }
        #endregion

        #region Populate Email Body
        /// <summary>
        /// Populate Email Body – This method is used to populate email body.
        /// Created By: Dipak B. Kansara
        /// Created On: 08/24/2017
        /// </summary>
        /// <param name="result">result</param>
        /// <param name="EmailBody">Email Body</param>
        /// <returns>string</returns>
        #region Email body Html
        private string PopulateEmailBody(ForgotPasswordModel result, string EmailBody)
        {
            var SiteUrl = Global.SiteUrl;
            EmailBody = EmailBody.Replace("#FullName#", Convert.ToString(result.FirstName));
            EmailBody = EmailBody.Replace("#click here#", "<a href=" + SiteUrl + "ResetPassword?UID=" + Guid.NewGuid() + "~" + Global.Encryption.Encrypt(result.UserID.ToString()) + ">click here</a>");
            return EmailBody;
        }
        #endregion
        #endregion
    }
}