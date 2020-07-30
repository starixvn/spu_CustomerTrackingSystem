using Fanex.NAP.CTS.Model.Account;
using Fanex.NAP.CTS.Service;
using Fanex.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Fanex.NAP.CTS.Core.Utilities;
using Fanex.NAP.CTS.Model.DTO;
using Fanex.NAP.SPU.Graph.Core;
using System.Web.Http.Results;
using Fanex.NAP.CTS.Model.GraphDB;
using Fanex.NAP.CTS.Core.Helpers.Xml;
using Fanex.NAP.CTS.Core.Helpers.Api;

namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    public class CtsController : ApiController
    {
        private readonly IAccountService _accountService;
        private readonly IEvidenceService _evidenceService;

        /// <summary>
        /// Using for external site (TW, ExtAcc site...)
        /// </summary>
        public CtsController()
        {
            _accountService = new AccountService();
            _evidenceService = new EvidenceService();
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
                var convertedCustIdList = CustIdList.Split(',').Select(a => Convert.ToInt32(a));
                var associationList = new List<CustomerEvidence>();

                // get association from graph
                var associationByDeviceList = DbSessionFactory.Execute<CustomerEvidence>("GetAssociationByDeviceByCustIdLevel" + AssociatedLevel
                                        , new { CustIdList = convertedCustIdList, AssociatedLevel }).Data;
                var associationByManualList = DbSessionFactory.Execute<CustomerEvidence>("GetAssociationByManualByCustIdAndLevel" + AssociatedLevel
                                        , new { CustIdList = convertedCustIdList, AssociatedLevel }).Data;
                associationList.AddRange(associationByDeviceList);
                associationList.AddRange(associationByManualList);
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
