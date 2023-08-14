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

        #region dcs center
        [HttpPost]
        public JsonResult<SearchAccountResultModel> SearchAccountDCSCenter(SearchAccountParamModel param)
        {
            try
            {
                var data = _fpsService.SearchAccount_DCSCenter(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Search Account info DCS Center exception: " + ex.Message);
                return Json(new SearchAccountResultModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssAccountInfoByCustSubscriberModel> GetAssAccountInfoByCustSubscriberDCSCenter(GetAssAccountInfoByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssAccountInfoByCustSubscriber_DCSCenter(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber DCS Center exception: " + ex.Message);
                return Json(new GetAssAccountInfoByCustSubscriberModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssAccountsByCustSubscriberResultModel> GetAssAccountsByCustSubscriberDCSCenter(GetAssAccountsByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssAccountsByCustSubscriber_DCSCenter(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber DCS Center exception: " + ex.Message);
                return Json(new GetAssAccountsByCustSubscriberResultModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssDevicesByCustSubscriberResultModel> GetAssDevicesByCustSubscriberDCSCenter(GetAssDevicesByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssDevicesByCustSubscriber_DCSCenter(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber DCS Center exception: " + ex.Message);
                return Json(new GetAssDevicesByCustSubscriberResultModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssMatrixAccountByCustSubscriberResultModel> GetAssMatrixAccountByCustSubscriberDCSCenter(GetAssMatrixAccountByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssMatrixAccountByCustSubscriber_DCSCenter(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber DCS Center exception: " + ex.Message);
                return Json(new GetAssMatrixAccountByCustSubscriberResultModel());
            }
        }

        [HttpPost]
        public JsonResult<TransactionResultModel> GetTransactionHistoryDCSCenter(TransactionParamModel param)
        {
            try
            {
                var data = _fpsService.GetTransactionHistory_DCSCenter(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Transaction History DCS Center exception: " + ex.Message);

                if (!string.IsNullOrEmpty(ex.Message) && ex.Message.ToLower().Contains("timeout expired"))
                {
                    return Json(new TransactionResultModel() { IsTimeout = true });
                }

                return Json(new TransactionResultModel());
            }
        }

        [HttpPost]
        public JsonResult<LatestTransactionResultModel> GetLastestTransactionBySubscriberDCSCenter(LatestTransactionParamModel param)
        {
            try
            {
                var data = _fpsService.GetLastestTransactionBySubscriber_DCSCenter(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Lastest Transaction By Subscriber DCS Center exception: " + ex.Message);
                return Json(new LatestTransactionResultModel());
            }
        }
        #endregion dcs center

        #region dcs extra

        #region get sub
        [HttpGet]
        public JsonResult<SubscriberFpsInfo> GetSubscriberInfoDCSExtra(string subscriberName)
        {
            try
            {
                var subscriber = _subscriberService.GetSubscriberBySubscriberName_DCSExtra(subscriberName);

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
                LogService.Error("Get Subscriber info DCS Extra exception: " + ex.Message);
            }

            return Json(new SubscriberFpsInfo());
        }
        #endregion

        #region search
        [HttpPost]
        public JsonResult<SearchAccountResultModel> SearchAccountDCSExtra(SearchAccountParamModel param)
        {
            try
            {
                var data = _fpsService.SearchAccount_DCSExtra(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Search Account info DCS Extra exception: " + ex.Message);
                return Json(new SearchAccountResultModel());
            }
        }

        [HttpPost]
        public JsonResult<List<SearchDeviceItemDCSExtraModel>> SearchDeviceDCSExtra(SearchDeviceParamModel param)
        {
            try
            {
                var data = _fpsService.SearchDevice_DCSExtra(param);

                return Json(data.ToList());

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Search Device info DCS Extra exception: " + ex.Message);
                return Json(new List<SearchDeviceItemDCSExtraModel>());
            }
        }
        #endregion

        #region account
        [HttpPost]
        public JsonResult<GetAssAccountInfoByCustSubscriberModel> GetAssAccountInfoByCustSubscriberDCSExtra(GetAssAccountInfoByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssAccountInfoByCustSubscriber_DCSExtra(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber DCS Extra exception: " + ex.Message);
                return Json(new GetAssAccountInfoByCustSubscriberModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssAccountsByCustSubscriberResultModel> GetAssAccountsByCustSubscriberDCSExtra(GetAssAccountsByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssAccountsByCustSubscriber_DCSExtra(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber DCS Extra exception: " + ex.Message);
                return Json(new GetAssAccountsByCustSubscriberResultModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssDevicesByCustSubscriberResultModel> GetAssDevicesByCustSubscriberDCSExtra(GetAssDevicesByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssDevicesByCustSubscriber_DCSExtra(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber DCS Extra exception: " + ex.Message);
                return Json(new GetAssDevicesByCustSubscriberResultModel());
            }
        }

        [HttpPost]
        public JsonResult<GetAssMatrixAccountByCustSubscriberResultModel> GetAssMatrixAccountByCustSubscriberDCSExtra(GetAssMatrixAccountByCustSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssMatrixAccountByCustSubscriber_DCSExtra(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber DCS Extra exception: " + ex.Message);
                return Json(new GetAssMatrixAccountByCustSubscriberResultModel());
            }
        }
        #endregion

        #region device
        [HttpPost]
        public JsonResult<GetAssDeviceInfoByIDSubscriberModel> GetAssDeviceInfoByIdSubscriberDCSExtra(GetAssDeviceInfoByIDSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssDeviceInfoByIDSubscriber_DCSExtra(param);

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by id subscriber DCS Extra exception: " + ex.Message);
                return Json(new GetAssDeviceInfoByIDSubscriberModel());
            }
        }

        [HttpPost]
        public JsonResult<List<GetAssAccByDevSubscriberItemModel>> GetAssAccountsByDeviceSubscriberDCSExtra(GetAssAccByDevSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssAccByDevSubscriber_DCSExtra(param);

                return Json(data.ToList());

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account by Device subscriber DCS Extra exception: " + ex.Message);
                return Json(new List<GetAssAccByDevSubscriberItemModel>());
            }
        }

        [HttpPost]
        public JsonResult<List<GetAssDevByDevSubscriberItemModel>> GetAssDevByDeviceSubscriberDCSExtra(GetAssDevByDevSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssDevByDevSubscriber_DCSExtra(param);

                return Json(data.ToList());

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Device by Device subscriber DCS Extra exception: " + ex.Message);
                return Json(new List<GetAssDevByDevSubscriberItemModel>());
            }
        }

        [HttpPost]
        public JsonResult<List<GetAssMatrixByDeviceSubscriberItemModel>> GetAssMatrixByDeviceSubscriberDCSExtra(GetAssMatrixByDeviceSubscriberParamModel param)
        {
            try
            {
                var data = _fpsService.GetAssMatrixByDeviceSubscriber_DCSExtra(param);

                return Json(data.ToList());

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Matrix by Device subscriber DCS Extra exception: " + ex.Message);
                return Json(new List<GetAssMatrixByDeviceSubscriberItemModel>());
            }
        }
        #endregion

        #region report
        [HttpPost]
        public JsonResult<TransactionHistoryByDeviceDCSExtraResultModel> GetTransactionHistoryByDeviceDCSExtra(TransactionHistoryByDeviceParamModel param)
        {
            try
            {
                var data = _fpsService.GetTransactionHistoryByDevice_DCSExtra(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Transaction History By Device DCS Extra exception: " + ex.Message);

                if (!string.IsNullOrEmpty(ex.Message) && ex.Message.ToLower().Contains("timeout expired"))
                {
                    return Json(new TransactionHistoryByDeviceDCSExtraResultModel() { IsTimeout = true });
                }

                return Json(new TransactionHistoryByDeviceDCSExtraResultModel());
            }
        }

        [HttpPost]
        public JsonResult<TransactionHistoryByAccountDCSExtraResultModel> GetTransactionHistoryByAccountDCSExtra(TransactionHistoryByAccountParamModel param)
        {
            try
            {
                var data = _fpsService.GetTransactionHistoryByAccount_DCSExtra(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Transaction History By Account DCS Extra exception: " + ex.Message);

                if (!string.IsNullOrEmpty(ex.Message) && ex.Message.ToLower().Contains("timeout expired"))
                {
                    return Json(new TransactionHistoryByAccountDCSExtraResultModel() { IsTimeout = true });
                }

                return Json(new TransactionHistoryByAccountDCSExtraResultModel());
            }
        }

        [HttpPost]
        public JsonResult<TransactionResultModel> GetTransactionHistoryDCSExtra(TransactionParamModel param)
        {
            try
            {
                var data = _fpsService.GetTransactionHistory_DCSExtra(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Transaction History DCS Extra exception: " + ex.Message);

                if (!string.IsNullOrEmpty(ex.Message) && ex.Message.ToLower().Contains("timeout expired"))
                {
                    return Json(new TransactionResultModel() { IsTimeout = true });
                }

                return Json(new TransactionResultModel());
            }
        }

        [HttpPost]
        public JsonResult<LatestTransactionResultModel> GetLastestTransactionBySubscriberDCSExtra(LatestTransactionParamModel param)
        {
            try
            {
                var data = _fpsService.GetLastestTransactionBySubscriber_DCSExtra(param);

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Lastest Transaction By Subscriber DCS Extra exception: " + ex.Message);
                return Json(new LatestTransactionResultModel());
            }
        }
        #endregion

        #endregion dcs extra
    }
}
