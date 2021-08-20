namespace Fanex.NAP.CTS.Service.Implement
{
    using Fanex.Data.Repository;
    using Fanex.NAP.CTS.Criteria.Account;
    using Fanex.NAP.CTS.Model.Account;
    using Fanex.NAP.CTS.Service.Interface;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RobotClassificationService : IRobotClassificationService
    {
        private readonly IDynamicRepository repository;

        public RobotClassificationService()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RobotClassification> GetLastestRobotClassification(long custID)
        {
            var criteria = new GetLastestRobotClassificationCriteria() { CustId = custID };
            return repository.Fetch<RobotClassification>(criteria);
        }
        
        public RobotClassification GetStatusRobotClassification(long custID)
        {
            var criteria = new GetLastestRobotClassificationCriteria() { CustId = custID };
            return repository.Fetch<RobotClassification>(criteria).FirstOrDefault();
        }

        public IEnumerable<RobotClassification> GetHistoryRobotClassification(long custID, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetHistoryRobotClassificationCriteria()
            {
                CustId = custID,
                FromDate = fromDate,
                ToDate = toDate
            };

            return repository.Fetch<RobotClassification>(criteria);
        }
    }
}
