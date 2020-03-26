using Fanex.NAP.CTS.Model.Account;
using Fanex.NAP.CTS.Service;
using Fanex.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
                _accountService.PlaceEvidence(model);
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
