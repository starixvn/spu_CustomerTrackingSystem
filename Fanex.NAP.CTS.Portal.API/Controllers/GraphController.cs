using Fanex.NAP.CTS.Portal.API.Model;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;

namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    public class GraphController : ApiController
    {
        /// <summary>
        /// Get Association By Account
        /// </summary>
        /// <param name="custId"></param>
        /// <returns>GraphCustomerModel</returns>
    
        public IHttpActionResult GetAssociationByAccount(int custId)
        {
            var result = new GraphCustomerModel { CustId = 0, Username = "test", Username2 = "abc", RoleId = 1 };

            return Json(new { result = result });
        }

        /// <summary>
        /// Get Association One Level
        /// </summary>
        /// <param name="custId"></param>
        /// <returns>List of GraphCustomerModel</returns>      
        public IEnumerable<GraphCustomerModel> GetAssociationOneLevel(int custId)
        {
            var result = new List<GraphCustomerModel>();

            result.Add(new GraphCustomerModel { CustId = 1, Username = "test", Username2 = "abc", RoleId = 1 });
            result.Add(new GraphCustomerModel { CustId = 2, Username = "testzzz", Username2 = "abczzz", RoleId = 1 });

            return result;
        }

        public string aaa()
        {
            return "";
        }
    }
}
