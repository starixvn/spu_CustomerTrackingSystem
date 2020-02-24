using Fanex.NAP.CTS.Model.Account;
using Fanex.NAP.CTS.Model.GraphDB;
using Fanex.NAP.SPU.Graph.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    /// <summary>
    /// Graph Controller
    /// </summary>
    public class GraphController : ApiController
    {
        /// <summary>
        /// Get Association with one Level
        /// </summary>
        /// <param name="ctsCustId"></param>
        /// <returns></returns>
        /// 
        [System.Web.Http.AcceptVerbs("GetDirectAssociation")]
        [HttpPost]
        public IEnumerable<GraphAssociationModel> GetDirectAssociation([FromBody] int ctsCustId)
        {
            var result = DbSessionFactory.Execute<GraphAssociationModel>("GetAssociationOneLevelByCustId", new { CTSCustID = ctsCustId }).Data;
            return result;
        }

        [System.Web.Http.AcceptVerbs("GetDirectAssociationMultiCust")]
        [HttpPost]
        public IEnumerable<GraphAssociationModel> GetDirectAssociationMultiCust([FromBody] List<int> ctsCustIds)
        {
            var result = DbSessionFactory.Execute<GraphAssociationModel>("GetAssociationOneLevelByCustIds", new { CTSCustIDs = ctsCustIds }).Data;
            return result;
        }

        /// <summary>
        /// Get Association with N Level
        /// </summary>
        /// <param name="ctsCustId"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetInDirectAssociation")]
        [HttpPost]
        public IEnumerable<GraphAssociationModel> GetInDirectAssociation([FromBody] int ctsCustId)
        {
            var result = DbSessionFactory.Execute<GraphAssociationModel>("GetAssociationNLevelByCustId", new { CTSCustID = ctsCustId }).Data;
            return result;
        }

        /// <summary>
        /// Get shortest path between 2 customers
        /// </summary>
        /// <param name="ctsCustIdFrom"></param>
        /// <param name="ctsCustIdTo"></param>
        /// <returns>Json string</returns>
        [System.Web.Http.AcceptVerbs("GetShortestPath")]
        [HttpGet]
        public JsonResult<ShortestPathOfAccount> GetShortestPath(int ctsCustIdFrom, int ctsCustIdTo)
        {         
            var result = DbSessionFactory.ExecuteSingle<ShortestPathOfAccount>("GetShortestPath", new { StartCustId = ctsCustIdFrom, EndCustId = ctsCustIdTo }).Data;
            return Json(result);
        }

        [System.Web.Http.AcceptVerbs("TestGetDirectAssociation")]
        [HttpGet]
        public IEnumerable<GraphAssociationModel> TestGetDirectAssociation(int ctsCustId)
        {
            var result = DbSessionFactory.Execute<GraphAssociationModel>("GetAssociationOneLevelByCustId", new { CTSCustID = ctsCustId }).Data;
            return result;
        }

        [System.Web.Http.AcceptVerbs("TestGetInDirectAssociation")]
        [HttpGet]
        public IEnumerable<GraphAssociationModel> TestGetInDirectAssociation(int ctsCustId)
        {
            var result = DbSessionFactory.Execute<GraphAssociationModel>("GetAssociationNLevelByCustId", new { CTSCustID = ctsCustId }).Data;
            return result;
        }
    }
}
