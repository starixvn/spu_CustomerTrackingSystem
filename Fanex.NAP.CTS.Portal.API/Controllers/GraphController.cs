using Fanex.NAP.CTS.Model.Account;
using Fanex.NAP.CTS.Model.GraphDB;
using Fanex.NAP.SPU.Graph.Core;
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
        /// <param name="custId"></param>
        /// <returns></returns>
        /// 
        [System.Web.Http.AcceptVerbs("GetDirectAssociation")]
        [HttpPost]
        public IEnumerable<GraphAssociationModel> GetDirectAssociation([FromBody] int custId)
        {
            var result = DbSessionFactory.Execute<GraphAssociationModel>("GetAssociationOneLevelByCustId", new { CustId = custId }).Data;
            return result;
        }

        /// <summary>
        /// Get Association with N Level
        /// </summary>
        /// <param name="custId"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetInDirectAssociation")]
        [HttpPost]
        public IEnumerable<GraphAssociationModel> GetInDirectAssociation([FromBody] int custId)
        {
            // return DbSessionFactory.Execute<GraphCustomerModel>("GetAssociationNLevelByCustId", new { CustId = custId }).Data;
            var result = new List<GraphAssociationModel>()
            {
                new GraphAssociationModel()
                {
                    CustId = 1,
                    Username = "harvey02424",
                    Username2 = "harvey.nguyen"
                }
            };
            return new List<GraphAssociationModel>();
        }

        /// <summary>
        /// Get association by level
        /// </summary>
        /// <param name="custId"></param>
        /// <param name="isOneLevel"></param>
        /// <returns>HttpResponseMessage</returns>
        public HttpResponseMessage Get(int custId, bool isOneLevel)
        {
            var query = isOneLevel ? "GetAssociationOneLevelByCustId" : "GetAssociationNLevelByCustId";

            var result = DbSessionFactory.Execute<GraphAssociationModel>(query, new { CustId = custId }).Data;

            if(result == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result);

            return response;
        }

        [System.Web.Http.AcceptVerbs("GetShortestPath")]
        [HttpGet]
        public JsonResult<ShortestPathOfAccount> GetShortestPath(int custIdFrom, int custIdTo)
        {
            custIdFrom = 23081993;
            custIdTo = 23081995;
            var result = DbSessionFactory.ExecuteSingle<ShortestPathOfAccount>("GetShortestPath", new { StartCustId = custIdFrom, EndCustId = custIdTo }).Data;
            return Json(result);
        }
    }
}
