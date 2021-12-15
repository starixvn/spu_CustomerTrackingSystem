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

            RegisterCTSCoreBundles(bundles);
            RegisterSearchBoxBundles(bundles);           
            RegisterNotificationBundles(bundles);            

            RegisterMatchMonitorBundles(bundles);
            RegisterMatchMonitorDetailBundles(bundles);            

            RegisterBettingStatistic(bundles);

            RegisterAccountDetailBundles(bundles);

            RegisterChainToEvidenceBundles(bundles);

            RegisterTransactionHistoryBundles(bundles);

            RobotClassificationBundles(bundles);

            RobotBettingBundles(bundles);

            RegisterCustomerCategoryBundles(bundles);

            RegisterProblemAccountMonitorBundles(bundles);

            RegisterProblemAccountMonitorDetailBundles(bundles);

            RegisterImportEvidencesBundles(bundles);

            RegisterAssociatedAccountMonitorBundles(bundles);

            RegisterSpecialCustomerClassBundles(bundles);

            RegisterProbationManagementBundles(bundles);

            RegisterManagerSubscriberBundles(bundles);

            RegisterAssociationReport(bundles);

            RegisterAssociationDetailReport(bundles);

            RegisterLatestTransaction(bundles);

            RegisterProblemCaseMonitor(bundles);

            RegisterAssociationDetectionBundles(bundles);

            RegisterTransactionReport(bundles);
        }

        private static void RegisterCTSCoreBundles(BundleCollection bundles)
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
                "~/scripts/MultipleSelect/jquery.multiselect.js",
                "~/Scripts/search.js",
                "~/Scripts/toogle.js"));

            bundles.Add(new ScriptBundle("~/finalize").Include("~/scripts/finalize.js"));
        }

        private static void RegisterSearchBoxBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/searchbox").Include("~/scripts/Home/SearchPage.js"));
        }

        private static void RegisterNotificationBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/notification").Include("~/scripts/Notification/*.js"));

            bundles.Add(new StyleBundle("~/notification-setting-styles").Include("~/Content/scss/pages/notification-setting/notification-setting.css"));

            bundles.Add(new ScriptBundle("~/notification-setting-scripts").Include("~/Scripts/NotificationSetting/notification-setting.js"));
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

        private static void RegisterTransactionHistoryBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/account/transaction-history-scripts").Include("~/Scripts/Account/TransactionHistory/*.js"));
        }
        
        private static void RobotClassificationBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/account/robot-classification-scripts").Include("~/Scripts/Account/RobotClassification/*.js"));
        }

        private static void RobotBettingBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/account/robot-betting-scripts").Include("~/Scripts/Account/RobotBetting/*.js"));
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
               "~/Scripts/ImportEvidences/index.js",
                "~/Scripts/tagify/jquery.tagify.min.js",
                "~/Scripts/tagify/tagify.min.js"));

            bundles.Add(new StyleBundle("~/import-evidences-styles").Include(
                    "~/Content/scss/pages/import-evidences/import-evidences.css",
                    "~/Content/scss/vendors/jquery-ui/base/tagify.css"));
        }

        private static void RegisterAssociationDetectionBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/association-detection-scripts").Include(
               "~/Scripts/AssociationDetection/index.js",
                "~/Scripts/tagify/jquery.tagify.min.js",
                "~/Scripts/tagify/tagify.min.js"));

            bundles.Add(new StyleBundle("~/association-detection-styles").Include(
                    "~/Content/scss/pages/association-detection/association-detection.css",
                    "~/Content/scss/vendors/jquery-ui/base/tagify.css"));
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

            bundles.Add(new StyleBundle("~/special-customer-class-styles").Include(
                 "~/Content/scss/pages/special-customer-class/special-customer-class.css"));
        }

        private static void RegisterProbationManagementBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/probation-management-styles").Include(
                "~/Content/scss/vendors/select2.min.css",
                "~/Content/scss/MultipleSelect/jquery.multiselect.filter.css",
                "~/Content/scss/MultipleSelect/jquery.multiselect.css",
                "~/Content/scss/pages/probation-management/probation-management.css"));

            bundles.Add(new ScriptBundle("~/probation-management-scripts").Include(
                "~/Scripts/select2.min.js",
                "~/Scripts/MultipleSelect/jquery.multiselect.filter.js",
                "~/Scripts/ProbationManagement/index.js"));
        }

        private static void RegisterManagerSubscriberBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/manage-subscriber-scripts").Include(
               "~/Scripts/ManageSubscriber/index.js"));

            bundles.Add(new StyleBundle("~/manage-subscriber-styles").Include(
                    "~/Content/scss/pages/manage-subscriber/manage-subscriber.css"));
        }

        private static void RegisterAssociationReport(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/association-report-scripts").Include(
                "~/Scripts/select2.min.js",
                "~/Scripts/MultipleSelect/jquery.multiselect.filter.js",
                "~/Scripts/AssociationReport/index.js"));

            bundles.Add(new StyleBundle("~/association-report-styles").Include(
                "~/Content/scss/vendors/select2.min.css",
                 "~/Content/scss/MultipleSelect/jquery.multiselect.filter.css",
                "~/Content/scss/MultipleSelect/jquery.multiselect.css",
                "~/Content/scss/pages/association-report/association-report.css"));
        }

        private static void RegisterAssociationDetailReport(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/association-detail-report-scripts").Include(
              "~/Scripts/AssociationReport/detail.js"));

            bundles.Add(new StyleBundle("~/association-detail-report-styles").Include(
                    "~/Content/scss/pages/association-report/association-report.css"));
        }

        private static void RegisterLatestTransaction(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/latest-transaction-scripts").Include(
                "~/Scripts/select2.min.js",
              "~/Scripts/LatestTransaction/index.js"));

            bundles.Add(new StyleBundle("~/latest-transaction-styles").Include(
                "~/Content/scss/vendors/select2.min.css",
                    "~/Content/scss/pages/latest-transaction/latest-transaction.css"));
        }

        private static void RegisterMatchMonitorBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/match-monitor-scripts").Include(
               "~/Scripts/select2.min.js",
               "~/Scripts/MultipleSelect/jquery.multiselect.filter.js",
               "~/Scripts/MatchMonitor/index.js"));

            bundles.Add(new StyleBundle("~/match-monitor-styles").Include(
                "~/Content/scss/vendors/select2.min.css",
                 "~/Content/scss/MultipleSelect/jquery.multiselect.filter.css",
                "~/Content/scss/MultipleSelect/jquery.multiselect.css",
                "~/Content/scss/pages/match-monitor/match-monitor.css"));
        }

        private static void RegisterMatchMonitorDetailBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/match-monitor-detail-scripts").Include(
               "~/Scripts/select2.min.js",
               "~/Scripts/MultipleSelect/jquery.multiselect.filter.js",
               "~/Scripts/MatchMonitor/detail.js"));

            bundles.Add(new StyleBundle("~/match-monitor-detail-styles").Include(
                "~/Content/scss/vendors/select2.min.css",
                 "~/Content/scss/MultipleSelect/jquery.multiselect.filter.css",
                "~/Content/scss/MultipleSelect/jquery.multiselect.css",
                "~/Content/scss/pages/match-monitor/match-monitor-detail.css",
                "~/Content/scss/vendors/jquery-ui/base/tagify.css"));
        }
        
        private static void RegisterProblemCaseMonitor(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/problem-case-monitor-scripts").Include(
                  "~/Scripts/select2.min.js",
                  "~/Scripts/MultipleSelect/jquery.multiselect.filter.js",
                  "~/Scripts/ProblemCaseMonitor/index.js"));

            bundles.Add(new StyleBundle("~/problem-case-monitor-styles").Include(
                "~/Content/scss/vendors/select2.min.css",
                 "~/Content/scss/MultipleSelect/jquery.multiselect.filter.css",
                "~/Content/scss/MultipleSelect/jquery.multiselect.css",
                "~/Content/scss/pages/problem-case-monitor/problem-case-monitor.css"));
        }

        private static void RegisterTransactionReport(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/transaction-report-scripts").Include(
                "~/Scripts/select2.min.js",
                "~/Scripts/MultipleSelect/jquery.multiselect.filter.js",
                "~/Scripts/TransactionReport/index.js"));

            bundles.Add(new StyleBundle("~/transaction-report-styles").Include(
                "~/Content/scss/vendors/select2.min.css",
                 "~/Content/scss/MultipleSelect/jquery.multiselect.filter.css",
                "~/Content/scss/MultipleSelect/jquery.multiselect.css",
                "~/Content/scss/pages/transaction-report/transaction-report.css"));
        }
    }
}