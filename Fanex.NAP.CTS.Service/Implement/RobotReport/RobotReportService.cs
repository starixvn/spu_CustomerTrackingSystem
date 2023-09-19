namespace Fanex.NAP.CTS.Service.Implement.RobotReport
{
    using Fanex.NAP.CTS.Model.RobotReport;
    using Fanex.NAP.CTS.ViewModel.RobotReport;
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.RobotReport;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using System.Linq;
    using Fanex.NAP.CTS.Service.Interface.RobotReport;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;
    using System;

    public class RobotReportService : IRobotReportService
    {
        private readonly IDynamicRepository _repository;
        private readonly IExportRobotReportService _exportExcelService;

        public RobotReportService()
        {
            _repository = new DynamicRepository();
            _exportExcelService = new ExportRobotReportService();
        }

        #region public method
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

            var items = default(IEnumerable<AIRobotReportModel>);

            if (param.IsMonthly)
            {
                items = from months in EachMonth(param.FromDate, param.ToDate)
                        join rs in result.firstResultSet on months.ToString("yyyy/MM") equals rs.GetMonthlyTime into rs1
                        from p in rs1.DefaultIfEmpty()
                        select p != null ? p : new AIRobotReportModel { DateRange = months };
            }
            else
            {
                items = from months in EachDay(param.FromDate, param.ToDate)
                        join rs in result.firstResultSet on months.ToString("yyyy/MM/dd") equals rs.GetDailyTime into rs1
                        from p in rs1.DefaultIfEmpty()
                        select p != null ? p : new AIRobotReportModel { DateRange = months };
            }

            return new AIRobotReportViewModel
            {
                Items = items,
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

            var items = default(IEnumerable<RobotReportModel>);

            if (param.IsMonthly)
            {
                items = from months in EachMonth(param.FromDate, param.ToDate)
                        join rs in result.firstResultSet on months.ToString("yyyy/MM") equals rs.GetMonthlyTime into rs1
                        from p in rs1.DefaultIfEmpty()
                        select p != null ? p : new RobotReportModel { DateRange = months };
            }
            else
            {
                items = from months in EachDay(param.FromDate, param.ToDate)
                        join rs in result.firstResultSet on months.ToString("yyyy/MM/dd") equals rs.GetDailyTime into rs1
                        from p in rs1.DefaultIfEmpty()
                        select p != null ? p : new RobotReportModel { DateRange = months };
            }
            return new RobotReportViewModel
            {
                Items = items,
                TFoot = result.secondResultSet?.Any() ?? false ? result.secondResultSet.ElementAt(0) : new RobotReportModel()
            };
        }

        public byte[] ExportRobotReport(string templatePath, string fileName, RobotReportParamModel param)
        {
            var data = GetRobotReport(param);
            return _exportExcelService.ExportRobotReport(templatePath, fileName, data, param);
        }

        public byte[] ExportAIRobotReport(string templatePath, string fileName, RobotReportParamModel param)
        {
            var data = GetAIRobotReport(param);
            return _exportExcelService.ExportAIRobotReport(templatePath, fileName, data, param);
        }
        #endregion

        #region private method
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

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = thru.Date; day.Date >= from.Date; day = day.AddDays(-1))
                yield return day;
        }

        private IEnumerable<DateTime> EachMonth(DateTime from, DateTime thru)
        {
            for (var day = thru.Date; day.Date >= from.Date; day = day.AddMonths(-1))
                yield return day;
        }
        #endregion
    }
}
