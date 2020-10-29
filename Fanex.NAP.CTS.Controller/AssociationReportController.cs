namespace Fanex.NAP.CTS.Controller
{   
    using Fanex.NAP.Core.Web.Attribute;   
    using System.Web.Mvc;
    using Fanex.NAP.Core.Service.Interface;
    using Fanex.NAP.Core.Service.Impl;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.CTS.Resource.Consts;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.ViewModel.AssociationReport;
    using Fanex.NAP.CTS.Core.Utilities;
    using System.Linq;
    using System.Collections.Generic;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Model.Account;

    [NapSuperAuth]
    [NapAuth(FunctionKey = FSFunctionKeys.AssociationReport, Policy = NAP.Core.Business.Constant.Policy.Read)]
    public class AssociationReportController : BaseController
    {
        private readonly IAssociationReportService _associationService;
        private readonly IManageSubscriberService _subscriberService;

        public override string BaseControllerName { get => Consts.MenuItemReport; }        

        public AssociationReportController()
        {
            _associationService = new AssociationReportService();
            _subscriberService = new ManageSubscriberService();
        }

        public ActionResult Index()
        {
            IUserService userService = new UserService();

            var isHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociationReport,
                            NAP.Core.Business.Constant.Policy.Host);

            var getAssociationStatus = _associationService.GetCustomerStaticStatus(3, true);
            getAssociationStatus.Insert(0, new CustomerStaticStatusModel { ItemId = 0, ItemNameDisplay = "All" });
           
            ViewBag.AccountStatus = _associationService.GetAccountStaticStatuses();
            ViewBag.Subscribers = _subscriberService.GetSubscribers();
            ViewBag.Evidences = _associationService.GetEvidenceList();
            ViewBag.AssociationStatus = getAssociationStatus;
            ViewBag.AssociationTypes = _associationService.GetCustomerStaticStatus(2, true);

            return View(isHostPermission);
        }

        [HttpPost]
        public ActionResult GetAssociations(AssociationFilterViewModel filter)
        {
            var graphAssociations = _associationService.GetAssociations(ConfigSettings.GetAssociations, filter);
            var associationList = Enumerable.Empty<AssociationViewModel>();

            if (graphAssociations != null && graphAssociations.Any())
            {
                var ctsCustIdList = string.Join(",", graphAssociations.Select(x => x.CTSCustId).Distinct());

                var categories = _associationService.GetCategories(ctsCustIdList);

                associationList = graphAssociations.Select(x =>
                {
                    var category = categories.FirstOrDefault(y=>y.CTSCustID == x.CTSCustId);

                    return new AssociationViewModel
                    {
                        CTSCustId = x.CTSCustId,
                        SiteName = x.SiteName,
                        UserName = x.UserName,
                        EvidenceCode = x.EvidenceCode,
                        AccountStatus = x.AccountStatus,
                        CategoryNames = category != null ? category.CategoryList : string.Empty
                    };
                });
            }

            return Json( new
            {
                data = associationList
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Detail(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAssociationDetail(AssociationDetailFilterViewModel filter)
        {
            var result = _associationService.GetAssociationDetail(filter);

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
