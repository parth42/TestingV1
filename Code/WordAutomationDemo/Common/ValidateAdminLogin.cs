using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Helpers;

namespace WordAutomationDemo.Common
{
    public class ValidateLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CurrentUserSession.UserID < 0)
            {
                filterContext.HttpContext.Session.RemoveAll();
                filterContext.HttpContext.Session.Clear();
                filterContext.HttpContext.Session.Abandon();
                filterContext.Result = new RedirectResult("~/Login/Index?Msg=Unauthorize", true);
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }


    public class ValidateUserPermission : ActionFilterAttribute
    {
        Global.Actions act;
        Global.Controlers ctl;

        public ValidateUserPermission(Global.Actions Action, Global.Controlers controller)
        {
            act = Action;
            ctl = controller;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool IsAuthorize = true;
            if (act == Global.Actions.Index && !CurrentUserSession.HasViewPermission)
            {
                IsAuthorize = false;
            }

            if (act == Global.Actions.Create && !CurrentUserSession.HasAddPermission)
            {
                IsAuthorize = false;
            }

            if (act == Global.Actions.Edit && !CurrentUserSession.HasEditPermission)
            {
                IsAuthorize = false;
            }

            if (act == Global.Actions.Delete && !CurrentUserSession.HasDeletePermission)
            {
                IsAuthorize = false;
            }

            if (act == Global.Actions.Details && !CurrentUserSession.HasDetailPermission)
            {
                IsAuthorize = false;
            }


            if (!IsAuthorize)
            {
                filterContext.Result = new RedirectResult("~/Login/UnAuthorised");
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }


}