using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;
using System.IO;
using WordAutomationDemo.Repository.Interfaces;

namespace WordAutomationDemo.Controllers
{
    /// <summary>
    /// Created By  : Dalvinder Singh
    /// Date : 31/07/2017
    /// </summary>
    [ValidateLogin]
    public class CompanyController : BaseController
    {
        ICompany iCompany;
        int _Count = 0;

        public CompanyController(ICompany ICompany)
        {
            iCompany = ICompany;
        }

        #region Index

        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Company)]
        public ActionResult Index()
        {
            return View();
        }

        #endregion

        #region Create

        [HttpGet]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Company)]
        public ActionResult Create()
        {
            var model = new CompanyModel();
            model.IsActive = true;
            model.SelectDateFormat = new SelectList(iCompany.GetDateFormatList(), "DateFormatID", "DateFormat");
          
            model.IsAppointmentEnable = true;
            return View(model);
        }

        [HttpPost]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Company)]
        public ActionResult Create(CompanyModel model, HttpPostedFileBase imgCompanyLogo)
        {
                if (ModelState.IsValid)
                {
                    if (!iCompany.IsDuplicateCompany(model.Name, 0, CurrentUserSession.UserID))
                    {
                        if (iCompany.InsertCompany(model, imgCompanyLogo))
                        {
                            return RedirectToAction("Index", "Company", new { Msg = "Created" });
                        }
                        else
                        {
                            ModelState.AddModelError("", Messages.ProvideAllFields.ToString());
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Name", string.Format(Messages.AlreadyExists, "Company"));
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", Messages.ProvideAllFields.ToString());
                    return View(model);
                }
        }

        #endregion

        #region Edit

        [HttpGet]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Company)]
        public ActionResult Edit()
        {
                if (Request.QueryString["UID"] == null)
                {
                    return RedirectToAction("Index", "Company");
                }
                else
                {
                    string UID = Global.UrlDecrypt(Request.QueryString["UID"]);
                    int companyID;
                    if (int.TryParse(UID, out companyID))
                    {
                        if (companyID > 0)
                        {
                           
                            var model = new CompanyModel();
                           
                            var UserID = CurrentUserSession.UserID;

                            var company = iCompany.GetCompanyById(companyID);
                            if (company != null && company.CompanyID > 0)
                            {
                                company.SelectDateFormat = new SelectList(iCompany.GetDateFormatList(), "DateFormatID", "DateFormat");
                                company.ExchangeServerPassword = Global.Encryption.Decrypt(company.ExchangeServerPassword);
                                return View(company);
                            }
                            else
                                return RedirectToAction("Index", "Company", new { Msg = "CompanyNotFound" });
                        }
                        else
                            return RedirectToAction("Index", "Company", new { Msg = "CompanyNotFound" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Company", new { Msg = "CompanyNotFound" });
                    }
                }
        }

        [HttpPost]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Company)]
        public ActionResult Edit(CompanyModel model, HttpPostedFileBase imgCompanyLogo)
        {
                if (ModelState.IsValid && model.CompanyID > 0)
                {

                    if (!iCompany.IsDuplicateCompany(model.Name, model.CompanyID, CurrentUserSession.UserID))
                    {
                        if (iCompany.UpdateCompany(model.CompanyID, model, imgCompanyLogo))
                        {
                            WordAutomationDemo.Helpers.CurrentUserSession.User.DTformat = iCompany.GetDateFormatByCompanyId(model.CompanyID);
                            WordAutomationDemo.Helpers.CurrentUserSession.User.DTFormatGrid = iCompany.GetDateFormatByCompanyId(model.CompanyID).Replace("Y","y").Replace("D","d") ;
                            WordAutomationDemo.Helpers.CurrentUserSession.User.IsMessengerServiceEnable =Convert.ToBoolean(  iCompany.GetMessangerServiceByCompanyId(model.CompanyID));
                            return RedirectToAction("Index", "Company", new { Msg = "CompanyEdited" });
                        }
                        else
                        {
                            ModelState.AddModelError("", string.Format(Messages.AlreadyExists, "Company"));
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", string.Format(Messages.AlreadyExists, "Company"));
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", Messages.ProvideAllFields.ToString());
                    return View(model);
                }
        }

        #endregion

        #region Delete

        [HttpPost]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Company)]
        public ActionResult Delete(int[] chkDelete)
        {
                if (chkDelete.Length > 0)
                {
                    iCompany.DeleteCompanies(chkDelete);
                    return RedirectToAction("Index", "Company", new { Msg = "deleted" });
                }
                else
                {
                    return RedirectToAction("Index", "Company", new { Msg = "NoSelect" });
                }

        }

        #endregion

        #region Details

        [HttpGet]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.Company)]
        public ActionResult Details()
        {
                if (Request.QueryString["UID"] == null)
                {
                    return RedirectToAction("Index", "Company");
                }
                else
                {
                    string UID = Global.UrlDecrypt(Request.QueryString["UID"]);
                    int companyID;
                    if (int.TryParse(UID, out companyID))
                    {
                        if (companyID > 0)
                        {
                            var model = new CompanyModel();
                            var UserID = CurrentUserSession.UserID;

                            var company = iCompany.GetCompanyById(companyID);

                            if (company != null && company.CompanyID > 0)
                            {
                                return View(company);
                            }
                            else
                                return RedirectToAction("Index", "Company", new { Msg = "CompanyNotFound" });
                        }
                        else
                            return RedirectToAction("Index", "Company", new { Msg = "CompanyNotFound" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Company");
                    }
                }
        }

        #endregion

        #region methods

        public ActionResult GetAllCompanies([DataSourceRequest]DataSourceRequest command)
        {
            try
            {
                var UserID = CurrentUserSession.UserID;

                var companylist = iCompany.Companylist();

                //   Apply filtering
                companylist = companylist.ApplyFiltering(command.Filters);

                //Get count of total records
                if (companylist != null)
                {
                    _Count = companylist.Count();
                }

                //Apply sorting
                companylist = companylist.ApplySorting(command.Groups, command.Sorts);

                //Apply paging
                companylist = companylist.ApplyPaging(command.Page, command.PageSize);

                foreach (var company in companylist)
                {
                    company.EncryptedCompanyID = Global.UrlEncrypt(company.CompanyID.ToString());
                }

                var result = new DataSourceResult()
                {
                    Data = companylist,
                    Total = _Count // Total number of records
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                _Count = 0;
                return null;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [HandleError]
        public ActionResult ValidateDuplicateCompany(string FieldList, string ValueList, int strAddOrEditID)
        {
            bool IsCompanyExists = iCompany.IsCompanyExistsByName(FieldList, ValueList, strAddOrEditID);
            if (IsCompanyExists)
            {
                return Json("0");
            }
            else
            {
                return Json(new { @status = "1" });
            }
        }

        #endregion
    }
}