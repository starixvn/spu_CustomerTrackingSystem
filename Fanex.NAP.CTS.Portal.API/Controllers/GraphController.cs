using Fanex.NAP.CTS.Portal.API.Model;
using System.Collections.Generic;
using System.Web.Http;

namespace Fanex.NAP.CTS.Portal.API.Controllers
{
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
        public IEnumerable<GraphCustomerModel> GetAssociationOneLevel(int custId)
        {
            var result = new List<GraphCustomerModel>();

            result.Add(new GraphCustomerModel { CustId = 1, Username = "test", Username2 = "abc", RoleId = 1 });
            result.Add(new GraphCustomerModel { CustId = 2, Username = "testzzz", Username2 = "abczzz", RoleId = 1 });

            return result;
        }

        /// <summary>
        /// Get Association with N Level
        /// </summary>
        /// <param name="custId"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetAssociationNLevel")]
        [HttpPost]
        public IEnumerable<GraphCustomerModel> GetAssociationNLevel(int custId)
        {
            var result = new List<GraphCustomerModel>();

            result.Add(new GraphCustomerModel { CustId = 1, Username = "test", Username2 = "abc", RoleId = 1 });
            result.Add(new GraphCustomerModel { CustId = 2, Username = "testzzz", Username2 = "abczzz", RoleId = 1 });

            return result;
        }
    }
}
