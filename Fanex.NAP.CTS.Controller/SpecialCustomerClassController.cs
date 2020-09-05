namespace Fanex.NAP.CTS.Controller
{  
    using Fanex.NAP.Core.Web.Attribute;  
    using Fanex.NAP.CTS.Resource.Consts;    
    using Fanex.NAP.CTS.Service.Implement;        
    using System.Linq;    
    using System.Web.Mvc;
    
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
    }
}