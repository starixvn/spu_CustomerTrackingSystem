using Fanex.Data;
using Fanex.Logging;
using Fanex.NAP.CTS.Core.Helpers.Api;
using Fanex.NAP.CTS.Core.Utilities;
using Fanex.NAP.CTS.Criteria.AssociationDetection;
using Fanex.NAP.CTS.Model.AssociationDetection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Service.Implement
{
    public interface IAssociationDetectionService
    {
        IEnumerable<GetAssociationDetectionModel> GetAssociationDetection(string ctsCustIds, bool fromMySQL = true);
    }

    public class AssociationDetectionService : IAssociationDetectionService
    {
        public IEnumerable<GetAssociationDetectionModel> GetAssociationDetection(string ctsCustIds, bool fromMySQL = true)
        {
            var result = fromMySQL ? GetAssociationDetectionFromMySQL(ctsCustIds) : GetAssociationDetectionFromGraphDB(ctsCustIds);

            return result ?? new List<GetAssociationDetectionModel>();
        }

        public IEnumerable<GetAssociationDetectionModel> GetAssociationDetectionFromMySQL(string ctsCustIds)
        {
            var criteria = new GetAssociationDetectionCriteria
            {
                CTSCustIds = ctsCustIds
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<GetAssociationDetectionModel>(criteria);
            }
        }

        public IEnumerable<GetAssociationDetectionModel> GetAssociationDetectionFromGraphDB(string ctsCustIds)
        {
            var ip_CTSCustIDs = ctsCustIds;

            var data = ApiHelper.Post<List<GetAssociationDetectionModel>>(ConfigSettings.GetAssociationDetection, ip_CTSCustIDs, 300);

            Task.WhenAll(data);

            return data.Result;
        }
    }
}
