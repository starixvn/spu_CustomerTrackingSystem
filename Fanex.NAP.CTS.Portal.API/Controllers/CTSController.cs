namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Core.Helpers.Api;
    using Fanex.NAP.CTS.Core.Helpers.Xml;
    using Fanex.NAP.CTS.Core.Utilities;
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
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Results;

    public class CtsController : ApiController
    {
        private readonly IAccountService _accountService;
        private readonly IEvidenceService _evidenceService;
        private readonly IAssociatedAccountService _associationService;


        /// <summary>
        /// Using for external site (TW, ExtAcc site...)
        /// </summary>
        public CtsController()
        {
            _accountService = new AccountService();
            _evidenceService = new EvidenceService();
            _associationService = new AssociatedAccountService();
        }

        [HttpPost]
        public bool PlaceEvidence(UpdatePlaceEvidenceModel model)
        {
            try
            {
                var result = _accountService.PlaceEvidence(model);


                if (result.ErrorCode == 0)
                {
                    _accountService.FPSAddNewEvidence(ConfigSettings.AddNewEvidence, model);
                }

                return true;
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
                string[] eviList = { "-1" }; // set default for getting all 
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

        //public JsonResult<string> GetAssociationForLetsTalk(string custIdList)
        //{
        //    var result = string.Empty;
        //    var eviList = ConfigurationManager.AppSettings["Environment"].ToString().Split(',');
        //    var convertedCustIdList = custIdList.Split(',').Select(a => Convert.ToInt32(a));
        //    var associationList = _associationService.GetFlaggedCustomerByLevel(convertedCustIdList, 0, eviList);
        //    var assList = associationList.Distinct(new CustomerEvidenceComparer());



        //    return Json(result);
        //}

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
                _evidenceService.AddCustomersEvidences(customerEvidenceList);

                return true;
            }
            catch (Exception ex)
            {
                LogService.Error("Add Customers Evidences exception: " + ex.Message);
                return false;
            }
        }

        [System.Web.Http.AcceptVerbs("GetAssociationForExtAcc")]
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
                var custClassifications = _accountService.GetCustomerClassificationForAPI(custIdList, 200);
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
        /// Get class of customer
        /// </summary>
        /// <param name="custIdList"></param>
        /// <returns>Customer class in XML format</returns>
        [HttpGet]
        public string GetCustomerClass(string custIdList)
        {
            var result = string.Empty;
            try
            {
                var custClasses = _accountService.GetCustomerClassByCustId(custIdList);
                result = custClasses.ToXmlString();
            }
            catch (Exception ex)
            {
                LogService.Error("GetCustomerClass failed: " + ex.Message + ex.StackTrace);
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
    }
}
