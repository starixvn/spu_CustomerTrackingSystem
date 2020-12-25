namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Model.SpecialCustomerClass;
    using Fanex.NAP.CTS.Model.SyncCustomerToMainDb;
    using Fanex.NAP.CTS.Resource.Consts;    
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.SpecialCustomerClass;
    using System;
    using System.Collections.Generic;
    using System.Linq;    
    using System.Web.Mvc;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.SpecialCustomerClass, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class SpecialCustomerClassController : BaseController
    {
        private readonly ISpecialCustomerClassService _customerClassService;
        private readonly ISyncCustomerToMainDbService _syncToMainDbService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public SpecialCustomerClassController()
        {
            _customerClassService = new SpecialCustomerClassService();
            _syncToMainDbService = new SyncCustomerToMainDbService();
        }            

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.SpecialCustomerClass,
                            NAP.Core.Business.Constant.Policy.Host);
            
            return View(isHostPermission);
        }
        
        [HttpGet]
        public ActionResult GetCustomerClass(string username)
        {
            var viewModel = Enumerable.Empty<CustomerClassViewModel>();
            var specialCustomers = _customerClassService.GetAllCustomerClass(username);

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
                        ? x.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss) 
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
                    
                    _syncToMainDbService.SyncCustomerClassToMainDb(list, "Insert Special Customer Class");
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

            var resultSyncToMainDb = _syncToMainDbService.SyncCustomerClassToMainDb(custToMainDbList, "Remove Special Customer Class");

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
