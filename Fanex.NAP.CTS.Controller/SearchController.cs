namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Implement.AIInsights;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Interface.AIInsights;
    using Fanex.NAP.CTS.ViewModel.Home;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    [NapSuperAuth]
    public class SearchController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IAIInsightsService _aIInsightsService;
        private readonly IMemoryCacheService _memoryCacheService;

        public SearchController()
        {
            _accountService = new AccountService();
            _aIInsightsService = new AIInsightsService();
            _memoryCacheService = new RedisCacheService();
        }
        [HttpPost]
        public async Task<ActionResult> Index(FormCollection form, HttpPostedFileBase uploadFile, string tabId)
        {
            string inputValue = form["txtInput"]?.Trim();

            bool hasText = !string.IsNullOrEmpty(inputValue) && inputValue.Length > 1 && inputValue.Length <= 50;
            var hasFile = uploadFile?.ContentLength > 0 &&
                            Consts.AIInsightFileSupportExtensions.Contains(Path.GetExtension(uploadFile.FileName).ToLower());

            if (hasFile && !hasText)
            {
                var userId = CurrentSession.UserId.ToString();

                var result = await _aIInsightsService.UploadFile(uploadFile, userId);
                _memoryCacheService.PushValueBySlidingTime(string.Format(Consts.AIInsightsUploadFileKey, userId, tabId), result?.response, 3);

                return RedirectToAction("Index", "AIInsights");
            }

            if (hasText)
            {
                var result = _accountService.SearchAccounts(inputValue);

                if (result.Count == 1)
                {
                    return RedirectToAction("Index", "Account", new { Id = result.First().CTSCustId });
                }

                return View("~/Views/Home/Search.cshtml", new SearchResultViewModel { SearchResults = result, InputValue = inputValue });
            }

            return View("~/Views/Home/Search.cshtml", new SearchResultViewModel { SearchResults = null, InputValue = inputValue });
        }
    }
}
