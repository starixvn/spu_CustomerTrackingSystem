namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Data;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NAP.CTS.Core.Extentions;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.API.ProblemCaseMonitor;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Model.Subscriber;
    using Fanex.NAP.CTS.Portal.API.ActionFilter;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Implement.Api;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Results;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;
    using IProblemCaseMonitorService = Service.Interface.Api.IProblemCaseMonitorService;

    /// <summary>
    /// CtsController
    /// </summary>
    public class CtsController : ApiController
    {
        private readonly IAccountService _accountService;
        private readonly ICustomerEvidenceService _custEvidenceService;
        private readonly IAssociatedAccountService _associationService;
        private readonly IProblemAccountMonitorService _problemAccountMonitorService;
        private readonly ICategoryService _categoryService;
        private readonly IRescanCustomerClassificationService _rescanClassificationService;
        private readonly ICustomerClassificationService _custClassificationService;
        private readonly ISubscriberService _subscriberService;
        private readonly IProblemCaseMonitorService _problemCaseMonitorService;
        private readonly IProblemAccountManagementService _problemAccountManagementService;
        private readonly ILogInputDataService logInputDataService;
        private readonly IAntiFraudCenterService _antiFraudCenterService;
        private readonly ISpecialCustomerClassService _customerClassService;
        private readonly IProblemAccountClassificationService _paClassificationService;
        private readonly ICtsAssociationApiService _ctsAssociationApiService;
        private readonly ICommonService _commonService;

        /// <summary>
        /// Using for external site (TW, ExtAcc site...)
        /// </summary>
        public CtsController()
        {
            _accountService = new AccountService();
            _associationService = new AssociatedAccountService();
            _custEvidenceService = new CustomerEvidenceService();
            _problemAccountMonitorService = new ProblemAccountMonitorService();
            _categoryService = new CategoryService();
            _rescanClassificationService = new RescanCustomerClassificationService();
            _custClassificationService = new CustomerClassificationService();
            _subscriberService = new SubscriberService();
            _problemCaseMonitorService = new Service.Implement.Api.ProblemCaseMonitorService();
            logInputDataService = new LogInputDataService();
            _antiFraudCenterService = new AntiFraudCenterService();
            _customerClassService = new SpecialCustomerClassService();
            _problemAccountManagementService = new ProblemAccountManagementService();
            _paClassificationService = new ProblemAccountClassificationService();
            _ctsAssociationApiService = new CtsAssociationApiService();
            _commonService = new CommonService();
        }

        [HttpGet]
        public IHttpActionResult HealthCheck()
        {
            try
            {
                _commonService.CheckConnection();

                return Ok("ok");
            }
            catch (Exception ex)
            {
                LogService.Error("Health Check API" + ex.Message);
            }

            return InternalServerError();
        }   

        /// <summary>
        /// Get Association data from MySQL
        /// </summary>
        /// <param name="custId"></param>
        /// <returns></returns>
        [AcceptVerbs("GetAssociationForExtAcc")]
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetAssociationForExtAccAPI)]
        public string GetAssociationForExtAcc(int custId)
        {
            string xmlString = string.Empty;
            try
            {
                var result = _associationService.GetAssociationForExtAcc(custId);

                if (result != null)
                {
                    var associationList = result.Select(a => new AssociationForExtAcc
                    {
                        CustId = a.CustId == null ? 0 : a.CustId.Value,
                        CustSubId = a.CustSubId,
                        AssociationDate = a.AssociationDate.ToString()
                    });

                    xmlString = XmlHelper.ToXmlString(associationList);
                }

                return xmlString;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationByDevice failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }   

        /// <summary>
        /// Get classification of customer
        /// </summary>
        /// <param name="custIdList"></param>
        /// <returns>Customer classification in XML format</returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetCustomerClassificationAPI)]
        public string GetCustomerClassification(string custIdList)
        {
            var result = string.Empty;
            try
            {
                var custClassifications = _accountService.GetCustomerClassificationForAPI(custIdList, "50,200");
                result = custClassifications.Select(c => new CustomerClassification()
                {
                    CustID = c.CustID,
                    ParentCategoryName = c.ParentCategoryName,
                    CategoryName = c.CategoryName
                }).ToXmlString();
            }
            catch (Exception ex)
            {
                LogService.Error("GetCustomerClassification failed - Data: " + custIdList
                    + "\nException" + ex.Message + ex.StackTrace);
            }

            return result;
        }

        /// <summary>
        /// Get classification of customer by Category's IDs
        /// </summary>
        /// <param name="categoryIDList"></param>
        /// <returns>Customer classification in XML format</returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetCustomerClassificationByCategoryIDsAPI)]
        public string GetCustomerClassificationByCategoryIDs(string categoryIDList)
        {
            try
            {
                var custClassifications = _accountService.GetCustomerClassificationByCategory(categoryIDList);
                return custClassifications.Any()
                    ? custClassifications.Select(c => new CustomerClassification()
                    {
                        CustID = c.CustID,
                        ParentCategoryName = "",
                        CategoryName = c.CategoryName
                    }).ToXmlString()
                    : string.Empty;
            }
            catch (Exception ex)
            {
                LogService.Error("GetCustomerClassificationByCategoryIDs failed: " + ex.Message + ex.StackTrace);
            }

            return string.Empty;
        }

        /// <summary>
        /// Get VVIP Customers
        /// </summary>
        /// <returns>Customer classification in XML format</returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetVVIPCustomerAPI)]
        public string GetVVIPCustomer()
        {
            try
            {
                var custClassifications = _accountService.GetCustomerClassificationByCategory("210");
                return custClassifications.Any()
                    ? custClassifications.Select(c => new CustomerClassification()
                    {
                        CustID = c.CustID,
                        ParentCategoryName = "",
                        CategoryName = c.CategoryName
                    }).ToXmlString()
                    : string.Empty;
            }
            catch (Exception ex)
            {
                LogService.Error("GetCustomerClassificationByCategoryIDs failed: " + ex.Message + ex.StackTrace);
            }

            return string.Empty;
        }

        /// <summary>
        /// Get class of customer
        /// </summary>
        /// <param name="custIdList"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns>Customer class in XML format</returns>
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.CheckCustomerNotExistedAPI)]
        public IEnumerable<CTSCustomer> CheckCustomerNotExisted(string custIdList, DateTime fromDate, DateTime toDate)
        {
            try
            {
                return _accountService.CheckCustomerNotExisted(custIdList, fromDate, toDate);
            }
            catch (Exception ex)
            {
                LogService.Error("CheckCustomerNotExisted failed: " + ex.Message + ex.StackTrace);
            }

            return null;
        }

        /// <summary>
        /// Get customer class's categories, class name
        /// </summary>
        /// <returns>A list of customer class's categories in JSON, API for LAP's website and TW BI</returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetCustomerClassCategoryAPI)]
        public JsonResult<IEnumerable<CategoryCustomerClassModel>> GetCustomerClassCategory()
        {
            try
            {
                var customerClassCategories = _categoryService.GetCustomerClassCategory();

                return Json(customerClassCategories);
            }
            catch (Exception ex)
            {
                LogService.Error("Get Customer Class's Categories exception: " + ex.Message);
                return Json(Enumerable.Empty<CategoryCustomerClassModel>());
            }
        }

        /// <summary>
        /// Get Subscriber info By Subscriber name
        /// </summary>
        /// <param name="subscriberName"></param>
        /// <returns>Subscriber Info</returns>
        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetSubscriberInfoAPI)]
        public JsonResult<SubscriberInfo> GetSubscriberInfo(string subscriberName)
        {
            try
            {
                var subscriber = _subscriberService.GetSubscriberBySubscriberName(subscriberName);

                if (subscriber != null && subscriber.DCSStatus)
                {
                    return Json(subscriber);
                }
                else
                {
                    LogService.Error($"{subscriberName} is not integrated on CTS.");
                }

                return Json(new SubscriberInfo());
            }
            catch (Exception ex)
            {
                LogService.Error("Get Subscriber info exception: " + ex.Message);
                return Json(new SubscriberInfo());
            }
        }

        /// <summary>
        /// Provide API to get association (Device, IP, AI)
        /// Integration team: NAP 
        /// </summary>
        /// <param name="custId"></param>
        [HttpGet]
        [Route("api/cts/Association/GetByCustId")]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetAssociationPerTypeAPI)]
        public IEnumerable<DetectAssociationByTypeModel> GetAssociationByCustId(long custId)
        {
            try
            {
                if (custId >= 0)
                {
                    return _ctsAssociationApiService.DetectAssociationByCustId(custId);
                }
            }
            catch (Exception ex)
            {
                LogService.Error("GetAssociationByCustId exception: " + ex.Message);
            }

            return Enumerable.Empty<DetectAssociationByTypeModel>();
        }

        [HttpGet]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetProblemCaseAPI)]
        public JsonResult<List<ProblemCaseModel>> GetProblemCase()
        {
            try
            {
                var problemCases = _problemCaseMonitorService.GetProlemCase(12, true);

                return Json(problemCases.ToList());
            }
            catch (Exception ex)
            {
                var logMessage = new LogMessage(ex.Message)
                    .AddSection("Data Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex)).ToString();

                LogService.Error(logMessage);

                return Json(new List<ProblemCaseModel>());
            }
        }        
  
    }
}