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

namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    public class CtsController : ApiController
    {
        private readonly IAccountService _accountService;

        public CtsController()
        {
            _accountService = new AccountService();
        }


        [HttpPost]
        public bool PlaceEvidence(UpdatePlaceEvidenceModel model)
        {
            try
            {
                //var result =_accountService.PlaceEvidence(model);
                _accountService.FPSAddNewEvidence(ConfigSettings.AddNewEvidence, model);


                //if (result.ErrorCode == 0)
                //{
                //    _accountService.FPSAddNewEvidence(ConfigSettings.AddNewEvidence, model);                    
                //}

                return true;
            }
            catch (Exception ex)
            {
                LogService.Error("PlaceEvidence exception: " + ex.Message);
                return false;
            }
        }

        [HttpGet]
        public string Test()
        {
            return "aa";
        }
    }
}
