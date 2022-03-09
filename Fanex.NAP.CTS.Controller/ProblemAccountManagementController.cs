namespace Fanex.NAP.CTS.Controller
{
    using Fanex.NAP.Core.Web.Attribute;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.ViewModel.ProblemAccountManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.ProblemAccountManagement, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class ProblemAccountManagementController : BaseController
    {
        private readonly IProblemAccountManagementService _problemAccountManagementService;        

        public override string BaseControllerName { get => Consts.MenuItemController; }

        public ProblemAccountManagementController()
        {
            _problemAccountManagementService = new ProblemAccountManagementService();
        }

        public ActionResult Index()
        {
            var sites = _problemAccountManagementService.GetAllSites();

            return View(sites);
        }

        [HttpPost]
        public ActionResult GetProblemAccount(ProblemAccountFilterViewModel filter)
        {
            var problemAccounts = Enumerable.Empty<ProblemAccountViewModel>();
            var data = _problemAccountManagementService.GetProblemAccounts(filter);

            if (data != null && data.Any())
            {
                problemAccounts = data.Select(x => new ProblemAccountViewModel
                {
                    UserName = x.UserName,
                    Site = x.Site,
                    Currency = x.Currency,
                    Category = x.CategoryName,
                    Comment = string.Empty,
                    CreatedDateStr = x.CreatedDate.ToString(Consts.FormatDateToString_MMMddyyyy)
                });
            }

            return Json(new
            {
                data = problemAccounts
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProblemAccountAllCategories()
        {
            var data = "";

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidateUsernames(string userNames)
        {
            var data = _problemAccountManagementService.GetProblemAccountCategories();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}
