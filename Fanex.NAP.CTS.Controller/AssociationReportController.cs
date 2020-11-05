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
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Model.Account;
    using System.Collections.Generic;

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
            ViewBag.AccountStatus = _associationService.GetAccountStaticStatuses();
            ViewBag.Subscribers = _subscriberService.GetSubscribers();
            ViewBag.Evidences = _associationService.GetEvidenceList();
            ViewBag.AssociationStatus = _associationService.GetCustomerStaticStatus(3, true);
            ViewBag.AssociationTypes = _associationService.GetCustomerStaticStatus(2, true);

            return View();
        }

        [HttpPost]
        public ActionResult GetAssociations(AssociationFilterViewModel filter)
        {
            var associationList = Enumerable.Empty<AssociationViewModel>();
            var graphAssociations = _associationService.GetAssociations(ConfigSettings.GetAssociations, filter);
            var total = 0;

            if (graphAssociations != null && graphAssociations.AssociationList != null && graphAssociations.AssociationList.Any())
            {
                total = graphAssociations.Total;
                var ctsCustIdList = string.Join(",", graphAssociations.AssociationList.Select(x => x.CTSCustId).Distinct());

                var categories = _associationService.GetCategories(ctsCustIdList);

                associationList = graphAssociations.AssociationList.Select(x =>
                {
                    var category = categories.FirstOrDefault(y=>y.CTSCustID == x.CTSCustId);

                    return new AssociationViewModel
                    {
                        CTSCustId = x.CTSCustId,
                        SubscriberId = x.SubscriberId,
                        SiteId = x.SiteId,
                        SiteName = filter.SiteName,
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
                recordsTotal = total,
                recordsFiltered = total,
                data = associationList
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Detail(AssociationDetailFilterViewModel filter)
        {
            IUserService userService = new UserService();

            ViewBag.CTSCustId = filter.CTSCustId;
            ViewBag.SubscriberId = filter.SubscriberId;            
            ViewBag.UserName = filter.UserName;
            ViewBag.FromDate = filter.FromDate.ToString(Consts.FormatDateToString_MMMddyyyy);
            ViewBag.ToDate = filter.ToDate.ToString(Consts.FormatDateToString_MMMddyyyy);
            ViewBag.AssociationStatus = filter.AssociationStatus;
            ViewBag.AssociationType = filter.AssociationType;
            ViewBag.AssociationTypeName = filter.AssociationTypeName;
            ViewBag.IsHostPermission = userService.IsHavingPermission(CurrentSession.UserId,
                            NapSettings.Settings.SiteId,
                            FSFunctionKeys.AssociationReport,
                            NAP.Core.Business.Constant.Policy.Host);

            return View();
        }

        [HttpPost]
        public ActionResult GetAssociationDetail(AssociationDetailFilterViewModel filter)
        {
            var associationList = Enumerable.Empty<AssociationDetailViewModel>();
            var graphAssociations = _associationService.GetAssociationDetail(ConfigSettings.GetAssociationDetail, filter);
            var total = 0;

            if (graphAssociations != null && graphAssociations.AssociationList != null && graphAssociations.AssociationList.Any())
            {
                total = graphAssociations.Total;
                var associationTypes = _associationService.GetCustomerStaticStatus(2, true);

                var groupForSite = graphAssociations.AssociationList.GroupBy(x => x.SiteID)
                    .Select(g => new { SiteId = g.Key, TotalAccounts = g.Count() });

                associationList = graphAssociations.AssociationList.Select(x => new AssociationDetailViewModel
                {
                    CTSCustId = x.CTSCustId,
                    UserName = x.UserName,
                    SubscriberId = x.SubscriberId,
                    SiteId = x.SiteID.HasValue ? x.SiteID.Value : 0,
                    SiteName = !string.IsNullOrEmpty(x.SiteName) ? x.SiteName : "-",
                    AccountStatus = x.StatusName,
                    AssociatedDate = x.AssociationDate,
                    AssociatedDateString = x.AssociationDate.ToString(Consts.FormatDateToString_MMMddyyyy),
                    IsLinked = x.AssociationStatus == 1,
                    AssociationTypeName = GetAssociationTypeName(associationTypes, x.AssociationType),
                    TotalAssociationAccount = groupForSite.FirstOrDefault(y=>y.SiteId == x.SiteID).TotalAccounts
                });
            }

            return Json(new
            {
                draw = filter.draw,
                recordsTotal = total,
                recordsFiltered = total,
                data = associationList,
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetAssociationTypeName(List<CustomerStaticStatusModel> associationTypes, int associaionType)
        {
            var association = associationTypes.FirstOrDefault(x => x.ItemId == associaionType);

            return association != null ? association.ItemName : string.Empty;
        }

        [HttpPost]
        [NapAuth(FunctionKey = FSFunctionKeys.AssociationReport, Policy = NAP.Core.Business.Constant.Policy.Host)]
        public ActionResult ActiveRemoveAssociation(int fromSubscriberId, long fromCTSCustId, int toSubscriberId, long toCTSCustId, bool isLinked)
        {
            if (fromSubscriberId <= 0 || fromCTSCustId <= 0 || toSubscriberId <= 0 || toCTSCustId <= 0)
            {
                return Json(new
                {
                    errorCode = -100,
                    errorMessage = "Paramater is invalid."
                }, JsonRequestBehavior.AllowGet);
            }
            
            var dbResult = _associationService.ActiveRemoveAssociation(fromSubscriberId, fromCTSCustId, toSubscriberId, toCTSCustId, CurrentSession.UserId, isLinked);

            return Json(new
            {
                errorCode = dbResult.ErrorCode,
                errorMessage = dbResult.ErrorMsg
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
