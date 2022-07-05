namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.Model.AssociatedGroupMonitor;
    using Fanex.NAP.CTS.Service.Interface;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AssociatedGroupMonitorService : IAssociatedGroupMonitorService
    {
        private readonly IDynamicRepository repository;

        public AssociatedGroupMonitorService()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<AssociatedGroupMonitorModel> GetAssociatedGroupMonitor()
        {
            var criteria = new GetAssociatedGroupMonitorCriteria();

            return repository.Fetch<AssociatedGroupMonitorModel>(criteria);             
        }


    }
}
