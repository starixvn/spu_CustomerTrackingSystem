namespace Fanex.NAP.CTS.Controller
{
    using Fanex.Common;
    using Fanex.Logging;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Core.BaseClasses;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Utilities;
    using Fanex.NAP.CTS.Model.ProblemAccountMonitor;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountMonitor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [NapAuth(FunctionKey = FSFunctionKeys.SpecialCustomerClass, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class SpecialCustomerClassController : BaseController
    {
        private readonly CustomerClassService _customerClassService;

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public SpecialCustomerClassController() 
            => _customerClassService = new CustomerClassService();

        public ActionResult Index()
        {           
            return View();
        }

        [HttpGet]
        public ActionResult GetCustomerClass()
        {
            var viewModel = _customerClassService.GetAllCustomerClass();

            return Json(new
            {               
                data = viewModel
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddCustomerClass(string username, int custClass, bool isAffectToAss)
        {
            _customerClassService.InsertCustomerClass(username, custClass, isAffectToAss);
            var a = username;
            return View();
        }
        
    }
}