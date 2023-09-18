namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.NAP.CTS.Model.RobotReport;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.RobotReport;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.RobotReport;
    using System.Collections.Generic;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;
    using Newtonsoft.Json;
    using System.Linq;

    public class RobotReportService : IRobotReportService
    {
        private readonly IDynamicRepository _repository;

        public RobotReportService()
        {
            _repository = new DynamicRepository();
        }

        public AIRobotReportViewModel GetAIRobotReport(RobotReportParamModel param)
        {
            var criteria = new GetAIRobotReportCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                IsDaily = param.IsDaily,
                IsMonthly = param.IsMonthly,
                IsLicensee = param.Sites == MMSites.All ? null : (bool?)(param.Sites == MMSites.Licensee)
            };

            var result = _repository.FetchMultiple<AIRobotReportModel, AIRobotReportModel>(criteria);

            return new AIRobotReportViewModel
            {
                Items = result.firstResultSet,
                TFoot = result.secondResultSet?.Any() ?? false ? result.secondResultSet.ElementAt(0) : new AIRobotReportModel()
            };
        }

        public RobotReportViewModel GetRobotReport(RobotReportParamModel param)
        {
            var custCounter = GetRobotReportCounter(param) ?? new List<RobotReportCounterView>();

            var criteria = new GetRobotReportCriteria
            {
                CustCounterJs = JsonConvert.SerializeObject(custCounter),
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                IsDaily = param.IsDaily,
                IsMonthly = param.IsMonthly,
                IsLicensee = param.Sites == MMSites.All ? null : (bool?)(param.Sites == MMSites.Licensee)
            };

            var result = _repository.FetchMultiple<RobotReportModel, RobotReportModel>(criteria);

            return new RobotReportViewModel
            {
                Items = result.firstResultSet,
                TFoot = result.secondResultSet?.Any() ?? false ? result.secondResultSet.ElementAt(0) : new RobotReportModel()
            };
        }

        private IEnumerable<RobotReportCounterView> GetRobotReportCounter(RobotReportParamModel param)
        {
            var criteria = new GetRobotReportCounterCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                IsLicensee = param.Sites == MMSites.All ? 2 : (param.Sites == MMSites.Licensee ? 1 : 0)
            };

            return _repository.Fetch<RobotReportCounterView>(criteria);
        }
    }
}
