namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Model.API.FPS.SearchAccount;
    using Fanex.NAP.CTS.Service.Implement.Api;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using Fanex.NAP.CTS.ViewModel.API.SearchAccount;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Results;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class FpsController : ApiController
    {
        private readonly ISearchAccountService _accountService;

        public FpsController()
        {
            _accountService = new SearchAccountService();
        }

        [HttpGet]
        public JsonResult<List<SearchAccountResultViewModel>> SearchAccount(int subscriberID, string userName, bool isSearchPartial)
        {
            try
            {
                var accounts = new List<SearchAccountResultViewModel>();
                var data = _accountService.SearchAccount(subscriberID, userName);

                if (data != null && data.Any())
                {
                    accounts = data.Select(x => new SearchAccountResultViewModel(x)).ToList();                   
                }

                return Json(accounts.ToList());
               
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Search Account info exception: " + ex.Message);
                return Json(new List<SearchAccountResultViewModel>());
            }
        }

    }
}
