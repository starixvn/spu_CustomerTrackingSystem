namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Resource.Consts;    
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.SpecialCustomerClass;
    using System;
    using System.Linq;    
    using System.Web.Mvc;    

    [NapAuth(FunctionKey = FSFunctionKeys.SpecialCustomerClass, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class SpecialCustomerClassController : BaseController
    {
        private readonly ISpecialCustomerClassService _customerClassService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public SpecialCustomerClassController() 
            => _customerClassService = new SpecialCustomerClassService();

        public ActionResult Index()
        {           
            return View();
        }

        [HttpGet]
        public ActionResult GetCustomerClass()
        {
            var viewModel = Enumerable.Empty<SpecialCustomerClassViewModel>();
            var specialCustomers = _customerClassService.GetAllCustomerClass();

            if (specialCustomers != null && specialCustomers.Any())
            {
                viewModel = specialCustomers.Select(x => new SpecialCustomerClassViewModel
                {
                    CTSCustID = x.CTSCustID,
                    RootCTSCustID = x.RootCTSCustID,
                    Username = x.Username,
                    SubscriberName = x.SubscriberName,
                    CustomerClass = x.CustomerClass,
                    CreatedBy = x.CreatedBy,
                    CreatedDateString = x.CreatedDate != DateTime.MinValue 
                        ? x.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyy) 
                        : string.Empty
                });
            }

            return Json(new { data = viewModel }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.SpecialCustomerClass, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult AddCustomerClass(string username, int custClass, bool isAffectToAss)
        {
            var resultInsertToCTS = _customerClassService.InsertCustomerClass(username, custClass, isAffectToAss, CurrentSession.UserId);

            if (string.IsNullOrEmpty(resultInsertToCTS.OpErrorMessage))
            {
                resultInsertToCTS.ErrorCode = 0;

                if (resultInsertToCTS.CustomerClassList != null && resultInsertToCTS.CustomerClassList.Any())
                {
                    var list = resultInsertToCTS.CustomerClassList.Where(c => c.CustID != 0);

                    _customerClassService.SyncCustomerClassToMainDb(list);
                }
            }
            else
            {
                resultInsertToCTS.ErrorCode = -1;
            }

            return Json(resultInsertToCTS, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.SpecialCustomerClass, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RemoveCustomerClass(long ctsCustId)
        {
            var opErrorMessage = string.Empty;
            var errorCode = _customerClassService.RemoveCustomerClass(ctsCustId);

            return Json(new
            {
                errorCode,
                opErrorMessage
            }, JsonRequestBehavior.AllowGet);
        }

    }
}