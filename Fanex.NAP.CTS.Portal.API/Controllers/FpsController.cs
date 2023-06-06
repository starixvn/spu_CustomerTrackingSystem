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

                if (!data.Items.Any())
                {
                    data = GetAssAccountsByCustSubscriber_Test();
                }

                return Json(data);

            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Ass Account info by cust subscriber exception: " + ex.Message);
                return Json(new GetAssAccountsByCustSubscriberResultModel());
            }
        }

        private GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriber_Test()
        {
            var data = new GetAssAccountsByCustSubscriberResultModel();

            data.Items = new List<GetAssAccountsByCustSubscriberItemModel>() {
                new GetAssAccountsByCustSubscriberItemModel() {
                    CTSCustID = 1,
                    RegisterName = "acb",
                    UserName = "def",
                    AssociatedAccount = 12,
                    DeviceInCommon = 1223, 
                    OtherDeives = 1
                }
            };

            data.TotalItem = 1;

            return data;
        }


        [HttpPost]
        public JsonResult<TransactionResultModel> GetTransactionHistory(TransactionParamModel param)
        {
            try
            {
                var data = _fpsService.GetTransactionHistory(param);

                if (!data.Items.Any())
                {
                    data = GenerateGetTransactionHistory_Test();
                }

                return Json(data);
            }
            catch (Exception ex)
            {
                LogService.Error("FPS Get Transaction History exception: " + ex.Message);
                return Json(new TransactionResultModel());
            }
        }        

        private TransactionResultModel GenerateGetTransactionHistory_Test()
        {
            var data = new TransactionResultModel();

            data.Items = new List<TransactionItemModel>() {
                new TransactionItemModel() {
                    Action = "login",
                    ActionResult  = "success",
                    FirstDeviceCode = "62962276a14fcbc8eede592a9fa12",
                    Browser = "Chrome",
                    TransID = 42214937,
                    IP = "49.2014.8.137",
                    OS = "Android",
                    LoginName = "account test",
                    TransTime = DateTime.Now,
                    URLDetails = "https://hrm.nexdev.net/Dashboard"
                }
            };

            data.TotalItem = 1;

            return data;
        }
    }
}
