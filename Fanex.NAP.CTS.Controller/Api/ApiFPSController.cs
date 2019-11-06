namespace Fanex.NAP.CTS.Controller.Api
{
    using CTS.Criteria.Api;
    using CTS.Service;
    using Fanex.NAP.CTS.Core;
    using System.Web.Http;

    public class ApiFPSController : ApiController
    {
        private readonly IFlaggedAccountService _flaggedAccountService;

        public ApiFPSController()
        {
            _flaggedAccountService = new FlaggedAccountService();
        }

        [HttpPost]
        public DBResult AddEvidence([FromBody]FlaggedAccountInsertCriteria criteria)
        {
            criteria.EviCode = criteria.EviCode ?? string.Empty;
            criteria.IndirectAccounts = criteria.IndirectAccounts ?? string.Empty;
            criteria.DirectAccounts = criteria.DirectAccounts ?? string.Empty;
            return _flaggedAccountService.InsertAccounts(criteria);
        }

        [HttpPost]
        public DBResult RemoveEvidence([FromBody]FlaggedAccountDeleteCriteria criteria)
        {
            criteria.EviCode = criteria.EviCode ?? string.Empty;
            criteria.IndirectAccounts = criteria.IndirectAccounts ?? string.Empty;
            criteria.DirectAccounts = criteria.DirectAccounts ?? string.Empty;
            return _flaggedAccountService.DeleteAccounts(criteria);
        }
    }
}