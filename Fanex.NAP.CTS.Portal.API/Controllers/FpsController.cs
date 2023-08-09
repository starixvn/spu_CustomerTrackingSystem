namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Model.API.FPS.Account;
    using Fanex.NAP.CTS.Model.API.FPS.Device;
    using Fanex.NAP.CTS.Model.API.FPS.Search;
    using Fanex.NAP.CTS.Model.API.FPS.SearchAccount;
    using Fanex.NAP.CTS.Model.API.FPS.Subscriber;
    using Fanex.NAP.CTS.Model.API.FPS.Transaction;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Implement.Api;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        public JsonResult<SearchAccountResultModel> SearchAccountCts(SearchAccountParamModel param)
        {
            try
            {
                var data = _fpsService.SearchAccountCts(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Search Account info Cts exception: " + ex.Message);
                return Json(new SearchAccountResultModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssAccountInfoByCustSubscriberModel> GetAssAccountInfoByCustSubscriberCts(GetAssAccountInfoByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssAccountInfoByCustSubscriberCts(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber Cts exception: " + ex.Message);
                return Json(new GetAssAccountInfoByCustSubscriberModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssAccountsByCustSubscriberResultModel> GetAssAccountsByCustSubscriberCts(GetAssAccountsByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssAccountsByCustSubscriberCts(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber Cts exception: " + ex.Message);
                return Json(new GetAssAccountsByCustSubscriberResultModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssDevicesByCustSubscriberResultModel> GetAssDevicesByCustSubscriberCts(GetAssDevicesByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssDevicesByCustSubscriberCts(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber Cts exception: " + ex.Message);
                return Json(new GetAssDevicesByCustSubscriberResultModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssMatrixAccountByCustSubscriberResultModel> GetAssMatrixAccountByCustSubscriberCts(GetAssMatrixAccountByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssMatrixAccountByCustSubscriberCts(param);

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

        #region get sub
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
        #endregion

        #region search
        [HttpPost]
        public JsonResult<SearchAccountResultModel> SearchAccountDcs(SearchAccountParamModel param)
        {
            try
            {
                var data = _fpsService.SearchAccountDcs(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Search Account info Dcs exception: " + ex.Message);
                return Json(new SearchAccountResultModel());
            }
        }

        [HttpPost]
        public JsonResult<List<SearchDeviceItemDcsModel>> SearchDeviceDcs(SearchDeviceParamModel param)
        {
            try
            {
                var data = _fpsService.SearchDeviceDcs(param);

                return Json(data.ToList());

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Search Device info Dcs exception: " + ex.Message);
                return Json(new List<SearchDeviceItemDcsModel>());
            }
        }
        #endregion

        #region account
        [HttpPost]
        public JsonResult<GetAssAccountInfoByCustSubscriberModel> GetAssAccountInfoByCustSubscriberDcs(GetAssAccountInfoByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssAccountInfoByCustSubscriberDcs(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber Dcs exception: " + ex.Message);
                return Json(new GetAssAccountInfoByCustSubscriberModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssAccountsByCustSubscriberResultModel> GetAssAccountsByCustSubscriberDcs(GetAssAccountsByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssAccountsByCustSubscriberDcs(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber Dcs exception: " + ex.Message);
                return Json(new GetAssAccountsByCustSubscriberResultModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssDevicesByCustSubscriberResultModel> GetAssDevicesByCustSubscriberDcs(GetAssDevicesByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssDevicesByCustSubscriberDcs(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber Dcs exception: " + ex.Message);
                return Json(new GetAssDevicesByCustSubscriberResultModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssMatrixAccountByCustSubscriberResultModel> GetAssMatrixAccountByCustSubscriberDcs(GetAssMatrixAccountByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssMatrixAccountByCustSubscriberDcs(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber Dcs exception: " + ex.Message);
                return Json(new GetAssMatrixAccountByCustSubscriberResultModel());
            }
        }
        #endregion

        #region device
        [HttpPost]
        public JsonResult<GetAssDeviceInfoByIDSubscriberModel> GetAssDeviceInfoByIdSubscriberDcs(GetAssDeviceInfoByIDSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssDeviceInfoByIDSubscriberDcs(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber Dcs exception: " + ex.Message);
                return Json(new GetAssDeviceInfoByIDSubscriberModel());
            }
        }
        #endregion

        #region report
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
        #endregion

        #endregion dcs
    }
}
