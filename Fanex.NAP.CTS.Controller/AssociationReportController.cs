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
         
            ViewBag.AccountStatus = _associationService.GetAccountStaticStatuses();
            ViewBag.Subscribers = _subscriberService.GetSubscribers();
            ViewBag.Evidences = _associationService.GetEvidenceList();
            ViewBag.AssociationStatus = _associationService.GetCustomerStaticStatus(3, true);
            ViewBag.AssociationTypes = _associationService.GetCustomerStaticStatus(2, true);

            return View(isHostPermission);
        }

        [HttpPost]
        public ActionResult GetAssociations(AssociationFilterViewModel filter)
        {
            var associationList = Enumerable.Empty<AssociationViewModel>();           
            var graphAssociations = _associationService.GetAssociations(ConfigSettings.GetAssociations, filter);

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
                draw = filter.draw,
                recordsTotal = 9,
                recordsFiltered = 9,
                data = associationList               
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Detail(AssociationDetailFilterViewModel filter)
        {
            ViewBag.UserName = filter.UserName;
            ViewBag.FromDate = filter.FromDate.ToShortDateString();
            ViewBag.ToDate = filter.ToDate.ToShortDateString();
            ViewBag.AssociationType = filter.AssociationType;

            return View();
        }

        [HttpPost]
        public ActionResult GetAssociationDetail(AssociationDetailFilterViewModel filter)
        {
            var associationList = _associationService.GetAssociationDetail(filter);

            return Json(new
            {
                draw = filter.draw,
                recordsTotal = 6,
                recordsFiltered = 6,
                data = associationList,
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
