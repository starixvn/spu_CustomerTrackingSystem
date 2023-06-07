namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Model.API.FPS.Account;
    using Fanex.NAP.CTS.Model.API.FPS.SearchAccount;
    using Fanex.NAP.CTS.Model.API.FPS.Transaction;
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
        private readonly IFpsService _fpsService;

        public FpsController()
        {
            _fpsService = new FpsService();
        }

        [HttpPost]       
        public JsonResult<SearchAccountResultModel> SearchAccount(SearchAccountParamModel param)
        {
            try
            {
                var data = _fpsService.SearchAccount(param);               

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
                var data = _fpsService.GetAssAccountInfoByCustSubscriber(param);
             
                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber exception: " + ex.Message);
                return Json(new GetAssAccountInfoByCustSubscriberModel());
            }
        }       

        [HttpPost]
        public JsonResult<GetAssAccountsByCustSubscriberResultModel> GetAssAccountsByCustSubscriber(GetAssAccountsByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssAccountsByCustSubscriber(param);
               
                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber exception: " + ex.Message);
                return Json(new GetAssAccountsByCustSubscriberResultModel());
            }
        }
        
        [HttpPost]
        public JsonResult<TransactionResultModel> GetTransactionHistory(TransactionParamModel param)
        {
            try
            {                    
                var data = _fpsService.GetTransactionHistory(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Transaction History exception: " + ex.Message);
                return Json(new TransactionResultModel());
            }
        }      

        [HttpPost]
        public JsonResult<LastestTransactionResultModel> GetLastestTransactionBySubscriber(LastestTransactionParamModel param)
        {
            try
            {
                var data = _fpsService.GetLastestTransactionBySubscriber(param);
              
                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Lastest Transaction By Subscriber exception: " + ex.Message);
                return Json(new LastestTransactionResultModel());
            }
        }       
    }
}
