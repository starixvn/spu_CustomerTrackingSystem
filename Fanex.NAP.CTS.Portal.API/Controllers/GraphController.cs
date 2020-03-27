using Fanex.Logging;
using Fanex.NAP.CTS.Model.Account;
using Fanex.NAP.CTS.Model.GraphDB;
using Fanex.NAP.SPU.Graph.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Linq;
using Newtonsoft.Json;

namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    /// <summary>
    /// Graph Controller
    /// </summary>
    public class GraphController : ApiController
    {
        /// <summary>
        /// Get association statistic
        /// </summary>
        /// <param name="ctsCustId"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetStatisticAssociation")]
        [HttpGet]
        public CustomerAssociationByCTSCustIdModel GetStatisticAssociation(int ctsCustId)
        {
            try
            {
                var result = DbSessionFactory.ExecuteSingle<CustomerAssociationByCTSCustIdModel>("GetStatisticForCustomer", new { CTSCustID = ctsCustId }).Data;
                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetStatisticAssociation failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Get Association with one Level
        /// </summary>
        /// <param name="ctsCustId"></param>
        /// <returns></returns>
        /// 
        [System.Web.Http.AcceptVerbs("GetDirectAssociation")]
        [HttpGet]
        public GraphAssociationPaging GetDirectAssociation(int ctsCustId, int takeRow, int skipRow)
        {
            try
            {
                var result = DbSessionFactory.ExecuteSingle<GraphAssociationPaging>("GetAssociationOneLevelByCustId", new
                {
                    CTSCustID = ctsCustId,
                    Skip = skipRow,
                    Pagesize = takeRow
                }).Data;
                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociation failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [System.Web.Http.AcceptVerbs("GetDirectAssociationByDevice")]
        [HttpGet]
        public IHttpActionResult GetDirectAssociationByDevice(int ctsCustId, int takeRow, int skipRow)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphAssociationModel>("GetAssociationOneLevelByCustIdByDevice", new
                {
                    CTSCustID = ctsCustId,
                    Skip = skipRow,
                    Pagesize = takeRow
                }).Data;

                return Ok(result);
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationByDevice failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [System.Web.Http.AcceptVerbs("GetDirectAssociationByIP")]
        [HttpGet]
        public IHttpActionResult GetDirectAssociationByIP(int ctsCustId, int takeRow, int skipRow)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphAssociationModel>("GetAssociationOneLevelByCustIdByIP", new
                {
                    CTSCustID = ctsCustId,
                    Skip = skipRow,
                    Pagesize = takeRow
                }).Data;

                return Ok(result);
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociation failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [System.Web.Http.AcceptVerbs("GetDirectAssociationMultiCust")]
        [HttpPost]
        public IEnumerable<GraphDirectAssociationModel> GetDirectAssociationMultiCust([FromBody] List<long> ctsCustIds)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphDirectAssociationModel>("GetAssociationOneLevelByCustIds", new { CTSCustIDs = ctsCustIds }).Data;
                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationMultiCust failed: " + ex.Message + ex.StackTrace);
                throw;
            }
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
            try
            {
                var result = DbSessionFactory.Execute<GraphAssociationModel>("GetAssociationNLevelByCustId", new { CTSCustID = ctsCustId }).Data;
                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetInDirectAssociation failed: " + ex.Message + ex.StackTrace);
                throw;
            }
            
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
        public GraphAssociationPaging TestGetDirectAssociation(int ctsCustId, int takeRow, int skipRow)
        {
            try
            {
                var result = DbSessionFactory.ExecuteSingle<GraphAssociationPaging>("GetAssociationOneLevelByCustId", new
                {
                    CTSCustID = ctsCustId,
                    Skip = skipRow,
                    Pagesize = takeRow
                }).Data;
                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociation failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [System.Web.Http.AcceptVerbs("TestGetInDirectAssociation")]
        [HttpGet]
        public IEnumerable<GraphAssociationModel> TestGetInDirectAssociation(int ctsCustId)
        {
            var result = DbSessionFactory.Execute<GraphAssociationModel>("GetAssociationNLevelByCustId", new { CTSCustID = ctsCustId }).Data;
            return result;
        }

        [System.Web.Http.AcceptVerbs("TestGetDirectAssociationMultiCust")]
        [HttpGet]
        public IEnumerable<GraphDirectAssociationModel> TestGetDirectAssociationMultiCust(string ctsCustIds)
        {
            var param = ctsCustIds.Split(',').Select(a => int.Parse(a));
            try
            {
                var result = DbSessionFactory.Execute<GraphDirectAssociationModel>("GetAssociationOneLevelByCustIds", new { CTSCustIDs = param }).Data;
                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationMultiCust failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }
    }
}
