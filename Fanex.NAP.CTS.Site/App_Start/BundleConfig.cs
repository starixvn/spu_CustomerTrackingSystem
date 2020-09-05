namespace Fanex.NAP.CTS.Site
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/JQuery/jquery-{version}.js",
                        "~/Scripts/JQuery/jquery-ui-1.12.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/JQuery/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/Bootstrap/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/styles").Include(
                      "~/Content/scss/vendors/select2.min.css",
                      "~/Content/scss/vendors/jquery-ui/base/jquery-ui.min.css",
                      "~/Content/scss/vendors/DataTables/css/jquery.dataTables.min.css",
                      "~/Content/scss/vendors/toast/jquery.toast.css",
                      "~/Content/scss/vendors/multiple-select.css",
                      "~/Content/scss/base/reset.css",
                      "~/Content/scss/pages/index.css"));

            bundles.Add(new StyleBundle("~/Content/styles/dashboard").Include(
                      "~/Content/scss/pages/dashboard/dashboard.min.css"));
             
            RegisterFSCoreBundles(bundles);
            RegisterSearchBoxBundles(bundles);

            //RegisterMatchMonitorBundles(bundles);
            //RegisterFraudCaseBundles(bundles);
            //RegisterSuspiciousCustomerBundles(bundles);
            //RegisterSuspiciousCustomerDetailsBundles(bundles);
            //RegisterScannedOfficeIpBundles(bundles);

            RegisterNotificationBundles(bundles);

            //RegisterDailyDataCaptureBundles(bundles);
            //RegisterCheckBetReportBundles(bundles);
            //RegisterCustomerMonitorBundles(bundles);
            //RegisterSubscriberAssignment(bundles);
            RegisterSearchBundles(bundles);

            RegisterBettingStatistic(bundles);

            RegisterAccountDetailBundles(bundles);
            RegisterChainToEvidenceBundles(bundles);

            RegisterTransactionHistoryBundles(bundles);

            RegisterCustomerCategoryBundles(bundles);

            RegisterProblemAccountMonitorBundles(bundles);

            RegisterProblemAccountMonitorDetailBundles(bundles);

            RegisterImportEvidencesBundles(bundles);

            RegisterAssociatedAccountMonitorBundles(bundles);

            RegisterSpecialCustomerClassBundles(bundles);

        }

        private static void RegisterFSCoreBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/core").Include(
                "~/Scripts/Moment/moment.min.js",
                "~/Scripts/select2.min.js",
                "~/Scripts/common.js",
                "~/Scripts/Fanex.Grid/Fanex.Grid.js",
                "~/Scripts/Fanex.Grid/Fanex.Pagination.js",
                "~/Scripts/DataTables_New/datatables.min.js",
                "~/Scripts/toast/jquery.toast.js",
                "~/Scripts/numeral/numeral.min.js",
                //"~/scripts/multiple-select.js",
                "~/scripts/MultipleSelect/jquery.multiselect.js",
                "~/Scripts/search.js",
                "~/Scripts/toogle.js"));

            bundles.Add(new ScriptBundle("~/finalize").Include("~/scripts/finalize.js"));
        }

        private static void RegisterSearchBoxBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/searchbox").Include("~/scripts/Home/SearchPage.js"));
        }

        private static void RegisterMatchMonitorBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/match-monitor").Include("~/scripts/MatchMonitor/*.js"));

            bundles.Add(new ScriptBundle("~/scripts/match-monitor/fraud-case-details").Include("~/scripts/matchmonitor/fraudcasedetails/*.js"));

            bundles.Add(new StyleBundle("~/Contents/CoreBetList").Include("~/Content/CoreBetList.css"));
        }

        private static void RegisterFraudCaseBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/fraud-case-styles").Include("~/content/scss/pages/configurations/fraud-case.css"));

            bundles.Add(new ScriptBundle("~/fraud-case-scripts").Include("~/scripts/fraudcase/fraud-case.js"));

            bundles.Add(new ScriptBundle("~/fraud-case-scripts/delayed-goal").Include("~/scripts/fraudcase/delayed-goal.js"));

            bundles.Add(new ScriptBundle("~/fraud-case-scripts/scanned-office-ip").Include("~/scripts/fraudcase/scanned-office-ip.js"));

            bundles.Add(new ScriptBundle("~/fraud-case-scripts/scanned-office-account").Include("~/scripts/fraudcase/scanned-office-account.js"));

            bundles.Add(new ScriptBundle("~/fraud-case-scripts/daily-data-capture").Include("~/scripts/fraudcase/daily-data-capture.js"));
        }

        private static void RegisterSuspiciousCustomerBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/suspicious-customer-styles").Include("~/content/scss/pages/suspicious-customer/suspicious-customer.css"));

            bundles.Add(new StyleBundle("~/scripts/suspicious-customer").Include("~/scripts/suspiciouscustomer/suspicious-customer.js"));
        }

        private static void RegisterSuspiciousCustomerDetailsBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/suspicious-customer-scripts").Include(
                "~/scripts/suspiciouscustomerdetails/*.js"));
        }

        private static void RegisterScannedOfficeIpBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/ScannedOfficeIp-scripts/main").Include("~/scripts/ScannedOfficeIp/scanned-office-ip.js"));
        }

        private static void RegisterDailyDataCaptureBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/daily-data-capture-scripts").Include("~/scripts/dailydatacapture/daily-data-capture.js"));
        }

        private static void RegisterNotificationBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/notification").Include(
                "~/scripts/Notification/*.js"));
        }

        private static void RegisterCheckBetReportBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/check-bet-report-scripts").Include("~/scripts/CheckBetReport/check-bet-report.js"));
        }

        private static void RegisterCustomerMonitorBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/customer-monitor-styles").Include("~/content/scss/pages/customer-monitor/customer-monitor.css"));

            bundles.Add(new ScriptBundle("~/scripts/customer-monitor").Include("~/scripts/customermonitor/*.js"));

            bundles.Add(new ScriptBundle("~/scripts/customer-monitor/customer-details").Include("~/scripts/customermonitor/customerdetails/*.js"));
        }

        private static void RegisterSubscriberAssignment(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/subscriber-assignment").Include("~/Scripts/SubscriberAssignment/*.js"));
        }

        private static void RegisterSearchBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/search-account-scripts").Include("~/Scripts/Search/Account/*.js"));
        }

        private static void RegisterBettingStatistic(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/statistic-scripts")
                .Include("~/Scripts/highcharts/highcharts.js",
                "~/Scripts/highcharts/full-screen.js",
                "~/Scripts/Account/BettingStatistic/sport-statistic-scripts.js",
                "~/Scripts/Account/BettingStatistic/turnover-by-bettype.js",
                "~/Scripts/Account/BettingStatistic/winlost-statistic-scripts.js"
                ));

            //bundles.Add(new StyleBundle("~/betting-statistic-styles").Include(
            //          "~/Content/scss/vendors/highcharts/highcharts.css"));
        }

        private static void RegisterAccountDetailBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/account-detail-scripts")
                .Include("~/Scripts/MultipleSelect/jquery.multiselect.filter.js",
                        "~/Scripts/popoto/d3.v3.min.js",
                        "~/Scripts/tagify/jquery.tagify.min.js",
                        "~/Scripts/tagify/tagify.min.js",
                        "~/scripts/Account/Associated/account-by-account.js",
                        "~/scripts/Account/Associated/graph.js",
                        "~/scripts/Account/index.js",
                        "~/scripts/Account/ChainToEvidence/account-evidence.js",
                        "~/scripts/Account/ChainToEvidence/account-exceptions.js"));

            bundles.Add(new StyleBundle("~/account-detail-styles").Include(
                      "~/Content/scss/MultipleSelect/jquery.multiselect.filter.css",
                      "~/Content/scss/MultipleSelect/jquery.multiselect.css",
                      "~/Content/scss/vendors/jquery-ui/base/tagify.css",
                      "~/Content/scss/pages/account/account.css"));
        }

        private static void RegisterEvidenceBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/evidence-scripts").Include("~/Scripts/Evidence/*.js"));
        }

        private static void RegisterTransactionHistoryBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/account/transaction-history-scripts").Include("~/Scripts/Account/TransactionHistory/*.js"));
        }

        private static void RegisterChainToEvidenceBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/chain-to-evidence-scripts")
                .Include("~/scripts/Account/ChainToEvidence/account-evidence.js",
                "~/scripts/Account/ChainToEvidence/account-exceptions.js"
                ));
        }

        private static void RegisterCustomerCategoryBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/account/customer-category-scripts").Include("~/Scripts/Account/CustomerCategory/*.js"));
        }

        private static void RegisterProblemAccountMonitorBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/problem-account-monitor-scripts").Include(
                "~/Scripts/select2.min.js",
                "~/Scripts/MultipleSelect/jquery.multiselect.filter.js",
                "~/Scripts/ProblemAccountMonitor/index.js"));

            bundles.Add(new StyleBundle("~/problem-account-monitor-styles").Include(
                "~/Content/scss/vendors/select2.min.css",
                 "~/Content/scss/MultipleSelect/jquery.multiselect.filter.css",
                 "~/Content/scss/MultipleSelect/jquery.multiselect.css",
                "~/Content/scss/pages/problem-account-monitor/problem-account-monitor.css"));
        }

        private static void RegisterProblemAccountMonitorDetailBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/problem-account-monitor-detail-scripts").Include(                
                "~/Scripts/MultipleSelect/jquery.multiselect.filter.js",
                "~/Scripts/ProblemAccountMonitor/detail.js"));

            bundles.Add(new StyleBundle("~/problem-account-monitor-detail-styles").Include(
                    "~/Content/scss/MultipleSelect/jquery.multiselect.filter.css",
                    "~/Content/scss/MultipleSelect/jquery.multiselect.css",
                    "~/Content/scss/pages/problem-account-monitor/problem-account-monitor.css"));
        }

        private static void RegisterImportEvidencesBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/import-evidences-scripts").Include(
               "~/Scripts/ImportEvidences/index.js"));

            bundles.Add(new StyleBundle("~/import-evidences-styles").Include(
                    "~/Content/scss/pages/import-evidences/import-evidences.css"));
        }

        private static void RegisterAssociatedAccountMonitorBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/associated-account-monitor-scripts").Include(
                "~/Scripts/select2.min.js",
                "~/Scripts/MultipleSelect/jquery.multiselect.filter.js",
                "~/Scripts/tagify/jquery.tagify.min.js",
                "~/Scripts/tagify/tagify.min.js",
                "~/Scripts/AssociatedAccountMonitor/index.js"));

            bundles.Add(new StyleBundle("~/associated-account-monitor-styles").Include(
                 "~/Content/scss/vendors/select2.min.css",
                 "~/Content/scss/MultipleSelect/jquery.multiselect.filter.css",
                 "~/Content/scss/MultipleSelect/jquery.multiselect.css",
                 "~/Content/scss/vendors/jquery-ui/base/tagify.css",
                 "~/Content/scss/pages/associated-account-monitor/associated-account-monitor.css"));
        }

        private static void RegisterSpecialCustomerClassBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/special-customer-class-scripts").Include(                        
                "~/Scripts/SpecialCustomerClass/index.js"));          
        }
    }
}