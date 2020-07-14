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

namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    public class CtsController : ApiController
    {
        private readonly IAccountService _accountService;
        private readonly IEvidenceService _evidenceService;

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
                                        , new { CustIdList = convertedCustIdList }).Data;
                var associationByManualList = DbSessionFactory.Execute<CustomerEvidence>("GetAssociationByManualByCustIdAndLevel"
                                        , new { CustIdList = convertedCustIdList, AssociatedLevel }).Data;
                associationList.AddRange(associationByDeviceList);
                associationList.AddRange(associationByManualList);

                var custIdList = string.Join(",",associationList.Select(c => c.CustId).Distinct());
                LogService.Info("GetFlaggedCustomersByLevel - custIdList: " + custIdList);
                var flaggedCustomers = _evidenceService.GetFlaggedCustomersByLevel(custIdList);
                LogService.Info("GetFlaggedCustomersByLevel - flaggedCustomers: " + flaggedCustomers.Count());
                var result = (from f in flaggedCustomers
                              join a in associationList on f.CustId equals a.CustId
                              select new CustomerEvidence
                              {
                                  CustId = a.CustId,
                                  Level0CustId = a.Level0CustId,
                                  EvidenceCode = f.EvidenceCode,
                                  EvidenceName = f.EvidenceName,
                                  CreatedDate = f.CreatedDate,
                                  AssociatedLevel = AssociatedLevel
                              });

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

        /// <summary>
        /// This controller is only for testing
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Test()
        {
            return "OK";
        }
    }
}
