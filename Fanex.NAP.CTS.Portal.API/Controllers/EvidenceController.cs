namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.SPU.Graph.Core;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Results;

    /// <summary>
    /// Evidence Controller
    /// </summary>
    public class EvidenceController : ApiController
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Evidence Controller
        /// </summary>
        public EvidenceController()
        {
            _accountService = new AccountService();
        }

        /// <summary>
        /// Place Evidence
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public JsonResult<Core.DBResult> PlaceEvidence(UpdatePlaceEvidenceModel model)
        {
            var result = _accountService.PlaceEvidence(model);

            return Json(result);
        }
    }
}
