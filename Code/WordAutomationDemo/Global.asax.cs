using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WordAutomationDemo.Common;
using WordAutomationDemo.Controllers;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Classes;

namespace WordAutomationDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteTable.Routes.MapHubs();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            UnityConfig.RegisterComponents();
        }

        protected void Application_AcquireRequestState(object sender, EventArgs args)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null && CurrentUserSession.User == null && Global.IsADEnabled)
            {
                string username = Request.ServerVariables["logon_user"];
                if (!string.IsNullOrEmpty(username))
                {
                    username = username.Substring(username.IndexOf(@"\") + 1);
                    var db = new ReadyPortalDBEntities();
                    var login = new LoginRepository(db);
                    var resp = login.CheckLogin(username);
                    if (resp == null)
                        return;
                    var priv = new RolePrivilegesRepository(db);
                    CurrentUser User = new CurrentUser();
                    User.UserName = resp.UserName;
                    User.FirstName = resp.FullName;
                    User.strUserID = resp.UserId.ToString();
                    User.UserID = resp.UserId;
                    User.RoleID = resp.Role_ID;
                    User.CompanyID = resp.Company_ID;
                    User.CompanyLogo = resp.CompanyLogo;
                    User.IsSuperAdmin = resp.IsSuperAdmin;
                    User.ProfileImage = resp.ProfileImage;
                    User.DTformat = resp.DTformat;
                    User.DTFormatGrid = resp.DTFormatGrid;
                    User.DTFormatGrid = resp.DTFormatGrid;
                    User.HasUpdatedDocViewPermission = resp.IsSuperAdmin || resp.IsAdminUser ? true : priv.HasUpdatedDocViewPermission(resp.Role_ID);
                    User.IsAdminUser = resp.IsAdminUser;
                    User.IsMessengerServiceEnable = resp.IsMessengerServiceEnable;
                    User.EmailID = resp.EmailID;
                    CurrentUserSession.User = User;
                    Session["EPT"] = login.EmployeePhoto(User.FirstName);
                    IList<vwRolePrivilege> MenuItems = null;
                    IList<vwRolePrivilege> MainItems = null;

                    var ParentList = login.GetParentList(resp.Role_ID);

                    MainItems = login.GetMainItems(resp.Role_ID);

                    MenuItems = login.GetMenuItems(resp.Role_ID);

                    Session["MainItems"] = MainItems;
                    Session["MenuItems"] = MenuItems;
                    HttpCookie cookie = new HttpCookie("Triyosoft");
                    cookie.Values.Add("UserName", User.UserName);
                    cookie.Values.Add("UserPassword", "n/a");
                    cookie.Values.Add("Rememberme", "on");
                    cookie.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Add(cookie);
                    String[] roles = new string[] { "user" };

                    GenericPrincipal principal = new GenericPrincipal(HttpContext.Current.User.Identity, roles);

                    Thread.CurrentPrincipal = HttpContext.Current.User = principal;
                }
            }
        }
    }
}
