namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Core;
    using Fanex.NAP.CTS.Criteria.MonitorFunction;
    using Fanex.NAP.CTS.Service.Interface;
    using static Fanex.NAP.CTS.Resource.Consts.Consts;

    public class MonitorCtsFunctionService : IMonitorCtsFunctionService
    {
        private readonly IDynamicRepository repository;

        public MonitorCtsFunctionService()
        {
            repository = new DynamicRepository();
        }

        public DBResult StatsAccessmentInsert(SystemId systemId, MonitorCtsFunctionId funcId, int userId)
        {
            var criteria = new StatsAccessmentInsertCriteria()
            {
                SystemId = (long) systemId,
                FunctionId = (int) funcId,
                UserId = userId
            };

            repository.Execute(criteria);

            return new DBResult
            {
                ErrorCode = string.IsNullOrEmpty(criteria.ErrorMessage) ? 0 : 1,
                ErrorMsg = criteria.ErrorMessage
            };
        }
    }
}