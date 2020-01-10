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
        /// <param name="custId"></param>
        /// <returns></returns>
        /// 
        [System.Web.Http.AcceptVerbs("GetDirectAssociation")]
        [HttpPost]
        public IEnumerable<GraphAssociationModel> GetDirectAssociation([FromBody] int custId)
        {
            // var result = DbSessionFactory.Execute<GraphAssociationModel>("GetAssociationOneLevelByCustId", new { CustId = custId }).Data;
            var result = new List<GraphAssociationModel>()
            {
                new GraphAssociationModel()
                {
                    CustId = 28128919,
                    Username = "harvey02424",
                    Username2 = "harvey.nguyen",
                    AssociationType = 1,
                    AssociationDate = DateTime.Now,
                    SharedDevices = 3,
                    SharedIPs = 2
                },
                new GraphAssociationModel()
                {
                    CustId = 28128919,
                    Username = "harvey02424",
                    Username2 = "harvey.nguyen",
                    AssociationType = 2,
                    AssociationDate = DateTime.Now,
                    SharedDevices = 3,
                    SharedIPs = 2
                },
                new GraphAssociationModel()
                {
                    CustId = 27833824,
                    Username = "terry",
                    Username2 = "terry.nguyen",
                    AssociationType = 2,
                    AssociationDate = DateTime.Now,
                    SharedDevices = 12,
                    SharedIPs = 8
                }
            };

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
                    CustId = 28128919,
                    Username = "harvey02424",
                    Username2 = "harvey.nguyen",
                    AssociationType = 1,
                    AssociationDate = DateTime.Now,
                    SharedDevices = 3,
                    SharedIPs = 2
                },
                new GraphAssociationModel()
                {
                    CustId = 28128919,
                    Username = "harvey02424",
                    Username2 = "harvey.nguyen",
                    AssociationType = 2,
                    AssociationDate = DateTime.Now,
                    SharedDevices = 3,
                    SharedIPs = 2
                },
                new GraphAssociationModel()
                {
                    CustId = 27833824,
                    Username = "terry",
                    Username2 = "terry.nguyen",
                    AssociationType = 2,
                    AssociationDate = DateTime.Now,
                    SharedDevices = 12,
                    SharedIPs = 8
                }
            };
            return result;
        }

        /// <summary>
        /// Get shortest path between 2 customers
        /// </summary>
        /// <param name="custIdFrom"></param>
        /// <param name="custIdTo"></param>
        /// <returns>Json string</returns>
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
