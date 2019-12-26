using Fanex.NAP.CTS.Model.Account;
using Fanex.NAP.SPU.Graph.Core;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
        [System.Web.Http.AcceptVerbs("GetAssociationOneLevel")]
        [HttpPost]
        public IEnumerable<GraphCustomerModel> GetAssociationOneLevel([FromBody] int custId)
        {
            return DbSessionFactory.Execute<GraphCustomerModel>("GetAssociationOneLevelByCustId", new { CustId = custId }).Data;
        }

        /// <summary>
        /// Get Association with N Level
        /// </summary>
        /// <param name="custId"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetAssociationNLevel")]
        [HttpPost]
        public IEnumerable<GraphCustomerModel> GetAssociationNLevel([FromBody] int custId)
        {
            return DbSessionFactory.Execute<GraphCustomerModel>("GetAssociationNLevelByCustId", new { CustId = custId }).Data;
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

            var result = DbSessionFactory.Execute<GraphCustomerModel>(query, new { CustId = custId }).Data;

            if(result == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, result);

            return response;
        }
    }
}
