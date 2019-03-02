using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Helpers
{

    public class CurrentUserSession
    {
        public static CurrentUser User
        {
            get
            {
                CurrentUser User = (CurrentUser)HttpContext.Current.Session["CCurrentUser"];
                return User;
            }
            set
            {
                HttpContext.Current.Session["CCurrentUser"] = value;
            }
        }

        public static CurrentPermission Permission
        {
            get
            {
                CurrentPermission Permission = (CurrentPermission)HttpContext.Current.Session["CurrentPermission"];
                return Permission;
            }
            set
            {
                HttpContext.Current.Session["CurrentPermission"] = value;
            }
        }

        //public static List<MenuModel> Permission
        //{
        //    get
        //    {
        //        List<MenuModel> Permission = (List<MenuModel>)HttpContext.Current.Session["CurrentUserPermission"];
        //        return Permission;
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["CurrentUserPermission"] = value;
        //    }
        //}

        public static int UserID
        {
            get
            {
                if (User != null)
                    return User.UserID;
                else
                    return -1;
            }
        }

        public static int RoleID
        {
            get
            {
                if (User != null)
                    return User.RoleID;
                else
                    return -1;
            }
        }

        public static string FirstName
        {
            get
            {
                if (User != null)
                    return User.FirstName;
                else
                    return string.Empty;
            }
        }

        public static string UserName
        {
            get
            {
                if (User != null)
                    return User.UserName;
                else
                    return string.Empty;
            }
        }

        public static bool HasViewPermission
        {
            get
            {
                if (User != null)
                    return Permission.HasViewPermission;
                else
                    return false;
            }
        }

        public static bool HasAddPermission
        {
            get
            {
                if (User != null)
                    return Permission.HasAddPermission;
                else
                    return false;
            }
        }

        public static bool HasEditPermission
        {
            get
            {
                if (User != null)
                    return Permission.HasEditPermission;
                else
                    return false;
            }
        }

        public static bool HasDeletePermission
        {
            get
            {
                if (User != null)
                    return Permission.HasDeletePermission;
                else
                    return false;
            }
        }

        public static bool HasDetailPermission
        {
            get
            {
                if (User != null)
                    return Permission.HasDetailPermission;
                else
                    return false;
            }
        }

        public static void ClearCurrentSession()
        {
            HttpContext.Current.Session.Remove("CCurrentUser");
            HttpContext.Current.Session.Remove("CurrentUserPermission");
        }
    }



    public class CurrentUser
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public string strUserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public int CompanyID { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string ProfileImage { get; set; }
        public string CompanyLogo { get; set; }
        public bool HasUpdatedDocViewPermission { get; set; }
        public string DTformat { get; set; }
        public string DTFormatGrid { get; set; }
        public bool IsAdminUser { get; set; }
        public bool IsMessengerServiceEnable { get; set; }
        public string EmailID { get; set; }
        public bool CanEdit { get; set; }
        public bool CanApprove { get; set; }

    }

    public class CurrentPermission
    {
        public bool HasViewPermission { get; set; }
        public bool HasAddPermission { get; set; }
        public bool HasEditPermission { get; set; }
        public bool HasDeletePermission { get; set; }
        public bool HasDetailPermission { get; set; }
    }


    //public class CurrentUserPermission
    //{
    //    public bool HasViewPermission { get; set; }
    //}




}