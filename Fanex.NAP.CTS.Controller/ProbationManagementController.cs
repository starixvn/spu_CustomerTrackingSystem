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
    using Newtonsoft.Json;
    using Fanex.NAP.CTS.Model.ProbationManagement;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.ProbationManagement, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ProbationManagementController : BaseController
    {
        private readonly IProbationManagementService _probationService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public ProbationManagementController()
        {
            _probationService = new ProbationManagementService();
        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.ProbationManagement,
                            NAP.Core.Business.Constant.Policy.Host);

            ViewBag.GeneralCategories = GetCategories(1);
            ViewBag.DetailedCategories = GetCategories(2);

            return View(isHostPermission);
        }

        [HttpPost]
        public ActionResult GetProbationCategory(string username, int daysLeft, string minMargin, string maxMargin, int categoryMode)
        {
            var viewModel = Enumerable.Empty<ProbationCategoryViewModel>();
            var minMarginDecimal = new decimal();
            var maxMarginDecimal = new decimal();

            decimal.TryParse(minMargin, out minMarginDecimal);
            decimal.TryParse(maxMargin, out maxMarginDecimal);

            var data = _probationService.GetProbationCategory(username, daysLeft, categoryMode);

            if (data != null && data.Any())
            {
                var probationModelList = data.Select(x => new ProbationModel
                {
                    CustId = x.CustID,
                    CategoryId = x.TargetCategoryID,
                    SportGroupId = x.SportGroupID
                });

                var accumulatedData = _probationService.GetAccummulatedInfo(probationModelList);

                viewModel = data.Select(x =>
                {
                    var accumulatedItem = accumulatedData.FirstOrDefault(y => y.CustId == x.CustID && y.SportId == y.SportId);
                    var currentMargin = new decimal();
                    var currentMarginString = "N/A";

                    if (accumulatedItem != null)
                    {
                        currentMargin = accumulatedItem.Margin;
                        currentMarginString = accumulatedItem.Margin.ToString("0.00");
                    }

                    return new ProbationCategoryViewModel
                    {
                        CustId = x.CustID,
                        UserName = x.Username,
                        SiteName = x.Site,
                        SportGroupId = x.SportGroupID,
                        SportName = x.Sport,
                        TargetCategoryName = x.TargetCategoryName,
                        TaggedDate = x.TaggedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmm),
                        FirstMargin = x.FirstMargin,
                        CurrentMargin = currentMargin,
                        CurrentMarginString = currentMarginString,
                        DaysLeft = x.DaysLeft
                    };
                });

                if (!string.IsNullOrEmpty(minMargin) && !string.IsNullOrEmpty(maxMargin))
                {
                    viewModel = viewModel.Where(x => x.CurrentMargin >= minMarginDecimal && x.CurrentMargin <= maxMarginDecimal);
                }
                else if(!string.IsNullOrEmpty(minMargin) && string.IsNullOrEmpty(maxMargin))
                {
                    viewModel = viewModel.Where(x => x.CurrentMargin >= minMarginDecimal);
                }
                else if (string.IsNullOrEmpty(minMargin) && !string.IsNullOrEmpty(maxMargin))
                {
                    viewModel = viewModel.Where(x => x.CurrentMargin <= maxMarginDecimal);
                }
            }

            return Json(new
            {
                data = viewModel
            }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.ProbationManagement, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult ChangeCategory(int custId, int sportGroupId, int targetCategoryId)
        {
            var jsonData = JsonConvert.SerializeObject(new { custId, sportGroupId, targetCategoryId });

            var updatedResult = _probationService.ChangeCategory(jsonData, string.Empty, CurrentSession.UserId);

            if (updatedResult.DBResult.ErrorCode == 0)
            {
                _probationService.SyncCustomerClassToMainDb(updatedResult.CustomerClassList);
            }

            return Json(updatedResult.DBResult, JsonRequestBehavior.AllowGet);
        }       

        private IEnumerable<CategoryViewModel> GetCategories(int categoryMode)
        {
            var categoriesViewModel = Enumerable.Empty<CategoryViewModel>();
            
            var data = _probationService.GetCategories(categoryMode);
            if (data != null && data.Any())
            {
                categoriesViewModel = data
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new CategoryViewModel
                    {
                        CategoryID = x.CategoryID,
                        CategoryName = x.CategoryName
                    });
            }

            return categoriesViewModel;
        }
    }
}
