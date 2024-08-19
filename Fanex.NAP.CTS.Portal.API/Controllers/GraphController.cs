namespace Fanex.NAP.CTS.Portal.API.Controllers
{
    using Fanex.Logging;
    using Fanex.NAP.CTS.Model.DTO;
    using Fanex.NAP.CTS.Portal.API.ActionFilter;
    using Fanex.NAP.CTS.Service;
    using Fanex.NAP.CTS.Service.Implement;
    using Fanex.NAP.CTS.Service.Interface;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Results;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    /// <summary>
    /// Graph Controller - Using for CTS site
    /// </summary>
    public class GraphController : ApiController
    {
        private readonly IAssociatedAccountService associationService;
        private readonly IAssociationDetectionService associationDetectionService;
        private readonly IAssociationReportService associationReportService;

        /// <summary>
        /// Constructor
        /// </summary>
        public GraphController()
        {
            associationService = new AssociatedAccountService();
            associationDetectionService = new AssociationDetectionService();
            associationReportService = new AssociationReportService();
        }

        #region API - Flagged Evidence Level

        /// <summary>
        /// API Get GetFlaggedEvidenceForLevel01 For Taiwan
        /// </summary>
        /// <param name="custIdList"></param>
        /// <returns></returns>
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetFlaggedEvidenceForLevel01API)]
        public JsonResult<List<CustomerEvidenceForTW>> GetFlaggedEvidenceForLevel01(string custIdList)
        {
            var result = new List<CustomerEvidenceForTW>();

            try
            {
                if (!string.IsNullOrEmpty(custIdList))
                {
                    result = associationService.GetFlaggedEvidenceByDirectAssForAPI(custIdList)?.ToList();
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"WEB API - GetFlaggedEvidenceForLevel01 - Data input: {custIdList} - failed: " + ex.Message + ex.StackTrace);
                throw;
            }

            return Json(result ?? new List<CustomerEvidenceForTW>());
        }

        /// <summary>
        /// API Get GetFlaggedEvidenceForLevel2 For Taiwan
        /// </summary>
        /// <param name="custIdList"></param>
        /// <returns></returns>
        [HttpPost]
        [TrackingFunctionUsageFilter(SystemId.CTSAPI, TrackingCtsFunctionId.GetFlaggedEvidenceForLevel2API)]
        public JsonResult<List<CustomerEvidenceForTW>> GetFlaggedEvidenceForLevel2(string custIdList)
        {
            var result = new List<CustomerEvidenceForTW>();

            try
            {
                if (!string.IsNullOrEmpty(custIdList))
                {
                    result = associationService.GetFlaggedEvidenceByLevel2AssForAPI(custIdList)?.ToList();
                }
            }
            catch (Exception ex)
            {
                LogService.Error($"WEB API - GetFlaggedEvidenceForLevel2 - Data input: {custIdList} - failed: " + ex.Message + ex.StackTrace);
                throw;
            }

            return Json(result ?? new List<CustomerEvidenceForTW>());
        }

        #endregion API - Flagged Evidence Level
    }
}