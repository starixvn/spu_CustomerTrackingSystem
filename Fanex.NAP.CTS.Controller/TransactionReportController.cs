namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
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
        public ActionResult GetTransactions(TransactionFilterViewModel filter)
        {
            int totalItems = 0;

            var transactionList = Enumerable.Empty<TransactionViewModel>();

            var data = _transactionReportService.GetTransactions(filter);

            if (data != null && data.Any())
            {
                transactionList = data.Select(x => new TransactionViewModel
                {
                    SubscriberId = x.SubscriberId,
                    SubscriberName = x.SubscriberName,
                    TotalTrans = x.TotalTrans,
                    ValidTrans = x.ValidTrans,
                    InvalidTrans = x.InvalidTrans,
                    BrowserlessValidTrans = x.BrowserlessValidTrans
                });

                totalItems = data.Count();
            }

            return Json(new
            {
                draw = filter.draw,
                recordsTotal = totalItems,
                recordsFiltered = totalItems,
                data = transactionList
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
