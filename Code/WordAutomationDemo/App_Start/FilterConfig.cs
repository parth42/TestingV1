using System.Web;
using System.Web.Mvc;
using WordAutomationDemo.Common;
using WordAutomationDemo.Models;
using WordAutomationDemo.Repository.Classes;

namespace WordAutomationDemo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
          //  filters.Add(new GlobalLogActivityCustomFilter());
            filters.Add(new OutputCacheAttribute
            {
                VaryByParam = "none",
                Duration = 1,
                NoStore = true
            });
        }
    }
}
