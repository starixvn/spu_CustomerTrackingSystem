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

        [HttpGet]
        public IEnumerable<CustomerEvidence> GetFlaggedCustomersByLevel(string CustIdList, int AssociatedLevel)
        {
            try
            {
                // get association from graph
                var associationList = DbSessionFactory.Execute<CustomerEvidence>("GetAssociationByCustIdAndLevel"
                                        ,new { CustIdList, AssociatedLevel }).Data;
                var custIdList = string.Join(",",associationList.Select(c => c.CustId));

                return _evidenceService.GetFlaggedCustomersByLevel(custIdList);
            }
            catch (Exception ex)
            {
                LogService.Error("Get Flagged Customers By Level exception: " + ex.Message);
                return new List<CustomerEvidence>();
            }
        }

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
