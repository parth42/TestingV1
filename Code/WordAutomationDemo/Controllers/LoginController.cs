using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Interfaces;

namespace WordAutomationDemo.Controllers
{
    /// <summary>
    /// Created By - Dalvinder Singh
    /// Created Date - 10/08/2017
    /// </summary>
    public class LoginController : Controller
    {
        ILogin iLogin;
        IRolePrivileges iRolePrivileges;

        public LoginController(ILogin ILogin, IRolePrivileges IRolePrivileges)
        {
            iLogin = ILogin;
            iRolePrivileges = IRolePrivileges;
        }

        // GET: Login
        public ActionResult Index()
        {

            if (Request.Cookies["Triyosoft"] != null)
            {
                ViewBag.UserName = Request.Cookies["Triyosoft"].Values["UserName"].ToString();
                ViewBag.UserPassword = Request.Cookies["Triyosoft"].Values["UserPassword"].ToString();
                if (Request.Cookies["Triyosoft"].Values["Rememberme"] == "on")
                {
                    ViewBag.Rememberme = "checked";
                }
                else
                {
                    ViewBag.Rememberme = "";
                }
            }

            if (CurrentUserSession.UserID > 0)
                return RedirectToAction("Approval", "Home");
            else
                return View();
        }

        [HttpPost]
        public ActionResult Index(int? id)
        {
            try
            {
                string UserName = Convert.ToString(Request.Form["UserName"]);
                string Password = Convert.ToString(Request.Form["UserPassword"]);
                string RememberMe = Convert.ToString(Request.Form["chkRemember"]);

                string strMessage = Global.IsADEnabled ? IsAuthenticated(System.Configuration.ConfigurationManager.AppSettings["LDAPDomain"], System.Configuration.ConfigurationManager.AppSettings["LDAPDomainAdminUserName"], System.Configuration.ConfigurationManager.AppSettings["LDAPDomainAdminPassword"], UserName, Password) : "1";

                if (strMessage == "1")
                {
                    var resp = iLogin.CheckLogin(UserName);
                    if (resp != null && resp.UserId > 0)
                    {
                        if (Global.IsADEnabled || Global.Encryption.Decrypt(resp.Password) == Password)
                        {
                            if (resp.IsActive)
                            {
                                if (resp.IsCompanyActive)
                                {
                                    if (resp.IsRoleActive)
                                    {
                                        CurrentUser User = new CurrentUser();
                                        User.UserName = resp.UserName;
                                        User.FirstName = resp.FullName;
                                        User.strUserID = resp.UserId.ToString();
                                        User.UserID = resp.UserId;
                                        User.RoleID = resp.Role_ID;
                                        User.CompanyID = resp.Company_ID;
                                        User.CompanyLogo = resp.CompanyLogo;
                                        User.IsSuperAdmin = resp.IsSuperAdmin;
                                        User.ProfileImage = resp.ProfileImage = string.IsNullOrWhiteSpace(resp.ProfileImage) ? "/CSS/images/no-photo.jpg" : "/ApplicationDocuments/ProfileImages/" + resp.ProfileImage;
                                        User.DTformat = resp.DTformat;
                                        User.DTFormatGrid = resp.DTFormatGrid;
                                        User.DTFormatGrid = resp.DTFormatGrid;
                                        User.HasUpdatedDocViewPermission = resp.IsSuperAdmin || resp.IsAdminUser ? true : iRolePrivileges.HasUpdatedDocViewPermission(resp.Role_ID);
                                        User.IsAdminUser = resp.IsAdminUser;
                                        User.IsMessengerServiceEnable = resp.IsMessengerServiceEnable;
                                        User.EmailID = resp.EmailID;
                                        User.CanEdit = resp.CanEdit;
                                        User.CanApprove = resp.CanApprove;
                                        CurrentUserSession.User = User;
                                        Session["EPT"] = iLogin.EmployeePhoto(User.FirstName);
                                        IList<vwRolePrivilege> MenuItems = null;
                                        IList<vwRolePrivilege> MainItems = null;

                                        var ParentList = iLogin.GetParentList(resp.Role_ID);

                                        MainItems = iLogin.GetMainItems(resp.Role_ID);

                                        MenuItems = iLogin.GetMenuItems(resp.Role_ID);

                                        Session["MainItems"] = MainItems;
                                        Session["MenuItems"] = MenuItems;
                                        if (RememberMe == "on")
                                        {
                                            HttpCookie cookie = new HttpCookie("Triyosoft");
                                            cookie.Values.Add("UserName", User.UserName);
                                            cookie.Values.Add("UserPassword", Password);
                                            cookie.Values.Add("Rememberme", "on");
                                            cookie.Expires = DateTime.Now.AddDays(7);
                                            Response.Cookies.Add(cookie);
                                        }
  
                                        else
                                        {
                                            var cookie = new HttpCookie("Triyosoft");
                                            cookie.Expires = DateTime.Now.AddDays(-1);
                                            Response.Cookies.Add(cookie);

                                        }


                                        //// REDIRECT ON THE BASES OF RIGHTS 
                                        //var permitted = CurrentUserSession.Permission.Where(x => x.ViewPermission == true).ToList();
                                        //var observable = new ObservableCollection<MenuModel>();
                                        //foreach (var item in permitted)
                                        //{
                                        //    observable.Add(item);
                                        //}

                                        //var MyTaskItem = permitted.Where(x => x.Action == "Approval" && x.Controller == "Home").FirstOrDefault();
                                        //if (MyTaskItem != null && MyTaskItem.MenuItemID > 0)
                                        //{
                                        //    int MyTaskItemIndex = permitted.IndexOf(MyTaskItem);
                                        //    observable.Move(MyTaskItemIndex, 0);
                                        //}
                                        //var CreateTaskItem = observable.Where(x => x.Action == "CreateAssignment" && x.Controller == "Home").FirstOrDefault();
                                        //if (CreateTaskItem != null && CreateTaskItem.MenuItemID > 0)
                                        //{
                                        //    int CreateTaskItemIndex = observable.IndexOf(CreateTaskItem);
                                        //    observable.Move(CreateTaskItemIndex, (observable.Count - 1));
                                        //}
                                        //var OrderedPermittedMenus = observable.ToList();
                                        //if (OrderedPermittedMenus != null && OrderedPermittedMenus.Count > 0)
                                        //{
                                        //    return RedirectToAction(OrderedPermittedMenus.FirstOrDefault().Action, OrderedPermittedMenus.FirstOrDefault().Controller);
                                        //}
                                        //else
                                        //{
                                        //    return RedirectToAction("Approval", "Home");
                                        //}
                                        return RedirectToAction("Approval", "Home");
                                    }
                                    else
                                    {
                                        return RedirectToAction("Index", "Login", new { Msg = "InActiveRole" });
                                    }
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Login", new { Msg = "InActiveCompany" });
                                }
                            }
                            else
                            {
                                return RedirectToAction("Index", "Login", new { Msg = "InActiveUser" });
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index", "Login", new { Msg = "Wrong" });
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login", new { Msg = "NotExist" });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Login", new { Msg = "ADNotFound", Ex = strMessage });
                }
            }
            catch (Exception ex)
            {
                iLogin.SendErrorToText(ex, "Login");
                return View();
            }
        }

        [HttpGet]
        [ValidateLogin]
        public ActionResult UnAuthorised()
        {
            return View();
        }

        public static string IsAuthenticated(string domain, string usr, string pwd, string userName, string userPass)
        {
            try
            {
                DirectoryEntry oDE = new DirectoryEntry("LDAP://" + domain, usr, pwd, System.DirectoryServices.AuthenticationTypes.Secure);

                DirectorySearcher search = new DirectorySearcher(oDE);
                search.Filter = "(samaccountname=" + userName + ")";
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    using (var context = new PrincipalContext(ContextType.Domain, domain, domain + "\\" + usr, pwd))
                    {
                        //Username and password for authentication.
                        if (context.ValidateCredentials(userName, userPass))
                            return "1";
                        else
                            return "Invalid username or password.";
                    }
                }
                else
                {
                    return "User does not exist in active directory";
                }
            }
            catch (DirectoryServicesCOMException cex)
            {
                return cex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}