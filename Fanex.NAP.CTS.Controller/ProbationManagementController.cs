namespace Fanex.NAP.CTS.Controller
{
    using System.Web.Mvc;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Implement;
    using System.Collections.Generic;
    using Fanex.NAP.CTS.ViewModel.ProbationManagement;
    using System.Linq;
    using Fanex.NAP.CTS.Model.ProbationManagement;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Core.Extentions;
    using System.Threading.Tasks;
    using System;
    using System.Text;
    using Fanex.NAP.CTS.Core;
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Helpers.Xml;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.ProbationManagement, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ProbationManagementController : BaseController
    {
        private const string ExportTemplate = "~/App_Data/Excel Template/ProbationManagement/ExportTemplate.xlsx";
        private readonly IProbationManagementService _probationService;
        private readonly ManageSubscriberService _subscriberService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public ProbationManagementController()
        {
            _probationService = new ProbationManagementService();
            _subscriberService = new ManageSubscriberService();
        }

        public ActionResult Index(int filterType = 0)
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.ProbationManagement,
                            NAP.Core.Business.Constant.Policy.Host);

            ViewBag.Subscribers = _subscriberService.GetSubscribers();
            ViewBag.AccountStaticStatuses = _probationService.GetAccountStaticStatuses();
          
            return View(new RenderProbationViewModel()
            {
                IsHostPermission = isHostPermission,
                ProbationFilterType = filterType
            });
        }      

        [HttpGet]
        public ActionResult ExportProbationCategory(ExportProbationCategoryFilter filter)
        {
            var probationCategoryList = Enumerable.Empty<ProbationCategoryViewModel>();
            var dBResult = new DBResult();

            filter = ParseFilter(filter);

            var data = _probationService.GetProbationCategory(filter, out dBResult);

            if (dBResult.ErrorCode != 0)
                return new HttpStatusCodeResult(dBResult.ErrorCode, dBResult.ErrorMsg);

            if (data != null && data.Any())
            {
                probationCategoryList = FillAccumulatedInfo(data, filter);
            }

            var csvData = _probationService.ExportCSV(probationCategoryList, filter);

            var csvFileName = string.Format("Probations_{0}.csv", DateTime.Now.ToString("yyyyMMdd"));

            var fileBytes = Encoding.UTF8.GetBytes(csvData);

            return File(fileBytes, "text/plain", csvFileName);
        }

        private ExportProbationCategoryFilter ParseFilter(ExportProbationCategoryFilter filter)
        {
            if (filter.DaysLeft != null && filter.DaysLeft.Contains("-"))
            {
                var arr = filter.DaysLeft.Split(new char[] { '-' });
                int fromDaysLeft = 0, toDaysLeft = 0;

                int.TryParse(arr[0], out fromDaysLeft);
                int.TryParse(arr[1], out toDaysLeft);

                filter.FromDaysLeft = fromDaysLeft;
                filter.ToDaysLeft = toDaysLeft;
            }

            return filter;
        }

        private IEnumerable<ProbationCategoryViewModel> FillAccumulatedInfo(IEnumerable<ProbationCategoryModel> data, ExportProbationCategoryFilter filter)
        {
            var probationCategoryList = Enumerable.Empty<ProbationCategoryViewModel>();
            var minWinLossDecimal = new decimal();
            var maxWinLossDecimal = new decimal();
            var minMarginDecimal = new decimal();
            var maxMarginDecimal = new decimal();

            decimal.TryParse(filter.MinWinLoss, out minWinLossDecimal);
            decimal.TryParse(filter.MaxWinLoss, out maxWinLossDecimal);
            decimal.TryParse(filter.MinMargin, out minMarginDecimal);
            decimal.TryParse(filter.MaxMargin, out maxMarginDecimal);

            var accumulatedData = GetAccumulatedInfo(data);

            probationCategoryList = data.Select(x =>
            {
                var isNotAccumulatedInfo = false;
                var accumulatedItem = accumulatedData.FirstOrDefault(y => y.CustId == x.CustID && y.SportId == x.SportGroupID);
                var currentWinLoss = new decimal();
                var currentWinLossString = "N/A";
                var currentMargin = new decimal();
                var currentMarginString = "N/A";

                if (accumulatedItem != null)
                {
                    currentWinLoss = accumulatedItem.Winloss;
                    currentWinLossString = accumulatedItem.Winloss.ToString(Consts.FormatDecimalToString_2Places);
                    currentMargin = accumulatedItem.Margin;
                    currentMarginString = accumulatedItem.Margin.ToString(Consts.FormatDecimalToString_2Places);
                }
                else
                {
                    isNotAccumulatedInfo = true;
                }

                return new ProbationCategoryViewModel
                {
                    CustId = x.CustID,
                    IsNotAccumulatedInfo = isNotAccumulatedInfo,
                    UserName = x.Username,
                    SiteName = x.Site,
                    Status = x.Status,
                    AssociatedWithPA = x.AssociatedWithPA,
                    SportGroupId = x.SportGroupID,
                    SportName = x.Sport,
                    TargetCategoryName = x.TargetCategoryName,
                    TaggedDate = x.TaggedDate.ToString(Consts.FormatDateToString_MMMddyyyy2),
                    CurrentWinLoss = currentWinLoss,
                    CurrentWinLossString = currentWinLossString,
                    FirstMargin = x.FirstMargin,
                    CurrentMargin = currentMargin,
                    CurrentMarginString = currentMarginString,
                    DaysLeft = x.DaysLeft
                };
            });

            if (!string.IsNullOrEmpty(filter.MinWinLoss) && !string.IsNullOrEmpty(filter.MaxWinLoss))
            {
                probationCategoryList = probationCategoryList.Where(x => x.CurrentWinLoss >= minWinLossDecimal && x.CurrentWinLoss <= maxWinLossDecimal);
            }
            else if (!string.IsNullOrEmpty(filter.MinWinLoss) && string.IsNullOrEmpty(filter.MaxWinLoss))
            {
                probationCategoryList = probationCategoryList.Where(x => x.CurrentWinLoss >= minWinLossDecimal);
            }
            else if (string.IsNullOrEmpty(filter.MinWinLoss) && !string.IsNullOrEmpty(filter.MaxWinLoss))
            {
                probationCategoryList = probationCategoryList.Where(x => x.CurrentWinLoss <= maxWinLossDecimal);
            }

            if (!string.IsNullOrEmpty(filter.MinMargin) && !string.IsNullOrEmpty(filter.MaxMargin))
            {
                probationCategoryList = probationCategoryList.Where(x => x.CurrentMargin >= minMarginDecimal && x.CurrentMargin <= maxMarginDecimal);
            }
            else if (!string.IsNullOrEmpty(filter.MinMargin) && string.IsNullOrEmpty(filter.MaxMargin))
            {
                probationCategoryList = probationCategoryList.Where(x => x.CurrentMargin >= minMarginDecimal);
            }
            else if (string.IsNullOrEmpty(filter.MinMargin) && !string.IsNullOrEmpty(filter.MaxMargin))
            {
                probationCategoryList = probationCategoryList.Where(x => x.CurrentMargin <= maxMarginDecimal);
            }
            probationCategoryList = probationCategoryList.OrderByDescending(x => x.DaysLeft);

            var custNotAccumulateds = probationCategoryList.Where(x => x.IsNotAccumulatedInfo);

            if (custNotAccumulateds.Any())
            {
                var custHasNotAccumulatedList = custNotAccumulateds.Select(x => new ProbationModel { CustId = x.CustId, SportGroupId = x.SportGroupId });

                LogService.Error("CTS - Probation Management - FillAccumulatedInfo List Cust has not Accumulated info: " + custHasNotAccumulatedList.ToXmlString());
            }

            return probationCategoryList;
        }

        private IEnumerable<AccumulatedInfoModel> GetAccumulatedInfo(IEnumerable<ProbationCategoryModel> data)
        {
            var accumulatedList = new List<AccumulatedInfoModel>();
            var tasks = new List<Task>();

            var probationModelList = data.Select(x => new ProbationModel
            {
                CustId = x.CustID,
                SportGroupId = x.SportGroupID
            }).OrderBy(x => x.CustId);

            var loopProbationModelList = probationModelList.Chunk(ConfigSettings.GetAccummulatedChunkSize).ToList();
            var batchTotal = loopProbationModelList.Count();
            var batchStart = 0;

            do
            {
                batchStart++;

                var customersXML = loopProbationModelList[batchStart - 1].ToXmlString();

                tasks.Add(Task.Run(() =>
                {
                    var accumulatedData = _probationService.GetAccummulatedInfo(customersXML);

                    accumulatedList.AddRange(accumulatedData);
                }));

                if (batchStart % 4 == 0 || batchStart == batchTotal)
                {
                    Task.WhenAll(tasks.ToArray()).Wait();
                    tasks = new List<Task>();
                }
            } while (batchStart < batchTotal);

            return accumulatedList;
        }

    }
}
