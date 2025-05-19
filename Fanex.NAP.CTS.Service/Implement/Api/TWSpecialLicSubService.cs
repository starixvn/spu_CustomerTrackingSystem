namespace Fanex.NAP.CTS.Service.Implement.Api
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.Api.TWSpecialLicSub;
    using Fanex.NAP.CTS.Model.API.TWSpecialLicSub;
    using Fanex.NAP.CTS.Service.Interface.Api;
    using System.Linq;

    public class TWSpecialLicSubService : ITWSpecialLicSubService
    {
        private readonly IDynamicRepository _repository;

        public TWSpecialLicSubService()
        {
            _repository = new DynamicRepository();
        }

        public InsertTWSpecialLicSubEntity InsertTWSpecialLicSub(ForceTWLicSubCCModel model)
        {
            var criteria = new InsertTWSpecialLicSubCriteria
            {
                Custid = model.custid,
                CustomerClass = model.customerClass
            };

            return _repository.Fetch<InsertTWSpecialLicSubEntity>(criteria)?.FirstOrDefault() ?? new InsertTWSpecialLicSubEntity();
        }
    }
}
