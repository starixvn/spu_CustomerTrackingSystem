namespace Fanex.NAP.CTS.Service.Implement.RobotReport
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Fanex.NAP.CTS.Criteria.RobotReport;
    using Fanex.NAP.CTS.Model.RobotReport;
    using Fanex.NAP.CTS.ViewModel.RobotReport;
    using Fanex.NAP.CTS.Service.Interface.RobotReport;
    using Fanex.NAP.CTS.Core.Service;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;
    using Fanex.NAP.CTS.Model.RobotReport.Entities;

    public class RobotReportService : IRobotReportService
    {
        private readonly ICriteriaService _repository;
        private readonly IExportRobotReportService _exportExcelService;

        public RobotReportService(ICriteriaService repository = null)
        {
            _repository = repository ?? new CriteriaService();
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

            var result = _repository.Fetch<AIRobotReportModel, AIRobotReportModel>(criteria);

            return new AIRobotReportViewModel
            {
                Items = result.firstResultSet?.Any() ?? false ? MergeAIRobotReportListWithCalendar(result.firstResultSet, param) : result.firstResultSet,
                TFoot = result.secondResultSet?.Any() ?? false ? result.secondResultSet.ElementAt(0) : new AIRobotReportModel()
            };
        }

        public RobotReportViewModel GetRobotReport(RobotReportParamModel param)
        {
            var custCounter = GetRobotReportCounter(param) ?? new List<RobotReportCounterModel>();

            var criteria = new GetRobotReportCriteria
            {
                CustCounterJs = JsonConvert.SerializeObject(custCounter),
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                IsDaily = param.IsDaily,
                IsMonthly = param.IsMonthly,
                IsLicensee = param.Sites == MMSites.All ? null : (bool?)(param.Sites == MMSites.Licensee)
            };

            var result = _repository.Fetch<RobotReportModel, RobotReportModel>(criteria);

            return new RobotReportViewModel
            {
                Items = result.firstResultSet?.Any() ?? false ? MergeRobotReportListWithCalendar(result.firstResultSet, param) : result.firstResultSet,
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
        private IEnumerable<RobotReportCounterModel> GetRobotReportCounter(RobotReportParamModel param)
        {
            var criteria = new GetRobotReportCounterCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                IsLicensee = param.Sites == MMSites.All ? 2 : (param.Sites == MMSites.Licensee ? 1 : 0)
            };

            var result = _repository.Fetch<RobotReportCounterEntity>(criteria);

            return result?.Select(x => new RobotReportCounterModel(x)) ?? Enumerable.Empty<RobotReportCounterModel>();
        }

        private IEnumerable<RobotReportModel> MergeRobotReportListWithCalendar(IEnumerable<RobotReportModel> data, RobotReportParamModel param)
        {
            var items = default(IEnumerable<RobotReportModel>);

            if (param.IsMonthly)
            {
                items = from months in EachMonth(param.FromDate, param.ToDate)
                        join rs in data on months.ToString(FormatDateTimeToString_yyyyMM) equals rs.GetMonthlyTime into rs1
                        from p in rs1.DefaultIfEmpty()
                        select p ?? new RobotReportModel { DateRange = months };
            }
            else
            {
                items = from months in EachDay(param.FromDate, param.ToDate)
                        join rs in data on months.ToString(FormatDateTimeToString_yyyyMMdd) equals rs.GetDailyTime into rs1
                        from p in rs1.DefaultIfEmpty()
                        select p ?? new RobotReportModel { DateRange = months };
            }

            return items;
        }

        private IEnumerable<AIRobotReportModel> MergeAIRobotReportListWithCalendar(IEnumerable<AIRobotReportModel> data, RobotReportParamModel param)
        {
            var items = default(IEnumerable<AIRobotReportModel>);

            if (param.IsMonthly)
            {
                items = from months in EachMonth(param.FromDate, param.ToDate)
                        join rs in data on months.ToString(FormatDateTimeToString_yyyyMM) equals rs.GetMonthlyTime into rs1
                        from p in rs1.DefaultIfEmpty()
                        select p ?? new AIRobotReportModel { DateRange = months };
            }
            else
            {
                items = from months in EachDay(param.FromDate, param.ToDate)
                        join rs in data on months.ToString(FormatDateTimeToString_yyyyMMdd) equals rs.GetDailyTime into rs1
                        from p in rs1.DefaultIfEmpty()
                        select p ?? new AIRobotReportModel { DateRange = months };
            }

            return items;
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
