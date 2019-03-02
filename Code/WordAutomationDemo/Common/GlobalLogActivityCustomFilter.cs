using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Classes;
using WordAutomationDemo.Repository.Interfaces;

namespace WordAutomationDemo.Common
{
    /// <summary>
    /// Created By : Dalvinder Singh
    /// Date : 18 Sep 2017
    /// </summary>
    /// <param name="filterContext"></param>
    public class GlobalLogActivityCustomFilter : ActionFilterAttribute
    {
        ILogActivity iLogActivity = new LogActivityRepository(new ReadyPortalDBEntities());

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            int editID = 0;
            if (filterContext.HttpContext.Request.QueryString["UID"] != null)
            {
                string UID = Global.UrlDecrypt(filterContext.HttpContext.Request.QueryString["UID"]);

                if (int.TryParse(UID, out editID)) { }

            }
            else if (filterContext.HttpContext.Request.QueryString["ProjectID"] != null) //From Home
            {
                string UID = filterContext.HttpContext.Request.QueryString["UID"];

                if (int.TryParse(UID, out editID)) { }
            }
            else if (filterContext.HttpContext.Request.QueryString["id"] != null)
            {
                string UID = Global.UrlDecrypt(filterContext.HttpContext.Request.QueryString["id"]);

                if (int.TryParse(UID, out editID)) { }
            }

            var activityDetails = string.Concat(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, "/", filterContext.ActionDescriptor.ActionName, " - ", filterContext.HttpContext.Request.RequestType);

            iLogActivity.AddLogActivity((byte)Global.LogActivityTypes.ActionVisited, activityDetails, editID > 0 ? new int[] { editID } : null);

            base.OnActionExecuting(filterContext);
        }
    }
}