using Fanex.Data;
using Fanex.NAP.CTS.Criteria.AssociationDetection;
using Fanex.NAP.CTS.Model.AssociationDetection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.CTS.Service.Implement
{
    public interface IAssociationDetectionService
    {
        IEnumerable<GetAssociationDetectionModel> GetAssociationDetection(string ctsCustIds);

    }
    public class AssociationDetectionService : IAssociationDetectionService
    {
        public IEnumerable<GetAssociationDetectionModel> GetAssociationDetection(string ctsCustIds)
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
    }
}
