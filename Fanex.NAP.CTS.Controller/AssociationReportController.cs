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

        public override string BaseControllerName { get => Consts.MenuItemReport; }        

        public AssociationReportController()
        {
            _associationService = new AssociationReportService();
        }

        public ActionResult Index()
        {         
            ViewBag.AccountStatus = _associationService.GetAccountStaticStatuses();
            ViewBag.Subscribers = GetSubscriberGroups();
            ViewBag.Evidences = _associationService.GetEvidenceList();
            ViewBag.AssociationStatus = _associationService.GetCustomerStaticStatus(3, true);
            ViewBag.AssociationTypes = _associationService.GetCustomerStaticStatus(2, true);

            return View();
        }

        private IEnumerable<SubscriberGroupViewModel> GetSubscriberGroups()
        {
            var subscriberGroups = Enumerable.Empty<SubscriberGroupViewModel>();
            var data = _associationService.GetAllSubscriberGroups();

            if (data != null)
            {
                subscriberGroups = data
                    .GroupBy(g => new { g.SubscriberGroupID, g.SubscriberGroupName })
                    .Select(x => new { Group = x.Key, SubscriberList = x.ToList() })
                    .Select(x => new SubscriberGroupViewModel
                    {
                        SubscriberGroupID = x.Group.SubscriberGroupID,
                        SubscriberGroupName = x.Group.SubscriberGroupName,
                        SubscriberList = x.SubscriberList.Select(y => new SubscriberViewModel
                        {
                            SubscriberID = y.SubscriberID,
                            SubscriberName = y.SubscriberName
                        })
                    });
            }

            return subscriberGroups;
        }

        [HttpPost]
        public ActionResult GetAssociations(AssociationFilterViewModel filter)
        {
            var associationList = Enumerable.Empty<AssociationViewModel>();
            int totalItems = 0;
            var associations = _associationService.GetAssociationFromMySql(filter, out totalItems);

            if (associations != null && associations.Any())
            {
                associationList = associations.GroupBy(x => x.UserName)
                    .Select(g => new AssociationViewModel
                    {
                        CTSCustId = g.FirstOrDefault().CTSCustId,
                        SubscriberId = g.FirstOrDefault().SubscriberId,
                        SubscriberName = g.FirstOrDefault().SubscriberName,
                        UserName = g.FirstOrDefault().UserName,
                        EvidenceCodes = string.Join(", ", g.Select(x => x.EvidenceCode).OrderBy(x => x)),
                        AccountStatus = g.FirstOrDefault().AccountStatus,
                        CategoryName = g.FirstOrDefault().CategoryName
                    });
            }

            return Json( new
            {
                draw = filter.draw,
                recordsTotal = totalItems,
                recordsFiltered = totalItems,
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
            var totalItems = 0;
            var associations = _associationService.GetAssociationDetailFromMySql(filter, out totalItems);            

            if (associations != null && associations.Any())
            {
                var associationTypes = _associationService.GetCustomerStaticStatus(2, true);

                var groupForSite = associations.GroupBy(x => x.SubscriberID)
                    .Select(g => new { SubscriberID = g.Key, TotalAccounts = g.Count() });

                associationList = associations.Select(x => new AssociationDetailViewModel
                {
                    CTSCustId = x.CTSCustID,
                    UserName = x.UserName,
                    SubscriberId = x.SubscriberID,
                    SubscriberName = x.SubscriberName,
                    AssociatedDate = x.AssociationDate,
                    AccountStatus = x.AccountStatus,
                    AssociatedDateString = x.AssociationDate.ToString(Consts.FormatDateToString_MMMddyyyyhhmmss),
                    IsLinked = x.AssociationStatus,                    
                    AssociationTypeName = GetAssociationTypeName(associationTypes, x.AssociationType.FirstOrDefault()),
                    TotalAssociationAccount = groupForSite.FirstOrDefault(y=>y.SubscriberID == x.SubscriberID).TotalAccounts
                }).OrderBy(x=>x.SubscriberId);
            }

            return Json(new
            {
                draw = filter.draw,
                recordsTotal = totalItems,
                recordsFiltered = totalItems,
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
