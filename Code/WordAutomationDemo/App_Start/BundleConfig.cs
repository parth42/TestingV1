using System.Web;
using System.Web.Optimization;

namespace WordAutomationDemo
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));


            bundles.Add(new StyleBundle("~/Bundles/CSS/Common").Include("~/CSS/Common/bootstrap.min.css",
                                                    "~/CSS/Common/bootstrap.css",
                                                    "~/CSS/Common/bootstrap-multiselect.css",
                                                    "~/CSS/Common/bootstrap-datetimepicker.css",
                                                    "~/CSS/Common/lightgallery.css",
                                                    "~/CSS/Common/jquery.smartmenus.bootstrap.css",
                                                    "~/CSS/Common/bootstrap-theme.css",
                                                    "~/CSS/Common/fonts.css",
                                                    //"~/CSS/Common/fonts/fonts.css",
                                                    "~/CSS/Common/font-awesome.css",
                                                    "~/CSS/Common/jquery-ui.min.css",
                                                    "~/CSS/Common/jquery-ui.theme.css",
                                                    "~/CSS/Common/datepicker.css",
                                                    "~/CSS/Common/bootstrap-datepicker.min.css",
                                                    "~/CSS/Common/kendo.common.min.css",
                                                    "~/CSS/Common/kendo.uniform.min.css",
                                                    "~/CSS/Common/kendo.default.min.css",
                                                    "~/CSS/Common/kendo.common.new.min.css",
                                                    "~/CSS/Common/cropper.css",
                                                    "~/CSS/Common/select2.css"));

            bundles.Add(new StyleBundle("~/Bundles/CSS/Custom").Include("~/CSS/Custom/Site.css",
                                                                    "~/CSS/Custom/sm-core-css.css",
                                                                    "~/CSS/Custom/sm-blue.css",
                                                                    "~/CSS/Custom/style.css",
                "~/CSS/Custom/Custom.css"));

            bundles.Add(new ScriptBundle("~/Bundles/Scripts/Common").Include("~/Scripts/Common/jquery-1.10.2.min.js",
                                                        "~/Scripts/Common/twitter-bootstrap-hover-dropdown.min.js",
                                                        "~/Scripts/Common/jquery.hoverIntent.minified.js",
                                                        "~/Scripts/Common/jquery.validate.min.js",
                                                        "~/Scripts/Common/jquery.validate.unobtrusive.min.js",
                                                        "~/Scripts/Common/modernizr-*",
                                                        "~/Scripts/Common/moment.min.js",
                                                        "~/Scripts/Common/bootstrap.min.js",
                                                        "~/Scripts/Common/html5shiv.js",
                                                        "~/Scripts/Common/jquery.smartmenus.js",
                                                        "~/Scripts/Common/jquery.smartmenus.bootstrap.min",
                                                        "~/Scripts/Common/respond.min.js",
                                                        "~/Scripts/Common/jquery-ui.min.js",
                                                        "~/Scripts/Common/jquery.multiselect.filter.min.js",
                                                        "~/Scripts/Common/jquery.multiselect.min.js",
                                                        "~/Scripts/Common/bootstrap-multiselect.js",
                                                        "~/Scripts/Common/bootstrap-datetimepicker.js",
                                                        "~/Scripts/Common/bootstrap-datepicker.js",
                                                        "~/Scripts/Custom/common.js",
                                                        "~/Scripts/Custom/CommonMessages.js",
                                                        "~/Scripts/Common/lightgallery-all.js",
                                                        "~/Scripts/Common/jquery.mousewheel.min.js",
                                                        "~/Scripts/Common/cropper.js",
                                                        "~/Scripts/Common/select2.min.js",
                                                        "~/Scripts/jquery.form.js",
                                                        "~/Scripts/Common/kendo.all.min.js",
                                                        "~/Scripts/Common/kendo.aspnetmvc.min.js",
                                                        "~/Scripts/Custom/passwordStrengthMeter.js",
                                                         "~/Scripts/Custom/SiteUrl.js"
                                                        ));

            bundles.Add(new ScriptBundle("~/bundles/SignalR").Include(
                      "~/Scripts/Common/jquery.signalR-1.1.2.min.js",
                     "~/Scripts/Common/ChatJs/Scripts/jquery.chatjs.signalradapter.js",
                       "~/Scripts/Common/ChatJs/Scripts/jquery.autosize.min.js",
                      "~/Scripts/Common/ChatJs/Scripts/jquery.activity-indicator-1.0.0.min.js",
                       "~/Scripts/Common/ChatJs/Scripts/jquery.chatjs.js"));
        }
    }
}
