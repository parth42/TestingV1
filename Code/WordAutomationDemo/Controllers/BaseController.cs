using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Helpers;
using WordAutomationDemo.Models;

namespace WordAutomationDemo.Controllers
{
    public class BaseController : AsyncController
    {
        public ReadyPortalDBEntities _entities = new ReadyPortalDBEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ModulePrivileges(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName);
            base.OnActionExecuting(filterContext);
        }

        private void ModulePrivileges(string Controller)
        {
            vwRolePrivilege RolePrivileges = new vwRolePrivilege();

            if (CurrentUserSession.User != null)
            {
                RolePrivileges = _entities.vwRolePrivileges.Where(p => p.RoleID == CurrentUserSession.User.RoleID && p.MenuItemController.Trim() == Controller && p.IsActive == 1).FirstOrDefault();

                CurrentPermission Permission = new CurrentPermission();
                Permission.HasViewPermission = RolePrivileges.ViewPermission.Value;
                Permission.HasAddPermission = RolePrivileges.Add.Value;
                Permission.HasEditPermission = RolePrivileges.Edit.Value;
                Permission.HasDeletePermission = RolePrivileges.Delete.Value;
                Permission.HasDetailPermission = RolePrivileges.Detail.Value;
                CurrentUserSession.Permission = Permission;
            }
            else
            {
                CurrentPermission Permission = new CurrentPermission();
                Permission.HasViewPermission = false;
                Permission.HasAddPermission = false;
                Permission.HasEditPermission = false;
                Permission.HasDeletePermission = false;
                Permission.HasDetailPermission = false;
                CurrentUserSession.Permission = Permission;
            }
        }
    }
}