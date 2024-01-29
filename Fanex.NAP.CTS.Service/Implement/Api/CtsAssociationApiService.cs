namespace Fanex.NAP.CTS.Service.Implement.Api
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.Api.CtsAssociationAPI;
    using Fanex.NAP.CTS.Model;
    using Fanex.NAP.CTS.Model.API;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CtsAssociationApiService : ICtsAssociationApiService
    {
        private readonly IDynamicRepository repository;

        public CtsAssociationApiService()
        {
            repository = new DynamicRepository();
        }

        #region Get Association

        #region Public Methods

        public IEnumerable<DetectAssociationByTypeModel> DetectAssociationByCustId(long custID)
        {
            if (custID <= 0)
            {
                return Enumerable.Empty<DetectAssociationByTypeModel>();
            }

            var assLists = new DetectAssociationByTypeViewModel();

            Parallel.Invoke(() =>
            {
                assLists.AsssociationByDevice = GetAssociationByType(new DetectAssociationByTypeFilterModel(custID, AssociationDetectionType.Device));
            }, () =>
            {
                assLists.AsssociationIP = GetAssociationByType(new DetectAssociationByTypeFilterModel(custID, AssociationDetectionType.IP));
            }, () =>
            {
                assLists.AsssociationAI = GetAssociationByType(new DetectAssociationByTypeFilterModel(custID, AssociationDetectionType.AI));
            });

            return assLists.GetAssociation();
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<DetectAssociationByTypeModel> GetAssociationByType(DetectAssociationByTypeFilterModel custAssType)
        {
            var criteria = new DetectAssociationByTypeCriteria
            {
                CustID = custAssType.CustID,
                IsDevice = custAssType.IsDevice,
                IsIP = custAssType.IsIP,
                IsAI = custAssType.IsAI
            };

            return repository.Fetch<DetectAssociationByTypeModel>(criteria);
        }

        #endregion Private Methods

        #endregion Get Association
    }
}