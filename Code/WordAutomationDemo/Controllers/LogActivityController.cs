using Kendo.Mvc.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Interfaces;
using WordAutomationDemo.CustomGridBinding;

namespace WordAutomationDemo.Controllers
{
    [ValidateLogin]
    public class LogActivityController : BaseController
    {
        private static int Count = 0;
        ILogActivity iLogActivity;
        public LogActivityController(ILogActivity ILogActivity)
        {
            iLogActivity = ILogActivity;
        }


        // GET: LogActivity
        [HttpGet]
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.LogActivity)]
        public ActionResult Index()
        {
            var ViewModel = new LogActivityModel
            {
                ModuleName = new CommonMessagesModel { ModuleName = "LogActivity" },
            };
            return View(ViewModel);
        }

        public ActionResult LogActivity_Read([DataSourceRequest]DataSourceRequest request)
        {
            var result = new DataSourceResult()
            {
                Data = GetLogActivityGridData(request), // Process data
                Total = Count // Total number of records
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable GetLogActivityGridData([DataSourceRequest]DataSourceRequest command)
        {
            var result = iLogActivity.LogActivitylist();

            //Apply filtering
            result = result.ApplyFiltering(command.Filters);

            //Get count of total records
            Count = result.Count();

            //Apply sorting
            result = result.ApplySorting(command.Groups, command.Sorts);

            //Apply paging
            result = result.ApplyPaging(command.Page, command.PageSize);

            //Apply grouping
            if (command.Groups.Any())
            {
                return result.ApplyGrouping(command.Groups);
            }

            return result.ToList();
        }
    }
}