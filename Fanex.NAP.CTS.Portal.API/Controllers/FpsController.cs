namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.API.FPS.Account;
    using Fanex.NAP.CTS.Model.API.FPS.Device;
    using Fanex.NAP.CTS.Model.API.FPS.SearchAccount;
    using Fanex.NAP.CTS.Model.API.FPS.Subscriber;
    using Fanex.NAP.CTS.Model.API.FPS.Transaction;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Implement.Api;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using Microsoft.Ajax.Utilities;
    using Newtonsoft.Json;
    using System;
    using System.Web.Http;
    using System.Web.Http.Results;

    public class FpsController : ApiController
    {
        private readonly IFpsService _fpsService;
        private readonly ISubscriberService _subscriberService;

        public FpsController()
        {
            _fpsService = new FpsService();
            _subscriberService = new SubscriberService();
        }

        #region cts
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
                LogService.Error("FPS Search Account info Cts exception: " + ex.Message);
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
                LogService.Error("FPS Get Ass Account info by cust subscriber Cts exception: " + ex.Message);
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
                LogService.Error("FPS Get Ass Account info by cust subscriber Cts exception: " + ex.Message);
                return Json(new GetAssAccountsByCustSubscriberResultModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssDevicesByCustSubscriberResultModel> GetAssDevicesByCustSubscriber(GetAssDevicesByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssDevicesByCustSubscriber(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber Cts exception: " + ex.Message);
                return Json(new GetAssDevicesByCustSubscriberResultModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssMatrixAccountByCustSubscriberResultModel> GetAssMatrixAccountByCustSubscriber(GetAssMatrixAccountByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssMatrixAccountByCustSubscriber(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber Cts exception: " + ex.Message);
                return Json(new GetAssMatrixAccountByCustSubscriberResultModel());
            }
        }

        [HttpPost]
        public JsonResult<TransactionResultModel> GetTransactionHistoryCts(TransactionParamModel param)
        {
            try
            {
                var data = _fpsService.GetTransactionHistoryCts(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Transaction History Cts exception: " + ex.Message);

                if (!string.IsNullOrEmpty(ex.Message) && ex.Message.ToLower().Contains("timeout expired"))
                {
                    return Json(new TransactionResultModel() { IsTimeout = true });
                }

                return Json(new TransactionResultModel());
            }
        }

        [HttpPost]
        public JsonResult<LastestTransactionResultModel> GetLastestTransactionBySubscriberCts(LastestTransactionParamModel param)
        {
            try
            {
                var data = _fpsService.GetLastestTransactionBySubscriberCts(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Lastest Transaction By Subscriber Cts exception: " + ex.Message);
                return Json(new LastestTransactionResultModel());
            }
        }
        #endregion cts

        #region dcs
        [HttpGet]
        public JsonResult<SubscriberFpsInfo> GetSubscriberInfoDcs(string subscriberName)
        {
            try
            {
                var subscriber = _subscriberService.GetSubscriberDcsExBySubscriberName(subscriberName);

                if (subscriber != null && subscriber.SubscriberStatus)
                {
                    return Json(subscriber);
                }
                else
                {
                    LogService.Error($"{subscriberName} is not integrated on DCS Extra.");
                }
            }
            catch (Exception ex)
            {
                LogService.Error("Get Subscriber info Dcs exception: " + ex.Message);
            }

            return Json(new SubscriberFpsInfo());
        }

        [HttpPost]
        public JsonResult<TransactionResultModel> GetTransactionHistoryDcs(TransactionParamModel param)
        {
            try
            {
                var data = _fpsService.GetTransactionHistoryDcs(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Transaction History Dcs exception: " + ex.Message);

                if (!string.IsNullOrEmpty(ex.Message) && ex.Message.ToLower().Contains("timeout expired"))
                {
                    return Json(new TransactionResultModel() { IsTimeout = true });
                }

                return Json(new TransactionResultModel());
            }
        }

        [HttpPost]
        public JsonResult<LastestTransactionResultModel> GetLastestTransactionBySubscriberDcs(LastestTransactionParamModel param)
        {
            try
            {
                var data = _fpsService.GetLastestTransactionBySubscriberDcs(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Lastest Transaction By Subscriber Dcs exception: " + ex.Message);
                return Json(new LastestTransactionResultModel());
            }
        }
        #endregion dcs
    }
}
