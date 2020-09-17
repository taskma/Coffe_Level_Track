using System.Web;
using System.Web.Optimization;

namespace CoffeLevel.Client.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Content/bootstrap/js/bootstrap.js")
                .Include("~/Content/bootstrap/js/bootstrap-datepicker.js")
                .Include("~/Scripts/ckeditor/ckeditor.js")
                .Include("~/Scripts/ckeditor/styles.js")
                .Include("~/Scripts/ckeditor/config.js")
                .Include("~/Scripts/ckeditor/lang/tr.js"));


            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/bootstrap-validation.js"));

            bundles.Add(new ScriptBundle("~/bundles/jstools").Include(
                        "~/Scripts/date.js"));

            bundles.Add(new ScriptBundle("~/bundles/googleapis").Include(
                       "~/Content/general/js/GoogleApi.js"));

            bundles.Add(new ScriptBundle("~/bundles/circleful").Include(
              "~/Content/general/js/circleful.js"));

 
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/bootstrap/css/bootstrap.css")
                .Include("~/Content/bootstrap/css/bootstrap-theme.css")
                .Include("~/Content/bootstrap/css/datepicker.css")
                .Include("~/Content/general/css/jquery.circliful.css")
                .Include("~/Content/general/css/general.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}