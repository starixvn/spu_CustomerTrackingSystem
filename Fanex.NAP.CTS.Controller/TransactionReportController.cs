namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.TransactionReport;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.TransactionReport, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class TransactionReportController : BaseController
    {
        private readonly ICommonService _commonService;
        private readonly ITransactionReportService _transactionReportService;

        public override string BaseControllerName { get => Consts.MenuItemReport; }

        public TransactionReportController()
        {
            _commonService = new CommonService();
            _transactionReportService = new TransactionReportService();
        }

        public ActionResult Index()
        {
            ViewBag.Subscribers = _commonService.GetSubscriberGroups();

            return View();
        }

        [HttpPost]
        [LogActionFilterAttribute(systemId = SystemId.CTSWeb, functionId = MonitorCtsFunctionId.TransactionReport)]
        public ActionResult GetTransactions(TransactionFilterViewModel filter)
        {
            int totalItem = 0;

            var transactionList = Enumerable.Empty<TransactionViewModel>();

            var data = _transactionReportService.GetTransactions(filter, out totalItem);

            if (data != null && data.Any())
            {
                transactionList = data.Select(x => new TransactionViewModel
                {
                    Subscriber = x.Subscriber,
                    TotalTrans = x.TotalTrans,
                    TotalValidTrans = x.TotalValidTrans,
                    ValidTransNewDevice = x.ValidTransNewDevice,
                    ValidTransOldDevice = x.ValidTransOldDevice,
                    ValidTransRecoverDevice = x.ValidTransRecoverDevice,
                    ValidTransNoDevice = x.ValidTransNoDevice,
                    InvalidTrans = x.InvalidTrans,
                    Browserless = x.ValidBrowserless
                });
            }

            return Json(new
            {
                draw = filter.draw,
                recordsTotal = totalItem,
                recordsFiltered = totalItem,
                data = transactionList
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
