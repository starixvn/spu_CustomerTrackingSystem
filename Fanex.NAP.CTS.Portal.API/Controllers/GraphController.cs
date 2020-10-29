namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Model.GraphDB;
    using Fanex.NAP.SPU.Graph.Core;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Results;

    /// <summary>
    /// Graph Controller - Using for CTS site
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
        /// Get all association one level
        /// </summary>
        /// <param name="ctsCustId"></param>
        /// <param name="assStatus"></param>
        /// <param name="takeRow"></param>
        /// <param name="skipRow"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetDirectAssociation")]
        [HttpGet]
        public GraphAssociationPaging GetDirectAssociation(int ctsCustId, int assStatus, int takeRow, int skipRow)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphAssociationPaging>("GetAssociationOneLevelByCustId", new
                {
                    CTSCustID = ctsCustId,
                    AssStatus = assStatus,
                    Skip = skipRow,
                    Pagesize = takeRow
                }).Data.FirstOrDefault();

                if (result == null)
                {
                    return new GraphAssociationPaging() { AssociationList = null, TotalItems = 0 };
                }

                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociation failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }


        [System.Web.Http.AcceptVerbs("GetDirectAssociationByManual")]
        [HttpGet]
        public GraphAssociationPaging GetDirectAssociationByManual(int ctsCustId, int assStatus, int takeRow, int skipRow)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphAssociationPaging>("GetAssociationOneLevelByCustIdByManual", new
                {
                    CTSCustID = ctsCustId,
                    AssStatus = assStatus,
                    Skip = skipRow,
                    Pagesize = takeRow
                }).Data.FirstOrDefault();

                return result ?? new GraphAssociationPaging() { AssociationList = null, TotalItems = 0 };
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationByManual failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [System.Web.Http.AcceptVerbs("GetDirectAssociationByDevice")]
        [HttpGet]
        public GraphAssociationPaging GetDirectAssociationByDevice(int ctsCustId, int assStatus, int takeRow, int skipRow)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphAssociationPaging>("GetAssociationOneLevelByCustIdByDevice", new
                {
                    CTSCustID = ctsCustId,
                    AssStatus = assStatus,
                    Skip = skipRow,
                    Pagesize = takeRow
                }).Data.FirstOrDefault();

                if (result == null)
                {
                    return new GraphAssociationPaging() { AssociationList = null, TotalItems = 0 };
                }

                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationByDevice failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }

        [System.Web.Http.AcceptVerbs("GetDirectAssociationByIP")]
        [HttpGet]
        public GraphAssociationPaging GetDirectAssociationByIP(int ctsCustId, int assStatus, int takeRow, int skipRow)
        {
            try
            {
                var result = DbSessionFactory.Execute<GraphAssociationPaging>("GetAssociationOneLevelByCustIdByIP", new
                {
                    CTSCustID = ctsCustId,
                    AssStatus = assStatus,
                    Skip = skipRow,
                    Pagesize = takeRow
                }).Data.FirstOrDefault();

                if (result == null)
                {
                    return new GraphAssociationPaging() { AssociationList = null, TotalItems = 0 };
                }

                return result;
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

        /// <summary>
        /// Grouping customer
        /// </summary>
        /// <param name="custIds">List custId</param>
        /// <returns></returns>
        [HttpPost]
        public IEnumerable<CustomerGroup> GroupingCustomer([FromBody] IEnumerable<int> custIds)
        {
            IEnumerable<CustomerGroup> result = null;
            try
            {
                if (custIds.Any())
                {
                    result = DbSessionFactory.Execute<CustomerGroup>("GroupCustomerByAssociationOneLevel", new { CustIds = custIds.Distinct() }).Data;
                }
            }
            catch (Exception ex)
            {
                LogService.Error("GroupingCustomer failed: " + ex.Message + ex.StackTrace);
            }

            return result;
        }


        [System.Web.Http.AcceptVerbs("GetDirectAssociationDevices")]
        [HttpGet]
        public DirectAssociationDeviceModel GetDirectAssociationDevices(long custId)
        {
            try
            {
               var result = new DirectAssociationDeviceModel();
               result = DbSessionFactory.Execute<DirectAssociationDeviceModel>("GetDevicesIdByCustId", new { ip_CustID = custId }).Data.SingleOrDefault();
               result.DirectAssociation = DbSessionFactory.Execute<DirectAssociationModel>("GetAssociatedDevicesByCustId", new { ip_CustID = custId }).Data.ToList();
               result.TotalDirectAssociation = result.DirectAssociation.Count();

               return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetDirectAssociationDevices failed: " + ex.Message + ex.StackTrace);

            }
        }

        /// <summary>
        /// Get association report
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("GetAssociationReport")]
        [HttpGet]
        public IEnumerable<AssociationReportModel> GetAssociationReport([FromBody] AssociationReportParameterModel paras)
        {
            try
            {
                var result = DbSessionFactory.Execute<AssociationReportModel>("GetAssociationReport", new
                {
                   ip_AssCreatedFrom = paras.AssociatedDateFrom,
                   ip_AssCreatedTo = paras.AssociatedDateTo,
                   ip_AccountStatus = paras.AccountStatus,
                   ip_EvidenceCode = paras.EvidenceCode,
                   ip_SiteName = paras.SiteName,
                   ip_AssStatus = paras.AssociationStatus,
                   ip_AssType = paras.AssociationType,
                   ip_TakeRow = paras.TakeRows,
                   ip_SkipRow = paras.SkipRows
                }).Data;

                return result;
            }
            catch (Exception ex)
            {
                LogService.Error("GetAssociationReport failed: " + ex.Message + ex.StackTrace);
                throw;
            }
        }
    }
}
