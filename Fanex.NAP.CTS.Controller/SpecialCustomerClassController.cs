namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using Fanex.NAP.CTS.Resource.Consts;    
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.SpecialCustomerClass;
    using System;
    using System.Collections.Generic;
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
            var viewModel = Enumerable.Empty<CustomerClassViewModel>();
            var specialCustomers = _customerClassService.GetAllCustomerClass();

            if (specialCustomers != null && specialCustomers.Any())
            {
                viewModel = specialCustomers.Select(x => new CustomerClassViewModel
                {
                    CTSCustID = x.CTSCustID,
                    CustID = x.CustID,
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
            var resultInsert = new ResultInsertRemoveViewModel
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty
            };

            var resultInsertToCTS = _customerClassService.InsertCustomerClass(username, custClass, isAffectToAss, CurrentSession.UserId);

            if (string.IsNullOrEmpty(resultInsertToCTS.OpErrorMessage))
            {
                if (resultInsertToCTS.CustomerClassList != null && resultInsertToCTS.CustomerClassList.Any())
                {
                    var list = resultInsertToCTS.CustomerClassList.Where(c => c.CustID != 0);

                    _customerClassService.SyncCustomerClassToMainDb(list, "Insert");
                }
            }
            else
            {
                resultInsert.ErrorCode = -1;
                resultInsert.ErrorMessage = resultInsertToCTS.OpErrorMessage;
            }

            return Json(resultInsert, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.SpecialCustomerClass, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult RemoveCustomerClass(int custID, long ctsCustID)
        {
            var result = new ResultInsertRemoveViewModel
            {
                ErrorCode = 0,
                ErrorMessage = string.Empty
            };

            var custToMainDbList = new List<CustomerClassForMainDb>
            {
                new CustomerClassForMainDb
                {
                    CustID = custID,
                    CustomerClass = -1
                }
            };

            var resultSyncToMainDb = _customerClassService.SyncCustomerClassToMainDb(custToMainDbList, "Remove");

            if (resultSyncToMainDb.ErrorCode == 0)
            {
                var resultRemoveToCTS = _customerClassService.RemoveCustomerClass(ctsCustID, CurrentSession.UserId);

                result.ErrorCode = resultRemoveToCTS.ErrorCode;
                result.ErrorMessage = resultRemoveToCTS.ErrorMsg;
            }
            else
            {
                result.ErrorCode = resultSyncToMainDb.ErrorCode;
                result.ErrorMessage = resultSyncToMainDb.ErrorMsg;
            }
           
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
