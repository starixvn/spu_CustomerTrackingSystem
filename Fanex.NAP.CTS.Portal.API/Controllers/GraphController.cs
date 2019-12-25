using Fanex.NAP.CTS.Portal.API.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
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
    
        public ActionResult<GraphCustomerModel> GetAssociationByAccount(int custId)
        {
            return new GraphCustomerModel { CustId = 0, Username = "test", Username2 = "abc", RoleId = 1 };
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
    }
}
