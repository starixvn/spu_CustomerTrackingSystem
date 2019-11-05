namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.CTS.Library.Services;
    using Fanex.NAP.CTS.Library.ViewModels.Transaction;
    using System.Web.Mvc;
    using Fanex.NAP.Core.Web.Attribute;    

    public class TransactionController : BaseController
    {
        private readonly ITransactionService _transactionService;

        public TransactionController()
        {
            _transactionService = new TransactionService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.TransactionHistory, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetTransactionHistory(int accountId)
        {
            return PartialView("TransactionHistory");
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.TransactionHistory, Policy = NAP.Core.Business.Constant.Policy.Read)]
        public ActionResult GetTransactionHistoryFilter(TransactionHistoryFilter filter)
        {            
            var data = _transactionService.GetTransactionHistory(filter);
                       
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetIPInformation(string ip)
        {
            var data = _transactionService.GetIPInformation(ip);

            return PartialView("IPInformation", data);
        }
    }
}