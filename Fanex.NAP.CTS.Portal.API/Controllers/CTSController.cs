namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using Fanex.NAP.SPU.Graph.Core;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Results;

    public class CtsController : ApiController
    {
        private readonly IAccountService _accountService;
        private readonly IEvidenceService _evidenceService;
        private readonly ICustomerEvidenceService _custEvidenceService;
        private readonly IAssociatedAccountService _associationService;
        private readonly ISyncCustomerToMainDbService _syncToMainDbService;
        private readonly IProblemAccountMonitorService _problemAccountMonitorService;

        /// <summary>
        /// Using for external site (TW, ExtAcc site...)
        /// </summary>
        public CtsController()
        {
            _accountService = new AccountService();
            _evidenceService = new EvidenceService();
            _associationService = new AssociatedAccountService();
            _custEvidenceService = new CustomerEvidenceService();
            _syncToMainDbService = new SyncCustomerToMainDbService();
            _problemAccountMonitorService = new ProblemAccountMonitorService();
        }

        /// <summary>
        /// Place Evidence on CTS
        /// </summary>
        /// <param name="evidence"></param>
        /// <returns></returns>
        [HttpPost]
        public bool PlaceEvidence(UpdatePlaceEvidenceModel evidence)
        {
            try
            {
                _accountService.ProcessPlaceEvidenceByCustId(evidence);

                return true;
            }
            catch (Exception ex)
            {
                LogService.Error("PlaceEvidence exception: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Place evidence for multiple customer
        /// </summary>
        /// <param name="custIds">List custId in string format, separated by comma</param>
        /// <param name="evidenceId">Evidence Id in CTS system</param>
        /// <returns>true: success; false: failed</returns>
        [HttpPost]
        public bool PlaceEvidenceMultipleByCustId(string custIds, int evidenceId)
        {
            try
            {
                int userId = Convert.ToInt32(ConfigurationManager.AppSettings["StarixITUserId"]);
                var result = _custEvidenceService.AddCustEvidenceMultipleByCustId(custIds, evidenceId, userId);

                if (string.IsNullOrEmpty(result))
                {
                    _syncToMainDbService.SyncCustomerClassToMainDb(custIds, true, false);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                LogService.Error("PlaceEvidence exception: " + ex.Message);
                return false;
            }
        }

        /// <summary>   
        /// Get flagged customers by level
        /// </summary>
        /// <param name="CustIdList">CustId in string format (saparate by comma) </param>
        /// <param name="AssociatedLevel"> Level 0, 1 or 2</param>
        /// <returns>A list of customer evidence in JSON format</returns>
        [HttpPost]
        public JsonResult<List<CustomerEvidence>> GetFlaggedCustomersByLevel(string CustIdList, int AssociatedLevel)
        {
            try
            {
                int[] eviList = { -1 }; // set default for getting all 
                var convertedCustIdList = CustIdList.Split(',').Select(a => Convert.ToInt32(a));
                var associationList = _associationService.GetFlaggedCustomerByLevel(convertedCustIdList, AssociatedLevel, eviList);
                var result = associationList.Distinct(new CustomerEvidenceComparer());

                return Json(result.ToList());
            }
            catch (Exception ex)
            {
                LogService.Error("Get Flagged Customers By Level exception: " + ex.Message);
                return Json(new List<CustomerEvidence>());
            }
        }

        /// <summary>   
        /// Get flagged customers by level and evidence
        /// </summary>
        /// <param name="custIdList">CustId in string format (saparate by comma) </param>
        /// <param name="associatedLevel"> Level 0, 1 or 2</param>
        /// <param name="eviIdList"> Evidence in string format (saparate by comma) (</param>
        /// <returns>A list of customer evidence in JSON format</returns>
        [HttpGet]
        public JsonResult<List<CustomerEvidence>> GetFlaggedCustByLvlNEvi(string custIdList, int associatedLevel, string eviIdList)
        {
            try
            {
                var convertedEviList = eviIdList.Split(',').Select(a => Convert.ToInt32(a));
                var convertedCustIdList = custIdList.Split(',').Select(a => Convert.ToInt32(a));
                var associationList = _associationService.GetFlaggedCustomerByLevel(convertedCustIdList, associatedLevel, convertedEviList);
                var result = associationList.Distinct(new CustomerEvidenceComparer());

                return Json(result.ToList());
            }
            catch (Exception ex)
            {
                LogService.Error("Get Flagged Customers By Level exception: " + ex.Message);
                return Json(new List<CustomerEvidence>());
            }
        }

        /// <summary>
        /// Add evidences to customers
        /// </summary>
        /// <param name="customerEvidenceList"></param>
        /// <returns></returns>
        [HttpPost]
        public bool AddCustomersEvidences(CustomerEvidenceList customerEvidenceList)
        {
            try
            {
                var result = _evidenceService.AddCustomersEvidences(customerEvidenceList);

                if (string.IsNullOrEmpty(result))
                {
                    _syncToMainDbService.SyncCustomerClassToMainDb(customerEvidenceList.CustIdList, true, false);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                LogService.Error("Add Customers Evidences exception: " + ex.Message);
                return false;
            }
        }

        [AcceptVerbs("GetAssociationForExtAcc")]
        [HttpGet]
        public string GetAssociationForExtAcc(int custId)
        {
            var xmlString = string.Empty;
            try
            {
                var cust = DbSessionFactory.Execute<GraphCustomer>("GetCustomerByCustId", new
                {
                    CustID = custId,
                    CustSubID = 0
                }).Data.FirstOrDefault();

                if (cust != null)
                {
                    var result = DbSessionFactory.Execute<GraphAssociationPaging>("GetAssociationOneLevelByCustIdByDevice", new
                    {
                        CTSCustID = cust.CTSCustId,
                        AssStatus = 1,
                        Skip = 0,
                        Pagesize = 100000
                    }).Data.FirstOrDefault();

                    if (result != null)
                    {
                        var associationList = result.AssociationList.Select(a => new AssociationForExtAcc
                        {
                            CustId = a.CustId == null ? 0 : a.CustId.Value,
                            CustSubId = a.CustSubId,
                            AssociationDate = a.AssociationDate.ToString()
                        });

                        xmlString = XmlHelper.ToXmlString(associationList);
                    }
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
        /// Get association by custId and level
        /// </summary>
        /// <param name="custId"></param>
        /// <param name="level">Level 1 for direct, level 2 for indirect</param>
        /// <returns>Association list in XML format</returns>
        [HttpGet]
        public string GetAssociationByCustId(int custId, int level)
        {
            var result = string.Empty;
            try
            {
                var spName = "GetAssociationByCustIdLevel" + level;
                result = DbSessionFactory.Execute<Association>(spName, new
                {
                    CustId = custId
                }).Data.ToXmlString();
            }
            catch (Exception ex)
            {
                LogService.Error("GetAssociationByCustId failed: " + ex.Message + ex.StackTrace);
            }

            return result;
        }

        /// <summary>
        /// Get classification of customer
        /// </summary>
        /// <param name="custIdList"></param>
        /// <returns>Customer classification in XML format</returns>
        [HttpGet]
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
                LogService.Error("GetCustomerClassification failed: " + ex.Message + ex.StackTrace);
            }

            return result;
        }

        /// <summary>
        /// Get classification of customer by Category's IDs
        /// </summary>
        /// <param name="categoryIDList"></param>
        /// <returns>Customer classification in XML format</returns>
        [HttpGet]
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
        public string GetVVIPCustomer()
        {
            try
            {
                var custClassifications = _accountService.GetCustomerClassificationByCategory("3");
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
        public string CheckCustomerNotExisted(string custIdList, DateTime fromDate, DateTime toDate)
        {
            var result = string.Empty;
            try
            {
                var customers = _accountService.CheckCustomerNotExisted(custIdList, fromDate, toDate);
                result = string.Join(",", customers.Select(c => c.CustID));
            }
            catch (Exception ex)
            {
                LogService.Error("CheckCustomerNotExisted failed: " + ex.Message + ex.StackTrace);
            }

            return result;
        }



        /// <summary>
        /// This controller is only for testing
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public void Test()
        {
            var ctsClient = new WebClient();
            ctsClient.QueryString.Add("CustId", "43913195");
            var e = ctsClient.DownloadString("http://ha.nexdev.net:7723/api/cts/GetAssociationForExtAcc");

            var a = ApiHelper.Get<string>("http://ha.nexdev.net:7723/api/cts/GetAssociationForExtAcc?CustId=43913195").Result;
        }

        /// <summary>
        ///  Update Removed Problem Account into CTS
        /// </summary>
        /// <param name="custIds">List custId in string format, separated by comma</param>
        /// <returns>true: success; false: failed</returns>
        [HttpPost]
        public bool RemoveProblemAccounts([FromBody] string custIds)
        {
            try
            {              
                _problemAccountMonitorService.UpdateRemovedProblemAccountToCTS(custIds);

                return true;
            }
            catch (Exception ex)
            {
                LogService.Error("CTS Portal API - Remove Problem Account API - exception: " + ex.Message, ex);

                return false;
            }
        }
    }
}
