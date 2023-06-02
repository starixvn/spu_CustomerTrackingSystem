namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Model.API.FPS.Account;
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
        private readonly IFpsAccountService _accountService;

        public FpsController()
        {
            _accountService = new FpsAccountService();
        }

        [HttpPost]       
        public JsonResult<SearchAccountResultModel> SearchAccount(SearchAccountParamModel param)
        {
            try
            {
                var data = _accountService.SearchAccount(param);               

                return Json(data);
               
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Search Account info exception: " + ex.Message);
                return Json(new SearchAccountResultModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssAccountInfoByCustSubscriberModel> GetAssAccountInfoByCustSubscriber(GetAssAccountInfoByCustSubscriberParamModel param)
        {
            try
            {
                var data = _accountService.GetAssAccountInfoByCustSubscriber(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Ass Account info by cust subscriber exception: " + ex.Message);
                return Json(new GetAssAccountInfoByCustSubscriberModel());
            }
        }

    }
}
