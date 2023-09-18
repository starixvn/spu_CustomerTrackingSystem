namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Model.RobotReport;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.RobotReport;
    using System.Collections.Generic;
    using System;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.RobotReport;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class RobotReportService : IRobotReportService
    {
        private readonly IDynamicRepository _repository;

        public RobotReportService()
        {
            _repository = new DynamicRepository();
        }

        public AIRobotReportViewModel GetAIRobotReport(RobotReportParamModel param)
        {
            //var fromMonth = new DateTime(param.FromDate.Year, param.FromDate.Month, 1);
            //var toMonth = new DateTime(param.ToDate.Year, param.ToDate.Month, 1);
            //toMonth = toMonth.AddMonths(1).AddDays(-1);

            //var result = new AIRobotReportViewModel();

            //result.Items = new List<AIRobotReportModel>();

            //for (int i = 0; i < 6; i++)
            //{
            //    result.Items.Add(
            //        new AIRobotReportModel
            //        {
            //            SCE = i,
            //            TD = i,
            //            TI = i,
            //            ORCD = i,
            //            TotalAccount = i,
            //            DateRange = DateTime.Now.ToString()
            //        });
            //}

            //result.TFoot = new List<AIRobotReportTotalAccountModel> {
            //    new AIRobotReportTotalAccountModel { AIType = "SCE", TotalAccount = 30}
            //};

            var criteria = new GetAIRobotReportCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                IsDaily = param.IsDaily,
                IsMonthly = param.IsMonthly,
                IsLicensee = param.Sites == MMSites.All ? null : (bool?)(param.Sites == MMSites.Licensee)
            };

            var result = _repository.FetchMultiple<AIRobotReportModel, AIRobotReportTotalAccountModel>(criteria);

            return new AIRobotReportViewModel
            {
                Items = result.firstResultSet,
                TFoot = result.secondResultSet,
            };
        }

        public RobotReportViewModel GetRobotReport(RobotReportParamModel param)
        {
            //var result = new RobotReportViewModel();

            //result.Items = new List<RobotReportModel>();

            //for (int i = 0; i < 6; i++)
            //{
            //    result.Items.Add(
            //        new RobotReportModel
            //        {
            //            AI = i,
            //            COUNTER = i,
            //            IMPERVA = i,
            //            TotalAccount = i,
            //            DateRange = DateTime.Now.ToString()
            //        });
            //}

            //result.TFoot = new List<RobotReportTotalAccountModel> {
            //    new RobotReportTotalAccountModel { RobotType = "AI", TotalAccount = 30}
            //};

            var criteria = new GetRobotReportCriteria
            {
                CustCounterJs = "[]",
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                IsDaily = param.IsDaily,
                IsMonthly = param.IsMonthly,
                IsLicensee = param.Sites == MMSites.All ? null : (bool?)(param.Sites == MMSites.Licensee)
            };

            var result = _repository.FetchMultiple<RobotReportModel, RobotReportTotalAccountModel>(criteria);

            return new RobotReportViewModel
            {
                Items = result.firstResultSet,
                TFoot = result.secondResultSet,
            };
        }
    }
}
