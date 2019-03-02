using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;

namespace WordAutomationDemo.Controllers
{
    [ValidateLogin]
    public class TestingController : Controller
    {
        // GET: Testing
        [ValidateUserPermission(Global.Actions.Index, Global.Controlers.User)]
        public ActionResult Index()
        {
            return View();
        }
    }
}